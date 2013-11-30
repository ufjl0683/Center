using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{
  public   interface I_MFCC_WD : I_MFCC_Base
    {
      void getCurrentWDData(string devName, ref DateTime dt, ref int average_wind_speed, ref int average_wind_direction, ref int max_wind_speed, ref int max_wind_direction,ref int degree);
      void loadValidCheckRule();
       void setTransmitCycle(string devName, int cycle);
  
  }
}
