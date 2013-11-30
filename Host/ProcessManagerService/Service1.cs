using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
//using System.Linq;
using System.ServiceProcess;
using System.Text;
using RemoteInterface;

namespace ProcessManagerService
{
    public partial class Service1 : ServiceBase
    {
        public static  RemoteInterface.HC.I_HC_Comm rhost = null;
        public static System.Collections.Hashtable hash_process = new System.Collections.Hashtable();

        public static Ds ds = new Ds();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Collections.ArrayList ary = new System.Collections.ArrayList();
            RemoteInterface.ServerFactory.SetChannelPort((int)RemoteInterface.RemotingPortEnum.ProcessManager);
            RemoteInterface.ServerFactory.RegisterRemoteObject(typeof(RemoteObject), RemotingPortEnum.ProcessManager.ToString());
            string processStr = RemoteInterface.Util.CPath(AppDomain.CurrentDomain.BaseDirectory + @"process.txt");


            System.IO.StreamReader sr = new System.IO.StreamReader(processStr, System.Text.Encoding.GetEncoding("big5"));


            while (!sr.EndOfStream)
            {
                string[] strs = sr.ReadLine().Split(new char[] { ',' });
                if (strs.Length != 4) continue;
                // ds.tblProcessInfo.AddtblProcessInfoRow(strs[0], System.Convert.ToInt32(strs[1]), strs[2], true);
                ary.Add(new ProcessWrapper(strs[0], System.Convert.ToInt32(strs[1]), strs[2], strs[3]));


            }

            foreach (ProcessWrapper pw in ary)
            {
                try
                {
                    // System.Diagnostics.ProcessStartInfo info=new ProcessStartInfo(
                    Process p = Process.Start(pw.ExecutingStr);
                    p.EnableRaisingEvents = true;
                    pw.Process = p;
                    //   p.Exited += new EventHandler(p_Exited);
                    hash_process.Add(pw.PName, pw);
                    System.Threading.Thread.Sleep(1000 * 10);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            System.Threading.Thread th = new System.Threading.Thread(CheckProcessWork);
            th.Start();
          
            

        }

        static void HostExecuteSQLCmd(string sql)
        {
            try
            {
                if (rhost == null)
                {
                    rhost = (RemoteInterface.HC.I_HC_Comm)RemoteInterface.RemoteBuilder.GetRemoteObj(
                        typeof(RemoteInterface.HC.I_HC_Comm), RemoteInterface.RemoteBuilder.getRemoteUri(RemoteInterface.RemoteBuilder.getHostIP(), (int)RemotingPortEnum.HOST, "Comm"));
                }
                rhost.dbExecute(sql);

            }
            catch (Exception ex)
            {
                ;
            }



        }


        static void CheckProcessWork()
        {
            string sqlstr = "insert into tblmfccstatelog (mfccid,conn_state,timestamp) values('{0}',{1},'{2}')";
            while (true)
            {
                try
                {
                    foreach (ProcessWrapper pw in hash_process.Values)
                    {
                        try
                        {

                            pw.Process.Refresh();

                            if (pw.Process.HasExited && pw.state == 1)
                            {
                                if (pw.Startcnt < 5)
                                {
                                    pw.Process.Start();
                                    Service1.AddErrLog(pw.PName + " restart!");
                                    if (!pw.bManual)
                                    {
                                        HostExecuteSQLCmd(string.Format(sqlstr, pw.PName, 2, DbCmdServer.getTimeStampString(DateTime.Now)));
                                        //   dbserver.SendSqlCmd(string.Format(sqlstr, pw.PName, 2, DbCmdServer.getTimeStampString(DateTime.Now)));
                                    }
                                    pw.bManual = false;
                                    // pw.Startcnt = 0;
                                }
                                else
                                {
                                    pw.Process.Start();
                                    Service1.AddErrLog(pw.PName + "start fail after 5 tries !");
                                }
                                pw.Startcnt++;
                            }
                        }

                        catch (Exception ex)
                        {
                            Service1.AddErrLog(ex.Message);
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

                System.Threading.Thread.Sleep(10000);
            }

        }

        static public void AddErrLog(string mesg)
        {
            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "process_err.log", System.DateTime.Now + "," + mesg + "\r\n");
        }

        protected override void OnStop()
        {
        }
    }
}
