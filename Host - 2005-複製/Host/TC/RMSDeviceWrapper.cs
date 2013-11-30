using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using RemoteInterface.HC;

namespace Host.TC
{
  public   class RMSDeviceWrapper:OutPutDeviceBase
    {
      public RMSDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction)
          : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus, direction)
      {
      }


      
#if DEBUG
      public override void output()
      {
      }
#else
       public override void output()
       {
           //throw new Exception("The method or operation is not implemented.");

           OutputQueueData outdata=this.getOutputdata();

           if (outdata == null || outdata.data == null)
           {
               this.getRemoteObj().SetDisplayOff(this.deviceName);
               return;
           }
           RMSOutputData data=(RMSOutputData)outdata.data;
           if (this.getRemoteObj() != null && this.getRemoteObj().getConnectionStatus(deviceName))
           this.getRemoteObj().SetModeAndPlanno(this.deviceName,(byte)data.mode, (byte)data.planno);
       }
#endif
     
       public new   I_MFCC_RMS getRemoteObj()
       {
             return (I_MFCC_RMS)base.getRemoteObj();

       }

      public void SetModeAndPlanno(string devname,OutputModeEnum mode,int ruleid,int priority, byte rmsmode, byte planno)
      {
           this.SetOutput(new OutputQueueData(this.deviceName,mode,ruleid,priority,new RMSOutputData(rmsmode,planno)));
          // output();
      }

   }


   
}
