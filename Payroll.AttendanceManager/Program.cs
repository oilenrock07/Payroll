using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using zkemkeeper;

namespace RTEvents
{
    public static class Program
    {
        //repository
        public static IDatabaseFactory _databaseFactory;
        public static IUnitOfWork _unitOfWork;
        public static ISettingRepository _settingRepository;

        public static IEnumerable<Setting> _settings;

        public static CZKEMClass _czkemClass;
        public static PayrollMain _mainForm;
        public static bool _connected = false;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            _databaseFactory = new DatabaseFactory();
            _unitOfWork = new UnitOfWork(_databaseFactory);
            
            LoadSettings();

            _mainForm = new PayrollMain();
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

    }
}