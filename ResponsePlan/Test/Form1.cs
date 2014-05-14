using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        Execution.Execution execution = null;
        DBConnect.ODBC_DB2Connect cmd;
 


        //BackgroundWorker bw = new BackgroundWorker();
        RemoteInterface.HC.I_HC_FWIS host;
        public Form1()
        {
            InitializeComponent();
            cmd = new DBConnect.ODBC_DB2Connect();
            execution = Execution.Execution.getBuilder();
            host=Execution.EasyClient.getHost();

             //eventid = execution..setMoveConstruction("test", DateTime.Now, "N6", "E", 17770, 18000, "1", "1100000000");
        }


        private void button6_Click(object sender, EventArgs e)
        {
            //產生EXECUTIONTABLE
            execution.GenerateExecutionTable(8588733);//360278)
            //execution.GenerateExecutionTable(387436);
           
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //產生IIPEVENTTABLE

            execution.InputIIP_Event(7785258);
        }
        int movingid = 21;
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                //Execution.EasyClient a = new Execution.EasyClient();
                //a.getExecutionObj("U.20101231160355.471964");
                host.SetMovingContructEvent(50000000, "測試人員", DateTime.Now, "N6", "E", 16000, 16000, 1, "0000000000", "測試施工");
                
                //host.SetMovingContructEvent(1, "0000", DateTime.Now, "N1", "S", 179000, 179000, 1, "1100000000", "施工測試");

                //host.SetMovingContructEvent(movingid, "測試人員", DateTime.Now, "N6", "E", 16770, 18000, 1, "1100000000", "測試施工");

                //System.Threading.Thread.Sleep(10000);

                //host.SetMovingContructEvent(movingid, "測試人員", DateTime.Now, "N6", "E", 17770, 18000, 1, "1100000000", "測試施工");

                //System.Threading.Thread.Sleep(10000);
                //host.SetMovingContructEvent(movingid, "測試人員", DateTime.Now, "N6", "E", 17970, 18000, 1, "1100000000", "測試施工");
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            execution.CloseMoving(50000000);           
        }

    }
}