using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HC;


namespace timccexchg
{
    class Program
    {

      //  static System.Timers.Timer tmr30Sec = new System.Timers.Timer(30 * 1000);
        static System.Threading.Timer tmr1min;
        static System.Threading.Timer tmr5min;
       // static System.Timers.Timer tmr2min = new System.Timers.Timer(1000 * 60 * 2);
        static System.Timers.Timer tmr30min = new System.Timers.Timer(1000 * 60 * 30);
        static System.Timers.Timer tmr1hour = new System.Timers.Timer(1000 * 60 * 60);

        static RemoteInterface.HC.I_HC_Comm rhostobj;
        static void Main(string[] args)
        {
            rhostobj = (I_HC_Comm)RemoteBuilder.GetRemoteObj(typeof(I_HC_Comm),
                RemoteBuilder.getRemoteUri(RemoteBuilder.getHostIP(), (int)RemotingPortEnum.HOST, "Comm"));


        

       
            if (!System.IO.Directory.Exists(timccexchg.Settings1.Default.OneMinVDCurrPath))
                System.IO.Directory.CreateDirectory(timccexchg.Settings1.Default.OneMinVDCurrPath);

            if (!System.IO.Directory.Exists(timccexchg.Settings1.Default.FiveMinVDCurrPath))
                System.IO.Directory.CreateDirectory(timccexchg.Settings1.Default.FiveMinVDCurrPath);

            if (!System.IO.Directory.Exists(timccexchg.Settings1.Default.OneMinVDHistPath))
                System.IO.Directory.CreateDirectory(timccexchg.Settings1.Default.OneMinVDHistPath);

            if (!System.IO.Directory.Exists(timccexchg.Settings1.Default.FiveMinVDHistPath))
                System.IO.Directory.CreateDirectory(timccexchg.Settings1.Default.FiveMinVDHistPath);

            if (!System.IO.Directory.Exists(timccexchg.Settings1.Default.EpConfCurrPath))
                System.IO.Directory.CreateDirectory(timccexchg.Settings1.Default.EpConfCurrPath);

            if (!System.IO.Directory.Exists(timccexchg.Settings1.Default.CommCurrPath))
                System.IO.Directory.CreateDirectory(timccexchg.Settings1.Default.CommCurrPath);


            init_tmr();
            setDateTimeFromHost();
            tmr1hour_Elapsed(null, null);

          
           
            while(true)
            System.Threading.Thread.Sleep(1000 * 60);
        }


        static void init_tmr()
        {
           // tmr2min.Elapsed += new System.Timers.ElapsedEventHandler(tmr2min_Elapsed);
           // tmr30Sec.Elapsed += new System.Timers.ElapsedEventHandler(tmr30Sec_Elapsed);
            DateTime nextTime = DateTime.Now.AddMinutes(1).AddSeconds(-DateTime.Now.Second+30);
            tmr1min = new System.Threading.Timer(tmr60Sec_Elapsed);
            tmr1min.Change((int)((TimeSpan)(nextTime - DateTime.Now)).TotalMilliseconds, System.Threading.Timeout.Infinite);
            tmr5min = new System.Threading.Timer(tmr5min_Elapsed);
            nextTime = DateTime.Now.AddMinutes(-DateTime.Now.Minute % 5).AddSeconds(-DateTime.Now.Second+30).AddMinutes(5);
            tmr5min.Change((int)((TimeSpan)(nextTime - DateTime.Now)).TotalMilliseconds, System.Threading.Timeout.Infinite);

           
            tmr30min.Elapsed += new System.Timers.ElapsedEventHandler(tmr30min_Elapsed);
            tmr1hour.Elapsed += new System.Timers.ElapsedEventHandler(tmr1hour_Elapsed);
            //tmr30Sec_Elapsed(null, null);
            //tmr2min_Elapsed(null, null);
          //  tmr30Sec.Start();
            //tmr2min.Start();
            tmr1hour.Start();
            tmr30min.Start();

        }

    //    static int oneHourCnt=0;

        static void tmr1hour_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.DateTime dt = System.DateTime.Now;

            dt =  dt.AddMinutes(-dt.Minute);
            dt = dt.AddSeconds(-dt.Second);
           
