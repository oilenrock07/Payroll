using System;
using System.Collections.Generic;
using System.Windows.Forms;
using zkemkeeper;

namespace RTEvents
{
    public static class Program
    {
        public static zkemkeeper.CZKEMClass _czkemClass;
        public static PayrollMain _mainForm;
        public static bool _connected = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _mainForm = new PayrollMain();
            _czkemClass = new CZKEMClass();

            Application.Run(_mainForm);
        }
    }
}