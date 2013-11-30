using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HWStatus;
using System.Collections;
using Comm.DB2;
using System.Data.Odbc;

namespace Comm.TC
{


    public delegate void FiveMinAvgEventHandler(object vdtc,VD_MinAvgData data);
    public delegate void OnRealTimeEventHandler(object vdtc,System.Data.DataSet ds);
    public delegate void OnTriggerEventHandler(object vdtc,System.Data.DataSet ds);
    public delegate void On20SecDataHandler(object vdtc, VD_MinAvgData datas);
    public delegate void On1MinEventDataHandler(object vdtc, VD_MinAvgData data);
   public class VDTC:TCBase
    {

        //Protocol protocol;

     
       private byte m_event_cycle = 20;//10~60
       private byte m_real_cycle = 60;// 30,60,150 sec
    //   private byte m_trans_mode = 1; //0:polling,1:active
       private byte m_event_mode = 0;//0:polling,1:active  20 sec 事件停止
       private byte m_real_mode = 1;//0:polling,1:active
     //  private byte m_hw_cycle = 0; //on change
     //  private byte m_trans_cycle = 1; //1:1min,5:5min
    //   public static  byte conflict_occupancy = 40;
    //   public static byte conflict_speed = 40;
       public event FiveMinAvgEventHandler OnFiveMinAvgData;
       public event OnRealTimeEventHandler OnRealTimeData;
       public event OnTriggerEventHandler OnTriggerEvent;
       public event On20SecDataHandler On20SecEvent;
       public event On1MinEventDataHandler On1MinTrafficData;


      public  VD_MinAvgData curr_1_min_data;
     //  private bool m_IsRealDataMode;
      // private Queue QueOneMinVDData = new Queue(10);
       private VD_OneMinDataStore OneMinDataStore ;

       private DateTime m_realEventEndTime;
       //現點速率終止時間

       public VDTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus, byte comm_state)
           : base(protocol, devicename, ip, port, deviceid, hw_status,opmode,opstatus,comm_state)
          {

              this.m_hw_cycle = 0;
              this.m_trans_cycle = 1;
              this.m_trans_mode = 1;

              OneMinDataStore = new VD_OneMinDataStore(this);
              this.OnTCReport += new OnTCReportHandler(VDTC_OnTCReport);
              this.OnConnectStatusChanged += new ConnectStatusChangeHandler(VDTC_OnConnectStatusChanged);
          }


       public VD_MinAvgData getLatestFivMinAvgData()
       {
          return  this.OneMinDataStore.getFiveMinMovingAvgData();
       }

       public bool IsRealDataMode
       {
           get
           {
               if (m_realEventEndTime == null || System.DateTime.Now > m_realEventEndTime)
                   return false;
               else
                   return true;


               
           }
       }
       public override void DownLoadConfig()
       {
          // throw new Exception("The method or operation is not implemented.");
           try
           {
               System.Data.DataSet[] sendDS = getConfigDs(this.DeviceName);
               foreach (System.Data.DataSet ds in sendDS)
               {

                   this.Send(this.m_protocol.GetSendPackage(ds, 0xffff));


               }
           }
           catch { ;}

           
         
       }

