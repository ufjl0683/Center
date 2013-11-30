using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using RemoteInterface;

namespace Host
{
   public  class Matrix
    {

       internal  MFCC.MFCC_Manager mfcc_mgr;
       public  TC.DevcieManager device_mgr;
        public RGS_PolygonSectionMapping rgs_polygon_section_mapping  = new RGS_PolygonSectionMapping();
       public VDJamEvalue vd_jam_eval = new VDJamEvalue();

       public  LineManager line_mgr;

       public  DbCmdServer dbServer = new DbCmdServer();

       public Host.WebReference.Service WebService = new Host.WebReference.Service();
       System.Timers.Timer tmr1min = new System.Timers.Timer(1000 * 120);
       public  Host.Event.Jam.JamManager jammgr;

        public AVI.AVIManager avimgr;

       public  Host.FiveMinTask FiveMinTask;
       public Host.OneMinTask OneMinTask;
       public TIMCC.TIMCC_SectionManager timcc_section_mgr;

       public Event.EventManager event_mgr ;

       public Event.Weather.WeatherManager weather_mgr;
       public Event.IID.IIDManager iid_mgr;
       public OutputDevicFetchManager output_device_fetch_mgr;

       public ETC.ETC_Manager etcmgr;
       public Event.MovingConstruct.MovingConstructManager moving_construct_mgr;

       public Event.TEM.TemManager tem_mgr;
       public Event.Redirect.RouteRedirectManager route_mgr;
       public Matrix()
       {

#if DEBUG
          // timcc_section_mgr = new Host.TIMCC.TIMCC_SectionManager("temp.xml");
           timcc_section_mgr = new Host.TIMCC.TIMCC_SectionManager("http://10.1.1.50/section_traffic_data.xml");
#else
           timcc_section_mgr = new Host.TIMCC.TIMCC_SectionManager("http://10.1.1.50/section_traffic_data.xml");
#endif

         
           mfcc_mgr = new MFCC.MFCC_Manager();
        
         //  rgs_polygon_section_mapping = new RGS_PolygonSectionMapping();
           tmr1min.Elapsed += new System.Timers.ElapsedEventHandler(tmr1min_Elapsed);
           Schedule.ScheduleManager.LoadAllSchedule();


       }


      

      public  void PostInitial()
       {
           //I_MFCC_VD robj =(I_MFCC_VD) RemoteBuilder.GetRemoteObj(typeof(I_MFCC_VD),
           //    RemoteBuilder.getRemoteUri("192.168.22.89",(int) RemotingPortEnum.MFCC_VD1, "MFCC_VD"));
          
           //robj.setRealTime("VD231", 1, 0, 1);
           ConsoleServer.WriteLine("設備管理啟動中......!");
           device_mgr = new Host.TC.DevcieManager(mfcc_mgr);
           ConsoleServer.WriteLine("設備管理啟動完成!");
           line_mgr = new LineManager();
           output_device_fetch_mgr = new OutputDevicFetchManager(this.device_mgr);
          
           FiveMinTask = new FiveMinTask();

           OneMinTask = new OneMinTask();

           avimgr = new Host.AVI.AVIManager();
           etcmgr = new Host.ETC.ETC_Manager();

           tmr1min.Start();
           System.Threading.Thread.Sleep(10000);
          
           getinitVd5minData();

           event_mgr = new Host.Event.EventManager();

           moving_construct_mgr = new Host.Event.MovingConstruct.MovingConstructManager();
           jammgr = new Host.Event.Jam.JamManager(device_mgr);
           tem_mgr = new Host.Event.TEM.TemManager();
           route_mgr = new Host.Event.Redirect.RouteRedirectManager();

#if DEBUG
           //    RemoteInterface.HC.FetchDeviceData[] d = output_device_fetch_mgr.Fetch(new string[] { "CMS" }, "N1",150000, 174200);
       //    jammgr.DoVD_InteropData("VD-N6-E-17.6", System.DateTime.Now.AddSeconds(-System.DateTime.Now.Second));
           //  jammgr.DoVD_InteropData( "VD-N6-E-18.0", System.DateTime.Now.AddSeconds(-System.DateTime.Now.Second));
      
#endif




           weather_mgr = new Host.Event.Weather.WeatherManager();
           iid_mgr = new Host.Event.IID.IIDManager();
           
       }

       public int getRGSNetworkModeJamStatus(int g_code_id, int sectionid)
       {
           int ret;
           ret= this.rgs_polygon_section_mapping.getJamLevel(g_code_id, sectionid);
           if (ret > 3 && ret < 255)
               return 3;
           else if (ret == 1 || ret == 2)
               return 2;
           else if (ret == 0)
               return 1;
           else
               return ret;
       }

       public TC.DevcieManager getDevicemanager()
       {
           if (device_mgr == null)
               throw new Exception("裝置管理啟動中!請稍後連線!");
           return device_mgr;
       }
       internal void getinitVd5minData()
       {
           System.Collections.IEnumerable ie = this.device_mgr.getDataDeviceEnum();
           foreach (Host.TC.DeviceBaseWrapper device in ie)
           {
               try
               {
                   if (device is Host.TC.VDDeviceWrapper)
                   {

                       I_MFCC_VD robj = (I_MFCC_VD)((Host.TC.VDDeviceWrapper)device).getRemoteObj();//.getVDLatest5MinAvgData(device.deviceName);
                       if (robj.getConnectionStatus(device.deviceName))
                       {
                           ((Host.TC.VDDeviceWrapper)device).Set5MinAvgData(robj.getVDLatest5MinAvgData(device.deviceName));

                       }
                   }
               }
               catch (Exception ex)
               {
                   ConsoleServer.WriteLine(device.deviceName + "," + ex.Message + ex.StackTrace);
               }
           }
       }

