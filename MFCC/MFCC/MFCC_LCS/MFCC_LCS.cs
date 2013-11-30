using System;
using System.Collections.Generic;
using System.Text;

namespace MFCC_LCS
{
   public  class MFCC_LCS:Comm.MFCC.MFCC_Base
    {

       public MFCC_LCS(string mfccid, string devType, int remotePort, int notifyPort, int consolePort, string regRemoteName, Type regRemoteType)
           : base(mfccid, devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
       {
       }

      

       public override void BindEvent(object tc)
       {
          // throw new Exception("The method or operation is not implemented.");
       }
   }
}
