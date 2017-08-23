﻿using System;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;
using System.Collections.Generic;
using Payroll.Entities.Enums;
using Payroll.Infrastructure.Implementations;
using Payroll.Repository.Models;
using System.Linq;

namespace Payroll.Service.Implementations
{
    public class AttendanceService : BaseEntityService<Attendance>, IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAttendanceLogService _attendanceLogService;
        private readonly IEmployeeHoursRepository _employeeHoursRepository;

        public AttendanceService(IUnitOfWork unitOfWork, IAttendanceRepository attendanceRepository,
            IAttendanceLogService attendanceLogService, IEmployeeHoursRepository employeeHoursRepository) : base(attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
            _unitOfWork = unitOfWork;
            _attendanceLogService = attendanceLogService;
            _employeeHoursRepository = employeeHoursRepository;
        }

        public int CreateWorkSchedule(int employeeId, AttendanceType attCode, DateTime datetime)
        {
            try
            {
                if (attCode == AttendanceType.ClockIn)
                {
                    var attendace = new Attendance()
                    {
                        ClockIn = datetime,
                        ClockOut = null,
                        EmployeeId = employeeId
                    };

                    _attendanceRepository.Add(attendace);
                }
                else if (attCode == AttendanceType.ClockOut)
                {
                    var attendance = _attendanceRepository.GetLastAttendance(employeeId);
                    attendance.ClockOut = datetime;

                    _attendanceRepository.Update(attendance);
                }

                _unitOfWork.Commit();
                return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public int CreateWorkSchedule(int employeeId, DateTime clockIn, DateTime clockOut)
        {
            try
            {
                var attendance = new Attendance()
                {
                    ClockIn = clockIn,
                    ClockOut = clockOut,
                    EmployeeId = employeeId,
                    IsManuallyEdited = false
                };

                _attendanceRepository.Add(attendance);
                _unitOfWork.Commit();

                return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public void CreateWorkSchedules()
        {
            IList<AttendanceLog> logs =
                   new List<AttendanceLog>(_attendanceLogService.GetAttendanceLogsToBeProcessed());

            CreateWorkSchedules(logs);
        }

        public int CreateWorkSchedules(IList<AttendanceLog> logs)
        {
            try
            {
                Attendance previousAttendance = null;
                foreach (var attendanceLog in logs) {
                    _attendanceLogService.Update(attendanceLog);

                    //If next employee
                    if (previousAttendance != null &&
                        attendanceLog.EmployeeId != previousAttendance.EmployeeId)
                            previousAttendance = null;

                    if (previousAttendance == null)
                        //Last record of the employee
                        previousAttendance
                            = _attendanceRepository.GetLastAttendance(attendanceLog.EmployeeId);

                    //If logout should find the last in of the employee
                        // If none we will be using first in first out rule 
                            // So it will not be recorded
                    Attendance attendance = null;
                    // For clock out
                    if (attendanceLog.Type.Equals(AttendanceType.ClockOut))
                    {
                        //If previous attendance is not null and clock out is null
                            // set the attendace clock out
                        if (previousAttendance != null &&
                                previousAttendance.ClockOut == null)
                        {
                            attendance = previousAttendance;

                            this.Update(attendance);

                            attendance.ClockOut = attendanceLog.ClockInOut;
                            attendanceLog.IsConsidered = true;
                        }
                        else
                        {
                            //Else do nothing
                                // Attendance entry should always have clock in 
                                // Double time out
                            attendanceLog.IsConsidered = false;
                        }
                       
                    }
                    else
                    {
                        //If previous attendance is null 
                            // Or it already have clockout
                                //It means we have to create new attendance
                        if (previousAttendance == null ||
                                (previousAttendance != null && previousAttendance.ClockOut != null ))
                        {
                            attendance = new Attendance()
                            {
                                ClockIn = attendanceLog.ClockInOut,
                                ClockOut = null,
                                EmployeeId = attendanceLog.EmployeeId,
                                IsManuallyEdited = false
                            };

                            attendanceLog.IsConsidered = true;

                            this.Add(attendance);
                   
                        }
                        else {
                            //Else do nothing
                                // This means it's double tap
                            attendanceLog.IsConsidered = false;
                        }
                    }

                    //Update attendance log
                    attendanceLog.IsRecorded = true;
                   
                   _unitOfWork.Commit();

                    //Set previous attendance
                    previousAttendance = attendance;

                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<Attendance> GetAttendanceByDateRange(int employeeId, DateTime fromDate, DateTime toDate)
        {
            return _attendanceRepository.GetAttendanceByDateRange(employeeId, fromDate.Date, toDate.Date);
        }

        public IList<Attendance> GetAttendanceByDate(int employeeId, DateTime date)
        {
            DateTime toDate = date.AddDays(1);
            return _attendanceRepository.GetAttendanceByDateRange(employeeId, date.Date, toDate.Date);
        }
        
        public IList<Attendance> GetAttendanceForProcessing(int employeeId, DateTime date)
        {
            DateTime toDate = date.AddDays(1);
            return _attendanceRepository.GetAttendanceByProcessing(employeeId, date.Date, toDate.Date, false);
        }

        public virtual IEnumerable<AttendanceDao> GetAttendanceAndHoursByDate(DateTime startDate, DateTime endDate, int employeeId)
        {
            endDate = endDate.AddDays(1).AddSeconds(-1);
            var attendances = _attendanceRepository.Find(a => a.IsActive && ((a.ClockIn >= startDate && a.ClockIn <= endDate) || (a.ClockOut >= startDate && a.ClockOut <= endDate)));
            var employeeHours = _employeeHoursRepository.GetAllActive();

            var query = from attendance in attendances
                        join employeeHour in employeeHours on attendance.AttendanceId equals employeeHour.OriginAttendanceId into result
                        from subResult in result.DefaultIfEmpty()
                        group subResult by attendance into grouped
                        select new AttendanceDao
                        {
                            EmployeeId =  grouped.Key.EmployeeId,
                            AttendanceId = grouped.Key.AttendanceId,
                            FirstName = grouped.Key.Employee.FirstName,
                            ClockIn = grouped.Key.ClockIn,
                            ClockOut = grouped.Key.ClockOut,
                            MiddleName = grouped.Key.Employee.MiddleName,
                            LastName = grouped.Key.Employee.LastName,
                            EmployeeHours = grouped
                        };

            if (employeeId > 0) query = query.Where(x => x.EmployeeId == employeeId);

            return query.ToList();
        }
    }
}
