using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.ServiceArea
{
 public   class ServiceAreaManager
    {

     RstRange RstEvent=null;

   
            System.Net.Sockets.TcpClient tcp;// = new System.Net.Sockets.TcpClient();
            private int _AvailableCnt = -1;
            public ServiceAreaManager()
            {
                new System.Threading.Thread(DoTask).Start();
            }
           

            public int AvailableCnt
            {
                get
                {
                    return _AvailableCnt;
                }
                set
                {
                    if (value != _AvailableCnt)
                    {
                        string sql = "insert into tblCingshueiServiceAreaData(ParkingArea,TimeStamp,Parking_Space) values('N','{0}' ,{1})";
                        Program.matrix.dbServer.SendSqlCmd(string.Format(sql,RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now),value));
                        if (value == 0 )  // 沒有空位
                        {
                            if(RstEvent==null)
                            {
                                RstEvent=new RstRange();
                                Program.matrix.event_mgr.AddEvent(RstEvent);
                            }
                        }
                        else  // 有空位
                        {
                            if(RstEvent != null)
                            {
                                RstEvent.invokeStop();
                                RstEvent=null;
                            }
                        }

                        _AvailableCnt = value;
                    }
                }
            }
            void DoTask()
            {

                while (true)
                {
                    try
                    {
                        Console.WriteLine("Connecting Service Area");
                        tcp = new System.Net.Sockets.TcpClient();
                        tcp.Connect("192.168.78.6", 1001);
                        System.IO.TextReader rd = new System.IO.StreamReader(tcp.GetStream());
                        while (true)
                        {
                            try
                            {
                                string s = rd.ReadLine();
                                

                                AvailableCnt = System.Convert.ToInt32(s.Substring(3, 3));


                                //  Console.WriteLine(AvailableCnt);
                            }
                            catch (System.IO.IOException)
                            {
                                break;
                            }
                            catch (Exception ex)
                            {
                                AvailableCnt = -1;
                           //     Console.WriteLine(ex.Message + "," + ex.StackTrace);
                            }

                        }

                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(5000);
                    }
                }

            }

        
    }
}
