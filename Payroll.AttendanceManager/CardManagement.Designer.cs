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
            this.GridView = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FullName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CardNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Priv = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EnrolledToRfid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Enabled = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UserIDTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.chbEnabled = new System.Windows.Forms.CheckBox();
            this.btnSetStrCardNumber = new System.Windows.Forms.Button();
            this.label89 = new System.Windows.Forms.Label();
            this.txtCardnumber = new System.Windows.Forms.TextBox();
            this.label55 = new System.Windows.Forms.Label();
            rtTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).BeginInit();
            this.groupBox5.SuspendLayout();
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
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(958, 450);
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
            this.label4.Size = new System.Drawing.Size(422, 18);
            this.label4.TabIndex = 46;
            this.label4.Text = "Please make sure your device has an optional ID card module. ";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.GridView);
            this.groupBox4.Location = new System.Drawing.Point(6, 52);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(946, 382);
            this.groupBox4.TabIndex = 43;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Download the Card Number(A property of user information)";
            // 
            // GridView
            // 
            this.GridView.AllowUserToAddRows = false;
            this.GridView.AllowUserToDeleteRows = false;
            this.GridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.FullName,
            this.CardNumber,
            this.Priv,
            this.EnrolledToRfid,
            this.Enabled});
            this.GridView.Location = new System.Drawing.Point(16, 23);
            this.GridView.Name = "GridView";
            this.GridView.ReadOnly = true;
            this.GridView.Size = new System.Drawing.Size(914, 353);
            this.GridView.TabIndex = 47;
            this.GridView.DoubleClick += new System.EventHandler(this.GridView_DoubleClick);
            // 
            // Id
            // 
            this.Id.DataPropertyName = "EmployeeId";
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            // 
            // FullName
            // 
            this.FullName.DataPropertyName = "FullName";
            this.FullName.HeaderText = "Name";
            this.FullName.Name = "FullName";
            this.FullName.ReadOnly = true;
            this.FullName.Width = 300;
            // 
            // CardNumber
            // 
            this.CardNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.CardNumber.DataPropertyName = "EmployeeCode";
            this.CardNumber.HeaderText = "Card Number";
            this.CardNumber.Name = "CardNumber";
            this.CardNumber.ReadOnly = true;
            this.CardNumber.Width = 122;
            // 
            // Priv
            // 
            this.Priv.DataPropertyName = "Privilege";
            this.Priv.HeaderText = "Privilege";
            this.Priv.Name = "Priv";
            this.Priv.ReadOnly = true;
            this.Priv.Width = 130;
            // 
            // EnrolledToRfid
            // 
            this.EnrolledToRfid.DataPropertyName = "EnrolledToRfid";
            this.EnrolledToRfid.HeaderText = "Enrolled";
            this.EnrolledToRfid.Name = "EnrolledToRfid";
            this.EnrolledToRfid.ReadOnly = true;
            // 
            // Enabled
            // 
            this.Enabled.DataPropertyName = "Enabled";
            this.Enabled.HeaderText = "Enabled";
            this.Enabled.Name = "Enabled";
            this.Enabled.ReadOnly = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtUserID);
            this.groupBox5.Controls.Add(this.txtName);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.chbEnabled);
            this.groupBox5.Controls.Add(this.btnSetStrCardNumber);
            this.groupBox5.Controls.Add(this.label89);
            this.groupBox5.Controls.Add(this.txtCardnumber);
            this.groupBox5.Controls.Add(this.label55);
            this.groupBox5.Location = new System.Drawing.Point(12, 468);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(681, 111);
            this.groupBox5.TabIndex = 45;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Upload the Card Number(part of users information)";
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(72, 30);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.ReadOnly = true;
            this.txtUserID.Size = new System.Drawing.Size(214, 24);
            this.txtUserID.TabIndex = 56;
            this.txtUserID.Text = "1";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(72, 73);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(214, 24);
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
            this.label15.Size = new System.Drawing.Size(48, 18);
            this.label15.TabIndex = 64;
            this.label15.Text = "Name";
            // 
            // chbEnabled
            // 
            this.chbEnabled.AutoSize = true;
            this.chbEnabled.Checked = true;
            this.chbEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbEnabled.Location = new System.Drawing.Point(404, 40);
            this.chbEnabled.Name = "chbEnabled";
            this.chbEnabled.Size = new System.Drawing.Size(15, 14);
            this.chbEnabled.TabIndex = 69;
            this.chbEnabled.UseVisualStyleBackColor = true;
            // 
            // btnSetStrCardNumber
            // 
            this.btnSetStrCardNumber.Location = new System.Drawing.Point(553, 21);
            this.btnSetStrCardNumber.Name = "btnSetStrCardNumber";
            this.btnSetStrCardNumber.Size = new System.Drawing.Size(122, 73);
            this.btnSetStrCardNumber.TabIndex = 0;
            this.btnSetStrCardNumber.Text = "Register and Save";
            this.btnSetStrCardNumber.UseVisualStyleBackColor = true;
            this.btnSetStrCardNumber.Click += new System.EventHandler(this.btnSetStrCardNumber_Click);
            // 
            // label89
            // 
            this.label89.AutoSize = true;
            this.label89.Location = new System.Drawing.Point(301, 36);
            this.label89.Name = "label89";
            this.label89.Size = new System.Drawing.Size(69, 18);
            this.label89.TabIndex = 67;
            this.label89.Text = "Enabled  ";
            // 
            // txtCardnumber
            // 
            this.txtCardnumber.Location = new System.Drawing.Point(404, 70);
            this.txtCardnumber.Name = "txtCardnumber";
            this.txtCardnumber.Size = new System.Drawing.Size(143, 24);
            this.txtCardnumber.TabIndex = 61;
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(301, 76);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(97, 18);
            this.label55.TabIndex = 66;
            this.label55.Text = "Card Number";
            // 
            // CardMaintenance
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(977, 589);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "CardMaintenance";
            this.Text = "Card Management";
            this.Load += new System.EventHandler(this.CardMaintenance_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Timer UserIDTimer;
        private System.Windows.Forms.DataGridView GridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn FullName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CardNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn Priv;
        private System.Windows.Forms.DataGridViewTextBoxColumn EnrolledToRfid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Enabled;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox chbEnabled;
        private System.Windows.Forms.Button btnSetStrCardNumber;
        private System.Windows.Forms.Label label89;
        private System.Windows.Forms.TextBox txtCardnumber;
        private System.Windows.Forms.Label label55;
    }
}

