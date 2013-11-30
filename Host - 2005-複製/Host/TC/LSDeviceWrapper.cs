using System;
using System.Collections.Generic;
using System.Text;

namespace Host.TC
{
    public class LSDeviceWrapper : DeviceBaseWrapper
    {
        System.DateTime lastReportTime;
        int curr_day_var=-1;
        int curr_mon_var=-1;
        //int curr_max_wind_speed=-1;
        //int curr_max_wind_direction=-1;
        int curr_degree=-1;
      //  public int start_mileage, end_mileage;
        public event System.EventHandler OnEvent;
        public LSDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus,string direction)
           : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus,direction)
        {
            //this.start_mileage = start_mileage;
            //this.end_mileage = end_mileage;
        
        }

        public void set10MinData(DateTime dt, int mon_var,int day_var, int degree)
        {
            this.lastReportTime = dt;
            this.curr_day_var = day_var;
            this.curr_mon_var = mon_var;
            //this.curr_max_wind_direction = max_wind_direction;
            //this.curr_max_wind_speed = max_wind_speed;
            this.curr_degree = degree;

        }

      //  public int event_degree = 0;
        public DateTime event_time;
        public int event_mon_var, event_day_var;
      //  public int   event_average_wind_speed, event_average_wind_direction,event_max_wind_speed,event_max_wind_direction;
        public void setEventData(DateTime dt, int mon_var, int day_var, int degree)
        {
            // if(rdevent
            event_day_var = mon_var;
            event_day_var = day_var;
          
            //this.event_time = dt;
            //this.event_average_wind_direction = average_wind_direction;
            //this.event_average_wind_speed = average_wind_speed;
            //this.event_max_wind_speed = max_wind_speed;
            //this.event_max_wind_direction = max_wind_direction;
            //this.event_degree = degree;
            if (this.event_degree != degree)
            {
                event_degree = degree;
                if (this.OnEvent != null)
                    this.OnEvent(this, null);
              
            }
        }

    }
}
