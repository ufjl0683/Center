using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QYClient
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();

           // Sy.mnuWindosw.

           
        }

        private void 離開ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void 系統SToolStripMenuItem_Click(object sender, EventArgs e)
        {
         
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("確定離開", "暫行系統", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                e.Cancel = true;
           
              
        }

      

        private void 最近五分鐘路段資料ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (IsFormInRun("FrmCurrentSectionData"))
                return;
            try
            {
                Form f = new FrmCurrentSectionData();
                f.MdiParent = this;
                f.Show();
            }
            catch 
            {
                ;
            }

        }

        private bool IsFormInRun(string fname )
        {
             foreach (System.Windows.Forms.Form f in this.MdiChildren)
            {
                if (f.Name == fname)
                {
                    f.Activate();

                    f.WindowState = FormWindowState.Normal;
                    return true;
                }
            }
            return false;
        }


        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

            if (IsFormInRun("FrmRGSConfig"))
                return;
            
            try
            {
                Form f = new FrmRGSConfig();
                f.MdiParent = this;
                f.Show();
            }
            catch
            {
                ;
            }

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch
            {
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {


            if (IsFormInRun("FrmIcons"))
                return;
            Form f = new FrmIcons();
            f.MdiParent = this;
            f.Show();
        }

        private void rGS訊息設定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void rGS輸入設定ToolStripMenuItem_Click(object sender, EventArgs e)
        {


            if (IsFormInRun("FrmRGSCustomInput"))
                return;
            Form f = new FrmRGSCustomInput();
            f.MdiParent = this;
            f.Show();
        }

        private void rGS狀態ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (IsFormInRun("FrmRGSStatus"))
                return;
            
            Form f = new FrmRGSStatus();
            f.MdiParent = this;
            f.Show();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {

            if (IsFormInRun("FrmRmsConfig"))
                return;
            Form f = new FrmRmsConfig();
            f.MdiParent = this;
            f.Show();
        }

        private void rMS狀態ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (IsFormInRun("FrmRmsStatus"))
                return;
            Form f = new FrmRmsStatus();
            f.MdiParent = this;
            f.Show();
        }

        private void rMS輸入設定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsFormInRun("FrmRmsModeSetting"))
                return;
            
            Form f = new FrmRmsModeSetting();
            f.MdiParent = this;
            f.Show();
        }

        private void rGS顯示狀態ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsFormInRun("FrmRmsModeSetting"))
                return;

            Form f = new FrmRGSDisplay();
            f.MdiParent = this;
            f.Show();
        }
    }
}