using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;


namespace RemoteInterface
{
    
    public  class DbCmdServer
    {
        //int transfercnt = 0;
        //int translimit = 1000;
        const int ThreadCnt = 2;
        public int MAX_QUEUE_CNT = 5000;
        System.Collections.Queue sqlQueue = System.Collections.Queue.Synchronized(new System.Collections.Queue(100));
        volatile int processcnt = 0;
        object sqlLockObj = new object();
        bool isPrint = false;
        System.Timers.Timer tmrMoniter = new System.Timers.Timer(1000 * 60);
        System.Threading.Thread[] th = new System.Threading.Thread[ThreadCnt];
        DBQueueMode dbqmode = DBQueueMode.Normal;
        int[] state = new int[ThreadCnt];
        string[] lastcmd = new string[ThreadCnt];
        bool IsStart = false;
       volatile  int ProcessCntPerMin = 0;
        volatile int errcnt = 0;
        public DbCmdServer()
        {


          //  Util.Log( Util.CPath(AppDomain.CurrentDomain.BaseDirectory+"dberr.log"),DateTime.Now+",dbserver started!\r\n");

            for (int i = 0; i < ThreadCnt; i++)
            {
               // System.Threading.Thread th;
                th[i] = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(DbSQLExecute_Task));
              //  th.Priority = System.Threading.ThreadPriority.Highest;
                th[i].Start(i);
            }

            tmrMoniter.Elapsed += new System.Timers.ElapsedEventHandler(tmrMoniter_Elapsed);
            tmrMoniter.Start();
            IsStart = true;
        }


        public void SetDbQueueMode(DBQueueMode mode)
        {
            this.dbqmode = mode;
        }
        void tmrMoniter_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            if ( sqlQueue.Count> MAX_QUEUE_CNT || errcnt>1000)
            {
                if(sqlQueue.Count> MAX_QUEUE_CNT)
                    RemoteInterface.Util.SysLog("dberr.log", string.Format("  quecnt>{0} , System exit!",MAX_QUEUE_CNT));
                if( errcnt>1000)
                    RemoteInterface.Util.SysLog("dberr.log", "  errcnt>1000 , System exit!"  );
                System.Environment.Exit(-1);
            }
            if (sqlQueue.Count > 500  || errcnt>100)
            {
                isPrint = true;
            }
            else
                isPrint = false;

                for (int i = 0; i < ThreadCnt; i++)
                {
                    ConsoleServer.WriteLine("=============Thread" + i + "alive:" + th[i].IsAlive +   "state:" + state[i]+ "Dbq Cnt:" + this.sqlQueue.Count+"ProcessCntPerMin:"+ProcessCntPerMin+ "=================");
                   if(isPrint)
                    ConsoleServer.WriteLine("last cmd:" + lastcmd[i] + "," );
                 
                  //  ConsoleServer.WriteLine("transfercnt:" + transfercnt);
                }

                ProcessCntPerMin = 0;
              //  System.Console.WriteLine();
                //transfercnt+=
                //try
                //{
                //    if(sqlQueue.Count > translimit)
                //        for (int i = 0; i < ThreadCnt; i++)
                //        {
                //            try
                //            {
                //                th[i].Abort();
                //            }
                //            catch { ;}
                //        }
                //    for (int i = 0; i < ThreadCnt; i++)
                //    {
                //        th[i] = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(DbSQLExecute_Task));
                //        //  th.Priority = System.Threading.ThreadPriority.Highest;
                //        th[i].Start(i);
                //    }
                    //translimit += 1000;
                //    //transfercnt++;
                //}
                //catch (Exception ex)
                //{
                //    ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                //}
            //}
           
            
            
