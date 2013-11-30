using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinConsoleClient
{
    public partial class FrmJamRangeMonitor : Form
    {
        string type = "JAMMGR";
        public FrmJamRangeMonitor()
        {
            InitializeComponent();

           
        }


        public FrmJamRangeMonitor(string type)
            : this()
        {
            this.type = type;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                this.Text = DateTime.Now.ToString();
                if (type == "JAMMGR")
                    this.textBox1.Text = (this.MdiParent as FrmMain).rhost.getJamRangeString();
                else if (type == "EVENTMGR")
                    this.textBox1.Text = (this.MdiParent as FrmMain).rhost.getEventString();
                else if (type == "REDIRECTSTATUS")
                    this.textBox1.Text = (this.MdiParent as FrmMain).rhost.getRedirectStatusString();
                else if (type == "ALLVDDEVICEINFO")
                    this.textBox1.Text = (this.MdiParent as FrmMain).rhost.GetAllVDInfo();
                else if (type == "TIMCCSECTIONMGR")
                    this.textBox1.Text = (this.MdiParent as FrmMain).rhost.getTIMCCSecManagerStatus();
                    
            }
            catch (Exception ex)
            {
                textBox1.Text = ex.Message;
            }

        }

        private void FrmJamRangeMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.timer1.Enabled = false;
        }

        private void FrmJamRangeMonitor_Activated(object sender, EventArgs e)
        {
            timer1_Tick(this, null);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void FrmJamRangeMonitor_Load(object sender, EventArgs e)
        {

        }
    }
}