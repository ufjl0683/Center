using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using RemoteInterface;

namespace Host.TC
{
   public  class DeviceBaseWrapper:IComparable,I_Positionable
    {
      
      public  string ip;
      public  int port;
       public string deviceType;
       public string deviceName;
       public byte[] hw_status = new byte[4];
       public byte opStatus, opMode;
       public string mfccid;
       public string direction;
     //  public string lineid;
       public int mile_m;
       public string location;
       public string lineid;
       public bool IsConnected;

       public int AryInx = -1;

       public I_Positionable PreDevice;
       public I_Positionable NextDevice;
       public int start_mileage, end_mileage;

       public int event_degree = 0;
       public DeviceBaseWrapper(string mfccid,string devicename, string deviceType, string ip, int port,string location,string lineid,int mile_m,byte[]hw_status,byte opmode,byte opstatus,string direction)
       {
           this.deviceName = devicename;
           this.ip = ip;
           this.deviceType = deviceType;
           this.port = port;
           this.location = location;
           this.mile_m = mile_m;
           this.mfccid = mfccid;
           this.hw_status = hw_status;
           this.opStatus = opstatus;
           this.opMode = opmode;
           this.direction = direction;
           this.lineid = lineid;
           if (this.deviceType != "CCTV") 
                updateHW_Status();
       }
       
       public void  set_HW_status(byte[]hwstatus,byte opstatus,byte opmode,bool isConnected)
       {
           this.hw_status = hwstatus;
           this.opMode = opmode;
           this.opStatus = opstatus;
           this.IsConnected = isConnected;
       }


       public void ReloadDeviceLocation()
       {
           System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
           System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select location  from tbldeviceconfig where devicename='"+this.deviceName+"'");
           cmd.Connection = cn;
           try
           {
               cn.Open();
               this.location= cmd.ExecuteScalar().ToString();
               
           }
           catch (Exception ex)
           {

               throw ex;

           }
           finally
           {
               cn.Close();
           }


       }

       public void updateHW_Status()
       {
           try
           {

               if (this.getRemoteObj() == null)
                   return;
               this.getRemoteObj().getDeviceStatus(this.deviceName, ref this.hw_status, ref this.opMode, ref this.opStatus, ref this.IsConnected);
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(this.deviceName + "getDeviceStatus," + ex.Message);
           }
       }

       public virtual I_MFCC_Base getRemoteObj()
       {
           if (this.deviceType == "CCTV")
               return null;
          return (I_MFCC_Base) Program.matrix.mfcc_mgr[this.mfccid].getRemoteObject();
          // return (I_MFCC_Base)Program.matrix.getRemoteObject(this.deviceName);
           
       }

       public bool IsRemoteObjectConnect()
       {
         
           try
           {
               if (this.deviceType == "CCTV")
                   return true;
               if (Program.matrix.mfcc_mgr[this.mfccid].getRemoteObject() == null)
                   return false;
              ((RemoteClassBase) Program.matrix.mfcc_mgr[this.mfccid].getRemoteObject()).HelloWorld();
               return true;
           }
           catch
           {
               return false;
           }
       }

       public override bool Equals(object obj)
       {
           //return base.Equals(obj);
           if (obj == null)
               return false;
           DeviceBaseWrapper dev = (DeviceBaseWrapper)obj;
           return this.lineid == dev.lineid && this.direction == dev.direction && this.mile_m == dev.mile_m;

       }
      

       public int CompareTo(object obj)
       {
           I_Positionable dev = (I_Positionable)obj;
           int result = 0;

           if (this.direction == "S" || this.direction == "E")
               result = this.mile_m - dev.getMileage();
           else
               result = -(this.mile_m - dev.getMileage());

        //   return result;

           return result;
           //throw new Exception("The method or operation is not implemented.");
       }



       #region I_Position 成員

       public string getLineID()
       {
           return this.lineid;
           //throw new Exception("The method or operation is not implemented.");
       }

       public string getDirection()
       {
           return this.direction;
           //throw new Exception("The method or operation is not implemented.");
       }

       public int getMileage()
       {
           return this.mile_m;
          // throw new Exception("The method or operation is not implemented.");
       }

       #endregion

       #region I_DevicePosition 成員


       public string getDevType()
       {
           return this.deviceType;
           // throw new Exception("The method or operation is not implemented.");
       }

       public string getDevName()
       {
           return this.deviceName;
          // throw new Exception("The method or operation is not implemented.");
       }

       #endregion

       #region I_Positionable 成員


       public I_Positionable getNextDev()
       {
           return this.NextDevice;
           //  throw new Exception("The method or operation is not implemented.");
       }

       public I_Positionable getPrevDev()
       {
           return PreDevice;
           //throw new Exception("The method or operation is not implemented.");
       }

       public void setPreDev(I_Positionable dev)
       {
           this.PreDevice = (I_Positionable)dev;
           //throw new Exception("The method or operation is not implemented.");
       }

       public void setNextDev(I_Positionable dev)
       {
           this.NextDevice = (I_Positionable)dev;
         //  throw new Exception("The method or operation is not implemented.");
       }

       #endregion
   }
}
