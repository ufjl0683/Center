using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using Comm.TC;
using RemoteInterface;

namespace MFCC_TEM
{
   public  class RemoteObj : Comm.MFCC.RemoteMFCCBase,RemoteInterface.MFCC.I_MFCC_TEM
    {
       // void setTransmitCycle(string devName, int cycle);


      

       public override object InitializeLifetimeService()
       {
           return null; //base.InitializeLifetimeService();
       }





       public override Comm.MFCC.MFCC_Base getMFCC_base()
       {

           return Program.mfcc_tem;

       }



       public void setLCSStatus(string devName,int status)
       {
           try
           {
               Program.mfcc_tem.setLCSStatus(devName, status);

           }
           catch (Exception ex)
           {
               throw new RemoteException(ex.Message + "," + ex.StackTrace);
           }
           
       }





    


       public void setCompareOutputMin(string devName, int min)
       {
           throw new Exception("The method or operation is not implemented.");
       }

    
   }
}
