namespace WinConsoleClient
{
    partial class FrmMain
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
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.newToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.commToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.hostToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.getSchStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getJamRangeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.getEventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simulateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.processManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip2
            // 
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem1,
            this.commToolStripMenuItem1,
            this.hostToolStripMenuItem1,
            this.simulateToolStripMenuItem,
            this.processManagerToolStripMenuItem});
            this.menuStrip2.Location = new System.Drawing.Point(0, 0);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(563, 24);
            this.menuStrip2.TabIndex = 1;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // newToolStripMenuItem1
            // 
            this.newToolStripMenuItem1.Name = "newToolStripMenuItem1";
            this.newToolStripMenuItem1.Size = new System.Drawing.Size(46, 20);
            this.newToolStripMenuItem1.Text = "New";
            this.newToolStripMenuItem1.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // commToolStripMenuItem1
            // 
            this.commToolStripMenuItem1.Name = "commToolStripMenuItem1";
            this.commToolStripMenuItem1.Size = new System.Drawing.Size(58, 20);
            this.commToolStripMenuItem1.Text = "Comm";
            this.commToolStripMenuItem1.Click += new System.EventHandler(this.commToolStripMenuItem_Click);
            // 
            // hostToolStripMenuItem1
            // 
            this.hostToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.getSchStatusToolStripMenuItem,
            this.getJamRangeToolStripMenuItem1,
            this.getEventToolStripMenuItem});
            this.hostToolStripMenuItem1.Name = "hostToolStripMenuItem1";
            this.hostToolStripMenuItem1.Size = new System.Drawing.Size(46, 20);
            this.hostToolStripMenuItem1.Text = "Host";
            // 
            // getSchStatusToolStripMenuItem
            // 
            this.getSchStatusToolStripMenuItem.Name = "getSchStatusToolStripMenuItem";
            this.getSchStatusToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.getSchStatusToolStripMenuItem.Text = "GetSchStatus";
            this.getSchStatusToolStripMenuItem.Click += new System.EventHandler(this.getScheduleStatusToolStripMenuItem1_Click);
            // 
            // getJamRangeToolStripMenuItem1
            // 
            this.getJamRangeToolStripMenuItem1.Name = "getJamRangeToolStripMenuItem1";
            this.getJamRangeToolStripMenuItem1.Size = new System.Drawing.Size(161, 22);
            this.getJamRangeToolStripMenuItem1.Text = "GetJamRange";
            this.getJamRangeToolStripMenuItem1.Click += new System.EventHandler(this.getJamRangeToolStripMenuItem_Click);
            // 
            // getEventToolStripMenuItem
            // 
            this.getEventToolStripMenuItem.Name = "getEventToolStripMenuItem";
            this.getEventToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.getEventToolStripMenuItem.Text = "GetEvent";
            this.getEventToolStripMenuItem.Click += new System.EventHandler(this.getEventToolStripMenuItem_Click);
            // 
            // simulateToolStripMenuItem
            // 
            this.simulateToolStripMenuItem.Name = "simulateToolStripMenuItem";
            this.simulateToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.simulateToolStripMenuItem.Text = "Simulate";
            this.simulateToolStripMenuItem.Click += new System.EventHandler(this.simulateToolStripMenuItem_Click);
            // 
            // processManagerToolStripMenuItem
            // 
            this.processManagerToolStripMenuItem.Name = "processManagerToolStripMenuItem";
            this.processManagerToolStripMenuItem.Size = new System.Drawing.Size(114, 20);
            this.processManagerToolStripMenuItem.Text = "ProcessManager";
            this.processManagerToolStripMenuItem.Click += new System.EventHandler(this.processManagerToolStripMenuItem_Click);
            // 
            // FrmMain
            // 
            this.ClientSize = new System.Drawing.Size(563, 345);
            this.Controls.Add(this.menuStrip2);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip2;
            this.Name = "FrmMain";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem commToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem hostToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem getSchStatusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getJamRangeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem getEventToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simulateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem processManagerToolStripMenuItem;
    }
}

