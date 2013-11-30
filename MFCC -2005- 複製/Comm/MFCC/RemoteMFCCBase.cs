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
                throw new RemoteInterface.RemoteException(ex.Message+ex.StackTrace);
            }
        }


        public System.Data.DataSet sendTC(string TC_name, System.Data.DataSet ds)
        {

            try
            {
              //  ConsoleServer.WriteLine("Send TC " + TC_name + ds.Tables[0].Rows[0].ToString());
                using (DataSet retDs = getMFCC_base().SendTC(TC_name, ds))
                {
                    if(retDs!=null)
                       retDs.AcceptChanges();
                    return retDs;
                }
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message+ex.StackTrace);
            }
            // throw new Exception("The method or operation is not implemented.");
        }

        public System.Data.DataSet sendTC(string ip, int port, System.Data.DataSet ds)
        {

            try
            {
                using (DataSet retDs = getMFCC_base().SendTC(ip, port, ds))
                {
                    retDs.AcceptChanges();
                    return retDs;
                }
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message+ex.StackTrace);
            }
            // throw new Exception("The method or operation is not implemented.");
        }

        public void ResetComm(string tc_name)
        {
            Comm.TCBase tc = null;
            try
            {
                tc = (Comm.TCBase)getMFCC_base().getTcManager()[tc_name];

              //  checkAllowConnect(tc);

                tc.ResetComm();
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
        }

        public string getCurrentTcCommStatusStr(string tc_name)
        {
            Comm.TCBase tc = null;
            try
            {
                tc = (Comm.TCBase)getMFCC_base().getTcManager()[tc_name];

             //   checkAllowConnect(tc);

                return tc.getCurrentTcCommStatusStr() + "MFCC dbqcnt:" + getMFCC_base().getDbServerCurrentQueueCnt();
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }


        }

       public  void setDeviceCommMointer(string devName,bool bStartEnd)
        {

            try
            {
                getMFCC_base().setDeviceCommMoniter(devName,bStartEnd);

               
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
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
                throw new RemoteInterface.RemoteException(ex.Message+ex.StackTrace);
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
                throw new RemoteInterface.RemoteException(ex.Message+ex.StackTrace);
            }

        }

     
        public I_HW_Status_Desc getHWdesc(string tc_name)
        {

            Comm.TCBase tc = null;
            try
            {
                tc = (Comm.TCBase)getMFCC_base().getTcManager()[tc_name];

                checkAllowConnect(tc);

                return new VD_HW_StatusDesc(tc.DeviceName,tc.getHwStaus());
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message+ex.StackTrace);
            }


            // throw new Exception("The method or operation is not implemented.");
        }

        public I_HW_Status_Desc getHWdesc(string ip, int port)
        {
            try
            {
                Comm.TCBase tc = (Comm.TCBase)getMFCC_base().getTcManager()[ip, port];
                checkAllowConnect(tc);
                return new VD_HW_StatusDesc(tc.DeviceName,tc.getHwStaus());
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message+ex.StackTrace);
            }

            //throw new Exception("The method or operation is not implemented.");
        }
        public void AddDevice(string devName)
        {
            try
            {
              getMFCC_base().AddDevice(devName);
              
               
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message+ex.StackTrace);
            }
        }

        public void Remove(string devName)
        {
            try
            {
                getMFCC_base().Remove(devName);


            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message+ex.StackTrace);
            }
        }
        protected void checkAllowConnect(Comm.TCBase tc)
        {

            if (tc == null)
                throw new RemoteInterface.RemoteException("無此tc!");
            if (!tc.IsConnected)
                throw new RemoteInterface.RemoteException("tc 未連線");
        }

        public abstract MFCC_Base getMFCC_base();
        public void ChangeDisplayCheckCycle(string tcname, int min)
        {
            try
            {

                Comm.OutputTCBase tc = (Comm.OutputTCBase)getMFCC_base().getTcManager()[tcname];
                tc.ChangeDisplayCheckCycle(min);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message+ex.StackTrace);
            }
        }

        public void ChangeDisplayCheckCycle(string ip, int port, int min)
        {
            try
            {

                Comm.OutputTCBase tc = (Comm.OutputTCBase)getMFCC_base().getTcManager()[ip, port];
                tc.ChangeDisplayCheckCycle(min);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message+ex.StackTrace);
            }

        }
        public bool getConnectionStatus(string tc_name)
        {
            try
            {
                Comm.TCBase tc = (Comm.TCBase)getMFCC_base().getTcManager()[tc_name];

                return tc.IsConnected;
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message+ex.StackTrace);
            }
        }
        public  bool getConnectionStatus(string ip, int port)
        {

            try
            {
                Comm.TCBase tc = (Comm.TCBase)getMFCC_base().getTcManager()[ip, port];

                return tc.IsConnected;
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message+ex.StackTrace);
            }
        }
        public void  getDeviceStatus(string devName, ref byte[] hw_status, ref byte opmode, ref byte opstatus, ref bool isConnected)
        {
            try
            {
                Comm.TCBase tc = (Comm.TCBase)getMFCC_base().getTcManager()[devName];

                hw_status=tc.getHwStaus();
                opmode=tc.m_opmode;
                opstatus=tc.m_opstatus;
                isConnected=tc.IsConnected;
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message+ex.StackTrace);
            }

        }
        public void downLoadConfigParam(string devName)
        {
            try
            {
                Comm.TCBase tc = (Comm.TCBase)getMFCC_base().getTcManager()[devName];
                checkAllowConnect(tc);
                tc.DownLoadConfig();
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message+ex.StackTrace);
            }
            // throw new Exception("The method or operation is not implemented.");
        }
        public void setCompareOutputMin(string devName, int min)
        {

            try{
                if (!this.getMFCC_base().getTcManager().IsContains(devName))
                    throw new Exception(devName+" not found!");

                TCBase tc = this.getMFCC_base().getTcManager()[devName];
                if (tc is OutputTCBase)
                    ((OutputTCBase)tc).setDisplayCompareCycle(min);
                else
                    throw new Exception(devName+" do not support this command!");
            }
           catch (Exception ex)
            {
                throw new RemoteException(ex.Message+ex.StackTrace);
            }


        }
   
        public void setSysDateTime(string devName)
        {

            try
            {
                if (!this.getMFCC_base().getTcManager().IsContains(devName))
                    throw new Exception(devName + " not found!");

                TCBase tc = this.getMFCC_base().getTcManager()[devName];
                DateTime dt=System.DateTime.Now;
                tc.TC_SetDateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }

        }
       //  void downLoadConfigParam(string devName)
    }
}
