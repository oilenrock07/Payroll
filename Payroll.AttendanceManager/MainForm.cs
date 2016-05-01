using System.Windows.Forms;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;

namespace AttendanceManager
{
    public partial class MainForm : Form
    {

        public IMachineRepository _machineRepository;

        public MainForm()
        {
            InitializeComponent();
            _machineRepository = new MachineRepository(Program._databaseFactory);
            var machines = _machineRepository.Find(x => x.IsActive);

            gvMachine.DataSource = machines;
        }
    }
}
