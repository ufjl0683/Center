using System;
using System.Collections.Generic;
using System.Text;
using Comm.SensorConfig;
using Comm.DataStore;
using System.IO;
namespace Comm.Controller
{
    public abstract class ControllerBase
    {
       // public byte[] hw_status = new byte[4];
        public  SensorBase[] devices;
        protected DeviceV2DLE v2dle;
        public int ListenPort;
        protected Protocol protocol = new Protocol(AppDomain.CurrentDomain.BaseDirectory+"V20.cgt");
        public abstract void OnReceiveText(TextPackage text);
        protected abstract void OnOneMinTmrTask();
        protected abstract void PeroidDataReportTask(object obj);
        protected abstract SensorBase CreateDevice(SensorConfigBase I_sebsor_config);
       // protected abstract void OnPeriodDataReport();
        object lockobj = new object();
        public ushort ControllerID;
        protected  PropertyBag  PropertyBag;
        System.Threading.Timer tmrHwStatus;
        protected DataStorage<double> dataStore = new DataStorage<double>();
        string DeviceType;
        DateTime BuildDate;
        byte VersionNo;
        public  ControllerConfigBase config;
        System.Threading.Timer tmrPeriodDataReport;
        System.Threading.Timer OneMinTmr;
     
        public ControllerBase(ControllerConfigBase config,PropertyBag property)
        {
            this.PropertyBag = property;
            this.DeviceType = config.device_type;
            this.ListenPort = config.listen_port;
            this.ControllerID = config.controller_id;
            this.BuildDate = config.build_date;
            this.VersionNo = config.version_no;
            tmrPeriodDataReport = new System.Threading.Timer(new System.Threading.TimerCallback(PeroidDataReportTask));
            this.config = config;
            CreateDevices(config);
            tmrHwStatus = new System.Threading.Timer(new System.Threading.TimerCallback(TmrHwStausTask));
           // tmrHwStatus.Change(0, 1000);
            SetHWStatusTmr();
          
          //  tmrPeriodDataReport.Change(GetNextTransmitSeconds()*1000,this.PropertyBag.TransmitCycle*60* 1000);
            SetTransmitCycleTmr();
            StartListening(ListenPort);
            OneMinTmr = new System.Threading.Timer(OneMinTask);
            OneMinTmr.Change(GetNextMinDifferentSec()*1000, 60 * 1000);
            this.protocol.Parse(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "protocol.txt"), false);
            this.PropertyBag.OnHWStatus_Changed += new EventHandler(PropertyBag_OnHWStatus_Changed);

        }

        void CreateDevices(ControllerConfigBase config)
        {
            if (config.sensors == null)
            {
                devices = new SensorBase[0];
                return;
            }
            devices = new SensorBase[config.sensors.Length];
            for (int i = 0; i < config.sensors.Length; i++)
            {
                devices[i] = CreateDevice(config.sensors[i]);
                devices[i].OnConnectionChanged += new OnConnectedChangedHandler(Device_OnConnectionChanged);
            }
            
          
        }

        void Device_OnConnectionChanged(int id, string senserName, bool IsConnected)
        {
            lock (this)
            {
                ushort c_status = (ushort)(this.PropertyBag.HWtatus[2] + (this.PropertyBag.HWtatus[3] << 8));
                if (!IsConnected)
                    c_status |= (ushort)(1 << id);
                else
                    c_status &= (ushort)(~(1 << id));
                this.PropertyBag.HWtatus[2] = (byte)(c_status % 256);
                this.PropertyBag.HWtatus[3] = (byte)(c_status / 256);
                this.PropertyBag.Serialize();
            }

            //throw new NotImplementedException();
        }


        void PropertyBag_OnHWStatus_Changed(object sender, EventArgs e)
        {
            try
            {
                SendPackage pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, this.ControllerID, new byte[] { 0x0a, PropertyBag.HWtatus[0], PropertyBag.HWtatus[1], PropertyBag.HWtatus[2], PropertyBag.HWtatus[3] });
                //throw new NotImplementedException();
                if (v2dle != null)
                    v2dle.Send(pkg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }
        }
     
   

