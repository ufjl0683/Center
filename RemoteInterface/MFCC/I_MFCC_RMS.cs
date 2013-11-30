using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{
  public  interface I_MFCC_RMS:RemoteInterface.MFCC.I_MFCC_Base
    {

      void SetModeAndPlanno(string devname,byte mode, byte planno);
      void SetModeAndPlanno(string ip,int port, byte mode, byte planno);
      void GetModeAndPlanno(string devname, ref byte mode, ref byte planno);
      void SetDisplayOff(string devName);
      
    }
}
