 using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;
using RemoteInterface;
using RemoteInterface.MFCC;
using Host.TC;
using Host.Schedule;
using Host.Event.Jam;



namespace Host
{
    class Program
    {

        public  static RemoteInterface.EventNotifyServer notifyServer;
        public   static Host.ScriptsManager ScriptMgr=new Host.ScriptsManager();
        public static Matrix matrix;
       // public static TravelModule travelModule;
        public static System.Timers.Timer tmrLineT78TravelTask = new System.Timers.Timer(1000 * 60);

        public static DateTime StartTime;

        static void Main(string[] args)
        {

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
 
            //CCTV.LockWindows lwnd = new Host.CCTV.LockWindows(3);
            //lwnd.setLock(1813, "ET_856122403", " ", 6);

            //CCTV.CCTV_Manager mgr = new CCTV.CCTV_Manager();
            //mgr.setETTULock("ET_858423803");
           // SendSMS("0988163835","新年快樂");

          //  lwnd.setLock(new Event.APID.APIDRangeData(new VDDeviceWrapper("","","","",100,"","",100,new byte[0],1,1,"",100,100)),  66, "desc1", "desc2", 1);

          //  initRemoteInterface();
            //WISOutputData d = new WISOutputData(0, 0, 0, 0, 0, "aa", new byte[] { 0x30, 0x30 }, new byte[] { 0 });
            //if (d.GetType() ==  typeof( CMSOutputData))
            //{
            //    Console.WriteLine("yes!");
            //}
            //else
            //{
            //    Console.WriteLine("No!");
            //}
            //Console.ReadKey();

            //ScheduleManager.AddSchedule(2139);
            //Console.ReadKey();

            StartTime = DateTime.Now;
            matrix = new Matrix();
            matrix.PostInitial();
         //   initRemoteInterface();

            //RemoteInterface.TEM.MonitorAlarmData d= new RemoteInterface.TEM.MonitorAlarmData("E602_TEM", DateTime.Now, "1", 1, 0, 1);
            //d.setStatus(1);
            //matrix.tem_mgr.setEventData("E602_TEM",d);
            //travelModule = new TravelModule();
            //travelModule.OnNewTravelData += new new_travel_time_handler(travelModule_OnNewTravelData);
            //tmrLineT78TravelTask.Elapsed += new System.Timers.ElapsedEventHandler(tmrLineT78TravelTask_Elapsed);
            //tmrLineT78TravelTask.Start();
           
          
             
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex=e.ExceptionObject as Exception;
            Util.SysLog("unhandleException.log", ex.Message + "," + ex.StackTrace);
        }


       
       

        static void tmrLineT78TravelTask_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
             //   LineT78AllUnitTravelCalc();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
             //   LineT78AllSectionTravelCalc();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
/*
        static void LineT78AllSectionTravelCalc()
        {
            Line line = matrix.getLine("T78");

            ConsoleServer.WriteLine("=========Line T78 Section W TravelTime ==========");

            foreach (Section sec in line.getAllSectionEnum("W"))
            {
                try
                {
                    ConsoleServer.WriteLine(sec.sectionName + ":" + sec.getTravelTime() + "s");
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                }
            }

                 ConsoleServer.WriteLine("=========Line T78 Section E TravelTime ==========");

            foreach (Section sec in line.getAllSectionEnum("E"))
            {
                try
                {
                    ConsoleServer.WriteLine(sec.sectionName + ":" + sec.getTravelTime() + "s");
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                }
            }
            

        }
 * 
 * */

        /*
        static void LineT78AllUnitTravelCalc()
        {
            int totalsec = 0;

            bool bisInvalid = false;
            // throw new Exception("The method or operation is not implemented.");
            Line line = matrix.getLine("T78");
           
            for (int i = line.startmileage / 1000; i <= (line.endmileage-1 )/ 1000; i++)
            {
                try
                {
                    int sec = 0;
                    sec = line["W", i].getTravelTime();
                    if (sec >= 0)
                    {
                        if (i == line.startmileage / 1000)
                            sec = (int)(sec * (((i + 1) * 1000.0 - line.startmileage) / 1000.0));
                        else if (i == line.endmileage / 1000)
                            sec = (int)(sec * (-(i * 1000.0 - line.endmileage) / 1000.0));
                    }
                    else
                    {
                        bisInvalid = true;
                        //continue;
                    }
                    totalsec += sec;
                    ConsoleServer.WriteLine(line["W", i].unitid + ":" + sec + "sec ");
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                }
            }

            ConsoleServer.WriteLine("==================================");
            ConsoleServer.WriteLine((bisInvalid) ? "total:-1 min" : "total:" + totalsec / 60 + "min");
            bisInvalid = false;
            totalsec = 0;
            //    Line line = matrix.getLine("T78");
            for (int i = line.startmileage / 1000; i <= (line.endmileage-1) / 1000; i++)
            {
                try
                {

                    int sec = 0;
                    sec = line["E", i].getTravelTime();
                    if (sec >= 0)
                    {
                        if (i == line.startmileage / 1000)
                            sec = (int)(sec * (((i + 1) * 1000.0 - line.startmileage) / 1000.0));
                        else if (i == line.endmileage / 1000)
                            sec = (int)(sec * (-(i * 1000.0 - line.endmileage) / 1000.0));
                    }
                    else
                        bisInvalid = true;
                    totalsec += sec;
                    ConsoleServer.WriteLine(line["E", i].unitid + ":" + sec + "sec ");
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                }
            }

            ConsoleServer.WriteLine("==================================");
            ConsoleServer.WriteLine((bisInvalid) ? "total:-1 min" : "total:" + totalsec / 60 + "min");

        }
         * */



        static void travelModule_OnNewTravelData(RGS_TravelDisplayData[] displayData)
        {
            foreach (RGS_TravelDisplayData data in displayData)
            {
                try
                {
                    TC.RGSDeviceWrapper rgs = (TC.RGSDeviceWrapper)matrix.getDeviceWrapper(data.devname);
                    if (data.icons != null)
                        rgs.SetTravelDisplay(data.icons, data.msgs, data.colors);
                    else
                        rgs.SetOutput(new OutputQueueData(rgs.deviceName,OutputModeEnum.TravelMode, OutputQueueData.TRAVEL_RULE_ID, OutputQueueData.TRAVEL_PRIORITY, null));
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                }
                
            }

        }


        public static void initRemoteInterface()
        {

            RemoteInterface.ConsoleServer.Start((int)RemoteInterface.ConsolePortEnum.Host);
            notifyServer = new RemoteInterface.EventNotifyServer((int)RemoteInterface.NotifyServerPortEnum.HOST);


            ServerFactory.SetChannelPort((int)RemoteInterface.RemotingPortEnum.HOST_FIWS);
            ServerFactory.RegisterRemoteObject(typeof(HC_FWIS_Robj), "FWIS");
            ServerFactory.SetChannelPort((int)RemoteInterface.RemotingPortEnum.HOST_TIMCC);
            ServerFactory.RegisterRemoteObject(typeof(HC_FWIS_Robj), "TIMCC");
            ServerFactory.SetChannelPort((int)RemoteInterface.RemotingPortEnum.HOST);
            ServerFactory.RegisterRemoteObject(typeof(HC_Comm_Robj), "Comm");
           
        }
    }

}
