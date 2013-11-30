using System;
using System.Collections.Generic;
using System.Text;
using Comm;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using RemoteInterface;


namespace Comm
{
 
     
   
    public delegate void ConnectStatusChangeHandler(object tc);
    public delegate void HWStatusChangeHandler(object tc,byte[] diff,byte opmode,byte opstatus);
    public delegate void OnTCReportHandler(object tc,TextPackage txt);
    public delegate void OnOPModeChangeHandler(object tc ,byte opmode);
    public delegate void OnOPStatusChangeHandler(object tc, byte opstatus);
    public delegate  void OnDbDemandHandler(object tc,string sql);
    public  abstract class TCBase
    {
        protected event ConnectStatusChangeHandler _OnConnectStatusChanged;
       
        private event ConnectStatusChangeHandler OnTCPConnectChange;
        public event HWStatusChangeHandler OnHwStatusChanged;
        public event ConnectStatusChangeHandler OnConnectStatusChanged;
        public event OnOPModeChangeHandler OnOpModeChanged;
        public event OnOPStatusChangeHandler OnOpStatusChanged;
        public event OnDbDemandHandler OnDbDemand;
        protected int m_CheckOutputCycle = 1; //資顯設備 輸出資料檢查週期 ,預設 1 min
      // public  V2DLE m_device;
        public I_DLE m_device;
       protected  string m_ip;
       protected  int m_port;
       protected  int m_deviceid;
       private volatile bool m_connected = false;

        
       TcpClient m_tcpclient;
       Thread th_init_com;
       protected  byte[] m_hwstaus = new byte[4];
        public  byte m_opmode, m_opstatus,m_comm_state;

       protected Protocol m_protocol = new Protocol();
       protected string m_deviceName;
      

        public event OnTCReportHandler  OnTCReport;
        public event OnTCReportHandler OnTCReceiveText;
        System.Timers.Timer OneMinTimer,OneHourTimer ;
      
        System.Timers.Timer tmrCheckConnectTimeOut = new System.Timers.Timer(1000 * 2 * 60); //2 min
        private bool forceClose = false;
        public bool IsTcpConnected = false;
        protected object currDispLockObj = new object();


        protected DateTime LastReceiveTime;

       // public bool IsAllowHWCheck = false;

        public bool IsF311z = false;
        protected int outputCompareMin = 1;
        private bool m_isenable=true;
        public bool IsEnable
        {

            set
            {
                this.m_isenable = value;
                if (this._OnConnectStatusChanged != null)
                    this._OnConnectStatusChanged(this);
            }
            get
            {
                return this.m_isenable;
            }
        }
     
        public TCBase(Protocol protocol,string devicename,string ip, int port, int deviceid,byte[]hw_status,byte opmode,byte opstatus,byte comm_state)
        {
            m_ip = ip;
            m_port = port;
            m_deviceid = deviceid;
            this.m_protocol = protocol;
            this.m_deviceName = devicename;
            this.m_hwstaus = hw_status;
            this.m_opmode = opmode;
            this.m_opstatus = opstatus;
            this.m_comm_state = comm_state;
           
            this.OnTCPConnectChange += new ConnectStatusChangeHandler(OnTcpConnectChanged);
            this._OnConnectStatusChanged += new ConnectStatusChangeHandler(TC_OnConnectStatusChanged);
          
            start_connect();
            OneMinTimer = new System.Timers.Timer(1000 *30);
            OneMinTimer.Elapsed += new System.Timers.ElapsedEventHandler(OneMinTimer_Elapsed);
            OneMinTimer.Start();
            OneHourTimer = new System.Timers.Timer(1000 * 60 * 60);
            OneHourTimer.Elapsed += new System.Timers.ElapsedEventHandler(OneHourTimer_Elapsed);
            OneHourTimer.Start();
           
            new System.Threading.Thread(ReportProcessTask).Start();
            LastReceiveTime = System.DateTime.Now;
          
        }

        public bool IsOnAckRegisted()
        {
            return m_device.IsOnAckRegisted();
        }

        public abstract void DownLoadConfig();


       

