using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using System.Diagnostics;

namespace ProcessManager
{
    class Program
    {

      public   static System.Collections.Hashtable hash_process = new System.Collections.Hashtable();

      public   static Ds ds=new Ds();

        static RemoteObject remoteobj = new RemoteObject();
        static void Main(string[] args)
        {
            System.Collections.ArrayList ary=new System.Collections.ArrayList();
            RemoteInterface.ServerFactory.SetChannelPort((int)RemoteInterface.RemotingPortEnum.ProcessManager);
            RemoteInterface.ServerFactory.RegisterRemoteObject(typeof(RemoteObject), RemotingPortEnum.ProcessManager.ToString());
            string processStr= RemoteInterface.Util.CPath(Environment.CurrentDirectory + @"\process.txt");
            
           
            System.IO.StreamReader sr = new System.IO.StreamReader(processStr,System.Text.Encoding.GetEncoding("big5"));
           
           
            while (!sr.EndOfStream)
            {
                string[] strs=sr.ReadLine().Split(new char[]{','});
               if (strs.Length != 4) continue;
              // ds.tblProcessInfo.AddtblProcessInfoRow(strs[0], System.Convert.ToInt32(strs[1]), strs[2], true);
               ary.Add(new ProcessWrapper(strs[0],System.Convert.ToInt32(strs[1]),strs[2],strs[3]));
               
                 
            }

            foreach(ProcessWrapper pw in ary  )
            {
                try
                {
                 // System.Diagnostics.ProcessStartInfo info=new ProcessStartInfo(
                  Process p=  Process.Start(pw.ExecutingStr);
                  p.EnableRaisingEvents = true;
                  pw.Process = p;  
               //   p.Exited += new EventHandler(p_Exited);
                  hash_process.Add(pw.PName,pw);
                  System.Threading.Thread.Sleep(1000 * 10);
                 
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            System.Threading.Thread th = new System.Threading.Thread(CheckProcessWork);
            th.Start();
          //  Util.SetSysTime(2009, 7, 7, 7, 7, 7);
           // System.Console.ReadKey();
            //object obj=new object();
            //lock (obj)
            //{
            //    System.Threading.Monitor.Wait(obj);
            //}

         //  System.Threading.Monitor.Wait(
            
        }

        static void CheckProcessWork()
        {
            while (true)
            {
                try
                {
                    foreach (ProcessWrapper pw in hash_process.Values)
                    {
                        try
                        {

                            pw.Process.Refresh();

                            if (pw.Process.HasExited  && pw.state==1 )
                            {
                                if (pw.Startcnt <5)
                                {
                                    pw.Process.Start();
                                    Program.AddErrLog(pw.PName + " restart!");
                                    pw.Startcnt = 0;
                                }
                                else
                                {
                                    Program.AddErrLog(pw.PName+"start fail after 5 tries !");
                                }
                            }
                        }

                        catch (Exception ex)
                        {
                            Program.AddErrLog(ex.Message);
                            pw.Startcnt++;
                        }
                      
                       
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                //if (remoteobj.IsAllProcessOk())

                //    System.Environment.SetEnvironmentVariable("MFCC_IS_ALIVE", "Y");
                    
                //else
                //    System.Environment.SetEnvironmentVariable("MFCC_IS_ALIVE", "N");   

                    System.Threading.Thread.Sleep(2000);
            }

        }

      static   public  void AddErrLog( string mesg)
        {
            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "process_err.log", System.DateTime.Now + "," + mesg+"\r\n");
        }
        //static void p_Exited(object sender, EventArgs e)// 重啟動機制
        //{
        //    Process p = (Process)sender;
        //    try
        //    {
        //        p.Start();
        //        Console.WriteLine(p.ToString() + "exit!  restart...");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //   // throw new Exception("The method or operation is not implemented.");
        //}
    }
}
