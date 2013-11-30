using System;
using System.Collections.Generic;
using System.Text;

namespace Host.TC
{
    public class SCMDeviceWrapper : DeviceBaseWrapper
    {
        //System.DateTime lastReportTime;
        //int curr_slope=-1;
        //int curr_shift=-1;
        //int curr_sink = -1;
      //  int degre=-1;
        //int curr_max_wind_speed=-1;
        //int curr_max_wind_direction=-1;
        //int curr_degree=-1;
      //  public int start_mileage, end_mileage;
        //public event System.EventHandler OnEvent;
        public SCMDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction)
           : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus,direction)
        {
            //this.start_mileage = start_mileage;
            //this.end_mileage = end_mileage;
        
        }

        //public void set1hourMinData(DateTime dt, int slope,int shift, int sink,int degree)
        //{
        //    this.lastReportTime = dt;
        //    this.curr_slope = slope;
        //    this.curr_sink = sink;
        //    this.curr_shift=shift;
        //    //this.curr_max_wind_direction = max_wind_direction;
        //    //this.curr_max_wind_speed = max_wind_speed;
        //    this.curr_degree = degree;

        //}

      //  public int event_degree = 0;
      //  public DateTime event_time;
      //  public int event_slope, event_shift,event_sink;
      ////  public int   event_average_wind_speed, event_average_wind_direction,event_max_wind_speed,event_max_wind_direction;
      //  public void setEventData(DateTime dt, int slope, int shift,int sink, int degree)
      //  {
      //      // if(rdevent
      //      event_slope = slope;
      //      event_shift = shift;
      //      event_sink = sink;
      //      //this.event_time = dt;
      //      //this.event_average_wind_direction = average_wind_direction;
      //      //this.event_average_wind_speed = average_wind_speed;
      //      //this.event_max_wind_speed = max_wind_speed;
      //      //this.event_max_wind_direction = max_wind_direction;
      //      //this.event_degree = degree;
      //      if (this.event_degree != degree)
      //      {
      //          event_degree = degree;
      //          if (this.OnEvent != null)
      //              this.OnEvent(this, null);
              
      //      }
      //  }

    }
}
