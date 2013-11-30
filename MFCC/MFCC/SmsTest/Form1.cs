using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SmsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SNSCOMSERVER.SnsComObject sms = new SNSCOMSERVER.SnsComObject();
            int ret = sms.Login("203.66.172.133", 8001, "10371", "10371");
            if (ret != 0)
            {
                throw new Exception("error!");
               // return 1;
            }

           string mesg=this.textBox1.Text;
           string[] messages = new string[mesg.Length % 70 == 0 ? mesg.Length / 70 : mesg.Length / 70 + 1];
           for (int i = 0; i < messages.Length; i ++)
            {
                if (i == messages.Length - 1)
                    messages[i] = mesg.Substring(i * 70);
                else
                    messages[i] = mesg.Substring(i * 70, 70);
            }

            for (int i = 0; i < messages.Length;i++ )
                ret = sms.SubmitBig5Message("0988163835", messages[i]);

            if (ret != 0)
            {
              //  return 1;
                throw new Exception("error!");
            }


            string smsid = sms.RespMessage;
            //do
            //{
            //    System.Threading.Thread.Sleep(1000);
            //    ret = sms.QryMessageStatus(phoneNo, smsid);

            //}
            //while (ret == 1 );
            //if (ret != 0)
            //    throw new Exception("error!");

            sms.Logout();
        }
    }
}