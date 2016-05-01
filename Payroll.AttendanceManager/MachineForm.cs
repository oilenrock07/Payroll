using System;
using System.Windows.Forms;
using Payroll.Entities;
using Payroll.Entities.Enums;
using System.Net;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using zkemkeeper;
using Payroll.Common.Extension;

namespace AttendanceManager
{
    public partial class MachineForm : Form
    {
        public string _ipAddress = "";
        public bool _connected = false;
        private int _iMachineNumber = 1;

        public static CZKEMClass _czkemClass;
        private readonly IAttendanceLogRepository _attendanceLogRepository;

        public MachineForm()
        {
            InitializeComponent();
            _attendanceLogRepository = new AttendanceLogRepository(Program._databaseFactory, Program._employeeRepository);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            lblIpAddress.Text = _ipAddress;
            _czkemClass = new CZKEMClass();
        }

        private void axCZKEM1_OnVerify(int iUserID)
        {
            lbRTShow.Items.Add("RTEvent OnVerify Has been Triggered,Verifying...");
            if (iUserID != -1)
            {
                lbRTShow.Items.Add("Verified OK,the UserID is " + iUserID.ToString());
            }
            else
            {
                lbRTShow.Items.Add("Verified Failed... ");
            }
        }

        //If your fingerprint(or your card) passes the verification,this event will be triggered
        private void axCZKEM1_OnAttTransactionEx(string sEnrollNumber, int iIsInValid, int iAttState, int iVerifyMethod, int iYear, int iMonth, int iDay, int iHour, int iMinute, int iSecond, int iWorkCode)
        {
            //if valid, insert to database and display the picture to screen
            if (iIsInValid == 0)
            {
                //insert to database
                var attendaceLog = new AttendanceLog
                {
                    EmployeeId = Convert.ToInt32(sEnrollNumber),
                    ClockInOut = DateTime.Now,
                    Type = (AttendanceType)Convert.ToInt16(iAttState)
                };

                _attendanceLogRepository.Add(attendaceLog);
                Program._unitOfWork.Commit();

                //display the picture to screen
                var url = Program.GetSettingValue("DISPLAY_LOGIN_URL", "http://payroll.logindisplay/api/payrollapi/");
                var client = WebRequest.Create(String.Format("{0}/{1}/{2}/{3}", url, sEnrollNumber, iAttState, DateTime.Now.Serialize()));
                client.GetResponse();
            }



            lbRTShow.Items.Add("RTEvent OnAttTrasactionEx Has been Triggered,Verified OK");
            lbRTShow.Items.Add("...UserID:" + sEnrollNumber);
            lbRTShow.Items.Add("...isInvalid:" + iIsInValid.ToString());
            lbRTShow.Items.Add("...attState:" + iAttState.ToString());
            lbRTShow.Items.Add("...VerifyMethod:" + iVerifyMethod.ToString());
            lbRTShow.Items.Add("...Workcode:" + iWorkCode.ToString());//the difference between the event OnAttTransaction and OnAttTransactionEx
            lbRTShow.Items.Add("...Time:" + iYear.ToString() + "-" + iMonth.ToString() + "-" + iDay.ToString() + " " + iHour.ToString() + ":" + iMinute.ToString() + ":" + iSecond.ToString());
        }

        //When you have enrolled a new user,this event will be triggered.
        private void axCZKEM1_OnNewUser(int iEnrollNumber)
        {
            lbRTShow.Items.Add("RTEvent OnNewUser Has been Triggered...");
            lbRTShow.Items.Add("...NewUserID=" + iEnrollNumber.ToString());
        }

        //When you swipe a card to the device, this event will be triggered to show you the card number.
        private void axCZKEM1_OnHIDNum(int iCardNumber)
        {
            lbRTShow.Items.Add("RTEvent OnHIDNum Has been Triggered...");
            lbRTShow.Items.Add("...Cardnumber=" + iCardNumber.ToString());
        }

        //When you have emptyed the Mifare card,this event will be triggered.
        private void axCZKEM1_OnEmptyCard(int iActionResult)
        {
            lbRTShow.Items.Add("RTEvent OnEmptyCard Has been Triggered...");
            if (iActionResult == 0)
            {
                lbRTShow.Items.Add("...Empty Mifare Card OK");
            }
            else
            {
                lbRTShow.Items.Add("...Empty Failed");
            }
        }

        //When you have written into the Mifare card ,this event will be triggered.
        private void axCZKEM1_OnWriteCard(int iEnrollNumber, int iActionResult, int iLength)
        {
            lbRTShow.Items.Add("RTEvent OnWriteCard Has been Triggered...");
            if (iActionResult == 0)
            {
                lbRTShow.Items.Add("...Write Mifare Card OK");
                lbRTShow.Items.Add("...EnrollNumber=" + iEnrollNumber.ToString());
                lbRTShow.Items.Add("...TmpLength=" + iLength.ToString());
            }
            else
            {
                lbRTShow.Items.Add("...Write Failed");
            }
        }

        //After function GetRTLog() is called ,RealTime Events will be triggered. 
        //When you are using these two functions, it will request data from the device forwardly.
        private void rtTimer_Tick(object sender, EventArgs e)
        {
            if (_czkemClass.ReadRTLog(_iMachineNumber))
            {
                while (_czkemClass.GetRTLog(_iMachineNumber))
                {
                    ;
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
                _czkemClass.Disconnect();

                _czkemClass.OnVerify -= new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
                _czkemClass.OnAttTransactionEx -= new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx);
                _czkemClass.OnNewUser -= new zkemkeeper._IZKEMEvents_OnNewUserEventHandler(axCZKEM1_OnNewUser);
                _czkemClass.OnHIDNum -= new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(axCZKEM1_OnHIDNum);
                _czkemClass.OnWriteCard -= new zkemkeeper._IZKEMEvents_OnWriteCardEventHandler(axCZKEM1_OnWriteCard);
                _czkemClass.OnEmptyCard -= new zkemkeeper._IZKEMEvents_OnEmptyCardEventHandler(axCZKEM1_OnEmptyCard);

                _connected = false;
                btnConnect.Text = "Connect";
                lblState.Text = "Disconnected";
                Cursor = Cursors.Default;
                return;
            }

            _connected = _czkemClass.Connect_Net(_ipAddress, Program._port);
            if (_connected)
            {
                btnConnect.Text = "Disconnect";
                btnConnect.Refresh();
                lblState.Text = "Connected";
                _iMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                
                if (_czkemClass.RegEvent(_iMachineNumber, 65535))//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                {
                    _czkemClass.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
                    _czkemClass.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx);
                    _czkemClass.OnNewUser += new zkemkeeper._IZKEMEvents_OnNewUserEventHandler(axCZKEM1_OnNewUser);
                    _czkemClass.OnHIDNum += new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(axCZKEM1_OnHIDNum);
                    _czkemClass.OnWriteCard += new zkemkeeper._IZKEMEvents_OnWriteCardEventHandler(axCZKEM1_OnWriteCard);
                    _czkemClass.OnEmptyCard += new zkemkeeper._IZKEMEvents_OnEmptyCardEventHandler(axCZKEM1_OnEmptyCard);
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
    }
}
