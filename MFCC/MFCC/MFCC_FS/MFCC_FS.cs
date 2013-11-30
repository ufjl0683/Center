using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_FS
{
    public class MFCC_FS : Comm.MFCC.MFCC_Base
    {


        
         public  MFCC_FS(string mfccid,string devType,int remotePort,int notifyPort,int consolePort,string regRemoteName,Type regRemoteType)
            :base( mfccid,devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
        {
          
           
          
          

          
        }

         public override void BindEvent(object tc)
         {
            
             
         }


      
    }
}
