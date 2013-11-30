using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using RemoteInterface;
using RemoteInterface.HWStatus;
namespace Comm.MFCC
{
    public abstract class RemoteMFCCBase:RemoteInterface.RemoteClassBase
    {
        public System.Data.DataSet getSendDSByFuncName(string funcname)
        {
            //throw new Exception("The method or operation is not implemented.");
            try
            {
                using (DataSet ds = getMFCC_base().getSendDsByFuncName(funcname))
                    return ds;
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message);
            }
        }


        public System.Data.DataSet sendTC(string TC_name, System.Data.DataSet ds)
        {

            try
            {
                using (DataSet retDs = getMFCC_base().SendTC(TC_name, ds))
                {
                    return retDs;
                }
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message);
            }
            // throw new Exception("The method or operation is not implemented.");
        }

        public System.Data.DataSet sendTC(string ip, int port, System.Data.DataSet ds)
        {

            try
            {
                using (DataSet retDs = getMFCC_base().SendTC(ip, port, ds))
                {
                    return getMFCC_base().SendTC(ip, port, ds);
                }
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message);
            }
            // throw new Exception("The method or operation is not implemented.");
        }





        public byte[] getHWstatus(string TC_name)
        {
            try
            {
                Comm.TCBase tc = (Comm.TCBase)getMFCC_base().getTcManager()[TC_name];
                checkAllowConnect(tc);
                return tc.getHwStaus();
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message);
            }



        }

        public byte[] getHWstatus(string ip, int port)
        {
            try
            {
                Comm.TCBase tc = (Comm.TCBase)getMFCC_base().getTcManager()[ip, port];
                checkAllowConnect(tc);
                return tc.getHwStaus();
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message);
            }

        }



        public I_HW_Status_Desc getHWdesc(string tc_name)
        {

            Comm.TCBase tc = null;
            try
            {
                tc = (Comm.TCBase)getMFCC_base().getTcManager()[tc_name];

                checkAllowConnect(tc);

                return new VD_HW_StatusDesc(tc.getHwStaus());
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }


            // throw new Exception("The method or operation is not implemented.");
        }

        public I_HW_Status_Desc getHWdesc(string ip, int port)
        {
            try
            {
                Comm.TCBase tc = (Comm.TCBase)getMFCC_base().getTcManager()[ip, port];
                checkAllowConnect(tc);
                return new VD_HW_StatusDesc(tc.getHwStaus());
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }

            //throw new Exception("The method or operation is not implemented.");
        }

        protected void checkAllowConnect(Comm.TCBase tc)
        {

            if (tc == null)
                throw new RemoteInterface.RemoteException("無此tc!");
            if (!tc.IsConnected)
                throw new RemoteInterface.RemoteException("tc 未連線");
        }

        public abstract MFCC_Base getMFCC_base();
        
      

    }
}
