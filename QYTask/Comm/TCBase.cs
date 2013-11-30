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
    public delegate void HWStatusChangeHandler(object tc,byte[] diff);
    public delegate void OnTCReportHandler(object tc,TextPackage txt);
    public  abstract class TCBase
    {
       protected event ConnectStatusChangeHandler _OnConnectStatusChanged;
      
       
       protected V2DLE m_device;
       protected  string m_ip;
       protected  int m_port;
       protected  int m_deviceid;
       private volatile bool m_connected = false;
       TcpClient m_tcpclient;
       Thread th_init_com;
       protected  byte[] m_hwstaus = new byte[4];

       protected Protocol m_protocol = new Protocol();
       protected string m_deviceName;
       public event HWStatusChangeHandler OnHwStatusChanged;
       public event ConnectStatusChangeHandler OnConnectStatusChanged;
        public event OnTCReportHandler  OnTCReport;
        System.Timers.Timer OneMinTimer,OneHourTimer ;

     
        public TCBase(Protocol protocol,string devicename,string ip, int port, int deviceid)
        {
            m_ip = ip;
            m_port = port;
            m_deviceid = deviceid;
            this.m_protocol = protocol;
            this.m_deviceName = devicename;
            this._OnConnectStatusChanged += new ConnectStatusChangeHandler(TC_OnConnectStatusChanged);
          //  th_1_min_task=new System.Threading.Thread(inTask);
            start_connect();
            OneMinTimer = new System.Timers.Timer(1000 * 60);
            OneMinTimer.Elapsed += new System.Timers.ElapsedEventHandler(OneMinTimer_Elapsed);
            OneMinTimer.Start();
            OneHourTimer = new System.Timers.Timer(1000 * 60 * 60);
            OneHourTimer.Elapsed += new System.Timers.ElapsedEventHandler(OneHourTimer_Elapsed);
        }

      public virtual  void OneHourTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

       public  virtual void OneMinTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
            try
            {
               System.DateTime dt= System.DateTime.Now;
                if (this.IsConnected)
                {
                    this.TC_GetHW_Status();
                    Console.WriteLine("secdiff:"+this.TC_SetDateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute,dt.Second));
                }

                if (m_device != null)
                {
                    Console.Write(this.IP + ":");
                  //  m_device.PrintQueueState();
                }
              
            }
            catch(Exception ex)
            { 
                Console.WriteLine(ex.Message);
                ;}
         //  Console.WriteLine("in one min task !");
                GC.Collect();
                GC.WaitForPendingFinalizers();
           
        }

        void TC_OnConnectStatusChanged(object tc)
        {
            try
            {
                if (this.IsConnected)
                {
                    this.m_device.OnReport += new OnTextPackageEventHandler(m_device_OnReport);

                }
                else
                {
                    this.m_device.OnReport -= new OnTextPackageEventHandler(m_device_OnReport);

                }
            }
            catch
            {
                ;
            }

            if (this.OnConnectStatusChanged != null)
                this.OnConnectStatusChanged(this);

            if (this.IsConnected)
            {
                try
                {
                    SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, this.m_deviceid, new byte[] { 0x01 });
                    if (m_device != null)
                    {
                        m_device.Send(pkg);
                        if (pkg.result == CmdResult.ACK)
                            m_device_OnReport(this, pkg.ReturnTextPackage);
                    }
                    this.TC_SetDateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            //throw new Exception("The method or operation is not implemented.");
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
                return m_connected;
            }

            set
            {

                if (m_connected != value && this._OnConnectStatusChanged != null)
                {
                    m_connected = value;
                    this._OnConnectStatusChanged(this);
                }
                else

                    m_connected = value;
            }
        }

        public void start_connect()
        {
            th_init_com = new Thread(Connect_Task);
            th_init_com.Start();
        }


        public int TC_SetDateTime(int year, int mon, int day, int hour, int min, int sec)
        {
            byte[] sendData=new byte[]{0x02,(byte)(year/256),(byte)(year %256),(byte)mon,(byte)day,(byte)hour,(byte)min,(byte)sec};
            SendPackage pkg=new SendPackage(CmdType.CmdQuery,CmdClass.A,this.DeviceID,sendData);
            
           
                this.m_device.Send(pkg);
                if (pkg.result == CmdResult.ACK)
                {

                    if (pkg.sendCnt > 1)
                        Console.WriteLine("SendCnt:" + pkg.sendCnt);
                    return (int)pkg.ReturnTextPackage.Text[1];
                }
                else
                    throw new Exception("對時命令錯誤!");
            
              
           

        }
        protected void checkConntected()
        {
            if (!m_connected)
                throw new Exception(this + "Device not connected!");
        }

        private volatile bool InConnect_Task = false;
        private void Connect_Task()
        {

            try
            {
                if (InConnect_Task) return;
                InConnect_Task = true;
                while (true)
                {
                    this.IsConnected = false;
                    try
                    {
                        if (m_tcpclient != null)
                        {
                            try { m_tcpclient.Close(); }
                            catch { ;}
                        }

                        if (m_device != null)
                        {
                            try { m_device.Close(); }
                            catch { ;}

                        }

                        m_tcpclient = new System.Net.Sockets.TcpClient();

                        m_tcpclient.Connect(new System.Net.IPAddress(V2DLE.getIP(m_ip)), m_port);

                        m_device = new V2DLE(this.DeviceName,m_tcpclient.GetStream());
                        //  m_device.OnReport += new OnTextPackageEventHandler(m_device_OnReport);

                        this.IsConnected = true;

                        this.m_device.OnCommError += new OnCommErrHandler(m_device_OnCommError);

                        Console.WriteLine(m_ip + " connected!");
                        //try
                        //{
                        //    byte[] senddata = new byte[] { 0x01 };//get hw_status
                        //    this.m_device.Send(new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, senddata));
                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine(ex.Message);
                        //}
                        
                        //  new System.Threading.Thread(ClientWork).Start();
                        break;
                    }
                    catch
                    {
                        
                      
                        Console.WriteLine(this + "connecting error!,retry...");
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        for (int i = 0; i < 500; i++)
                            System.Diagnostics.Debug.Print("collect");
                        System.Threading.Thread.Sleep(20000);
                    }

                }
            }
            finally
            {
                InConnect_Task = false;
            }
        }

        void m_device_OnCommError(object sender, Exception ex)
        {
            //throw new Exception("The method or operation is not implemented.");
            if (ex is System.IO.IOException)
            {
                Console.WriteLine(this.m_ip + "," + ex.Message + "," + "reconnecting");
                this.IsConnected = false;
                start_connect();
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

        public byte[] TC_GetHW_Status()
        {
            SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, this.m_deviceid, new byte[] { 0x01});
            this.m_device.Send(pkg);

            if (pkg.result != CmdResult.ACK)
                return null;

            return new byte[] { pkg.ReturnTextPackage.Text[1], pkg.ReturnTextPackage.Text[2], pkg.ReturnTextPackage.Text[3], pkg.ReturnTextPackage.Text[4] };

        }


        public void Send(SendPackage pkg)
        {
            this.m_device.Send(pkg);
        }

        public abstract I_HW_Status_Desc getStatusDesc();
        public  void m_device_OnReport(object sender, TextPackage txtObj)
        {
            byte[] diff = new byte[4];
            try
            {
                if (txtObj.Cmd == 0x0a && txtObj.Text.Length == 5 || txtObj.Cmd == 0x01 && txtObj.Text.Length == 5)
                {

                    for (int i = 0; i < 4; i++)
                        diff[i] = (byte)(txtObj.Text[i + 1] ^ m_hwstaus[i]);
                    System.Array.Copy(txtObj.Text, 1, m_hwstaus, 0, 4);
                    if (this.OnHwStatusChanged != null)
                        this.OnHwStatusChanged(this, diff);

                }
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
            try
            {
                if (this.OnTCReport != null)
                    this.OnTCReport(this, txtObj);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //throw new Exception("The method or operation is not implemented.");
        }




    }
}