                if (dt.Hour  == 5)
                {
                    System.DateTime dt12;
                    dt12 = dt.AddHours(-dt.Hour);
                  
                    try
                    {
                        CreateEpConfXml(dt12);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        System.Environment.Exit(-1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message+","+ex.StackTrace);
                    }
                    try
                    {
                        Create1DayWeatherData(dt12);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        System.Environment.Exit(-1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + "," + ex.StackTrace);
                    }

                    try
                    {
                        Create1DayMaintainData( dt12);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        System.Environment.Exit(-1);
                    }
                    catch (Exception ex)
                    {
                           Console.WriteLine(ex.Message+","+ex.StackTrace);
                    }

                    try
                    {
                        //dt = dt.AddMinutes(-dt.Minute);
                        //dt = dt.AddSeconds(-dt.Second);
                       Create1DayVDData(dt12);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        System.Environment.Exit(-1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message+","+ex.StackTrace);
                    }

                    try
                    {
                        //dt = dt.AddMinutes(-dt.Minute);
                        //dt = dt.AddSeconds(-dt.Second);
                        Create1Day_vd_usable_rate_data(dt12);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        System.Environment.Exit(-1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + "," + ex.StackTrace);
                    }

                    try
                    {
                        //dt = dt.AddMinutes(-dt.Minute);
                        //dt = dt.AddSeconds(-dt.Second);
                        Create1Day_ramp_traffic_data(dt12);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        System.Environment.Exit(-1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + "," + ex.StackTrace);
                    }

                    try
                    {
                        SelectDown(3);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        System.Environment.Exit(-1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + "," + ex.StackTrace);
                    }
                  

                }
                try
                {
                    XmlReport.DeleteData();
                }
                catch (OutOfMemoryException ex)
                {
                    System.Environment.Exit(-1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "," + ex.StackTrace);
                }
            

                try
                {
                    Create1HourVDData(dt.AddHours(-1));
                }
                catch (OutOfMemoryException ex)
                {
                    System.Environment.Exit(-1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message+","+ex.StackTrace);
                }

                try
                {
                    Create1HourRampTrafficData(dt.AddHours(-1));
                }
                catch (OutOfMemoryException ex)
                {
                    System.Environment.Exit(-1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "," + ex.StackTrace);
                }
                try
                {
                    SelectDown(3);
                }
                catch (OutOfMemoryException ex)
                {
                    System.Environment.Exit(-1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "," + ex.StackTrace);
                }
                //try
                //{
                //    //dt = dt.AddMinutes(-dt.Minute);
                //    //dt = dt.AddSeconds(-dt.Second);
                //    Create1Day_ramp_traffic_data(dt.AddHours(-1));
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message + "," + ex.StackTrace);
                //}

         //   oneHourCnt++;
            
