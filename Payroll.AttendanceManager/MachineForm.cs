using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Payroll.Entities;
using Payroll.Entities.Enums;
using System.Net;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Service.Implementations;
using Payroll.Service.Interfaces;
using zkemkeeper;
using Payroll.Common.Extension;

namespace AttendanceManager
{
    public partial class MachineForm : Form
    {
        public string _ipAddress = "";
        public bool _connected = false;
        public int _machineNumber = 0;
        public int _sdkMachineNumber = 1;
        private bool _isNewEmployee = true;

        public CZKEMClass _czkemClass;

        private readonly IAttendanceLogRepository _attendanceLogRepository;
        private readonly IEmployeeMachineService _employeeMachineService;
        private readonly IEmployeeMachineRepository _employeeMachineRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDatabaseFactory _databaseFactory;
        private readonly IUnitOfWork _unitOfWork;

        private Dictionary<string, int> _employeeCodes; 

        public MachineForm()
        {
            InitializeComponent();

            _databaseFactory = new DatabaseFactory();
            _unitOfWork = new UnitOfWork(_databaseFactory);

            _employeeMachineRepository = new EmployeeMachineRepository(_databaseFactory);
            _employeeRepository = new EmployeeRepository(_databaseFactory, new EmployeeDepartmentRepository(_databaseFactory));
            _employeeMachineService = new EmployeeMachineService(_employeeMachineRepository, _employeeRepository);
            _attendanceLogRepository = new AttendanceLogRepository(_databaseFactory, _employeeRepository);
        }

        #region SDK Events
        private void axCZKEM1_OnVerify(int iUserID)
        {
            lbRTShow.Items.Add("**OnVerify**");
            if (iUserID != -1)
            {
                lbRTShow.Items.Add("Verified OK,the UserID is " + iUserID.ToString());
            }
            else
            {
                lbRTShow.Items.Add("Verified Failed");
            }
        }

