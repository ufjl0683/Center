using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using RemoteInterface;

namespace Host.TC
{
  public   class DevcieManager
    {
     
      MFCC.MFCC_Manager mfccmgr;
      public  bool IsInLoadWrapper = false;
   //   OdbcConnection cnDevice;
      System.Collections.Hashtable devices =System.Collections.Hashtable.Synchronized( new System.Collections.Hashtable());
      public DevcieManager(MFCC.MFCC_Manager mfccmgr)
      {
          
       
         this.mfccmgr = mfccmgr;
         IsInLoadWrapper = true;
        
             loadAllDeviceWrapper();
        
         IsInLoadWrapper = false;
      }


      public   DeviceBaseWrapper this[string deviceName]
      {
          get
          {
              deviceName = deviceName.Trim();
              if (devices.Contains(deviceName))
                  return (DeviceBaseWrapper)devices[deviceName];
              else
              {
                  if(IsInLoadWrapper)
                      throw new Exception("設備管理啟動中....請稍後再試!");
                  else
                        throw new Exception(deviceName+",not found!");
              }
          }
      }

      public  bool IsContainDevice(string devname)
      {

          return devices.Contains(devname);
      }

       public  System.Collections.IEnumerable getOutputDeviceEnum()
       {
        System.Collections.IEnumerator ie=this.devices.GetEnumerator();
        while (ie.MoveNext())
        {
            if (((System.Collections.DictionaryEntry)ie.Current).Value  is TC.OutPutDeviceBase)
                yield return ((System.Collections.DictionaryEntry)ie.Current).Value;
        }

       }


      public System.Collections.IEnumerable getAllDeviceEnum()
      {
         
          System.Collections.IEnumerator ie = this.devices.GetEnumerator();
          while (ie.MoveNext())
          {
             // if (!(((System.Collections.DictionaryEntry)ie.Current).Value is TC.OutPutDeviceBase))
                  yield return ((System.Collections.DictionaryEntry)ie.Current).Value;
          }

      }


      public System.Collections.IEnumerable getDataDeviceEnum()
      {
          System.Collections.IEnumerator ie = this.devices.GetEnumerator();
          while (ie.MoveNext())
          {
              if (!(((System.Collections.DictionaryEntry)ie.Current).Value is  TC.OutPutDeviceBase))
                  yield return ((System.Collections.DictionaryEntry)ie.Current).Value;
          }

      }
      public  RemoteInterface.MFCC.I_MFCC_Base getRemoteObject(string deviceName)
      {
          if (mfccmgr[((TC.DeviceBaseWrapper)this[deviceName]).mfccid] == null)
              return null;
          else
        return ( (MFCC.MFCC_Object) mfccmgr[((TC.DeviceBaseWrapper)this[deviceName]).mfccid]).getRemoteObject();
      }

   

      private void loadAllDeviceWrapper()
      {
          System.Collections.ArrayList threadAry = new System.Collections.ArrayList();
          System.Data.Odbc.OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
          System.Data.Odbc.OdbcCommand cmd = new OdbcCommand("select distinct mfccid from tblDeviceConfig",cn);
          System.Data.Odbc.OdbcDataReader rd;
        //  System.Collections.ArrayList TMPARY = new System.Collections.ArrayList();
         // System.Threading.ThreadPool.SetMaxThreads(300, 300);
          try
          {
              cn.Open();
              rd = cmd.ExecuteReader();
              while (rd.Read())
              {
                  string devType = rd[0].ToString().Trim();
                //  System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(loadDeviceWrapper),devType);
                  
                  System.Threading.Thread th=new System.Threading.Thread(loadDeviceWrapper);
                  th.Name = devType;
                  th.Start(devType);
                  threadAry.Add(th);
                //  TMPARY.Add(devType);
                  
              }
              rd.Close();

              foreach (System.Threading.Thread t in threadAry)
              {
                  System.Console.WriteLine("Waitting " + t.Name);
                  t.Join();
               //   TMPARY.Remove(t.Name);
              }
             

          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(ex.Message +","+ ex.StackTrace);
          }
          finally
          {
            
              cn.Close();
          }

      }



