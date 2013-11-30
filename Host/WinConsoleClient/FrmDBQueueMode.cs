using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinConsoleClient
{
    public partial class FrmDBQueueMode : Form
    {
        public FrmDBQueueMode()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (radioButton1.Checked)
                    (this.Owner as FrmMain).rhost.SetDbqMode(RemoteInterface.HC.DBQueueMode.Normal);
                else if (radioButton2.Checked)
                    (this.Owner as FrmMain).rhost.SetDbqMode(RemoteInterface.HC.DBQueueMode.Slow);
                else
                    (this.Owner as FrmMain).rhost.SetDbqMode(RemoteInterface.HC.DBQueueMode.Reject);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