        //If your fingerprint(or your card) passes the verification,this event will be triggered
        private void axCZKEM1_OnAttTransactionEx(string sEnrollNumber, int iIsInValid, int iAttState, int iVerifyMethod, int iYear, int iMonth, int iDay, int iHour, int iMinute, int iSecond, int iWorkCode)
        {
            //if valid, insert to database and display the picture to screen
            try
            {
                if (iIsInValid == 0)
                {
                    //insert to database
                    var employeeId = _employeeCodes[sEnrollNumber];
                    var attendaceLog = new AttendanceLog
                    {
                        EmployeeId = employeeId,
                        ClockInOut = DateTime.Now,
                        Type = (AttendanceType)Convert.ToInt16(iAttState),
                        IpAddress = _ipAddress,
                        MachineId = _machineNumber
                    };

                    _attendanceLogRepository.Add(attendaceLog);
                    _unitOfWork.Commit();

                    //display the picture to screen
                    var url = Program.GetSettingValue("DISPLAY_LOGIN_URL", "http://payroll.logindisplay/api/payrollapi/");
                    var client = WebRequest.Create(String.Format("{0}/{1}/{2}/{3}/{4}", url, sEnrollNumber, _ipAddress, iAttState, DateTime.Now.Serialize()));
                    client.GetResponse();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            



            lbRTShow.Items.Add("**OnAttTrasactionEx**");
            lbRTShow.Items.Add("\tUserID:" + sEnrollNumber);
            lbRTShow.Items.Add("\tisInvalid:" + iIsInValid.ToString());
            lbRTShow.Items.Add("\tattState:" + iAttState.ToString());
            lbRTShow.Items.Add("\tVerifyMethod:" + iVerifyMethod.ToString());
            lbRTShow.Items.Add("\tWorkcode:" + iWorkCode.ToString());//the difference between the event OnAttTransaction and OnAttTransactionEx
            lbRTShow.Items.Add("\tTime:" + iYear.ToString() + "-" + iMonth.ToString() + "-" + iDay.ToString() + " " + iHour.ToString() + ":" + iMinute.ToString() + ":" + iSecond.ToString());
        }


        //When you swipe a card to the device, this event will be triggered to show you the card number.
        private void axCZKEM1_OnHIDNum(int iCardNumber)
        {
            lbRTShow.Items.Add("**OnHIDNum**");
            lbRTShow.Items.Add("\tCardnumber=" + iCardNumber.ToString());
        }
        #endregion

        #region Form Events
        private void Form_Load(object sender, EventArgs e)
        {
            lblIpAddress.Text = _ipAddress;
            _czkemClass = new CZKEMClass();
            _employeeCodes = new Dictionary<string, int>();
            Rebind();
        }

        private void Rebind(bool showOnlyNotRegistered = false)
        {
            //load the employees that are registered to this machine
            var employees = showOnlyNotRegistered
                ? _employeeMachineService.GetEmployeesNotRegistered(_machineNumber)
                : _employeeMachineService.GetEmployees(_machineNumber);

            _employeeCodes.Clear();
            foreach (var employee in employees)
            {
                _employeeCodes.Add(employee.EmployeeCode,employee.EmployeeId);
            }

            GridView.AutoGenerateColumns = false;
            GridView.DataSource = employees;

            foreach (var control in this.Controls)
            {
                var box = control as TextBox;
                if (box != null)
                {
                    box.Text = "";
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (_ipAddress.Trim() == "" || _ipAddress.Trim() == "")
            {
                MessageBox.Show("IP and Port cannot be null", "Error");
                return;
            }

            int idwErrorCode = 0;

            Cursor = Cursors.WaitCursor;
            if (_connected)
            {
                Disconnect();
                Cursor = Cursors.Default;
                return;
            }

            _connected = _czkemClass.Connect_Net(_ipAddress, Program._port);
            if (_connected)
            {
                btnConnect.Text = "Disconnect";
                btnConnect.Refresh();
                lblState.Text = "Connected";
                _sdkMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.

                if (_czkemClass.RegEvent(_sdkMachineNumber, 65535))//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                {
                    _czkemClass.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
                    _czkemClass.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx);
                    _czkemClass.OnHIDNum += new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(axCZKEM1_OnHIDNum);
                }

                _connected = true;
            }
            else
            {
                _czkemClass.GetLastError(ref idwErrorCode);
                MessageBox.Show("Unable to connect the device,ErrorCode=" + idwErrorCode, "Error");
            }
            Cursor = Cursors.Default;
        }

        private void GridView_DoubleClick(object sender, EventArgs e)
        {
            var grid = (DataGridView)sender;
            txtUserID.Text = grid.CurrentRow.Cells["Id"].Value.ToString();
            txtName.Text = GetNameFromGrid(grid.CurrentRow.Cells);
            txtCardnumber.Text = (grid.CurrentRow.Cells["CardNumber"].Value ?? "").ToString();
            _isNewEmployee = grid.CurrentRow.Cells["CardNumber"].Value == null;
        }
        #endregion

        private string GetNameFromGrid(DataGridViewCellCollection cell)
        {
            return String.Format("{0} {1}", cell["FirstName"].Value, cell["LastName"].Value);
        }

        private void btnSetStrCardNumber_Click(object sender, EventArgs e)
        {
            if (_connected == false)
            {
                MessageBox.Show("Please connect the device first!", "Error");
                return;
            }

            if (String.IsNullOrEmpty(txtUserID.Text))
            {
                MessageBox.Show("Please select employee to register", "Error");
                return;
            }



            var sCardnumber = txtCardnumber.Text.Trim();

            Cursor = Cursors.WaitCursor;
            _czkemClass.EnableDevice(_sdkMachineNumber, false);
            RegisterToMachine(sCardnumber, Convert.ToInt32(txtUserID.Text), txtName.Text);

            _czkemClass.RefreshData(_sdkMachineNumber);//the data in the device should be refreshed
            _czkemClass.EnableDevice(_sdkMachineNumber, true);
            Cursor = Cursors.Default;

        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            Rebind();
        }

        private void Disconnect()
        {
            _czkemClass.Disconnect();

            _czkemClass.OnVerify -= new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
            _czkemClass.OnAttTransactionEx -= new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx);
            //_czkemClass.OnNewUser -= new zkemkeeper._IZKEMEvents_OnNewUserEventHandler(axCZKEM1_OnNewUser);
            _czkemClass.OnHIDNum -= new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(axCZKEM1_OnHIDNum);
            //_czkemClass.OnWriteCard -= new zkemkeeper._IZKEMEvents_OnWriteCardEventHandler(axCZKEM1_OnWriteCard);
            //_czkemClass.OnEmptyCard -= new zkemkeeper._IZKEMEvents_OnEmptyCardEventHandler(axCZKEM1_OnEmptyCard);

            _connected = false;
            btnConnect.Text = "Connect";
            lblState.Text = "Disconnected";
        }

        private void MachineForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_connected) Disconnect();
        }

