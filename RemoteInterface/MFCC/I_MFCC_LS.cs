using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{
  public   interface I_MFCC_LS : I_MFCC_Base
    {
      void getCurrentLSData(string devName, ref DateTime dt, ref int day_var, ref int mon_var, ref int degree);
      void loadValidCheckRule();
      void setTransmitCycle(string devName, int cycle);
  
  }
}
