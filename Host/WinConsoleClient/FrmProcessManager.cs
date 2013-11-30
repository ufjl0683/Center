using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinConsoleClient
{
    public partial class FrmProcessManager : Form
    {
        RemoteInterface.I_ProcessManager robj;
        public FrmProcessManager()
        {
            InitializeComponent();

          
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            robj = RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.I_ProcessManager),
              RemoteInterface.RemoteBuilder.getRemoteUri(txtIp.Text, (int)RemoteInterface.RemotingPortEnum.ProcessManager, RemoteInterface.RemotingPortEnum.ProcessManager.ToString())) as RemoteInterface.I_ProcessManager;

          System.Data.DataSet ds=new DataSet();
          ds.Merge(robj.getProcessStatus());
          this.dataGridView1.SuspendLayout();
          this.dataGridView1.DataSource = ds;
          this.dataGridView1.DataMember = "tblProcessInfo";
          this.dataGridView1.ResumeLayout();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.BindingContext[dataGridView1.DataSource, "tblProcessInfo"].Position == -1)
                return;

           System.Data.DataRowView o = this.BindingContext[dataGridView1.DataSource, "tblProcessInfo"].Current as System.Data.DataRowView;

         robj.SetProcessRunningState(   o["process_name"].ToString(),1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.BindingContext[dataGridView1.DataSource, "tblProcessInfo"].Position == -1)
                return;

            System.Data.DataRowView o = this.BindingContext[dataGridView1.DataSource, "tblProcessInfo"].Current as System.Data.DataRowView;

            robj.SetProcessRunningState(o["process_name"].ToString(), 0);
        }
    }
}