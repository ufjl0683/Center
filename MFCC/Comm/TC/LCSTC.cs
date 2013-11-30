using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HWStatus;

namespace Comm.TC
{
  public   class LCSTC: OutputTCBase
    {
    //  public event OnOutputChangedHandler OnOutputChanged;

      System.Data.DataSet currDisplayds;
      public LCSTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus, byte comm_state)
          : base(protocol, devicename, ip, port, deviceid, hw_status,opmode,opstatus,comm_state)
      {

          this.OnTCReport += new OnTCReportHandler(LCSTC_OnTCReport);
          this.OnTCReceiveText += new OnTCReportHandler(LCSTC_OnTCReceiveText);
      }

      void LCSTC_OnTCReceiveText(object tc, TextPackage txt)
      {
          //throw new Exception("The method or operation is not implemented.");
          if (txt.Text[0] == 0xc5)
              LCS_LedTest_Report(txt.Text, this.DeviceName);
          
      }


      private void LCS_LedTest_Report(byte[] txt, string devname)//C5  C5 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
      {
          string SqlLedTest = "";
          string SqlLedTestDetail = "";
          int k = 0;//燈箱編號位址
          int SignCnt = txt[5];
          bool InsertDb = false;

          DateTime t = DateTime.Now;

          for (int i = 1; i < SignCnt + 1; i++)
          {
              for (int j = 7 + (41 * (i - 1)); j < (7 + 40 * i); j++)
              {
                  if (txt[j] > 0)
                  {
                      k = 6 + 41 * (i - 1);
                      SqlLedTestDetail = "INSERT INTO db2inst1.tblLEDTestDetail_LCS(Devicename,Timestamp,sign_no,led_status1,led_status2,led_status3,led_status4,led_status5,led_status6,led_status7,led_status8,led_status9,led_status10,led_status11,led_status12,led_status13,led_status14,led_status15,led_status16,led_status17,led_status18,led_status19,led_status20,led_status21,led_status22,led_status23,led_status24,led_status25,led_status26,led_status27,led_status28,led_status29,led_status30,led_status31,led_status32,led_status33,led_status34,led_status35,led_status36,led_status37,led_status38,led_status39,led_status40) VALUES('" + devname + "','" + RemoteInterface.DbCmdServer.getTimeStampString(t) + "'," + txt[k] + "," + txt[k + 1] + "," + txt[k + 2] + "," + txt[k + 3] + "," + txt[k + 4] + "," + txt[k + 5] + "," + txt[k + 6] + "," + txt[k + 7] + "," + txt[k + 8] + "," + txt[k + 9] + "," + txt[k + 10] + "," + txt[k + 11] + "," + txt[k + 12] + "," + txt[k + 13] + "," + txt[k + 14] + "," + txt[k + 15] + "," + txt[k + 16] + "," + txt[k + 17] + "," + txt[k + 18] + "," + txt[k + 19] + "," + txt[k + 20] + "," + txt[k + 21] + "," + txt[k + 22] + "," + txt[k + 23] + "," + txt[k + 24] + "," + txt[k + 25] + "," + txt[k + 26] + "," + txt[k + 27] + "," + txt[k + 28] + "," + txt[k + 29] + "," + txt[k + 30] + "," + txt[k + 31] + "," + txt[k + 32] + "," + txt[k + 33] + "," + txt[k + 34] + "," + txt[k + 35] + "," + txt[k + 36] + "," + txt[k + 37] + "," + txt[k + 38] + "," + txt[k + 39] + "," + txt[k + 40] + ")";
                      this.InvokeDBDemand(SqlLedTestDetail);
                      SqlLedTestDetail = "";
                      InsertDb = true;
                      break;
                  }
              }
          }
          if (InsertDb)
          {
              SqlLedTest = "INSERT INTO db2inst1.tblLEDTest_LCS(Devicename,Timestamp,Sign_cnt) VALUES('" + devname + "','" + RemoteInterface.DbCmdServer.getTimeStampString(t) + "'," + txt[5] + ")";
              this.InvokeDBDemand(SqlLedTest);              
          }
      }