       private System.Data.DataSet[] getConfigDs(string devName)
       {
         
          OdbcConnection cn = new OdbcConnection(Db2.db2ConnectionStr);
          OdbcCommand cmd = new OdbcCommand();
          cmd.Connection = cn;
          OdbcDataReader rd = null;

          // string devName = "";
          try
          {
              cn.Open();
              string selectCmd = "";
              selectCmd += "Select * From db2inst1.tblDeviceConfig devCfg,db2inst1.tblVdConfig VdCfg";
              selectCmd += " Where devCfg.DEVICENAME=VdCfg.DEVICENAME";
              selectCmd += "   and devCfg.DeviceName ='" + devName + "'";

              cmd.CommandText = selectCmd;
              rd = cmd.ExecuteReader();

              System.Data.DataSet[] dss = new System.Data.DataSet[3];
            //  int i = 0;
              rd.Read();
              System.Data.DataSet ds = null;
              System.Data.DataRow r = null;

              #region ++++ VD ++++
              dss = new System.Data.DataSet[3];
              #region ---- 設備狀態查詢 ----

              ds = this.m_protocol.GetSendDataSet("set_config_param");  //Replace here
              r = ds.Tables["tblmain"].Rows[0];
              r["loop_dist_lane_0"] = rd[Db2.getFiledInx(rd, "loop_dist_lane1")];
              r["loop_dist_lane_1"] = rd[Db2.getFiledInx(rd, "loop_dist_lane2")];
              r["loop_dist_lane_2"] = rd[Db2.getFiledInx(rd, "loop_dist_lane3")];
              r["loop_dist_lane_3"] = rd[Db2.getFiledInx(rd, "loop_dist_lane4")];
              r["loop_dist_lane_4"] = rd[Db2.getFiledInx(rd, "loop_dist_lane5")];
              r["loop_dist_lane_5"] = rd[Db2.getFiledInx(rd, "loop_dist_lane6")];
              r["lane_0"] = rd[Db2.getFiledInx(rd, "lane1")];
              r["lane_1"] = rd[Db2.getFiledInx(rd, "lane2")];
              r["lane_2"] = rd[Db2.getFiledInx(rd, "lane3")];
              r["lane_3"] = rd[Db2.getFiledInx(rd, "lane4")];
              r["lane_4"] = rd[Db2.getFiledInx(rd, "lane5")];
              r["lane_5"] = rd[Db2.getFiledInx(rd, "lane6")];
              r["small_car_max_length"] = rd[Db2.getFiledInx(rd, "small_car_max_length")];
              r["big_car_max_length"] = rd[Db2.getFiledInx(rd, "big_car_max_length")];
              r["vehicle_min_length"] = rd[Db2.getFiledInx(rd, "vehicle_min_length")];
              r["vehicle_max_length"] = rd[Db2.getFiledInx(rd, "vehicle_max_length")];
              r["max_speed"] = rd[Db2.getFiledInx(rd, "max_speed")];
              r["lane_cnt"] = rd[Db2.getFiledInx(rd, "lane_count")];
              r["delay_const"] = rd[Db2.getFiledInx(rd, "delay_const")];
              r["length"] = 36;
             // ds.Tables["tblmain"].Rows.Add(r);
              ds.AcceptChanges();
              dss[0] = ds;

              #endregion ---- 設備狀態查詢 ----
              #region ---- VD 車道數 & 偵測方向 ----

              ds = this.m_protocol.GetSendDataSet("set_lane_count_direction");
              r = ds.Tables["tblmain"].Rows[0];

              r["lane_count"] = rd[Db2.getFiledInx(rd, "lane_count")];
              int myByte = 0;
              if (rd[Db2.getFiledInx(rd, "detmap1")].ToString() == "1") myByte += 1;
              if (rd[Db2.getFiledInx(rd, "detmap2")].ToString() == "1") myByte += 2;
              if (rd[Db2.getFiledInx(rd, "detmap3")].ToString() == "1") myByte += 4;
              if (rd[Db2.getFiledInx(rd, "detmap4")].ToString() == "1") myByte += 8;
              if (rd[Db2.getFiledInx(rd, "detmap5")].ToString() == "1") myByte += 16;
              if (rd[Db2.getFiledInx(rd, "detmap6")].ToString() == "1") myByte += 32;
              r["detmap"] = myByte;
             // ds.Tables["tblmain"].Rows.Add(r);
              ds.AcceptChanges();
              dss[1] = ds;

              #endregion ---- VD 車道數 & 偵測方向 ----
              #region ---- VD 觸動組態 ----

              ds = this.m_protocol.GetSendDataSet("set_trig_config");
            //  r = ds.Tables[1].NewRow();

              ds.Tables[0].Rows[0]["lane_count"] = rd[Db2.getFiledInx(rd, "lane_count")];

              //第一車道觸動組態
              for (int i = 1; i <= System.Convert.ToInt32(ds.Tables[0].Rows[0]["lane_count"]); i++)
              {
                  r = ds.Tables[1].NewRow();
                  r["lane_id"]=i;
                  r["occ_time_limit"] = rd[Db2.getFiledInx(rd, "occ_time_limit_lane"+i)];
                  ds.Tables[1].Rows.Add(r);
              }
              ////第二車道觸動組態
              //r["occ_time_limit_lane2"] = rd[Db2.getFiledInx(rd, "occ_time_limit_lane2")];
              ////第三車道觸動組態
              //r["occ_time_limit_lane3"] = rd[Db2.getFiledInx(rd, "occ_time_limit_lane3")];
              ////第四車道觸動組態
              //r["occ_time_limit_lane4"] = rd[Db2.getFiledInx(rd, "occ_time_limit_lane4")];
              ////第五車道觸動組態
              //r["occ_time_limit_lane5"] = rd[Db2.getFiledInx(rd, "occ_time_limit_lane5")];
              ////第六車道觸動組態
              //r["occ_time_limit_lane6"] = rd[Db2.getFiledInx(rd, "occ_time_limit_lane6")];
              ds.AcceptChanges();
              dss[2] = ds;

              #endregion ---- VD 觸動組態 ----
              #endregion ++++ VD ++++


           

              return dss;
          }
          catch (Exception ex)
          {
              throw new Exception(ex.Message + ex.StackTrace);
          }
          finally
          {
              rd.Close();
              cn.Close();
          }



       }