            //throw new Exception("The method or operation is not implemented.");
        }
      
          
       

        public int getCurrentQueueCnt()
        {
            return sqlQueue.Count;
        }

      static   public string getDbConnectStr()
        {
 
            return "dsn=TCS;uid=db2inst1;pwd=db2inst1";
 
        }

      static public string getSSHMC_DbConnectStr()
      {

          return "Data Source=192.192.161.2;Initial Catalog=SSHMC01;User ID=david;Password=ufjl0683@";
      }
      



        void DbSQLExecute_Task(object args)
        {

          //  bool isInLock = false;

#if DEBUG
            return;
#endif
            int inx = System.Convert.ToInt32(args);
            System.Data.Odbc.OdbcConnection cn;
            System.DateTime dt = System.DateTime.Now; ;
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();
            cmd.CommandTimeout = 120;
            cn = new System.Data.Odbc.OdbcConnection(DbCmdServer.getDbConnectStr());
            cmd.Connection = cn;
            cn.Open();
           
            ConsoleServer.WriteLine("Db task started!");
            state[inx] = 0;
            while (true)
            {
                try
                {


                             state[inx] = 1;
                          //  lock (sqlLockObj)
                          //  {
                                state[inx] = 2;
                                if (sqlQueue.Count == 0)
                                {
                                   if(isPrint)
                                    ConsoleServer.WriteLine("process cnt:" + processcnt);
                                   cn.Close();

                                    lock(this.sqlLockObj)
                                    {
                                        state[inx]=10;
                                     System.Threading.Monitor.Wait(sqlLockObj);
                                     state[inx] = 11;
                                    }
                                   
                                   cn.Open();
                                   processcnt = 0;
                                   
                                }
                                state[inx] = 3;
                            
                             //}



                         
                   
                     
                        while( sqlQueue.Count > 0)
                        {
                            try
                            {
                               
                              dt = System.DateTime.Now;
                                //lock (sqlLockObj)
                                //{
                                    state[inx] = 4;
                                    try
                                    {
                                        cmd.CommandText = System.Convert.ToString(sqlQueue.Dequeue());
                                    }
                                    catch { ;}
                                   
                                //}
                               
                                lastcmd[inx] = cmd.CommandText;
                                state[inx] = 9;
                                cmd.ExecuteNonQuery();
                                ProcessCntPerMin++;
                                processcnt++;

                                state[inx] = 5;
                                if (System.DateTime.Now - dt > new TimeSpan(0, 0, 30))
                                    ConsoleServer.WriteLine("db Executeion time longer than 30 sec:" + cmd.CommandText);
                                state[inx] = 6;
                                errcnt = 0;

                                if (dbqmode == DBQueueMode.Slow)  //2013/2/25
                                    System.Threading.Thread.Sleep(1000);
                              //  ConsoleServer.WriteLine("finish!");
                            }
                              
                            //catch (System.Data.Odbc.OdbcException odbcex)
                            //{
                                
                             
                            //      if(odbcex.ErrorCode!=-2147467259)  // repeat unixodbc32
                            //    {
                            //        ConsoleServer.WriteLine(odbcex.ErrorCode+","+odbcex.Message+cmd.CommandText);
                              
                                   
                            //    }
                            //      else
                            //        Console.WriteLine(odbcex.ErrorCode + "," + odbcex.Message);

                            //    try { cn.Close(); }
                            //    catch { ;}
                            //    try
                            //    {
                            //        cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
                            //        cmd.Connection = cn;
                            //        cn.Open();
                            //    }
                            //    catch { ;}

                            //}
                            catch (Exception ex1)
                            {

                                errcnt++;
                                //if(ex1 is System.Data.Odbc.OdbcException)
                                //   ConsoleServer.WriteLine("db exception:"+(ex1  as System.Data.Odbc.OdbcException).Message + cmd.CommandText);
                                //else
                                ConsoleServer.WriteLine("db exception:"+ex1.Message + cmd.CommandText);
                                RemoteInterface.Util.SysLog("dberr.log",System.DateTime.Now+","+ex1.Message+","+cmd.CommandText);
                              
                               
                                try {

                                   // if ( cn.State == System.Data.ConnectionState.Broken || cn.State == System.Data.ConnectionState.Closed )
                                   // {
                                        try
                                        {
                                            cn.Close();
                                        }
                                        catch { ;}
                                        try
                                        {
                                            cn.Open();
                                            cmd.Connection = cn;
                                        }
                                        catch { ;}
                                  //  }
                                
                                }
                                catch { ;}
                                //try
                                //{
                                // //   state[inx] = 7;

                                // ////   cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
                                // //   cmd.Connection = cn;
                                // //   cn.Open();
                                // //   state[inx] = 8;

                                //}
                                //catch (Exception ex){

                                //    ConsoleServer.WriteLine(" In DbSQLExecute_Task " + "," + ex.Message + "," + ex.StackTrace);
                                //}
                            }

                        }
                   
                   


                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                }
               

                



            }
        }

        public void SendCmdImmediately(string sqlCmd)
        {
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(DbCmdServer.getDbConnectStr());
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand(sqlCmd);

            cmd.Connection = cn;

            cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();

        }

        public void SendSqlCmd(string cmd)
        {

            if (dbqmode == DBQueueMode.Reject)
            {
                RemoteInterface.Util.SysLog("dbreject.log", cmd);
                return;
            }

            this.sqlQueue.Enqueue(cmd);
                lock (sqlLockObj)
                {
                  
                     
                     System.Threading.Monitor.Pulse(sqlLockObj);
                }

        }

        public static string getTimeStampString(System.DateTime dt)
        {
            return string.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}.000000", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second); ;
        }

        public static string getTimeStampString(int year, int month, int day, int hour, int min, int sec)
        {

            return string.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}.000000", year, month, day, hour, min, sec); ;

        }

    }
}
