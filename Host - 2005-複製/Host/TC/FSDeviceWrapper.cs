using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;
using RemoteInterface;

namespace Host.TC
{
  public   class FSDeviceWrapper : OutPutDeviceBase
    {
      public FSDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction)
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
          try
          {

              RemoteInterface.MFCC.I_MFCC_FS robj = ((RemoteInterface.MFCC.I_MFCC_FS)this.getRemoteObj());
              OutputQueueData qdata = this.getOutputdata();
              //  throw new Exception("The method or operation is not implemented.");
              if (!this.IsConnected)
                  return;

              if (qdata == null || qdata.data == null)
              {
                  robj.setDisplayOff(this.deviceName);
                  return;
              }

              robj.SendDisplay(this.deviceName, ((FSOutputData)qdata.data).type);

          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(this.deviceName + "," + ex.Message);
          }


      }

#endif
  }
}