       public void SetRealData(int laneid, int cycle, int durationMin)
       {
           //try
           //{

               if (laneid != 0)
               {
                   this.m_real_cycle = (byte)cycle;
                   this.m_realEventEndTime = System.DateTime.Now.AddMinutes(durationMin);
                   this.TC_SendCycleSettingData();
               }
               Tc_SetRealData(laneid);
           //}
           //catch (Exception ex)
           //{
           //    ConsoleServer.WriteLine( DeviceName+ex.Message+ex.StackTrace);
           //}
       }
       public override void SetTransmitCycle(int cycle)
       {
           this.m_trans_cycle = (byte)cycle;
           this.TC_SendCycleSettingData();
       }
       private void Tc_SetRealData(int laneid)
       {

          

               Comm.SendPackage pkg = new Comm.SendPackage(Comm.CmdType.CmdSet, Comm.CmdClass.B, 0xffff, new byte[] { 0x11, (byte)laneid });

               this.Send(pkg);
           
         
       }
       public System.Data.DataSet Tc_GetVDData(System.DateTime dt)
       {
           System.Data.DataSet ds = this.m_protocol.GetSendDataSet("get_unread_data");
           ds.Tables[0].Rows[0]["day"] = dt.Day;
           ds.Tables[0].Rows[0]["hour"] = dt.Hour;
           ds.Tables[0].Rows[0]["minute"] = dt.Minute;
           ds.AcceptChanges();
           System.Data.DataSet retDs;
           SendPackage pkg = m_protocol.GetSendPackage(ds,0xffff);
           this.Send(pkg);
           try
           {
               if (pkg.ReturnTextPackage == null)
                   throw new Exception(this.DeviceName + ",無法回補" + dt.ToString() + "資料!");
               retDs = m_protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message + pkg.ReturnTextPackage);
               throw ex;
           }

