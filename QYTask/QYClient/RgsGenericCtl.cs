using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace QYClient
{
    public delegate  void  RGSCtlEventHandler(object sender,EventArgs e);

    public partial class RgsGenericCtl : UserControl
    {

       public  volatile bool IsUserInput=false;
       
        System.Windows.Forms.RichTextBox[] txtmsgs;
        bool b_readOnly = false;
        byte m_iconId1=0,m_iconId2 = 0;

        public event RGSCtlEventHandler OnMsgChange;
       
        public RgsGenericCtl()
        {
            InitializeComponent();

            txtmsgs = new RichTextBox[] {this.rtxtmsg1,this.rtxtnsg2,this.rtxtmsg3,this.rtxtmsg4 };
        }


        public byte iconId1
        {
            get
            {
                return this.m_iconId1;
            }
            set
            {
                this.m_iconId1 = value;
                if (value == 0)
                    this.picIcon1.Image = null;
                else

                    this.picIcon1.Image = (Bitmap)QYClient.Properties.Resources.ResourceManager.GetObject("icon" + value);
            }
        }

        public byte iconId2
        {
            get
            {
                return this.m_iconId2;
            }
            set
            {
                this.m_iconId2 = value;
                if (value == 0)
                    this.picIcon2.Image = null;
                else
                    this.picIcon2.Image = (Bitmap)QYClient.Properties.Resources.ResourceManager.GetObject("icon" + value);
            }
        }

        public void setForeColor(int row,int pos,Color color)
        {
            if (pos == -1) return;
            txtmsgs[row].Select(pos,1);
            txtmsgs[row].SelectionColor = color;
             txtmsgs[row].Select(pos,0);;
        }

        public void setTextAndColor(int row, string text, Color[] colors)
        {
            this.txtmsgs[row].Text = text;
            for (int i = 0; i < colors.Length; i++)
            {
                setForeColor(row,i,colors[i]);
            }
        }
        public bool ReadOnly
        {
            get
            {
                return this.b_readOnly;
            }
            set
            {
                b_readOnly = value;
                foreach (RichTextBox rbox in this.txtmsgs)
                    rbox.ReadOnly = value;
            }
        }

        public void setTextEnable(int txtId, bool bEnabled)
        {
            this.txtmsgs[txtId].ReadOnly=!bEnabled;
        }
      
        public string getMsgText(int inx)
        {
            try
            {
                return txtmsgs[inx].Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        public string[] getAllMsgs()
        {
            string[] ret;
            ret = new string[4];
            for (int i = 0; i < 4; i++)
                ret[i] = this.txtmsgs[i].Text;

            return ret;
        }


        public Color[] getMsgForeColor(int inx)
        {
          //  txtmsgs[inx].Text=txtmsgs[inx];
            int positionbak;
            positionbak = txtmsgs[inx].SelectionStart;
          Color[] retColor=new Color[txtmsgs[inx].Text.Length];

            for(int i=0;i<retColor.Length;i++)
            {
                txtmsgs[inx].Select(i,1);
                retColor[i] = txtmsgs[inx].SelectionColor;
             }
             txtmsgs[inx].SelectionStart = positionbak;
             txtmsgs[inx].SelectionLength = 0;

             return retColor;
        }


        public Color[][] getAllMsgForeColor()
        {
            Color[][] ret;

            ret = new Color[4][];
            for (int i = 0; i < 4; i++)
                ret[i]= getMsgForeColor(i);

            return ret;

            
        }
        public Color[][] getAllMsgBackColor()
        {
            Color[][] ret;

            ret = new Color[4][];
            for (int i = 0; i < 4; i++)
                ret[i] = getMsgBackColor(i);

            return ret;


        }
        public Color[] getMsgBackColor(int inx)
        {
            //  txtmsgs[inx].Text=txtmsgs[inx];
            Color[] retColor = new Color[txtmsgs[inx].Text.Length];
            int positionbak;
            positionbak = txtmsgs[inx].SelectionStart;
            for (int i = 0; i < retColor.Length; i++)
            {
                txtmsgs[i].Select(i, 1);
                retColor[i] = txtmsgs[inx].SelectionBackColor;
            }
            txtmsgs[inx].SelectionStart = positionbak;
            txtmsgs[inx].SelectionLength=0;
            return retColor;
        }

        public void setMsgText(int inx, string text)
        {
            try
            {
                if(!b_readOnly)
                txtmsgs[inx].Text = text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
        }

       // object focusTxt;
        
        private void rtxtmsg1_Enter(object sender, EventArgs e)
        {
            設定顏色ToolStripMenuItem.Tag = sender;
        }

        private void 設定顏色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorChoice.ShowDialog() == DialogResult.Cancel)
                return;

            if (設定顏色ToolStripMenuItem.Tag is RichTextBox && ((RichTextBox)設定顏色ToolStripMenuItem.Tag).Focused)
            {
                RichTextBox c = (RichTextBox)設定顏色ToolStripMenuItem.Tag;
                if (c.ReadOnly) return;
                c.SelectionColor = colorChoice.Color;
            }
        }

        private void rtxtmsg1_TextChanged(object sender, EventArgs e)
        {
            if (this.OnMsgChange != null && this.IsUserInput)
                this.OnMsgChange(this,e);
        }

       public void setOff()
       {
           this.IsUserInput = false;
           for(int i=0;i<4;i++)
               txtmsgs[i].Text="";
           this.iconId1 = this.iconId2 = 0;
           this.IsUserInput = true;
       }

      


        //public Color[] ForeColors
        //{

        //    get
        //    {

        //       Color[] forcolors = new Color[msg.Length];
        //        backcolors = new Color[msg.Length];
        //        for (int inx = 0; inx < msg.Length; inx++)
        //        {
        //            c.Select(inx, 1);
        //            forcolors[inx] = c.SelectionColor;
        //            backcolors[inx] = c.SelectionBackColor;

        //        }
        //    }
        //}


        //public Color[] BackColors
        //{

        //    get
        //    {

        //    }
        //}



    }
}
