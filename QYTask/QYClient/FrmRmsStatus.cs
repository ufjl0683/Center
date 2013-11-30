using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RemoteInterface;

namespace QYClient
{
    public partial class FrmRmsStatus : Form
    {
        DataSet ds1;
        RemoteInterface.EventNotifyClient nclient;
        public FrmRmsStatus()
        {
            InitializeComponent();
            try
            {
                this.ds.tblRMSMode.AddtblRMSModeRow(0, "固定時制");
                // this.ds.tblRMSMode.AddtblRMSModeRow(1, "區域交通反應");
                this.ds.tblRMSMode.AddtblRMSModeRow(2, "預設時制");
               this.ds.Merge(Util.robj.get_rms_config());
               this.dataGridView1.DataSource = ds;
               this.dataGridView1.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                nclient = new RemoteInterface.EventNotifyClient(QYClient.Properties.Settings.Default.NotifyServerIP, QYClient.Properties.Settings.Default.NotifyServerPort, false);
                nclient.OnConnect += new RemoteInterface.OnConnectEventHandler(nclient_OnConnect);
               
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        
        }

        void nclient_OnConnect()
        {
            nclient.RegistEvent(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.RMS_Connection_Event, "*", null));
            nclient.RegistEvent(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.RMS_HW_Status_Event, "*", null));
            nclient.RegistEvent(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.RMS_Mode_Change_Event, "*", null));
            nclient.OnEvent += new RemoteInterface.NotifyEventHandler(nclient_OnEvent);
            //throw new Exception("The method or operation is not implemented.");
        }

        void nclient_OnEvent(RemoteInterface.NotifyEventObject objEvent)
        {
            //throw new Exception("The method or operation is not implemented.");
            if (objEvent.type == RemoteInterface.EventEnumType.RMS_Connection_Event)
            {
                this.ds.tblRmsConfig.FindByip(objEvent.devip).connected = (bool)objEvent.EventObj;
            }
            else if (objEvent.type == RemoteInterface.EventEnumType.RMS_HW_Status_Event)
            {
               I_HW_Status_Desc hw_desc = (I_HW_Status_Desc)objEvent.EventObj;
                QYTask.Ds.tblRmsConfigRow r = ds.tblRmsConfig.FindByip(objEvent.devip);
                System.Collections.IEnumerator ie = hw_desc.getEnum().GetEnumerator();
                while (ie.MoveNext())
                {
                    if ((int)ie.Current == 1 || (int)ie.Current == 0 || (int)ie.Current == 8 || (int)ie.Current == 9 || (int)ie.Current == 10 || (int)ie.Current == 11)
                        r[hw_desc.getDesc((int)ie.Current)] = hw_desc.getStatus((int)ie.Current);
                    r.EndEdit();
                    r.AcceptChanges();
                }

            }
            else if (objEvent.type == RemoteInterface.EventEnumType.RMS_Mode_Change_Event)
            {
                System.Data.DataTable tbl = (System.Data.DataTable)objEvent.EventObj;

                foreach (DataRow r in tbl.Rows)
                {
                    ds.tblRmsConfig.FindByip(r["ip"].ToString()).ctl_mode =(uint) r["ctl_mode"];
                    ds.tblRmsConfig.FindByip(r["ip"].ToString()).planno=(uint)r["planno"];
                    ds.tblRmsConfig.FindByip(r["ip"].ToString()).EndEdit();
                    r.AcceptChanges();
                    
                }
            }
        }

        private void FrmRmsStatus_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(nclient!=null)
            this.nclient.close();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "ctl_mode")
            {
                switch (System.Convert.ToInt32(e.Value))
                {
                    case 0:  //fixed mode
                        e.Value = "固定";
                        e.FormattingApplied = true;
                        break;
                    case 2://default mode
                        e.Value = "預設";
                        e.FormattingApplied = true;
                        break;
                }
            }

        }
    }
}