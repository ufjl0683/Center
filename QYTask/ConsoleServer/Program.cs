using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace ConsoleServer
{
    class Program
    {

        static TcpListener tcpServer;
        static System.Collections.ArrayList TcpClients = new System.Collections.ArrayList(10);
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                tcpServer = new TcpListener(System.Net.IPAddress.Any, 8000);
                Console.WriteLine("ConsoleServer Listen port:8000");
            }
            else
            {
                tcpServer = new TcpListener(System.Net.IPAddress.Any, System.Convert.ToInt32(args[0]));
                Console.WriteLine("ConsoleServer Listen port:"+args[0]);
            }

             tcpServer.Start();
             new System.Threading.Thread(TerminalServertask).Start();
             while (true)
             {

                 TcpClient tcp=  tcpServer.AcceptTcpClient();
                 TcpClients.Add(tcp);
             }
             

           
        }


        static void TerminalServertask()
        {

            System.Collections.Queue que = new System.Collections.Queue(10);
            while (true)
            {

                try
                {
                    string str = System.Console.ReadLine();
                    foreach (TcpClient tcp in TcpClients)
                    {
                        try
                        {
                            System.IO.StreamWriter sw = new System.IO.StreamWriter(tcp.GetStream(),System.Text.Encoding.Unicode);
                            sw.WriteLine(str);
                            sw.Flush();
                        }
                        catch (Exception)
                        {
                            que.Enqueue(tcp);
                        }
                    }


                    for (int i = 0; i < que.Count; i++)
                    {
                        TcpClient client = (TcpClient)que.Dequeue();
                        TcpClients.Remove(client);
                        client.Close();
                       
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
