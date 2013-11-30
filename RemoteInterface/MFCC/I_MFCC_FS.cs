using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{
    public   interface I_MFCC_FS : RemoteInterface.MFCC.I_MFCC_Base
    {
        void SendDisplay(string devName, byte type);
        void setDisplayOff(string devName);
        byte GetCurrentTCDisplayType(string devName);
    }
}
