/**********************************************************
 * Demo for Standalone SDK.Created by Darcy on Oct.15 2009*
***********************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using Payroll.Entities;
using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using RTEvents;
using RTEvents.Properties;

namespace Payroll.AttendanceManager
{
    public partial class CardMaintenance : Form
    {
        private readonly IDatabaseFactory _databaseFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _employeeRepository;

        //properties
        private int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.

        public CardMaintenance()
        {
            InitializeComponent();
            _databaseFactory = new DatabaseFactory();
            _unitOfWork = new UnitOfWork(_databaseFactory);
            _employeeRepository = new EmployeeRepository(_databaseFactory);
        }

        /**************************************************************************************************
        * Before you refer to this demo,we strongly suggest you read the development manual deeply first. *
        * This part is for demonstrating the RealTime Events triggered by your operations on the device.  *
        * Here is part of the real time events, more pls refer to the RTEvents demo                       *
        * *************************************************************************************************/
        #region RealTime Events

        //After function GetRTLog() is called ,RealTime Events will be triggered. 
        //When you are using these two functions, it will request data from the device forwardly.
        private void rtTimer_Tick(object sender, EventArgs e)
        {
            if (Program._czkemClass.ReadRTLog(iMachineNumber))
            {
                while (Program._czkemClass.GetRTLog(iMachineNumber))
                {
                    ;
                }
            }
        }

        #endregion

        /**************************************************************************************************
        * Before you refer to this demo,we strongly suggest you read the development manual deeply first. *
        * This part is for demonstrating  operations on card(ID card and HID card) device.                *
        * It shows how to get or set card number,how to write data to Mifare card or empty it, etc.       *
        * *************************************************************************************************/
        #region Card Operation

        //Empty the Mifare Card(For both Black&White and TFT screen devices)
        private void btnEmptyCard_Click(object sender, EventArgs e)
        {
            if (Program._connected == false)
            {
                MessageBox.Show("Please connect the device first", "Error");
                return;
            }
            int idwErrorCode = 0;

            Cursor = Cursors.WaitCursor;
            if (Program._czkemClass.EmptyCard(iMachineNumber))
            {
                MessageBox.Show("EmptyCard(Mifare)! ", "Success");
            }
            else
            {
                Program._czkemClass.GetLastError(ref idwErrorCode);
                MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            Cursor = Cursors.Default;
        }

        //It is mainly for demonstrating how to download the cardnumber from the device.
        //Card number is part of the user information.
        private void btnGetStrCardNumber_Click(object sender, EventArgs e)
        {
            if (Program._connected == false)
            {
                MessageBox.Show("Please connect the device first!", "Error");
                return;
            }

            string sdwEnrollNumber = "";
            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;
            bool bEnabled = false;
            string sCardnumber = "";

            //lvCard.Items.Clear();
            //lvCard.BeginUpdate();
            Cursor = Cursors.WaitCursor;
            Program._czkemClass.EnableDevice(iMachineNumber, false);//disable the device
            Program._czkemClass.ReadAllUserID(iMachineNumber);//read all the user information to the memory
            while (Program._czkemClass.SSR_GetAllUserInfo(iMachineNumber, out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))//get user information from memory
            {
                if (Program._czkemClass.GetStrCardNumber(out sCardnumber))//get the card number from the memory
                {
                    ListViewItem list = new ListViewItem();
                    list.Text = sdwEnrollNumber;
                    list.SubItems.Add(sName);
                    list.SubItems.Add(sCardnumber);
                    list.SubItems.Add(iPrivilege.ToString());
                    list.SubItems.Add(sPassword);
                    if (bEnabled == true)
                    {
                        list.SubItems.Add("true");
                    }
                    else
                    {
                        list.SubItems.Add("false");
                    }
                    //lvCard.Items.Add(list);
                }
            }
            Program._czkemClass.EnableDevice(iMachineNumber, true);//enable the device
            //lvCard.EndUpdate();
            Cursor = Cursors.Default;
        }

        //Upload the cardnumber as part of the user information
        private void btnSetStrCardNumber_Click(object sender, EventArgs e)
        {

            if (Program._connected == false)
            {
                MessageBox.Show("Please connect the device first!", "Error");
                return;
            }

            int idwErrorCode = 0;

            bool bEnabled = true;
            bEnabled = chbEnabled.Checked;
            string sName = txtName.Text.Trim();
            string sPassword = "";//txtPassword.Text.Trim();
            int iPrivilege = 0;//Convert.ToInt32(cbPrivilege.Text.Trim());
            string sCardnumber = txtCardnumber.Text.Trim();

            Cursor = Cursors.WaitCursor;
            Program._czkemClass.EnableDevice(iMachineNumber, false);
            string sdwEnrollNumber = txtUserID.Text.Trim();
            Program._czkemClass.SetStrCardNumber(sCardnumber);//Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
            if (Program._czkemClass.SSR_SetUserInfo(iMachineNumber, sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload the user's information(card number included)
            {
                //update the record in db
                try
                {
                    var employee = _employeeRepository.GetById(Convert.ToInt32(sdwEnrollNumber));
                    _employeeRepository.Update(employee);
                    employee.EmployeeCode = txtCardnumber.Text;
                    employee.EnrolledToRfid = true;
                    employee.Enabled = chbEnabled.Checked;
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    
                }

                MessageBox.Show(Resources.Message_CardRegistrationSuccess);
                Rebind();
            }
            else
            {
                Program._czkemClass.GetLastError(ref idwErrorCode);
                MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode, "Error");
            }
            Program._czkemClass.RefreshData(iMachineNumber);//the data in the device should be refreshed
            Program._czkemClass.EnableDevice(iMachineNumber, true);
            Cursor = Cursors.Default;
        }

        bool bAddControl = true;
        #endregion

        private void Rebind()
        {
            //load the employees to the grid
            var employees = _employeeRepository.Find(x => x.IsActive).ToList();
            GridView.AutoGenerateColumns = false;
            GridView.DataSource = employees;
        }

        private void CardMaintenance_Load(object sender, EventArgs e)
        {
            Rebind();
            RegisterEvents();
        }

        public void RegisterEvents()
        {
            if (Program._czkemClass.RegEvent(iMachineNumber, 65535))
            {
                Program._czkemClass.OnHIDNum += new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(OnCardTap);
            }
        }

        private void GridView_DoubleClick(object sender, EventArgs e)
        {
            var grid = (DataGridView) sender;
            txtUserID.Text = grid.CurrentRow.Cells[0].Value.ToString();
            txtName.Text = grid.CurrentRow.Cells[1].Value.ToString();
            txtCardnumber.Text = grid.CurrentRow.Cells[2].Value.ToString();
            chbEnabled.Checked = Convert.ToBoolean(grid.CurrentRow.Cells[5].Value);
        }

        private void OnCardTap(int iCardNumber)
        {
            txtCardnumber.Text = iCardNumber.ToString();
        }

        private void CardMaintenance_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program._czkemClass.OnHIDNum -= new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(OnCardTap);
        }

    }
} 