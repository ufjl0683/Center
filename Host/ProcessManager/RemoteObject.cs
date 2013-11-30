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
                        if (!p.Process.HasExited)
                        {
                            p.Process.Refresh();
                            ds.tblProcessInfo.AddtblProcessInfoRow(p.PName, p.ConsolePort, p.ExecutingStr, p.state, p.Process.TotalProcessorTime.TotalSeconds, p.Process.VirtualMemorySize64, !p.Process.HasExited, p.pid,p.Pdesc);

                        }
                        else
                        {
                            ds.tblProcessInfo.AddtblProcessInfoRow(p.PName, p.ConsolePort, p.ExecutingStr, p.state, -1, -1, !p.Process.HasExited, p.pid,p.Pdesc);
                        }

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
           

              if ((state==0 || state==2) && !p.Process.HasExited)  //手動關閉
              {
                  p.Process.Kill();
                  p.state = state;
              }
              else if (state == 1 && p.Process.HasExited)
              {

                  p.state = state;
                  p.Startcnt = 0;
                  p.bManual = true;

              }
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
                      if (pw.Process.HasExited  && pw.Startcnt>=5 && pw.state!=1 )
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
