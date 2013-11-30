using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HWStatus;
using Comm.TC;


namespace Comm.TC
{
  public   class VITC : TCBase
    {

        //private byte m_hw_cycle = 0; //on change
        //private byte m_trans_cycle = 5; //1:1min,5:5min
        //private byte m_trans_mode = 1; //0:polling,1:active
        public delegate void VIDegreeChangeHandler(Comm.TC.VITC tcc,int datatype, DateTime dt, int distance, int degree);
        public event VIDegreeChangeHandler On_DegreeChange;
        public delegate void VIFiveMinDataHandler(Comm.TC.VITC tcc, DateTime dt, int distance, int degree,int datatype);
        public event VIFiveMinDataHandler On_VI_FivceMinData;
        public int curr_distance=-1; //能見度
        public int curr_degree = -1;
        public DateTime last_receive_time;
       

      public VITC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus, byte comm_state)
           : base(protocol, devicename, ip, port, deviceid, hw_status,opmode,opstatus, comm_state)
          {

              m_hw_cycle = 0; //on change
              m_trans_cycle = 5; //1:1min,5:5min
              m_trans_mode = 1; //0:polling,1:active


            

              this.OnTCReport += new OnTCReportHandler(VITC_OnTCReport);
             this.OnConnectStatusChanged += new ConnectStatusChangeHandler(VITC_OnConnectStatusChanged);
          }


          //public override void OneMinTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) // 1 min 週期任務
          //{
          //    base.OneMinTimer_Elapsed(sender, e);
          //    if (!this.IsTcpConnected) return;

          //    try
          //    {
          //        TC_SendCycleSettingData();
          //    }
          //    catch { ;}

          //    //   Util.GC();


          //}


          void VITC_OnConnectStatusChanged(object tc)
          {
              // throw new Exception("The method or operation is not implemented.");
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
                      System.DateTime dt = System.DateTime.Now.AddMinutes(-5);
                      byte[] data = new byte[] { 0x25, (byte)dt.Day, (byte)dt.Hour, (byte)(dt.Minute / 5 * 5) };
                      Comm.SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.B, 0xffff, data);
                      this.Send(pkg);
                      curr_distance = (pkg.ReturnTextPackage.Text[8] * 256 + pkg.ReturnTextPackage.Text[9] == 65535) ? -1 : pkg.ReturnTextPackage.Text[8] * 256 + pkg.ReturnTextPackage.Text[9];
                      curr_degree = (pkg.ReturnTextPackage.Text[10]==255) ? -1 : pkg.ReturnTextPackage.Text[10];
                      last_receive_time = dt;
                  }
                  
              }
              catch (Exception ex)
              {
                  ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
              }
          }

          void VITC_OnTCReport(object tc, TextPackage txt)
          {
             
              try
              {
                  if (txt.Text[0] == 0x20 )   // cycle data  and 主動回報
                  {



                      System.DateTime dt = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, txt.Text[6], txt.Text[7], txt.Text[8], 0);

                     //urr_amount = (txt.Text[9] == 255) ? -1 : txt.Text[9];
                      curr_distance=(txt.Text[9] * 256 + txt.Text[10] == 65535) ? -1 : txt.Text[9] * 256 + txt.Text[10];
                      curr_degree = (txt.Text[11] == 255) ? -1 : txt.Text[11];
                      last_receive_time = dt;

                      if (On_VI_FivceMinData != null)
                          this.On_VI_FivceMinData(this, dt, curr_distance,  curr_degree,txt.Text[1]==0?1:0);
                  }
                  else if (txt.Text[0] == 0x22 )
                  {
                      System.DateTime dt = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, txt.Text[6], txt.Text[7], txt.Text[8], 0);
                      int distance = (txt.Text[9] * 256 + txt.Text[10] == 65535) ? -1 : txt.Text[9] * 256 + txt.Text[10];
                      int degree = (txt.Text[11] == 255) ? -1 : txt.Text[11];
                      if (this.On_DegreeChange != null)
                          this.On_DegreeChange(this,txt.Text[1], dt, distance, degree);
                  }
              }
              catch (Exception ex)
              {
                  ConsoleServer.WriteLine(this.DeviceName + "," + ex.Message);
              }
              //  this.dbServer.SendSqlCmd(string.Format(sql,  txt.Text[9], txt.Text[10] * 256 + txt.Text[11], txt.Text[12], tc.DeviceName, Comm.DB2.Db2.getTimeStampString(dt)));


          }

          public void getCurrentVIData(ref DateTime dt, ref int distance,  ref int degree)
          {
              dt = last_receive_time;
              distance=curr_distance;
             
              degree = curr_degree;
          }

          public override void DownLoadConfig()
          {
              // throw new Exception("The method or operation is not implemented.");
          }


          public override RemoteInterface.I_HW_Status_Desc getStatusDesc()
          {
              return new VI_HW_StatusDesc(this.DeviceName, this.m_hwstaus);
            
          }
    }
}
