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
        public RemoteInterface.HC.I_HC_Comm rhost;
        public FrmMain()
        {
            if(Environment.GetCommandLineArgs().Length==1)
                  rhost=(RemoteInterface.HC.I_HC_Comm)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_Comm),RemoteInterface.RemoteBuilder.getRemoteUri("10.21.50.4",(int)RemoteInterface.RemotingPortEnum.HOST,"Comm"));
            else
                  rhost = (RemoteInterface.HC.I_HC_Comm)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_Comm), RemoteInterface.RemoteBuilder.getRemoteUri(Environment.GetCommandLineArgs()[1], (int)RemoteInterface.RemotingPortEnum.HOST, "Comm"));

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
                Form f = new FrmConsole(fd.MfccId, fd.txIP.Text, fd.Port);
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

        private void getJamRangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                FrmJamRangeMonitor f = new FrmJamRangeMonitor();
                f.MdiParent = this;
                f.Show();

               // MessageBox.Show(rhost.getJamRangeString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void getEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                FrmJamRangeMonitor f = new FrmJamRangeMonitor("EVENTMGR");
                f.MdiParent = this;
                f.Show();

                // MessageBox.Show(rhost.getJamRangeString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void simulateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form f = new FrmSimulate();
            f.MdiParent = this;
            f.Show();
        }

        private void processManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FrmProcessManager().ShowDialog();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

        }
    }
}