        private void RegisterToMachine(string employeeCode, int employeeId, string name, bool alertOnSuccess = true)
        {
            var idwErrorCode = 0;
            var canUpload = false;
            var isSuccess = false;
            var retryNumber = 10;

            //force register the card to machine
            //This will retry to register the employee for 10 times
            for (int i = 0; i <= retryNumber && !isSuccess; i++)
            {
                canUpload = _czkemClass.SetStrCardNumber(employeeCode);//Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
                if (canUpload)
                {
                    if (_czkemClass.SSR_SetUserInfo(_sdkMachineNumber, employeeCode, name, null, 0, true))//upload the user's information(card number included)
                    {
                        //update the record in db
                        try
                        {
                            if (!_isNewEmployee)
                            {
                                var existingEmployeeMachine = _employeeMachineRepository.GetByEmployeeId(employeeId, _machineNumber);
                                if (existingEmployeeMachine != null)
                                {
                                    _employeeMachineRepository.Update(existingEmployeeMachine);
                                    existingEmployeeMachine.IsActive = false;
                                }
                            }

                            var employeeMachine = new EmployeeMachine
                            {
                                EmployeeId = Convert.ToInt32(employeeId),
                                MachineId = _machineNumber,
                                UpdateDate = DateTime.Now
                            };

                            _employeeMachineRepository.Add(employeeMachine);
                            _unitOfWork.Commit();
                        }
                        catch (Exception ex)
                        {

                        }

                        isSuccess = true;
                        _czkemClass.RefreshData(_sdkMachineNumber);
                        if (alertOnSuccess) MessageBox.Show("Employee Successfully Registered");
                        Rebind();
                    }
                    else
                    {
                        if (i == retryNumber)
                        {
                            _czkemClass.GetLastError(ref idwErrorCode);
                            MessageBox.Show("Operation failed, ErrorCode=" + idwErrorCode, "Error" + "\nPlease Try Again");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Unable to register employee to device");
                }
            }
        }

        private void btnRegisterAll_Click(object sender, EventArgs e)
        {
            if (_connected == false)
            {
                MessageBox.Show("Please connect the device first!", "Error");
                return;
            }

            foreach (DataGridViewRow row in GridView.Rows)
            {
                var checkbox = row.Cells["Enrolled"] as DataGridViewCheckBoxCell;
                var cardNumber = row.Cells["CardNumber"].Value;
                if (!(bool)checkbox.Value && cardNumber != null)
                {
                    var cells = row.Cells;
                    RegisterToMachine(cardNumber.ToString(), Convert.ToInt32(cells["Id"].Value), GetNameFromGrid(cells), false);
                }
            }

            Rebind();
        }

        private void chkShowNotRegistered_CheckedChanged(object sender, EventArgs e)
        {
            Rebind(chkShowNotRegistered.Checked);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var directory = String.Format("{0}/{1}", Environment.CurrentDirectory, DateTime.Now.ToString("yyyyMMdd")); 
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fileName = "log.txt";
            var fullFileName = Path.Combine(directory, fileName);
            using (var streamWriter = new StreamWriter(fullFileName))
            {
                streamWriter.Write(lbRTShow.Text);
            }
            
            
            lbRTShow.ResetText();
        }
    }
}
