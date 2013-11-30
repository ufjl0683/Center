using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Drawing;
using RemoteInterface.MFCC;
using RemoteInterface;
using Comm;

namespace test
{
    class Program
    {


        static void Main(string[] args)
        {


            HostNotifyTest();
           // rgsTest();
         //  vdtest();
            //System.Timers.Timer tmr = new System.Timers.Timer(10000);
            //tmr.Elapsed += new System.Timers.ElapsedEventHandler(tmr_Elapsed);
            //tmr.Start();
          //  RMSTest();
         //   BufferTest();
            //while (true)
            //{
            //    tmr.Stop();
            //    tmr.Start();
            //    System.Threading.Thread.Sleep(1000);
            //}
           // AVITCTest();

          
             //System.DateTime dt=System.DateTime.MinValue;
             //tt( dt);


            //Protocol protocol=new Protocol();
            //protocol.Parse(System.IO.File.ReadAllText("protocol.txt"));
            //Comm.TC.VDTC vctc = new Comm.TC.VDTC(protocol, "vd226", "192.168.78.226", 1001, 0xffff,new byte[] { 0, 0, 0, 0 });



            //I_MFCC_VD robj = (I_MFCC_VD)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_VD),
            //   RemoteBuilder.getRemoteUri("192.168.22.89", (int)RemotingPortEnum.MFCC_VD1, "MFCC_VD"));
            

            //RemoteInterface.EventNotifyClient nclient = new EventNotifyClient("192.168.22.89", (int)RemoteInterface.NotifyServerPortEnum.MFCC_VD1, false);

            //nclient.OnConnect += new OnConnectEventHandler(nclient_OnConnect);

            //robj.setRealTime("VDQ-N3182-S-1", 3, 0, 2);
            //robj.setRealTime("VD-N1-S-179.9", 3, 0, 2);
            //robj.setRealTime("VD-N1-N-193.5", 3, 0, 2);
          
           // robj.setRealTime("VD-011935-N-M", 3, 0, 2);

         //   RGS_Test_Task();

            //RemoteInterface.HC.I_HC_FWIS robj_Mfcc = (RemoteInterface.HC.I_HC_FWIS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_FWIS)
            //    , RemoteInterface.RemoteBuilder.getRemoteUri("192.168.22.89", (int)RemoteInterface.RemotingPortEnum.HOST, "FWIS"));
            //robj_Mfcc.SetDbChangeNotify(RemoteInterface.HC.DbChangeNotifyConst.JamEvalTable, null);



         //   Color[] color = new Color[] { Color.Red, Color.Red, Color.Red, Color.Red };
         //   Color[] color1 = new Color[] { Color.Black, Color.Black, Color.Black, Color.Black };

         //   RemoteInterface.MFCC.RGS_Generic_Message_Data[] mess1 = new RemoteInterface.MFCC.RGS_Generic_Message_Data[1];
         //   RemoteInterface.MFCC.RGS_Generic_Message_Data meg = new RemoteInterface.MFCC.RGS_Generic_Message_Data
         //                         ("系統測試", color, color1, Convert.ToUInt16(0), Convert.ToUInt16(0));
         //   mess1[0] = meg;

         //   RemoteInterface.MFCC.RGS_Generic_ICON_Data[] icon = new RGS_Generic_ICON_Data[0]; // new RemoteInterface.MFCC.RGS_Generic_ICON_Data[1];
         //   //icon[0]=   new RemoteInterface.MFCC.RGS_Generic_ICON_Data(Convert.ToByte(1), 0, 0);
         ////   icon[0] = icontemp1;

         //   RemoteInterface.MFCC.RGS_GenericDisplay_Data Gdata = new RemoteInterface.MFCC.RGS_GenericDisplay_Data
         //   (Convert.ToByte(2), Convert.ToByte(0), new RemoteInterface.MFCC.RGS_Generic_ICON_Data[0], mess1, new RemoteInterface.MFCC.RGS_Generic_Section_Data[0]);                       //通訊協定架構

         //   while (true)
         //   {
         //       try
         //       {
         //           // if(robj_Mfcc.getConnectionStatus("RGS-N1-S-203.79","RGS-N1-S-203.79"))
         //           robj_Mfcc.RGS_setManualGenericDisplay("RGS-N3-N-198.182", Gdata,false);
         //       }
         //       catch (Exception ex)
         //       {
         //           Console.WriteLine(ex.Message);
         //       }
         //       if (Console.ReadKey().KeyChar == 'e')

         //           break;//傳送至Host
         //   }
          
       
            //RemoteInterface.MFCC.I_MFCC_WIS robj = (RemoteInterface.MFCC.I_MFCC_WIS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.MFCC.I_MFCC_WIS),
            //    RemoteInterface.RemoteBuilder.getRemoteUri("127.0.0.1", (int)RemoteInterface.RemotingPortEnum.MFCC_WIS, "MFCC_WIS"));

            //robj.SendDisplay("WIS-N6-W-30.8", 0, 0,"系統測試", new byte[] { 0x32, 0x32, 0x32, 0x32 });
            //robj.setDisplayOff("WIS-N6-W-30.8");



            ////ds = robj.getSendDSByFuncName("get_ctl_mode_and_plan_no");

            //try
            //{
            //    retds = robj.sendTC("CMS-N6-W-3.8", ds);
            //    Console.Write(retds.Tables[0].Rows[0]["plan_no"].ToString());
            //}
            //catch
            //{
            //    ;
            //}
          //  DbTest();
            WD_Test();
           // SCM_Test();
            Console.ReadKey();

        }