        void OneMinTask(object state)
        {
            try
            {

                try
                {
                    for (int i = 0; i < this.devices.Length; i++)
                    {
                        if (!devices[i].IsConnected)
                            this.PropertyBag.HWtatus[i / 8+2] |= (byte)(1 << i % 8);
                        else
                            this.PropertyBag.HWtatus[i / 8+2] &= (byte)(~(1 << i % 8));

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "," + ex.StackTrace);
                };

                try
                {



                    OnOneMinTmrTask();
                }
                catch { ;}

               //dt.Subtract(new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0)).TotalSeconds;
               // sec = PropertyBag.TransmitCycle - (sec % PropertyBag.TransmitCycle);
                 int sec=GetNextMinDifferentSec();
                 OneMinTmr.Change(sec * 1000, 60 * 1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }
        }

       public static  int GetNextMinDifferentSec(  )
        {
            DateTime dt = DateTime.Now;
            dt = dt.AddMinutes(1).AddSeconds(-dt.Second);
            int sec = (int)dt.Subtract(DateTime.Now).TotalSeconds;
            if (sec < 5)
                sec += 60;
           return sec ;  
        }


     
        protected void SetTransmitCycleTmr()
        {
            if (PropertyBag.TransmitMode == 0 || PropertyBag.TransmitCycle == 0)
            {
                tmrPeriodDataReport.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                return;
            }
          System.DateTime dt=  System.DateTime.Now;
          int sec = (int)dt.Subtract(new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0)).TotalSeconds;
          sec = PropertyBag.TransmitCycle * 60 - (sec % (PropertyBag.TransmitCycle * 60));
          tmrPeriodDataReport.Change(sec * 1000, PropertyBag.TransmitCycle * 60*1000);
        }

