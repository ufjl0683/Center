using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using System.Data;
namespace Comm.MFCC
{

    public delegate void OnTC_HWStatusCHangedHandler(object tcobj,byte[]diff,byte opmode,byte opstatus);
    public delegate void OnTC_ConnectStatusChangedHandler(object tcobj);
  public   class TC_Manager
    {

      System.Collections.Hashtable hs_tc_name =System.Collections.Hashtable.Synchronized( new System.Collections.Hashtable());
      System.Collections.Hashtable hs_tc_ip_Port =System.Collections.Hashtable.Synchronized( new System.Collections.Hashtable());

      public event OnTC_HWStatusCHangedHandler OnTCHWStausChanged;
      public event OnTC_ConnectStatusChangedHandler OnTCConnectStatusChanged;
      public TC_Manager(System.Collections.ArrayList devAry)
      {

          foreach (TCBase tc in devAry)
          {
              hs_tc_ip_Port.Add(TC_Manager.getIP_PortKey(tc.IP,tc.port),tc);
              hs_tc_name.Add(tc.DeviceName, tc);
              tc.OnHwStatusChanged += new HWStatusChangeHandler(tc_OnHwStatusChanged);
              tc.OnConnectStatusChanged += new ConnectStatusChangeHandler(tc_OnConnectStatusChanged);
          }

        }

      public TC_Manager()
      {
      }

      public System.Collections.IEnumerable GetDevEnum()
      {

         return   hs_tc_name.Values;
          
      }
      public void Remove(string devName)
      {
         
         TCBase tc=(TCBase)hs_tc_name[devName];

         hs_tc_ip_Port.Remove(TC_Manager.getIP_PortKey(tc.IP,tc.port));
         hs_tc_name.Remove(tc.DeviceName);
          try{
          tc.Close();
          }catch{;}
      }
      public void AddTC(TCBase tc)
      {
          if(!this.hs_tc_name.Contains(tc.DeviceName))
          {
              hs_tc_name.Add(tc.DeviceName, tc);
              hs_tc_ip_Port.Add(TC_Manager.getIP_PortKey(tc.IP, tc.port), tc);

          }
      }
      void tc_OnConnectStatusChanged(object tc)
      {
         // throw new Exception("The method or operation is not implemented.");
          if (this.OnTCConnectStatusChanged != null)
              this.OnTCConnectStatusChanged(tc);
      }

      void tc_OnHwStatusChanged(object tcobj, byte[] diff,byte opmode,byte opstatus)
      {

          if (this.OnTCHWStausChanged != null)
              this.OnTCHWStausChanged(tcobj, diff,opmode,opstatus);

        //TCBase tc= (TCBase)tcobj;
        //I_HW_Status_Desc desc = tc.getStatusDesc();
        // System.Collections.IEnumerator ie= desc.getEnum(diff).GetEnumerator();
        // ConsoleServer.WriteLine(tc.ToString());
        // while (ie.MoveNext())
        // {
        //     ConsoleServer.WriteLine(ie.Current.ToString() + ":"+desc.getStatus((int)ie.Current));
        // }
          
         // ConsoleServer.WriteLine(tc.ToString()+":"+);
        //w new Exception("The method or operation is not implemented.");
      }

      public TCBase   this[string  devname]
      {
          get
          {   
              
              return (TCBase)hs_tc_name[devname];
          }
      }

      public bool IsContains(string devname)
      {
          return this.hs_tc_name.Contains(devname);
      }

      public TCBase this[string ip, int port]
      {
          get
          {
              return (TCBase)hs_tc_ip_Port[getIP_PortKey(ip,port)];
          }
        
      }

      public static string getIP_PortKey(string ip,int port)
      {
          return ip + string.Format("{0:00000}", port);
      }

    
    }
}
