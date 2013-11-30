using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HWStatus;



namespace Comm.TC
{

    class ModeAndPlanData
    {
        public byte mode, planno;
        public  ModeAndPlanData(byte mode,byte planno)
        {
            this.mode = mode;
            this.planno = planno;
        }
    }

  public    class RMSTC:OutputTCBase
    {
    //  public event OnOutputChangedHandler OnOutputChanged;
      const int CHECK_COUNT_DOWN_MINUTE = 3;
      private byte curr_mode=2, curr_planno=0;
      public    bool isDisplayOff=true;
    //  public event OnOutputChangedHandler OnOutputChanged;

      public int curr_lamep = 0;

      public RMSTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status,byte opmode,byte opstatus,byte comm_state)
          : base(protocol, devicename, ip, port, deviceid, hw_status,opmode,opstatus, comm_state)
          {

            
              
          }

      

          public override string GetCurrentDisplayDecs()
          {
              if (isDisplayOff) return "";

              return "mode:" + curr_mode + " planno:" + curr_planno;

          }

          public void SetModeAndPlanno(byte mode,byte planno)
          {
              checkConntected();
            
              if (mode != curr_mode || (mode == 0 || mode == 4 || mode==5) && curr_planno != planno  || isDisplayOff)
              {
                  curr_mode = mode;
                  curr_planno = planno;
                  
                  new System.Threading.Thread(AsynTC_SetModeAndPlan).Start(new ModeAndPlanData(mode,planno));
                //  TC_SetModeAndPlanNo(mode, planno);

                  CheckCountDown = CHECK_COUNT_DOWN_MINUTE;
                  this.InvokeOutPutChangeEvent(this, this.GetCurrentDisplayDecs());
              }
              isDisplayOff = false;
          }

         private void AsynTC_SetModeAndPlan(object modeAndPlan)
          {
            //  byte[] modePaln = (byte[])modeAndPlan;
              try
              {
                  TC_SetModeAndPlanNo(((ModeAndPlanData)modeAndPlan).mode, ((ModeAndPlanData)modeAndPlan).planno);
              }
              catch (Exception ex)
              {
                  ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
              }
          }


          public override void DownLoadConfig()
          {
             // throw new Exception("The method or operation is not implemented.");
          }
          public override void TC_SetDisplayOff()
          {
              checkConntected();
              byte []data = new byte[] { 0xa5, 2 };
              if (!this.isDisplayOff )
                  this.InvokeOutPutChangeEvent(this, "儀控中止");

              SendPackage pkg=new SendPackage( CmdType.CmdSet,CmdClass.B,0xffff,data);
              Send(pkg);
              isDisplayOff=true;
              //throw new Exception("The method or operation is not implemented.");
          } 

          public override I_HW_Status_Desc getStatusDesc()
          {
             // throw new Exception("The method or operation is not implemented.")
              return new RMS_HW_StatusDesc(this.DeviceName,this.m_hwstaus);
          }

       volatile int CheckCountDown = 0;  // RMS 命令變更後兩各檢查週期內不執行比對
           protected override  void CheckDisplayTask() // void checkModeAndPlannoTask()
            {
              //  SendPackage pkg;
               byte mode=0,planno=0;
                //while (true)
                //{

               getRMSdata();
                
               if (CheckCountDown > 0)
               {
                   CheckCountDown--;
                   return;
               }

                    try
                    {
                        if (this.IsConnected)
                        {
                        //  pkg=  TC_GetCurrModeAndPlanNo();
                            this.TC_GetModeAndPlanno(ref  mode, ref  planno);

                         
                            if(!this.isDisplayOff)

                               
                             if (  mode != curr_mode || (mode==0 || mode==4) && curr_planno!=planno )
                             {
                                 ConsoleServer.WriteLine(this.ToString()+", mode or  planno err!");
                                 this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), "mode:" + mode + "," + "planno:" + planno);

                                 SetModeAndPlanno(curr_mode, curr_planno);
                                 CheckCountDown = CHECK_COUNT_DOWN_MINUTE;
                             }
                              
                             
                         // }

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(this.ToString() + "," + ex.Message);
                    }

