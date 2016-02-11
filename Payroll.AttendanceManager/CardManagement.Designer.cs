namespace Payroll.AttendanceManager
{
    partial class CardMaintenance
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Timer rtTimer;
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lvCard = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnGetStrCardNumber = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.chbEnabled = new System.Windows.Forms.CheckBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnSetStrCardNumber = new System.Windows.Forms.Button();
            this.label89 = new System.Windows.Forms.Label();
            this.cbPrivilege = new System.Windows.Forms.ComboBox();
            this.txtCardnumber = new System.Windows.Forms.TextBox();
            this.Privilege = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.label90 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.cbUserID = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbTmpToWrite = new System.Windows.Forms.ComboBox();
            this.label28 = new System.Windows.Forms.Label();
            this.btnWriteCard = new System.Windows.Forms.Button();
            this.btnEmptyCard = new System.Windows.Forms.Button();
            this.label31 = new System.Windows.Forms.Label();
            this.UserIDTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbRTShow = new System.Windows.Forms.ListBox();
            rtTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtTimer
            // 
            rtTimer.Interval = 800;
            rtTimer.Tick += new System.EventHandler(this.rtTimer_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Location = new System.Drawing.Point(479, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(491, 423);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Download or Upload Card Number";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Crimson;
            this.label4.Location = new System.Drawing.Point(13, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(305, 13);
            this.label4.TabIndex = 46;
            this.label4.Text = "Please make sure your device has an optional ID card module. ";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lvCard);
            this.groupBox4.Controls.Add(this.btnGetStrCardNumber);
            this.groupBox4.Location = new System.Drawing.Point(6, 52);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(479, 239);
            this.groupBox4.TabIndex = 43;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Download the Card Number(A property of user information)";
            // 
            // lvCard
            // 
            this.lvCard.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.lvCard.GridLines = true;
            this.lvCard.Location = new System.Drawing.Point(6, 16);
            this.lvCard.Name = "lvCard";
            this.lvCard.Size = new System.Drawing.Size(467, 186);
            this.lvCard.TabIndex = 45;
            this.lvCard.UseCompatibleStateImageBehavior = false;
            this.lvCard.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "UserID";
            this.columnHeader1.Width = 54;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 41;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Cardnumber";
            this.columnHeader3.Width = 78;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Privilege";
            this.columnHeader4.Width = 92;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Password";
            this.columnHeader5.Width = 76;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Enabled";
            this.columnHeader6.Width = 84;
            // 
            // btnGetStrCardNumber
            // 
            this.btnGetStrCardNumber.Location = new System.Drawing.Point(181, 208);
            this.btnGetStrCardNumber.Name = "btnGetStrCardNumber";
            this.btnGetStrCardNumber.Size = new System.Drawing.Size(117, 23);
            this.btnGetStrCardNumber.TabIndex = 1;
            this.btnGetStrCardNumber.Text = "GetStrCardNumber";
            this.btnGetStrCardNumber.UseVisualStyleBackColor = true;
            this.btnGetStrCardNumber.Click += new System.EventHandler(this.btnGetStrCardNumber_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtUserID);
            this.groupBox5.Controls.Add(this.txtName);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.chbEnabled);
            this.groupBox5.Controls.Add(this.txtPassword);
            this.groupBox5.Controls.Add(this.btnSetStrCardNumber);
            this.groupBox5.Controls.Add(this.label89);
            this.groupBox5.Controls.Add(this.cbPrivilege);
            this.groupBox5.Controls.Add(this.txtCardnumber);
            this.groupBox5.Controls.Add(this.Privilege);
            this.groupBox5.Controls.Add(this.label55);
            this.groupBox5.Controls.Add(this.label90);
            this.groupBox5.Location = new System.Drawing.Point(6, 304);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(479, 111);
            this.groupBox5.TabIndex = 44;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Upload the Card Number(part of users information)";
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(90, 15);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(69, 20);
            this.txtUserID.TabIndex = 56;
            this.txtUserID.Text = "1";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(238, 15);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(76, 20);
            this.txtName.TabIndex = 57;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(40, 19);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(62, 18);
            this.label16.TabIndex = 63;
            this.label16.Text = "User ID";
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(208, 20);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(36, 17);
            this.label15.TabIndex = 64;
            this.label15.Text = "Name";
            // 
            // chbEnabled
            // 
            this.chbEnabled.AutoSize = true;
            this.chbEnabled.Location = new System.Drawing.Point(383, 50);
            this.chbEnabled.Name = "chbEnabled";
            this.chbEnabled.Size = new System.Drawing.Size(15, 14);
            this.chbEnabled.TabIndex = 69;
            this.chbEnabled.UseVisualStyleBackColor = true;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(383, 15);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(67, 20);
            this.txtPassword.TabIndex = 58;
            // 
            // btnSetStrCardNumber
            // 
            this.btnSetStrCardNumber.Location = new System.Drawing.Point(181, 80);
            this.btnSetStrCardNumber.Name = "btnSetStrCardNumber";
            this.btnSetStrCardNumber.Size = new System.Drawing.Size(117, 23);
            this.btnSetStrCardNumber.TabIndex = 0;
            this.btnSetStrCardNumber.Text = "SetStrCardNumber";
            this.btnSetStrCardNumber.UseVisualStyleBackColor = true;
            this.btnSetStrCardNumber.Click += new System.EventHandler(this.btnSetStrCardNumber_Click);
            // 
            // label89
            // 
            this.label89.AutoSize = true;
            this.label89.Location = new System.Drawing.Point(332, 51);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(52, 13);
            this.label89.TabIndex = 67;
            this.label89.Text = "Enabled  ";
            // 
            // cbPrivilege
            // 
            this.cbPrivilege.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPrivilege.FormattingEnabled = true;
            this.cbPrivilege.Location = new System.Drawing.Point(90, 47);
            this.cbPrivilege.Name = "cbPrivilege";
            this.cbPrivilege.Size = new System.Drawing.Size(69, 21);
            this.cbPrivilege.TabIndex = 59;
            // 
            // txtCardnumber
            // 
            this.txtCardnumber.Location = new System.Drawing.Point(238, 47);
            this.txtCardnumber.Name = "txtCardnumber";
            this.txtCardnumber.Size = new System.Drawing.Size(76, 20);
            this.txtCardnumber.TabIndex = 61;
            // 
            // Privilege
            // 
            this.Privilege.Location = new System.Drawing.Point(30, 50);
            this.Privilege.Name = "Privilege";
            this.Privilege.Size = new System.Drawing.Size(61, 19);
            this.Privilege.TabIndex = 65;
            this.Privilege.Text = "Privilege";
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(174, 51);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(66, 13);
            this.label55.TabIndex = 66;
            this.label55.Text = "CardNumber";
            // 
            // label90
            // 
            this.label90.AutoSize = true;
            this.label90.Location = new System.Drawing.Point(328, 20);
            this.label90.Name = "label90";
            this.label90.Size = new System.Drawing.Size(53, 13);
            this.label90.TabIndex = 68;
            this.label90.Text = "Password";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.cbUserID);
            this.groupBox7.Controls.Add(this.label8);
            this.groupBox7.Controls.Add(this.label3);
            this.groupBox7.Controls.Add(this.cbTmpToWrite);
            this.groupBox7.Controls.Add(this.label28);
            this.groupBox7.Controls.Add(this.btnWriteCard);
            this.groupBox7.Controls.Add(this.btnEmptyCard);
            this.groupBox7.Controls.Add(this.label31);
            this.groupBox7.Location = new System.Drawing.Point(11, 192);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(462, 117);
            this.groupBox7.TabIndex = 41;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Write data into Mifare Card or Empty It";
            // 
            // cbUserID
            // 
            this.cbUserID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUserID.FormattingEnabled = true;
            this.cbUserID.Location = new System.Drawing.Point(55, 39);
            this.cbUserID.Name = "cbUserID";
            this.cbUserID.Size = new System.Drawing.Size(87, 21);
            this.cbUserID.TabIndex = 50;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.Crimson;
            this.label8.Location = new System.Drawing.Point(5, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(319, 13);
            this.label8.TabIndex = 47;
            this.label8.Text = "Please make sure your device has an optional mifare card module.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 33;
            this.label3.Text = "Count";
            // 
            // cbTmpToWrite
            // 
            this.cbTmpToWrite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTmpToWrite.FormattingEnabled = true;
            this.cbTmpToWrite.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.cbTmpToWrite.Location = new System.Drawing.Point(55, 66);
            this.cbTmpToWrite.Name = "cbTmpToWrite";
            this.cbTmpToWrite.Size = new System.Drawing.Size(54, 21);
            this.cbTmpToWrite.TabIndex = 32;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(5, 42);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(40, 13);
            this.label28.TabIndex = 15;
            this.label28.Text = "UserID";
            // 
            // btnWriteCard
            // 
            this.btnWriteCard.Location = new System.Drawing.Point(130, 89);
            this.btnWriteCard.Name = "btnWriteCard";
            this.btnWriteCard.Size = new System.Drawing.Size(75, 23);
            this.btnWriteCard.TabIndex = 23;
            this.btnWriteCard.Text = "WriteCard";
            this.btnWriteCard.UseVisualStyleBackColor = true;
            this.btnWriteCard.Click += new System.EventHandler(this.btnWriteCard_Click);
            // 
            // btnEmptyCard
            // 
            this.btnEmptyCard.Location = new System.Drawing.Point(230, 89);
            this.btnEmptyCard.Name = "btnEmptyCard";
            this.btnEmptyCard.Size = new System.Drawing.Size(75, 23);
            this.btnEmptyCard.TabIndex = 24;
            this.btnEmptyCard.Text = "EmptyCard";
            this.btnEmptyCard.UseVisualStyleBackColor = true;
            this.btnEmptyCard.Click += new System.EventHandler(this.btnEmptyCard_Click);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.ForeColor = System.Drawing.Color.Crimson;
            this.label31.Location = new System.Drawing.Point(106, 69);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(274, 13);
            this.label31.TabIndex = 26;
            this.label31.Text = "(Count of Fingerprint Templates to Write into Mifare Card)";
            // 
            // UserIDTimer
            // 
            this.UserIDTimer.Enabled = true;
            this.UserIDTimer.Tick += new System.EventHandler(this.UserIDTimer_Tick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbRTShow);
            this.groupBox3.Location = new System.Drawing.Point(12, 39);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(461, 147);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Show the RealTime Events Related with the Card";
            // 
            // lbRTShow
            // 
            this.lbRTShow.FormattingEnabled = true;
            this.lbRTShow.Location = new System.Drawing.Point(8, 16);
            this.lbRTShow.Name = "lbRTShow";
            this.lbRTShow.Size = new System.Drawing.Size(445, 121);
            this.lbRTShow.TabIndex = 4;
            // 
            // CardMaintenance
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(977, 479);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox7);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "CardMaintenance";
            this.Text = "Card Management";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Button btnWriteCard;
        private System.Windows.Forms.Button btnEmptyCard;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnGetStrCardNumber;
        private System.Windows.Forms.Button btnSetStrCardNumber;
        private System.Windows.Forms.ListView lvCard;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox chbEnabled;
        private System.Windows.Forms.Label label90;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label89;
        private System.Windows.Forms.ComboBox cbPrivilege;
        private System.Windows.Forms.TextBox txtCardnumber;
        private System.Windows.Forms.Label Privilege;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.ComboBox cbTmpToWrite;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Timer UserIDTimer;
        private System.Windows.Forms.ComboBox cbUserID;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox lbRTShow;
    }
}

