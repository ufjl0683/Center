using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{
    public interface  I_MFCC_CSLS:RemoteInterface.MFCC.I_MFCC_Base
    {
        void SetDisplay(string devName, System.Data.DataSet ds);
        void SetDisplayOff(string devName);
    }
}
