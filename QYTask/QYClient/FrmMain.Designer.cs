namespace QYClient
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
            this.mnuSystem = new System.Windows.Forms.MenuStrip();
            this.系統SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.監視MToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.最近五分鐘路段資料ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rGS狀態ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rMS狀態ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.rGS輸入設定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rMS輸入設定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWindosw = new System.Windows.Forms.ToolStripMenuItem();
            this.rGS顯示狀態ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSystem.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuSystem
            // 
            this.mnuSystem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.系統SToolStripMenuItem,
            this.監視MToolStripMenuItem,
            this.toolStripMenuItem4,
            this.mnuWindosw});
            this.mnuSystem.Location = new System.Drawing.Point(0, 0);
            this.mnuSystem.MdiWindowListItem = this.mnuWindosw;
            this.mnuSystem.Name = "mnuSystem";
            this.mnuSystem.Size = new System.Drawing.Size(740, 24);
            this.mnuSystem.TabIndex = 1;
            this.mnuSystem.Text = "menuStrip1";
            // 
            // 系統SToolStripMenuItem
            // 
            this.系統SToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripSeparator1,
            this.toolStripMenuItem5,
            this.toolStripSeparator2,
            this.toolStripMenuItem1});
            this.系統SToolStripMenuItem.Name = "系統SToolStripMenuItem";
            this.系統SToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.系統SToolStripMenuItem.Text = "系統[&S]";
            this.系統SToolStripMenuItem.Click += new System.EventHandler(this.系統SToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(131, 22);
            this.toolStripMenuItem2.Text = "RGS 配置表";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(131, 22);
            this.toolStripMenuItem3.Text = "Icon配置表";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(128, 6);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(131, 22);
            this.toolStripMenuItem5.Text = "RMS配置表";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(128, 6);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(131, 22);
            this.toolStripMenuItem1.Text = "離開";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // 監視MToolStripMenuItem
            // 
            this.監視MToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.最近五分鐘路段資料ToolStripMenuItem,
            this.rGS狀態ToolStripMenuItem,
            this.rMS狀態ToolStripMenuItem,
            this.rGS顯示狀態ToolStripMenuItem});
            this.監視MToolStripMenuItem.Name = "監視MToolStripMenuItem";
            this.監視MToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.監視MToolStripMenuItem.Text = "監視[&M]";
            // 
            // 最近五分鐘路段資料ToolStripMenuItem
            // 
            this.最近五分鐘路段資料ToolStripMenuItem.Name = "最近五分鐘路段資料ToolStripMenuItem";
            this.最近五分鐘路段資料ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.最近五分鐘路段資料ToolStripMenuItem.Text = "旅行時間";
            this.最近五分鐘路段資料ToolStripMenuItem.Click += new System.EventHandler(this.最近五分鐘路段資料ToolStripMenuItem_Click);
            // 
            // rGS狀態ToolStripMenuItem
            // 
            this.rGS狀態ToolStripMenuItem.Name = "rGS狀態ToolStripMenuItem";
            this.rGS狀態ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.rGS狀態ToolStripMenuItem.Text = "RGS 狀態";
            this.rGS狀態ToolStripMenuItem.Click += new System.EventHandler(this.rGS狀態ToolStripMenuItem_Click);
            // 
            // rMS狀態ToolStripMenuItem
            // 
            this.rMS狀態ToolStripMenuItem.Name = "rMS狀態ToolStripMenuItem";
            this.rMS狀態ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.rMS狀態ToolStripMenuItem.Text = "RMS 狀態";
            this.rMS狀態ToolStripMenuItem.Click += new System.EventHandler(this.rMS狀態ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rGS輸入設定ToolStripMenuItem,
            this.rMS輸入設定ToolStripMenuItem});
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(56, 20);
            this.toolStripMenuItem4.Text = "設定[&T]";
            // 
            // rGS輸入設定ToolStripMenuItem
            // 
            this.rGS輸入設定ToolStripMenuItem.Name = "rGS輸入設定ToolStripMenuItem";
            this.rGS輸入設定ToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.rGS輸入設定ToolStripMenuItem.Text = "RGS輸入設定";
            this.rGS輸入設定ToolStripMenuItem.Click += new System.EventHandler(this.rGS輸入設定ToolStripMenuItem_Click);
            // 
            // rMS輸入設定ToolStripMenuItem
            // 
            this.rMS輸入設定ToolStripMenuItem.Name = "rMS輸入設定ToolStripMenuItem";
            this.rMS輸入設定ToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.rMS輸入設定ToolStripMenuItem.Text = "RMS輸入設定";
            this.rMS輸入設定ToolStripMenuItem.Click += new System.EventHandler(this.rMS輸入設定ToolStripMenuItem_Click);
            // 
            // mnuWindosw
            // 
            this.mnuWindosw.Name = "mnuWindosw";
            this.mnuWindosw.Size = new System.Drawing.Size(60, 20);
            this.mnuWindosw.Text = "視窗[&W]";
            // 
            // rGS顯示狀態ToolStripMenuItem
            // 
            this.rGS顯示狀態ToolStripMenuItem.Name = "rGS顯示狀態ToolStripMenuItem";
            this.rGS顯示狀態ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.rGS顯示狀態ToolStripMenuItem.Text = "RGS 顯示狀態";
            this.rGS顯示狀態ToolStripMenuItem.Click += new System.EventHandler(this.rGS顯示狀態ToolStripMenuItem_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 505);
            this.Controls.Add(this.mnuSystem);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.mnuSystem;
            this.Name = "FrmMain";
            this.Text = "暫行系統";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.mnuSystem.ResumeLayout(false);
            this.mnuSystem.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuSystem;
        private System.Windows.Forms.ToolStripMenuItem 系統SToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 監視MToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 最近五分鐘路段資料ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuWindosw;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem rGS輸入設定ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rGS狀態ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem rMS狀態ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rMS輸入設定ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rGS顯示狀態ToolStripMenuItem;
    }
}