        void m_device_OnReceiveText(object sender, TextPackage txtObj)
        {

            try
            {
                this.LastReceiveTime = System.DateTime.Now;
                this.IsTcpConnected = true;
                if (this.IsConnected == false)
                    new System.Threading.Thread(new ParameterizedThreadStart(AsyncSetIsConnect)).Start(true);
                if (this.OnTCReceiveText != null)
                    this.OnTCReceiveText(this, txtObj);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }
                
        }

        public string  getCurrentTcCommStatusStr()
        {

            string retStr;
           
            retStr =  (( m_device!=null)?this.m_device.getQueueState():"") + "\r\n IsTcpConnected:" + IsTcpConnected + "\r\n IsConnected:" + IsConnected + "\r\n InConnect_Task:" + InConnect_Task;
            
            return retStr;
        }

        public virtual void ResetComm()
        {

            try
            {
                this.IsConnected = false;
                this.IsTcpConnected = false;
                this.start_connect();
            }
            catch { ;}
        }

      
        private void AsyncSetIsConnect(object isconnected)
        {
            //while (true)
            //{
                //lock (AsyncSetIsConnectObj)
                //{     
            try
            {
                this.IsConnected = (bool)isconnected;
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
            }

                //}
            //}
        }


        protected byte m_hw_cycle = 0; //on change
        protected byte m_trans_cycle = 5; //1:1min,5:5min
        protected byte m_trans_mode = 1; //0:polling,1:active

        public virtual void SetTransmitCycle(int cycle)
        {
            this.m_trans_cycle = (byte)cycle;
            this.TC_SendCycleSettingData();

            
        }

        public virtual void TC_SendCycleSettingData() //傳送傳輸週期設定
        {              
            if (!this.IsTcpConnected) return;

            //if (this.m_protocol.DeviceType == "ETTU" || this.m_protocol.DeviceType == "TEM")
            //    return;
               
            byte[] senddata = new byte[] { 0x03, 0, m_trans_cycle, m_trans_mode, m_hw_cycle };  //set trans cycle

            if (this.m_protocol.DeviceType.Trim().ToUpper() == "RMS")
                senddata[1] = 15;
            else if (this.m_protocol.DeviceType.Trim().ToUpper() == "VI")
                senddata[1] = 3;
            else if (this.m_protocol.DeviceType.Trim().ToUpper() == "RD")
                senddata[1] = 8;
            else if (this.m_protocol.DeviceType.Trim().ToUpper() == "RGS")
                senddata[1] = 20;
            else if (this.m_protocol.DeviceType.Trim().ToUpper() == "WD")
                senddata[1] = 4;
            else if (this.m_protocol.DeviceType.Trim().ToUpper() == "LCS")
                senddata[1] = 18;
            else if (this.m_protocol.DeviceType.Trim().ToUpper() == "CMS" || this.m_protocol.DeviceType.Trim().ToUpper()=="CMSRST")
                senddata[1] = 11;
            else if (this.m_protocol.DeviceType.Trim().ToUpper() == "CSLS")
                senddata[1] = 14;
            else if (this.m_protocol.DeviceType.Trim().ToUpper() == "MAS")
                senddata[1] = 12;
            else if (this.m_protocol.DeviceType.Trim().ToUpper() == "WIS")
                senddata[1] = 21;
            else if (this.m_protocol.DeviceType.Trim().ToUpper() == "BS")
                senddata[1] = 9;
            else if (this.m_protocol.DeviceType.Trim().ToUpper() == "VD")
                senddata[1] = 0;
            else if (this.m_protocol.DeviceType.Trim().ToUpper() == "TTS")
                senddata[1] = 16;
            else if (this.m_protocol.DeviceType.Trim().ToUpper() == "AVI")
                senddata[1] = 17;
            else if (this.m_protocol.DeviceType.Trim().ToUpper() == "FS")
                senddata[1] = 13;
            else if (this.m_protocol.DeviceType.Trim().ToUpper() == "LS")
                senddata[1] = 5;
            try
            {
               // if(this.m_protocol.DeviceType != "SCM")
                    this.m_device.Send(new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, senddata));

                //byte[] senddata1 = new byte[] { 0x03, 1, m_event_cycle, m_event_mode, m_hw_cycle };  //set event cycle
                //this.m_device.Send(new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, senddata1));
                //byte[] senddata2 = new byte[] { 0x03, 2, m_real_cycle, m_real_mode, m_hw_cycle };  //set real cycle
                //this.m_device.Send(new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, senddata2));
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(this.DeviceName + " " + "In 1min task:" + ex.Message);
            }
        }