    private  void loadDeviceWrapper(object mfccid)
      {
          OdbcConnection cn;
          OdbcCommand cmd;
          OdbcDataReader rd=null;
          cn = new OdbcConnection(Global.Db2ConnectionString);
          cmd = new OdbcCommand();
          cmd.Connection = cn;
          try
          {
         //     Console.WriteLine("load " + mfccid.ToString() + "....");
              cn.Open();
#if !DEBUG 
              cmd.CommandText = "select devicename from tbldeviceconfig where mfccid='"+mfccid.ToString()+"'";
#else
              cmd.CommandText = "select devicename from tbldeviceconfig where    (  Lineid='N1'  )   and mfccid='" + mfccid.ToString() + "'  ";
             // cmd.CommandText = "select devicename from tbldeviceconfig where enable='Y' and mfccid='" + mfccid.ToString() + "'";
#endif
              rd = cmd.ExecuteReader();

              while (rd.Read())
              {
                  string devicename = rd[0] as string;
                 
                      this.AddDeviceWrapper(devicename, false, cn);
                 
                  ConsoleServer.Write(".");
                  /*
                  string devicetype = rd[1] as string;
                  string mfccid = rd[2] as string;
                  string location = rd[3] as string;
                  string lineid = rd[4] as string;
                  string direction = rd[5] as string;
                  int mile_m = System.Convert.ToInt32(rd[6]);
                  string ip = rd[7] as string;
                  int port = System.Convert.ToInt32(rd[8]);

                  byte[] hwstatus = new byte[4];
                  hwstatus[0]=System.Convert.ToByte(rd[9]);
                  hwstatus[1] = System.Convert.ToByte(rd[10]);
                  hwstatus[2] = System.Convert.ToByte(rd[11]);
                  hwstatus[3] = System.Convert.ToByte(rd[12]);
                  byte opmode = System.Convert.ToByte(rd[13]);
                  byte opstatus = System.Convert.ToByte(rd[14]); 

                  if (devicetype == "VD")
                  {
                      TC.VDDeviceWrapper wrapper = new TC.VDDeviceWrapper(mfccid, devicename, devicetype, ip, port, location,lineid, mile_m,hwstatus,opmode,opstatus);
                      devices.Add(devicename,wrapper);
                  }
                  else if (devicetype == "RGS")
                  {
                      TC.RGSDeviceWrapper wrapper = new TC.RGSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location,lineid, mile_m,hwstatus,opmode,opstatus);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "CMS")
                  {
                    
                      TC.CMSDeviceWrapper wrapper = new TC.CMSDeviceWrapper(mfccid, devicename, devicetype, ip, port,location, lineid, mile_m,hwstatus,opmode,opstatus);
                      //devices.Add(devicename, new TC.CMSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m));
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "RMS")
                  {
                      TC.RMSDeviceWrapper wrapper = new TC.RMSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m,hwstatus,opmode,opstatus);
                    //  devices.Add(devicename, new TC.RMSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m));
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "WIS")
                  {
                      TC.WISDeviceWrapper wrapper = new TC.WISDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m,hwstatus,opmode,opstatus);
                     // devices.Add(devicename, new TC.WISDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m));
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "LCS")
                  {
                      TC.LCSDeviceWrapper wrapper = new TC.LCSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m,hwstatus,opmode,opstatus);
                     // devices.Add(devicename, new TC.LCSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m));
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "CSLS")
                  {
                      TC.CSLSDeviceWrapper wrapper = new TC.CSLSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m,hwstatus,opmode,opstatus);
                  //    devices.Add(devicename, new TC.CSLSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m));
                      devices.Add(devicename, wrapper);
                  }
                   * */





              }

          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
              System.Environment.Exit(-1);
          }
          finally
          {
              try
              {
                  rd.Close();
                  rd.Dispose();
                  cn.Close();
              }
              catch { ;}

          }
      //    Console.WriteLine("load " + mfccid.ToString() + "finish!");
      }

      public void RemoveDeviceWrapper(string devName)
      {
          if (!devices.ContainsKey(devName))

              
              
                  throw new Exception(devName + " not in list!");

                  DeviceBaseWrapper wrapper = (DeviceBaseWrapper)devices[devName];
                  wrapper.getRemoteObj().Remove(wrapper.deviceName);

                  devices.Remove(devName);
              

      }


