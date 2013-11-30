using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;
using RemoteInterface.MFCC;
using Host.Schedule;
using System.Drawing;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(System.Convert.ToInt32(3.9));
           // string[]tests=null;
          //  test(ref tests);

          //RMS_test();
           // ClientTest.Service.Service serv = new ClientTest.Service.Service();
           // Console.WriteLine(serv.HelloWorld());
           // serv.printReport(18);
           // FetchTest();
            EtcTest();
            Console.ReadKey();
         //   RemoteInterface.HC.I_HC_Comm robjComm = (RemoteInterface.HC.I_HC_Comm)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_Comm), RemoteInterface.RemoteBuilder.getRemoteUri(RemoteInterface.RemoteBuilder.getHostIP(), (int)RemoteInterface.RemotingPortEnum.HOST, "Comm"));
           // Console.WriteLine(((RemoteInterface.RemoteClassBase)robj1).HelloWorld());
            //RemoteInterface.HC.I_HC_FWIS robjFIWS = (RemoteInterface.HC.I_HC_FWIS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_FWIS), RemoteInterface.RemoteBuilder.getRemoteUri("10.21.50.4", (int)RemoteInterface.RemotingPortEnum.HOST, "FWIS"));
            // RemoteInterface.Util.SetSysTime(robjComm.getDateTime());
            
            
        }

        static void EtcTest()
        {
            Host.ETC.ETC_Manager mgr = new Host.ETC.ETC_Manager();
        }
        static void FetchTest()
        {
            RemoteInterface.HC.I_HC_FWIS robj = (RemoteInterface.HC.I_HC_FWIS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_FWIS), RemoteInterface.RemoteBuilder.getRemoteUri("10.21.50.4", (int)RemoteInterface.RemotingPortEnum.HOST, "FWIS"));

           //object[] objs= robj.getOutputQueueData("CMS-N1-N-241.7");
           //FetchDeviceData[] data = robj.Fetch(new string[] { "CMS" }, "N1", "N", 243800, 10, 1,false);

           //foreach (FetchDeviceData d in data)
           //    Console.WriteLine(d.DevName+","+d.SegId+","+d.Mileage);

           //Console.WriteLine(objs.Length);
            FetchDeviceData[] data = robj.Fetch(new string[] { "RMS" }, "N6", "W", 18000, 1, 1, false);

           foreach (FetchDeviceData d in data)
               Console.WriteLine(d.DevName + "," + d.SegId+","+d.Mileage);

         //  Console.WriteLine(objs.Length);

        }
        
        static  void SchTest()
        {
            //for (int i = 2; i <= 9; i++)
            //    Host.Schedule.Scheduler.AddSchedule(new Host.Schedule.DailySchedule(i.ToString(), System.DateTime.Now.AddSeconds(i), 0, new ScheduleJob[] { new ScheduleJob("RGS-TEST-232", i, null) }, true));


            //for (int i = 10; i <= 20; i++)
            //    Host.Schedule.Scheduler.AddSchedule(new Host.Schedule.OneTimeSchedule(i.ToString(), System.DateTime.Now.AddSeconds(i), 1, new ScheduleJob[] { new ScheduleJob("RGS-TEST-232", i, null) }, true));

            //for (int i = 21; i <= 30; i++)
            //    Host.Schedule.Scheduler.AddSchedule(new Host.Schedule.DailySchedule(i.ToString(), System.DateTime.Now.AddSeconds(i), 1, new ScheduleJob[] { new ScheduleJob("RGS-TEST-232", i, null) }, true));


            //for (int i = 31; i <= 40; i++)
            //    Host.Schedule.Scheduler.AddSchedule(new Host.Schedule.OneTimeSchedule(i.ToString(), System.DateTime.Now.AddSeconds(i), 1, new ScheduleJob[] { new ScheduleJob("RGS-TEST-232", i, null) }, true));

            Host.Schedule.ScheduleManager.LoadAllSchedule();
            //Host.Schedule.OnSchedulerEvent += new Host.SchedulerEventDelegate(Scheduler_OnSchedulerEvent);
            Console.ReadKey();
          //  Console.WriteLine(Host.Schedule.Scheduler.Count());
        }
        static void RMS_test()
        {
            RemoteInterface.HC.I_HC_FWIS robjFIWS = (RemoteInterface.HC.I_HC_FWIS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_FWIS), RemoteInterface.RemoteBuilder.getRemoteUri("10.21.50.4", (int)RemoteInterface.RemotingPortEnum.HOST, "FWIS"));
            robjFIWS.RMS_setManualModeAndPlan("RMSTC-T78-25-W-1", 0, 29, true);

            
           // Console.WriteLine(((RemoteInterface.RemoteClassBase)robj1).HelloWorld());
        }

        static void Scheduler_OnSchedulerEvent(Host.Schedule.SchedulerEventType type, int schid)
        {

            //throw new Exception("The method or operation is not implemented.");
        }

           // robjFIWS.SetDbChangeNotify(DbChangeNotifyConst.TravelSettingData, "RGS-TEST-232");


            //int volume = 0;
            //int speed = 0;
            //int occupancy = 0;
            //int jameLevel = 0;
            //int travelSec = 0;

            //robjFIWS.GetAllTrafficData("N99", "N", 14000, 15000, ref  volume, ref  speed, ref  occupancy, ref   jameLevel, ref  travelSec);

          

           // 連Host
            //RemoteInterface.HC.I_HC_Comm robj_Host = (RemoteInterface.HC.I_HC_Comm)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_FWIS)
            //, RemoteInterface.RemoteBuilder.getRemoteUri("10.21.50.4", (int)RemoteInterface.RemotingPortEnum.HOST, "Comm"));

            
            //  RemoteInterface.Util.SetSysTime( robj_Host.getDateTime());
            
            //連Mfcc
            //I_MFCC_LCS robj_Mfcc = (I_MFCC_LCS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(I_MFCC_LCS)
            //        , RemoteBuilder.getRemoteUri("192.168.22.89", (int)RemotingPortEnum.MFCC_LCS, "MFCC_LCS"));            

            //System.Data.DataSet DS = robjComm.getSendDsByFuncName("LCS", "set_ctl_sign");                     

            //DS.Tables[0].Rows[0]["sign_cnt"] = Convert.ToByte(sign_cnt);

            //for (int i = 0; i <= sign_cnt-1; i++)
            //{
                
            //    DShow = DS.Tables[1].NewRow();
            //    DShow["sign_no"] = Convert.ToByte(i);
            //    DShow["sign_status"] = Convert.ToByte(0);
            //    DS.Tables[1].Rows.Add(DShow);
            //}
            
            //DS.Tables[1].Rows[0]["sign_status"] = Convert.ToByte(2);
            //DS.Tables[1].Rows[8]["sign_status"] = Convert.ToByte(2);
            //DS.Tables[1].Rows[16]["sign_status"] = Convert.ToByte(2);
            //DS.Tables[1].Rows[24]["sign_status"] = Convert.ToByte(2);         
            
            //DS.Tables[1].Rows[1]["sign_status"] = Convert.ToByte(8);
            //DS.Tables[1].Rows[9]["sign_status"] = Convert.ToByte(8);
            //DS.Tables[1].Rows[17]["sign_status"] = Convert.ToByte(8);
            //DS.Tables[1].Rows[25]["sign_status"] = Convert.ToByte(8);

            //DS.AcceptChanges();
            //robjFIWS.SetOutput("LCS-TEST-234", new LCSOutputData(DS),true);
            //robj2.WIS_setManualDisplay("WIS-TEST-233", 0, 0, 0, "abc", new byte[] { 0x30, 0x30, 0x30 }, true);

            //robj2.setManualModeOff("WIS-TEST-233");

            //RemoteInterface.MFCC.I_MFCC_WIS wisobj = (I_MFCC_WIS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(I_MFCC_WIS),
            //    RemoteInterface.RemoteBuilder.getRemoteUri("127.0.0.1", (int)RemoteInterface.RemotingPortEnum.MFCC_WIS, "MFCC_WIS"));
            // wisobj.SendDisplay("WIS-TEST-233", 0, 0, "測試中", new byte[] { 0x30, 0x30, 0x30 });






            //  RemoteInterface.HC.VD5MinMovingData[] data = robj2.GetAllVD5MinAvgData();
            //// Console.WriteLine(data.Length);
            //int prio = 0;
            //CMSOutputData obj = (CMSOutputData)robj2.GetCurrentOutput("CMS-TEST-234", ref prio);
            //System.Console.WriteLine(obj.ToString());
             
            //   VDJamEvalue eval = new VDJamEvalue();
            //   eval.getLevel("F", 100, 3);

               //System.Collections.ArrayList ary = new System.Collections.ArrayList();
               //ary.Add("d");
               //ary.Add("c");
               //ary.Add("b");
               //ary.Add("a");

               //ary.Sort();

               //object[] data = ary.ToArray();
               //foreach (object obj in data)
               //    Console.WriteLine(obj.ToString());
            //robjFIWS.setManualModeOff("LCS-TEST-234");
         //   System.Console.ReadKey();

        }


      
    }


