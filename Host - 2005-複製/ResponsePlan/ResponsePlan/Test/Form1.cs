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
        int eventid = 6000;


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
            execution.GenerateExecutionTable(65430);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //產生IIPEVENTTABLE
            execution.InputIIP_Event(65430);
        }
        int movingid = 21;
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                host.SetMovingContructEvent(movingid, "測試人員", DateTime.Now, "N6", "E", 16770, 18000, 1, "1100000000", "測試施工");

                System.Threading.Thread.Sleep(10000);

                host.SetMovingContructEvent(movingid, "測試人員", DateTime.Now, "N6", "E", 17770, 18000, 1, "1100000000", "測試施工");

                System.Threading.Thread.Sleep(10000);
                host.SetMovingContructEvent(movingid, "測試人員", DateTime.Now, "N6", "E", 17970, 18000, 1, "1100000000", "測試施工");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            execution.CloseMoving(movingid);
        }

    }
}