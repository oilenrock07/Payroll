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

        public static IEnumerable<Setting> _settings;

        //public static PayrollMain _mainForm;
        public static MainForm _mainForm;

        public static int _port = 4370;

        //todo: load the error codes
        public static Dictionary<int, string> _errorCodes;
 
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            _databaseFactory = new DatabaseFactory();
            _unitOfWork = new UnitOfWork(_databaseFactory);

            LoadSettings();

            //_mainForm = new PayrollMain();
            _mainForm = new MainForm();          

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