      protected  void SetHWStatusTmr()
        {
            if (PropertyBag.HWCycle == 0)
            {
                tmrHwStatus.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                return;
            }
               
            
            System.DateTime dt = System.DateTime.Now;
            int sec = (int)dt.Subtract(new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0)).TotalSeconds;
            sec = PropertyBag.HWCycle * 60 - (sec % (PropertyBag.HWCycle * 60));
            tmrHwStatus.Change(sec * 1000, PropertyBag.HWCycle * 60*1000);
        }

  
    
       
        void TmrHwStausTask(object obj)
        {
            if (v2dle == null ) return;
          
            try
            {
                if (PropertyBag.TransmitCycle == 0)
                    return;
                DateTime dt = DateTime.Now;
           
                
               
               
                    this.v2dle.Send(new Comm.SendPackage(Comm.CmdType.CmdSet, Comm.CmdClass.A, this.ControllerID,
                   new byte[] { 0x0B,
                     PropertyBag.HWtatus[0],PropertyBag.HWtatus[1],PropertyBag.HWtatus[2],
                     PropertyBag.HWtatus[3],1/* comstate*/,PropertyBag.OPStatus,PropertyBag.OPMode}));
                     SetHWStatusTmr();
                    //int sec = (int)dt.Subtract(new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0)).TotalSeconds;
                    //sec = PropertyBag.HWCycle - (sec % PropertyBag.HWCycle);
                    //tmrHwStatus.Change(sec * 1000, PropertyBag.HWCycle * 1000);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void StartListening(int port)
        {
            ListenPort = port;
            new System.Threading.Thread(ListenTask).Start();
        }

        void ListenTask()
        {

            System.Net.Sockets.TcpListener listener=null;
            System.Net.Sockets.TcpClient client;
            while (true)
            {
                try
                {
                    if (listener == null)
                    {
                        listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any,ListenPort);
                        Console.WriteLine("listen:" + ListenPort);
                        listener.Start();
                    }

                    client = listener.AcceptTcpClient();
                    Console.WriteLine("=================Accept connection!================");
                    v2dle = new DeviceV2DLE("client", client.GetStream());
                    v2dle.OnReceiveText += new OnTextPackageEventHandler(v2dle_OnReceiveText);
                    v2dle.OnCommError += new OnCommErrHandler(v2dle_OnCommError);
                    lock (lockobj)
                    {
                        System.Threading.Monitor.Wait(lockobj);
                    }
                }
                catch {

                    System.Threading.Thread.Sleep(1000);
                    ;}
                

            }

        }

        void v2dle_OnCommError(object sender, Exception ex)
        {
            v2dle.OnReceiveText -= v2dle_OnReceiveText;
            v2dle.OnCommError -= v2dle_OnCommError;
            v2dle.Close();
            Console.WriteLine("Dispose V2dle!");
            v2dle = null;
            lock (lockobj)
            {
                System.Threading.Monitor.Pulse(lockobj);
            }
        }
        public void SendDirect(ushort controllerid, byte[] text, byte seq)
        {
            if (v2dle == null) return;
            byte[] data = this.v2dle.PackData(controllerid, text, (byte)seq);


            System.IO.Stream stream = v2dle.GetStream();
            lock (stream)
            {
                stream.Write(data, 0, data.Length);
            }
           // v2dle.isReportCmd = true;
        }
        public void SendDirect(SendPackage pkg)
        {
            if (v2dle == null) return;
            byte[] data = this.v2dle.PackData(pkg.address, pkg.text, (byte)pkg.Seq);


            System.IO.Stream stream = v2dle.GetStream();
            lock (stream)
            {
                stream.Write(data, 0, data.Length);
            }
            // v2dle.isReportCmd = true;
        }

        public byte[] To04DataBytes(byte[] data)
        {
            byte[] ret = new byte[data.Length + 7];
            ret[0] = 0x04;
            ret[1] = this.PropertyBag.HWtatus.hwstatus1;
            ret[2] = this.PropertyBag.HWtatus.hwstatus2;
            ret[3] = this.PropertyBag.HWtatus.hwstatus3;
            ret[4] = this.PropertyBag.HWtatus.hwstatus4;
            ret[5] = (byte)(data.Length / 256);
            ret[6] = (byte)(data.Length % 256);
            System.Array.Copy(data, 0, ret, 7, data.Length);
            return ret;

        }
        void v2dle_OnReceiveText(object sender, TextPackage txt)
        {
            //throw new NotImplementedException();
            System.Data.DataSet ds;
            if (v2dle == null)
                return;
            try
            {
                switch (txt.Text[0])
                {
                    case 0x04:  //query
                        txt.IsQueryCmd = true;

                        switch (txt.Text[1])
                        {
                            case 0x02: //get date time

                               
                                SendDirect(ControllerID, new byte[] { 04,this.PropertyBag.HWtatus[0],this.PropertyBag.HWtatus[1]
                                    ,this.PropertyBag.HWtatus[2],this.PropertyBag.HWtatus[3],0,7,2 , (byte)(DateTime.Now.Year/256),(byte)(DateTime.Now.Year %256),
                            (byte)DateTime.Now.Month,(byte)DateTime.Now.Day,(byte)DateTime.Now.Hour,(byte)DateTime.Now.Minute,(byte)DateTime.Now.Second}, (byte)txt.Seq);
                                break;
                            case 0x03:  //trandmission cycle
                                int _devtype=1;
                                
                               SendDirect(this.ControllerID,
                                   new byte[]{0x04,PropertyBag.HWtatus[0],PropertyBag.HWtatus[1],PropertyBag.HWtatus[2],PropertyBag.HWtatus[3],
                                       0x00,0x05,0x03,1,PropertyBag.TransmitCycle,PropertyBag.TransmitMode,PropertyBag.HWCycle},(byte)txt.Seq);
                               break;
                            case 0x07:
                               SendDirect(this.ControllerID,
                                new byte[]{0x04,PropertyBag.HWtatus[0],PropertyBag.HWtatus[1],PropertyBag.HWtatus[2],PropertyBag.HWtatus[3],
                                       0x00,0x02,0x07,PropertyBag.IsAllowManualMode?(byte)0:(byte)1}, (byte)txt.Seq);
//cmd=0x04                  
                           

//description="Get Trasmiission cycle"
//class=A
//func_name="get_transmission_cycle"
//type=Query
//send= protocol_code(1:3-3) device_type(1:4 "4_AM Cycle")
//return= hw_status_1(1:0-255) hw_status_2(1:0-255) hw_status_3(1:0-255) hw_status_4(1:0-255) 
//protocol_length(2:0-65530) protocol_code(1:3-3)  device_type(1:4 "4_AM Cycle")  transmission_cycle(1:1-255) transmit_mode(1:0 "Polling",1 "Active" ) hwcyc(1: 0 "State change" ,1 "5sec",2 "10Sec",3 "20sec",4 "1min",5 "5min")
//test=@cmd protocol_code(3) device_type(4)
                                break;

                            case 0x08:
                                SendDirect(this.ControllerID,
                             new byte[]{0x04,PropertyBag.HWtatus[0],PropertyBag.HWtatus[1],PropertyBag.HWtatus[2],PropertyBag.HWtatus[3],
                                       0x00,0x03,0x08,txt.Text[2],this.config.sensors[txt.Text[2]].execution_mode }, (byte)txt.Seq);
                                break;

                            case 0x20:
                                byte[] configbytes = GetConfigBytes();
                                byte[] data = new byte[configbytes.Length + 3];

                                data[0] = 0x20;
                                data[1] = (byte)(configbytes.Length / 256);
                                data[2] = (byte)(configbytes.Length % 256);
                                System.Array.Copy(configbytes, 0, data, 3, configbytes.Length);

                                SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, this.ControllerID, To04DataBytes(data));
                                pkg.Seq = txt.Seq;
                                SendDirect(pkg);
                                break;
                            default:
                                OnReceiveText(txt);
                                break;
                        }
                        break;
                    case 0x02:  //set date time get diff
                        int year = txt.Text[1] * 256 + txt.Text[2];
                        int month = txt.Text[3];
                        int day = txt.Text[4];
                        int hour = txt.Text[5];
                        int min = txt.Text[6];
                        int sec = txt.Text[7];
                      
                        DateTime d = new DateTime(year, month, day, hour, min, sec);
                        uint diff = (uint)Math.Abs(d.Subtract(DateTime.Now).TotalSeconds);
                        if (diff > 255)
                            diff = 255;
                        if(diff<3)
                            Comm.Util.SetSysTime(d);
                        txt.IsQueryCmd = true;
                        SendDirect(this.ControllerID, new byte[] { 2, (byte)diff }, (byte)txt.Seq);
                        break;

                    case 0x01: //get hw status
                       
                        // ds = protocol.GetSendDsByTextPackage(txt, CmdType.CmdQuery);
                        txt.IsQueryCmd = true;
                    //    ds= protocol.GetReturnDataSet("get_HW_Status");

                        SendDirect(this.ControllerID, new byte[] { 01, this.PropertyBag.HWtatus[0], this.PropertyBag.HWtatus[1], this.PropertyBag.HWtatus[2], this.PropertyBag.HWtatus[3] }, (byte)txt.Seq);
                       
                        
                        break;
                    case 0x00:  //reset
                        Console.WriteLine("reset!");
                        Environment.Exit(0);
                      
                        break;
                    case 0x03: //Set transmit cycle
//[Command]
//cmd=0x03
//description= "Set Trasmiission cycle"
//class=A
//func_name="set_transmission_cycle"
//type=Set
//send=device_type(1:4 "4_AM",18 "18_Other")  transmission_cycle(1:0-255) transmit_mode(1:0 "Polling",1 "Active" ) hwcyc(1: 0 "State change" ,1 "5sec",2 "10Sec",3 "20sec",4 "1min",5 "5min")
//return=
//test=@cmd device_type(4) transmission_cycle(5) transmit_mode(1) hwcyc(1)
                        ds = protocol.GetSendDsByTextPackage(txt, CmdType.CmdSet);
                        byte device_type= (byte)ds.Tables[0].Rows[0]["device_type"];
                        byte transmission_cycle = (byte)ds.Tables[0].Rows[0]["transmission_cycle"];
                        byte transmit_mode = (byte)ds.Tables[0].Rows[0]["transmit_mode"];
                        byte hwcyc = (byte)ds.Tables[0].Rows[0]["hwcyc"];

                        PropertyBag.HWCycle = hwcyc;
                        PropertyBag.TransmitCycle = transmission_cycle;
                        PropertyBag.TransmitMode = transmit_mode;
                        SetHWStatusTmr();
                        SetTransmitCycleTmr();

                        Console.WriteLine("transmission_cycle:" + transmission_cycle);
                        break;

                    case 0x07: // set Manul Mode
                        if (this.PropertyBag.IsAllowManualMode != ((txt.Text[1] == 0) ? true : false))
                        {
                            this.PropertyBag.IsAllowManualMode = (txt.Text[1] == 0) ? true : false;
                            SendPackage pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, this.ControllerID, new byte[]{0x07,
                            (byte)(PropertyBag.IsAllowManualMode?0:1)});
                            PostSend(pkg);
                        }
                        break;

                    case 0x08:// set Execution mode
                      //  this.config.execution_mode = txt.Text[1]; execution mode will remove
                        this.config.sensors[txt.Text[1]].execution_mode = txt.Text[2];
                        this.config.Serialize();
                        break;
                    case 0x0b: //get hw status
                        txt.IsQueryCmd = true;
                        SendDirect(this.ControllerID, new byte[] { 0x0b, this.PropertyBag.HWtatus[0], this.PropertyBag.HWtatus[1], this.PropertyBag.HWtatus[2], this.PropertyBag.HWtatus[3],PropertyBag.ComState ,PropertyBag.OPStatus,PropertyBag.OPMode}, (byte)txt.Seq);
                        break;

                 
                    case 0x0E:
                        txt.IsQueryCmd = true;


                        SendDirect(this.ControllerID,
                            new byte[] {0x0e,this.PropertyBag.HWtatus[0],this.PropertyBag.HWtatus[1],this.PropertyBag.HWtatus[2],this.PropertyBag.HWtatus[3],
                           (byte)(BuildDate.Year/256),(byte)(BuildDate.Year%256),(byte)BuildDate.Month,(byte)BuildDate.Day,
                            0x30,0x30,0x30,0x30,0x30,1,this.VersionNo,
                            (byte)(this.ControllerID/256),(byte)(this.ControllerID%256)}, (byte)txt.Seq);
                        break;
                    case 0x20:  //set Communication param
                        SetConfig(txt);
                        break;

                       

                     default:
                         OnReceiveText(txt);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }
           
        }

        byte[] GetConfigBytes()
        {

            MemoryStream ms = new MemoryStream();
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(ControllerConfigBase));
            ser.Serialize(ms, this.config);
            return ms.GetBuffer();

        }

        void SetConfig(Comm.TextPackage text)
        {


            MemoryStream ms = new MemoryStream(text.Text, 3, text.Text[1] * 256 + text.Text[2]);
            this.config = ControllerConfigBase.Deserialize(ms);

            config.Serialize(AppDomain.CurrentDomain.BaseDirectory + "config.xml");
        }
        public void PostSend(SendPackage pkg)
        {
            new System.Threading.Thread(PostSendThreadJob).Start(pkg);
        }

        void PostSendThreadJob(object pkg)
        {
            try
            {
                this.v2dle.Send(pkg as SendPackage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("PostSendThreadJob:" + ex.Message);
            }

        }

        public static DateTime GetYMDHM(DateTime dt)
        {

            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);

        }

    }
}
