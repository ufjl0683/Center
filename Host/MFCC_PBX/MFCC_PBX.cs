using System;
using System.Collections.Generic;
using System.Text;
using Comm.MFCC;
using RemoteInterface;

namespace MFCC_PBX
{
   public  class MFCC_PBX:MFCC_Base
    {


       public MFCC_PBX(string mfccid, string devType, int remotePort, int notifyPort, int consolePort, string regRemoteName, Type regRemoteType)
           : base(mfccid, devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
       {

         
       }

       public override void BindEvent(object tc)
       {
           
         //  ((Comm.TCBase)tc).OnTCReport += new Comm.OnTCReportHandler(MFCC_AVI_OnTCReport);
       }


       public override void AfterDeviceAllStart()
       {
        //   base.AfterDeviceAllStart();
         
       }
        
     
            
       

    }
}
