using System;
using System.Collections.Generic;
using System.Text;

namespace Host.TC
{
    class RDDeviceWrapper : DeviceBaseWrapper
    {
        public int curr_amount=-1,curr_acc_amount=-1,curr_degree=-1;
        public DateTime lastReportTime=System.DateTime.MinValue;
      //  public int start_mileage, end_mileage;
       
        public event System.EventHandler OnEvent;


        public RDDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction, int start_mileage, int end_mileage)
           : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus,direction)
       {
           this.start_mileage = start_mileage;
           this.end_mileage = end_mileage;
       }

        public void  set5MinData(DateTime dt,int amount,int acc_amount,int degree)
        {
            this.lastReportTime=dt;
            this.curr_amount=amount;
            this.curr_acc_amount=acc_amount;
            this.curr_degree=degree;
            
        }


        public int  event_pluviometric = 0;
        public DateTime event_time;
        public void setEventData(DateTime dt, int pluviometric,int degree )
        {
           // if(rdevent
            
            this.event_time = dt;
            this.event_pluviometric = pluviometric;
            if (event_degree != degree)
            {
                this.event_degree = degree;
                if (this.OnEvent != null)
                    this.OnEvent(this, null);
            }
        }

    }
}