        //void tmrCheckConnectTimeOut_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{

        //    this.IsConnected = false;
        //  //  ConsoleServer.WriteLine(this.DeviceName + ", 斷線!");

        //    //throw new Exception("The method or operation is not implemented.");
        //}

       

      public virtual  void OneHourTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
            try
            {
                System.DateTime dt = System.DateTime.Now;
                if (this.IsTcpConnected)
                {
                    
                    this.TC_GetHW_Status();
                    ConsoleServer.WriteLine(this.DeviceName + "secdiff:" + this.TC_SetDateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
                }

                //   Util.GC();
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
                ;
            }
        }

        int mincnt = 0;
       public  virtual void OneMinTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
            
        //   ConsoleServer.WriteLine("in one min task !");
            try
            {
                //mincnt = (mincnt + 1) % 60;
                //mincnt = (mincnt+1) % 2;
                //if(mincnt% 2==0)
                //IsAllowHWCheck = true;
                hw_statuscnt = 0;
                System.DateTime dt = System.DateTime.Now;


                if ( this.m_device!=null &&  this.m_device.getTotalQueueCnt() > 20)
                {
                    ConsoleServer.WriteLine(this.DeviceName + "Queue too many Data!, Comm Reset!");
                    this.ResetComm();
                    return;
                }
              
                if (this.IsTcpConnected)
                {

                   
                    //if(this is Comm.TC.AVITC)
                    //         ConsoleServer.WriteLine("週期詢問狀態");
                    //  ConsoleServer.WriteLine(this.DeviceName + "secdiff:" + this.TC_SetDateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
                    try
                    {
                        if(mincnt==0)
                        this.TC_GetHW_Status();
                        else
                        this.TC_SendCycleSettingData();
                    }
                    catch { ;}


                    if (System.DateTime.Now - this.LastReceiveTime > new TimeSpan(0, 3, 0))
                    {
                        this.LastReceiveTime = System.DateTime.Now;
                        this.IsConnected = false;
                       // this.IsTcpConnected = true;
                        ConsoleServer.WriteLine(this.DeviceName + "communication keep silent over 3 min!!");
                       
                      //  this.start_connect();
                       
                    }
                }else if (!this.IsTcpConnected && !InConnect_Task)
                {
                    this.start_connect();
                }

                //   Util.GC();
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
                ;
            }
          

        }
        private void OnTcpConnectChanged(object tc)
        {
            try
            {
                if (m_device != null)
                {
                    if (this.IsTcpConnected)
                    {

                        this.m_device.OnReport += new OnTextPackageEventHandler(m_device_OnReport);
                        this.m_device.OnReceiveText += new OnTextPackageEventHandler(m_device_OnReceiveText);
                        //this.m_device.OnTextSending += new OnSendingAckNakHandler(m_device_OnTextSending);
                        //this.m_device.OnAck += new OnAckEventHandler(m_device_OnAck);
                        //this.m_device.OnBeforeAck += new OnSendingAckNakHandler(m_device_OnBeforeAck);
                        //this.m_device.OnBeforeNak += new OnSendingAckNakHandler(m_device_OnBeforeNak);
                        //this.LastReceiveTime = System.DateTime.Now;
                    }
                    else
                    {
                

                        this.m_device.OnReport -= new OnTextPackageEventHandler(m_device_OnReport);


                        this.m_device.OnReceiveText -= new OnTextPackageEventHandler(m_device_OnReceiveText);
                        //this.m_device.OnTextSending -= new OnSendingAckNakHandler(m_device_OnTextSending);
                        //this.m_device.OnAck -= new OnAckEventHandler(m_device_OnAck);
                        //this.m_device.OnBeforeAck -= new OnSendingAckNakHandler(m_device_OnBeforeAck);
                        //this.m_device.OnBeforeNak -= new OnSendingAckNakHandler(m_device_OnBeforeNak);

                    }
                }
            }
            catch(Exception ex)
            {
                ConsoleServer.WriteLine("on TCpChange Event process"+ex.Message);
            }
        }


       
        //void m_device_OnBeforeNak(object sender, ref byte[] data)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //void m_device_OnBeforeAck(object sender, ref byte[] data)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //void m_device_OnAck(object sender, AckPackage AckObj)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //void m_device_OnTextSending(object sender, ref byte[] data)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        void TC_OnConnectStatusChanged(object tc)
        {


            if (this.OnConnectStatusChanged != null)
            {
                if (this.IsConnected != (m_comm_state != 3))
                {
                    m_comm_state=(this.IsConnected)?(byte)1:(byte)3;
                    this.OnConnectStatusChanged(this);
                }
            }
           

            if (this.IsTcpConnected)
            {
                try
                {
                    if (m_protocol.DeviceType != "TEM" && m_protocol.DeviceType != "ETTU")
                    {
                        SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.B, this.m_deviceid, new byte[] { 0x01 });
                        if (m_device != null)
                        {
                            m_device.Send(pkg);
                            if (pkg.result == CmdResult.ACK)
                                m_device_OnReport(this, pkg.ReturnTextPackage);
                        }
                        this.TC_SetDateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);
                    }
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
                }
            }
          
        }

               
        public int  port
        {
            get
            {
                return m_port;
            }
            set
            {
                m_port = value;

            }
        }

        public int DeviceID
        {
            get
            {
                return this.m_deviceid;
            }
        }
        public string IP
        {
            get
            {
                return m_ip;
            }
            set
            {
                m_ip = value;
            }
        }


        public string DeviceName
        {
            get
            {
                return m_deviceName;
            }
        }
        public bool IsConnected
        {
            get
            {
                return m_connected && IsTcpConnected  &&  IsEnable;
            }

            set
            {

                lock (this)
                {
                    if (m_connected != value)
                    {
                        if (value == true)

                            ConsoleServer.WriteLine(this.DeviceName + ",連線");



                        else

                            ConsoleServer.WriteLine(this.DeviceName + ",斷線");

                        m_connected = value;

                        if (this._OnConnectStatusChanged != null)
                            this._OnConnectStatusChanged(this);

                    }
                   
                  
                }
            }
        }

        public void start_connect()
        {
            if (InConnect_Task || IsTcpConnected)
                return;
            try
            {
                th_init_com = new Thread(Connect_Task);
           
              
                th_init_com.Start();
            }catch(Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
                ;}

        }

        public virtual int TC_SetDateTime(int year, int mon, int day, int hour, int min, int sec)
        {
            byte[] sendData=new byte[]{0x02,(byte)(year/256),(byte)(year %256),(byte)mon,(byte)day,(byte)hour,(byte)min,(byte)sec};
            SendPackage pkg=new SendPackage(CmdType.CmdQuery,CmdClass.A,this.DeviceID,sendData);
            
           
                this.m_device.Send(pkg);
                if (pkg.result == CmdResult.ACK)
                {

                    //if (pkg.sendCnt > 1)
                    //    ConsoleServer.WriteLine("SendCnt:" + pkg.sendCnt);
                    return (int)pkg.ReturnTextPackage.Text[1];
                }
                else
                {
                    throw new Exception("對時命令錯誤!"+pkg);
                }
            
              
           

        }
        protected void checkConntected()
        {
            if (!m_connected)
                throw new Exception(this + "Device not connected!");
        }

        protected volatile bool InConnect_Task = false;
   
        private void Connect_Task()
        {

            try
            {
            
              
           //     intaskcnt++;
                InConnect_Task = true;
                //while (true)
                //{

                if (forceClose)
                    return;
                    this.IsTcpConnected=false;
                    this.IsConnected = false;
                    try
                    {
                        if (m_tcpclient != null)
                        {
                            try { m_tcpclient.GetStream().Close(); }
                            catch { ;}
                            try { m_tcpclient.Close(); }
                            catch { ;}
                        }

                        if (m_device != null)
                        {
                            try {
                                m_device.Close();
                              //  m_device = null;
                            
                            }
                            catch { ;}

                        }

                        m_tcpclient = new System.Net.Sockets.TcpClient();
                       // m_tcpclient.ReceiveTimeout
                        m_tcpclient.Connect(new System.Net.IPAddress(V2DLE.getIP(m_ip)), m_port);

                        if (this.m_protocol.DeviceType == "ETTU")
                            m_device = new ETTUDLE1(this.DeviceName, m_tcpclient.GetStream());
                        else if (this.m_protocol.DeviceType == "SCM")
                            m_device = new TCDLE30(this.DeviceName, m_tcpclient.GetStream());
                        else
                            m_device = new V2DLE(this.DeviceName, m_tcpclient.GetStream());
                      
                        ConsoleServer.WriteLine(m_ip + "tcp connected!");
                     //   System.Threading.Thread.Sleep(3000);
                        this.IsTcpConnected = true;
                        if (this.OnTCPConnectChange != null)
                            this.OnTCPConnectChange(this);
                        this.m_device.OnCommError += new OnCommErrHandler(m_device_OnCommError);

                      
                        try
                        {
                            this.TC_SetDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                            ConsoleServer.WriteLine(this.DeviceName+"set date time ok");
                        }
                        catch (Exception ex){
                            ConsoleServer.WriteLine(ex.Message+","+ex.StackTrace);}
                        //this.IsConnected = true;



                        return;
                    }
                    catch(Exception ex)
                    {   

                     //   ConsoleServer.WriteLine(this +ex.Message+ "connecting error!,retry...");
                        return;
                      
                    }
                // Util.GC();
                  //  System.Threading.Thread.Sleep(10000);
                //}
             
            }
            finally
            {
                InConnect_Task = false;
             //   intaskcnt--;
            }
        }

        void m_device_OnCommError(object sender, Exception ex)
        {
         
            if (ex is System.IO.IOException)
            {
                ConsoleServer.WriteLine(this.m_ip + "," + ex.Message + "," + "reconnecting");
              
                this.IsTcpConnected = false;
                this.IsConnected = false;
                try
                {
                   // ((Comm.V2DLE)sender).Enabled = false;
                    ((Comm.I_DLE)sender).setEnable(false);
                }
                catch { ;}
                if (this.OnTCPConnectChange != null)
                {
                    try
                    {
                        this.OnTCPConnectChange(this);
                    }
                    catch (Exception ex1)
                    {
                        ConsoleServer.WriteLine(DeviceName + ex1.Message + ex1.StackTrace);
                    }
                }
               
            }
        }

        public byte[] getHwStaus()
        {
            return m_hwstaus;
        }

        public override string ToString()
        {
            return string.Format("devicename:{0} type:{1} ip:{2}  port:{3} deviceId:{4} Connected:{5}", m_deviceName, m_protocol.DeviceType, m_ip, m_port, m_deviceid, this.IsConnected);
        }

        public virtual byte[] TC_GetHW_Status()
        {
            if (this.m_protocol.DeviceType == "ETTU" || this.m_protocol.DeviceType == "TEM")
                return new byte[] { 0, 0, 0, 0};
        

            SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.B, this.m_deviceid, new byte[] { 0x0B});
            this.m_device.Send(pkg);

            if (pkg.result != CmdResult.ACK)
                return null;
            // 2011-2-16 檢查是否狀態有變
            m_device_OnReport(this, pkg.ReturnTextPackage);

            return new byte[] { pkg.ReturnTextPackage.Text[1], pkg.ReturnTextPackage.Text[2], pkg.ReturnTextPackage.Text[3], pkg.ReturnTextPackage.Text[4] };

        }

        public void InvokeDBDemand(string sql)
        {
            if(this.OnDbDemand!=null)
                this.OnDbDemand(this,sql);
        }
        public void Send(SendPackage pkg)
        {

            if (m_device == null) return;
            this.m_device.Send(pkg);
            if (pkg.result != CmdResult.ACK  && pkg.Cmd!=0xA5)
                throw new Exception(this.DeviceName+","+pkg.ToString());
        }

        public abstract I_HW_Status_Desc getStatusDesc();

        System.Collections.Queue ReportQueue = System.Collections.Queue.Synchronized(new System.Collections.Queue());

        void ReportProcessTask()
        {
            while (true)
            {
                try
                {
                   
                        if (ReportQueue.Count == 0)
                        {
                             lock (ReportQueue)
                            {
                                 System.Threading.Monitor.Wait(ReportQueue);
                            }
                        }
                        else
                            DoReport((TextPackage)ReportQueue.Dequeue());

                   
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }
        int hw_statuscnt = 0;
        void DoReport(TextPackage txtObj)
        {
            if (!this.IsEnable)
                return;
            byte[] diff = new byte[4];
            bool IsOpmodeChanged=false,IsOpstatusChanged=false;
            if (txtObj == null)
                return;
            try
            {
                if (this.m_protocol.DeviceType == "SCM")
                {
                    if (txtObj.Text[0] == 0x0f && txtObj.Text[1] == 0x04)  //硬體狀態主動回報
                    {
                        for (int i = 0; i < 2; i++)
                            diff[i] = (byte)(txtObj.Text[2 +i] ^ m_hwstaus[i]);
                        System.Array.Copy(txtObj.Text, 2, m_hwstaus, 0, 2);



                        if (this.OnHwStatusChanged != null && (diff[0] + diff[1] + diff[2] + diff[3] != 0))
                        {
                            hw_statuscnt++;
                            if (hw_statuscnt <= 5)
                                this.OnHwStatusChanged(this, diff, m_opmode, m_opstatus);

                        }

                    }
                }
                else
                if (txtObj.Cmd == 0x0a && txtObj.Text.Length == 5 || txtObj.Cmd == 0x01 && txtObj.Text.Length == 5  || txtObj.Cmd==0x0b)
                {
                    if (txtObj.Cmd == 0x0b)
                    {
                        if(m_opmode!=txtObj.Text[7])
                        {
                            IsOpmodeChanged=true;
                            m_opmode = txtObj.Text[7];
                        }
                        if (m_opstatus != txtObj.Text[6])
                        {
                            m_opstatus = txtObj.Text[6];
                            IsOpstatusChanged = true;
                        }
                    }
                    for (int i = 0; i < 4; i++)
                        diff[i] = (byte)(txtObj.Text[i + 1] ^ m_hwstaus[i]);
                    System.Array.Copy(txtObj.Text, 1, m_hwstaus, 0, 4);

                    if (IsOpmodeChanged && this.OnOpModeChanged != null)
                        this.OnOpModeChanged(this, m_opmode);

                    if (IsOpstatusChanged && this.OnOpStatusChanged != null)
                        this.OnOpStatusChanged(this, m_opstatus);

                    if (this.OnHwStatusChanged != null && (diff[0] + diff[1] + diff[2] + diff[3] != 0))
                    {
                        hw_statuscnt++;
                        if(hw_statuscnt<=2)
                        this.OnHwStatusChanged(this, diff, m_opmode, m_opstatus);

                    }

                }
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(this.DeviceName+":"+ex.Message + "\n" + ex.StackTrace);
            }
            try
            {
                if (this.OnTCReport != null)
                {
                    //  Console.WriteLine("<OnReport>" + txtObj.ToString());
                    this.OnTCReport(this, txtObj);
                }
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(this.DeviceName+ex.Message+ex.StackTrace);
            }
        }

        public  void m_device_OnReport(object sender, TextPackage txtObj)
        {
           
                try
                {
                    if (txtObj == null)
                        return;
                  
                    ReportQueue.Enqueue(txtObj);
                     lock (ReportQueue)
                     {
                         System.Threading.Monitor.PulseAll(ReportQueue);
                     }

                }
                catch (Exception ex) { ConsoleServer.WriteLine(this.DeviceName + ex.Message + ex.StackTrace); ;}
           
            //throw new Exception("The method or operation is not implemented.");
        }

        public  void Close()
        {
            try{

                forceClose = true;
                this.IsTcpConnected = false;
                this.m_connected = false;
                if (this.m_device != null)
                    // this.m_device.Enabled = false;
                    this.m_device.setEnable(false);
                try
                {

                    this.m_tcpclient.GetStream().Close();
                }
                catch { ;}
                try
                {

                    this.m_tcpclient.Close();
                }
                catch { ;}
                try
                {
                    this.m_device.Close();
                }
                catch { ;}

                

            }catch
            {;
            }
        }

        protected void AsyncSend(SendPackage pkg)
        {
            new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(AsynSendPackageTask)).Start(pkg);

        }

        private  void AsynSendPackageTask(object pkg)
        {
            try
            {

                this.Send((SendPackage)pkg);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }
        }
    }
}
