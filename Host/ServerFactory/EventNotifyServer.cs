using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace RemoteInterface
{
 public   class EventNotifyServer
    {
        System.Net.Sockets.TcpListener tcp;
        System.Collections.ArrayList clientStreamArray =  System.Collections.ArrayList.Synchronized( new System.Collections.ArrayList());
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bFormat = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        System.Timers.Timer tmrClearConnecttion;
       public  EventNotifyServer(int port)
        {
            tcp = new TcpListener(System.Net.IPAddress.Any, port);
            Console.WriteLine("binding notify server port at " + port);
            tcp.Start();
            new System.Threading.Thread(ServerWork).Start();
            new System.Threading.Thread(NotifyWork).Start();
         //   new System.Threading.Thread(NotifyWork).Start();
         //   ConsoleServer.WriteLine("two notify server started!");
             tmrClearConnecttion = new System.Timers.Timer(60 * 1000);
             tmrClearConnecttion.Elapsed+=new System.Timers.ElapsedEventHandler(tmrClearConnecttion_Elapsed);
             
             tmrClearConnecttion.Start();
            
          
        }

     void tmrClearConnecttion_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
     {
         //throw new Exception("The method or operation is not implemented.");
         this.NotifyAll(new NotifyEventObject(EventEnumType.TEST,"*",new object()));
         ConsoleServer.WriteLine("clear task finish ,num of nclient:" + clientStreamArray.Count);
     }

     System.Collections.Queue notifyQueue = System.Collections.Queue.Synchronized(new System.Collections.Queue());


     void NotifyWork()
     {
         NotifyEventObject notifyObj;
         System.Collections.Stack stack = new System.Collections.Stack();
         while (true)
         {

             try
             {
                  
                    
                         if (notifyQueue.Count == 0)
                         {
                             lock (notifyQueue)
                             {
                                 System.Threading.Monitor.Wait(notifyQueue);
                             }
                         }
                         else
                         {
                             stack.Clear();
                             notifyObj = (NotifyEventObject)notifyQueue.Dequeue();
                             foreach (ClientConnection cn in clientStreamArray)
                             {
                                 try
                                 {

                                     if (cn.IsConnected)
                                     {
                                         cn.DoNotify((NotifyEventObject)notifyObj);
                                         // cnt++;
                                     }
                                     else
                                     {
                                         Console.WriteLine("client dead");
                                         stack.Push(cn);
                                     }


                                 }
                                 catch (Exception ex)
                                 {
                                     Console.WriteLine("client dead" + ex.Message);
                                     stack.Push(cn);

                                     //clientStreamArray.Remove(stream);
                                 }
                             }
                             while (stack.Count > 0)
                             {
                                 ClientConnection cc = (ClientConnection)stack.Pop();
                                 clientStreamArray.Remove(cc);
                                 cc.Dispose();
                             }
                         
                     }

            
                 //lock (clientStreamArray)
                 //{
                    
                 //}
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message + ex.StackTrace);
             }

         }
     }
        public void NotifyAll(RemoteInterface.NotifyEventObject notifyObj)
        {

          //  int cnt = 0;
            // 必要時解除註解
           // System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(DoNotifyAllWork));
          //  th.Start(notifyObj);
       
        
               notifyQueue.Enqueue(notifyObj);
               lock (notifyQueue)
               {
                  
                 System.Threading.Monitor.Pulse(notifyQueue);
               }
     
           // return cnt;

        }

        public bool IsRegistered(string DeviceName)
        {
          
            foreach (ClientConnection cn in clientStreamArray.ToArray())
            {
                foreach (NotifyEventObject regobj in cn.Register.ToArray())
                {
                    if (regobj.deviceName == DeviceName)
                        return true;
                }
            }
            return false;
        }


        //void DoNotifyAllWork(Object objSerial)
        //{
        //    System.Collections.Stack stack = new System.Collections.Stack();
        //    //lock (clientStreamArray)
        //    //{
        //        foreach (ClientConnection cn in clientStreamArray)
        //        {
        //            try
        //            {
        //                if(cn.IsConnected)
        //                  cn.DoNotify((NotifyEventObject)objSerial);
        //                else
        //                {
        //                     Console.WriteLine("client dead");
        //                     stack.Push(cn);
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine("client dead" + ex.Message);
        //                stack.Push(cn);
        //                //clientStreamArray.Remove(stream);
        //            }
        //        }
        //        while (stack.Count > 0)
        //        {
        //            ClientConnection cc =(ClientConnection) stack.Pop();
        //            clientStreamArray.Remove(cc);
        //            cc.Dispose();
        //        }
        //    //}
        //}


     private void ServerWork()
     {
        
         while (true)
         {
             try
             {
                 TcpClient client = tcp.AcceptTcpClient();
                 //lock (clientStreamArray)
                 //{
                     clientStreamArray.Add(new ClientConnection(client));
                     ConsoleServer.WriteLine("Accept nocify client total cnt:"+((IPEndPoint)client.Client.RemoteEndPoint).Address+"," +clientStreamArray.Count);
                 //}
             }
             catch (Exception ex)
             {
                 ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
             }
         }
     }



        
    }

    class ClientConnection : IDisposable
    {
         public System.Collections.ArrayList Register=System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList());
        
        TcpClient tcp;
        System.IO.Stream stream;
        

        public bool IsConnected
        {
            get
            {
                 
                return tcp.Connected;
            }
        }
        public ClientConnection(TcpClient tcp)
        {
            this.tcp = tcp;
            this.stream =tcp.GetStream();
            new System.Threading.Thread(ReceiveWork).Start();
        }

        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bFormat = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        public void DoNotify(NotifyEventObject obj)
        {   //don't try catch here
            //  System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bFormat = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            foreach (NotifyEventObject regobj in Register)
            {
                lock (stream)
                {
                    if (obj.type == EventEnumType.TEST)
                    {
                        bFormat.Serialize(stream, obj);
                        stream.Flush();
                    }

                    if (obj.type == regobj.type)
                        if (regobj.deviceName == "*" || regobj.deviceName == obj.deviceName)
                        {
                            // if (regobj.port == -1 || regobj.port == obj.port)
                            bFormat.Serialize(stream, obj);
                            stream.Flush();

                        }
                }
               
            }

        }

        public void ReceiveWork()
        {
            NotifyEventObject obj;
           //   System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bFormat = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            while (true)
            {
                try
                {
                    obj = (NotifyEventObject)bFormat.Deserialize(stream);

                    Register.Add(obj);

                }
                catch 
                {
                    break;
                }
            }
        }






     

        public void Dispose()
        {
            this.tcp.Close();
            this.stream.Close();
            this.stream.Dispose();
            this.Register.Clear();
            this.Register = null;
            //throw new Exception("The method or operation is not implemented.");
        }

      
    }

    
}
