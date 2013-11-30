using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.MFCC;
using Comm;
using RemoteInterface.TEM;
namespace MFCC_TEM
{


      public class LCSDeviceData
      {

       //   public int status=0;
          public string deviceName;
          public  int tunnid, place, div;
          public  LCSDeviceData(string devName,int tunnid,int place,int div)
          {
              this.deviceName = devName;
              this.tunnid = tunnid;
              this.place = place;
              this.div = div;
          }


      }
    public class MFCC_TEM : Comm.MFCC.MFCC_DataColloetBase
    {
        /*
         * VD-N6-E-18.0
 
國姓一號西向
 VD-N6-W-17.9
 
國姓二號東向
 VD-N6-E-24.7
 
國姓二號西向
 VD-N6-W-24.7
 
埔里東向
 VD-N6-E-27.8
 
埔里西向
 VD-N6-W-27.5
         * 
         * */

        RemoteInterface.MFCC.I_MFCC_VD roj_vd2;
        RemoteInterface.MFCC.I_MFCC_VI roj_vi;
        RemoteInterface.HC.I_HC_Comm rhost;
        RemoteInterface.ExactIntervalTimer tmr1min;
      //  System.Timers.Timer tmr1min = new System.Timers.Timer(1000 * 60);
        string[] vd_devNames ;
        int[] vd_dirs;
        int[] vd_mile_ks;
        int[] vd_mile_ms;

        string[] vi_devNames = new string[2];
        int[] vi_dirs = new int[2];
        int[] vi_mile_ks = new int[2];
        int[] vi_mile_ms = new int[2];


        System.Collections.Hashtable hsFireAlarmData = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
        System.Collections.Hashtable hsPowerAlarmData = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
        System.Collections.Hashtable hsSecurityAlarmData = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
        System.Collections.Hashtable hsLightAlarmData = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
        System.Collections.Hashtable hsAirAlarmData = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
        System.Collections.Hashtable hsMonitorAlarmData = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());

        System.Collections.Hashtable hsLCSDeviceData = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
     // new   Protocol protocol;

        //RemoteInterface.EventNotifyClient nclient;

        string  TEM_DEVNAME="E602_TEM";
     
      public MFCC_TEM(string mfccid, string devType, int remotePort, int notifyPort, int consolePort, string regRemoteName, Type regRemoteType)
          : base(mfccid, devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
        {
            initArray();

            loadVDData();
            loadVIData();
            loadLCSData();
         
            new System.Threading.Thread(MFCCVD2_ConnectTask).Start();
            new System.Threading.Thread(MFCCVI_ConnectTask).Start();
            
          
          tmr1min= new ExactIntervalTimer(10);
          tmr1min.OnElapsed+=new OnConnectEventHandler(tmr1min_OnElapsed);
       //  new System.Threading.Thread(this.TEMTest).Start();
         // nclient = new EventNotifyClient();
           
        
        }

   

        public void loadLCSData()
        {
            string sql = "select t1.devicename as devicename,tunnel,place,div from tblLCSConfig  t2 inner join tbldeviceconfig t1 on t1.devicename=t2.devicename  where location='T' and lineid='N6' ";

            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(DbCmdServer.getDbConnectStr());
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand(sql);
            System.Data.Odbc.OdbcDataReader rd;
            try
            {
                cn.Open();
                cmd.Connection = cn;
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    LCSDeviceData devcdata;
                    string devicename=rd[0].ToString();
                    int tunnid=System.Convert.ToInt32(rd[1]);
                    int place=System.Convert.ToInt32(rd[2]);
                    int div=System.Convert.ToInt32(rd[3]);
                    devcdata = new LCSDeviceData(devicename, tunnid, place, div);
                    this.hsLCSDeviceData.Add(devicename, devcdata);
                    ConsoleServer.WriteLine("loadoing " + devicename);


                   
                }
                rd.Close();
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }
            finally
            {
               
                cn.Close();
            }

        }
      public     void setLCSStatus(string devName, int status)
        {

            if (!hsLCSDeviceData.Contains(devName))
                throw new Exception(this.mfccid + " Can't find " + devName);

             sendTemLCSStatus(devName, status);
              
            // notify tem here


        }
        void TEMTest()
        {
            Console.ReadKey();
            RemoteInterface.TEM.SecurityAlarmData sd = new SecurityAlarmData(this.TEM_DEVNAME, DateTime.Now, "1", 1, 1, 1);
            (this.hsSecurityAlarmData[sd.Key] as RemoteInterface.TEM.SecurityAlarmData).setStatus(1);
          //  this.data_OnEvent(sd);

            Console.ReadKey();
            (this.hsSecurityAlarmData[sd.Key] as RemoteInterface.TEM.SecurityAlarmData).setStatus(0);
          //  this.data_OnEventStop(sd);
        }

void  tmr1min_OnElapsed(object sender)
{
 	  try
            {
                if (System.DateTime.Now.Minute % 5 == 0)
                {
                    try
                    {
                        doVDTask();
                      
                    }
                    catch { ;}
                    try
                    {
                        doLCSTask();
                    }
                    catch { ;}
                }
               


               
            }
            catch { ;}
            try
            {
                doVITask();
            }
            catch { ;}

          
}