        static void HostNotifyTest()
        {
            RemoteInterface.HC.I_HC_FWIS robj = (RemoteInterface.HC.I_HC_FWIS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_FWIS),
              RemoteInterface.RemoteBuilder.getRemoteUri("127.0.0.1", (int)RemoteInterface.RemotingPortEnum.HOST, "FWIS"));
            robj.SetDbChangeNotify(RemoteInterface.HC.DbChangeNotifyConst.RediretRoute_Change, null);
        }
        static void WD_Test()
        {

            RemoteInterface.MFCC.I_MFCC_WD robj = (RemoteInterface.MFCC.I_MFCC_WD)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.MFCC.I_MFCC_WD),
                RemoteInterface.RemoteBuilder.getRemoteUri("10.21.50.226", (int)RemoteInterface.RemotingPortEnum.MFCC_WD2, "MFCC_WD"));
        //    return robj.getScriptSource(devtype);

            robj.getCurrentTcCommStatusStr("WD-N3-N-171.1");
            Console.ReadKey();
        }

        static void  SCM_Test()
        {
             Protocol protocol=new Protocol();
            protocol.Parse(System.IO.File.ReadAllText("SCM-20100226.txt"),false);
            
            Comm.TC.SCMTC scm=new Comm.TC.SCMTC(protocol,"SCM","10.21.50.221",1001,0xffff,new byte[]{0,0,0,0},0,0,0);
             Console.ReadKey();
           
        }

        static void CustomMakeCode()
        {
            byte[] data;
            string s = "abc\ue000dd";



            data = System.Text.UnicodeEncoding.Unicode.GetBytes(s);
            Console.WriteLine(RemoteInterface.Util.ToHexString(data));


            data = RemoteInterface.Util.StringToBig5Bytes(s);
            Console.WriteLine(RemoteInterface.Util.ToHexString(data));

            s = RemoteInterface.Util.Big5BytesToString(data);

            System.Console.WriteLine(RemoteInterface.Util.ToHexString(System.Text.UnicodeEncoding.Unicode.GetBytes(s)));
        }

       static  void tt( DateTime dt)
        {
            dt = System.DateTime.Now;
        }

        static void DbTest()
        {
            string selectCmd="";
             selectCmd += "Select * From tbldeviceconfig";
           
            selectCmd += "   where DeviceName ='" + "VD-T78-W-25.1" + "'";

            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand(selectCmd);
            cmd.Connection = cn;
            cn.Open();
            System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
            rd.Read();
           
         //   Console.WriteLine(name);
            //for (int i = 0; i < name.Length; i++)
            //    Console.Write(((int)name[i] + " ");
            
            Console.WriteLine(rd.GetString(Comm.DB2.Db2.getFiledInx(rd,"ip")));
            rd.Close();
            cn.Close();
        }

        static public void SendSqlCmd(string cmd)
        {
            sqlQueue.Enqueue(cmd);
            lock (sqlQueue)
                System.Threading.Monitor.Pulse(sqlQueue);

        }
        static  void  BufferTest()
        {
            for (int i = 0; i < 2; i++)
            {
                new System.Threading.Thread(DbSQLExecute_Task).Start();
            }
            System.Threading.Thread.Sleep(2000);
            for (int i = 0; i < 20000; i++)
                SendSqlCmd(i.ToString());

            System.Threading.Thread.Sleep(2000);
            Console.WriteLine("="+hs.Count);
        }

      static  System.Collections.Hashtable hs = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
      static  System.Collections.Queue sqlQueue = System.Collections.Queue.Synchronized(new System.Collections.Queue());
      static  void DbSQLExecute_Task()
        {
            System.Data.Odbc.OdbcConnection cn;
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();
            cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
            cmd.Connection = cn;
          //  cn.Open();

            while (true)
            {
                try
                {

                    if (sqlQueue.Count == 0)
                    {
                        lock (sqlQueue)
                            System.Threading.Monitor.Wait(sqlQueue);
                    }


                    try
                    {

                        while (sqlQueue.Count > 0)
                        {
                            try
                            {
                              
                               cmd.CommandText = sqlQueue.Dequeue().ToString();
                               hs.Add(cmd.CommandText, cmd.CommandText);
                               Console.WriteLine(cmd.CommandText);
                                //cmd.ExecuteNonQuery();

                            }
                            catch (Exception ex1)
                            {
                                ConsoleServer.WriteLine(ex1.Message);
                                try
                                {//cn.Close(); }
                                }
                                catch { ;}
                                try
                                {
                                  //  cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
                                //    cmd.Connection = cn;
                                //    cn.Open();
                                }
                                catch { ;}
                            }

                        }
                    }
                    catch (Exception ex1)
                    {
                        ConsoleServer.WriteLine(ex1.Message);
                    }



                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                }






            }
        }

        static void rgsTest()
        {
            string desc="";
                 Color[,] colors=null ;
            I_MFCC_RGS robj = (I_MFCC_RGS)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_RGS),
               RemoteBuilder.getRemoteUri("10.21.50.8", (int)RemotingPortEnum.MFCC_RGS, "MFCC_RGS"));
            try
            {
                colors = robj.getIconPic("RGS-TEST-232", 1,ref desc);
               Console.WriteLine(colors[0,0].G);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            

          //  Comm.Protocol protocol  = new Protocol();
          //  protocol.Parse(System.IO.File.ReadAllText("RGS.txt"), false);
          //  Comm.TC.RGSTC tc = new Comm.TC.RGSTC(protocol, "RGS232", "192.168.22.232", 4660, 0xffff, new byte[] { 0, 0, 0, 0 }, 0, 0);
          //  string desc="";
          //  Console.ReadKey();
          //Color[][]colors=  Util.BitMapToColors(tc.TC_GetIconPic(1, ref desc));
        }
        static void vdtest()
        {
            //    Comm.Protocol protocol=new Protocol();
            //    protocol.Parse(System.IO.File.ReadAllText("protocol.txt"),false);
            //    Comm.TC.VDTC tc = new Comm.TC.VDTC(protocol,"vdtest" ,"10.21.30.183", 1001,0xffff,new byte[]{0,0,0,0},0,0);
            //    tc.OnConnectStatusChanged += new ConnectStatusChangeHandler(tc_OnConnectStatusChanged);


            Color[,] colors = null;
            I_MFCC_VD robj = (I_MFCC_VD)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_VD),
               RemoteBuilder.getRemoteUri("127.0.0.1", (int)RemotingPortEnum.MFCC_VD2, "MFCC_VD"));
           // string str = robj.getCurrentTcCommStatusStr("VD-N6-E-35.3");
            robj.downLoadConfigParam("VD-N6-W-28.4");
           // Console.WriteLine(str);
            Console.ReadKey();
        }

        static void tc_OnConnectStatusChanged(object tc)
        {
           // throw new Exception("The method or operation is not implemented.");
            Console.WriteLine(tc.ToString());
        }
        static void nclient_OnConnect(object sender)
        {
            //throw new Exception("The method or operation is not implemented.");
            ((RemoteInterface.EventNotifyClient)sender).OnEvent += new NotifyEventHandler(Program_OnEvent);
            ((RemoteInterface.EventNotifyClient)sender).RegistEvent(new NotifyEventObject(EventEnumType.VD_Real_Data_Event, "*", null));
        }

        static void Program_OnEvent(object sender, NotifyEventObject objEvent)
        {
           // throw new Exception("The method or operation is not implemented.");
            Console.WriteLine(objEvent.EventObj.ToString());
        }

        static string getProtocolStr(string devtype)
        {
             RemoteInterface.HC.I_HC_Comm robj=  (RemoteInterface.HC.I_HC_Comm) RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_Comm),
                 RemoteInterface.RemoteBuilder.getRemoteUri("10.21.50.4",(int)RemoteInterface.RemotingPortEnum.HOST,"Comm"));
             return robj.getScriptSource(devtype);
        }

        static void RMSTCTest()
        {
            
            Protocol protocol=new Protocol();
            protocol.Parse(getProtocolStr("RMS"));
          //  Comm.TC.RMSTC tc = new Comm.TC.RMSTC(protocol, "test", "10.21.63.130", 1001, 0xffff, new byte[] { 0, 0, 0, 0 }, 0, 0);
            
        }

        static void AVITCTest()
        {

            Protocol protocol = new Protocol();
            protocol.Parse(getProtocolStr("AVI"));
          //  Comm.TC.AVITC tc = new Comm.TC.AVITC(protocol, "test", "192.168.90.227", 1001, 0xffff, new byte[] { 0, 0, 0, 0 }, 0, 0);

        }
        static void RMSTest()
        {
            DataSet retds;
            RemoteInterface.MFCC.I_MFCC_RMS robj = (RemoteInterface.MFCC.I_MFCC_RMS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.MFCC.I_MFCC_RMS),
RemoteInterface.RemoteBuilder.getRemoteUri("10.21.50.8", (int)RemoteInterface.RemotingPortEnum.MFCC_RMS, "MFCC_RMS"));
            // robj.getSendDSByFuncName();
            System.Data.DataSet ds = robj.getSendDSByFuncName("set_ctl_mode_and_plan_no");
            ds.Tables[0].Rows[0]["ctl_mode"] = 0;
            ds.Tables[0].Rows[0]["plan_no"] = 30;

            ds.AcceptChanges();

            robj.sendTC("RMSTC-N3-260-S-1", ds);
            System.Console.ReadKey();
            ds.Tables[0].Rows[0]["ctl_mode"] = 0;
            ds.Tables[0].Rows[0]["plan_no"] = 14;

            ds.AcceptChanges();

            robj.sendTC("RMSTC-N3-260-S-1", ds);

            

             // retds  =   robj.sendTC("RMSTC-T76-19-W-1", ds);
             //System.Data.DataRow r = retds.Tables[0].Rows[0];
             //for (int i = 0; i < retds.Tables[0].Columns.Count; i++)
             //{
             //    Console.WriteLine(retds.Tables[0].Columns[i].ColumnName + "=" + r[i]);
             //}
              
              

           //  r["sgmtyp"] = 1;//設定特定日型態數

           //  r["sgmcnt"] = 1;//設定時段分段數

           //  r["yy"] = 2009;//設定年

           //  r["mo"] = 10;//設定月

           //  r["dd"] = 6;//設定日

           //  r["scnt"] = 1;//設定分段總數



           //  r = ds.Tables[1].NewRow();



           //  r["start_hour"] = 00; ;//設定開始時

           //  r["start_min"] = 00;//設定開始分

           //  r["end_hour"] = 23;//設定開始時

           //  r["end_min"] = 59; //設定開始分

           //  r["plan_no"] = 30;//設定時制編號

           //  ds.Tables[1].Rows.Add(r);
           //  ds.AcceptChanges();

           ////  DataSet retds = robj_Mfcc.sendTC(DevName, ds);
           //  ds.AcceptChanges();

       //      DataSet retds = robj.sendTC("RMS-TEST-235", ds);




        }

        static void tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("time out!");
          //  throw new Exception("The method or operation is not implemented.");
        }
        public static  void RGS_Test_Task()
        {
            Protocol protocol=new Protocol();
            protocol.Parse(System.IO.File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory+"protocol.txt"));
            Comm.TC.RGSTC tc = new Comm.TC.RGSTC(protocol, "RGSTest", "59.120.20.153", 4660,0xffff, new byte[] { 0, 0, 0, 0 },0,0,0);
            // Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.manager["RGS232"];

            while (!tc.IsConnected) ;
            tc.m_device.OnAck += new OnAckEventHandler(m_device_OnAck);
            tc.m_device.OnSendingPackage += new OnSendPackgaeHandler(m_device_OnSendingPackage);
            tc.m_device.OnReceiveText += new OnTextPackageEventHandler(m_device_OnReceiveText);
            while (true)
            {

                System.Drawing.Color[][] color;
                color = new System.Drawing.Color[1][];
                color[0] = new System.Drawing.Color[4] { System.Drawing.Color.Red, System.Drawing.Color.Red, System.Drawing.Color.Red, System.Drawing.Color.Red };
                try
                {
                    if (tc.IsConnected)
                        tc.SetTravelDisplay(new byte[0], new string[] { "系統測試" }, color);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }

        //void m_device_OnReceiveText(object sender, TextPackage txtObj)
        //{
        //    // Console.WriteLine(txtObj.ToString());
        //    //throw new Exception("The method or operation is not implemented.");
        //}

        //void m_device_OnSendingPackage(object sender, SendPackage pkg)
        //{
        //    Console.WriteLine("===>" + pkg.ToString());
        //    //throw new Exception("The method or operation is not implemented.");
        //}

        //void m_device_OnAck(object sender, AckPackage AckObj)
        //{
        //    //throw new Exception("The method or operation is not implemented.");
        //    Console.WriteLine(AckObj);
        //}






        static void m_device_OnSendingPackage(object sender, Comm.SendPackage pkg)
        {
            //throw new Exception("The method or operation is not implemented.");
            Console.WriteLine("sending ==>"+pkg);
        }

        static void m_device_OnAck(object sender, Comm.AckPackage AckObj)
        {
           // throw new Exception("The method or operation is not implemented.");
            Console.WriteLine("\t"+AckObj);

        }

        static void m_device_OnBeforeAck(object sender, ref byte[] data)
        {
           byte[]ackdata=new byte[data.Length-2];
           System.Array.Copy(data, 2, ackdata,0, data.Length -2);

            Console.WriteLine( new Comm.AckPackage(ackdata).ToString());

          //  throw new Exception("The method or operation is not implemented.");
        }

        static void m_device_OnReceiveText(object sender, Comm.TextPackage txtObj)
        {
            Console.WriteLine(txtObj.ToString());
            //throw new Exception("The method or operation is not implemented.");
        }

        static void m_device_OnTextSending(object sender, ref byte[] data)
        {
            Console.WriteLine("sendng ==>["+Comm.V2DLE.ToHexString(data)+"]");
            //throw new Exception("The method or operation is not implemented.");
        }

        public void VD_Test()
        {
            string src = System.IO.File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "protocol.txt");
            Comm.Protocol protocol = new Comm.Protocol();
            protocol.Parse(src);
            Comm.TC.VDTC vdtc = new Comm.TC.VDTC(protocol, "vd231", "192.168.22.231", 1001, 0xffff, new byte[] { 0, 0, 0, 0 },0,0,0);
            while (!vdtc.IsConnected)
                System.Threading.Thread.Sleep(1000);


             vdtc.m_device.OnTextSending += new Comm.OnSendingAckNakHandler(m_device_OnTextSending);
            vdtc.m_device.OnSendingPackage += new Comm.OnSendPackgaeHandler(m_device_OnSendingPackage);
            vdtc.m_device.OnReceiveText += new Comm.OnTextPackageEventHandler(m_device_OnReceiveText);
            vdtc.m_device.OnBeforeAck += new Comm.OnSendingAckNakHandler(m_device_OnBeforeAck);
            vdtc.m_device.OnAck += new Comm.OnAckEventHandler(m_device_OnAck);
            //  vdtc.m_device.

            while (true)
            {
                Console.ReadKey();
                vdtc.SetRealData(3, 0, 1);
                Console.ReadKey();
                vdtc.SetRealData(0, 0, 1);
            }

        }
    }
}
