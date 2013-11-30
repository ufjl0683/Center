using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using Comm.TC;
using RemoteInterface;

namespace MFCC_FS
{
   public  class RemoteObj : Comm.MFCC.RemoteMFCCBase, I_MFCC_FS
    {
       // void setTransmitCycle(string devName, int cycle);


      

       public override object InitializeLifetimeService()
       {
           return null; //base.InitializeLifetimeService();
       }





       public override Comm.MFCC.MFCC_Base getMFCC_base()
       {

           return Program.mfcc_fs;

       }


      

       public void SendDisplay(string devName,byte type)
       {
           //throw new Exception("The method or operation is not implemented.");
           try
           {
               Comm.TC.FSTC tc = (Comm.TC.FSTC)Program.mfcc_fs.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.TC_SetDisplay(type);
           }
           catch (Exception ex)
           {
               throw new RemoteException(ex.Message+","+ex.StackTrace);
           }
       }

       public void setDisplayOff(string devName)
       {
            try
           {
               Comm.TC.FSTC tc = (Comm.TC.FSTC)Program.mfcc_fs.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.TC_SetDisplayOff();
               ConsoleServer.WriteLine(devName+"熄滅!");
           }
           catch (Exception ex)
           {
               throw new RemoteException(ex.Message+","+ex.StackTrace);
           }
       }

       public byte GetCurrentTCDisplayType(string devName)
       {
           try
           {
               Comm.TC.FSTC tc = (Comm.TC.FSTC)Program.mfcc_fs.getTcManager()[devName];
               checkAllowConnect(tc);
               return tc.TC_GetDisplayType();
              // tc.GetCurrentDispaly(boardid, ref mesg,ref  color);
             //  ConsoleServer.WriteLine(devName + ",bid=" + boardid + ",mesg=" + mesg + ",color=" + color);
           }
           catch (Exception ex)
           {
               throw new RemoteException(ex.Message+","+ex.StackTrace);
           }
       }
     
   }
}
