using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{
   public  interface I_MFCC_TEM: RemoteInterface.MFCC.I_MFCC_Base
    {

       void setLCSStatus(string devName,int status);
       
    }
}
