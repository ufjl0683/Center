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




      public override void output()
      {
          try
          {

              RemoteInterface.MFCC.I_MFCC_FS robj = ((RemoteInterface.MFCC.I_MFCC_FS)this.getRemoteObj());
              OutputQueueData qdata = this.getOutputdata();
              //  throw new Exception("The method or operation is not implemented.");
              if (!this.IsConnected)
              {
                  if (qdata != null)
                  {
                      qdata.IsSuccess = false;
                      qdata.status = 2;
                   
                  }
                  return;
              }

              if (qdata == null || qdata.data == null)
              {
#if DEBUG
#else
                  robj.setDisplayOff(this.deviceName);
#endif
                  return;
              }
#if DEBUG
#else
              try
              {
                  qdata.status = 1;
                  robj.SendDisplay(this.deviceName, ((FSOutputData)qdata.data).type);
              }
              catch (Exception ex)
              {
                  qdata.IsSuccess = false;
              }
#endif

          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(this.deviceName + "," + ex.Message);
          }


      }


  }
}
