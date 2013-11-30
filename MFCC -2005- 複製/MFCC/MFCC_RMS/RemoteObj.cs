using System;
using System.Collections.Generic;
using System.Text;
using Comm;
using RemoteInterface;
using RemoteInterface.MFCC;

namespace MFCC_RMS
{
   public  class RemoteObj:Comm.MFCC.RemoteMFCCBase,RemoteInterface.MFCC.I_MFCC_RMS
    {

       public override object InitializeLifetimeService()
       {
           return null;
       }

        public override Comm.MFCC.MFCC_Base getMFCC_base()
        {
           return Program.mfcc_rms;
           // throw new Exception("The method or operation is not implemented.");
        }




      

        public void SetModeAndPlanno(string devname, byte mode, byte planno)
        {
           // throw new Exception("The method or operation is not implemented.");
            try
            {
                Comm.TC.RMSTC tc = (Comm.TC.RMSTC)this.getMFCC_base().getTcManager()[devname];
                checkAllowConnect(tc);
                tc.SetModeAndPlanno(mode, planno);
               
                
               // tc.SetModeAndPlanno(mode, planno);
                ConsoleServer.WriteLine(devname+" mode:"+mode+" planno:"+planno);
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message+ex.StackTrace);
            }
        }

        public void SetModeAndPlanno(string ip, int port, byte mode, byte planno)
        {

            try
            {
                Comm.TC.RMSTC tc = (Comm.TC.RMSTC)this.getMFCC_base().getTcManager()[ip,port];
                checkAllowConnect(tc);
                tc.SetModeAndPlanno(mode, planno);
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
            }
           // throw new Exception("The method or operation is not implemented.");
        }

       public void GetModeAndPlanno(string devname, ref byte mode, ref byte planno)
       {
           try
           {
               Comm.TC.RMSTC tc = (Comm.TC.RMSTC)this.getMFCC_base().getTcManager()[devname];
               checkAllowConnect(tc);
               tc.TC_GetCurrentModeAndPlanNo(ref mode,ref planno);

           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }
       }
     


       public void SetDisplayOff(string devName)
       {
           try
           {
               Comm.TC.RMSTC tc = (Comm.TC.RMSTC)this.getMFCC_base().getTcManager()[devName];
               checkAllowConnect(tc);
               tc.TC_SetDisplayOff();
               ConsoleServer.WriteLine(devName + "儀控中止");
           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }
       }

     
      

      

      
   }
}
