using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using RTEvents;
using zkemkeeper;

namespace AttendanceManager
{
    public static class Program
    {
        //repository
        public static IDatabaseFactory _databaseFactory;
        public static IUnitOfWork _unitOfWork;
        public static ISettingRepository _settingRepository;
        public static IEmployeeRepository _employeeRepository;
        public static IEmployeeDepartmentRepository _employeeDepartmentRepository;

        public static IEnumerable<Setting> _settings;

        public static CZKEMClass _czkemClass;
        //public static PayrollMain _mainForm;
        public static MainForm _mainForm;
        
        public static bool _connected = false;

        //todo: load the error codes
        public static Dictionary<int, string> _errorCodes;
 
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            _databaseFactory = new DatabaseFactory();
            _unitOfWork = new UnitOfWork(_databaseFactory);
            _employeeDepartmentRepository = new EmployeeDepartmentRepository(_databaseFactory);
            _employeeRepository = new EmployeeRepository(_databaseFactory, _employeeDepartmentRepository);

            LoadSettings();

            //_mainForm = new PayrollMain();
            _mainForm = new MainForm();
            _czkemClass = new CZKEMClass();
            

            Application.Run(_mainForm);
        }

        public static void LoadSettings()
        {
            _settingRepository = new SettingRepository(_databaseFactory);
            _settings = _settingRepository.GetAll().ToList();
        }

        public static string GetSettingValue(string key, string defaultValue = "")
        {
            var setting = _settings.FirstOrDefault(x => x.SettingKey == key);
            return setting != null ? setting.Value : defaultValue;
        }

        public static Dictionary<int, string> GetErrorCodes()
        {
            var doc = new XmlDocument();
            doc.Load(String.Format("{0}/device_error_codes.xml", Environment.CurrentDirectory));

            return new Dictionary<int, string>();
        }

    }
}