using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace RemoteInterface
{
   public  class ConsoleServer
    {
       
            static TcpListener tcpServer;
            static System.Collections.ArrayList TcpClients = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
      //  static System.IO.MemoryStream ms = new System.IO.MemoryStream();
          
          public  static void Start(int port)
            {
                
                    tcpServer = new TcpListener(System.Net.IPAddress.Any, port);
                    Console.WriteLine("ConsoleServer Listen port:"+port);
               

                tcpServer.Start();
                new System.Threading.Thread(TerminalServertask).Start();

                new System.Threading.Thread(ClientListenTask).Start();
               
                

            }


       private static void ClientListenTask()
       {
           while (true)
           {
               try
               {
                   TcpClient tcp = tcpServer.AcceptTcpClient();
                   TcpClients.Add(tcp);
               }
               catch
               {
                   ;
               }
           }
       }

        static System.Collections.Queue outputQueue = new System.Collections.Queue(10);

        public static void Write(string s)
        {
            if (outputQueue.Count < 1000)
                outputQueue.Enqueue(s);

            lock (lockobj)
            {
                System.Console.Write(s);
                System.Threading.Monitor.PulseAll(lockobj);
            }
            
            
        }
     //  static int inx = 0;
       public static void WriteLine(string s)
       {
           try
           {
               if (outputQueue.Count < 1000)
                   outputQueue.Enqueue(s + "\r\n");


               lock (lockobj)
               {
                   System.Console.WriteLine(s);
                   System.Threading.Monitor.PulseAll(lockobj);
               }
           }
           catch
           { ;}

       }


      static  object lockobj = new object();
        private    static void TerminalServertask()
            {

                System.Collections.Queue deadTCPque = System.Collections.Queue.Synchronized(new System.Collections.Queue());
              //System.Console.Out
                while (true)
                {

                    try
                    {
                        lock (lockobj)
                        {
                            if (outputQueue.Count == 0)
                                System.Threading.Monitor.Wait(lockobj);
                        //    for (int i = 0; i < outputQueue.Count; i++)
                            while(outputQueue.Count>0)
                            {
                                string str = (string)outputQueue.Dequeue();
                                byte[] data = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, System.Text.Encoding.Unicode.GetBytes(str));
                             
                                foreach (TcpClient tcp in TcpClients)
                                {
                                    try
                                    {
                                        if (tcp.Connected)
                                        {
                                            //System.IO.StreamWriter sw = new System.IO.StreamWriter(tcp.GetStream(), System.Text.Encoding.Unicode);
                                          //  Console.Write("[" + str + "]");
                                            tcp.GetStream().Write(data,0,data.Length);
                                            tcp.GetStream().Flush();
                                            
                                           // sw.Write(str);
                                            ///sw.Flush();
                                        }
                                        else
                                            deadTCPque.Enqueue(tcp);
                                    }
                                    catch (Exception)
                                    {
                                        
                                        deadTCPque.Enqueue(tcp);
                                        
                                    }
                                }
                            }

                        } // lock


                        for (int i = 0; i < deadTCPque.Count; i++)
                        {
                            TcpClient client = (TcpClient)deadTCPque.Dequeue();
                            TcpClients.Remove(client);
                            client.Close();
                            client = null;

                        }

                    }
                    catch
                    {
                        ;
                    }
                }
            }
        }
    
}
