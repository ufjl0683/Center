using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinConsoleClient
{
    public partial class FrmMain : Form
    {
         RemoteInterface.HC.I_HC_Comm rhost=(RemoteInterface.HC.I_HC_Comm)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_Comm),RemoteInterface.RemoteBuilder.getRemoteUri("10.21.50.4",(int)RemoteInterface.RemotingPortEnum.HOST,"Comm"));
        public FrmMain()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmNewDialog fd = new FrmNewDialog();
            fd.ShowDialog();
            if (fd.DialogResult != DialogResult.OK)
                return;
            if (fd.optConsole.Checked)
            {
                Form f = new Form2(fd.MfccId, fd.txIP.Text, fd.Port);
                f.MdiParent = this;
                f.Show();
            }
            else
            {
                Form f = new FrmMoniter(fd.MfccId, fd.txIP.Text, fd.Port,fd.txtDevName.Text);
                f.MdiParent = this;
                f.Show();
            }
            
        }

        private void commToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form f = new FrmCommDialog();
            f.MdiParent = this;
            f.Show();
        }

       

        private void getScheduleStatusToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            MessageBox.Show(rhost.getAllSchduleStatus());
        }
    }
}