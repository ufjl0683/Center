using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using Comm.TC;
using RemoteInterface;

namespace MFCC_WD
{
   public  class RemoteObj : Comm.MFCC.RemoteMFCCBase, I_MFCC_WD
    {
       // void setTransmitCycle(string devName, int cycle);


       public void getCurrentWDData(string devName, ref DateTime dt, ref int average_wind_speed, ref int average_wind_direction, ref int max_wind_speed, ref int max_wind_direction, ref int degree)
       {
           try
           {
               Comm.TC.WDTC tc = (Comm.TC.WDTC)Program.mfcc_wd.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.getCurrentWDData( ref  dt, ref  average_wind_speed, ref  average_wind_direction, ref  max_wind_speed, ref  max_wind_direction, ref  degree);
               
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
               Program.mfcc_wd.loadRangeTable();
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


       public void setTransmitCycle(string devName, int cycle)
       {
           // throw new Exception("The method or operation is not implemented.");
           try
           {
               Comm.TC.WDTC tc = (Comm.TC.WDTC)Program.mfcc_wd.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.SetTransmitCycle(cycle);
           }
           catch (Exception ex)
           {
               throw new RemoteException(ex.Message + ex.StackTrace);
           }
       }


       public override Comm.MFCC.MFCC_Base getMFCC_base()
       {

           return Program.mfcc_wd;

       }

    }
}
