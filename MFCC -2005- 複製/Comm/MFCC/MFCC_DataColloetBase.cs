using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace Comm.MFCC
{
   public abstract class MFCC_DataColloetBase : MFCC_Base
    {

     protected  System.Collections.Queue InDbQueue = System.Collections.Queue.Synchronized(new System.Collections.Queue());
      public  MFCC_DataColloetBase(string mfccid, string devType, int remotePort, int notifyPort, int consolePort, string regRemoteName, Type regRemoteType)
           : base(mfccid, devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
       {
          // new System.Threading.Thread(WriteDbTask).Start();
       }


       //void WriteDbTask()
       //{
       //    System.Data.Odbc.OdbcConnection cn;
       //    cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
       //    System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();
       //    cmd.Connection = cn;

       //    while (true)
       //    {


       //        try
       //        {

       //            lock (InDbQueue)
       //            {
       //                if (InDbQueue.Count == 0)
       //                    System.Threading.Monitor.Wait(InDbQueue);
       //            }

       //            cn.Open();
       //            while (InDbQueue.Count != 0)
       //            {
       //              //  Comm.I_DB_able data = (Comm.I_DB_able)InDbQueue.Dequeue();
       //                string data = InDbQueue.Dequeue().ToString();
       //                cmd.CommandText = data; // data.getExecuteSql();
       //                cmd.ExecuteNonQuery();


       //            }


       //        }
       //        catch (Exception ex)
       //        {
       //            ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
       //        }
       //        finally
       //        {
       //            try
       //            {
       //                cn.Close();
       //            }
       //            catch { ;}

       //        }
       //        ConsoleServer.WriteLine("One min vd data to db completed!!");

       //    }  //while


       //}
       //protected void ToDb(string sqlstr)
       //{
       //    this.InDbQueue.Enqueue(sqlstr);
       //    lock (this.InDbQueue)
       //    {
       //        System.Threading.Monitor.PulseAll(InDbQueue);
       //    }

       //}

    }
}
