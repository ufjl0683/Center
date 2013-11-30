using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{
   public interface I_MFCC_RD : I_MFCC_Base
    {
       void loadValidCheckRule();
       void setTransmitCycle(string devName, int cycle);
       void getCurrentRDData(string  devName,ref DateTime dt, ref int amount, ref int acc_amount, ref int degree);
    }
}
