using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using Comm.TC;
using RemoteInterface;
using System.Data;
using RemoteInterface.MFCC;

namespace MFCC_BS
{
   public  class RemoteObj : Comm.MFCC.RemoteMFCCBase, I_MFCC_BS
    {
       // void setTransmitCycle(string devName, int cycle);


       public void getCurrentBSData(string devName, ref DateTime dt,ref int slope,ref int shift ,ref int sink,ref int degree)
       {
           try
           {
               Comm.TC.BSTC tc = (Comm.TC.BSTC)Program.mfcc_bs.getTcManager()[devName];
               checkAllowConnect(tc);
                tc.getCurrentBSData(ref  dt,ref slope, ref shift,ref sink,ref degree);
               
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
               Program.mfcc_bs.loadRangeTable();
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

           return Program.mfcc_bs;

       }

    }
}
