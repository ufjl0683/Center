using System;
using System.Collections.Generic;
using System.Text;

namespace Host.TC
{
    class VIDeviceWrapper : DeviceBaseWrapper
    {
        public int curr_distance=-1,curr_degree=-1;
        public DateTime lastReportTime=System.DateTime.MinValue;
     //   public int start_mileage, end_mileage;
        public event System.EventHandler OnEvent;
        public VIDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction,int start_mileage,int end_mileleage)
            : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus, direction)
           {
               this.start_mileage = start_mileage;
               this.end_mileage = end_mileleage;
           }

        public void  set5MinData(DateTime dt,int distance,int degree)
        {
            this.lastReportTime=dt;
            this.curr_distance=distance;
           
            this.curr_degree=degree;
            
        }

        public DateTime event_time;
        public int event_distance;

        public void setEventData(System.DateTime dt, int distance, int degree)
        {
            this.event_time = dt;
            
            this.event_distance = distance;
            if(this.event_degree != degree)
            {
                this.event_degree = degree;
                if (this.OnEvent != null)
                    this.OnEvent(this, null);
            }
        }

    }
}
