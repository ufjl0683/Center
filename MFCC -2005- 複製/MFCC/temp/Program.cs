using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using System.Data;
using System.Reflection;
using System.Drawing;



namespace temp
{

    public delegate void new_travel_time_handler(DataSet ds);
    class sdata
    {
        byte[] data = new byte[10000];
        public  sdata pt;
        
    }

    class Program
    {

     
        public static RemoteInterface.EventNotifyServer eventDispatcher;

        public static event new_travel_time_handler OnNewTravelData;
        public static Ds Curr5minSecDs;
        public static Ds RGSConfDs = new Ds();
        public static Ds RMSConfDs = new Ds();
        public static System.Timers.Timer tmr1min = new System.Timers.Timer(100);
        //public static RGS_Manager rgs_manager;
        //public static RMS_Manager rms_manager;
        public static System.Collections.Hashtable ip_hash = new System.Collections.Hashtable();

        static RemoteInterface.EventNotifyClient nc;
        static void Main(string[] args)
        {


            byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes("888");

           System.Console.WriteLine(data[0]);

           int i = 0xffff;
       
           System.Console.WriteLine((short)(i));
            MAS_Test();

            Console.ReadKey();
        }


        public static void MAS_Test()
        {
            DataSet dataset = null;
            DataSet retds = null, ds = new DataSet();
            RemoteInterface.MFCC.I_MFCC_MAS robj = (RemoteInterface.MFCC.I_MFCC_MAS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.MFCC.I_MFCC_MAS),
                RemoteInterface.RemoteBuilder.getRemoteUri("127.0.0.1", (int)RemoteInterface.RemotingPortEnum.MFCC_MAS, "MFCC_MAS"));
            robj.SendDisplay("MAS-N1-S-175.3", 1, 0, 0, "系統測試", new byte[] { 0x20, 0x20, 0x20, 0x20 });
            robj.SendDisplay("MAS-N1-S-175.3", 2,0, 0, "系統測試", new byte[] { 0x20, 0x20, 0x20, 0x20 });
            robj.SendDisplay("MAS-N1-S-175.3", 3, 0, 0, "系統測試", new byte[] { 0x20, 0x20, 0x20, 0x20 });
            robj.SetDisplayOff("MAS-N1-S-175.3", 1);
        }
        public static void RMS_Test()
        {
            DataSet dataset = null;
            DataSet retds = null, ds = new DataSet();
            RemoteInterface.MFCC.I_MFCC_RMS robj = (RemoteInterface.MFCC.I_MFCC_RMS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.MFCC.I_MFCC_RMS),
                RemoteInterface.RemoteBuilder.getRemoteUri("192.168.22.89", (int)RemoteInterface.RemotingPortEnum.MFCC_RMS, "MFCC_RMS"));
            retds = robj.getSendDSByFuncName("get_ctl_mode_and_plan_no");

