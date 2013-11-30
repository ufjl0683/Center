using System;
using System.Collections.Generic;
using System.Text;
using Comm;
using RemoteInterface.HWStatus;

namespace QYTask
{
  public   class RGS_Manager
    {

       System.Collections.Hashtable RGS_tcs = new System.Collections.Hashtable();
      Comm.Protocol rgs_protocol = new Comm.Protocol();
      

      public RGS_Manager()
      {

          establish_RGS_TC();
      }
      void establish_RGS_TC()
      {
          rgs_protocol.Parse(System.IO.File.ReadAllText(Util.CPath(AppDomain.CurrentDomain.BaseDirectory+"RGS.txt")), false);
          lock (QYTask.Program.RGSConfDs.tblRGSMain)
          {

              foreach (QYTask.Ds.tblRGSMainRow r in Program.RGSConfDs.tblRGSMain.Rows)
              {
                  Comm.TC.RGSTC tc = new Comm.TC.RGSTC(rgs_protocol, r.rgs_name, r.ip, (int)r.port, r.deviec_id);
                  //AD HWSTATUS EVENT HERE!
                  tc.OnConnectStatusChanged += new Comm.ConnectStatusChangeHandler(tc_OnConnectStatusChanged);
                  tc.OnHwStatusChanged += new Comm.HWStatusChangeHandler(tc_OnHwStatusChanged);
                  RGS_tcs.Add(r.ip, tc);

              }
          }

      }
      

      public int Count
      {
          get
          {
              return RGS_tcs.Count;
          }
      }


      public System.Collections.IEnumerator getEnum()
      {
          foreach (Comm.TCBase tc in RGS_tcs.Values)
          {
              yield return tc;
          }
      }

      public Comm.TC.RGSTC this[string ip]
      {
          get
          {
              return (Comm.TC.RGSTC)RGS_tcs[ip];
          }
      }

      

      void tc_OnHwStatusChanged(object sender, byte[] diff)
      {
         
          Comm.TCBase tc = (Comm.TCBase)sender;
          RGS_HW_StatusDesc status_desc = new RGS_HW_StatusDesc(tc.getHwStaus(), diff);
          if(Program.eventDispatcher!=null)
          Program.eventDispatcher.NotifyAll(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.RGS_HW_Status_Event,tc.IP,
             status_desc));

          lock (QYTask.Program.RGSConfDs.tblRGSMain)
          {
          QYTask.Ds.tblRGSMainRow r=(QYTask.Ds.tblRGSMainRow)Program.ip_hash[tc.IP];// Program.RGSConfDs.tblRGSMain.FindByip(tc.IP);
        
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
                         r[((RGS_HW_Status_Bit_Enum)statusBit).ToString()] = status_desc.getStatus(statusBit);

                 }
                 r.EndEdit();
                 r.AcceptChanges();
             }

             if (Program.eventDispatcher != null)
          Program.eventDispatcher.NotifyAll(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.RGS_HW_Status_Event,tc.IP,status_desc));
          
      }

      void tc_OnConnectStatusChanged(object tcsender)
      {
          Comm.TCBase tc = (Comm.TCBase)tcsender;
          try
          {
              lock (QYTask.Program.RGSConfDs.tblRGSMain)
              {
                  QYTask.Ds.tblRGSMainRow r = (QYTask.Ds.tblRGSMainRow)Program.ip_hash[tc.IP];// Program.RGSConfDs.tblRGSMain.FindByip(tc.IP);

                  if (r != null)
                  {
                      r["connected"] = tc.IsConnected;
                      r.EndEdit();
                      r.AcceptChanges();

                  }
              }

              if (Program.eventDispatcher != null)

              Program.eventDispatcher.NotifyAll(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.RGS_Connection_Event, tc.IP, tc.IsConnected));
          }
          catch (Exception ex)
          {
              Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
          }
          
          
         
      }
    }
}
