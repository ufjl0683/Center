using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            SNSCOMSERVER.SnsComObject sms = new SNSCOMSERVER.SnsComObject();
            int ret = sms.Login("203.66.172.133", 8001, "10371", "10371");
            if (ret != 0)
                throw new Exception("error!");

            ret = sms.SubmitBig5Message("0988163835","中文測試");

            if (ret != 0)
                throw new Exception("error!");


            string smsid = sms.RespMessage;
            do
            {
                System.Threading.Thread.Sleep(1000);
                ret = sms.QryMessageStatus("0988163835", smsid);

            }
            while (ret == 1);
            if (ret != 0)
                throw new Exception("error!");

            sms.Logout();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}