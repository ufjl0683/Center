namespace WinConsoleClient
{
    partial class FrmNewDialog
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該公開 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.cbPort = new System.Windows.Forms.ComboBox();
            this.optConsole = new System.Windows.Forms.RadioButton();
            this.optMoniter = new System.Windows.Forms.RadioButton();
            this.txtDevName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.sd = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP Address:";
            // 
            // txIP
            // 
            this.txIP.Location = new System.Drawing.Point(100, 23);
            this.txIP.Name = "txIP";
            this.txIP.Size = new System.Drawing.Size(138, 22);
            this.txIP.TabIndex = 1;
            this.txIP.Text = "10.21.50.4";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(67, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port:";
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(306, 32);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(306, 65);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // cbPort
            // 
            this.cbPort.FormattingEnabled = true;
            this.cbPort.Location = new System.Drawing.Point(101, 53);
            this.cbPort.Name = "cbPort";
            this.cbPort.Size = new System.Drawing.Size(137, 20);
            this.cbPort.TabIndex = 6;
            this.cbPort.SelectedIndexChanged += new System.EventHandler(this.cbPort_SelectedIndexChanged);
            // 
            // optConsole
            // 
            this.optConsole.AutoSize = true;
            this.optConsole.Checked = true;
            this.optConsole.Location = new System.Drawing.Point(101, 135);
            this.optConsole.Name = "optConsole";
            this.optConsole.Size = new System.Drawing.Size(61, 16);
            this.optConsole.TabIndex = 7;
            this.optConsole.TabStop = true;
            this.optConsole.Text = "Console";
            this.optConsole.UseVisualStyleBackColor = true;
            // 
            // optMoniter
            // 
            this.optMoniter.AutoSize = true;
            this.optMoniter.Location = new System.Drawing.Point(178, 134);
            this.optMoniter.Name = "optMoniter";
            this.optMoniter.Size = new System.Drawing.Size(60, 16);
            this.optMoniter.TabIndex = 8;
            this.optMoniter.Text = "Moniter";
            this.optMoniter.UseVisualStyleBackColor = true;
            // 
            // txtDevName
            // 
            this.txtDevName.Location = new System.Drawing.Point(100, 89);
            this.txtDevName.Name = "txtDevName";
            this.txtDevName.Size = new System.Drawing.Size(138, 22);
            this.txtDevName.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "DevName:";
            // 
            // FrmNewDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button1;
            this.ClientSize = new System.Drawing.Size(393, 185);
            this.Controls.Add(this.txtDevName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.optMoniter);
            this.Controls.Add(this.optConsole);
            this.Controls.Add(this.cbPort);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txIP);
            this.Controls.Add(this.label1);
            this.Name = "FrmNewDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmNewDialog";
            this.Load += new System.EventHandler(this.FrmNewDialog_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmNewDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.TextBox txIP;
        private System.Windows.Forms.ComboBox cbPort;
        public System.Windows.Forms.RadioButton optConsole;
        public System.Windows.Forms.RadioButton optMoniter;
        public System.Windows.Forms.TextBox txtDevName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SaveFileDialog sd;
    }
}