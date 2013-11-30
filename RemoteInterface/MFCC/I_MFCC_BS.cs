using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{
  public   interface I_MFCC_BS : I_MFCC_Base
    {
      void getCurrentBSData(string devName, ref DateTime dt, ref int slope, ref int shift, ref int sink,ref int degree);
      void loadValidCheckRule();
      void setTransmitCycle(string devName, int cycle);
  
  }
}
