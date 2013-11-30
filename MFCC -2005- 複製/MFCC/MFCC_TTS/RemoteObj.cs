using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using Comm.TC;
using RemoteInterface;

namespace MFCC_TTS
{
   public  class RemoteObj : Comm.MFCC.RemoteMFCCBase, I_MFCC_TTS
    {
       // void setTransmitCycle(string devName, int cycle);


      

       public override object InitializeLifetimeService()
       {
           return null; //base.InitializeLifetimeService();
       }





       public override Comm.MFCC.MFCC_Base getMFCC_base()
       {

           return Program.mfcc_tts;

       }


      

       public void SendDisplay(string devName, byte boardid, string mesg, byte color)
       {
           //throw new Exception("The method or operation is not implemented.");
           try
           {
               Comm.TC.TTSTC tc = (Comm.TC.TTSTC)Program.mfcc_tts.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.TC_SendDisplay(boardid,mesg,color);
               ConsoleServer.WriteLine(devName + ",bid=" + boardid + ",mesg=" + mesg + ",color=" + color);
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
               throw new RemoteException(ex.Message+","+ex.StackTrace);
           }
       }

       public void setDisplayOff(string devName)
       {
            try
           {
               Comm.TC.TTSTC tc = (Comm.TC.TTSTC)Program.mfcc_tts.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.TC_SetDisplayOff();
               ConsoleServer.WriteLine(devName+"熄滅!");
           }
           catch (Exception ex)
           {
               throw new RemoteException(ex.Message+","+ex.StackTrace);
           }
       }

       public void GetCurrentTCDisplay(string devName, byte boardid, ref string mesg, ref byte color)
       {
           try
           {
               Comm.TC.TTSTC tc = (Comm.TC.TTSTC)Program.mfcc_tts.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.GetCurrentDispaly(boardid, ref mesg,ref  color);
              // 
           }
           catch (Exception ex)
           {
               throw new RemoteException(ex.Message+","+ex.StackTrace);
           }
       }



      


       public void setDisplayOff(string devName, byte boardid)
       {
           try
           {
               Comm.TC.TTSTC tc = (Comm.TC.TTSTC)Program.mfcc_tts.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.TC_SetDisplayOff(boardid);
               ConsoleServer.WriteLine(devName + ",bid=" + boardid + "熄滅");
           }
           catch (Exception ex)
           {
               throw new RemoteException(ex.Message+","+ex.StackTrace);
           }
         
           // throw new Exception("The method or operation is not implemented.");
       }

      
   }
}
