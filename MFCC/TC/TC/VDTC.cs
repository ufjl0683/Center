using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HWStatus;
using System.Collections;

namespace Comm.TC
{


    public delegate void FiveMinAvgEventHandler(object vdtc,VD_MinAvgData data);
    public delegate void OnRealTimeEventHandler(object vdtc,System.Data.DataSet ds);
   public class VDTC:TCBase
    {

        //Protocol protocol;

       private byte m_trans_cycle = 1; //1:1min,5:5min
       private byte m_event_cycle = 20;//10~60
       private byte m_real_cycle = 60;// 30,60,150 sec
       private byte m_trans_mode = 1; //0:polling,1:active
       private byte m_event_mode = 1;//0:polling,1:active
       private byte m_real_mode = 1;//0:polling,1:active
       private byte m_hw_cycle = 0; //on change
       public static  byte conflict_occupancy = 40;
       public static byte conflict_speed = 40;
       public event FiveMinAvgEventHandler OnFiveMinAvgData;
       public event OnRealTimeEventHandler OnRealTimeData;


       private bool m_IsRealDataMode;
      // private Queue QueOneMinVDData = new Queue(10);
       private VD_OneMinDataStore OneMinDataStore ;


        public VDTC(Protocol protocol , string devicename,string ip, int port, int deviceid):base(protocol,devicename,ip,port,deviceid)
          {

              //this.protocol = protocol;
            
             // new System.Threading.Thread(checkModeAndPlannoTask).Start() ;

              OneMinDataStore = new VD_OneMinDataStore(this);
              this.OnTCReport += new OnTCReportHandler(VDTC_OnTCReport);
              this.OnConnectStatusChanged += new ConnectStatusChangeHandler(VDTC_OnConnectStatusChanged);
          }

       public bool IsRealDataMode
       {
           get
           {
               return m_IsRealDataMode;
           }
       }

       public void Tc_SetRealData(int laneid)
       {

           if (laneid == 0)
               this.m_IsRealDataMode = false;
           else
               this.m_IsRealDataMode = true;
           Comm.SendPackage pkg = new Comm.SendPackage(Comm.CmdType.CmdSet, Comm.CmdClass.B, 0xffff, new byte[] { 0x11, (byte)laneid });
           
           this.Send(pkg);
          
         
       }

       void VDTC_OnConnectStatusChanged(object tc)
       {
           //throw new Exception("The method or operation is not implemented.");
           if (((TCBase)tc).IsConnected)
           {
               getLast5VD_Data(); //取得最近五分鐘資料
           }
       }

       private void getLast5VD_Data() //取得最近五分鐘資料
       {

           System.DateTime dt = System.DateTime.Now.AddMinutes(-4);
           System.Data.DataSet ds;
           for (int i = 0; i < 5; i++)
           {
                
               byte[] sendData = new byte[] { 0x15, (byte)dt.Day, (byte)dt.Hour,(byte) dt.Minute };
               try
               {
                  SendPackage pkg;
                   
                   this.m_device.Send(pkg=new SendPackage(CmdType.CmdQuery, CmdClass.A, 0xffff, sendData));
                   if (pkg.result == CmdResult.ACK)
                   {
                       ds = m_protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
                       OneMinDataStore.inData(getOneMinAvgData(ds));
                       
                   }
                   else
                   {
                       ConsoleServer.WriteLine(pkg.result.ToString() + pkg.ToString());
                   }
                   
               }
               catch (Exception ex)
               {
                   ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
               }

               dt = dt.AddMinutes(1);

           }

           VD_MinAvgData fiveMinData = this.OneMinDataStore.getFiveMinMovingAvgData();
           ConsoleServer.WriteLine("five min avg:" + fiveMinData.ToString());
           if (this.OnFiveMinAvgData != null)
               this.OnFiveMinAvgData(this, fiveMinData);
       }

