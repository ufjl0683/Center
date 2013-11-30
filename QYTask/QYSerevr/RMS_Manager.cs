using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HWStatus;

namespace QYTask
{
    class RMS_Manager
    {


         System.Collections.Hashtable RMS_tcs = new System.Collections.Hashtable();
      Comm.Protocol RMS_protocol = new Comm.Protocol();
     
      public RMS_Manager()
      {

          establish_RMS_TC();
      }
      void establish_RMS_TC()
      {
          RMS_protocol.Parse(System.IO.File.ReadAllText(Util.CPath(AppDomain.CurrentDomain.BaseDirectory+"RMS.txt")), false);
          lock (QYTask.Program.RMSConfDs.tblRmsConfig)
          {

              foreach (QYTask.Ds.tblRmsConfigRow r in Program.RMSConfDs.tblRmsConfig.Rows)
              {
                  Comm.TC.RMSTC tc = new Comm.TC.RMSTC(RMS_protocol, r.rms_name, r.ip, (int)r.port, 0xffff);
                  //AD HWSTATUS EVENT HERE!
                  tc.OnConnectStatusChanged += new Comm.ConnectStatusChangeHandler(tc_OnConnectStatusChanged);
                  tc.OnHwStatusChanged += new Comm.HWStatusChangeHandler(tc_OnHwStatusChanged);
                  RMS_tcs.Add(r.ip, tc);

              }
          }

      }

      public int Count
      {
          get
          {
              return RMS_tcs.Count;
          }
      }


       //public void SetModeAndPlane(byte mode,byte planno)
       //{


       //}

      public System.Collections.IEnumerator getEnum()
      {
          foreach (Comm.TCBase tc in RMS_tcs.Values)
          {
              yield return tc;
          }
      }

      public Comm.TC.RMSTC this[string ip]
      {
          get
          {
              return (Comm.TC.RMSTC)RMS_tcs[ip];
          }
      }

      void tc_OnHwStatusChanged(object sender, byte[] diff)
      {
         
          Comm.TCBase tc = (Comm.TCBase)sender;
           RMS_HW_StatusDesc status_desc = new RMS_HW_StatusDesc(tc.getHwStaus(), diff);
           if (Program.eventDispatcher != null)
          Program.eventDispatcher.NotifyAll(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.RMS_HW_Status_Event,tc.IP,
             status_desc));

          lock (QYTask.Program.RMSConfDs.tblRmsConfig)   
          {
          QYTask.Ds.tblRmsConfigRow r= Program.RMSConfDs.tblRmsConfig.FindByip(tc.IP);
        
             if (r == null) return;
            
                 r["hwstatus1"] = tc.getHwStaus()[0];
                 r["hwstatus2"] = tc.getHwStaus()[1];
                 r["hwstatus3"] = tc.getHwStaus()[2];
                 r["hwstatus4"] = tc.getHwStaus()[3];
                 r.EndEdit();
                 r.AcceptChanges();
                 System.Collections.IEnumerator ie = status_desc.getEnum().GetEnumerator();
                 while (ie.MoveNext())
                 {
                     int statusBit = (int)ie.Current;

                     if (statusBit == 0 || statusBit == 1 || statusBit == 8 || statusBit == 9 || statusBit == 10 || statusBit == 11)
                         r[((RMS_HW_Status_Bit_Enum)statusBit).ToString()] = status_desc.getStatus(statusBit);

                 }
                 r.EndEdit();
                 r.AcceptChanges();
             }

             if (Program.eventDispatcher != null)
          Program.eventDispatcher.NotifyAll(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.RMS_HW_Status_Event,tc.IP,status_desc));
          
      }

      void tc_OnConnectStatusChanged(object tcsender)
      {
          Comm.TCBase tc = (Comm.TCBase)tcsender;
          try
          {
              lock (QYTask.Program.RMSConfDs.tblRmsConfig)
              {
                  QYTask.Ds.tblRmsConfigRow r = Program.RMSConfDs.tblRmsConfig.FindByip(tc.IP);

                  if (r != null)
                  {
                      r["connected"] = tc.IsConnected;
                      r.EndEdit();
                      r.AcceptChanges();

                  }
              }


              if (Program.eventDispatcher != null)
              Program.eventDispatcher.NotifyAll(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.RMS_Connection_Event, tc.IP, tc.IsConnected));
          }
          catch (Exception ex)
          {
              Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
          }
          
          
         
      }


    }
}