        void doLCSTask()
        {
            foreach (LCSDeviceData data in hsLCSDeviceData.Values)
            {

                try
                {
                   int status= this.r_host_comm.getTEMLCSStatus(data.deviceName);
                   sendTemLCSStatus(data.deviceName, status);
                    //do notify job to tem
                }
                catch(Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                }

              

            }



        }


        void sendTemLCSStatus(string devName, int status)
        {

            byte[] data=new byte[]{0x26,0,0,0,0};
            LCSDeviceData lcs = hsLCSDeviceData[devName] as LCSDeviceData;
            data[1] = (byte)lcs.tunnid;
            data[2] =(byte) lcs.place;
            data[3] =(byte) lcs.div;
            data[4] = (byte)status;
            SendPackage pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, data);
            Program.mfcc_tem.manager[TEM_DEVNAME].Send(pkg);
           

        }

        void initArray()
        {
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(RemoteInterface.DbCmdServer.getDbConnectStr());
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select count(*) from tbldeviceConfig where location='T' and lineid='N6' and device_type='VD' ");
            try
            {
                cmd.Connection = cn;
                cn.Open();
                int count = System.Convert.ToInt32(cmd.ExecuteScalar());
                 vd_devNames = new string[count];
                vd_dirs = new int[count];
                vd_mile_ks = new int[count];
                vd_mile_ms = new int[count];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
            finally
            {
                cn.Close();
            }
           
        }


        void MFCCVI_ConnectTask()
        {

            while (true)
            {
                try
                {
                    ConsoleServer.WriteLine("vi reconnecting..!");
                    if (roj_vi == null)

                        roj_vi = (I_MFCC_VI)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(I_MFCC_VI), RemoteBuilder.getRemoteUri(RemoteBuilder.getMFCC_IP("MFCC_VI"), (int)RemotingPortEnum.MFCC_VI, "MFCC_VI"));
                    else
                    {
                        ConsoleServer.WriteLine("vi reconnected!");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message);
                }

                System.Threading.Thread.Sleep(1000 * 5);
            }
        }
        void MFCCVD2_ConnectTask()
        {

            while (true)
            {
                try
                {
                    ConsoleServer.WriteLine("vd2 reconnecting..!");
                    if (roj_vd2 == null)

                        roj_vd2 = (I_MFCC_VD)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(I_MFCC_VD), RemoteBuilder.getRemoteUri(RemoteBuilder.getMFCC_IP("MFCC_VD2"), (int)RemotingPortEnum.MFCC_VD2, "MFCC_VD"));
                    else
                    {
                        ConsoleServer.WriteLine("vd2 reconnected!");
                        return;
                    }
                }
                catch(Exception ex) {
                    ConsoleServer.WriteLine(ex.Message);}

                System.Threading.Thread.Sleep(1000 * 5);
            }
        }


        public void loadVIData()
        {
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(RemoteInterface.DbCmdServer.getDbConnectStr());
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select devicename,direction,mile_m from tbldeviceConfig where devicename in ('VI-N6-W-37.2','VI-N6-E-34.6')");

            try
            {
                cmd.Connection = cn;
                cn.Open();
                System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                int inx = 0;
                while (rd.Read())
                {

                    vi_devNames[inx] = rd[0].ToString();

                    if (rd[1].ToString() == "E")
                        vi_dirs[inx] = 0;
                    else if (rd[1].ToString() == "W")
                        vi_dirs[inx] = 1;
                    else if (rd[1].ToString() == "S")
                        vi_dirs[inx] = 2;

                    else if (rd[1].ToString() == "N")
                        vi_dirs[inx] = 3;


                    vi_mile_ks[inx] = System.Convert.ToInt32(rd[2]) / 1000;

                    vi_mile_ms[inx] = System.Convert.ToInt32(rd[2]) % 1000;
                    inx++;
                }
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }
            finally
            {
                cn.Close();
            }

        }


        public void loadVDData()
        {
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(RemoteInterface.DbCmdServer.getDbConnectStr());
         System.Data.Odbc.OdbcCommand cmd;

            try{
              
                cn.Open();
              
             
                cmd  = new System.Data.Odbc.OdbcCommand("select devicename,direction,mile_m from tbldeviceConfig where location='T' and lineid='N6' and device_type='VD'");
                cmd.Connection = cn;
                System.Data.Odbc.OdbcDataReader rd= cmd.ExecuteReader();
                int inx=0;
                while(rd.Read())
                {
                   
                     vd_devNames[inx]=rd[0].ToString();

                    if(rd[1].ToString()=="E")
                     vd_dirs[inx]=0;
                    else if(rd[1].ToString()=="W")
                        vd_dirs[inx]=1;
                     else if(rd[1].ToString()=="S")
                        vd_dirs[inx]=2;

                     else if(rd[1].ToString()=="N")
                        vd_dirs[inx]=3;


                    vd_mile_ks[inx]=System.Convert.ToInt32(rd[2])/1000;

                      vd_mile_ms[inx]=System.Convert.ToInt32(rd[2])%1000;
                    inx++;
                }
              }
            catch(Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message+","+ex.StackTrace);
            }
            finally
            {
                cn.Close();
            }
        }
    

        void doVDTask()
        {
              if (roj_vd2 == null)
                return;

            for(int i=0;i<vd_devNames.Length;i++)
            {
                try
                {

                    VD1MinCycleEventData data;
                    if (!roj_vd2.getConnectionStatus(vd_devNames[i]))
                        continue;

                    data = roj_vd2.getVDLatest1MinData(vd_devNames[i]);
                    SetTemVDData(vd_devNames[i], vd_dirs[i], vd_mile_ks[i], vd_mile_ms[i], data.orgds);
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(vd_devNames[i]+","+ex.Message + "," + ex.StackTrace);
                }
            }
        }
        void doVITask()
        {
            DateTime dt = new DateTime() ;
            int distance=0, degree=0;
            if (roj_vi == null)
                return;
            for (int i = 0; i < vi_devNames.Length; i++)
            {
                try
                {
                    if (roj_vi.getConnectionStatus(vi_devNames[i]))
                    {
                        roj_vi.getCurrentVIData(vi_devNames[i], ref dt, ref distance, ref degree);
                        System.Data.DataSet ds = this.protocol.GetSendDataSet("send_visibility_event");
                        System.Data.DataRow r = ds.Tables[0].Rows[0];

                        r["dir"] = vi_dirs[i];
                        r["mile_k"] = vi_mile_ks[i];
                        r["mile_m"] = vi_mile_ms[i];
                        r["status"] = (degree > 0) ? 1 : 0;

                        Comm.SendPackage pkg = this.protocol.GetSendPackage(ds, 0xffff);
                        Program.mfcc_tem.manager[TEM_DEVNAME].Send(pkg);

                    }
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message+","+ex.StackTrace);
                }
            }

        }

        void SetTemVDData(string devname, int dir, int milek, int milem, System.Data.DataSet ds)
        {
            System.Text.StringBuilder sql = new StringBuilder();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ms.WriteByte(0x21);
            ms.WriteByte((byte)dir);
            ms.WriteByte((byte)(milek/256));
            ms.WriteByte((byte)(milek % 256));
            ms.WriteByte((byte)((milem ) / 256));
            ms.WriteByte((byte)((milem ) % 256));
            ms.WriteByte((byte)ds.Tables[1].Rows.Count); //lanecnt
            ms.WriteByte(0); //odd
            System.DateTime dt= DateTime.Now;
            dt=dt.AddSeconds(-dt.Second).AddMinutes(-5);
            dt=dt.AddMinutes(-(dt.Minute % 5));
            //sql.Append("select ");
            //for(int i=0;i<ds.Tables[1].Rows.Count;i++)
            //{
               
            //}
            
           // string sql = "select small_car_volume_lane1,big_car_volume,connect_car_volume,small_car_speed,big_car_speed,connect_car_speed,average_occupancy from tblvddata5min where devicename='{0}' and timestamp='{1}' ";
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(RemoteInterface.DbCmdServer.getDbConnectStr());
            System.Data.Odbc.OdbcCommand cmd=new System.Data.Odbc.OdbcCommand();
            cmd.Connection = cn;

            try
            {

                cn.Open();

                for (int i = 0; i < (byte)ds.Tables[1].Rows.Count; i++)
                {
                    sql = new StringBuilder("select ");
                    sql.Append("small_car_volume_lane").Append(i + 1).Append(",");
                    sql.Append("big_car_volume_lane").Append(i + 1).Append(",");
                    sql.Append("connect_car_volume_lane").Append(i + 1).Append(",");

                    sql.Append("small_car_speed_lane").Append(i + 1).Append(",");
                    sql.Append("big_car_speed_lane").Append(i + 1).Append(",");
                    sql.Append("connect_car_speed_lane").Append(i + 1).Append(",");
                    sql.Append("average_occupancy_lane").Append(i + 1).Append(",");

                    sql.Remove(sql.ToString().Length - 1, 1).Append(" from tblvddata5min   where devicename='{0}' and timestamp='{1}'");
                    cmd.CommandText = string.Format(sql.ToString(), devname, RemoteInterface.DbCmdServer.getTimeStampString(dt));

                    System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                    if (!rd.Read())
                    {
                        rd.Close();
                        continue;
                    }

                 //   System.Data.DataRow r = ds.Tables[1].Rows[i];

                    ms.WriteByte((byte)i); //loopid
                    ms.WriteByte(0); //odd
                    int car_vol = System.Convert.ToInt32(rd[0]);
                    ms.WriteByte((byte)(car_vol / 256));
                    ms.WriteByte((byte)(car_vol % 256));
                    int big_vol = System.Convert.ToInt32(rd[1]);
                    ms.WriteByte((byte)(big_vol / 256));
                    ms.WriteByte((byte)(big_vol % 256));
                    int cn_vol = System.Convert.ToInt32(rd[2]);
                    ms.WriteByte((byte)(cn_vol / 256));
                    ms.WriteByte((byte)(cn_vol % 256));
                    int carspd = System.Convert.ToByte(rd[3]);
                    ms.WriteByte((byte)carspd);
                    carspd = System.Convert.ToByte(rd[4]);
                    ms.WriteByte((byte)carspd);

                    carspd = System.Convert.ToByte(rd[5]);
                    ms.WriteByte((byte)carspd);
                    ms.WriteByte(System.Convert.ToByte(rd[6]));

                    rd.Close();

                }
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }
            finally
            {
                cn.Close();
            }
         

            Comm.SendPackage pkg=new Comm.SendPackage(Comm.CmdType.CmdSet,Comm.CmdClass.A,0xffff,ms.ToArray());
            Program.mfcc_tem.manager[TEM_DEVNAME].Send(pkg);
            

        }

        public override void BindEvent(object tc)
        {
          //  throw new Exception("The method or operation is not implemented.");
            ((Comm.TCBase)tc).OnTCReport += new Comm.OnTCReportHandler(MFCC_TEM_OnTCReport);
        }

        void MFCC_TEM_OnTCReport(object tcobj, Comm.TextPackage txt)
        {
            //throw new Exception("The method or operation is not implemented.");

            if (txt.Text[0] == 0x0b || txt.Text[0] == 0x01)
                return;

            TCBase tc = tcobj as TCBase;
             
            System.Data.DataSet ds = null;
            try
            {
              
               
                ds=protocol.GetSendDsByTextPackage(txt, CmdType.CmdReport);

                System.Data.DataRow r = ds.Tables[0].Rows[0];
              //  string key;
                switch (txt.Text[0])
                {

                    case 0x10:  //filealarm

                     //   key = r["tunnel"].ToString() + "-" + r["place"] + "-" + r["div"];
                        RemoteInterface.TEM.FireAlarmData fdata = new RemoteInterface.TEM.FireAlarmData(tc.DeviceName, System.DateTime.Now,
                             r["tunnel"].ToString(), Convert.ToInt32(r["place"]), Convert.ToInt32(r["div"]), Convert.ToInt32(r["status"]));
                        if (!hsFireAlarmData.Contains(fdata.Key))
                        {

                            fdata.OnEvent += new RemoteInterface.TEM.OnEventHandler(data_OnEvent);
                            fdata.OnEventStop += new RemoteInterface.TEM.OnEventHandler(data_OnEventStop);
                            hsFireAlarmData.Add(fdata.Key, fdata);
                            ConsoleServer.WriteLine("新增火警分區:" + fdata);
                            //  this.r_host_comm.setTemEvent(tc.DeviceName, data);

                        }
                        else
                        {
                            (hsFireAlarmData[fdata.Key] as FireAlarmData).setStatus(Convert.ToInt32(r["status"]));
                            ConsoleServer.WriteLine("火警分區:" + fdata);
                        }


                         
                        break;

                    case 0x13:   //power
                       // key = r["tunnel"].ToString() + "-" + r["place"];
                        RemoteInterface.TEM.PowerAlarmData pdata = new RemoteInterface.TEM.PowerAlarmData(tc.DeviceName, System.DateTime.Now,
                              r["tunnel"].ToString(), Convert.ToInt32(r["place"]), Convert.ToInt32(r["status"]));
                        if (!hsPowerAlarmData.Contains(pdata.Key))
                        {

                            pdata.OnEvent += new RemoteInterface.TEM.OnEventHandler(data_OnEvent);
                            pdata.OnEventStop += new RemoteInterface.TEM.OnEventHandler(data_OnEventStop);
                            hsPowerAlarmData.Add(pdata.Key, pdata);
                            ConsoleServer.WriteLine("新增配電分區:" + pdata);
                            //   this.r_host_comm.setTemEvent(tc.DeviceName, data);

                        }
                        else
                        {
                            (hsPowerAlarmData[pdata.Key] as PowerAlarmData).setStatus(Convert.ToInt32(r["status"]));
                            ConsoleServer.WriteLine("配電:" + pdata);
                        }



                        break;
                    case 0x14:  //security
                       // key = r["tunnel"].ToString() + "-" + r["place"]+"-"+r["card_reader"];\
                        RemoteInterface.TEM.SecurityAlarmData sdata = new RemoteInterface.TEM.SecurityAlarmData(tc.DeviceName, System.DateTime.Now,
                              r["tunnel"].ToString(), Convert.ToInt32(r["place"]), Convert.ToInt32(r["card_reader"]), Convert.ToInt32(r["status"]));
                        if (!hsSecurityAlarmData.Contains(sdata.Key))
                        {

                            sdata.OnEvent += new RemoteInterface.TEM.OnEventHandler(data_OnEvent);
                            sdata.OnEventStop += new RemoteInterface.TEM.OnEventHandler(data_OnEventStop);
                            hsSecurityAlarmData.Add(sdata.Key, sdata);
                            ConsoleServer.WriteLine("新增門禁分區:" + sdata);
                            // this.r_host_comm.setTemEvent(tc.DeviceName, data);

                        }
                        else
                        {
                            (hsSecurityAlarmData[sdata.Key] as SecurityAlarmData).setStatus(Convert.ToInt32(r["status"]));
                            ConsoleServer.WriteLine("門禁:" + sdata);
                        }



                        break;

                    case 0x15:

                        RemoteInterface.TEM.MonitorAlarmData mdata = new RemoteInterface.TEM.MonitorAlarmData(tc.DeviceName, System.DateTime.Now,
                                                    r["tunnel"].ToString(), Convert.ToInt32(r["place"]), Convert.ToInt32(r["location"]), Convert.ToInt32(r["status"]));
                        if (!hsMonitorAlarmData.Contains(mdata.Key))
                        {

                            mdata.OnEvent += new RemoteInterface.TEM.OnEventHandler(data_OnEvent);
                            mdata.OnEventStop += new RemoteInterface.TEM.OnEventHandler(data_OnEventStop);
                            hsMonitorAlarmData.Add(mdata.Key, mdata);
                            ConsoleServer.WriteLine("新增監控分區:" + mdata);
                            // this.r_host_comm.setTemEvent(tc.DeviceName, data);

                        }
                        else
                        {
                            (hsMonitorAlarmData[mdata.Key] as MonitorAlarmData).setStatus(Convert.ToInt32(r["status"]));
                            ConsoleServer.WriteLine("監控:" + mdata);
                        }





                        break;

                    case 0x12:  //light
                       // key = r["tunnel"].ToString() + "-" + r["place"] + "-" + r["div"];
                        RemoteInterface.TEM.LightAlarmData ldata = new RemoteInterface.TEM.LightAlarmData(tc.DeviceName, System.DateTime.Now,
                              r["tunnel"].ToString(), Convert.ToInt32(r["place"]), Convert.ToInt32(r["div"]), Convert.ToInt32(r["required"]), Convert.ToInt32(r["damaged"]));
                        if (!hsLightAlarmData.Contains(ldata.Key))
                        {

                            ldata.OnEvent += new RemoteInterface.TEM.OnEventHandler(data_OnEvent);
                            ldata.OnEventStop += new RemoteInterface.TEM.OnEventHandler(data_OnEventStop);
                            hsLightAlarmData.Add(ldata.Key, ldata);
                            ConsoleServer.WriteLine("新增照明分區:" + ldata);
                            //  this.r_host_comm.setTemEvent(tc.DeviceName, data);
                        }
                        else
                        {
                            (hsLightAlarmData[ldata.Key] as LightAlarmData).setStatus(Convert.ToInt32(r["required"]), Convert.ToInt32(r["damaged"]));
                            ConsoleServer.WriteLine("照明:" + ldata);
                        }



                        break;

                    case 0x11:  //air 


                        AirAlarmData vidata, codata, nodata, no2data, noxdata ;

                       

                        int tunnel = System.Convert.ToInt32(r["tunnel"]);
                        int place = System.Convert.ToInt32(r["place"]);
                        int mile_k = System.Convert.ToInt32(r["mile_k"]);
                        int mile_m = System.Convert.ToInt32(r["mile_m"]);
                        vidata = new AirAlarmData(tc.DeviceName, tunnel, place, mile_k, mile_m, "VI"
                            , System.Convert.ToInt32(r["vi_density_2"]), System.Convert.ToInt32(r["vi_odd_2"]), System.Convert.ToInt32(r["vi_level_2"]));
                        codata = new AirAlarmData(tc.DeviceName, tunnel, place, mile_k, mile_m, "CO"
                           , System.Convert.ToInt32(r["co_density_1"]), System.Convert.ToInt32(r["co_odd_1"]), System.Convert.ToInt32(r["co_level_1"]));
                        nodata = new AirAlarmData(tc.DeviceName, tunnel, place, mile_k, mile_m, "NO"
                          , System.Convert.ToInt32(r["no_density_3"]), System.Convert.ToInt32(r["no_odd_3"]), System.Convert.ToInt32(r["no_level_3"]));
                        no2data = new AirAlarmData(tc.DeviceName, tunnel, place, mile_k, mile_m, "NO2"
                         , System.Convert.ToInt32(r["no2_density_4"]), System.Convert.ToInt32(r["no2_odd_4"]), System.Convert.ToInt32(r["no2_level_4"]));

                        noxdata = new AirAlarmData(tc.DeviceName, tunnel, place, mile_k, mile_m, "NOX"
                        , System.Convert.ToInt32(r["nox_density_5"]), System.Convert.ToInt32(r["nox_odd_5"]), System.Convert.ToInt32(r["nox_level_5"]));

                        AirAlarmData[] airDatas = new AirAlarmData[] { vidata, codata, nodata, no2data, noxdata };
                        foreach (AirAlarmData a in airDatas)
                        {
                            if (!hsAirAlarmData.Contains(a.Key))
                            {
                                hsAirAlarmData.Add(a.Key, a);
                                a.OnEvent += new OnEventHandler(data_OnEvent);
                                a.OnEventStop += new OnEventHandler(data_OnEventStop);
                                ConsoleServer.WriteLine("新增空氣品質分區:" + a);
                            }
                            else
                            {
                                (hsAirAlarmData[a.Key] as AirAlarmData).setData(a.density, a.level);
                                ConsoleServer.WriteLine("空氣品質:" + a);
                            }

                        }
                         //for(int i=0;i < ds.Tables[0].Columns.Count;i++)
                         //{
                         //    ConsoleServer.Write(ds.Tables[0].Columns[i].ColumnName + ":" + r[i].ToString()+" ");

                         //}
                         //ConsoleServer.WriteLine("");

                        break;

                }


               


               


            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
                ConsoleServer.WriteLine(txt.ToString());
            }
           
          //  ConsoleServer.WriteLine(txt.ToString());
           
            //try
            //{
            //    string result="";
            //    string sql = "insert into tblTEMStateLog (devicename,timestamp,lane_id,cam_id,event_id,action_type) values('{0}','{1}',{2},{3},{4},{5})";
            //    System.Data.DataSet ds = this.protocol.GetSendDsByTextPackage(txt,Comm.CmdType.CmdReport);
            //    int year,mon,day,hr,min,sec;
            //    System.Data.DataRow r=ds.Tables[0].Rows[0];
            //    year=System.Convert.ToInt32(r["year"]);
            //    mon=System.Convert.ToInt32(r["month"]);
            //    day=System.Convert.ToInt32(r["day"]);
            //    hr=System.Convert.ToInt32(r["hour"]);
            //    min=System.Convert.ToInt32(r["minute"]);
            //    sec=System.Convert.ToInt32(r["second"]);

            //    DateTime dt=new DateTime(year,mon,day,hr,min,sec);
                
            //    for(int i =0;i<ds.Tables["tblevent_lane_count"].Rows.Count;i++)
            //    {
            //        string dbcmd=string.Format(sql, (tc as Comm.TCBase).DeviceName,Comm.DB2.Db2.getTimeStampString(dt),System.Convert.ToInt32(ds.Tables["tblevent_lane_count"].Rows[i]["lane_id"]),
            //            System.Convert.ToUInt32(r["cam_id"]),System.Convert.ToInt32(r["event_id"]),System.Convert.ToInt32(r["action_type"]));
            //        dbServer.SendSqlCmd(dbcmd);
            //    }
            //    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            //    {
            //       result+=ds.Tables[0].Columns[i].ColumnName+":"+ds.Tables[0].Rows[0][i]+",";
            //    }
            //    ConsoleServer.WriteLine(result.TrimEnd(new char[]{','}));
            //}
            //catch (Exception ex)
            //{
            //    ConsoleServer.WriteLine(txt.ToString());
            //    ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            //}

        }

        void data_OnEventStop(object sender)
        {
            try
            {
                if (sender is FireAlarmData)
                    ConsoleServer.WriteLine("火警事件結束," + sender);
                else if (sender is PowerAlarmData)
                    ConsoleServer.WriteLine("配電事件結束," + sender);
                else if (sender is SecurityAlarmData)
                    ConsoleServer.WriteLine("門禁事件結束," + sender);
                else if (sender is LightAlarmData)
                    ConsoleServer.WriteLine("照明事件結束," + sender);
                else if (sender is AirAlarmData)
                    ConsoleServer.WriteLine("空氣品質事件結束" + sender);

                sendHostEvent(TEM_DEVNAME, sender);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }

          //  throw new Exception("The method or operation is not implemented.");
        }

        void sendHostEvent(string devName,object eventdata)
        {
            if (rhost == null)
            {
                rhost = (RemoteInterface.HC.I_HC_Comm)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_Comm), RemoteInterface.RemoteBuilder.getRemoteUri(RemoteInterface.RemoteBuilder.getHostIP(), (int)RemoteInterface.RemotingPortEnum.HOST, "Comm"));
            }

            rhost.setTemEvent(devName, eventdata);

        }

        void data_OnEvent(object sender)
        {
            try
            {
                if (sender is FireAlarmData)
                    ConsoleServer.WriteLine("火警事件," + sender);
                else if (sender is PowerAlarmData)
                    ConsoleServer.WriteLine("配電事件," + sender);
                else if (sender is SecurityAlarmData)
                    ConsoleServer.WriteLine("門禁事件," + sender);
                else if (sender is LightAlarmData)
                    ConsoleServer.WriteLine("照明事件," + sender);

                else if (sender is AirAlarmData)
                    ConsoleServer.WriteLine("空氣品質事件" + sender);

                sendHostEvent(TEM_DEVNAME, sender);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }
            
           // throw new Exception("The method or operation is not implemented.");
        }

      
    }
}
