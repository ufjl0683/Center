using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HWStatus;
using RemoteInterface.MFCC;
using RemoteInterface;

namespace Comm.TC
{


  public   class CSLSTC:OutputTCBase
    {
    //  public event OnOutputChangedHandler OnOutputChanged;

      System.Data.DataSet currDisplayds;
    //  int curr_speed=255;

      public CSLSTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status,byte opmode,byte opstatus,byte comm_state)
          : base(protocol, devicename, ip, port, deviceid, hw_status,opmode,opstatus,comm_state)
      {

          this.OnTCReport += new OnTCReportHandler(CSLSTC_OnTCReport);
          this.OnTCReceiveText += new OnTCReportHandler(CSLSTC_OnTCReceiveText);
      }

      void CSLSTC_OnTCReceiveText(object tc, TextPackage txt)
      {
          //throw new Exception("The method or operation is not implemented.");
          if (txt.Text[0] == 0xb1)
              CSLS_LedTest_Report(txt.Text, this.DeviceName);
      }


      private void CSLS_LedTest_Report(byte[] txt, string devname)//B1  B1 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 
      {
          int ErrorSum = 0;
          string SqlLedTest = "";
          string SqlLedTestDetail = "";

          for (int i = 6; i < txt.Length; i++)
          {
              if (txt[i] > 0)
              {
                  ErrorSum++;
                  break;
              }
          }
          DateTime t = DateTime.Now;

          if (ErrorSum != 0)
          {
              SqlLedTest = "INSERT INTO db2inst1.TBLLEDTest_CSLS(DEVICENAME,TIMESTAMP,sls_no,led_status_1,led_status_2,led_status_3,led_status_4,led_status_5,led_status_6,led_status_7,led_status_8,led_status_9,led_status_10,led_status_11,led_status_12) VALUES('" + devname + "','" + RemoteInterface.DbCmdServer.getTimeStampString(t) + "'," + txt[5] + "," + txt[6] + "," + txt[7] + "," + txt[8] + "," + txt[9] + "," + txt[10] + "," + txt[11] + "," + txt[12] + "," + txt[13] + "," + txt[14] + "," + txt[15] + "," + txt[16] + "," + txt[17] + ") ";
              this.InvokeDBDemand(SqlLedTest);
          }
      }

      void CSLSTC_OnTCReport(object tc, TextPackage txt)
      {
#if DEBUG
          //if ((tc as TCBase).DeviceName != "CSLS-N6-E-17.5")
          //    return;
#endif 
          if (txt.Text[0] == 0xb5)
          {
              if (currDisplayds == null && txt.Text[1] == 0xff)
              {
                  this.InvokeOutPutWrongEvent("255", txt.Text[1].ToString());
                  return;
              }

              this.InvokeOutPutWrongEvent(currDisplayds.Tables[0].Rows[0]["speed"].ToString(), txt.Text[1].ToString());

          }

          //throw new Exception("The method or operation is not implemented.");
      }


      public override void DownLoadConfig()
      {
         // throw new Exception("The method or operation is not implemented.");
      }

      protected override void CheckDisplayTask()
      {
        
          if (!this.IsConnected)
              return;

          if (this.currDisplayds == null)
          {
              this.TC_SetDisplayOff();
              return;
          }

          lock (currDispLockObj)
          {
              bool isequal = true;
              try
              {
                  System.Data.DataSet ds;
                  ds = this.m_protocol.GetSendDataSet("get_speed");
                  SendPackage pkg = this.m_protocol.GetSendPackage(ds, 0xffff);
                  this.Send(pkg);
                  ds = m_protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
                  isequal = System.Convert.ToInt32(ds.Tables[0].Rows[0]["speed"]) == System.Convert.ToInt32(currDisplayds.Tables[0].Rows[0]["speed"]);
                  if (!isequal)
                  {
                      ConsoleServer.WriteLine("compare output different");
                      this.InvokeOutPutWrongEvent(currDisplayds.Tables[0].Rows[0]["speed"].ToString(), ds.Tables[0].Rows[0]["speed"].ToString());
                      this.TC_SetDislay(currDisplayds);
                  }
                  else
                  {
                      this.InvokeOutPutWrongEvent(ds.Tables[0].Rows[0]["speed"].ToString(), ds.Tables[0].Rows[0]["speed"].ToString());

                  }



              }
              catch (Exception ex)
              {
                  ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
              }


          }
      }


      public override string GetCurrentDisplayDecs()
      {

          if (currDisplayds == null)
              return "";

          return "spd:" + currDisplayds.Tables[0].Rows[0]["speed"];
          //throw new Exception("The method or operation is not implemented.");
      }
      public void TC_SetDislay(System.Data.DataSet ds)
      {
          checkConntected();

          if (ds.Tables[0].Rows[0]["func_name"].ToString() != "set_speed")
              throw new Exception("only support func_name = set_speed");


          lock (this.currDispLockObj)
          {
              if (currDisplayds == null)
              {


                  this.InvokeOutPutChangeEvent(this, ds.Tables[0].Rows[0]["speed"].ToString());
              }
              else
                  if (currDisplayds.Tables[0].Rows[0]["speed"].ToString() != ds.Tables[0].Rows[0]["speed"].ToString())

                      this.InvokeOutPutChangeEvent(this, ds.Tables[0].Rows[0]["speed"].ToString());
                  
                  



                      currDisplayds = ds;
              Comm.SendPackage pkg = this.m_protocol.GetSendPackage(ds, 0xffff);

              this.Send(pkg);

          }

      }

      public override void TC_SetDisplayOff()
      {
          checkConntected();
          System.Data.DataSet ds;
          ds = this.m_protocol.GetSendDataSet("set_speed");
          ds.Tables[0].Rows[0]["speed"] = 0xff;
          ds.AcceptChanges();
          SendPackage pkg = this.m_protocol.GetSendPackage(ds, 0xffff);
          this.Send(pkg);

          if (this.currDisplayds != null)
              this.InvokeOutPutChangeEvent(this, "熄滅");
          lock (this.currDispLockObj)
          {
              this.currDisplayds = null;
          }
      }

      public override I_HW_Status_Desc getStatusDesc()
      {
          return new CSLS_HW_StatusDesc(this.DeviceName, m_hwstaus);
      }

    }
}
