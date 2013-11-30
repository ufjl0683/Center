using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using Comm.TC;
using RemoteInterface;

namespace MFCC_VI
{
   public  class RemoteObj : Comm.MFCC.RemoteMFCCBase, I_MFCC_VI
    {
       // void setTransmitCycle(string devName, int cycle);
      

       public void getCurrentVIData(string devName, ref DateTime dt, ref int distance, ref int degree)
       {
           try
           {
               Comm.TC.VITC tc = (Comm.TC.VITC)Program.mfcc_vi.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.getCurrentVIData(ref dt,ref  distance, ref degree);
               
           }
           catch (Exception ex)
           {
               throw new RemoteException(ex.Message+ex.StackTrace);
           }
       }

       public override object InitializeLifetimeService()
       {
           return null; //base.InitializeLifetimeService();
       }





       public override Comm.MFCC.MFCC_Base getMFCC_base()
       {

           return Program.mfcc_vi;

       }


     


       public void loadValidCheckRule()
       {
           //throw new Exception("The method or operation is not implemented.");
           try
           {
               Program.mfcc_vi.loadValidCheckRule();
           }
           catch (Exception ex)
           {
               throw new RemoteException(ex.Message + ex.StackTrace);
           }
       }

    
   }
}
