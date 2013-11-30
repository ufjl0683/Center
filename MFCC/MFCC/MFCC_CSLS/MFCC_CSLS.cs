using System;
using System.Collections.Generic;
using System.Text;
using Comm.MFCC;

namespace MFCC_CSLS
{
   public  class MFCC_CSLS:MFCC_Base
    {
      //  WIS_Manager wis_manager; wis_manager;
         public MFCC_CSLS(string mfccid,string devType, int remotePort, int notifyPort, int consolePort, string regRemoteName, Type regRemoteType)
             : base(mfccid,devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
         {

         }


       public override void BindEvent(object tc)
       {
           //throw new Exception("The method or operation is not implemented.");
       }

      



    }
}
