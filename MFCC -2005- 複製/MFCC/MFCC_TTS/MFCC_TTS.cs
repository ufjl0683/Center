using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_TTS
{
    public class MFCC_TTS : Comm.MFCC.MFCC_Base
    {


        
         public  MFCC_TTS(string mfccid,string devType,int remotePort,int notifyPort,int consolePort,string regRemoteName,Type regRemoteType)
            :base( mfccid,devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
        {
          
           
          
          

          
        }

         public override void BindEvent(object tc)
         {
            
             
         }


      
    }
}
