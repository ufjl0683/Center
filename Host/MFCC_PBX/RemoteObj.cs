using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HWStatus;
using System.Data;
using RemoteInterface.MFCC;


namespace MFCC_PBX
{
    class RemoteObj:Comm.MFCC.RemoteMFCCBase,I_MFCC_PBX
    {



        public override object InitializeLifetimeService()
        {
            return null; //base.InitializeLifetimeService();
        }


     


        public override Comm.MFCC.MFCC_Base getMFCC_base()
        {

           return Program.mfcc_pbx;
           
        }

      



       


      


        

      
   


        //public void downLoadConfigParam(string devName)
        //{
        //    try
        //    {
        //        Comm.TC.RDTC tc = (Comm.TC.RDTC)Program.mfcc_rd.getTcManager()[devName];
        //        checkAllowConnect(tc);
        //        tc.DownLoadConfig();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new RemoteException(ex.Message);
        //    }
        //   // throw new Exception("The method or operation is not implemented.");
        //}

     
    }
}
