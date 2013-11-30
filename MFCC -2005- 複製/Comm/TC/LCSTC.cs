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

                      this.InvokeOutPutWrongEvent(this.GetCurrentDisplayDecs(),this.GetDisplayDecs(ds));
                      

                      this.TC_SetDislayForce(currDisplayds);
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
              outstr += "[" + currDisplayds.Tables[1].Rows[i]["sign_no"] + ":" + currDisplayds.Tables[1].Rows[i]["sign_status"] + "] ";
          return outstr;
          //throw new Exception("The method or operation is not implemented.");
      }

      public  string GetDisplayDecs(System.Data.DataSet ds)
      {
          string outstr = "";
          if (ds == null)
              return "";
          for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
              outstr += "[" + ds.Tables[1].Rows[i]["sign_no"] + ":" + ds.Tables[1].Rows[i]["sign_status"] + "] ";
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
