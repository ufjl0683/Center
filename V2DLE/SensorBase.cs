using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.JScript.Vsa;
using Comm.SensorConfig;

namespace Comm
{
    public enum COMTYPE
    {
        TCP,COM
    }
    public delegate void OnConnectedChangedHandler(int id,string senserName,bool IsConnected);
    public  abstract class SensorBase
    {
       const int  MAX_QUEUE_CNT=50;
       protected Comm.I_DLE sensorDev;
       public COMTYPE ComType;
       public  System.Net.IPEndPoint endpoint;
       public System.IO.Ports.SerialPort com;
       public string ComPort;
       public int baud;
       public string SensorName;
       public DateTime lastReceiveTime;
       bool _IsConnected;
       public abstract void SensorDev_OnCommError(object sender);
       public abstract void sensorDev_OnReceiveText(object sender, Comm.TextPackage txtObj);
       System.Timers.Timer OneMinTmr = new System.Timers.Timer();
       public int ID;
       protected Microsoft.JScript.Vsa.VsaEngine jseng = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
      
       public event OnConnectedChangedHandler OnConnectionChanged;

       System.Collections.Generic.Queue<double>[] queValueAry =new Queue<double>[]{ new Queue<double>(), new Queue<double>() ,new Queue<double>()};
        
       public SensorBase(int id, string sensorname, System.Net.IPEndPoint endpoint )
       {

           this.ComType = COMTYPE.TCP;
           this.SensorName = sensorname;
           this.endpoint = endpoint;
         
           new System.Threading.Thread(TCPConnectTask).Start();
           ID = id;
           OneMinTmr.Interval = 1000 * 60;
           OneMinTmr.Elapsed += new System.Timers.ElapsedEventHandler(OneMinTmr_Elapsed);
           OneMinTmr.Start();
        
        }

       void OneMinTmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
       {
           if (System.DateTime.Now.Subtract(lastReceiveTime).TotalMinutes < 1)
               return;
           if (this.ComType == COMTYPE.COM)
               ComConnect();
           else
           {
               if (!IsInConnectTask)
                   OnCommError(this, new Exception("通訊逾時!"));
           }

           
           //throw new NotImplementedException();
       }
       public SensorBase(int id, string sensorname, string ComPort, int baud)
       {
           this.ComPort = ComPort;
           this.baud = baud;
           this.SensorName = sensorname;
           ComType = COMTYPE.COM;
           
           ComConnect();
           ID = id;
           OneMinTmr.Interval = 1000 * 60;
           OneMinTmr.Elapsed += new System.Timers.ElapsedEventHandler(OneMinTmr_Elapsed);
           OneMinTmr.Start();
       }
       protected void SetDataToQueue(double[] data)
       {
           for (int i = 0; i < data.Length; i++)
           {
               queValueAry[i].Enqueue(data[i]);
               while (queValueAry[i].Count > MAX_QUEUE_CNT)
                   queValueAry[i].Dequeue();


           }


       }

       public double  GetValue(int valueinx)
       {
            System.Collections.Generic.List<double> list = new List<double>(this.queValueAry[valueinx]);
            list.Sort();
            double[] values = list.ToArray();
            double sum = 0;
            int totalcnt = list.Count;

            int cnt = 0;
            for (int i = (int)(totalcnt * 0.2); i < (int)(totalcnt * .8); i++)
            {
                sum += values[i];
                cnt++;
            }

            if (cnt == 0 || totalcnt != MAX_QUEUE_CNT)
            {
               // queValue1.Clear();
                return double.NegativeInfinity;
            }
            if (DateTime.Now.Subtract(lastReceiveTime) > TimeSpan.FromMinutes(1))
            {
                for (int i = 0; i < 3; i++)
                    this.queValueAry[i].Clear();

                return double.NegativeInfinity;
            }
         
            return sum / cnt;


        
       }
       public bool IsConnected
        {
            get
            {
           
                return _IsConnected;

            }
            set
            {
                if (value != _IsConnected)
                {
                    _IsConnected = value;
                    if (OnConnectionChanged != null)
                        this.OnConnectionChanged(ID, this.SensorName, _IsConnected);
                }
            }
         }
       

   
      
        
      

     

      
        public  void ComConnect()
        {

            if (com != null)
            {
                try
                {
                    com.Close();
                    com.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "," + ex.StackTrace);
                }
            }
           com = new System.IO.Ports.SerialPort(ComPort, baud, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
           com.Open();
            byte[] tmp=new byte[512];
           // while (com.BaseStream.Read(tmp,0, 512) == 512) ;
            sensorDev = new Comm.SirfDLE(SensorName, com.BaseStream);
            sensorDev.OnCommError += new Comm.OnCommErrHandler(OnCommError);
            sensorDev.OnReceiveText += new OnTextPackageEventHandler(OnReceiveText);   //new Comm.OnTextPackageEventHandler(delDev_OnReceiveText);
        }

        void OnReceiveText(object sender, Comm.TextPackage txtObj)
        {
            try
            {
                this.IsConnected= DateTime.Now.Subtract(lastReceiveTime) < TimeSpan.FromMinutes(1);
                lastReceiveTime = DateTime.Now;
                sensorDev_OnReceiveText(sender, txtObj);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }

        }

       volatile bool IsInConnectTask = false;
       System.Net.Sockets.TcpClient client;
       void TCPConnectTask()
       {
        
           if (IsInConnectTask)
               return;
           while (true)
           {

               IsInConnectTask = true;
         
               if (client != null)
               {

                   try
                   {
                       client.Close();
                   }
                   catch { ;}
                   
               }

               client = new System.Net.Sockets.TcpClient();
               try
               {
                   client.Connect(endpoint);
                   if (!client.Connected)
                       throw new Exception();
                   sensorDev = new Comm.SirfDLE(SensorName, client.GetStream());
                   sensorDev.OnCommError += new Comm.OnCommErrHandler(OnCommError);
                   sensorDev.OnReceiveText += new Comm.OnTextPackageEventHandler(OnReceiveText);
                   Console.WriteLine(endpoint.Address + ",connected!");
                   this.IsConnected = true;
                   break;
               }
               catch
               {
                   this.IsConnected = false;
                   Console.WriteLine(endpoint + " connect error!"); ;
               }

               System.Threading.Thread.Sleep(5000);
               
           }
           IsInConnectTask = false;





       }

       void   OnCommError(object sender, Exception ex)
       {
           //throw new NotImplementedException();
           sensorDev.OnCommError -= new Comm.OnCommErrHandler( OnCommError);
           sensorDev.OnReceiveText -= new Comm.OnTextPackageEventHandler(OnReceiveText);
           this.IsConnected = false;
           Console.WriteLine(this.SensorName+"," + ex.Message);
           this.SensorDev_OnCommError(this);

           if (this.ComType == COMTYPE.TCP)
           {
               if(!this.IsInConnectTask)
                new System.Threading.Thread(TCPConnectTask).Start();
           }
           else
               ComConnect();
       }
         
    }
}