      void LCSTC_OnTCReport(object tc, TextPackage txt)
      {
#if DEBUG

          //if ((tc as TCBase).DeviceName != "LCS-T72-E-29.94")
          //    return;
#endif 
          

          if (txt.Text[0] == 0xc3)
          {
              System.Data.DataSet ds = m_protocol.GetSendDsByTextPackage(txt, CmdType.CmdReport);

              this.InvokeOutPutWrongEvent(GetDisplayDecs(currDisplayds),GetDisplayDecs(ds));
          }
          //throw new Exception("The method or operation is not implemented.");
      }



      public override void DownLoadConfig()
      {
         // throw new Exception("The method or operation is not implemented.");
      }


      protected override void CheckDisplayTask()
      {
         
          byte[] currSig = new byte[32];

          if (this.currDisplayds == null)
              return;

          if (!this.IsConnected)
              return;
         
          lock (currDispLockObj)
          {
              System.Data.DataSet ds;
             
              bool isequal = true;
              try
              {
                
                  SendPackage pkg;

                  foreach (System.Data.DataRow r in currDisplayds.Tables["tblsign_cnt"].Rows)

                      currSig[System.Convert.ToInt32(r["sign_no"])] = System.Convert.ToByte(r["sign_status"]);



                  ds = this.m_protocol.GetSendDataSet("get_ctl_sign");
                  pkg = this.m_protocol.GetSendPackage(ds, 0xffff);
                  this.Send(pkg);
                  ds = m_protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
                  // isequal = System.Convert.ToInt32(ds.Tables[0].Rows[0]["sign_cnt"]) == System.Convert.ToInt32(currDisplayds.Tables[0].Rows[0]["sign_cnt"]);
                  //if (!isequal)
                  //{
                  //    ConsoleServer.WriteLine("compare output different");
                  //    // this.TC_SetDislay(currDisplayds);
                  //}
                  //else
                  //{
                  for (int i = 0; i < System.Convert.ToInt32(ds.Tables[0].Rows[0]["sign_cnt"]); i++)
                  {
                      //  isequal  = isequal && (int)ds.Tables["tblsig_cnt"].Rows[i]["sign_no"]==(int)currDisplayds.Tables["tblsig_cnt"].Rows[i]["sign_no"];
                      int siginx = System.Convert.ToInt32(ds.Tables["tblsign_cnt"].Rows[i]["sign_no"]);
                      isequal = isequal && System.Convert.ToByte(ds.Tables["tblsign_cnt"].Rows[i]["sign_status"]) == currSig[siginx];
                       if(System.Convert.ToByte(ds.Tables["tblsign_cnt"].Rows[i]["sign_status"]) != currSig[siginx])
                           ConsoleServer.WriteLine("inx:"+siginx+",ds:"+ds.Tables["tblsign_cnt"].Rows[i]["sign_status"]+",currsig"+currSig[siginx]);
                  }
                  //}

                  if (!isequal)
                  {
                      ConsoleServer.WriteLine("compare output different");

                      this.InvokeOutPutWrongEvent(this.GetCurrentDisplayDecs(), this.GetDisplayDecs(ds));


                      this.TC_SetDislayForce(currDisplayds);
                  }
                  else
                  {
                      this.InvokeOutPutWrongEvent(this.GetCurrentDisplayDecs(), this.GetCurrentDisplayDecs());
                  }
              }
              catch (Exception ex)
              {
                  ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
              }


          }
      }
      public void TC_SetDislay(System.Data.DataSet ds)
      {
          checkConntected();

          string oldDispStr = "";
          if (ds.Tables[0].Rows[0]["func_name"].ToString() != "set_ctl_sign")
              throw new Exception("only support func_name = set_ctl_sign");


          if (this.currDisplayds == null )

          {
              lock (currDispLockObj)
              currDisplayds = ds;

              Comm.SendPackage pkg = this.m_protocol.GetSendPackage(ds, 0xffff);
              this.Send(pkg);
              
             this.InvokeOutPutChangeEvent(this, this.GetCurrentDisplayDecs());
              return;
          }
          else
             oldDispStr = this.GetCurrentDisplayDecs();
           

        
         
          lock(currDispLockObj)
              currDisplayds = ds;
          if (oldDispStr != this.GetCurrentDisplayDecs())
          {
              Comm.SendPackage pkg = this.m_protocol.GetSendPackage(ds, 0xffff);
              this.Send(pkg);
              this.InvokeOutPutChangeEvent(this, this.GetCurrentDisplayDecs());
          }

      
      }

