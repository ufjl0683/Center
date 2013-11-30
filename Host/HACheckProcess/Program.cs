using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace HACheckProcess
{
    class Program
    {
        static void Main(string[] args)
        {
         try{
                 I_ProcessManager robj= (I_ProcessManager)RemoteBuilder.GetRemoteObj(typeof(I_ProcessManager),RemoteBuilder.getRemoteUri("127.0.0.1",(int)RemotingPortEnum.ProcessManager,RemotingPortEnum.ProcessManager.ToString()));

                 System.Data.DataTable tbl = robj.getProcessStatus();
                 foreach (System.Data.DataRow r in tbl.Rows)
                 {
                     for (int i = 0; i < tbl.Columns.Count; i++)
                     {
                         Console.Write(tbl.Columns[i].ColumnName + ":" + r[i] + " ");

                     }
                     Console.WriteLine();
                 }
                 if (robj.IsAllProcessOk())
                     System.Environment.Exit(0);
                 else
                     System.Environment.Exit(-1);
            }
          catch(Exception ex)
          {
                Console.WriteLine(ex.Message);
                System.Environment.Exit(-1);
          }
            


        }
    }
}
