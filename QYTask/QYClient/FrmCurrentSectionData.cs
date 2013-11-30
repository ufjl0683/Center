using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QYClient
{

    delegate void invokeMethod(RemoteInterface.NotifyEventObject obj);
    public partial class FrmCurrentSectionData : Form
    {
        System.Data.DataSet ds;
     //   RemoteInterface.IQYCommands robj;
        RemoteInterface.EventNotifyClient nclient;
      //  System.Runtime.Remoting.Channels.Tcp.TcpClientChannel tcp = new System.Runtime.Remoting.Channels.Tcp.TcpClientChannel();
        public FrmCurrentSectionData()
        {
           
            InitializeComponent();
          
          
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FrmCurrentSectionData_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                nclient.close();
            }
            catch
            {
                ;
            }
        }

        private void FrmCurrentSectionData_Load(object sender, EventArgs e)
        {
            try
            {
               
                nclient = new RemoteInterface.EventNotifyClient(QYClient.Properties.Settings.Default.NotifyServerIP,QYClient.Properties.Settings.Default.NotifyServerPort,false);
                nclient.OnConnect += new RemoteInterface.OnConnectEventHandler(nclient_OnConnect);
                
             
                this.nclient.OnEvent += new RemoteInterface.NotifyEventHandler(nclient_OnEvent);
                try
                {

                    this.Text = (Util.robj.get_current_travel_data_time_stamp() == null) ? "" : Util.robj.get_current_travel_data_time_stamp().ToString();
                }
                catch
                {
                    ;
                }
               // this.Text = ds.Tables[0].Rows[0][0].ToString();
                ds = Util.robj.get_current_travel_time();
               
                this.dataGridView1.DataSource = ds;
                this.dataGridView1.DataMember = "tblRGS_Config";
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                try
                {
                   // this.Dispose();
                }
                catch (Exception ex1)
                {
                    MessageBox.Show(ex1.Message);
                }

            }
        }

        void nclient_OnConnect()
        {
           // throw new Exception("The method or operation is not implemented.");
            nclient.RegistEvent(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.QY_New_Travel_Time_Data, "*", null));
        }

        System.Data.DataSet tmpDs;
        void nclient_OnEvent(RemoteInterface.NotifyEventObject objEvent)
        {
            //throw new Exception("The method or operation is not implemented.");
            // MessageBox.Show("New Section Data");

            if (objEvent.type == RemoteInterface.EventEnumType.QY_New_Travel_Time_Data)
            {
                try
                {
                   
                    this.Invoke(new invokeMethod(UpdateMethod), objEvent);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
                MessageBox.Show("未定義的事件資料");


        }

        void UpdateMethod(RemoteInterface.NotifyEventObject objEvent)
        {
            this.dataGridView1.DataSource = objEvent.EventObj;
            this.dataGridView1.DataMember = "tblRGS_Config";
            this.dataGridView1.Refresh();
            try
            {
                this.Text = Util.robj.get_current_travel_data_time_stamp().ToString();
            }
            catch (Exception ex)
            {
                ;
            }
        }

        int rinx = -1;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            rinx = e.RowIndex;

        }

        private void cm_Opening(object sender, CancelEventArgs e)
        {
            
        }

        private void 檢視ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = dataGridView1.Rows[rinx].Cells["ip"].Value.ToString();
                byte display_part = System.Convert.ToByte(dataGridView1.Rows[rinx].Cells["display_part"].Value);

                new FrmTravelTimeDetail(Util.robj.get_travel_sections_detail_data(ip, display_part)).ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}