            try
            {
                retds = robj.sendTC("RMS235", retds);
            }
            catch
            {
                ;
            }
        }

        public static void WIS_Test()
        {
            RemoteInterface.MFCC.I_MFCC_WIS robj = (RemoteInterface.MFCC.I_MFCC_WIS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.MFCC.I_MFCC_WIS), RemoteInterface.RemoteBuilder.getRemoteUri("127.0.0.1", (int)RemoteInterface.RemotingPortEnum.MFCC_WIS, "MFCC_WIS"));

           // robj.SendDisplay("cms170", 0, 0, 0, "加油", new byte[] { 0x30, 0x30 });
           bool isconnect= robj.getConnectionStatus("WIS130");
           robj.ChangeDisplayCheckCycle("WIS130", 1);
           robj.SendDisplay("WIS130", 0, 0, "霧慢行", new byte[] { 0x30, 0x30,0x30 });
        }
        public static void ETTU_Test()
        {
            System.IO.Ports.SerialPort com3 = new System.IO.Ports.SerialPort("Com3", 9600);
            com3.Open();
           Comm.ETTUDLE dev=new Comm.ETTUDLE(com3.BaseStream);
           dev.OnReceiveText += new Comm.OnTextPackageEventHandler(dev_OnReceiveText);
           dev.OnTextSending += new Comm.OnSendingAckNakHandler(dev_OnTextSending);
           Comm.SendPackage pkg = new Comm.SendPackage(Comm.CmdType.CmdQuery, Comm.CmdClass.A, 0xffff, new byte[] { 0x27, 0x00 });
           dev.Send(pkg);
           pkg = new Comm.SendPackage(Comm.CmdType.CmdQuery, Comm.CmdClass.A, 0xffff, new byte[] { 0x27, 0x00 });
           dev.Send(pkg);
        }

        static void dev_OnReceiveText(object sender, Comm.TextPackage txtObj)
        {
            Console.WriteLine("rev:[" + Comm.V2DLE.ToHexString(txtObj.Text) + "]");
            if (txtObj.Text[0] == 0x27 && txtObj.Text[1] == 0x00)
            {
                Comm.ETTUDLE dev = (Comm.ETTUDLE)sender;
                lock (dev.GetStream())
                {
                    byte[] data = dev.PackData(0xffff, new byte[] { 0x29, 0x00 }, 0xc7);
                    dev.GetStream().Write(data,0,data.Length);
                }
            }
            //throw new Exception("The method or operation is not implemented.");
        }

        static void dev_OnTextSending(object sender, ref byte[] data)
        {
            Console.WriteLine("snd==>" + Comm.V2DLE.ToHexString(data) );

            
         //   throw new Exception("The method or operation is not implemented.");
        }

        public static void CMS_test()
        {
         RemoteInterface.MFCC.I_MFCC_CMS robj=  (RemoteInterface.MFCC.I_MFCC_CMS)  RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.MFCC.I_MFCC_CMS),RemoteInterface.RemoteBuilder.getRemoteUri("127.0.0.1",(int)RemoteInterface.RemotingPortEnum.MFCC_CMS,"MFCC_CMS"));
          
            robj.SendDisplay("cms170",0,0,0,"加油",new byte[]{0x30,0x30});
                
        }

        public static void QYServerSim()
        {
            try
            {

                //init_comm();
                load_icons_table();
                load_RGS_table();

                load_RMS_table();


                //rgs_manager = new RGS_Manager();
                //rms_manager = new RMS_Manager();


                //  Program.OnNewTravelData += new new_travel_time_handler(Program_OnNewSecData);


                tmr1min.Elapsed += new System.Timers.ElapsedEventHandler(tmr1min_Elapsed);
                tmr1min.Start();
                tmr1min_Elapsed(null, null);
                //   new System.Threading.Thread(section_data_1min_task).Start();
                //  new System.Threading.Thread(DisplayTask).Start();
                // new System.Threading.Thread(RmsModeTask).Start();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine("press any key to exit!");
                Console.ReadKey();
                //System.Environment.Exit(-1);
            }
        }
        static int tmrcnt = 0;
        static void tmr1min_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (tmrcnt++ % 600 == 0)
                section_data_1min_task();
            else
                System.GC.WaitForPendingFinalizers();
            // throw new Exception("The method or operation is not implemented.");
        }
           // memtest();
           // VD_Ttest();
        

        //public static Ds RGSConfDs = new Ds();
        //public static Ds RMSConfDs = new Ds();

        static void memtest()
        {
            System.Threading.Thread th = new System.Threading.Thread(threadComsumeMemeTask);
            th.Start();
            while (true)
            {
               
                    sdata s1 = new sdata();
                    sdata s2 = new sdata();
                    s1.pt = s2;
                    s2.pt = s1;
              
                
               // temp.Ds ds = new Ds();
                
                System.Threading.Thread.Sleep(1000*60);

            }
        }

        public static string getSaveDirFileName(System.DateTime dt)
        {
            if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "SecData"))
                System.IO.Directory.CreateDirectory(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "SecData"));
            string dir = string.Format(AppDomain.CurrentDomain.BaseDirectory + @"SecData\{0:0000}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);

            string filename = string.Format(@"{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);

            if (!System.IO.Directory.Exists(Util.CPath(dir)))
                System.IO.Directory.CreateDirectory(Util.CPath(dir));

            if (System.IO.File.Exists(temp.Util.CPath(dir + @"\" + filename)))
               return  temp.Util.CPath(dir + @"\" + filename); // return "";
            else
                return temp.Util.CPath(dir + @"\" + filename);


        }


        public static void calcuate_travel_time()
        {
            Console.WriteLine("calcute travel time!");
            lock (RGSConfDs.tblRGS_Config)
            {
                foreach (Ds.tblRGS_ConfigRow r in RGSConfDs.tblRGS_Config.Rows)
                {
                    System.Data.DataView vw = null;

                    vw = Util.get_travel_sections_detail_data(r.from_milepost, r.end_milepost, r.freewayId, r.direction);
                    //if(r.direction=="S")
                    //     vw =new DataView(Curr5minSecDs.tblSecTrafficData,
                    //    string.Format("from_milepost>={0} and  from_milepost < {1} and freewayId={2} and directionId='S'", r.from_milepost, r.end_milepost-1000, r.freewayId)
                    //    ,"",DataViewRowState.CurrentRows);
                    //else if(r.direction=="N")
                    //    vw =new DataView(Curr5minSecDs.tblSecTrafficData,
                    //    string.Format("from_milepost<={0} and  from_milepost > {1} and freewayId={2}  and directionId='N'", r.from_milepost, r.end_milepost+1000, r.freewayId)
                    //, "", DataViewRowState.CurrentRows);

                    r.lowerlimit = r.upperlimit = 0;
                    r.traveltime = 0;
                    r.RowError = "";
                    for (int i = 0; i < vw.Count; i++)
                    {

                        float traveltime = (float)vw[i]["travel_time"];
                        if (traveltime == 65535)
                            r.RowError = "無效的旅行時間";

                        r.traveltime += traveltime;
                        r.lowerlimit += System.Convert.ToUInt16(vw[i]["section_upper_limit"]);
                        r.upperlimit += System.Convert.ToUInt16(vw[i]["section_lower_limit"]);

                    }
                    r.lowerlimit = (ushort)(r.lowerlimit / 60);
                    r.upperlimit = (ushort)(r.upperlimit / 60);

                    r.message1 = r.msg_temp1;

                    if (r.HasErrors)
                    {
                        r.message1 = r.message2 = "";
                        r.messageColor2 = r.messageColor1 = "";

                    }
                    else
                    {

                        if (r.traveltime < r.upperlimit)
                        {

                            r.message1 = r.msg_temp1;
                            r.message2 = r.msg_temp2.Replace("@", System.Convert.ToUInt16(r.upperlimit).ToString());
                            r.RowError = "旅行時間超過上限值";



                        }
                        else
                        {
                            r.message2 = r.msg_temp2.Replace("@", System.Convert.ToUInt16(r.traveltime).ToString());
                            // set display color

                        }

                        if (r.traveltime > r.lowerlimit)
                        {
                            r.message1 = "";
                            r.message2 = "";
                            r.messageColor2 = r.messageColor1 = "";
                            r.RowError = "旅行時間超過下限值";
                        }

                        // set display color
                        Color[] colors = new Color[r.message1.Length];
                        for (int i = 0; i < colors.Length; i++)
                            colors[i] = Color.Red;
                        r.messageColor1 = Util.ToColorString(colors);

                        colors = new Color[r.message2.Length];
                        for (int i = 0; i < colors.Length; i++)
                        {
                            int colorinx = r.msg_temp2.IndexOf('@');
                            if (i >= colorinx && i <= System.Convert.ToUInt16(r.upperlimit).ToString().Length + colorinx - 1)
                                colors[i] = Color.Orange;
                            else

                                colors[i] = Color.Red;
                        }
                        r.messageColor2 = Util.ToColorString(colors);

                    }





                }
            }
        }
        public static void section_data_1min_task()
        {

            Console.WriteLine("section_data_1min_task started!");

           //while(true)
           //{
                try
                {
                    System.Net.WebRequest req = System.Net.HttpWebRequest.Create(temp.Settings1.Default.TIMCC_uri);

                    System.IO.Stream stream = req.GetResponse().GetResponseStream();
                    System.IO.TextReader rd = new System.IO.StreamReader(stream);
                    System.IO.TextWriter wr = new System.IO.StreamWriter(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "tmp.xml"));
                    wr.Write(rd.ReadToEnd());
                    wr.Flush();
                    wr.Close();
                    rd.Close();
                    stream.Close();
                    wr.Dispose();
                    rd.Dispose();
                    stream.Dispose();


                    Ds ds = five_min_section_parser(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "tmp.xml"));
                   
                    Curr5minSecDs = ds;


                    string dest;
                    if ((dest = getSaveDirFileName(ds.tblFileAttr[0].time)) != "")   // new 5 min data
                    {
                        Console.WriteLine("section_data_1min_task: new section data->" + ds.tblFileAttr[0][0].ToString());


                        System.IO.File.Copy(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "tmp.xml"), dest,true);
                        if (OnNewTravelData != null)
                        {

                            calcuate_travel_time();

                            OnNewTravelData(RGSConfDs);
                            try
                            {
                                NotifyDisplayTask();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("section_data_1min_task:" + ex.Message);
                            }
                        }


                    }



                }
                catch (Exception ex)
                {
                    Console.WriteLine("section_data_1min_task:" + ex.Message + ex.StackTrace);

                    try
                    {
                        if (Curr5minSecDs == null || System.Math.Abs(((TimeSpan)(System.DateTime.Now - Curr5minSecDs.tblFileAttr[0].time)).Minutes) >= 20)
                        {
                            lock (RGSConfDs.tblRGS_Config)
                            {
                                foreach (temp.Ds.tblRGS_ConfigRow r in RGSConfDs.tblRGS_Config.Rows)
                                {
                                    r.RowError = "Timcc 連線資料異常!";
                                    r.mode = 1; //手動
                                    Console.WriteLine("寫入 RowErr");



                                }
                                if (OnNewTravelData != null)
                                {

                                    try
                                    {
                                        DataSet ds1 = new DataSet();

                                        ds1.Tables.Add(Util.getPureDataTable(RGSConfDs.tblRGS_Config));
                                        OnNewTravelData(ds1);
                                        NotifyDisplayTask();
                                    }
                                    catch (Exception ex1)
                                    {
                                        Console.WriteLine("section_data_1min_task:" + ex1.StackTrace);
                                    }
                                }
                            }
                            System.Console.WriteLine("Timcc 連線異常!");
                        }
                    }
                    catch (Exception ex1)
                    {
                        Console.WriteLine(ex1.Message);
                    }

                }



                // System.GC.Collect();
                //System.GC.WaitForPendingFinalizers();

                //System.Threading.Thread.Sleep(60 * 1000);
            //}//





        }
        private static System.Object dispLockObj = new object();
        internal static void NotifyDisplayTask()
        {
            lock (dispLockObj)
            {
                System.Threading.Monitor.Pulse(dispLockObj);
            }
        }

        public static Ds five_min_section_parser(string uri)
        {

            System.Xml.XmlReader rd = null;
            Ds ds = new Ds();

            using (rd = System.Xml.XmlTextReader.Create(uri))
            {
                while (rd.Read())
                {

                    if (rd.Name == "traffic_data" && rd.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        Ds.tblSecTrafficDataRow r = ds.tblSecTrafficData.NewtblSecTrafficDataRow();

                        string dir = "";
                        switch (System.Convert.ToString(rd["directionId"]))
                        {
                            case "1":
                                dir = "E";
                                break;
                            case "2":
                                dir = "W";
                                break;
                            case "3":
                                dir = "S";
                                break;
                            case "4":
                                dir = "N";
                                break;
                        }
                        r.directionId = dir;
                        r.end_location = System.Convert.ToString(rd["end_location"]);
                        r.end_milepost = System.Convert.ToUInt32(rd["end_milepost"]);
                        r.expresswayId = rd["expresswayId"].ToString();
                        r.freewayId = rd["freewayId"].ToString();
                        r.from_location = rd["from_location"].ToString();
                        r.from_milepost = System.Convert.ToUInt32(rd["from_milepost"]);
                        r.section_lower_limit = System.Convert.ToUInt32(rd["section_lower_limit"]);
                        r.section_upper_limit = System.Convert.ToUInt32(rd["section_upper_limit"]);
                        r.travel_time = System.Convert.ToSingle(rd["travel_time"]);

                        ds.tblSecTrafficData.AddtblSecTrafficDataRow(r);

                    }
                    else if (rd.Name == "file_attribute" && rd.NodeType == System.Xml.XmlNodeType.Element)

                        ds.tblFileAttr.AddtblFileAttrRow(System.Convert.ToDateTime(rd["time"]));


                }
                rd.Close();

            }
            return ds;

        }
        static    void threadComsumeMemeTask()
        {
            byte[]bs;
            
            while (true)
            {
                bs = new byte[10000];
                sdata s1 = new sdata();
                sdata s2 = new sdata();
                System.Net.WebRequest req = System.Net.HttpWebRequest.Create(temp.Settings1.Default.TIMCC_uri);

                System.IO.Stream stream = req.GetResponse().GetResponseStream();
                System.IO.TextReader rd = new System.IO.StreamReader(stream);
                System.IO.TextWriter wr = new System.IO.StreamWriter(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "tmp.xml"));
                wr.Write(rd.ReadToEnd());
                wr.Flush();
                wr.Close();
                rd.Close();
                stream.Close();
                wr.Dispose();
                rd.Dispose();
                stream.Dispose();


                Ds ds = five_min_section_parser(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "tmp.xml"));
                // Curr5minSecDs.Dispose();
                Curr5minSecDs = ds;
                System.Threading.Thread.Sleep(1000);
                
            }
        }

        private static void load_RMS_table()
        {
            int rowcnt = 0;
            System.IO.TextReader tr = new System.IO.StreamReader(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "rms_config.csv"), System.Text.Encoding.GetEncoding("big5"));
           // temp.Ds.tblRmsConfigRow rr;
            string s = "";

            string[] fields;
            s = tr.ReadLine();

            fields = s.Split(new char[] { ',' });

            for (int i = 0; i < fields.Length; i++)
                fields[i] = fields[i].Trim();


            while ((s = tr.ReadLine()) != null)
            {


                string[] tmp = s.Split(new char[] { ',' });

                temp.Ds.tblRmsConfigRow r = RMSConfDs.tblRmsConfig.NewtblRmsConfigRow(); // RGSConfDs.tblRGS_Config.NewRow();

                for (int i = 0; i < tmp.Length; i++)
                {
                    r[i] = System.Convert.ChangeType(tmp[i], RMSConfDs.tblRmsConfig.Columns[i].DataType);
                }
                r.ctl_mode_setting = r.ctl_mode_setting = 2; //default mode;

                r.ctl_mode = 2;
                r.planno_setting = r.planno = 0;



                RMSConfDs.tblRmsConfig.AddtblRmsConfigRow((Ds.tblRmsConfigRow)r);
                rowcnt++;
                //----------------------------just for test
                //if (rowcnt == 1)
                //    break;

            }

            RMSConfDs.AcceptChanges();

            tr.Close();
            tr.Dispose();
            Console.WriteLine("RMS_Table loaded!");
        }

        private static void load_icons_table()
        {
            //  throw new Exception("The method or operation is not implemented.");


            System.IO.TextReader tr = new System.IO.StreamReader(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "icons.csv"), System.Text.Encoding.GetEncoding("big5"));


            RGSConfDs.tblIcons.AddtblIconsRow(0, "無");
            string s = "";
            string[] fields;
            s = tr.ReadLine();

            fields = s.Split(new char[] { ',' });

            for (int i = 0; i < fields.Length; i++)
                fields[i] = fields[i].Trim();



            while ((s = tr.ReadLine()) != null)
            {
                string[] tmp = s.Split(new char[] { ',' });

                System.Data.DataRow r = RGSConfDs.tblIcons.NewRow();

                for (int i = 0; i < tmp.Length; i++)
                {
                    r[i] = System.Convert.ChangeType(tmp[i], RGSConfDs.tblIcons.Columns[i].DataType);
                }


                RGSConfDs.tblIcons.Rows.Add(r);
            }

        }
        static void load_RGS_table()
        {

            int rowcnt = 0;

            System.IO.TextReader tr = new System.IO.StreamReader(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "RGS_Config.csv"), System.Text.Encoding.GetEncoding("big5"));

            temp.Ds.tblRGSModeRow rr;

            rr = RGSConfDs.tblRGSMode.NewtblRGSModeRow();
            rr.value = 0;
            rr.display = "旅行時間模式";
            RGSConfDs.tblRGSMode.AddtblRGSModeRow(rr);
            rr = RGSConfDs.tblRGSMode.NewtblRGSModeRow();
            rr.value = 1;
            rr.display = "手動輸入模式";
            RGSConfDs.tblRGSMode.AddtblRGSModeRow(rr);




            string s = "";
            string[] fields;
            s = tr.ReadLine();

            fields = s.Split(new char[] { ',' });

            for (int i = 0; i < fields.Length; i++)
                fields[i] = fields[i].Trim();



            while ((s = tr.ReadLine()) != null)
            {


                string[] tmp = s.Split(new char[] { ',' });

                System.Data.DataRow r = RGSConfDs.tblRGS_Config.NewRow();

                for (int i = 0; i < tmp.Length; i++)
                {
                    r[i] = System.Convert.ChangeType(tmp[i], RGSConfDs.tblRGS_Config.Columns[i].DataType);
                }
                if (r["freewayId"].ToString() == "0")
                    r["mode"] = 1;
                else
                    r["mode"] = 0;
                r["ficon"] = 0;

                r["message1"] = "";
                r["message2"] = "";

                r["finput1"] = "";
                r["finput2"] = "";
                r["traveltime"] = 0;
                r["lowerlimit"] = 0;
                r["upperlimit"] = 0;
                lock (temp.Program.RGSConfDs.tblRGSMain)
                {
                    if (ip_hash[r["ip"].ToString()] == null)
                    {
                        temp.Ds.tblRGSMainRow rmain = RGSConfDs.tblRGSMain.NewtblRGSMainRow();
                        ip_hash.Add(r["ip"].ToString(), rmain);
                        rmain.ip = r["ip"].ToString();
                        rmain.rgs_name = r["rgs_name"].ToString();
                        rmain.location = System.Convert.ToUInt32(r["location"]);
                        rmain.freewayId = r["freewayId"].ToString();
                        rmain.direction = r["direction"].ToString();
                        rmain.deviec_id = System.Convert.ToInt32(r["deviec_id"].ToString(), 16);
                        rmain.port = (uint)r["port"];
                        rmain.connected = false;
                        rmain.hwstatus1 = rmain.hwstatus2 = rmain.hwstatus3 = rmain.hwstatus4 = 0;
                        rmain.DisplayErr = rmain.DownLinkErr = rmain.DeviceErr = rmain.LED_ModuleErr = rmain.UplinkErr = rmain.CabineteOpen = false;
                        rmain.EndEdit();
                        RGSConfDs.tblRGSMain.AddtblRGSMainRow(rmain);
                    }
                }
                RGSConfDs.tblRGS_Config.Rows.Add(r);
                rowcnt++;
                ////----------------------------just for test
                //if (rowcnt == 2)
                //    break;
            }



        }



        static void mono_version()
        {
            Type t = Type.GetType("Mono.Runtime");
            MethodInfo mi = t.GetMethod("GetDisplayName", BindingFlags.NonPublic |
           BindingFlags.Static);
            string s = (string)mi.Invoke(null, null);
            if (mi != null)
                Console.WriteLine(s);
        }
        static RemoteInterface.EventNotifyClient nclient;// = new EventNotifyClient("127.0.0.1", (int)RemotingPortEnum.MFCC_VD, true);

        static void VD_Ttest()
        {
            DataSet ds=null;
            RemoteInterface.MFCC.I_MFCC_VD robj=null;
          
            nclient.OnConnect += new OnConnectEventHandler(nclient_OnConnect);
            try
            {
               robj = (RemoteInterface.MFCC.I_MFCC_VD)RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.MFCC.I_MFCC_VD), RemoteBuilder.getRemoteUri("127.0.0.1", (int)RemotingPortEnum.MFCC_VD1, "MFCC_VD"));
            }
            catch { ;}

            while (true)
            {
                if(robj==null)
                    robj = (RemoteInterface.MFCC.I_MFCC_VD)RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.MFCC.I_MFCC_VD), RemoteBuilder.getRemoteUri("127.0.0.1", (int)RemotingPortEnum.MFCC_VD1, "MFCC_VD"));
                try
                {
                    ds = robj.getSendDSByFuncName("get_date_time");
                    foreach (DataColumn c in ds.Tables[0].Columns)
                    {
                        Console.Write(c.ColumnName + ":" + ds.Tables[0].Rows[0][c.ColumnName].ToString() + "\t");
                    }

                }
              
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
               
               
                try
                {
                    ds = robj.sendTC("vd180", ds);
                    foreach (DataColumn c in ds.Tables[0].Columns)
                    {
                        Console.Write(c.ColumnName + ":" + ds.Tables[0].Rows[0][c.ColumnName].ToString() + "\t");
                    }

                }
                catch (Exception ex)
                {
                  //  robj = null;
                    
                    Console.WriteLine(ex.Message);
                }

               
                byte[] status ;
                try
                {
                    status = robj.getHWstatus("vd180");
                    I_HW_Status_Desc desc = robj.getHWdesc("vd180");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }


                robj.setRealTime("vd180",1,30,20);
                Console.ReadKey();
                robj.setRealTime("vd180",0,30,0);

            }
            
           
        }

        static void nclient_OnConnect(object sender)
        {
            //throw new Exception("The method or operation is not implemented.");
            try
            {
                nc.OnEvent += new NotifyEventHandler(nclient_OnEvent);
            }
            catch (Exception ex)
            {
                //do some thing here
            }
        }

        static void nclient_OnEvent(object sender,NotifyEventObject objEvent)
        {
           // throw new Exception("The method or operation is not implemented.");
            if (objEvent.type == EventEnumType.VD_Real_Data_Event)
            {
                System.Data.DataSet ds = (System.Data.DataSet)objEvent.EventObj;

                System.Data.DataRow r = ds.Tables[0].Rows[0];
                for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                {
                    Console.Write(ds.Tables[0].Columns[i].ColumnName + ":" + r[i].ToString());
                }
                Console.WriteLine();
            }

        }
    }
}
