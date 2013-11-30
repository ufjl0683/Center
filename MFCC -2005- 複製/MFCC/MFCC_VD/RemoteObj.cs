using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HWStatus;
using System.Data;
using RemoteInterface.MFCC;


namespace MFCC_VD
{
    class RemoteObj:Comm.MFCC.RemoteMFCCBase,I_MFCC_VD
    {



        public override object InitializeLifetimeService()
        {
            return null; //base.InitializeLifetimeService();
        }


        public  void setRealTime(string tc_name, int laneid, int cycle,int durationMin)
        {
            Comm.TC.VDTC tc = (Comm.TC.VDTC)Program.mfcc_vd.getTcManager()[tc_name];
            checkAllowConnect(tc);
        //    Comm.SendPackage pkg = new Comm.SendPackage(Comm.CmdType.CmdSet, Comm.CmdClass.B,0xffff, new byte[] { 0x11, (byte)laneid });

            try
            {
                tc.SetRealData(laneid, cycle, durationMin);
                
            }
            catch(Exception ex)

            {
                throw new RemoteInterface.RemoteException(ex.Message + "\r\n" + ex.StackTrace);
            }
            // throw new Exception("The method or operation is not implemented.");


        }

        public   void setRealTime(string ip, int port, int laneid,int cycle,int durationMin)
        {

            Comm.TC.VDTC tc = (Comm.TC.VDTC)Program.mfcc_vd.getTcManager()[ip, port];
            checkAllowConnect(tc);
         
            try
            {
                tc.SetRealData(laneid, cycle, durationMin);
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + "\r\n" + ex.StackTrace);
            }
         
        }


        public override Comm.MFCC.MFCC_Base getMFCC_base()
        {

            return Program.mfcc_vd;
           
        }

        public VD1MinCycleEventData getVDLatest5MinAvgData(string devname)
        {
            try
            {
                Comm.TC.VDTC tc = (Comm.TC.VDTC)Program.mfcc_vd.getTcManager()[devname];
                checkAllowConnect(tc);
                 return tc.getLatestFivMinAvgData().ToVD1MinCycleEventData();
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message+ex.StackTrace);
            }
        }


        public VD1MinCycleEventData getVDLatest1MinData(string devname)
        {
            try
            {
                Comm.TC.VDTC tc = (Comm.TC.VDTC)Program.mfcc_vd.getTcManager()[devname];
                checkAllowConnect(tc);
                return tc.curr_1_min_data.ToVD1MinCycleEventData();
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
        }

       


        public void loadValidCheckRule()
        {
            try
            {
                Comm.TC.VDDataValidCheck.LoadRule();
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message+ex.StackTrace);
            }
        }


        

        public void setTransmitCycle(string devName,int cycle)
        {
           // throw new Exception("The method or operation is not implemented.");
            try
            {
                Comm.TC.VDTC tc = (Comm.TC.VDTC)Program.mfcc_vd.getTcManager()[devName];
                checkAllowConnect(tc);
                tc.SetTransmitCycle(cycle);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message+ex.StackTrace);
            }
        }


        public void enableVD20SecData(string devName,bool enabled)
        {
            try
            {
                Comm.TC.VDTC tc = (Comm.TC.VDTC)Program.mfcc_vd.getTcManager()[devName];
                checkAllowConnect(tc);
                tc.enable20SecEvent(enabled);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
        }







      



      
    }
}