      public void AddDeviceWrapper(string devName, System.Data.Odbc.OdbcConnection cn)
      {
          AddDeviceWrapper(devName, false,cn);
      }


     
      public void AddDeviceWrapper(string devName, bool isMfccAddDevice, System.Data.Odbc.OdbcConnection cnn)
      {
          OdbcDataReader rd = null;
          OdbcConnection cn = null; ;
          OdbcCommand cmd;
          try
          {

              devName = devName.Trim();
              if (isMfccAddDevice)
                  cn = new OdbcConnection(Global.Db2ConnectionString);
              else
                  cn = cnn;

              cmd = new OdbcCommand();
              cmd.Connection = cn;
              if (this.devices.ContainsKey(devName))
                  throw new Exception(devName + "already in list");

              if (isMfccAddDevice)
                 cn.Open();
              if (devName.StartsWith("VD"))
                  cmd.CommandText = "select tbldeviceconfig.devicename,device_type,mfccid,location,lineid,direction,mile_m,ip,port,hw_status_1,hw_status_2,hw_status_3,hw_status_4,op_mode,op_status,enable,start_mileage,end_mileage from tbldeviceconfig inner join tblVDConfig on tbldeviceconfig.devicename=tblVDConfig.devicename where  tbldeviceconfig.devicename='" + devName + "'";
              else if(devName.StartsWith("RD"))
                  cmd.CommandText = "select tbldeviceconfig.devicename,device_type,mfccid,location,lineid,direction,mile_m,ip,port,hw_status_1,hw_status_2,hw_status_3,hw_status_4,op_mode,op_status,enable,start_mileage,end_mileage from tbldeviceconfig inner join tblRDConfig on tbldeviceconfig.devicename=tblRDConfig.devicename where  tbldeviceconfig.devicename='" + devName + "'";
              else if (devName.StartsWith("VI"))
                  cmd.CommandText = "select tbldeviceconfig.devicename,device_type,mfccid,location,lineid,direction,mile_m,ip,port,hw_status_1,hw_status_2,hw_status_3,hw_status_4,op_mode,op_status,enable,start_mileage,end_mileage from tbldeviceconfig inner join tblVIConfig on tbldeviceconfig.devicename=tblVIConfig.devicename where  tbldeviceconfig.devicename='" + devName + "'";
              else if (devName.StartsWith("WD")  || devName.StartsWith("TWD"))
                  cmd.CommandText = "select tbldeviceconfig.devicename,device_type,mfccid,location,lineid,direction,mile_m,ip,port,hw_status_1,hw_status_2,hw_status_3,hw_status_4,op_mode,op_status,enable,start_mileage,end_mileage from tbldeviceconfig inner join tblWDConfig on tbldeviceconfig.devicename=tblWDConfig.devicename where  tbldeviceconfig.devicename='" + devName + "'";
              
              else
                  cmd.CommandText = "select devicename,device_type,mfccid,location,lineid,direction,mile_m,ip,port,hw_status_1,hw_status_2,hw_status_3,hw_status_4,op_mode,op_status,enable from tbldeviceconfig where  devicename='" + devName + "'";
              rd = cmd.ExecuteReader();

              if (!rd.Read())
                  throw new Exception(devName + " not  found in database!");
              else
              {

                  string devicename = (rd[0] as string).Trim();
                  string devicetype = rd[1] as string;
                  string mfccid = rd[2] as string;
                  string location = rd[3] as string;
                  string lineid = rd[4] as string;
                  string direction = rd[5] as string;



                  int mile_m = System.Convert.ToInt32(rd[6]);
                  string ip = rd[7] as string;
                  int port = System.Convert.ToInt32(rd[8]);

                  byte[] hwstatus = new byte[4];
                  hwstatus[0] = System.Convert.ToByte(rd[9]);
                  hwstatus[1] = System.Convert.ToByte(rd[10]);
                  hwstatus[2] = System.Convert.ToByte(rd[11]);
                  hwstatus[3] = System.Convert.ToByte(rd[12]);
                  byte opmode = System.Convert.ToByte(rd[13]);
                  byte opstatus = System.Convert.ToByte(rd[14]);
                  bool enable = (rd[15].ToString() == "Y") ? true : false;  //目前沒用
                  
                  if (devicetype == "VD")
                  {
                      int start_mileage, end_mileage;
                      start_mileage = System.Convert.ToInt32(rd[15+1]);

                      end_mileage = System.Convert.ToInt32(rd[16+1]);
                      TC.VDDeviceWrapper wrapper = new TC.VDDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction,start_mileage,end_mileage);
                        
                      if( isMfccAddDevice)
                      wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                   
                  }
                  else if (devicetype == "RGS")
                  {
                      TC.RGSDeviceWrapper wrapper = new TC.RGSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                     if (isMfccAddDevice)
                      wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "CMS")
                  {

                      TC.CMSDeviceWrapper wrapper = new TC.CMSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                     if (isMfccAddDevice)
                      wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "RMS")
                  {
                      TC.RMSDeviceWrapper wrapper = new TC.RMSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                     if (isMfccAddDevice)
                      wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "WIS")
                  {
                      TC.WISDeviceWrapper wrapper = new TC.WISDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                      if (isMfccAddDevice)
                      wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "LCS")
                  {
                      TC.LCSDeviceWrapper wrapper = new TC.LCSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                      if (isMfccAddDevice)
                      wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "CSLS")
                  {
                      TC.CSLSDeviceWrapper wrapper = new TC.CSLSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                      if (isMfccAddDevice)
                      wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "AVI")
                  {
                      TC.AVIDeviceWrapper wrapper = new TC.AVIDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                      if (isMfccAddDevice)
                          wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "RD")
                  {
                      int start_mileage, end_mileage;
                      start_mileage = System.Convert.ToInt32(rd[15+1]);

                      end_mileage = System.Convert.ToInt32(rd[16+1]);

                      TC.RDDeviceWrapper wrapper = new TC.RDDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction,start_mileage,end_mileage);
                      if (isMfccAddDevice)
                          wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "VI")
                  {
                      int start_mileage, end_mileage;
                      start_mileage = System.Convert.ToInt32(rd[15+1]);

                      end_mileage = System.Convert.ToInt32(rd[16+1]);
                      TC.VIDeviceWrapper wrapper = new TC.VIDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction,start_mileage,end_mileage);
                      if (isMfccAddDevice)
                          wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "WD")
                  {
                      int start_mileage, end_mileage;
                      start_mileage = System.Convert.ToInt32(rd[15+1]);

                      end_mileage = System.Convert.ToInt32(rd[16+1]);
                      TC.WDDeviceWrapper wrapper = new TC.WDDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction,start_mileage,end_mileage);
                      if (isMfccAddDevice)
                          wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                          devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "TTS")
                  {
                      TC.TTSDeviceWrapper wrapper = new TC.TTSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                      if (isMfccAddDevice)
                          wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "FS")
                  {
                      TC.FSDeviceWrapper wrapper = new TC.FSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                      if (isMfccAddDevice)
                          wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "MAS")
                  {
                      TC.MASDeviceWrapper wrapper = new TC.MASDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                      if (isMfccAddDevice)
                          wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "IID")
                  {
                      TC.IIDDeviceWrapper wrapper = new TC.IIDDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                      if (isMfccAddDevice)
                          wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "ETTU")
                  {
                      TC.ETTUDeviceWrapper wrapper = new TC.ETTUDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                      if (isMfccAddDevice)
                          wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "LS")
                  {
                      TC.LSDeviceWrapper wrapper = new TC.LSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                      if (isMfccAddDevice)
                          wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "TEM")
                  {
                      TC.TEMDeviceWrapper wrapper = new TC.TEMDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                      if (isMfccAddDevice)
                          wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "CMSRST")
                  {
                      TC.CMSRSTDeviceWrapper wrapper = new TC.CMSRSTDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                      if (isMfccAddDevice)
                          wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "BS")
                  {
                      TC.BSDeviceWrapper wrapper = new TC.BSDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                      if (isMfccAddDevice)
                          wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "SCM")
                  {
                      TC.SCMDeviceWrapper wrapper = new TC.SCMDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                      if (isMfccAddDevice)
                          wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }
                  else if (devicetype == "CCTV")
                  {
                      TC.CCTVDeviceWrapper wrapper = new TC.CCTVDeviceWrapper(mfccid, devicename, devicetype, ip, port, location, lineid, mile_m, hwstatus, opmode, opstatus, direction);
                      //if (isMfccAddDevice)
                      //    wrapper.getRemoteObj().AddDevice(wrapper.deviceName);
                      devices.Add(devicename, wrapper);
                  }






              }

          }
         
          finally
          {
              try
              {
                  rd.Close();
                  rd.Dispose();
                  if (isMfccAddDevice)
                         cn.Close();
              }
              catch (Exception ex){
                  ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
                  ;}

          }
      }

    }
}
