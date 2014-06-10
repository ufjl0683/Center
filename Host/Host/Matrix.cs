using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using RemoteInterface;
using Host.TC;
using Host.Event.Jam;
using Host.Schedule;

namespace Host
{
   public  class Matrix
    {

       internal  MFCC.MFCC_Manager mfcc_mgr;
       public  TC.DevcieManager device_mgr;
       public RGS_PolygonSectionMapping rgs_polygon_section_mapping;
       public VDJamEvalue vd_jam_eval = new VDJamEvalue();

       public  LineManager line_mgr;

       public DbCmdServer dbServer = new DbCmdServer() { MAX_QUEUE_CNT = 100000 };

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
       public Event.Redirect74.RouteRedirectManagerT74 route_mgr74;
       public Event.APID.APIDManager apid_mgr;
       public CCTV.CCTV_Manager cctvmgr;

       public Event.LTR.LTR_Manager ltr_mgr;
       public Event.RampControl.RampControl_Manager rampctl_mgr;
       public Event.ServiceArea.ServiceAreaManager svcarea_mgr;
       public Event.MetroNetwork.MetroNetworkManager metro_network_mgr;
       public Event.CSLSControl.CSLSControlEventManager csls_mgr;
       public XmlWeatherManager xml_weather_mgr;
       public FiveMinVDAVGDataManager vd5minavg_mgr;
       public Matrix()
       {


        //   new Event.MovingConstruct.MovingConstructRange(1, "1", DateTime.Now, "N1", "N", 100, 1000, 0, "AA", "AA");
#if DEBUG
          // timcc_section_mgr = new Host.TIMCC.TIMCC_SectionManager("temp.xml");
           timcc_section_mgr = new Host.TIMCC.TIMCC_SectionManager("http://10.1.1.50/section_traffic_data.xml");
           xml_weather_mgr = new XmlWeatherManager();
       
#else
           timcc_section_mgr = new Host.TIMCC.TIMCC_SectionManager("http://10.21.50.100/N_section_traffic_data.xml");
            xml_weather_mgr = new XmlWeatherManager();
#endif


           mfcc_mgr = new MFCC.MFCC_Manager();
        
         //  rgs_polygon_section_mapping = new RGS_PolygonSectionMapping();
           tmr1min.Elapsed += new System.Timers.ElapsedEventHandler(tmr1min_Elapsed);
#if !DEBUG
           Schedule.ScheduleManager.LoadAllSchedule();
#endif

       }


      

