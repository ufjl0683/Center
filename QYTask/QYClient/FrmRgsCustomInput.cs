using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QYClient
{
    public partial class FrmRGSCustomInput : Form
    {
        public FrmRGSCustomInput()
        {
            InitializeComponent();
          //  DataSet ds1 = Util.robj.getRGSAllTables();
            try
            {
                this.ds.Merge(Util.robj.get_RGS_all_tables());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

          this.rgsctl.OnMsgChange += new RGSCtlEventHandler(rgsctl_OnMsgChange);
        }

        void rgsctl_OnMsgChange(object sender, EventArgs e)
        {
           // MessageBox.Show("rgs changed!");
            System.Data.DataRowView rv ;
            for (int i = 0; i < this.bsmainRGSConf.CurrencyManager.Count; i++)
            {
                rv = (System.Data.DataRowView)this.bsmainRGSConf[i];
                if ((byte)rv["mode"] == 1)
                {
                    rv["finput1"] = this.rgsctl.getMsgText(((byte)rv["display_part"] - 1) * 2);
                    rv["finput2"] = this.rgsctl.getMsgText(((byte)rv["display_part"] - 1) * 2+1);
                    rv["finputcolor1"] = Util.ToColorString( this.rgsctl.getMsgForeColor(((byte)rv["display_part"] - 1) * 2));
                    rv["finputcolor2"] = Util.ToColorString(this.rgsctl.getMsgForeColor(((byte)rv["display_part"] - 1) * 2+1));
                    this.bsmainRGSConf.EndEdit();

                }
            }


            

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void bsmainRGSConf_CurrentChanged(object sender, EventArgs e)
        {

         //   System.Data.DataRowView r = (System.Data.DataRowView)bsmainRGSConf.Current;
            rgsctl.IsUserInput = false;
            for (int i = 0; i < bsmainRGSConf.Count; i++)
            {
                System.Data.DataRowView r = (System.Data.DataRowView)bsmainRGSConf[i];
                //rgsctl.setTextEnable(0, false);
                //rgsctl.setTextEnable(1, false);
                //rgsctl.setTextEnable(2, false);
                //rgsctl.setTextEnable(3, false);
                if (System.Convert.ToByte(r["display_part"]) == 1) //上半部
                {

                    if ((byte)r["mode"] == 0)//旅行時間模式
                    {
                        rgsctl.iconId1 = (byte)r["iconId"];
                        rgsctl.setMsgText(0, r["msg_temp1"].ToString());
                        rgsctl.setMsgText(1, r["msg_temp2"].ToString());
                        rgsctl.setForeColor(1, r["msg_temp2"].ToString().IndexOf('@'), Color.Orange);
                        rgsctl.setTextEnable(0, false);
                        rgsctl.setTextEnable(1, false);
                    }
                    else  //手動模式
                    {
                        rgsctl.iconId1 = (byte)r["ficon"];
                        rgsctl.setMsgText(0, r["finput1"].ToString());
                        rgsctl.setMsgText(1, r["finput2"].ToString());
                        rgsctl.setTextEnable(0, true);
                        rgsctl.setTextEnable(1, true);
                        rgsctl.setTextAndColor(0,r["finput1"].ToString(),Util.ToColors(r["finputColor1"].ToString()));
                        rgsctl.setTextAndColor(1, r["finput2"].ToString(), Util.ToColors(r["finputColor2"].ToString()));

                    }
                    rgsctl.iconId2 = 0;
                    rgsctl.setMsgText(2, "");
                    rgsctl.setMsgText(3, "");

                }
                else  // cms 下半部
                {
                    if ((byte)r["mode"] == 0)  //旅行時間模式
                    {
                        rgsctl.iconId2 = (byte)r["iconId"];
                        rgsctl.setMsgText(2, r["msg_temp1"].ToString());
                        rgsctl.setMsgText(3, r["msg_temp2"].ToString());
                        rgsctl.setForeColor(3, r["msg_temp2"].ToString().IndexOf('@'), Color.Orange);
                        rgsctl.setTextEnable(2, false);
                        rgsctl.setTextEnable(3, false);

                    }
                    else  //手動模式
                    {

                        rgsctl.iconId2 = (byte)r["ficon"];
                        rgsctl.setMsgText(2, r["finput1"].ToString());
                        rgsctl.setMsgText(3, r["finput2"].ToString());
                        rgsctl.setTextEnable(2, true);
                        rgsctl.setTextEnable(3, true);
                        rgsctl.setTextAndColor(2, r["finput1"].ToString(), Util.ToColors(r["finputColor1"].ToString()));
                        rgsctl.setTextAndColor(3, r["finput2"].ToString(), Util.ToColors(r["finputColor2"].ToString()));

                    }

                }
            } //for
            rgsctl.IsUserInput = true;
        }

        private void bs1_CurrentChanged(object sender, EventArgs e)
        {
           
            
        }

        private void bsmainRGSConf_CurrentItemChanged(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            bsmainRGSConf_CurrentChanged(null, null);
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
           
                try
                {
            System.Data.DataRowView r = (System.Data.DataRowView)this.bsmainRGSConf.Current;
            if (dataGridView1.Columns[e.ColumnIndex].Name == "send")
            {

            
                    if ((byte)r["mode"] == 1)

                        Util.robj.setFreeInputModeAndMessage(r["ip"].ToString(), (byte)r["display_part"], (byte)r["ficon"], r["finput1"].ToString(), r["finputcolor1"].ToString(),
                            r["finput2"].ToString(), r["finputcolor2"].ToString());
                    else
                        Util.robj.setTravelMode(r["ip"].ToString(), (byte)r["display_part"]);
                    MessageBox.Show("傳送完成");

             
            }
            else if (dataGridView1.Columns[e.ColumnIndex].Name == "off")
            {
                r["ficon"] = 0;
                r["mode"] = 1;
                r["finput1"] ="";
                r["finput2"] = "";
                r["finputcolor1"] = "";
                r["finputcolor2"] = "";
                r.EndEdit();
                this.bs1.EndEdit();

                Util.robj.setFreeInputModeAndMessage(r["ip"].ToString(), (byte)r["display_part"], (byte)r["ficon"], r["finput1"].ToString(), r["finputcolor1"].ToString(),
                    r["finput2"].ToString(), r["finputcolor2"].ToString());

                MessageBox.Show("傳送完成");
                bsmainRGSConf_CurrentChanged(null, null);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show("RGS 連線異常" + ex.Message);
        }

            //MessageBox.Show("傳送"+e.RowIndex);
        }// func


      
    }
}