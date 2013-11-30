using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using RemoteInterface;
 

namespace ProcessManager
{
  public   class RemoteObject:RemoteInterface.RemoteClassBase,RemoteInterface.I_ProcessManager
    {



      

       public DataTable getProcessStatus()
        {
            //lock (Program.ds)
            //{
                try
                {
                    ProcessManager.Ds ds = new Ds();
                    foreach ( ProcessWrapper p  in Program.hash_process.Values )
                    {
                        ds.tblProcessInfo.AddtblProcessInfoRow(p.PName, p.ConsolePort, p.ExecutingStr,p.state, p.Process.TotalProcessorTime.TotalSeconds,p.Process.VirtualMemorySize64,!p.Process.HasExited,p.pid);
                      // r.isAlive= !((ProcessWrapper)(Program.hash_process[p.process_name])).Process.HasExited;
                        
                        
                    }
                    ds.AcceptChanges();
                    return Util.getPureDataTable(ds.tblProcessInfo);
                }
                catch (Exception ex)
                {
                    throw new RemoteException(ex.Message);
                }

            //}
        }

      public void SetProcessRunningState(string pName,int state)
      {
          try
          {
              ProcessWrapper p = Program.hash_process[pName] as ProcessWrapper;
              p.state = state;
              if (p.state == 0 || p.state == 2)
                  p.Process.Kill();
          }
          catch (Exception ex)
          {
              throw new RemoteException(ex.Message);
          }

      }


    

      public bool IsAllProcessOk()
      {
          lock (Program.ds)
          {
              try
              {

                  foreach (Ds.tblProcessInfoRow r in Program.ds.tblProcessInfo.Rows)
                  {
                      ProcessWrapper pw=(ProcessWrapper)Program.hash_process[r.process_name];
                      if (pw.Process.HasExited  && pw.Startcnt>=5 )
                          return false;
                  }

                
              }
              catch (Exception ex)
              {
                  throw new RemoteException(ex.Message);
              }

              return true;
          }
      }








      


        public void setDateTime(DateTime dt)
        {
            Util.SetSysTime((ushort)dt.Year, (ushort)dt.Month, (ushort)dt.Day, (ushort)dt.Hour, (ushort)dt.Minute, (ushort)dt.Second);
            //throw new Exception("The method or operation is not implemented.");
        }

     
    }
}
