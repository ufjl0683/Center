using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HWStatus;
using Comm.TC;
using System.Data;


namespace Comm.TC
{
  public   class BSTC : TCBase
    {
     

        public DateTime last_receive_time;

      int curr_slope = -1, curr_shift = -1,curr_degree=0,curr_sink=-1;

        DataSet currData;
        public delegate void BSCycleDataHandler(Comm.TC.BSTC tc, System.DateTime dt, DataSet ds);
        public delegate void BSDegreeChangeHandler(Comm.TC.BSTC tc, System.DateTime dt, DataSet ds);
        public event BSCycleDataHandler On_BS_CycleData;
        public event BSDegreeChangeHandler On_DegreeChange;
        public BSTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus, byte comm_state)
           : base(protocol, devicename, ip, port, deviceid, hw_status,opmode,opstatus,comm_state)
          {


              m_hw_cycle = 0; //on change
              m_trans_cycle = 60; //60 min
              m_trans_mode = 1; //0:polling,1:active



              this.OnTCReport += new OnTCReportHandler(BSTC_OnTCReport);
              this.OnConnectStatusChanged += new ConnectStatusChangeHandler(BSTC_OnConnectStatusChanged);
              BSTC_OnConnectStatusChanged(this);
          }

      void BSTC_OnTCReport(object tc, TextPackage txt)
      {
         
              try
              {
                  if (txt.Text[0] == 0x68  )   // cycle data  and 主動回報
                  {



                      System.DateTime dt = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, txt.Text[6], txt.Text[7], txt.Text[8], 0);

                    
                        
                      last_receive_time = dt;

                      DataSet ds0x68=m_protocol.GetSendDsByTextPackage(txt,CmdType.CmdReport);
                     // this.currData = ds0x30;
                      curr_slope = System.Convert.ToInt32(ds0x68.Tables[0].Rows[0]["bs_slope"]);
                      curr_shift = System.Convert.ToInt32( ds0x68.Tables[0].Rows[0]["bs_shift"]);
                      curr_sink = System.Convert.ToInt32( ds0x68.Tables[0].Rows[0]["bs_sink"]);
                      curr_degree = System.Convert.ToInt32(ds0x68.Tables[0].Rows[0]["bs_degree"]);
                     

                      if (On_BS_CycleData != null)
                          this.On_BS_CycleData((BSTC)tc,dt, ds0x68);
                  }
                      /*

                  else if (txt.Text[0] == 0x32)  //event
                  {
                      System.DateTime dt = new DateTime(System.DateTime.Now.Year, txt.Text[6], txt.Text[7], txt.Text[8], txt.Text[9], 0);
                      DataSet ds0x32 = m_protocol.GetSendDsByTextPackage(txt, CmdType.CmdReport);
                      // this.currData = ds0x30;
                     // curr_day_var = System.Convert.ToInt32(ds0x32.Tables[0].Rows[0]["day_var"]);
                      //curr_day_var = System.Convert.ToInt32(ds0x32.Tables[0].Rows[0]["mon_var"]);
                      if (this.On_DegreeChange != null)
                          this.On_DegreeChange(this, dt, ds0x32);


                  }
                       * */
              }
              catch (Exception ex)
              {
                  ConsoleServer.WriteLine(this.DeviceName + "," + ex.Message);
              }
              //  this.dbServer.SendSqlCmd(string.Format(sql,  txt.Text[9], txt.Text[10] * 256 + txt.Text[11], txt.Text[12], tc.DeviceName, Comm.DB2.Db2.getTimeStampString(dt)));

          
          
      }

          public  void  getCurrentBSData( ref System.DateTime dt,ref int slope,ref int shift,ref int sink,ref int  degree)
          {
              dt = last_receive_time;
              slope = curr_slope;
              shift = curr_shift;
              sink = curr_sink;
              degree = curr_degree;
            
              /*
              dt = last_receive_time;
              day_var=curr_day_var;
              mon_var=curr_mon_var;
              degree=curr_degree;
               * */
             
          }

          public override void DownLoadConfig()
          {
              // throw new Exception("The method or operation is not implemented.");
          }

      void BSTC_OnConnectStatusChanged(object tc)
      {
         
          try
          {
              if (((TCBase)tc).IsConnected)
              {
                  try
                  {
                      this.TC_SendCycleSettingData();
                  }
                  catch (Exception ex1)
                  {
                      ConsoleServer.WriteLine(this.DeviceName + "," + ex1.Message);
                  }
                  ConsoleServer.WriteLine(((TCBase)tc).DeviceName + "  set cycle settting!");
                  System.DateTime dt = System.DateTime.Now.AddMinutes(-DateTime.Now.Minute);
                  byte[] data = new byte[] { 0x6d, (byte)dt.Day, (byte)dt.Hour, (byte)((dt.Minute/30) *30) };

                  try
                  {
                      Comm.SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.B, 0xffff, data);
                      this.Send(pkg);
                      if (pkg.ReturnTextPackage == null)
                          return;
                      try
                      {
                          DataSet ds = this.m_protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);

                          last_receive_time = dt;
                          this.currData = ds;
                      }
                      catch (Exception ex1)
                      {
                          ConsoleServer.WriteLine(this.DeviceName + "," + ex1.Message + "," + ex1.StackTrace);
                      }
                  }
                  catch (Exception ex)
                  {
                      ConsoleServer.WriteLine(this.DeviceName + "," + ex.Message+","+ex.StackTrace);
                  }
                  
                      
                    
                 
                  
              }
             
          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
          }
      }


          public override RemoteInterface.I_HW_Status_Desc getStatusDesc()
          {
              return new BS_HW_StatusDesc(this.DeviceName, this.m_hwstaus);
              //throw new Exception("The method or operation is not implemented.");
          }
    }
}
