using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using Comm.TC;
using RemoteInterface;
using System.Data;

namespace MFCC_LS
{
   public  class RemoteObj : Comm.MFCC.RemoteMFCCBase, I_MFCC_LS
    {
       // void setTransmitCycle(string devName, int cycle);


       public void getCurrentLSData(string devName, ref DateTime dt,ref int day_var,ref int mon_var,ref int degree)
       {
           try
           {
               Comm.TC.LSTC tc = (Comm.TC.LSTC)Program.mfcc_ls.getTcManager()[devName];
               checkAllowConnect(tc);
                tc.getCurrentLSData(ref  dt,ref day_var, ref mon_var,ref degree);
               
           }
           catch (Exception ex)
           {
               throw new RemoteException(ex.Message+ex.StackTrace);
           }
       }

       public void loadValidCheckRule()
       {
           try
           {
               Program.mfcc_ls.loadRangeTable();
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

           return Program.mfcc_ls;

       }

    }
}