                   // System.Threading.Thread.Sleep(30 * 1000);
                    this.getHwStaus();
                //}


            }

      private void getRMSdata()
      {

          try
          {
              SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.B, 0xffff, new byte[] { 0x87 });
              this.Send(pkg);
              if (pkg.result != CmdResult.ACK)
                  return;

              System.Data.DataSet ds = this.m_protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
              System.Data.DataRow r=ds.Tables[0].Rows[0];
              string output = "mode:{0} planno:{1} rmsrate:{2} ";
              output = string.Format(output, r["ctl_mode"], r["plan_no"], r["rmsrate"]);


              for(int i=0;i< ds.Tables["tblwarnsetno"].Rows.Count;i++)
              {
                  System.Data.DataRow rr=ds.Tables["tblwarnsetno"].Rows[i];

                  output += rr["warnset"] + ":" + rr["warn_message_id"] + " ";
              }


              string sql = "update tblrmsconfig set rmsdata='{0}' where devicename='{1}'";

              sql = string.Format(sql, output.Trim(), this.DeviceName);
              this.InvokeDBDemand(sql);

          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine("In getRMSdata"+ ex.Message + "," + ex.StackTrace);
          }

         // throw new Exception("The method or operation is not implemented.");
      }

          private void TC_SetModeAndPlanNo(byte mode, byte planno)
          {

              checkConntected();

              if (mode == 5) //手動燈號模式
              {
                SendPackage pkg;
                   try
                  {
                       pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0xA5, 01 });
                      m_device.Send(pkg);
                  }
                  catch { ;}


                  pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, new byte[] { 0x81,(byte)( planno%10),(byte)(planno/10) });

                  // planno > 10 開啟警示燈 <10 關閉警示燈
                  m_device.Send(pkg);
                  
              }

              else


              if (this.IsF311z)
              {
                  SendPackage pkg = null;
                  try
                  {
                      pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0xA5, 01 });
                      m_device.Send(pkg);
                  }
                  catch { ;}



                  //if (mode == 0 || mode == 4)  //fixed,local area integreted
                  //{

                  //    pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0x83, planno, 1, 1 });
                  //    m_device.Send(pkg);
                  //}

                  pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0x80, mode });
                  m_device.Send(pkg);


                  if (mode == 0 || mode == 4)  //fixed,local area integreted
                  {
                      System.Threading.Thread.Sleep(40 * 1000);  //wait for 90 sec
                      pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0x83, planno, 1, 1 });
                      m_device.Send(pkg);
                  }
                  
              }
              else
              {
                  SendPackage pkg = null;
                  try
                  {
                      pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0xA5, 01 });
                      m_device.Send(pkg);
                  }
                  catch { ;}


                  pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0x82, mode,planno });
                  m_device.Send(pkg);

              }

          string sql = "update tblRMSConfig set last_mode={0},last_planno={1} where devicename='{2}'";
              this.InvokeDBDemand(string.Format(sql, mode, planno, this.DeviceName));

          }

          //private void StartRampControl()
          //{
          //    SendPackage pkg = null;
          //    // new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0x04, 0xA5 });
          //    //  m_device.Send(pkg);

          //    //if (pkg.ReturnTextPackage.Text[8] == 2)
          //    //{
          //        pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0xA5, 01 });
          //        m_device.Send(pkg);
          //    //}

          //}
          //private void StopRampControl()
          //{
          //    SendPackage pkg = null;  
          //    // new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0x04, 0xA5 });
          //    // m_device.Send(pkg);

          //    //if (pkg.ReturnTextPackage.Text[8] == 1)
          //    //{
          //        pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0xA5, 02 });
          //        m_device.Send(pkg);
          //    //}
          //}

          //private SendPackage TC_GetCurrModeAndPlanNo()
          //{
          //    checkConntected();
          //    SendPackage pkg=new SendPackage(CmdType.CmdQuery,CmdClass.A,m_deviceid,new byte[]{0x04,0x82});
          //     m_device.Send(pkg);
               
          //    return  pkg;
          //}
         //private void TC_GetCurrModeAndPlanNo(ref byte mode, ref byte planno)
         // {
         //     checkConntected();
         //     SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, m_deviceid, new byte[] { 0x04, 0x82 });
         //    // m_device.Send(pkg);
               
         //  //   return pkg;
         // }

          private void TC_GetModeAndPlanno(ref byte mode, ref byte planno)  // for compare only
          {
              checkConntected();
              if (this.IsF311z)
              {
                  SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, m_deviceid, new byte[] { 0x04, 0x80 });
                  m_device.Send(pkg);
                  mode = pkg.ReturnTextPackage.Text[8];
                  pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, m_deviceid, new byte[] { 0x04, 0x83 });
                  m_device.Send(pkg);
                  planno = pkg.ReturnTextPackage.Text[8];
              }
              else
              {
                  SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, m_deviceid, new byte[] { 0x04, 0x82 });
                  m_device.Send(pkg);
                  mode = pkg.ReturnTextPackage.Text[8];
                  //pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, m_deviceid, new byte[] { 0x04, 0x83 });
                 // m_device.Send(pkg);
                  planno = pkg.ReturnTextPackage.Text[9];
              }

          }
          public void TC_GetCurrentModeAndPlanNo(ref byte mode,ref byte planno)
          {
              checkConntected();

             System.Data.DataSet ds= this.m_protocol.GetSendDataSet("get_hw_monitoring");
             SendPackage pkg = this.m_protocol.GetSendPackage(ds, 0xffff);
             m_device.Send(pkg);
             ds= this.m_protocol.GetReturnDsByTextPackage( pkg.ReturnTextPackage);
             mode = System.Convert.ToByte(ds.Tables[0].Rows[0]["ctl_mode"]);
             planno =System.Convert.ToByte( ds.Tables[0].Rows[0]["plan_no"]);
              //SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, m_deviceid, new byte[] { 0x04, 0x80 });
              //m_device.Send(pkg);
              //mode = pkg.ReturnTextPackage.Text[8];
              //pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, m_deviceid, new byte[] { 0x04, 0x83 });
              //m_device.Send(pkg);
              //planno = pkg.ReturnTextPackage.Text[8];
              

          }


     

          //public  void m_device_OnReport(object sender, TextPackage txtObj)
          //{
          //    throw new Exception("The method or operation is not implemented.");
          //}
      }
}
