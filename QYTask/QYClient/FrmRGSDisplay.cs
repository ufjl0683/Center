using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QYClient
{
    public partial class FrmRGSDisplay : Form
    {

        RemoteInterface.EventNotifyClient ncclient;
        public FrmRGSDisplay()
        {
            InitializeComponent();
            try
            {
                
                this.ds.Merge(Util.robj.get_rgs_conf_table());
                this.dataGridView1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            for (int i = 0; i < ds.tblRGS_Config.Rows.Count; i++)
            {
                try
                {
                   // dataGridView1.Rows[i].Cells["Connected"].Value = Util.robj.IsRGSConnected(dataGridView1.Rows[i].Cells["IP"].Value.ToString());
                    ds.tblRGS_Config[i].connected = Util.robj.IsRGSConnected(ds.tblRGS_Config[i].ip);
                   
                }
                catch(Exception ex)
                {
                   MessageBox.Show(ex.Message) ;
                }
            }
            this.dataGridView1.Refresh();

            try
            {
                ncclient = new RemoteInterface.EventNotifyClient(QYClient.Properties.Settings.Default.NotifyServerIP, QYClient.Properties.Settings.Default.NotifyServerPort, false);
                ncclient.OnConnect += new RemoteInterface.OnConnectEventHandler(ncclient_OnConnect);
                ncclient.OnEvent += new RemoteInterface.NotifyEventHandler(ncclient_OnEvent);
              
                    
            }
            catch (Exception ex)
            {
              
                 MessageBox.Show(ex.Message);
            }

        }

        void ncclient_OnConnect()
        {
           // throw new Exception("The method or operation is not implemented.");
            ncclient.RegistEvent(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.RGS_Connection_Event, "*", null));
            ncclient.RegistEvent(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.RGS_Display_Event, "*", null));
        }

        void ncclient_OnEvent(RemoteInterface.NotifyEventObject objEvent)
        {
            //throw new Exception("The method or operation is not implemented.");

            try
            {
                lock (ds)
                {
                    if (objEvent.type == RemoteInterface.EventEnumType.RGS_Connection_Event)
                    {
                        try
                        {
                            ds.tblRGS_Config.FindByipdisplay_part(objEvent.devip, 1).connected = (bool)objEvent.EventObj;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    //for (int i = 0; i < dataGridView1.RowCount; i++)
                    //{
                    //    try
                    //    {
                    //        dataGridView1.Rows[i].Cells["Connected"].Value = Util.robj.IsRGSConnected(dataGridView1.Rows[i].Cells["IP"].Value.ToString());
                    //    }
                    //    catch(Exception ex)
                    //    {
                    //        MessageBox.Show(ex.Message);
                    //    }
                    //}
                    else if (objEvent.type == RemoteInterface.EventEnumType.RGS_Display_Event)
                    {
                        //this.ds.Clear();
                        //this.ds.Merge(Util.robj.get_rgs_conf_table());

                        //  QYTask.Ds.
                        System.Data.DataTable tbl = (System.Data.DataTable)objEvent.EventObj;
                        foreach (System.Data.DataRow r in tbl.Rows)
                        {
                            QYTask.Ds.tblRGS_ConfigRow dsr = ds.tblRGS_Config.FindByipdisplay_part(r["ip"].ToString(), System.Convert.ToByte(r["display_part"]));
                            dsr.curr_icon = (byte)r["curr_icon"];
                            dsr.curr_msg1 = (string)r["curr_msg1"];
                            dsr.curr_msg2 = (string)r["curr_msg2"];
                        }
                        //  this.dataGridView1.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FrmRGSDisplay_FormClosing(object sender, FormClosingEventArgs e)
        {
            ncclient.close();
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}