       void VDTC_OnTCReport(object tc, TextPackage txt)// 主動回報事件
       {
           //throw new Exception("The method or operation is not implemented.");
           System.Data.DataSet ds=null;
          
               if (txt.Text[0] == 0x10) //cycle data
               {
                   try
                   {
                       ds=m_protocol.GetReturnDsByTextPackage(txt);
                        VD_MinAvgData data= getOneMinAvgData(ds);
                       OneMinDataStore.inData(data);

                       VD_MinAvgData fiveMinData = this.OneMinDataStore.getFiveMinMovingAvgData();

                       if (this.OnFiveMinAvgData != null)
                           this.OnFiveMinAvgData(this, fiveMinData);


                       ConsoleServer.WriteLine("Five min AVG:" + fiveMinData.ToString());
                      

                   }
                   catch (Exception ex)
                   {
                       ConsoleServer.WriteLine(ex.Message);
                   }
               }
               else  if(txt.Cmd==0x17) // 20 sec 事件資料
               {
                  ConsoleServer.WriteLine(txt.ToString());
                  try
                  {
                      ds = m_protocol.GetReturnDsByTextPackage(txt);
                      //if (this.OnRealTimeData != null)
                      //    this.OnRealTimeData(this, ds);
                  }
                  catch (Exception ex)
                  {
                      ConsoleServer.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                  }
              }

              else if (txt.Cmd == 0x18)  //現點速率
              {
                  ConsoleServer.WriteLine(txt.ToString());
                  try
                  {
                      ds = m_protocol.GetReturnDsByTextPackage(txt);
                      if (this.OnRealTimeData != null)
                          this.OnRealTimeData(this, ds);
                  }
                  catch (Exception ex)
                  {
                      ConsoleServer.WriteLine("現點速率:"+ex.Message + "\r\n" + ex.StackTrace);
                  }
              }


               
          
       }
         
        public override I_HW_Status_Desc getStatusDesc()//硬體故障描述
        {
            //return this.getStatusDesc\
            return new VD_HW_StatusDesc(this.m_hwstaus);
        }

       public override void OneHourTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
       {
           base.OneHourTimer_Elapsed(sender, e);

           //do check unread data here
       }
       public override void OneMinTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) // 1 min 週期任務
       {
           base.OneMinTimer_Elapsed(sender, e);
           if (!this.IsConnected) return;

           sendCycleSettingData();

           Util.GC();

          // ConsoleServer.Write("outer 1min");
       }

