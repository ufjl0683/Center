using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HWStatus;
using System.Data;
using RemoteInterface.MFCC;


namespace MFCC_RD
{
    class RemoteObj:Comm.MFCC.RemoteMFCCBase,I_MFCC_RD
    {



        public override object InitializeLifetimeService()
        {
            return null; //base.InitializeLifetimeService();
        }


     


        public override Comm.MFCC.MFCC_Base getMFCC_base()
        {

            return Program.mfcc_rd;
           
        }

      



       


        public void loadValidCheckRule()
        {


            try
            {
                Program.mfcc_rd.loadValidCheckRule();
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }


        

        public void setTransmitCycle(string devName,int cycle)
        {
           // throw new Exception("The method or operation is not implemented.");
            try
            {
                Comm.TC.RDTC tc = (Comm.TC.RDTC)Program.mfcc_rd.getTcManager()[devName];
                checkAllowConnect(tc);
                tc.SetTransmitCycle(cycle);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message+ex.StackTrace);
            }
        }


        public void getCurrentRDData(string devName, ref DateTime dt, ref int amount, ref int acc_amount, ref int degree)
        {
            try
            {
                Comm.TC.RDTC tc = (Comm.TC.RDTC)Program.mfcc_rd.getTcManager()[devName];
                checkAllowConnect(tc);
                tc.getCurrentRDData(ref dt, ref amount,ref acc_amount, ref degree);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message+","+ex.StackTrace);
            }
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