            //throw new Exception("The method or operation is not implemented.");
        }

        static void tmr30min_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            //setDateTimeFromHost();

            //throw new Exception("The method or operation is not implemented.");
        }


     
        static void tmr5min_Elapsed(object sender)
        {
            // throw new Exception("The method or operation is not implemented.");
            // System.DateTime vd5minDatetime = System.DateTime.Now.AddMinutes(-5);
            Console.WriteLine(System.DateTime.Now + ",in  5 min  task");
            System.DateTime dt5min = System.DateTime.Now.AddMinutes(-5);
            dt5min = dt5min.AddMinutes(-dt5min.Minute % 5);
            dt5min = dt5min.AddSeconds(-dt5min.Second);

            try
            {
                Create5MinVDXml(dt5min);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
           
            try
            {
                CreateVD1968data(dt5min);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
            try
            {
                Create5MinVIData(dt5min);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
            try
            {
                Create5MinRDData(dt5min);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
            try
            {
               Create5MinUnitTrafficData(dt5min);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }

            try
            {
                Create5MinSectionData(dt5min);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
          
            try
            {
                if(dt5min.Minute%10==0)
                      Create10MinAmData(dt5min);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }

            try
            {
                SelectDown(2);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }


            DateTime nextTime = DateTime.Now.AddMinutes(-DateTime.Now.Minute % 5).AddSeconds(-DateTime.Now.Second+30).AddMinutes(5);
            int m_sec=(int)((TimeSpan)(nextTime - DateTime.Now)).TotalMilliseconds;
            if(m_sec<=0) m_sec=1000;
            tmr5min.Change(m_sec, System.Threading.Timeout.Infinite);
            //  tmr2min.Start();
        }

        private static void setDateTimeFromHost()
        {
            try
            {
                System.DateTime dt = rhostobj.getDateTime();
                RemoteInterface.Util.SetSysTime((ushort)dt.Year, (ushort)dt.Month, (ushort)dt.Day, (ushort)dt.Hour, (ushort)dt.Minute, (ushort)dt.Second);
                Console.WriteLine("對時完成!");
            }
                 catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch(Exception ex) { Console.WriteLine(ex.Message+","+ex.StackTrace);}
        }

        static void tmr60Sec_Elapsed(object sender)  //one min task
        {
            Console.WriteLine(System.DateTime.Now + "in 1min task");
            System.DateTime dt = System.DateTime.Now.AddMinutes(-1);
            dt = dt.AddSeconds(-dt.Second);
            try
            {

                new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Create1minVdXml)).Start(dt);

                //  Create1minVdXml(dt);
            }
           
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }

            try
            {
                new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Create1MinSmsData)).Start(dt);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace); 
            }

            try{
                new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Create1MinIncidentData)).Start(dt);
               // Create1MinIncidentData(dt);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
         try{
             new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(CreatelMinEqOperationData)).Start(dt);
              //  CreatelMinEqOperationData(dt);
               // CreateEpConfXml(dt);
            }
             catch (OutOfMemoryException ex)
             {
                 System.Environment.Exit(-1);
             }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
            try
            {
                new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Create1MinConnectionStatus)).Start(dt);
             //   Create1MinConnectionStatus(dt);
                // CreateEpConfXml(dt);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }

            try
            {
                new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Create1MinAVITravelData)).Start(dt);
                //   Create1MinConnectionStatus(dt);
                // CreateEpConfXml(dt);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
            try
            {
                new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Create1MinAVIData)).Start(dt);
                //   Create1MinConnectionStatus(dt);
                // CreateEpConfXml(dt);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
            
            try
            {
                new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Create1MinSectionData)).Start(dt);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
             try
            {
                new System.Threading.Thread(new System.Threading.ParameterizedThreadStart( Create1MinTaiChung)).Start(dt);
            }
             catch (OutOfMemoryException ex)
             {
                 System.Environment.Exit(-1);
             }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
             try
             {
                 new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(CreateMiaoLiCounty1minVD)).Start(dt);
             }
             catch (OutOfMemoryException ex)
             {
                 System.Environment.Exit(-1);
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message + "," + ex.StackTrace);
             }
             try
             {
                 new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(CreateMiaoLiCounty1minSection)).Start(dt);
             }
             catch (OutOfMemoryException ex)
             {
                 System.Environment.Exit(-1);
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message + "," + ex.StackTrace);
             }
             try
             {
                 new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(CreateMiaoLiCounty1minCMS)).Start(dt);
             }
             catch (OutOfMemoryException ex)
             {
                 System.Environment.Exit(-1);
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message + "," + ex.StackTrace);
             }
             try
             {
                 new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SelectDown)).Start(1);
             }
             catch (OutOfMemoryException ex)
             {
                 System.Environment.Exit(-1);
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message + "," + ex.StackTrace);
             }
             try
             {
                 new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SelectDown)).Start(1);
             }
             catch (OutOfMemoryException ex)
             {
                 System.Environment.Exit(-1);
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message + "," + ex.StackTrace);
             }

             try
             {
                 new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SelectDown)).Start(5);
             }
             catch (OutOfMemoryException ex)
             {
                 System.Environment.Exit(-1);
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message + "," + ex.StackTrace);
             }
             try
             {
                 new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SelectDown)).Start(6);
             }
             catch (OutOfMemoryException ex)
             {
                 System.Environment.Exit(-1);
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message + "," + ex.StackTrace);
             }
             try
             {
                 new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SelectDown)).Start(7);
             }
             catch (OutOfMemoryException ex)
             {
                 System.Environment.Exit(-1);
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message + "," + ex.StackTrace);
             }
            
             try
             {
                 new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SelectDown)).Start(8);
             }
             catch (OutOfMemoryException ex)
             {
                 System.Environment.Exit(-1);
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message + "," + ex.StackTrace);
             }
             try
             {
                 new System.Threading.Thread(XmlReport.Hanbger).Start();
             }
             catch (OutOfMemoryException ex)
             {
                 System.Environment.Exit(-1);
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message + "," + ex.StackTrace);
             }
             try
             {
                 new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Create1MinNantouCmsData)).Start(dt);
             }
             catch (OutOfMemoryException ex)
             {
                 System.Environment.Exit(-1);
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message + "," + ex.StackTrace);
             }

             try
             {
                 new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Create1MinRspData)).Start(dt);

             }
             catch (OutOfMemoryException ex)
             {
                 System.Environment.Exit(-1);
             }
             catch (Exception ex)
             {
                  Console.WriteLine(ex.Message + "," + ex.StackTrace);
             }
             
            
            
            
            
              
           

            DateTime nextTime = DateTime.Now.AddMinutes(1).AddSeconds(-DateTime.Now.Second+30);
            int m_sec=(int)((TimeSpan)(nextTime - DateTime.Now)).TotalMilliseconds;
            if (m_sec <= 0) m_sec = 1000;
            tmr1min.Change(m_sec, System.Threading.Timeout.Infinite);


        }

        static void SelectDown(object param)
        {
            try
            {
                int i = System.Convert.ToInt32(param);
                XmlReport.SelectDown(i);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }
        }

        static void Create1MinSmsData(object ddt)
        {
            System.DateTime dt = (System.DateTime)ddt;
            string currpathFileName, histpathFilename;
            currpathFileName = timccexchg.Settings1.Default.CommCurrPath + "\\inc_shortmessage_data.xml";
           // string hispath = timccexchg.Settings1.Default.OneMinAVISectionHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
            //histpathFilename = hispath
            //  + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_1min_avi_travel_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);

            //if (!System.IO.Directory.Exists(hispath))
            //    System.IO.Directory.CreateDirectory(hispath);


            try
            {
                System.DateTime t = System.DateTime.Now;
                XmlReport.inc_shortmessage_data(timccexchg.Settings1.Default.dbConnectStr, dt, currpathFileName, "");
                Console.WriteLine("Create1MinSmsData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }



        }


        static void Create1MinAVITravelData(object ddt)
        {
            System.DateTime dt = (System.DateTime)ddt;
            string currpathFileName, histpathFilename;
            currpathFileName = timccexchg.Settings1.Default.CommCurrPath + "\\1min_avi_travel_data.xml";
            string hispath = timccexchg.Settings1.Default.OneMinAVISectionHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
            histpathFilename = hispath
              + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_1min_avi_travel_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);

            if (!System.IO.Directory.Exists(hispath))
                System.IO.Directory.CreateDirectory(hispath);


            try
            {
                System.DateTime t = System.DateTime.Now;
                XmlReport.one_min_avi_travel_data(timccexchg.Settings1.Default.dbConnectStr, dt, currpathFileName, histpathFilename);
                Console.WriteLine("Create1MinAVITravelData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
        }


      static  public void Create1MinRspData(object ddt)
        {

            System.DateTime dt = (System.DateTime)ddt;
            string hispath = timccexchg.Settings1.Default.OneMinRspPlanDataHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);

            string currpathFileName, histpathFilename; ;
            currpathFileName = timccexchg.Settings1.Default.CommCurrPath + "\\1min_rsp_plan_data.xml";

            histpathFilename = hispath
            + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_1min_rsp_plan_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);

            if (!System.IO.Directory.Exists(hispath))
                System.IO.Directory.CreateDirectory(hispath);

            try
            {
                System.DateTime t = System.DateTime.Now;
                XmlReport.one_min_reponse_plan_data(timccexchg.Settings1.Default.dbConnectStr, dt, currpathFileName, histpathFilename);
                Console.WriteLine("Create1MinRspData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }

        }

        static void Create1MinAVIData(object ddt)
        {
         
            System.DateTime dt = (System.DateTime)ddt;
            string hispath = timccexchg.Settings1.Default.OneMinAVIPlateHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);

            string currpathFileName, histpathFilename; ;
            currpathFileName = timccexchg.Settings1.Default.CommCurrPath + "\\1min_avi_data.xml";

            histpathFilename = hispath
            + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_1min_avi_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);

            if (!System.IO.Directory.Exists(hispath))
                System.IO.Directory.CreateDirectory(hispath);

            try
            {
                System.DateTime t = System.DateTime.Now;
                XmlReport.avi_1min_data(timccexchg.Settings1.Default.dbConnectStr, dt, currpathFileName, histpathFilename);
                Console.WriteLine("Create1MinAVIData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
        }


           static void Create1MinNantouCmsData(object ddt)
        {
            System.DateTime dt = (System.DateTime)ddt;
            string currpathFileName;
            currpathFileName = timccexchg.Settings1.Default.CommCurrPath + "\\1min_nantou_cms_data.xml";
            string hispath = timccexchg.Settings1.Default.OneMinSectionMiaoLiHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);

            if (!System.IO.Directory.Exists(hispath))
                System.IO.Directory.CreateDirectory(hispath);
            string histpathFilename = hispath
           + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_1min_nantou_cms_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);

            try
            {
                System.DateTime t = System.DateTime.Now;
                XmlReport.NantouCounty_CMS(timccexchg.Settings1.Default.dbConnectStr,  currpathFileName, histpathFilename);
                Console.WriteLine("Create1MinNantouCmsData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }
        }

        static void Create1MinSectionData(object ddt)
        {
            System.DateTime dt = (System.DateTime)ddt;
            string currpathFileName;
            currpathFileName = timccexchg.Settings1.Default.CommCurrPath + "\\1min_section_traffic_data.xml";
            string hispath = timccexchg.Settings1.Default.OneMinSectionVDHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);

            if (!System.IO.Directory.Exists(hispath))
                System.IO.Directory.CreateDirectory(hispath);
            string histpathFilename = hispath
           + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_1min_section_traffic_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);

            try
            {
                System.DateTime t = System.DateTime.Now;
                XmlReport.onemin_section_traffic_data(timccexchg.Settings1.Default.dbConnectStr, dt, currpathFileName, histpathFilename);
                Console.WriteLine("Create1MinSectionData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
        }


        static void Create1MinConnectionStatus(object ddt)
        {
            try
            {
                System.DateTime dt = (System.DateTime)ddt;
                System.DateTime t = System.DateTime.Now;
                XmlReport.one_connect_data(timccexchg.Settings1.Default.dbConnectStr, dt, timccexchg.Settings1.Default.CommCurrPath + "\\1min_connect_data.txt", "");
                Console.WriteLine("Create1MinConnectionStatus Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
        }

        static void Create1MinTaiChung(object ddt)
        {
            try
            {
                System.DateTime dt = (System.DateTime)ddt;
                System.DateTime t = System.DateTime.Now;
                XmlReport.TaichungCity_VD(timccexchg.Settings1.Default.dbConnectStr,  timccexchg.Settings1.Default.CommCurrPath + "\\TaichungCity_VD_RT.xml", "");
                Console.WriteLine("TaichungCity_VD Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }

            try
            {
                System.DateTime dt = (System.DateTime)ddt;
                System.DateTime t = System.DateTime.Now;
                XmlReport.TaichungCity_Section(timccexchg.Settings1.Default.dbConnectStr, timccexchg.Settings1.Default.CommCurrPath + "\\TaichungCity_RD_RT.xml", "");
                Console.WriteLine("TaichungCity_RD_RT.xml Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }
            try
            {
                System.DateTime dt = (System.DateTime)ddt;
                System.DateTime t = System.DateTime.Now;
                XmlReport.TaichungCity_CMS(timccexchg.Settings1.Default.dbConnectStr, timccexchg.Settings1.Default.CommCurrPath + "\\TaichungCity_CMS_RT.xml", "");
                Console.WriteLine("TaichungCity_CS_RT.xml Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }
        }

        static void Create5MinSectionData(object  ddt)
        {
            string currpathFileName, hisPathFilenname;
            System.DateTime dt = (System.DateTime)ddt;
            string hispath = timccexchg.Settings1.Default.SecDataHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
            DateTime t = DateTime.Now;
            
            if (!System.IO.Directory.Exists(hispath))
                System.IO.Directory.CreateDirectory(hispath);


            hisPathFilenname = hispath
            + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_section_traffic_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


            currpathFileName = timccexchg.Settings1.Default.SecDataCurrPath + "\\section_traffic_data.xml";

            XmlReport.section_traffic_data(timccexchg.Settings1.Default.dbConnectStr, dt,
               currpathFileName, hisPathFilenname);
            Console.WriteLine("Create5MinSectionData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);


        }
        static void Create5MinVDXml(System.DateTime dt)
        {
          
            string currpathFileName, hisPathFilenname;

            DateTime t = DateTime.Now;
            string hispath = timccexchg.Settings1.Default.FiveMinVDHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
            if (!System.IO.Directory.Exists(hispath))
                System.IO.Directory.CreateDirectory(hispath);


            hisPathFilenname = hispath
            + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_5min_vd_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


            currpathFileName = timccexchg.Settings1.Default.FiveMinVDCurrPath + "\\5min_vd_data.xml";

            XmlReport.five_min_vd_data(  timccexchg.Settings1.Default.dbConnectStr, dt,
               currpathFileName, hisPathFilenname);
            Console.WriteLine("Create5minVdXml Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);

        }

        static void Create5MinUnitTrafficData(System.DateTime dt)
        {

            string currpathFileName, hisPathFilenname;

            DateTime t = DateTime.Now;
            string hispath = timccexchg.Settings1.Default.FiveMinVDHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
            if (!System.IO.Directory.Exists(hispath))
                System.IO.Directory.CreateDirectory(hispath);


            hisPathFilenname = hispath
            + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


            currpathFileName = timccexchg.Settings1.Default.FiveMinVDCurrPath + "\\unit_traffic_data.xml";

            XmlReport.unit_section_traffic_data(timccexchg.Settings1.Default.dbConnectStr, dt,
               currpathFileName, "");
            Console.WriteLine("Create5MinUnitTrafficData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);

        }
        static void Create5MinVIData(System.DateTime dt)
        {

            string currpathFileName, hisPathFilenname;

            DateTime t = DateTime.Now;
            string hispath = timccexchg.Settings1.Default.FiveMinVIHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
            if (!System.IO.Directory.Exists(hispath))
                System.IO.Directory.CreateDirectory(hispath);


            hisPathFilenname = hispath
            + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_5min_vi_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


            currpathFileName = timccexchg.Settings1.Default.FiveMinVDCurrPath + "\\5min_vi_data.xml";

            XmlReport.five_min_vi_data(timccexchg.Settings1.Default.dbConnectStr, dt,
               currpathFileName, hisPathFilenname);
            Console.WriteLine("Create5MinVIData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);

        }
        static void Create5MinRDData(System.DateTime dt)
        {

            string currpathFileName, hisPathFilenname;

            DateTime t = DateTime.Now;
            string hispath = timccexchg.Settings1.Default.FiveMinRDHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
            if (!System.IO.Directory.Exists(hispath))
                System.IO.Directory.CreateDirectory(hispath);


            hisPathFilenname = hispath
            + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_5min_rd_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


            currpathFileName = timccexchg.Settings1.Default.FiveMinVDCurrPath + "\\5min_rd_data.xml";

            XmlReport.five_min_rd_data(timccexchg.Settings1.Default.dbConnectStr, dt,
               currpathFileName, hisPathFilenname);
            Console.WriteLine("Create5MinRDData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);

        }
        static void Create10MinAmData(System.DateTime dt)
        {

            string currpathFileName, hisPathFilenname;

            DateTime t = DateTime.Now;
            string hispath = timccexchg.Settings1.Default.TenMinWDHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
            if (!System.IO.Directory.Exists(hispath))
                System.IO.Directory.CreateDirectory(hispath);


            hisPathFilenname = hispath
            + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_10min_am_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


            currpathFileName = timccexchg.Settings1.Default.FiveMinVDCurrPath + "\\10min_am_data.xml";

            XmlReport.ten_min_am_data(timccexchg.Settings1.Default.dbConnectStr, dt,
               currpathFileName, hisPathFilenname);
            Console.WriteLine("Create10MinAmData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);

        }


        static void CreateMiaoLiCounty1minCMS(object ddt)
        {
            try
            {
                System.DateTime t = System.DateTime.Now;

                System.DateTime dt = (System.DateTime)ddt;

                string currpathFileName, hisPathFilenname;


                string hispath = timccexchg.Settings1.Default.OneMinSectionMiaoLiHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
                if (!System.IO.Directory.Exists(hispath))
                    System.IO.Directory.CreateDirectory(hispath);




                hisPathFilenname = hispath
                + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_miaoli_1min_cms_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


                currpathFileName = timccexchg.Settings1.Default.OneMinVDCurrPath + "\\miaoli_1min_cms_data.xml";

                XmlReport.MiaoliCounty_CMS(timccexchg.Settings1.Default.dbConnectStr,
                   currpathFileName, hisPathFilenname);

                Console.WriteLine("CreateMiaoLiCounty1minCMS Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }
        }

        static void CreateMiaoLiCounty1minSection(object ddt)
        {
            try
            {
                System.DateTime t = System.DateTime.Now;

                System.DateTime dt = (System.DateTime)ddt;

                string currpathFileName, hisPathFilenname;


                string hispath = timccexchg.Settings1.Default.OneMinSectionMiaoLiHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
                if (!System.IO.Directory.Exists(hispath))
                    System.IO.Directory.CreateDirectory(hispath);




                hisPathFilenname = hispath
                + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_miaoli_1min_sec_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


                currpathFileName = timccexchg.Settings1.Default.OneMinVDCurrPath + "\\miaoli_1min_sec_data.xml";

                XmlReport.MiaoliCounty_Section(timccexchg.Settings1.Default.dbConnectStr,
                   currpathFileName, hisPathFilenname);

                Console.WriteLine("CreateMiaoLiCounty1minSection Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }
        }

        static void CreateMiaoLiCounty1minVD(object ddt)
        {
            try
            {
                System.DateTime t = System.DateTime.Now;

                System.DateTime dt = (System.DateTime)ddt;

                string currpathFileName, hisPathFilenname;


                string hispath = timccexchg.Settings1.Default.OneMinVDMiaoLiHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
                if (!System.IO.Directory.Exists(hispath))
                    System.IO.Directory.CreateDirectory(hispath);




                hisPathFilenname = hispath
                + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_miaoli_1min_vd_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


                currpathFileName = timccexchg.Settings1.Default.OneMinVDCurrPath + "\\miaoli_1min_vd_data.xml";

                XmlReport.MiaoliCounty_VD(timccexchg.Settings1.Default.dbConnectStr, 
                   currpathFileName, hisPathFilenname);

                Console.WriteLine("CreateMiaoLiCounty1minVD Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }
        }
        static void Create1minVdXml(object ddt)
        {
            try
            {
                System.DateTime t = System.DateTime.Now;

                System.DateTime dt = (System.DateTime)ddt;

                string currpathFileName, hisPathFilenname;


                string hispath = timccexchg.Settings1.Default.OneMinVDHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
                if (!System.IO.Directory.Exists(hispath))
                    System.IO.Directory.CreateDirectory(hispath);




                hisPathFilenname = hispath
                + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_1min_vd_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


                currpathFileName = timccexchg.Settings1.Default.OneMinVDCurrPath + "\\1min_vd_data.xml";

                XmlReport.one_min_vd_data(timccexchg.Settings1.Default.dbConnectStr, dt,
                   currpathFileName, hisPathFilenname);

                Console.WriteLine("Create1minVdXml Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }

        }
        static void  CreateEpConfXml( System.DateTime dt)
        {
              string currpathFileName;
              currpathFileName = timccexchg.Settings1.Default.EpConfCurrPath + "\\1day_eq_config_data.xml";

              try
              {
                  System.DateTime t = System.DateTime.Now;
                  XmlReport.one_day_eq_config_data(timccexchg.Settings1.Default.dbConnectStr, dt, currpathFileName, "");
                  Console.WriteLine("CreateEpConfXml Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
              }
              catch (OutOfMemoryException ex)
              {
                  System.Environment.Exit(-1);
              }
              catch (Exception ex)
              {
                  Console.WriteLine(ex.Message+","+ex.StackTrace);
              }


        }
        static void Create1MinIncidentData(object ddt)
        {
          
            System.DateTime dt = (System.DateTime)ddt;
            string currpathFileName;
            currpathFileName = timccexchg.Settings1.Default.CommCurrPath + "\\1min_incident_data.xml";

            try
            {
                System.DateTime t = System.DateTime.Now;
                XmlReport.one_min_incident_data(timccexchg.Settings1.Default.dbConnectStr, dt, currpathFileName, "");
                Console.WriteLine("Create1MinIncidentData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);

            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
           
        }

       


        static void CreatelMinEqOperationData(object ddt)
        {
            System.DateTime dt = (System.DateTime)ddt;
            string currpathFileName;
            currpathFileName = timccexchg.Settings1.Default.CommCurrPath + "\\1min_eq_operation_data.xml";
         

            try
            {
                System.DateTime t = System.DateTime.Now;
                XmlReport.one_min_eq_operation_data(timccexchg.Settings1.Default.dbConnectStr, dt, currpathFileName, "");
                Console.WriteLine("CreatelMinEqOperationData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+","+ex.StackTrace);
            }
          
        }
        static void CreateVD1968data(System.DateTime dt)
        {
            string vddefcurrpathFileName, vdcurrPathFileName, cmsdefcurrpathFileName, cmscurrPathFileName;
            vddefcurrpathFileName = timccexchg.Settings1.Default.CommCurrPath + "\\vd_def.xml";
            vdcurrPathFileName =  timccexchg.Settings1.Default.CommCurrPath + "\\vd.xml";
             cmsdefcurrpathFileName = timccexchg.Settings1.Default.CommCurrPath + "\\cms_def.xml";
            cmscurrPathFileName =  timccexchg.Settings1.Default.CommCurrPath + "\\cms.xml";
            try
            {
              
               // XmlReport.VD_1968_data(timccexchg.Settings1.Default.dbConnectStr, dt, currpathFileName, "",currPathFileName1,"");
                try
                {
                    DateTime t = DateTime.Now;
                    XmlReport.VD_1968_data(timccexchg.Settings1.Default.dbConnectStr, dt, vdcurrPathFileName, "");
                    Console.WriteLine("Create VD_1968_data Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
                }
                catch (OutOfMemoryException ex)
                {
                    System.Environment.Exit(-1);
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                }

                try
                {
                    DateTime t = DateTime.Now;
                    XmlReport.VD_1968_Def_data(timccexchg.Settings1.Default.dbConnectStr, dt, vddefcurrpathFileName, "");
                    Console.WriteLine("Create VD_1968_Def_data Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
                }
                catch (OutOfMemoryException ex)
                {
                    System.Environment.Exit(-1);
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                }

                try
                {
                    DateTime t = DateTime.Now;
                    XmlReport.CMS_1968_Data(timccexchg.Settings1.Default.dbConnectStr, dt, cmscurrPathFileName, "");
                    Console.WriteLine("Create CMS_1968_Data Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
                }
                catch (OutOfMemoryException ex)
                {
                    System.Environment.Exit(-1);
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                }
                try
                {
                    DateTime t = DateTime.Now;
                    XmlReport.CMS_1968_Def_Data(timccexchg.Settings1.Default.dbConnectStr, dt, cmsdefcurrpathFileName, "");
                    Console.WriteLine("Create CMS_1968_Def_Data Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);

                }
                catch (OutOfMemoryException ex)
                {
                    System.Environment.Exit(-1);
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                }

             

            }
            catch (OutOfMemoryException ex)
            {
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message+","+ex.StackTrace);
            }
        }
        static void Create1HourVDData(System.DateTime dt)
        {

            string currpathFileName, hisPathFilenname;

            DateTime t = DateTime.Now;
            string hispath = timccexchg.Settings1.Default.OneHourVDHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
            if (!System.IO.Directory.Exists(hispath))
                System.IO.Directory.CreateDirectory(hispath);


            hisPathFilenname = hispath
            + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_1hour_vd_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


            currpathFileName = timccexchg.Settings1.Default.FiveMinVDCurrPath + "\\1hour_vd_data.xml";

            XmlReport.one_hr_vd_data(timccexchg.Settings1.Default.dbConnectStr, dt,
               currpathFileName, hisPathFilenname);
            Console.WriteLine("Create1HourVDData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);

        }
        static void Create1HourRampTrafficData(System.DateTime dt)
        {

            string currpathFileName, hisPathFilenname;

            DateTime t = DateTime.Now;
            string hispath = timccexchg.Settings1.Default.OneHourRampTrafficDataHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
            if (!System.IO.Directory.Exists(hispath))
                System.IO.Directory.CreateDirectory(hispath);


            hisPathFilenname = hispath
            + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_1hour_ramp_traffic_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


            currpathFileName = timccexchg.Settings1.Default.FiveMinVDCurrPath + "\\1hour_ramp_traffic_data.xml";

            XmlReport.hour_ramp_traffic_data(timccexchg.Settings1.Default.dbConnectStr, dt,
               currpathFileName, hisPathFilenname);
            Console.WriteLine("Create1HourRampTrafficData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);

        }


        static void Create1DayWeatherData(System.DateTime dt)
        {
            //string currpathFileName, hisPathFilenname;

            DateTime t = DateTime.Now;
            //string hispath = timccexchg.Settings1.Default.OneDayVDHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
            //if (!System.IO.Directory.Exists(hispath))
            //    System.IO.Directory.CreateDirectory(hispath);


            //hisPathFilenname = hispath
            //+ "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_1day_vd_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


            //currpathFileName = timccexchg.Settings1.Default.FiveMinVDCurrPath + "\\1day_vd_data.xml";

            XmlReport.one_day_WeathData(timccexchg.Settings1.Default.OneMinVDCurrPath + "\\");
            Console.WriteLine("Create1DayWeatherData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
        }
        static void Create1DayVDData(System.DateTime dt)
        {

            string currpathFileName, hisPathFilenname;

            DateTime t = DateTime.Now;
            string hispath = timccexchg.Settings1.Default.OneDayVDHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
            if (!System.IO.Directory.Exists(hispath))
                System.IO.Directory.CreateDirectory(hispath);


            hisPathFilenname = hispath
            + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_1day_vd_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


            currpathFileName = timccexchg.Settings1.Default.FiveMinVDCurrPath + "\\1day_vd_data.xml";

            XmlReport.one_day_vd_data(timccexchg.Settings1.Default.dbConnectStr, dt,
               currpathFileName, hisPathFilenname);
            Console.WriteLine("Create1DayVDData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);

        }
        static void Create1DayMaintainData(System.DateTime dt)
        {
            string currpathFileName;
            //, hisPathFilenname;

            DateTime t = DateTime.Now;

           // string hispath = timccexchg.Settings1.Default.OneDayVDHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
            //if (!System.IO.Directory.Exists(hispath))
            //    System.IO.Directory.CreateDirectory(hispath);


          //  hisPathFilenname = hispath
            // + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


            currpathFileName = timccexchg.Settings1.Default.FiveMinVDCurrPath + "\\1day_maintain_data.xml";

            XmlReport.one_day_maintain_data(timccexchg.Settings1.Default.dbConnectStr, dt,
               currpathFileName,"");

            Console.WriteLine("Create1DayMaintainData Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);

        }
        static void Create1Day_vd_usable_rate_data(System.DateTime dt)
        {
            string currpathFileName, hisPathFilenname;

            DateTime t = DateTime.Now;
            string hispath = timccexchg.Settings1.Default.OneDayVDUnUseHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
            if (!System.IO.Directory.Exists(hispath))
                System.IO.Directory.CreateDirectory(hispath);


            hisPathFilenname = hispath
            + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_1day_vd_usable_rate_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


            currpathFileName = timccexchg.Settings1.Default.FiveMinVDCurrPath + "\\1day_vd_usable_rate_data.xml";

            XmlReport.vd_usable_rate_data(timccexchg.Settings1.Default.dbConnectStr, dt,
               currpathFileName, hisPathFilenname);
            Console.WriteLine("Create1Day_vd_usable_rate_data Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
        }
        static void Create1Day_ramp_traffic_data(System.DateTime dt)
        {
            string currpathFileName, hisPathFilenname;

            DateTime t = DateTime.Now;
            string hispath = timccexchg.Settings1.Default.OneDayRampTrafficDataHistPath + "\\" + string.Format("{0:00}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
            if (!System.IO.Directory.Exists(hispath))
                System.IO.Directory.CreateDirectory(hispath);


            hisPathFilenname = hispath
            + "\\" + string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}_1day_ramp_traffic_data.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);


            currpathFileName = timccexchg.Settings1.Default.FiveMinVDCurrPath + "\\1day_ramp_traffic_data.xml";

            XmlReport.day_ramp_traffic_data(timccexchg.Settings1.Default.dbConnectStr, dt,
               currpathFileName, hisPathFilenname);
            Console.WriteLine("Create1Day_ramp_traffic_data Cost:" + ((TimeSpan)(System.DateTime.Now - t)).TotalSeconds);
        }
    }
}
