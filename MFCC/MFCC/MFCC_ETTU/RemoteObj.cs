using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using Comm.TC;
using RemoteInterface;
using Comm;

namespace MFCC_ETTU
{
   public  class RemoteObj : Comm.MFCC.RemoteMFCCBase,RemoteInterface.MFCC.I_MFCC_ETTU
    {
       // void setTransmitCycle(string devName, int cycle);


      

       public override object InitializeLifetimeService()
       {
           return null; //base.InitializeLifetimeService();
       }





       public override Comm.MFCC.MFCC_Base getMFCC_base()
       {

           return Program.mfcc_ettu;

       }



       

      
   }
}
