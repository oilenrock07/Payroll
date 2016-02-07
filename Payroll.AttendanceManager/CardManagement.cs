/**********************************************************
 * Demo for Standalone SDK.Created by Darcy on Oct.15 2009*
***********************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using RTEvents;

namespace Payroll.AttendanceManager
{
    public partial class CardMaintenance : Form
    {
        public CardMaintenance()
        {
            InitializeComponent();
        }

        /*************************************************************************************************
        * Before you refer to this demo,we strongly suggest you read the development manual deeply first.*
        * This part is for demonstrating the communication with your device.                             *
        * ************************************************************************************************/
        #region Communication
        private int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.

        #endregion

        /**************************************************************************************************
        * Before you refer to this demo,we strongly suggest you read the development manual deeply first. *
        * This part is for demonstrating the RealTime Events triggered by your operations on the device.  *
        * Here is part of the real time events, more pls refer to the RTEvents demo                       *
        * *************************************************************************************************/
        #region RealTime Events

        //When you have enrolled a new user,this event will be triggered.
        private void axCZKEM1_OnNewUser(int iEnrollNumber)
        {
            lbRTShow.Items.Add("RTEvent OnNewUser Has been Triggered...");
            lbRTShow.Items.Add("...NewUserID=" + iEnrollNumber.ToString());
        }

        //When you swipe a card to the device, this event will be triggered to show you the number of the card.
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

        //After you swipe your card to the device,this event will be triggered.
        //If your card passes the verification,the return value  will be user id, or else the value will be -1
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

        //If your card passes the verification,this event will be triggered
        private void axCZKEM1_OnAttTransactionEx(string sEnrollNumber, int iIsInValid, int iAttState, int iVerifyMethod, int iYear, int iMonth, int iDay, int iHour, int iMinute, int iSecond, int iWorkCode)
        {
            lbRTShow.Items.Add("RTEvent OnAttTrasactionEx Has been Triggered,Verified OK");
            lbRTShow.Items.Add("...UserID:" + sEnrollNumber);
            lbRTShow.Items.Add("...isInvalid:" + iIsInValid.ToString());
            lbRTShow.Items.Add("...attState:" + iAttState.ToString());
            lbRTShow.Items.Add("...VerifyMethod:" + iVerifyMethod.ToString());
            lbRTShow.Items.Add("...Workcode:" + iWorkCode.ToString());//the difference between the event OnAttTransaction and OnAttTransactionEx
            lbRTShow.Items.Add("...Time:" + iYear.ToString() + "-" + iMonth.ToString() + "-" + iDay.ToString() + " " + iHour.ToString() + ":" + iMinute.ToString() + ":" + iSecond.ToString());

            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;
            bool bEnabled = false;
            string sCardnumber = "";

            while (Program._czkemClass.SSR_GetUserInfo(iMachineNumber, sEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))//get user information from memory
            {
                if (Program._czkemClass.GetStrCardNumber(out sCardnumber))//get the card number from the memory
                {
                    lbRTShow.Items.Add("...Cardnumber:" + sCardnumber);
                    return;
                }
            }
        }

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

        //Write someone' fingerprint templates into Mifare card, after performing this order, the prompt to slip card will appear on the device LCD.
        private void btnWriteCard_Click(object sender, EventArgs e)
        {
            if (Program._connected == false)
            {
                MessageBox.Show("Please connect the device first", "Error");
                return;
            }

            if (cbUserID.Text.Trim()=="" || cbTmpToWrite.Text.Trim()=="")
            {
                MessageBox.Show("UserID and The Count of Tmp to Write cannot be null", "Error");
                return;
            }
            int idwErrorCode = 0;

            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;
            bool bEnabled = false;
            int idwFigerIndex;
            int iTmpLength = 0;

            string sdwEnrollNumber = cbUserID.Text.Trim();//modify by Darcy on Nov.23 2009
            int iTmpToWrite=Convert.ToInt32(cbTmpToWrite.Text.Trim());//the possible values 1,2,3,4
            int iTmpCount = 0;//the count of the fingerprint templates to be written in
            byte[] byTmpData0=new byte[700];
            byte[] byTmpData1 = new byte[700];//9.0 fingerprint arithmetic templates
            byte[] byTmpData2 = new byte[700];
            byte[] byTmpData3 = new byte[700];
            Cursor = Cursors.WaitCursor;

            Program._czkemClass.ReadAllTemplate(iMachineNumber);//it's nesessary to read the templates to the memory
            if (Program._czkemClass.SSR_GetUserInfo(iMachineNumber, sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))//modify by Darcy on Nov.23 2009
            {
                //Here we write at most 4 fingerprint templates(the user's first four ones) in the Mifare card.
                //If you want to write other indexs of the templates,you can write your own code to achive.
                for (idwFigerIndex = 0; idwFigerIndex < iTmpToWrite; idwFigerIndex++)
                {
                    byte[] byTmpData = new byte[700];
                    if (Program._czkemClass.SSR_GetUserTmp(iMachineNumber, sdwEnrollNumber, idwFigerIndex, out byTmpData[0], out iTmpLength))//modify by Darcy on Nov.23 2009
                    {
                        iTmpCount++;
                        switch (iTmpCount)
                        {
                            case 1:
                                Array.Copy(byTmpData,byTmpData0,iTmpLength);
                                break;
                            case 2:
                                Array.Copy(byTmpData, byTmpData1, iTmpLength);
                                break;
                            case 3:
                                Array.Copy(byTmpData,byTmpData2,iTmpLength);
                                break;
                            case 4:
                                Array.Copy(byTmpData,byTmpData3,iTmpLength);
                                break;
                        }
                    }
                    byTmpData=null;
                }
            }
            int iEnrollNumber = Convert.ToInt32(sdwEnrollNumber);
            if (Program._czkemClass.WriteCard(iMachineNumber, iEnrollNumber, 0, ref byTmpData0[0], 1, ref byTmpData1[0], 2, ref byTmpData2[0], 3, ref byTmpData3[0]))//write templates into card
            {
                MessageBox.Show("WriteCard(Mifare)! ", "Success");
            }
            else
            {
                Program._czkemClass.GetLastError(ref idwErrorCode);
                MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }

            Cursor = Cursors.Default;
        }

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

            lvCard.Items.Clear();
            lvCard.BeginUpdate();
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
                    lvCard.Items.Add(list);
                }
            }
            Program._czkemClass.EnableDevice(iMachineNumber, true);//enable the device
            lvCard.EndUpdate();
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

            if (txtUserID.Text.Trim() == "" || cbPrivilege.Text.Trim() == "" || txtCardnumber.Text.Trim() == "")
            {
                MessageBox.Show("UserID,Privilege,Cardnumber must be inputted first!", "Error");
                return;
            }
            int idwErrorCode = 0;

            bool bEnabled = true;
            if (chbEnabled.Checked)
            {
                bEnabled = true;
            }
            else
            {
                bEnabled = false;
            }
            string sName = txtName.Text.Trim();
            string sPassword = txtPassword.Text.Trim();
            int iPrivilege = Convert.ToInt32(cbPrivilege.Text.Trim());
            string sCardnumber = txtCardnumber.Text.Trim();

            Cursor = Cursors.WaitCursor;
            Program._czkemClass.EnableDevice(iMachineNumber, false);
            string sdwEnrollNumber = txtUserID.Text.Trim();
            Program._czkemClass.SetStrCardNumber(sCardnumber);//Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
            if (Program._czkemClass.SSR_SetUserInfo(iMachineNumber, sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload the user's information(card number included)
            {
                MessageBox.Show("(SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Success");
            }
            else
            {
                Program._czkemClass.GetLastError(ref idwErrorCode);
                MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            Program._czkemClass.RefreshData(iMachineNumber);//the data in the device should be refreshed
            Program._czkemClass.EnableDevice(iMachineNumber, true);
            Cursor = Cursors.Default;
        }

        //add by Darcy on Nov.23 2009
        //Add the existed userid to DropDownLists.
        bool bAddControl = true;
        private void UserIDTimer_Tick(object sender, EventArgs e)
        {
            if (Program._connected == false)
            {
                cbUserID.Items.Clear();
                bAddControl = true;
                return;
            }
            else
            {
                if (bAddControl == true)
                {
                    string sEnrollNumber = "";
                    string sName = "";
                    string sPassword = "";
                    int iPrivilege = 0;
                    bool bEnabled = false;

                    Program._czkemClass.EnableDevice(iMachineNumber, false);
                    Program._czkemClass.ReadAllUserID(iMachineNumber);//read all the user information to the memory
                    while (Program._czkemClass.SSR_GetAllUserInfo(iMachineNumber, out sEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))
                    {
                        cbUserID.Items.Add(sEnrollNumber);
                    }
                    bAddControl = false;
                    Program._czkemClass.EnableDevice(iMachineNumber, true);
                }
                return;
            }
        }
        #endregion

    }
} 