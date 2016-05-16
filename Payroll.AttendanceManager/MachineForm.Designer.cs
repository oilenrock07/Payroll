namespace AttendanceManager
{
    partial class MachineForm
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
            this.GridView = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FirstName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CardNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Enrolled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnRegisterAll = new System.Windows.Forms.Button();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.btnSetStrCardNumber = new System.Windows.Forms.Button();
            this.txtCardnumber = new System.Windows.Forms.TextBox();
            this.label55 = new System.Windows.Forms.Label();
            this.lblIpAddress = new System.Windows.Forms.Label();
            this.lblState = new System.Windows.Forms.Label();
            this.lbRTShow = new System.Windows.Forms.ListBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.chkShowNotRegistered = new System.Windows.Forms.CheckBox();
            this.btnReload = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // GridView
            // 
            this.GridView.AllowUserToAddRows = false;
            this.GridView.AllowUserToDeleteRows = false;
            this.GridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.FirstName,
            this.LastName,
            this.CardNumber,
            this.Enrolled});
            this.GridView.Location = new System.Drawing.Point(13, 44);
            this.GridView.Margin = new System.Windows.Forms.Padding(4);
            this.GridView.Name = "GridView";
            this.GridView.ReadOnly = true;
            this.GridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GridView.Size = new System.Drawing.Size(764, 314);
            this.GridView.TabIndex = 48;
            this.GridView.DoubleClick += new System.EventHandler(this.GridView_DoubleClick);
            // 
            // Id
            // 
            this.Id.DataPropertyName = "EmployeeId";
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            // 
            // FirstName
            // 
            this.FirstName.DataPropertyName = "FirstName";
            this.FirstName.HeaderText = "FirstName";
            this.FirstName.Name = "FirstName";
            this.FirstName.ReadOnly = true;
            this.FirstName.Width = 150;
            // 
            // LastName
            // 
            this.LastName.DataPropertyName = "LastName";
            this.LastName.HeaderText = "LastName";
            this.LastName.Name = "LastName";
            this.LastName.ReadOnly = true;
            this.LastName.Width = 150;
            // 
            // CardNumber
            // 
            this.CardNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.CardNumber.DataPropertyName = "EmployeeCode";
            this.CardNumber.HeaderText = "Card Number";
            this.CardNumber.Name = "CardNumber";
            this.CardNumber.ReadOnly = true;
            this.CardNumber.Width = 113;
            // 
            // Enrolled
            // 
            this.Enrolled.DataPropertyName = "Enrolled";
            this.Enrolled.HeaderText = "Enrolled";
            this.Enrolled.Name = "Enrolled";
            this.Enrolled.ReadOnly = true;
            this.Enrolled.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Enrolled.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(465, 31);
            this.label1.TabIndex = 49;
            this.label1.Text = "Employees registered to this machine";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(780, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 24);
            this.label2.TabIndex = 51;
            this.label2.Text = "Status:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnRegisterAll);
            this.groupBox5.Controls.Add(this.txtUserID);
            this.groupBox5.Controls.Add(this.txtName);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.btnSetStrCardNumber);
            this.groupBox5.Controls.Add(this.txtCardnumber);
            this.groupBox5.Controls.Add(this.label55);
            this.groupBox5.Location = new System.Drawing.Point(14, 412);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(764, 126);
            this.groupBox5.TabIndex = 52;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Card Details";
            // 
            // btnRegisterAll
            // 
            this.btnRegisterAll.Location = new System.Drawing.Point(556, 17);
            this.btnRegisterAll.Name = "btnRegisterAll";
            this.btnRegisterAll.Size = new System.Drawing.Size(202, 48);
            this.btnRegisterAll.TabIndex = 67;
            this.btnRegisterAll.Text = "Register All";
            this.btnRegisterAll.UseVisualStyleBackColor = true;
            this.btnRegisterAll.Click += new System.EventHandler(this.btnRegisterAll_Click);
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(72, 30);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.ReadOnly = true;
            this.txtUserID.Size = new System.Drawing.Size(214, 22);
            this.txtUserID.TabIndex = 56;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(72, 73);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(214, 22);
            this.txtName.TabIndex = 57;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(13, 36);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(62, 18);
            this.label16.TabIndex = 63;
            this.label16.Text = "User ID";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(19, 79);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(45, 16);
            this.label15.TabIndex = 64;
            this.label15.Text = "Name";
            // 
            // btnSetStrCardNumber
            // 
            this.btnSetStrCardNumber.Location = new System.Drawing.Point(556, 70);
            this.btnSetStrCardNumber.Name = "btnSetStrCardNumber";
            this.btnSetStrCardNumber.Size = new System.Drawing.Size(202, 45);
            this.btnSetStrCardNumber.TabIndex = 0;
            this.btnSetStrCardNumber.Text = "Register and Save";
            this.btnSetStrCardNumber.UseVisualStyleBackColor = true;
            this.btnSetStrCardNumber.Click += new System.EventHandler(this.btnSetStrCardNumber_Click);
            // 
            // txtCardnumber
            // 
            this.txtCardnumber.Location = new System.Drawing.Point(402, 32);
            this.txtCardnumber.Name = "txtCardnumber";
            this.txtCardnumber.ReadOnly = true;
            this.txtCardnumber.Size = new System.Drawing.Size(143, 22);
            this.txtCardnumber.TabIndex = 61;
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(308, 36);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(88, 16);
            this.label55.TabIndex = 66;
            this.label55.Text = "Card Number";
            // 
            // lblIpAddress
            // 
            this.lblIpAddress.AutoSize = true;
            this.lblIpAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIpAddress.ForeColor = System.Drawing.Color.Red;
            this.lblIpAddress.Location = new System.Drawing.Point(1072, 17);
            this.lblIpAddress.Name = "lblIpAddress";
            this.lblIpAddress.Size = new System.Drawing.Size(145, 24);
            this.lblIpAddress.TabIndex = 54;
            this.lblIpAddress.Text = "192.168.254.100";
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblState.ForeColor = System.Drawing.Color.Blue;
            this.lblState.Location = new System.Drawing.Point(838, 16);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(126, 24);
            this.lblState.TabIndex = 55;
            this.lblState.Text = "Disconnected";
            // 
            // lbRTShow
            // 
            this.lbRTShow.FormattingEnabled = true;
            this.lbRTShow.ItemHeight = 16;
            this.lbRTShow.Location = new System.Drawing.Point(784, 44);
            this.lbRTShow.Name = "lbRTShow";
            this.lbRTShow.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbRTShow.Size = new System.Drawing.Size(355, 500);
            this.lbRTShow.TabIndex = 56;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(970, 9);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(87, 28);
            this.btnConnect.TabIndex = 57;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // chkShowNotRegistered
            // 
            this.chkShowNotRegistered.AutoSize = true;
            this.chkShowNotRegistered.Location = new System.Drawing.Point(14, 376);
            this.chkShowNotRegistered.Name = "chkShowNotRegistered";
            this.chkShowNotRegistered.Size = new System.Drawing.Size(184, 20);
            this.chkShowNotRegistered.TabIndex = 58;
            this.chkShowNotRegistered.Text = "Show Not Registered Only";
            this.chkShowNotRegistered.UseVisualStyleBackColor = true;
            this.chkShowNotRegistered.CheckedChanged += new System.EventHandler(this.chkShowNotRegistered_CheckedChanged);
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(576, 365);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(202, 48);
            this.btnReload.TabIndex = 68;
            this.btnReload.Text = "Reload Grid";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1145, 44);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 54);
            this.button1.TabIndex = 69;
            this.button1.Text = "Clear and Export";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MachineForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1229, 587);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.chkShowNotRegistered);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.lbRTShow);
            this.Controls.Add(this.lblState);
            this.Controls.Add(this.lblIpAddress);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GridView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MachineForm";
            this.Text = "MachineForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MachineForm_FormClosing);
            this.Load += new System.EventHandler(this.Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView GridView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button btnSetStrCardNumber;
        private System.Windows.Forms.TextBox txtCardnumber;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.Label lblIpAddress;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.ListBox lbRTShow;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnRegisterAll;
        private System.Windows.Forms.CheckBox chkShowNotRegistered;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn FirstName;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CardNumber;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Enrolled;
        private System.Windows.Forms.Button button1;
    }
}