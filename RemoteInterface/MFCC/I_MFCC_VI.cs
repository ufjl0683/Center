using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{
  public   interface I_MFCC_VI : I_MFCC_Base
    {
      void getCurrentVIData(string devName, ref DateTime dt, ref int distance,  ref int degree);
      void loadValidCheckRule();
      void setTransmitCycle(string devName, int cycle);
    }
}
