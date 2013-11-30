using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{
    [Serializable]
   public  class WD_10Min_Data
    {

       public string devName;
          public  DateTime dt;
      public int average_wind_speed,  average_wind_direction,  max_wind_speed,  max_wind_direction,  am_degree;
       public WD_10Min_Data(string devName, DateTime dt, int average_wind_speed, int average_wind_direction, int max_wind_speed, int max_wind_direction, int am_degree)
       {
           this.devName=devName;
           this.dt = dt;
           this.average_wind_speed = average_wind_speed;
           this.average_wind_direction = average_wind_direction;
           this.max_wind_speed = max_wind_speed;
           this.max_wind_direction = max_wind_direction;
           this.am_degree = am_degree;

       }
    }
}
