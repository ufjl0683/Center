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
    public partial class FrmRGSStatus : Form
    {
        RemoteInterface.EventNotifyClient nclient;
        public FrmRGSStatus()
        {
            InitializeComponent();
            try
            {
                this.ds.Merge(Util.robj.get_all_rgs_display_status());
            }
            catch (Exception ex)
            {
              MessageBox.Show( ex.Message);
            }
            try
            {
                this.dgv.DataSource = this.dgv.DataSource;
                this.dgv.Refresh();
            }
            catch
            {
                ;
            }

            nclient = new RemoteInterface.EventNotifyClient(QYClient.Properties.Settings.Default.NotifyServerIP,
                QYClient.Properties.Settings.Default.NotifyServerPort, true);
            nclient.OnConnect += new RemoteInterface.OnConnectEventHandler(nclient_OnConnect);

        
            //for (int i = 0; i < dgv.RowCount; i++)
            //{
            //    dgv.Rows[i].Cells["Display"].Value = new QYClient.RgsGenericCtl();
            //    dgv.Rows[i].Cells["Display"].ValueType = typeof(QYClient.RgsGenericCtl);
            //}

        }

        void nclient_OnConnect()
        {
            nclient.RegistEvent(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.RGS_Connection_Event, "*", null));
            nclient.RegistEvent(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.RGS_HW_Status_Event, "*", null));
            nclient.OnEvent += new RemoteInterface.NotifyEventHandler(nclient_OnEvent);
            //throw new Exception("The method or operation is not implemented.");
        }

        void nclient_OnEvent(RemoteInterface.NotifyEventObject objEvent)
        {
            //throw new Exception("The method or operation is not implemented.");
            try
            {
                if (objEvent.type == RemoteInterface.EventEnumType.RGS_Connection_Event)
                {
                    QYTask.Ds.tblRGSMainRow r = ds.tblRGSMain.FindByip(objEvent.devip);

                    r.connected = (bool)objEvent.EventObj;
                    r.EndEdit();
                    r.AcceptChanges();
                }
                else if (objEvent.type == RemoteInterface.EventEnumType.RGS_HW_Status_Event)
                {
                   I_HW_Status_Desc hw_desc = (I_HW_Status_Desc)objEvent.EventObj;
                    QYTask.Ds.tblRGSMainRow r = ds.tblRGSMain.FindByip(objEvent.devip);
                    System.Collections.IEnumerator ie = hw_desc.getEnum().GetEnumerator();
                    while (ie.MoveNext())
                    {
                        if ((int)ie.Current == 1 || (int)ie.Current == 0 || (int)ie.Current == 8 || (int)ie.Current == 9 || (int)ie.Current == 10 || (int)ie.Current == 11)
                            r[hw_desc.getDesc((int)ie.Current)] = hw_desc.getStatus((int)ie.Current);
                        r.EndEdit();
                        r.AcceptChanges();
                    }

                }
            }
            catch (System.Data.DataException)
            {
            }
            catch (Exception )
            {
                // MessageBox.Show(ex.Message);
            }
           

        }

        private void FrmRGSStatus_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.nclient.close();
        }

        private void dgv_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
    }
}