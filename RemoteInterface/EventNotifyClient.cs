using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface
{

    public delegate void NotifyEventHandler(object sender,NotifyEventObject objEvent);
    public delegate void OnConnectEventHandler(object sender);

   public class EventNotifyClient
    {
        volatile  bool connected = false;
       public event NotifyEventHandler OnEvent;
       public event OnConnectEventHandler OnConnect;
        System.Net.Sockets.TcpClient tcp;
       byte[] ipByte = new byte[4];
      public  int port;
      // public string ip;
       bool bAutoRetry = false;
       bool IsDisposing = false;
       System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf;

       public EventNotifyClient()
       {

       }

       public void Connect(string strIP, int port)
       {
           this.bAutoRetry = false;
           this.port = port;
           string[] ips = strIP.Split(new char[] { '.' });

           for (int i = 0; i < 4; i++)
               ipByte[i] = System.Convert.ToByte(ips[i]);


           tcp = new System.Net.Sockets.TcpClient();

           tcp.Connect(new System.Net.IPAddress(ipByte), port);

           bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
           connected = true;
           Console.WriteLine("NotifySerevr connected!");
           new System.Threading.Thread(ClientWork).Start();
           if (this.OnConnect != null)
           {
               try
               {
                   this.OnConnect(this);
               }
               catch { ;}
           }

          
           
       }
        public EventNotifyClient(string strIP,int port,bool bAutoRetry)
        {
            this.bAutoRetry = bAutoRetry;
            this.port = port;
            string[]ips=strIP.Split(new char[]{'.'});
           
            for(int i=0;i<4;i++)
                ipByte[i]=System.Convert.ToByte(ips[i]);


            //if (bAutoRetry)
            //{
                System.Threading.Thread th = new System.Threading.Thread(Connect_Task);
                th.Start();

                
            //}
            //else
            //{
            //    tcp = new System.Net.Sockets.TcpClient();

            //    tcp.Connect(new System.Net.IPAddress(ipByte), port);

            //    bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            //    connected = true;
            //    Console.WriteLine("NotifySerevr connected!");
            //    new System.Threading.Thread(ClientWork).Start();
            //}
         
             
        }

       public void  RegistEvent(NotifyEventObject eventObj)
       {
           if(!connected)
              throw new Exception("通訊斷線或連線中，請稍後再試!");

          bf.Serialize(tcp.GetStream(),eventObj);
       }

       public void ClientWork()
       {
           NotifyEventObject obj=null;
           while (!this.IsDisposing)
           {
               try
               {
                 

                   obj = (NotifyEventObject)bf.Deserialize(tcp.GetStream());
                    
           
               if (OnEvent != null)
               {
                   OnEvent(this,obj);
               }
              
               }
               catch(Exception ex)
               {
                   Console.WriteLine(ex.Message+",reconnecting!");
                  
                       connected = false;
                      // tcp.GetStream().Close();
                       tcp.Close();

                      if(bAutoRetry  && !IsDisposing)
                       new System.Threading.Thread(Connect_Task).Start();
                       break;
               }
               
              
           }
       }

       public void close()
       {
           this.IsDisposing = true;
           this.tcp.Close();
           
       }

       volatile bool intask = false;
       public void Connect_Task()
       {
           if (intask)
               return;
           intask = true;
         //  System.Threading.Thread.Sleep(5000);
           while (!this.IsDisposing)
           {
               try
               {

                   tcp = new System.Net.Sockets.TcpClient();

                   tcp.Connect(new System.Net.IPAddress(ipByte), port);
                 
                   bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                   connected = true;
                   Console.WriteLine("NotifySerevr connected!");
                   new System.Threading.Thread(ClientWork).Start();
                   if (this.OnConnect != null)
                   {
                       try
                       {
                           this.OnConnect(this);
                       }
                       catch { ;}
                   }
                       
                   break;
               }
               catch(Exception ex)
               {
                   Console.WriteLine(ex.Message+"Notify Serevr connecting error!,retry...");
                   try
                   {
                       tcp.Close();
                       GC.Collect();
                       GC.WaitForPendingFinalizers();
                       for (int i = 0; i < 500; i++)
                           System.Diagnostics.Debug.Print("collect");
                   }
                   catch { ;}

                   System.Threading.Thread.Sleep(10000);
               }

           }
           intask = false;
       }

      
     
    }
}
