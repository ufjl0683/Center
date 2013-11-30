using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HWStatus;
using Comm.TC;


namespace Comm.TC
{
  public   class WDTC : TCBase
    {
      //private byte m_hw_cycle = 0; //on change
      //  private byte m_trans_cycle = 5; //1:1min,5:5min
      //  private byte m_trans_mode = 1; //0:polling,1:active

        public DateTime last_receive_time;

      int curr_average_wind_speed = -1, curr_average_wind_direction = -1, curr_max_wind_speed = -1, curr_max_wind_direction = -1, curr_degree=-1;
         
        public delegate void WDCycleDataHandler(Comm.TC.WDTC tc, System.DateTime dt, int average_wind_speed, int average_wind_direction,int max_wind_speed,int max_wind_direction,int degree,int datatype);
        public event WDCycleDataHandler On_WD_CycleData;
        public delegate void WDDegreeChangeHandler(Comm.TC.WDTC tcc,int data_type, DateTime dt, int average_wind_speed, int average_wind_direction, int max_wind_speed, int max_wind_direction, int degree);
        public event WDDegreeChangeHandler On_DegreeChange;

      public WDTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus, byte comm_state)
           : base(protocol, devicename, ip, port, deviceid, hw_status,opmode,opstatus,comm_state)
          {


              m_hw_cycle = 0; //on change
              m_trans_cycle = 10; //1:1min,5:5min
              m_trans_mode = 1; //0:polling,1:active



              this.OnTCReport += new OnTCReportHandler(WDTC_OnTCReport);
              this.OnConnectStatusChanged += new ConnectStatusChangeHandler(WDTC_OnConnectStatusChanged);
          }

      void WDTC_OnTCReport(object tc, TextPackage txt)
      {
          //throw new Exception("The method or operation is not implemented.");



              // throw new Exception("The method or operation is not implemented.");
              try
              {
                  if (txt.Text[0] == 0x28  )   // cycle data  and 主動回報
                  {



                      System.DateTime dt = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, txt.Text[6], txt.Text[7], txt.Text[8], 0);

                     //urr_amount = (txt.Text[9] == 255) ? -1 : txt.Text[9];
                      this.curr_average_wind_speed=(txt.Text[9] == 255) ? -1 : txt.Text[9];
                      this.curr_average_wind_direction = (txt.Text[10] == 255) ? -1 : txt.Text[10];
                      this.curr_max_wind_speed = (txt.Text[11] == 255) ? -1 : txt.Text[11];
                      this.curr_max_wind_direction = (txt.Text[12] == 255) ? -1 : txt.Text[12];
                      this.curr_degree = (txt.Text[13] == 255) ? -1 : txt.Text[13];
                      last_receive_time = dt;

                      if (On_WD_CycleData != null)
                          this.On_WD_CycleData(this, dt, curr_average_wind_speed,curr_average_wind_direction,curr_max_wind_speed,curr_max_wind_direction,  curr_degree,txt.Text[1]==0?1:0);
                  }
                  else if(txt.Text[0]==0x2a)
                  {
                      int avgwindspd, avgwinddir, maxwindspd, maxwindir, amdegree;

                      avgwindspd = txt.Text[9];
                      avgwinddir = txt.Text[10];
                      maxwindspd = txt.Text[11];
                      maxwindir = txt.Text[12];
                      amdegree = txt.Text[13];
                      if (this.On_DegreeChange != null)
                          this.On_DegreeChange(this,txt.Text[1] , System.DateTime.Now, avgwindspd, avgwinddir, maxwindspd, maxwindir, amdegree);
                  }
              }
              catch (Exception ex)
              {
                  ConsoleServer.WriteLine(this.DeviceName + "," + ex.Message);
              }
              //  this.dbServer.SendSqlCmd(string.Format(sql,  txt.Text[9], txt.Text[10] * 256 + txt.Text[11], txt.Text[12], tc.DeviceName, Comm.DB2.Db2.getTimeStampString(dt)));


          
      }

          public  void  getCurrentWDData( ref System.DateTime dt, ref  int average_wind_speed, ref  int average_wind_direction, ref int  max_wind_speed, ref int  max_wind_direction, ref int  degree)
          {
             dt = last_receive_time;
             average_wind_speed = curr_average_wind_speed;
             average_wind_direction = curr_average_wind_direction;
             max_wind_speed = curr_max_wind_speed;
             max_wind_direction = curr_max_wind_direction;
             degree = curr_degree;
          }

          public override void DownLoadConfig()
          {
              // throw new Exception("The method or operation is not implemented.");
          }

      void WDTC_OnConnectStatusChanged(object tc)
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
                  byte[] data = new byte[] { 0x2d, (byte)dt.Day, (byte)dt.Hour, (byte)(dt.Minute / 5 * 5) };
                  Comm.SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.B, 0xffff, data);
                  this.Send(pkg);
                

                  byte[] ret = pkg.ReturnTextPackage.Text;

              
                  curr_average_wind_direction =(ret[9]==255)?-1:ret[9];
                  curr_average_wind_speed =(ret[8]==255)?-1:ret[8];
                  curr_max_wind_speed = (ret[10]==255)?-1:ret[10];
                  curr_max_wind_direction =(ret[11]==255)?-1:ret[11];
                  curr_degree = (ret[12]==255)?-1:ret[12];
                  last_receive_time = dt;
              }
             
          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
          }
      }


          public override RemoteInterface.I_HW_Status_Desc getStatusDesc()
          {
              return new WD_HW_StatusDesc(this.DeviceName, this.m_hwstaus);
              //throw new Exception("The method or operation is not implemented.");
          }
    }
}
