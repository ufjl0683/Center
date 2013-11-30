using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RemoteInterface;

namespace WinConsoleClient
{
    public partial class FrmCommDialog : Form
    {
        RemoteInterface.HC.I_HC_Comm robj;
        public FrmCommDialog()
        {
            InitializeComponent();
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            robj = (RemoteInterface.HC.I_HC_Comm)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_Comm),
             RemoteInterface.RemoteBuilder.getRemoteUri(txtHostIp.Text.Trim(), (int)RemotingPortEnum.HOST, "Comm"));
           
            if (this.radioButton1.Checked)
            {
                try
                {
                    MessageBox.Show(robj.getTCCommStatusStr(this.cbDevName.Text.Trim()));
                     
                  //  MessageBox.Show(robj.getCurrentDBQueueCnt().ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else if (this.radioButton2.Checked)
            {
                try
                {
                    robj.ResetComm(this.cbDevName.Text.Trim());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void FrmCommDialog_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            robj = (RemoteInterface.HC.I_HC_Comm)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_Comm),
             RemoteInterface.RemoteBuilder.getRemoteUri(txtHostIp.Text.Trim(), (int)RemotingPortEnum.HOST, "Comm"));
         this.cbDevName.DataSource= robj.getDeviceNames(txtDevType.Text);
            
        }
    }
}