      private void TC_SetDislayForce(System.Data.DataSet ds)
      {
          checkConntected();

      
          if (ds.Tables[0].Rows[0]["func_name"].ToString() != "set_ctl_sign")
              throw new Exception("only support func_name = set_ctl_sign");



              Comm.SendPackage pkg = this.m_protocol.GetSendPackage(ds, 0xffff);
              this.Send(pkg);







      }


      public override string GetCurrentDisplayDecs()
      {
          string outstr = "";
          if (currDisplayds == null)
              return "";

         
          for (int i = 0; i < currDisplayds.Tables[1].Rows.Count; i++)
          {
              outstr += "[" + currDisplayds.Tables[1].Rows[i]["sign_no"] + ":" + currDisplayds.Tables[1].Rows[i]["sign_status"] + "] ";
            
          }
          return outstr;
          //throw new Exception("The method or operation is not implemented.");
      }

      public  string GetDisplayDecs(System.Data.DataSet ds)
      {
          string outstr = "";
          if (ds == null)
              return "熄滅";

          int offcnt = 0;
          for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
          {
              outstr += "[" + ds.Tables[1].Rows[i]["sign_no"] + ":" + ds.Tables[1].Rows[i]["sign_status"] + "] ";
              if (System.Convert.ToInt32(ds.Tables[1].Rows[i]["sign_status"]) == 0)
                  offcnt++;
          }

          if (offcnt == ds.Tables[1].Rows.Count)
              return "熄滅";
          else
          return outstr;
          //throw new Exception("The method or operation is not implemented.");
      } 

      public  override void  TC_SetDisplayOff()
      {
          checkConntected();
          System.Data.DataSet ds,ds1;
          ds = this.m_protocol.GetSendDataSet("get_ctl_sign");
          this.m_protocol.GetSendPackage(ds, 0xffff);
          SendPackage pkg = this.m_protocol.GetSendPackage(ds, 0xffff);
          this.Send(pkg);
          ds = m_protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
          int sigcnt = System.Convert.ToInt32(ds.Tables[1].Rows.Count);
          ds1 = m_protocol.GetSendDataSet("set_ctl_sign");
          System.Data.DataRow r = ds1.Tables[0].Rows[0];
          r["sign_cnt"] = sigcnt;
          for (int i = 0; i < sigcnt; i++)
          {
              ds1.Tables["tblsign_cnt"].Rows.Add(ds.Tables[1].Rows[i]["sign_no"], 0);
             
          }

          ds1.AcceptChanges();
         
          lock (currDispLockObj)
          {
              if (currDisplayds != null )

                  this.InvokeOutPutChangeEvent(this, "熄滅");
              this.Send(m_protocol.GetSendPackage(ds1,0xffff));
             this.currDisplayds = null;
          }


          ds.Dispose();
          ds1.Dispose();

         
      }

      public override I_HW_Status_Desc getStatusDesc()
      {
          return new LCS_HW_StatusDesc(this.DeviceName, m_hwstaus);
      }

    }
}