      public  void PostInitial()
       {
           //I_MFCC_VD robj =(I_MFCC_VD) RemoteBuilder.GetRemoteObj(typeof(I_MFCC_VD),
           //    RemoteBuilder.getRemoteUri("192.168.22.89",(int) RemotingPortEnum.MFCC_VD1, "MFCC_VD"));

           //robj.setRealTime("VD231", 1, 0, 1);
           ConsoleServer.WriteLine("設備管理啟動中......!");
           device_mgr = new Host.TC.DevcieManager(mfcc_mgr);
           ConsoleServer.WriteLine("設備管理啟動完成!");
          

           vd5minavg_mgr = new FiveMinVDAVGDataManager();
#if DEBUG
           //new HC_FWIS_Robj().RGS_setManualGenericDisplay("RGS-N6-W-0.4",
           // new RGS_GenericDisplay_Data(2, 0, new RGS_Generic_ICON_Data[0],
           //     new RGS_Generic_Message_Data[]{new  RGS_Generic_Message_Data("系統測試",
           //       new System.Drawing.Color[]{System.Drawing.Color.Red,System.Drawing.Color.Red,System.Drawing.Color.Red,System.Drawing.Color.Red},new System.Drawing.Color[]{System.Drawing.Color.Black,System.Drawing.Color.Black,System.Drawing.Color.Black,System.Drawing.Color.Black},0,0)}
           //     , new RGS_Generic_Section_Data[0]),false);
           //(this.device_mgr["RGS-N6-W-0.4"].getRemoteObj() as I_MFCC_RGS).setGenericDisplay("RGS-N6-W-0.4",
           // new RGS_GenericDisplay_Data(2, 0, new RGS_Generic_ICON_Data[0],
           //     new RGS_Generic_Message_Data[]{new  RGS_Generic_Message_Data("系統測試",
           //       new System.Drawing.Color[]{System.Drawing.Color.Red,System.Drawing.Color.Red,System.Drawing.Color.Red,System.Drawing.Color.Red},new System.Drawing.Color[]{System.Drawing.Color.Black,System.Drawing.Color.Black,System.Drawing.Color.Black,System.Drawing.Color.Black},0,0)}
           //     , new RGS_Generic_Section_Data[0]));
           //    RemoteInterface.HC.FetchDeviceData[] d = output_device_fetch_mgr.Fetch(new string[] { "CMS" }, "N1",150000, 174200);
           //    jammgr.DoVD_InteropData("VD-N6-E-17.6", System.DateTime.Now.AddSeconds(-System.DateTime.Now.Second));
           //  jammgr.DoVD_InteropData( "VD-N6-E-18.0", System.DateTime.Now.AddSeconds(-System.DateTime.Now.Second));
#endif
           this.rgs_polygon_section_mapping = new RGS_PolygonSectionMapping();
           line_mgr = new LineManager();



           output_device_fetch_mgr = new OutputDevicFetchManager(this.device_mgr);


           //    output_device_fetch_mgr.Fetch(new string[] { "RMS" }, "N6" , "W", 35000, 1, 0, false);

          
#if !DEBUG
           FiveMinTask = new FiveMinTask();

       //    OneMinTask = new OneMinTask();
          // avimgr = new Host.AVI.AVIManager();
         //  etcmgr = new Host.ETC.ETC_Manager();  //2014-5-14 停用
#endif
           etcmgr = new Host.ETC.ETC_Manager();
        OneMinTask = new OneMinTask();
           avimgr = new Host.AVI.AVIManager();
           tmr1min.Start();
           System.Threading.Thread.Sleep(10000);

        
#if DEBUG
           //{
           //    string ret = "";
           //    try
           //    {
           //        ret = "FiveMinQueueCnt:" + Program.matrix.vd5minavg_mgr.VDFiveMinQueueCnt;
           //        foreach (DeviceBaseWrapper dev in Program.matrix.device_mgr.getDataDeviceEnum())
           //        {
           //            if (dev is VDDeviceWrapper)
           //            {
           //                VDDeviceWrapper vddev = dev as VDDeviceWrapper;
           //                ret += vddev.ToString() + "\r\n";

           //            }
           //        }

           //     Console.WriteLine(ret);
           //    }
           //    catch (Exception ex)
           //    {
           //        throw new RemoteException(ex.Message + "," + ex.StackTrace);
           //    }
           //}
           //int vol=0,spd=0,occ=0,level=0,ttime=0,lttime=0,httime=0;
           //this.line_mgr["N3"].getAllTrafficData("N", 224700, 231400, ref vol, ref spd, ref occ, ref level, ref ttime, ref lttime, ref httime);
#endif
           event_mgr = new Host.Event.EventManager();

       
#if DEBUG

#else  
           cctvmgr = new CCTV.CCTV_Manager();

           moving_construct_mgr = new Host.Event.MovingConstruct.MovingConstructManager();
          // moving_construct_mgr.setEvent(1,"test",DateTime.Now,"N1","S",15000,16000,16,"0001","TEST","Y" );
           //moving_construct_mgr.setEvent(1, "test", DateTime.Now, "N1", "S", 16000, 17000, 16, "0001", "TEST", "Y");
         //  moving_construct_mgr.CloseMovingConstructEvent(1);
#endif
           jammgr = new Host.Event.Jam.JamManager(device_mgr);
#if DEBUG
           //Program.initRemoteInterface();
           //Console.ReadKey();
           route_mgr74 = new Host.Event.Redirect74.RouteRedirectManagerT74();
           route_mgr = new Host.Event.Redirect.RouteRedirectManager();
           metro_network_mgr = new Event.MetroNetwork.MetroNetworkManager();
#endif
#if !DEBUG
           tem_mgr = new Host.Event.TEM.TemManager();
         
           rampctl_mgr = new Event.RampControl.RampControl_Manager();
           metro_network_mgr = new Event.MetroNetwork.MetroNetworkManager();
           route_mgr74 = new Host.Event.Redirect74.RouteRedirectManagerT74();
           route_mgr = new Host.Event.Redirect.RouteRedirectManager();
           ltr_mgr = new Event.LTR.LTR_Manager();

           weather_mgr = new Host.Event.Weather.WeatherManager();

           iid_mgr = new Host.Event.IID.IIDManager();
           svcarea_mgr = new Event.ServiceArea.ServiceAreaManager();
           csls_mgr = new Event.CSLSControl.CSLSControlEventManager(device_mgr);
#endif
           Program.initRemoteInterface();
           try
           {
               WebService.SendSMS("0988163835", "host is online!");
           }
           catch(Exception ex) {

               Console.WriteLine(ex.Message + "," + ex.StackTrace);
               ;}
#if !DEBUG
           try
           {
               WebService.SendSMS("0932500190", "host is online!");
           }
           catch { ;}
           try
           {
               WebService.SendSMS("0919712057", "host is online!");
           }
           catch { ;}
#endif




#if DEBUG
           //RemoteInterface.HC.I_HC_Comm rrobj = (RemoteInterface.HC.I_HC_Comm)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_Comm),
           //    RemoteInterface.RemoteBuilder.getRemoteUri("10.21.50.224", (int)RemoteInterface.RemotingPortEnum.HOST_FIWS, "Comm"));
           //int pr = 0;
           // rrobj.GetCurrentOutput("CMS-N3-N-123.6", ref pr);
           //(this.device_mgr["RGS-N6-W-0.4"].getRemoteObj() as I_MFCC_RGS).setGenericDisplay("RGS-N6-W-0.4",
           // new RGS_GenericDisplay_Data(2, 0, new RGS_Generic_ICON_Data[0],
           //     new RGS_Generic_Message_Data[]{new  RGS_Generic_Message_Data("系統測試",
           //       new System.Drawing.Color[]{System.Drawing.Color.Red,System.Drawing.Color.Red,System.Drawing.Color.Red,System.Drawing.Color.Red},new System.Drawing.Color[]{System.Drawing.Color.Black,System.Drawing.Color.Black,System.Drawing.Color.Black,System.Drawing.Color.Black},0,0)}
           //     , new RGS_Generic_Section_Data[0]));
           //    RemoteInterface.HC.FetchDeviceData[] d = output_device_fetch_mgr.Fetch(new string[] { "CMS" }, "N1",150000, 174200);
           //    jammgr.DoVD_InteropData("VD-N6-E-17.6", System.DateTime.Now.AddSeconds(-System.DateTime.Now.Second));
           //  jammgr.DoVD_InteropData( "VD-N6-E-18.0", System.DateTime.Now.AddSeconds(-System.DateTime.Now.Second));
#else
       apid_mgr = new Event.APID.APIDManager();
#endif






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
                         //  ((Host.TC.VDDeviceWrapper)device).Set5MinAvgData(robj.getVDLatest5MinAvgData(device.deviceName));
                           vd5minavg_mgr.Set5MinAvgData(robj.getVDLatest5MinAvgData(device.deviceName));
                       }
                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine(device.deviceName + "," + ex.Message + ex.StackTrace);
               }
           }
       }

       volatile bool bInTmr1min = false;
       int TimeCnt=0;
       void tmr1min_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
       {
           
           if (bInTmr1min) return;
           bInTmr1min = true;
           try
           {
              // printVd5minData();
               if(TimeCnt==2)
                     OneMinOutputDisplayTask();
               TimeCnt = (TimeCnt + 1) % 3;
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
           }

           try
           {
               if (HC_FWIS_Robj.dictPerformance["GetDeviceStatus"].CallCount < 200 && HC_Comm_Robj.dictPerformance["setVDFiveMinData"].CallCount < 200 && DateTime.Now.Subtract(Program.StartTime).TotalMinutes > 20)
               {

                   try
                   {
                       Util.SysLog( "restore.txt",
                           DateTime.Now.ToString() + "," + HC_FWIS_Robj.dictPerformance["GetDeviceStatus"].CallCount + "," + HC_Comm_Robj.dictPerformance["setVDFiveMinData"].CallCount); 
                   }
                   catch
                   { ;}
                   try
                   {
#if !DEBUG
                       WebService.SendSMS("0988163835", "host self-restart!," + HC_FWIS_Robj.dictPerformance["GetDeviceStatus"].CallCount + "," + HC_Comm_Robj.dictPerformance["setVDFiveMinData"].CallCount);
#endif               
                       }
                   catch { ;}
                   try
                   {
#if !DEBUG
                       WebService.SendSMS("0932500190", "host self-restart!");
#endif
                   }
                   catch { ;}
#if !DEBUG 
                   Environment.Exit(0);
#endif
                   
               }
           }
           catch { ;}

           try
           {
               WriteFIWSPerformance();
           }
           catch { ;}
           try
           {
               WriteCommPerformance();

           
           }
           catch { ;}

           try
           {
               WriteAVIDeviceQueueCnt();
           }
           catch
           {
               ;
           }

           try
           {
               WriteAVISectionQueueCnt();
           }
           catch
           {
               ;
           }

           try
           {
               WriteEventMrgQueue();
           }
           catch { ;}


           GC.Collect();
           bInTmr1min = false;
       }

       int LastDBQueueCnt = -1;
       bool IsDBQueueOverNotify = false;
       void WriteEventMrgQueue()
       {
           try
           {
               bool isnew = false;
               string filename = string.Format("mem{0:00}{1:00}{2:00}.txt", DateTime.Now.Year % 100, DateTime.Now.Month, DateTime.Now.Day);
               System.IO.StreamWriter wd;
               if (!System.IO.File.Exists(Util.CPath(AppDomain.CurrentDomain.BaseDirectory) + filename))
               {
                   isnew = true;
                   wd = new System.IO.StreamWriter(System.IO.File.Create(Util.CPath(AppDomain.CurrentDomain.BaseDirectory) + filename));
               }
               else
                   wd = System.IO.File.AppendText(Util.CPath(AppDomain.CurrentDomain.BaseDirectory) + filename);

               string str;
               if (isnew)
               {
                   str="DateTime,Event,CSLS,IID,JAM,LTR,METRO,MOVING,RampControl,TEM,Weather,aid,threadcnt,dbqueue";
                   wd.WriteLine(str);

               }
               int workkercnt, completcnt;
               System.Threading.ThreadPool.GetAvailableThreads(out workkercnt, out completcnt);
               str = DateTime.Now.ToString() + ","+event_mgr.GetEventCnt()+","+ csls_mgr.GetEventCnt()+","+iid_mgr.GetEventCnt()+","+
                   jammgr.GetEventCnt()+","+ltr_mgr.GetEventCnt()+","+metro_network_mgr.GetEventCnt()+","+moving_construct_mgr.GetEventCnt()+","+
                   rampctl_mgr.GetEventCnt()+","+tem_mgr.GetEventCnt()+","+weather_mgr.GetEventCnt()+","+apid_mgr.GetEventCnt()+","+workkercnt+","+dbServer.getCurrentQueueCnt();
               wd.WriteLine(str);
               wd.Close();

               int currentdbcnt = dbServer.getCurrentQueueCnt();
               if (!IsDBQueueOverNotify && LastDBQueueCnt < currentdbcnt && LastDBQueueCnt != -1 && currentdbcnt > 60000)
               {
                   IsDBQueueOverNotify = true;
                   WebService.SendSMS("0932500190", "host Queue cnt > 60000!");
                   WebService.SendSMS("0988163835", "host Queue cnt > 60000!");
               }
               if (currentdbcnt < 60000)
                   IsDBQueueOverNotify = false;

               LastDBQueueCnt = dbServer.getCurrentQueueCnt();
           }
           catch
           {
               ;
           }


       }


       void WriteAVISectionQueueCnt()
       {

           try
           {

               bool isnew = false;
               string filename = string.Format("avisections{0:00}{1:00}{2:00}.txt", DateTime.Now.Year % 100, DateTime.Now.Month, DateTime.Now.Day);
               System.IO.StreamWriter wd;
               if (!System.IO.File.Exists(Util.CPath(AppDomain.CurrentDomain.BaseDirectory) + filename))
               {
                   isnew = true;
                   wd = new System.IO.StreamWriter(System.IO.File.Create(Util.CPath(AppDomain.CurrentDomain.BaseDirectory) + filename));
               }
               else
                   wd = System.IO.File.AppendText(Util.CPath(AppDomain.CurrentDomain.BaseDirectory) + filename);

               string str;

               if (isnew)
               {
                   str = "DateTime,";


                   foreach (AVI.AVISection sec in avimgr.sections)
                   {
                      
                           str += sec.secid+ ",";
                   }



                   wd.WriteLine(str.TrimEnd(",".ToCharArray()));


               }

               str = DateTime.Now + ",";
               foreach (AVI.AVISection sec in avimgr.sections)
               {
                   
                       str += (sec as AVI.AVISection).dataContainer.Count + ",";

               }
               wd.WriteLine(str.TrimEnd(",".ToCharArray()));
               wd.Close();
           }
           catch
           {
               ;
           }

       }

       void WriteAVIDeviceQueueCnt()
       {

           try
           {

               bool isnew = false;
               string filename = string.Format("avi{0:00}{1:00}{2:00}.txt", DateTime.Now.Year % 100, DateTime.Now.Month, DateTime.Now.Day);
               System.IO.StreamWriter wd;
               if (!System.IO.File.Exists(Util.CPath(AppDomain.CurrentDomain.BaseDirectory) + filename))
               {
                   isnew = true;
                   wd = new System.IO.StreamWriter(System.IO.File.Create(Util.CPath(AppDomain.CurrentDomain.BaseDirectory) + filename));
               }
               else
                   wd = System.IO.File.AppendText(Util.CPath(AppDomain.CurrentDomain.BaseDirectory) + filename);

               string str;

               if (isnew)
               {
                   str = "DateTime,";


                   foreach (DeviceBaseWrapper tc in device_mgr.getAllDeviceEnum())
                   {
                       if (tc is AVIDeviceWrapper)
                           str += tc.deviceName + ",";
                   }



                   wd.WriteLine(str.TrimEnd(",".ToCharArray()));


               }

               str = DateTime.Now + ",";
               foreach (DeviceBaseWrapper tc in device_mgr.getAllDeviceEnum())
               {
                   if (tc is AVIDeviceWrapper)
                       str += (tc as AVIDeviceWrapper).CurrDataCnt + ",";

               }
               wd.WriteLine(str.TrimEnd(",".ToCharArray()));
               wd.Close();
           }
           catch
           {
               ;
           }

       }
       void WriteCommPerformance()
       {
           try
           {
               bool isnew = false;
               string filename = string.Format("comm{0:00}{1:00}{2:00}.txt", DateTime.Now.Year % 100, DateTime.Now.Month, DateTime.Now.Day);
               System.IO.StreamWriter wd;
               if (!System.IO.File.Exists(Util.CPath(AppDomain.CurrentDomain.BaseDirectory) + filename))
               {
                   isnew = true;
                   wd = new System.IO.StreamWriter(System.IO.File.Create(Util.CPath(AppDomain.CurrentDomain.BaseDirectory) + filename));
               }
               else
                   wd = System.IO.File.AppendText(Util.CPath(AppDomain.CurrentDomain.BaseDirectory) + filename);

               string str;
               string[] keys = new string[HC_Comm_Robj.dictPerformance.Keys.Count];
               HC_Comm_Robj.dictPerformance.Keys.CopyTo(keys, 0);
               if (isnew)
               {
                   str = "DateTime,";


                   foreach (string key in keys)
                       str += key + ","+"balance,";

                   wd.WriteLine(str.TrimEnd(",".ToCharArray()));
               }

               str = DateTime.Now.ToString() + ",";
               foreach (string key in keys)
               {

                   str += HC_Comm_Robj.dictPerformance[key].CallCount.ToString() + ","+ HC_Comm_Robj.dictPerformance[key].InCount+",";
                   HC_Comm_Robj.dictPerformance[key].CallCount = 0;
                  
               }

               wd.WriteLine(str.TrimEnd(",".ToCharArray()));
               wd.Close();
           }
           catch
           { ;}

           //}

       }

       void WriteFIWSPerformance()
       {
           try
           {
               bool isnew = false;
               string filename = string.Format("fiws{0:00}{1:00}{2:00}.txt", DateTime.Now.Year % 100, DateTime.Now.Month, DateTime.Now.Day);
               System.IO.StreamWriter wd;
               if (!System.IO.File.Exists(Util.CPath(AppDomain.CurrentDomain.BaseDirectory) + filename))
             
               {
                   isnew = true;
                   wd = new System.IO.StreamWriter(System.IO.File.Create(Util.CPath(AppDomain.CurrentDomain.BaseDirectory) + filename));
               }
               else
                   wd = System.IO.File.AppendText(Util.CPath(AppDomain.CurrentDomain.BaseDirectory) + filename);

               string str;
               string[] keys = new string[HC_FWIS_Robj.dictPerformance.Keys.Count];
               HC_FWIS_Robj.dictPerformance.Keys.CopyTo(keys, 0);
               if (isnew)
               {
                    str="DateTime,";
                   
                  
                       foreach (string key in keys)
                           str += key + ","+"balance,";
                
                   wd.WriteLine(str.TrimEnd(",".ToCharArray()));
               }

               str = DateTime.Now.ToString()+",";
               foreach (string key in keys)
               {
                  
                   str += HC_FWIS_Robj.dictPerformance[key].CallCount.ToString() + ","+HC_FWIS_Robj.dictPerformance[key].InCount+",";
                   HC_FWIS_Robj.dictPerformance[key].CallCount = 0;
               }

               wd.WriteLine(str.TrimEnd(",".ToCharArray()));
               wd.Close();
           }
           catch
           { ;}

           //}

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

#if DEBUG

       void ThreadTest()
       {
           System.Threading.Thread.Sleep(5000);
       }
#endif
       volatile  bool InOneMinOutputDisplayTask = false;
       void OneMinOutputDisplayTask()
       {
          

          
           System.Collections.IEnumerator ie = device_mgr.getOutputDeviceEnum().GetEnumerator();
         //  System.Threading.Thread[] th = new System.Threading.Thread[200];
           int thcnt = 0;

           if (InOneMinOutputDisplayTask)
           {
              
               return;
           }
           InOneMinOutputDisplayTask = true;
           int workkercnt, completcnt;
           //for (int i = 0; i < 100; i++)
           //    new System.Threading.Thread(ThreadTest).Start();
           System.Threading.ThreadPool.GetAvailableThreads(out workkercnt, out completcnt);
           if (workkercnt == 0)
           {
               InOneMinOutputDisplayTask = false;
               return;
           }
           while (ie.MoveNext())
           {
               try
               {
                  
                  // System.Threading.ThreadPool.SetMaxThreads(50,50);

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
