using System;
using System.Collections.Generic;
using System.Text;

namespace Host.TC
{
  public   class IIDDeviceWrapper: DeviceBaseWrapper
    {
      public IIDDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction)
          : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus, direction)
      {
      }


    }
}
