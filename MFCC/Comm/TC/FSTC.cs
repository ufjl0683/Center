using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HWStatus;
using RemoteInterface;
using Comm;
using System.Data;

namespace Comm.TC
{
    public class FSTC : OutputTCBase
    {
        byte curr_type=0;
        public FSTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus, byte comm_state)
          : base(protocol, devicename, ip, port, deviceid, hw_status,opmode,opstatus, comm_state)
        {


            this.OnTCReport += new OnTCReportHandler(FSTC_OnTCReport);
            this.OnTCReceiveText += new OnTCReportHandler(FSTC_OnTCReceiveText);
          
            
        }

        void FSTC_OnTCReceiveText(object tc, TextPackage txt)
        {
            //throw new Exception("The method or operation is not implemented.");
            if (txt.Text[0] == 0xd1)
                FS_LedTest_Report(txt.Text, this.DeviceName);


        }

        private void FS_LedTest_Report(byte[] txt, string devname)//D1  D1 00 00 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 CF FF FF EF FF FF EF EF EF EF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07 FF FF FB 07 FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
        {
            string SqlLedTest = "";
            string SqlLedTestDetail = "";
            int k = 0;//燈箱編號位址
            int SignCnt = txt[6];
            bool InsertDb = false;

            DateTime t = DateTime.Now;

            for (int i = 1; i < SignCnt + 1; i++)
            {
                for (int j = 7 + (30 * (i - 1)); j < (7 + 30 * i); j++)
                {
                    if (txt[j] > 0)
                    {
                        k = 6 + 30 * (i - 1);
                        SqlLedTestDetail = "INSERT INTO db2inst1.tblLEDTestDetail_FS(Devicename,Timestamp,sign_no,led_status1,led_status2,led_status3,led_status4,led_status5,led_status6,led_status7,led_status8,led_status9,led_status10,led_status11,led_status12,led_status13,led_status14,led_status15,led_status16,led_status17,led_status18,led_status19,led_status20,led_status21,led_status22,led_status23,led_status24,led_status25,led_status26,led_status27,led_status28,led_status29,led_status30) VALUES('" + devname + "','" + RemoteInterface.DbCmdServer.getTimeStampString(t) + "'," + (i - 1) + "," + txt[k + 1] + "," + txt[k + 2] + "," + txt[k + 3] + "," + txt[k + 4] + "," + txt[k + 5] + "," + txt[k + 6] + "," + txt[k + 7] + "," + txt[k + 8] + "," + txt[k + 9] + "," + txt[k + 10] + "," + txt[k + 11] + "," + txt[k + 12] + "," + txt[k + 13] + "," + txt[k + 14] + "," + txt[k + 15] + "," + txt[k + 16] + "," + txt[k + 17] + "," + txt[k + 18] + "," + txt[k + 19] + "," + txt[k + 20] + "," + txt[k + 21] + "," + txt[k + 22] + "," + txt[k + 23] + "," + txt[k + 24] + "," + txt[k + 25] + "," + txt[k + 26] + "," + txt[k + 27] + "," + txt[k + 28] + "," + txt[k + 29] + "," + txt[k + 30] + ")";
                        this.InvokeDBDemand(SqlLedTestDetail);
                        SqlLedTestDetail = "";
                        InsertDb = true;
                        break;
                    }
                }
            }
            if (InsertDb)
            {
                SqlLedTest = "INSERT INTO db2inst1.tblLEDTest_FS(Devicename,Timestamp,sign_flash,sign_cnt) VALUES('" + devname + "','" + RemoteInterface.DbCmdServer.getTimeStampString(t) + "'," + txt[5] + "," + txt[6] + ")";
                this.InvokeDBDemand(SqlLedTest);                
            }
        }

        void FSTC_OnTCReport(object tc, TextPackage txt)
        {

            if (txt.Text[0] == 0xd4)
            {
                byte type = txt.Text[1];
                this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(),GetDisplayDecs(type));

            }
            //throw new Exception("The method or operation is not implemented.");
        }
      public override void DownLoadConfig()
      {
        //  throw new Exception("The method or operation is not implemented.");
      }


      private void TC_SetDispalyOnOff(bool onoff)
      {
          byte[] data = new byte[] { 0xd0,(byte)((onoff)?1:0)};
          SendPackage pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, data);
          this.Send(pkg);
      }
      public override void TC_SetDisplayOff()
      {
          if(curr_type!=0)
              this.InvokeOutPutChangeEvent(this,this.GetDisplayDecs(0) );
          this.TC_SetDispalyOnOff(false);
          curr_type = 0;
         // throw new Exception("The method or operation is not implemented.");
      }

        public void TC_SetDisplay(byte type)
        {
            lock (this.currDispLockObj)
            {
                byte[] data = new byte[] { 0xd2, type };

                SendPackage pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, data);
                this.Send(pkg);
                if (curr_type != type)
                {
                    curr_type = type;
                    this.InvokeOutPutChangeEvent(this, GetDisplayDecs(type));

                }


                

                this.TC_SetDispalyOnOff(true);
            }
        }

        public byte TC_GetDisplayType()
        {
            byte[] data = new byte[] { 0x04,0xd2};
            SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, 0xffff, data);
            this.Send(pkg);
            if (pkg.result == CmdResult.ACK)
            {
                return pkg.ReturnTextPackage.Text[8];
            }
            else
                throw new Exception(pkg.result.ToString());

        }
      public override string GetCurrentDisplayDecs()
      {
          if (curr_type == 0)
              return "熄滅";
          else
          {
              switch (curr_type)
              {
                  case 1:
                      return "VI";
                  case 2:
                      return "WD";
                  case 3:
                      return "RD";
                  default:
                      throw new Exception("type 不符");
              }

         }
      }


        public  string GetDisplayDecs(byte type)
        {
            if (type == 0)
                return "熄滅";
            else
            {
                switch (type)
                {
                    case 1:
                        return "VI";
                    case 2:
                        return "WD";
                    case 3:
                        return "RD";
                    default:
                        throw new Exception("type 不符");
                }

            }
        }

      protected override void CheckDisplayTask()
      {
         //throw new Exception("The method or operation is not implemented.");
          if (curr_type==0 || !IsConnected) return;

          try
          {
              byte type;
              if ((type = this.TC_GetDisplayType()) != curr_type)
              {
                  this.TC_SetDisplay(curr_type);
                  this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), GetDisplayDecs(type));
              }
              else
              {
                  this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), GetCurrentDisplayDecs());
              }
               

          }
          catch(Exception ex)
          {
              ConsoleServer.WriteLine(this.DeviceName + "," + ex.Message+ex.StackTrace);
          }

          
      }

      public override RemoteInterface.I_HW_Status_Desc getStatusDesc()
      {
          return new FS_HW_StatusDesc(this.m_deviceName, m_hwstaus);
      }
  }
}
