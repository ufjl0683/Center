using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_MAS
{
    public class MFCC_MAS : Comm.MFCC.MFCC_Base
    {


        
         public  MFCC_MAS(string mfccid,string devType,int remotePort,int notifyPort,int consolePort,string regRemoteName,Type regRemoteType)
            :base( mfccid,devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
        {
          
           
          
          

          
        }

         public override void BindEvent(object tc)
         {
             ((Comm.TCBase)tc).OnTCReport += new Comm.OnTCReportHandler(MFCC_MAS_OnTCReport);
             
         }

        void MFCC_MAS_OnTCReport(object tc, Comm.TextPackage txt)
        {
            //throw new Exception("The method or operation is not implemented.");
        }


      
    }
}
