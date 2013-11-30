using System;
using System.Collections.Generic;

using System.Text;
using Comm;
using RemoteInterface;
namespace DigitTime
{
    class Program
    {
       
        static void Main(string[] args)
        {
             Protocol protocol;
             string ip;
             int port;
             V2DLE dle;
             System.Net.Sockets.TcpClient tcp;
            protocol = new Protocol();
            protocol.Parse(System.IO.File.ReadAllText(Protocol.CPath(AppDomain.CurrentDomain.BaseDirectory+"TIME.txt")),false);
            ip = protocol.ip;
            port = protocol.port;
           
            while (true)
            {
                try
                {
                    bool isCommErr = false;
                    tcp = new System.Net.Sockets.TcpClient();
                    tcp = ConnectTask(ip, port);
                    dle = new V2DLE("DigitTimer", tcp.GetStream());
                    dle.OnCommError+=(s,a)=>
                        {
                            isCommErr = true;
                            dle.Close();
                        };
                    while (!isCommErr)
                    {
                       
                        System.Data.DataSet ds = protocol.GetSendDataSet("report_system_time");
                        SendPackage pkg = protocol.GetSendPackage(ds, 0xffff);
                        pkg.cls = CmdClass.A;
                        dle.Send(pkg);
                        if (pkg.result == CmdResult.ACK)
                        {
                            System.Data.DataSet retDs = protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
                            int yr,mon,dy,hr,min,sec;
                            yr=System.Convert.ToInt32(retDs.Tables[0].Rows[0]["year"]);
                            mon = System.Convert.ToInt32(retDs.Tables[0].Rows[0]["month"]);
                            dy = System.Convert.ToInt32(retDs.Tables[0].Rows[0]["day"]);
                            hr = System.Convert.ToInt32(retDs.Tables[0].Rows[0]["hour"]);
                            min = System.Convert.ToInt32(retDs.Tables[0].Rows[0]["minute"]);
                            sec = System.Convert.ToInt32(retDs.Tables[0].Rows[0]["second"]);
                             DateTime dt = new DateTime(yr, mon, dy, hr, min, sec);
                             Console.WriteLine(dt.ToString());
                             RemoteInterface.Util.SetSysTime(dt);
                        }
                        else
                        {
                            Console.WriteLine(pkg.result.ToString());
                        }
                       

                        System.Threading.Thread.Sleep(1000 * 60 );
                    }
                }
                catch (Exception ex)
                {
                    ;
                }
            }

        }

        static System.Net.Sockets.TcpClient  ConnectTask(string ip,int port)
        {
            System.Net.Sockets.TcpClient tcp;
            while (true)
            {
                try
                {
                    Console.WriteLine("try connect to {0}:{1}......", ip, port);
                    tcp = new System.Net.Sockets.TcpClient();
                    tcp.Connect(ip, port);
                    Console.WriteLine(" {0}:{1} Connected!", ip, port);
                    return tcp;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    System.Threading.Thread.Sleep(1000 * 5);
                  //  Console.WriteLine("retr connect to {0}:{1}!", ip, port);
                }

            }

        }
    }
}
