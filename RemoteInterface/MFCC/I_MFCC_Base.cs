using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace RemoteInterface.MFCC
{
  public  interface I_MFCC_Base
    {
        DataSet getSendDSByFuncName(string funcname);
        DataSet sendTC(string TC_name, DataSet ds);
        DataSet sendTC(string ip, int port, System.Data.DataSet ds);
        byte[] getHWstatus(string TC_name);  
        byte[] getHWstatus(string ip, int port);
        I_HW_Status_Desc getHWdesc(string tc_name);
        I_HW_Status_Desc getHWdesc(string ip, int port);
        void  ChangeDisplayCheckCycle(string tc_name,int min);
        void ChangeDisplayCheckCycle(string ip, int port, int min);
        bool getConnectionStatus(string tc_name);
        bool getConnectionStatus(string ip, int port);
        string getCurrentTcCommStatusStr(string tc_name);
        void ResetComm(string tc_name);
        void AddDevice(string devName);
        void Remove(string devName);
        void getDeviceStatus(string devName, ref byte[] hw_status, ref byte opmode, ref byte opstatus, ref bool isConnected);
        void setDeviceCommMointer(string devName,bool bStartEnd );
        void downLoadConfigParam(string devName);
        void setCompareOutputMin(string devName,int min);
        int setSysDateTime(string devName);
        void setDeviceEnable(string devName,bool isEnable);
        int getCurrentDBQueueCnt();
    }
}