       private  void sendCycleSettingData() //傳送傳輸週期設定
       {
           byte[] senddata = new byte[] {0x03,0, m_trans_cycle,m_trans_mode, m_hw_cycle };  //set trans cycle
           try
           {
               this.m_device.Send(new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, senddata));

               byte[] senddata1 = new byte[] { 0x03, 1, m_event_cycle, m_event_mode, m_hw_cycle };  //set event cycle
               this.m_device.Send(new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, senddata1));
               byte[] senddata2 = new byte[] { 0x03, 2, m_real_cycle, m_real_mode, m_hw_cycle };  //set real cycle
               this.m_device.Send(new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, senddata2));
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine("In VD 1min task:"+ex.Message);
           }
       }

       public VD_MinAvgData getOneMinAvgData(System.Data.DataSet ds)
       {
           System.Data.DataRow rmain = ds.Tables["tblMain"].Rows[0];

           VD_MinAvgData[] laneData = new VD_MinAvgData[ds.Tables["tbllane_count"].Rows.Count];
           int invalidcnt = 0;
           VD_MinAvgData ret = new VD_MinAvgData(this);

           ret.year = System.DateTime.Now.Year;
           ret.month = System.DateTime.Now.Month;
           ret.day = System.Convert.ToInt32(ds.Tables["tblmain"].Rows[0]["day"]);
           ret.hour = System.Convert.ToInt32(ds.Tables["tblmain"].Rows[0]["hour"]);
           ret.min = System.Convert.ToInt32(ds.Tables["tblmain"].Rows[0]["minute"]);

           for (int laneid = 0; laneid < ds.Tables["tbllane_count"].Rows.Count; laneid++)
           {
               int small_car_volume = 0, big_car_volume = 0, connect_car_volume = 0;
               int small_car_speed = 0, big_car_speed = 0, connect_car_speed = 0, average_occupancy = 0, average_car_interval = 0;
               System.Data.DataRow laneRow = ds.Tables["tbllane_count"].Rows[laneid];

               laneData[laneid] = new VD_MinAvgData(this);


               small_car_volume = System.Convert.ToInt32(laneRow["small_car_volume"]);

               big_car_volume = System.Convert.ToInt32(laneRow["big_car_volume"]);

               connect_car_volume = System.Convert.ToInt32(laneRow["connect_car_volume"]);
               big_car_speed = System.Convert.ToInt32(laneRow["big_car_speed"]);
               small_car_speed = System.Convert.ToInt32(laneRow["small_car_speed"]);
               connect_car_speed = System.Convert.ToInt32(laneRow["connect_car_speed"]);
               average_occupancy = System.Convert.ToInt32(laneRow["average_occupancy"]);
               average_car_interval = System.Convert.ToInt32(laneRow["average_car_interval"]);




               //  車道總流量計算

               if (small_car_volume == 0xff && big_car_volume == 0xff && connect_car_volume == 0xff)
                   laneData[laneid].vol = -1;
               else
               {
                   if (small_car_volume != 0xff)
                       laneData[laneid].vol += small_car_volume;
                   if (big_car_volume != 0xff)
                       laneData[laneid].vol += big_car_volume;
                   if (connect_car_volume != 0xff)
                       laneData[laneid].vol += connect_car_volume;
               }

               if (laneData[laneid].vol == -1 || connect_car_speed == 0xff && small_car_speed == 0xff && big_car_speed == 0xff)
                   laneData[laneid].speed = -1;
               else
               {
                   if (connect_car_volume != 0xff && connect_car_speed != 0xff)
                       laneData[laneid].speed += connect_car_volume * connect_car_speed;
                   if (small_car_volume != 0xff && small_car_speed != 0xff)
                       laneData[laneid].speed += small_car_volume * small_car_speed;

                   if (big_car_volume != 0xff && big_car_speed != 0xff)
                       laneData[laneid].speed += big_car_volume * big_car_speed;

               }

               if (laneData[laneid].speed == -1 || laneData[laneid].vol == -1)
                   laneData[laneid].speed = -1;
               else if (laneData[laneid].vol == 0)

                   laneData[laneid].speed = 0;
               else

                   laneData[laneid].speed = laneData[laneid].speed / laneData[laneid].vol;

               if (average_occupancy == 0xff)
                   laneData[laneid].occupancy = -1;
               else
                   laneData[laneid].occupancy = average_occupancy;

               laneData[laneid].interval = average_car_interval;

           }// for
           // 記算不分車道總流量
           invalidcnt = 0;
           for (int laneId = 0; laneId < laneData.Length; laneId++)
           {
               ret.vol += laneData[laneId].vol;
               if (laneData[laneId].vol == -1)
                   invalidcnt++;
           }

           if (ret.vol == laneData.Length * -1)
               ret.vol = -1;
           else
               ret.vol = ret.vol + invalidcnt + (ret.vol + invalidcnt) / (laneData.Length - invalidcnt) * invalidcnt;

           //計算平均車速

           invalidcnt = 0;
           int totalvol = 0;
           for (int laneId = 0; laneId < laneData.Length; laneId++)
           {
               if (laneData[laneId].vol != -1 && laneData[laneId].speed != -1)
               {
                   ret.speed += laneData[laneId].vol * laneData[laneId].speed;
                   totalvol += laneData[laneId].vol;
               }
               else
                   invalidcnt++;
           }

           if (invalidcnt == laneData.Length)
               ret.speed = -1;
           else if (totalvol == 0)
               ret.speed = 0;
           else
               ret.speed = ret.speed / totalvol;


           //計算平均佔有率

           invalidcnt = 0;
           for (int laneId = 0; laneId < laneData.Length; laneId++)
           {

               if (laneData[laneId].occupancy == -1)
                   invalidcnt++;
               else
                   ret.occupancy += laneData[laneId].occupancy;

           }

           if (invalidcnt == laneData.Length)
               ret.occupancy = -1;
           else
               ret.occupancy = ret.occupancy / (laneData.Length - invalidcnt);



           return ret;
       }
       //public void OneMinVD_DataTask()
       //{
       //}
    }
}
