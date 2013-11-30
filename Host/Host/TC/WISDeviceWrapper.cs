using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;
using Host.TC;

namespace Host.TC
{
    class WISDeviceWrapper : OutPutDeviceBase
    {

        public WISDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction)
            : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus,direction)
       {
       }


       public void setWIS_Dispaly(string devicename,OutputModeEnum mode, int ruleid, int priority, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors)
       {
           CMSOutputData WISdata = new CMSOutputData(icon_id, g_code_id, hor_space, mesg, colors);
           OutputQueueData data = new OutputQueueData(devicename, mode, ruleid, priority, WISdata);
           this.SetOutput(data);
           //output();
       }

        
#if DEBUG
      public override void output()
      {
          //OutputQueueData data = this.getOutputdata();
          //if (this.getRemoteObj() != null && this.getRemoteObj().getConnectionStatus(this.deviceName))
          //    if (data == null || data.data == null)
          //        this.getRemoteObj().setDisplayOff(this.deviceName);
          //    else
          //    {
          //        CMSOutputData cmddata = (CMSOutputData)data.data;
          //        this.getRemoteObj().SendDisplay(this.deviceName, cmddata.g_code_id, cmddata.hor_space, cmddata.mesg, cmddata.colors);

          //    }
      }
#else
       public override void output()
       {
           OutputQueueData data = this.getOutputdata();
           if (data == null)
           {
               if (this.IsConnected)
                   ((RemoteInterface.MFCC.I_MFCC_WIS)this.getRemoteObj()).setDisplayOff(this.deviceName);

               return;
           }
           if (this.getRemoteObj() != null && this.getRemoteObj().getConnectionStatus(this.deviceName))
           if (data==null || data.data == null)
               this.getRemoteObj().setDisplayOff(this.deviceName);
           else
           {
               CMSOutputData cmddata = (CMSOutputData)data.data;
               this.getRemoteObj().SendDisplay(this.deviceName, cmddata.g_code_id, cmddata.hor_space, cmddata.mesg, cmddata.colors);

           }
          // throw new Exception("The method or operation is not implemented.");
       }

#endif

       public new  RemoteInterface.MFCC.I_MFCC_WIS getRemoteObj()
       {
           return (RemoteInterface.MFCC.I_MFCC_WIS)base.getRemoteObj();
       }
    }

   
}
