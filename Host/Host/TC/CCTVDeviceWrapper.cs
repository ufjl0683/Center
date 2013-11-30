using System;
using System.Collections.Generic;
using System.Text;

namespace Host.TC
{
    class CCTVDeviceWrapper: DeviceBaseWrapper
    {
        public CCTVDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction)
           : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus,direction)
        {
            //this.start_mileage = start_mileage;
            //this.end_mileage = end_mileage;
        
        }
    }
}
