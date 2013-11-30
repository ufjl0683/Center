using System;
using System.Collections.Generic;
using System.Text;
using Host.TC;
using RemoteInterface;

namespace Host
{
  public   class FiveMinTask
    {
      
   //   System.Timers.Timer tmr5MinTask = new System.Timers.Timer(1000 * 30);

   //   int last5minInx = -1;

      public FiveMinTask()
      {
        //  this.tmr5MinTask.Elapsed += new System.Timers.ElapsedEventHandler(tmr5MinTask_Elapsed);
       //   tmr5MinTask.Start();

          //System.DateTime dt = DateTime.Now.AddSeconds(-DateTime.Now.Second).AddMinutes(-DateTime.Now.Minute%5+5).AddSeconds(30); ;

          //int m_sec = (int)((TimeSpan)(dt - DateTime.Now)).TotalMilliseconds;
          //if (m_sec <= 0)
          //    m_sec = 1000;
          //System.Threading.Thread.Sleep(m_sec);
          new System.Threading.Thread(tmr5MinTask_Elapsed).Start();

      }

      void tmr5MinTask_Elapsed()
      {
          //if (System.DateTime.Now.Minute % 5 == 0 && System.DateTime.Now.Minute / 5 != last5minInx)
          //{
          //    System.DateTime timestamp=new DateTime(System.DateTime.Now.Year,System.DateTime.Now.Month,System.DateTime.Now.Day,
          //        System.DateTime.Now.Hour,System.DateTime.Now.Minute,0);
          //    last5minInx = System.DateTime.Now.Minute / 5;
          int HourCnt = 0;

          while (true)
          {
              HourCnt++;
              try
              {
                  if (HourCnt ==12)  // one hour write isconnected state of all device
                  {
                      foreach (DeviceBaseWrapper dev in Program.matrix.device_mgr.getAllDeviceEnum())
                      {
                          if (dev.IsConnected)
                              Program.matrix.dbServer.SendSqlCmd("update tblDeviceConfig set comm_state=1,update_time='" + RemoteInterface.DbCmdServer.getTimeStampString(System.DateTime.Now) + "' where devicename='" + dev.deviceName + "'");
                          else
                              Program.matrix.dbServer.SendSqlCmd("update tblDeviceConfig set comm_state=3,update_time='" + RemoteInterface.DbCmdServer.getTimeStampString(System.DateTime.Now) + "'   where devicename='" + dev.deviceName + "'");
                      }

                      HourCnt = 0;

                  }

                  try
                  {
                      // 更新所有設備硬體狀態
                      new System.Threading.Thread(UpdateAllDeviceStatusTask).Start();


                  }
                  catch { ;}

                  System.DateTime dt = DateTime.Now.AddSeconds(-DateTime.Now.Second).AddMinutes(-DateTime.Now.Minute % 5 + 5).AddSeconds(30); ;

                  int m_sec = (int)((TimeSpan)(dt - DateTime.Now)).TotalMilliseconds;
                  if (m_sec <= 0)
                      m_sec = 1000;
                  System.Threading.Thread.Sleep(m_sec);

              }
              catch { ;}

          }
             

         // }

          //throw new Exception("The method or operation is not implemented.");
      }


      public void UpdateAllDeviceStatusTask()
      {

      
          try
          {
              ConsoleServer.WriteLine("========In UpdateAllDevice Status  Task()==========");
           //   Util.Log(AppDomain.CurrentDomain.BaseDirectory + "uphwstatus.log", DateTime.Now + "begin!");
              foreach (DeviceBaseWrapper dev in Program.matrix.getDevicemanager().getAllDeviceEnum())
              {
                  try
                  {

                      dev.updateHW_Status();
                  }
                  catch (Exception ex)
                  {
                      ConsoleServer.WriteLine("UpdateAllDeviceTask:" + ex.Message);
                  }
              }
           //   Util.Log(AppDomain.CurrentDomain.BaseDirectory + "uphwstatus.log", DateTime.Now + "end!");

          }
          catch { ;}

      }


   


      //void TotblDegreeLogVD(System.DateTime dt)
      //{
      //    Host.TC.DevcieManager devMgr = Program.matrix.getDevicemanager();
      //    foreach (DeviceBaseWrapper dev in devMgr.getDataDeviceEnum())
      //    {

      //        string sqlstr;
      //        sqlstr = "insert into tblDegreeLogVD (devicename,timestamp,level) values('{0}', TIMESTAMP('{1}'),{2}) ";

      //        if (dev is VDDeviceWrapper)
      //        {
      //            try
      //            {
      //                VDDeviceWrapper vddev = (VDDeviceWrapper)dev;
      //                Program.matrix.dbServer.SendSqlCmd(string.Format(sqlstr, vddev.deviceName, RemoteInterface.DbCmdServer.getTimeStampString(dt),vddev.jamLevel));
      //            }
      //            catch (Exception ex)
      //            {
      //                ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
      //            }
                   
      //        }

      //    }

      //}


      //void TotblDegreeLogSection(System.DateTime dt)
      //{
        
      //    foreach (Line line in Program.matrix.line_mgr.getLineEnum())
      //    {

      //        foreach (Section section in line.getAllSectionEnum())
      //        {

      //            string sqlstr;
      //            sqlstr = "insert into tblDegreeLogSection (sectionid,timestamp,level) values('{0}', TIMESTAMP('{1}'),{2}) ";

                  
      //                try
      //                {
                         
      //                    Program.matrix.dbServer.SendSqlCmd(string.Format(sqlstr,section.sectionId, RemoteInterface.DbCmdServer.getTimeStampString(dt),section.getJamLevel()));
      //                }
      //                catch (Exception ex)
      //                {
      //                    ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
      //                }

                 
      //        }

      //    }

      //}
    }
}
