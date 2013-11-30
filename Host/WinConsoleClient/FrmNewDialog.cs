using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinConsoleClient
{
    public partial class FrmNewDialog : Form
    {
        public FrmNewDialog()
        {
            InitializeComponent();
          
        }

        private void FrmNewDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (optMoniter.Checked)
            {
                if ( txtDevName.Text == "")
                {
                    MessageBox.Show("必須指定存檔路徑DevName");
                }
            }
        }
        public string MfccId
        {

            get
            {
              return  cbPort.SelectedItem.ToString();
            }
        }

        public int Port
        {
            get
            {
                return ((ConsolePortItem)this.cbPort.SelectedItem).Port;
            }
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            //this.Close();
        }

        // QYServer=8000,
        //Host=8010,
        //MFCC_VD1=8020,
        //MFCC_VD2=8021,
        //MFCC_RGS=8030,
        //MFCC_RMS=8040,
        //MFCC_CMS=8050,
        //MFCC_WIS=8060,
        //MFCC_LCS=8070,
          //MFCC_AVI=8100
        private void FrmNewDialog_Load(object sender, EventArgs e)
        {

            this.cbPort.Items.Add(new ConsolePortItem("Host",8010));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_VD1",8020));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_VD2", 8021));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_VD3", 8022));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_VD4", 8023));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_VD5", 8024));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_VD6", 8025));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_VD7", 8026));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_VD8", 8027));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_VD9", 8028));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_RGS",8030));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_RMS",8040));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_RMS2", 8044));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_CMS1",8050));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_CMS2", 8051));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_CMS3", 8052));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_CMS4", 8053));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_CMS5", 8054));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_WIS",8060));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_LCS",8070));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_CSLS1", 8080));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_CSLS2", 8081));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_AVI", 8100));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_RD", 8110));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_VI", 8120));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_WD", 8130));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_TTS", 8140));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_FS", 8150));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_MAS", 8160));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_PBX", 8170));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_SVWS", 8180));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_IID", 8190));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_ETTU",8200));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_LS", 8210));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_TEM", 8220));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_SCM", 8230));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_CMSRST", 8240));
            this.cbPort.Items.Add(new ConsolePortItem("MFCC_BS", 8250));
        
            this.cbPort.SelectedIndex = 0;



        }

        private void cbPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            string mfccid = cbPort.SelectedItem.ToString();
            if (mfccid == "Host" || mfccid == "MFCC_PBX" || mfccid == "MFCC_IID" || mfccid == "MFCC_SVWS" || mfccid == "MFCC_ETTU" || mfccid == "MFCC_AVI" || mfccid=="MFCC_TEM" )

                this.txIP.Text = "10.21.50.4";
            else if (mfccid.StartsWith("MFCC_VD"))
                this.txIP.Text = "10.21.50.17";
            else
                this.txIP.Text = "10.21.50.8";
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //if (sd.ShowDialog() == DialogResult.Cancel)
            //    return;
            //this.txtlogPathFileName.Text = sd.FileName;
        }


    }

   internal class ConsolePortItem
    {
        string mfccid;
        int port;
      internal  ConsolePortItem(string mfccid, int port)
        {
            this.mfccid = mfccid;
            this.port = port;
        }

       public override string ToString()
       {
           return mfccid; ;
       }
    public    int Port
       {
           get
           {
               return port;
           }
       }
    }
}