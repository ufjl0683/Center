using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinConsoleClient
{
    public partial class FrmSimulate : Form
    {
        public FrmSimulate()
        {
            InitializeComponent();
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (btnVD.Text == "開始")
            {
                btnVD.Text = "結束";
                tmrvd.Start();
            }
            else
            {
                btnVD.Text = "開始";
                tmrvd.Stop();
            }
            tmrvd_Tick(null, null);
        }

        private void tmrvd_Tick(object sender, EventArgs e)
        {

            this.BindingContext[ds, "tblVD5Min"].EndCurrentEdit();
            ds.tblVD5Min.AcceptChanges();

            foreach (DataSet1.tblVD5MinRow r in ds.tblVD5Min.Rows)
            {
                try
                {
                    RemoteInterface.MFCC.VD1MinCycleEventData data = new RemoteInterface.MFCC.VD1MinCycleEventData(r.DeviceName, System.DateTime.Now, r.Speed, r.Vol, r.Occupancy, r.Length, r.Interval, null, null, true);
                    (this.MdiParent as FrmMain).rhost.setVDFiveMinData(r.DeviceName, data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
           
        }

        private void tmrvi_Tick(object sender, EventArgs e)
        {
            this.BindingContext[ds, "tblVI5Min"].EndCurrentEdit();
            ds.tblVD5Min.AcceptChanges();

            foreach (DataSet1.tblVI5MinRow r in ds.tblVI5Min.Rows)
            {
                try
                {
                   // RemoteInterface.MFCC.VD1MinCycleEventData data = new RemoteInterface.MFCC.VD1MinCycleEventData(r.DeviceName, System.DateTime.Now, r.Speed, r.Vol, r.Occupancy, r.Length, r.Interval, null, null, true);
                    (this.MdiParent as FrmMain).rhost.setVIEventData(r.DeviceName,DateTime.Now,r.Distance, r.Degree);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
       

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (button1.Text == "開始")
            {
                button1.Text = "結束";
                tmrvi.Start();
            }
            else
            {
                button1.Text = "開始";
                tmrvi.Stop();
            }
            tmrvi_Tick(null, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (button2.Text == "開始")
            {
                button2.Text = "結束";
                tmrrd.Start();
            }
            else
            {
                button2.Text = "開始";
                tmrrd.Stop();
            }
            tmrrd_Tick(null, null);

        }

        private void tmrrd_Tick(object sender, EventArgs e)
        {
            this.BindingContext[ds, "tblrd5Min"].EndCurrentEdit();
            ds.tblRD5Min.AcceptChanges();

            foreach (DataSet1.tblRD5MinRow r in ds.tblRD5Min.Rows)
            {
                try
                {
                    // RemoteInterface.MFCC.VD1MinCycleEventData data = new RemoteInterface.MFCC.VD1MinCycleEventData(r.DeviceName, System.DateTime.Now, r.Speed, r.Vol, r.Occupancy, r.Length, r.Interval, null, null, true);
                    (this.MdiParent as FrmMain).rhost.setRDEventData(r.DeviceName, DateTime.Now, r.pluviometric, r.Degree);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (button3.Text == "開始")
            {
                button3.Text = "結束";
                tmrwd.Start();
            }
            else
            {
                button3.Text = "開始";
                tmrwd.Stop();
            }
            tmrwd_Tick(null, null);

        }

        private void tmrwd_Tick(object sender, EventArgs e)
        {
            this.BindingContext[ds, "tblwd5Min"].EndCurrentEdit();
            ds.tblWD5Min.AcceptChanges();

            foreach (DataSet1.tblWD5MinRow r in ds.tblWD5Min.Rows)
            {
                try
                {
                    // RemoteInterface.MFCC.VD1MinCycleEventData data = new RemoteInterface.MFCC.VD1MinCycleEventData(r.DeviceName, System.DateTime.Now, r.Speed, r.Vol, r.Occupancy, r.Length, r.Interval, null, null, true);
                    (this.MdiParent as FrmMain).rhost.setWDEventData(r.DeviceName, DateTime.Now, r.average_wind_speed, r.average_wind_direction, r.max_wind_speed, r.max_wind_direction, r.Degree);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (button4.Text == "開始")
            {
                button4.Text = "結束";
                tmrls.Start();
            }
            else
            {
                button4.Text = "開始";
                tmrls.Stop();
            }
            tmrls_Tick(null, null);
        }

        private void tmrls_Tick(object sender, EventArgs e)
        {
            this.BindingContext[ds, "tblLSData"].EndCurrentEdit();
            ds.tblLSData.AcceptChanges();

            foreach (DataSet1.tblLSDataRow r in ds.tblLSData.Rows)
            {
                try
                {
                    // RemoteInterface.MFCC.VD1MinCycleEventData data = new RemoteInterface.MFCC.VD1MinCycleEventData(r.DeviceName, System.DateTime.Now, r.Speed, r.Vol, r.Occupancy, r.Length, r.Interval, null, null, true);
                    (this.MdiParent as FrmMain).rhost.setLSEventData(r.DeviceName, DateTime.Now,r.mon_var,r.day_var,r.Degree);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

            if (button7.Text == "開始")
            {
                button7.Text = "結束";
                tmrls.Start();
            }
            else
            {
                button7.Text = "開始";
                tmrbs.Stop();
            }
            tmrbs_Tick(null, null);
        }

        private void tmrbs_Tick(object sender, EventArgs e)
        {

            this.BindingContext[ds, "tblBSData"].EndCurrentEdit();
            ds.tblBSData.AcceptChanges();

            foreach (DataSet1.tblBSDataRow r in ds.tblBSData.Rows)
            {
                try
                {
                    // RemoteInterface.MFCC.VD1MinCycleEventData data = new RemoteInterface.MFCC.VD1MinCycleEventData(r.DeviceName, System.DateTime.Now, r.Speed, r.Vol, r.Occupancy, r.Length, r.Interval, null, null, true);
                    (this.MdiParent as FrmMain).rhost.setBS_EventData(r.DeviceName, DateTime.Now,r.slope,r.shift,r.shift,r.Degree );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void button5_Click_1(object sender, EventArgs e)
        {

        SaveFileDialog dlog=    new  SaveFileDialog();
         if(   dlog.ShowDialog()== System.Windows.Forms.DialogResult.Cancel)
             return;
         this.ds.tblVD5Min.WriteXml(dlog.FileName);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;
            ds.tblVD5Min.Clear();

            ds.tblVD5Min.ReadXml(dlg.FileName);
            ds.AcceptChanges();
        }

       
    }
}