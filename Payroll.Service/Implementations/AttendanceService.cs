using System;
using Payroll.Common.Enums;
using Payroll.Entities;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Service.Interfaces;
using System.Collections.Generic;
using Payroll.Entities.Enums;

namespace Payroll.Service.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAttendanceLogService _attendanceLogService;

        public AttendanceService(IUnitOfWork unitOfWork, IAttendanceRepository attendanceRepository,
            IAttendanceLogService attendanceLogService)
        {
            _attendanceRepository = attendanceRepository;
            _unitOfWork = unitOfWork;
            _attendanceLogService = attendanceLogService;
        }

        public int CreateWorkSchedule(int employeeId, AttendanceCode attCode, DateTime datetime)
        {
            try
            {
                if (attCode == AttendanceCode.ClockIn)
                {
                    var attendace = new Attendance()
                    {
                        ClockIn = datetime,
                        ClockOut = null,
                        EmployeeId = employeeId
                    };

                    _attendanceRepository.Add(attendace);
                }
                else if (attCode == AttendanceCode.ClockOut)
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

        public int CreateWorkSchedulesByDateRange(DateTime fromDate, DateTime toDate)
        {
            try
            {
                IList<AttendanceLog> logs =
                   new List<AttendanceLog>(_attendanceLogService.GetAttendanceLogsToBeProcessed(fromDate, toDate));

                Attendance previousAttendance = null;
                foreach (var attendanceLog in logs) {
                    //If next employee
                    if (attendanceLog.EmployeeId != previousAttendance.EmployeeId)
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
                        //It means we have to create new attendance
                        if (previousAttendance == null)
                        {
                            attendance = new Attendance()
                            {
                                ClockIn = attendanceLog.ClockInOut,
                                ClockOut = null,
                                EmployeeId = attendanceLog.EmployeeId,
                                IsManuallyEdited = false
                            };

                            attendanceLog.IsConsidered = true;
                        }
                        else {
                            //Else do nothing
                                // This means it's double tap
                            attendanceLog.IsConsidered = false;
                        }
                    }

                    //Save or Update
                    if (attendance != null)
                        this.Save(attendance);

                    //Update attendance log
                    attendanceLog.IsRecorded = true;
                    _attendanceLogService.Save(attendanceLog);

                    _unitOfWork.Commit();

                    //Set previous attendance
                    previousAttendance = attendance;

                }
                return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public void Save(Attendance attendance)
        {
            if (attendance.AttendanceId != null)
                _attendanceRepository.Update(attendance);
            else
                _attendanceRepository.Add(attendance);

        }

        public IList<Attendance> GetAttendanceByDateRange(int employeeId, DateTime fromDate, DateTime toDate)
        {
            return _attendanceRepository.GetAttendanceByDateRange(employeeId, fromDate, toDate);
        }
    }
}