           return retDs;


       }

       void VDTC_OnConnectStatusChanged(object tc)
       {
           //throw new Exception("The method or operation is not implemented.");
           if (((TCBase)tc).IsConnected)
           {
               try
               {
                   this.TC_SendCycleSettingData();
                   ConsoleServer.WriteLine(((TCBase)tc).DeviceName + "  set cycle settting!");
                   getLast5VD_Data(); //取得最近五分鐘資料
               }
               catch (Exception ex)
               {
                   ConsoleServer.WriteLine(this.DeviceName + ex.Message + ex.StackTrace);
               }
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
                   
                   this.m_device.Send(pkg=new SendPackage(CmdType.CmdQuery, CmdClass.C, 0xffff, sendData));
                   if (pkg.result == CmdResult.ACK)
                   {
                       if (pkg.ReturnTextPackage == null)
                           Console.WriteLine("PAUSER HERE");

                       ds = m_protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
                       OneMinDataStore.inData(getOneMinAvgData(ds,dt.Year,dt.Month));
                       
                   }
                   else
                   {
                       ConsoleServer.WriteLine(this.DeviceName+ " "+pkg.result.ToString() + pkg.ToString());
                   }
                   
               }
               catch (Exception ex)
               {
                   ConsoleServer.WriteLine(this.DeviceName+" "+ex.Message + ex.StackTrace);
               }

               dt = dt.AddMinutes(1);

           }

           VD_MinAvgData fiveMinData = this.OneMinDataStore.getFiveMinMovingAvgData();
           // 顯示取得最近5分鐘資料
          // ConsoleServer.WriteLine(this.DeviceName+" "+"five min avg:" + fiveMinData.ToString());
           if (this.OnFiveMinAvgData != null)
               this.OnFiveMinAvgData(this, fiveMinData);
       }

       DateTime dtlast0x10 = new DateTime();
       void VDTC_OnTCReport(object tc, TextPackage txt)// 主動回報事件
       {
          
      
           System.Data.DataSet ds=null;
          
               if (txt.Text[0] == 0x10  ) //cycle data 1 min cycle
               {
                   try
                   {
                       ds=m_protocol.GetSendDsByTextPackage(txt,CmdType.CmdReport);
                       ds.AcceptChanges();
                       if (true/*System.Convert.ToInt32(ds.Tables[0].Rows[0]["response_type"]) == 0 || System.Convert.ToInt32(ds.Tables[0].Rows[0]["response_type"]) == 4*/)
                       {

                         
                           VD_MinAvgData data = getOneMinAvgData(ds,System.DateTime.Now.Year,System.DateTime.Now.Month);
                           curr_1_min_data = data;
                           if (dtlast0x10 != data.dateTime)
                                OneMinDataStore.inData(data);

                         // if(dtlast0x10!=data.dateTime)
                           VD_MinAvgData fiveMinData = this.OneMinDataStore.getFiveMinMovingAvgData();
                           


                           if (this.OnFiveMinAvgData != null)
                               try
                               {
                                   this.OnFiveMinAvgData(this, fiveMinData);
                               }
                               catch (Exception ex)
                               {
                                   ConsoleServer.WriteLine(this.DeviceName+ex.Message+ex.StackTrace);
                               }
                           if (this.On1MinTrafficData != null)
                               try
                               {
                                   if (dtlast0x10 != data.dateTime)
                                       this.On1MinTrafficData(this, data);
                                   else
                                       ConsoleServer.WriteLine(this.DeviceName+"1min data repeat!");


                               }
                               catch (Exception ex) { ConsoleServer.WriteLine(this.DeviceName+ex.Message+ex.StackTrace); };

                           dtlast0x10 = data.dateTime;
                           // 顯示取得最近1分鐘資料
                        //   ConsoleServer.WriteLine(this.DeviceName + " " + "Five min AVG:" + fiveMinData.ToString());
                       }
                      

                   }
                   catch (Exception ex)
                   {
                       ConsoleServer.WriteLine(this.DeviceName+" "+"Five min AVG"+ex.Message);
                   }
               }
               else  if(txt.Cmd==0x17) // 20 sec 事件資料
               {
                //  ConsoleServer.WriteLine(txt.ToString());
                  try
                  {
                      ds = m_protocol.GetSendDsByTextPackage(txt, CmdType.CmdReport);
                      ds.AcceptChanges();
                      if (this.On20SecEvent != null)
                          this.On20SecEvent(this, getOneMinAvgData(ds, System.DateTime.Now.Year, System.DateTime.Now.Month));
                      //if (this.OnRealTimeData != null)
                      //    this.OnRealTimeData(this, ds);
                  }
                  catch (Exception ex)
                  {
                      ConsoleServer.WriteLine(this.DeviceName+" "+" 20 sec 事件資料"+ex.Message + "\r\n" + ex.StackTrace);
                  }
              }
                  

              else if (txt.Cmd == 0x18)  //現點速率
              {
                
                  try
                  {
                      ConsoleServer.WriteLine(txt.ToString());
                      if (System.DateTime.Now >= m_realEventEndTime)
                      {
                          ConsoleServer.WriteLine(this.DeviceName + "關閉現點速率");
                          //this.Tc_SetRealData(0);
                          AsyncSend(new SendPackage(CmdType.CmdSet,CmdClass.B,0xffff,new byte[]{0x11,00}));
                          return;
                      }
                      ds = m_protocol.GetSendDsByTextPackage(txt, CmdType.CmdReport);
                      ds.AcceptChanges();
                      if (this.OnRealTimeData != null)
                          this.OnRealTimeData(this, ds);
                  }
                  catch (Exception ex)
                  {
                      ConsoleServer.WriteLine(this.DeviceName+" "+"現點速率:"+ex.Message + "\r\n" + ex.StackTrace);
                  }
              }
              else if (txt.Cmd == 0x1a)  //觸動事件
              {
                  try
                  {
                      ds = m_protocol.GetSendDsByTextPackage(txt, CmdType.CmdReport);
                      ds.AcceptChanges();
                      if (this.OnTriggerEvent != null)
                          this.OnTriggerEvent(this, ds);
                  }
                  catch (Exception ex)
                  {
                      ConsoleServer.WriteLine(this.DeviceName+" "+"觸動事件:" + ex.Message + "\r\n" + ex.StackTrace);
                  }
              }


               
          
       }
         
        public override I_HW_Status_Desc getStatusDesc()//硬體故障描述
        {
            //return this.getStatusDesc\
            return new VD_HW_StatusDesc(this.DeviceName,this.m_hwstaus);
        }

       public override void OneHourTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
       {
           base.OneHourTimer_Elapsed(sender, e);

           //do check unread data here
       }
       public override void OneMinTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) // 1 min 週期任務
       {
           base.OneMinTimer_Elapsed(sender, e);
           //if (!this.IsTcpConnected) return;
           //try
           //{
           //    TC_SendCycleSettingData();
           //}
           //catch { ;}

        //   Util.GC();

         
       }

       public  override void TC_SendCycleSettingData() //傳送傳輸週期設定
       {
           if (!this.IsTcpConnected) return;
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
               ConsoleServer.WriteLine(this.DeviceName+" "+"In VD 1min task:"+ex.Message);
           }
       }

       public VD_MinAvgData getOneMinAvgData(System.Data.DataSet ds,int year,int month)
       {
           System.Data.DataRow rmain = ds.Tables["tblMain"].Rows[0];

           VD_MinAvgData[] laneData = new VD_MinAvgData[ds.Tables["tbllane_count"].Rows.Count];
           int invalidcnt = 0;
           VD_MinAvgData ret = new VD_MinAvgData(this.DeviceName);

           ret.year = year ;
           ret.month = month;
           ret.day = System.Convert.ToInt32(ds.Tables["tblmain"].Rows[0]["day"]);
           ret.hour = System.Convert.ToInt32(ds.Tables["tblmain"].Rows[0]["hour"]);
           ret.min = System.Convert.ToInt32(ds.Tables["tblmain"].Rows[0]["minute"]);
           if (ds.Tables[0].Rows[0]["func_name"].ToString() == "report_event_data") // for 20sec event data
               ret.sec =System.Convert.ToInt32( ds.Tables[0].Rows[0]["second"]);
           ret.orgds = ds;

           for (int laneid = 0; laneid < ds.Tables["tbllane_count"].Rows.Count; laneid++)
           {
               int small_car_volume = 0, big_car_volume = 0, connect_car_volume = 0;
               int small_car_speed = 0, big_car_speed = 0, connect_car_speed = 0, average_occupancy = 0, average_car_interval = 0;
               int small_car_length = 0, big_car_length = 0, connect_car_length = 0;
               System.Data.DataRow laneRow = ds.Tables["tbllane_count"].Rows[laneid];

               laneData[laneid] = new VD_MinAvgData(this.DeviceName);

               laneData[laneid].year = ret.year;
               laneData[laneid].month = ret.month;
               laneData[laneid].day = ret.day;
               laneData[laneid].hour = ret.hour;
               laneData[laneid].min= ret.min;
               laneData[laneid].sec = ret.sec;
               small_car_volume = System.Convert.ToInt32(laneRow["small_car_volume"]);

               big_car_volume = System.Convert.ToInt32(laneRow["big_car_volume"]);

               connect_car_volume = System.Convert.ToInt32(laneRow["connect_car_volume"]);
               big_car_speed = System.Convert.ToInt32(laneRow["big_car_speed"]);
               small_car_speed = System.Convert.ToInt32(laneRow["small_car_speed"]);
               connect_car_speed = System.Convert.ToInt32(laneRow["connect_car_speed"]);
               average_occupancy = System.Convert.ToInt32(laneRow["average_occupancy"]);
               //if (average_occupancy == 255)
               //    average_occupancy = -1;
               average_car_interval = System.Convert.ToInt32(laneRow["average_car_interval"]);

               if (average_car_interval == 65535 )
                   average_car_interval = -1;
               //if (average_car_interval == 255)
               //    average_car_interval = -1;


               small_car_length = System.Convert.ToInt32(laneRow["small_car_length"]);
               big_car_length = System.Convert.ToInt32(laneRow["big_car_length"]);
               connect_car_length = System.Convert.ToInt32(laneRow["connect_car_length"]);

               if (small_car_volume == 0 && small_car_speed > 0 || small_car_volume == 0 && small_car_length > 0)
               {
                   laneData[laneid].small_car_volume = -1;
               }
               else
               {
                   laneData[laneid].small_car_volume = (small_car_volume == 255) ? -1 : small_car_volume;
                   if (!Comm.TC.VDDataValidCheck.SpeedVolumnRangeCheck(laneData[laneid].speed, laneData[laneid].vol))
                   {
                       laneData[laneid].small_car_volume = -1;
                      // laneData[laneid].small_car_speed = -1;
                   }

               }


               if (big_car_volume == 0 && big_car_speed > 0 || big_car_volume == 0 && big_car_length > 0)
               {
                   laneData[laneid].big_car_volume = -1;
               }
               else
               {
                   laneData[laneid].big_car_volume = (big_car_volume == 255) ? -1 : big_car_volume;
                   if (!Comm.TC.VDDataValidCheck.SpeedVolumnRangeCheck(laneData[laneid].speed, laneData[laneid].vol))
                   {
                       laneData[laneid].big_car_volume = -1;
                      // laneData[laneid].big_car_speed = -1;
                   }
               }


               if (connect_car_volume == 0 && connect_car_speed > 0 || connect_car_volume == 0 && connect_car_length > 0)
               {
                   laneData[laneid].connect_car_volume = -1;
               }
               else
               {
                   laneData[laneid].connect_car_volume = (connect_car_volume == 255) ? -1 : connect_car_volume;
                   if (!Comm.TC.VDDataValidCheck.SpeedVolumnRangeCheck(laneData[laneid].speed, laneData[laneid].vol))
                   {
                       laneData[laneid].connect_car_volume = -1;
                      // laneData[laneid].connect_car_speed = -1;
                   }
               }

               
               laneData[laneid].small_car_speed = (small_car_speed == 255 || small_car_volume==-1) ? -1 : small_car_speed;
               laneData[laneid].big_car_speed=(big_car_speed==255 || big_car_volume==-1)?-1:big_car_speed;
               laneData[laneid].connect_car_speed = (connect_car_speed == 255) ? -1 : connect_car_speed;
               laneData[laneid].occupancy = (average_occupancy == 255) ? -1 : average_occupancy;

               //if(average_car_interval==65535 || average_car_interval==255)
               //    Console.WriteLine("wild!");
               laneData[laneid].interval = average_car_interval;
               laneData[laneid].small_car_length = (small_car_length == 255) ? -1 : small_car_length;
               laneData[laneid].big_car_length = (big_car_length == 255) ? -1 : big_car_length;
               laneData[laneid].connect_car_length = (connect_car_length == 255) ? -1 : connect_car_length;

              



/*
               ret.small_car_volume+=(small_car_volume==-1)?0:small_car_volume;
               ret.big_car_volume+=(big_car_volume == -1) ? 0 : big_car_volume;
               ret.connect_car_volume += (connect_car_volume == -1) ? 0 : connect_car_volume;

 * */

               /*
               ret.connect_car_speed += (laneData[laneid].connect_car_speed == -1) ? 0 : laneData[laneid].connect_car_speed;
               ret.small_car_speed += (laneData[laneid].small_car_speed == -1) ? 0 : laneData[laneid].small_car_speed;
               ret.big_car_speed += (laneData[laneid].big_car_speed == -1) ? 0 : laneData[laneid].big_car_speed;

               ret.connect_car_length += (laneData[laneid].connect_car_length == -1) ? 0 : laneData[laneid].connect_car_length;
               ret.small_car_length += (laneData[laneid].small_car_length == -1) ? 0 : laneData[laneid].small_car_length;
               ret.big_car_length += (laneData[laneid].big_car_length == -1) ? 0 : laneData[laneid].big_car_length;
                * 
                */

 





               //  車道總流量計算

               if ( laneData[laneid].small_car_volume == -1 || laneData[laneid].big_car_volume ==-1 ||  laneData[laneid].connect_car_volume == -1  )
               {
                   laneData[laneid].vol = -1;
                   laneData[laneid].big_car_volume = laneData[laneid].small_car_volume = laneData[laneid].connect_car_volume = -1;
               }
               else
               {
                   //if (small_car_volume != 0xff)
                   //{
                       laneData[laneid].vol += small_car_volume;
                     
                   //}
                   //else
                   //    laneData[laneid].small_car_volume = -1;

                   //if (big_car_volume != 0xff)
                   //{
                       laneData[laneid].vol += big_car_volume;
                     
                   //}
                   //else
                   //    laneData[laneid].big_car_volume = -1;

                   //if (connect_car_volume != 0xff)
                   //{
                       laneData[laneid].vol += connect_car_volume;

                   //}
                   //else
                   //    laneData[laneid].connect_car_volume = -1;
                  
               }
 

             



               if (laneData[laneid].vol == -1 || laneData[laneid].connect_car_speed == -1 || laneData[laneid].small_car_speed ==-1 || laneData[laneid].big_car_speed == -1)
               {
                   laneData[laneid].speed = -1;
                   connect_car_speed = small_car_speed = big_car_speed = -1;
               }
               else
               {
                 //  if (connect_car_volume != 0xff && connect_car_speed != 0xff)
                       laneData[laneid].speed += connect_car_volume * connect_car_speed;
                  // if (small_car_volume != 0xff && small_car_speed != 0xff)
                       laneData[laneid].speed += small_car_volume * small_car_speed;

                  // if (big_car_volume != 0xff && big_car_speed != 0xff)
                       laneData[laneid].speed += big_car_volume * big_car_speed;

               }

               if (laneData[laneid].speed == -1 || laneData[laneid].vol == -1)
                   laneData[laneid].speed = -1;

               else if (laneData[laneid].vol == 0)

                   laneData[laneid].speed = 0;
               else

                   laneData[laneid].speed = laneData[laneid].speed / laneData[laneid].vol;

               //車道平均車長計算


               if (laneData[laneid].vol == -1 || connect_car_length == 0xff || small_car_length == 0xff || big_car_length == 0xff)
               {
                   laneData[laneid].length = -1;
                   connect_car_length = small_car_length = big_car_length = -1;
               }
               else
               {
                 //  if (connect_car_length != 0xff && connect_car_volume != 0xff)
                       laneData[laneid].length += connect_car_volume * connect_car_length;
                //   if (small_car_volume != 0xff && small_car_length != 0xff)
                       laneData[laneid].length += small_car_volume *small_car_length;

               //    if (big_car_volume != 0xff && big_car_length != 0xff)
                       laneData[laneid].length += big_car_volume * big_car_length;

               }




               if (laneData[laneid].length == -1 || laneData[laneid].vol == -1)
                   laneData[laneid].length= -1;
               else if (laneData[laneid].vol == 0)

                   laneData[laneid].length = 0;
               else

                   laneData[laneid].length = laneData[laneid].length / laneData[laneid].vol;






               if (average_occupancy == 0xff)
                   laneData[laneid].occupancy = -1;
               else
                   laneData[laneid].occupancy = average_occupancy;

             //  laneData[laneid].interval = average_car_interval;

           }// for
           // 記算不分車道總流量


           invalidcnt = 0;
           ret.vol = 0;
           for (int laneId = 0; laneId < laneData.Length; laneId++)
           {
              // ret.vol += laneData[laneId].vol;
               if (laneData[laneId].vol == -1)
                   invalidcnt++;
               else
                   ret.vol += laneData[laneId].vol;
           }

           if (invalidcnt == laneData.Length)
               ret.vol = -1;
           else
           {
               ret.vol = ret.vol + invalidcnt + (ret.vol + invalidcnt) / (laneData.Length - invalidcnt) * invalidcnt;
               if(invalidcnt!=0)
                ret.isReCalculate = true;
              // this.is
           }

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
           totalvol = 0;
           for (int laneId = 0; laneId < laneData.Length; laneId++)
           {

               if (laneData[laneId].vol == -1)
                   invalidcnt++;
               else
               {
                   ret.occupancy += laneData[laneId].occupancy * laneData[laneId].vol;
                   totalvol += laneData[laneId].vol;
               }

           }
           if (invalidcnt == laneData.Length)
               ret.occupancy= -1;
           else if (totalvol == 0)
               ret.occupancy = 0;
           else
               ret.occupancy = ret.occupancy / totalvol;
           //invalidcnt = 0;
        
           //for (int laneId = 0; laneId < laneData.Length; laneId++)
           //{

           //    if (laneData[laneId].occupancy == -1)
               
           //        invalidcnt++;
           //    else
           //        ret.occupancy += laneData[laneId].occupancy;

           //}

           //if (invalidcnt == laneData.Length)
           //    ret.occupancy = -1;
           //else
           //    ret.occupancy = ret.occupancy / (laneData.Length - invalidcnt);

          


           //計算平均車長

           invalidcnt = 0;
           totalvol = 0;
           for (int laneId = 0; laneId < laneData.Length; laneId++)
           {

               if (laneData[laneId].vol == -1 )
                   invalidcnt++;
               else
               {
                   ret.length += laneData[laneId].length * laneData[laneId].vol;
                   totalvol += laneData[laneId].vol;
               }

           }
           if (invalidcnt == laneData.Length)
               ret.length = -1;
           else if (totalvol == 0)
               ret.length = 0;
           else
               ret.length = ret.length / totalvol;

           //計算平均interval

           invalidcnt = 0;
           totalvol = 0;
           for (int laneId = 0; laneId < laneData.Length; laneId++)
           {

               if (laneData[laneId].vol == -1)
                   invalidcnt++;
               else
               {
                   ret.interval += laneData[laneId].interval * laneData[laneId].vol;
                   totalvol += laneData[laneId].vol;
               }

           }
           if (invalidcnt == laneData.Length)
               ret.interval = -1;
           else if (totalvol == 0)
               ret.interval = 0;
           else
               ret.interval = ret.interval / totalvol;


           //計算平均interval


           ret.orgVDLaneData = laneData;



           //計算平均 各車種   車流 


           invalidcnt = 0;
           totalvol = 0;
           for (int laneId = 0; laneId < laneData.Length; laneId++)
           {

               if (laneData[laneId].small_car_volume == -1)
                   invalidcnt++;
               else
               {
                   ret.small_car_volume += laneData[laneId].small_car_volume ;
                  // totalvol += laneData[laneId].small_car_volume;
               }

           }
           if (invalidcnt == laneData.Length)
               ret.small_car_volume = -1;


           invalidcnt = 0;
           totalvol = 0;
           for (int laneId = 0; laneId < laneData.Length; laneId++)
           {

               if (laneData[laneId].big_car_volume == -1)
                   invalidcnt++;
               else
               {
                   ret.big_car_volume += laneData[laneId].big_car_volume;
                   // totalvol += laneData[laneId].small_car_volume;
               }

           }
           if (invalidcnt == laneData.Length)
               ret.big_car_volume = -1;


           invalidcnt = 0;
           totalvol = 0;
           for (int laneId = 0; laneId < laneData.Length; laneId++)
           {

               if (laneData[laneId].connect_car_volume== -1)
                   invalidcnt++;
               else
               {
                   ret.connect_car_volume += laneData[laneId].connect_car_volume;
                   // totalvol += laneData[laneId].small_car_volume;
               }

           }
           if (invalidcnt == laneData.Length)
               ret.connect_car_volume = -1;

             // 各車種長度

           invalidcnt = 0;
           totalvol = 0;
           for (int laneId = 0; laneId < laneData.Length; laneId++)
           {

               if (laneData[laneId].small_car_volume== -1)
                   invalidcnt++;
               else
               {
                   ret.small_car_length += laneData[laneId].small_car_length*laneData[laneId].small_car_volume;
                   // totalvol += laneData[laneId].small_car_volume;
               }

           }
           if (invalidcnt == laneData.Length)
               ret.small_car_length = -1;
           else if( ret.small_car_volume==0)
               ret.small_car_length =0;
           else
               ret.small_car_length = ret.small_car_length / ret.small_car_volume;





           invalidcnt = 0;
           totalvol = 0;
           for (int laneId = 0; laneId < laneData.Length; laneId++)
           {

               if (laneData[laneId].big_car_volume == -1)
                   invalidcnt++;
               else
               {
                   ret.big_car_length += laneData[laneId].big_car_length * laneData[laneId].big_car_volume;
                  
               }

           }
           if (invalidcnt == laneData.Length)
               ret.big_car_length = -1;
           else if( ret.big_car_volume==0)
               ret.big_car_length=0;
           else
               ret.big_car_length = ret.big_car_length / ret.big_car_volume;



           invalidcnt = 0;
           totalvol = 0;
           for (int laneId = 0; laneId < laneData.Length; laneId++)
           {

               if (laneData[laneId].connect_car_volume == -1)
                   invalidcnt++;
               else
               {
                   ret.connect_car_length += laneData[laneId].connect_car_length * laneData[laneId].connect_car_volume;
                  
               }

           }

           if (invalidcnt == laneData.Length)
               ret.connect_car_length = -1;
           else if(ret.connect_car_volume==0)
                ret.connect_car_length=0;
           else
               ret.connect_car_length = ret.connect_car_length / ret.connect_car_volume;

         

           //計算平均 車束


           invalidcnt = 0;
           totalvol = 0;
           for (int laneId = 0; laneId < laneData.Length; laneId++)
           {

               if (laneData[laneId].small_car_volume == -1)
                   invalidcnt++;
               else
               {
                   ret.small_car_speed += laneData[laneId].small_car_speed * laneData[laneId].small_car_volume;
                   // totalvol += laneData[laneId].small_car_volume;
               }

           }
           if (invalidcnt == laneData.Length)
               ret.small_car_length = -1;
           else if(ret.small_car_volume==0)
               ret.small_car_speed=0;
           else
               ret.small_car_speed = ret.small_car_speed / ret.small_car_volume;

           //------------big car speed
           invalidcnt = 0;
           totalvol = 0;
           for (int laneId = 0; laneId < laneData.Length; laneId++)
           {

               if (laneData[laneId].big_car_volume == -1)
                   invalidcnt++;
               else
               {
                   ret.big_car_speed += laneData[laneId].big_car_speed * laneData[laneId].big_car_volume;
                   // totalvol += laneData[laneId].small_car_volume;
               }

           }
           if (invalidcnt == laneData.Length)
               ret.big_car_speed = -1;
           else  if(ret.big_car_volume==0)
               ret.big_car_speed=0;
           else
               ret.big_car_speed = ret.big_car_speed / ret.big_car_volume;



           invalidcnt = 0;
           totalvol = 0;
           for (int laneId = 0; laneId < laneData.Length; laneId++)
           {

               if (laneData[laneId].connect_car_volume == -1)
                   invalidcnt++;
               else
               {
                   ret.connect_car_speed += laneData[laneId].connect_car_speed * laneData[laneId].connect_car_volume;
                   // totalvol += laneData[laneId].small_car_volume;
               }

           }
           if (invalidcnt == laneData.Length)
               ret.big_car_speed = -1;
           else if (ret.connect_car_volume == 0)
               ret.connect_car_speed = 0;
           else
               ret.connect_car_speed = ret.connect_car_speed / ret.connect_car_volume;
           


           return ret;
       }

       public  void enable20SecEvent(bool enabled)
       {
           this.m_event_mode = (enabled) ?(byte) 1 : (byte)0;

       }

       //public void OneMinVD_DataTask()
       //{
       //}
    }
}