       volatile bool bInTmr1min = false;

       void tmr1min_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
       {
           
           if (bInTmr1min) return;
           bInTmr1min = true;
           try
           {
              // printVd5minData();
               OneMinOutputDisplayTask();
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
           }
           bInTmr1min = false;
       }


       void printVd5minData()
       {

           foreach (Host.TC.DeviceBaseWrapper dev in device_mgr.getDataDeviceEnum())
           {
               try
               {
                   if (dev.deviceType == "VD")
                       RemoteInterface.ConsoleServer.WriteLine(((TC.VDDeviceWrapper)dev).ToString() +"\t" +((TC.VDDeviceWrapper)dev).jamLevel);
               }
               catch (Exception ex)
               {
                   ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
               }
           }
       }

      public I_MFCC_Base getRemoteObject(string deviceName)
      {
        return   device_mgr.getRemoteObject(deviceName);
      }

       public TC.DeviceBaseWrapper getDeviceWrapper(string devicename)
       {
           try
           {
               if (device_mgr != null)
                   return device_mgr[devicename];
               else
                   return null;
           }
           catch (Exception ex)
           {
               throw new Exception(devicename + "," + ex.Message);
           }
       }

       volatile  bool InOneMinOutputDisplayTask = false;
       void OneMinOutputDisplayTask()
       {
          

          
           System.Collections.IEnumerator ie = device_mgr.getOutputDeviceEnum().GetEnumerator();
         //  System.Threading.Thread[] th = new System.Threading.Thread[200];
           int thcnt = 0;
         
           if (InOneMinOutputDisplayTask)
               return;
           InOneMinOutputDisplayTask = true;
           while (ie.MoveNext())
           {
               try
               {

                   System.Threading.ThreadPool.SetMaxThreads(100,100);
                   System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(OutputDeviceTask),ie.Current);

                   /* 
                   if (!((TC.OutPutDeviceBase)ie.Current).IsConnected)
                       continue;

                   th[thcnt % th.Length] = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(OutputDeviceTask));

                   th[thcnt % th.Length].Start(ie.Current);
                 thcnt++;
                 if (thcnt % th.Length == 0)
                 {
                     ConsoleServer.WriteLine("=======Display Task turn" + thcnt / th.Length+ "begin!==============");
                     for (int i = 0; i < th.Length; i++)
                         th[i].Join();

                     ConsoleServer.WriteLine("=============Display Task turn" + thcnt / th.Length + "end!=============");

                 }
                    * 
                    * */

               }
               catch (Exception ex)
               {
                   ConsoleServer.WriteLine("OneMinOutputDisplayTask:" + ((TC.OutPutDeviceBase)ie.Current).deviceName + "," + ex.Message + ex.StackTrace);
               }
           }

           InOneMinOutputDisplayTask = false;
       }

       private void OutputDeviceTask(object objDevice)
       {
           try
           {
               TC.OutPutDeviceBase outdevice = ((TC.OutPutDeviceBase)objDevice);
               if (outdevice.getRemoteObj() == null)
                   return;
             //  lock( outdevice.getRemoteObj())
                    outdevice.output();

           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine("OneMinOutputDisplayTask:" + ((TC.OutPutDeviceBase)objDevice).deviceName + "," + ex.Message + ex.StackTrace);
           }
       }

       public int getUnitVDTravelTime(string lineid, string direction, int milekm)
       {
           return line_mgr[lineid][direction, milekm].getTravelTime();
       }

       public int getVDSectionTravelTime(string lineid, string dir, string secid)
       {
          Section sec=(Section) line_mgr[lineid].getSection(dir, secid);
          return sec.getVD_TravelTime();
       }
       //public int getVDTravelTime(string lineid,string dir, int startmile, int endmile)
       //{
       //    Line line =(Line) line_mgr[lineid];
       //    int totalSec=0;
       //    bool IsInvalid = false ;
       //    int tmp;
       //    if (startmile > endmile)  //swap  so startmile always < endmile
       //    {
       //        tmp = startmile;
       //        startmile = endmile;
       //        endmile = tmp;
       //    }
       //    for (int i = startmile / 1000; i <= endmile / 1000; i++)
       //    {
       //        int unitsec;
       //        UnitRoad unit = line[dir,i];
       //        unitsec=unit.getVD_TravelTime();
       //        if (i == startmile / 1000)
                         
       //                unitsec = (int)(unitsec * ((i + 1) * 1000.0 - startmile) / 1000.0);

       //        else if(i==endmile/1000)

       //                unitsec = (int)(unitsec * (endmile - i * 1000) / 1000.0);

       //            if (unitsec < 0) IsInvalid = true;

       //            totalSec += unitsec;
                 
              
               
       //    }

       //    return (IsInvalid)?-1:totalSec;

       //}

       public Line getLine(string lineid)
       {
           
           return (Line)line_mgr[lineid];
       }

    }
}
