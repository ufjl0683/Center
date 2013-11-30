using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QYClient
{
    public partial class FrmRmsModeSetting : Form
    {
        public FrmRmsModeSetting()
        {
            InitializeComponent();
            this.ds.tblRMSMode.AddtblRMSModeRow(0, "固定時制");
           // this.ds.tblRMSMode.AddtblRMSModeRow(1, "區域交通反應");
            this.ds.tblRMSMode.AddtblRMSModeRow(2, "預設時制");
            this.ds.AcceptChanges();
           
            try
            {
                System.Windows.Forms.DataGridViewComboBoxColumn col = (System.Windows.Forms.DataGridViewComboBoxColumn)this.dataGridView1.Columns["planno_setting"];
                for (uint i = 0; i < 32; i++)
                {
                    //  System.Windows.Forms.DataGridViewComboBoxColumn col = (System.Windows.Forms.DataGridViewComboBoxColumn)this.dataGridView1.Columns["planno_setting"];
                    col.Items.Add(i);
                }
                this.ds.Merge(Util.robj.get_rms_config());
                this.dataGridView1.Refresh();
                


                //for (int i = 0; i < this.dataGridView1.RowCount; i++)
                //    this.dataGridView1.Rows[i].Cells["Send"].Value = "傳送";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name != "Send")
                return;

            System.Data.DataRowView r = (System.Data.DataRowView)this.bs.Current;
            try
            {
               
                Util.robj.set_rms_mode_planno(r["ip"].ToString(), (byte)(uint)r["ctl_mode_setting"],(byte)(uint)r["planno_setting"]);  
                //if ((byte)r["mode"] == 1)

                //    Util.robj.setFreeInputModeAndMessage(r["ip"].ToString(), (byte)r["display_part"], (byte)r["ficon"], r["finput1"].ToString(), r["finputcolor1"].ToString(),
                //        r["finput2"].ToString(), r["finputcolor2"].ToString());
                //else
                //    Util.robj.setTravelMode(r["ip"].ToString(), (byte)r["display_part"]);
                MessageBox.Show("傳送完成");

            }
            catch (Exception ex)
            {
                MessageBox.Show("RMS:連線異常"+ex.Message);
            }
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            int planno = System.Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["planno_setting"].Value);
            if (planno < 0 || planno > 31)
            {
                e.Cancel = true;
                MessageBox.Show("planno 必需介於0~31之間");
            }
        }

        private void bs_CurrentItemChanged(object sender, EventArgs e)
        {
            
        }
    }
}