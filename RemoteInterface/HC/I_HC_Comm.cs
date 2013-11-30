using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using RemoteInterface.MFCC;

namespace RemoteInterface.HC
{
    public interface I_HC_Comm
    {
        DataSet getSendDsByFuncName(string devType, string funcName);
        string getScriptSource(string devType);

        void setVDFiveMinData(string devName,RemoteInterface.MFCC.VD1MinCycleEventData data);
        void setRDEventData(string devName, DateTime dt, int pluviometric, int degree);
        void SetETTUCCTVLock(string etid);
        void setRDFiveMinData(string devName,DateTime dt, int ammount, int acc_amount, int degree);
        
        void setWDTenMinData(string DeviceName,DateTime dt,int average_wind_speed, int average_wind_direction,int  max_wind_speed,int  max_wind_direction, int am_degree);
        void setWDEventData(string devName, DateTime dt, int average_wind_speed, int average_wind_direction, int max_wind_speed, int max_wind_direction, int am_degree);

        void setVIFiveMinData(string devName, DateTime dt, int distance, int degree);
        void setVIEventData(string devName, DateTime dt, int distance, int degree);
        void setBS_EventData(string devName,DateTime dt, int slope, int shift, int sink, int level);

        string getTCCommStatusStr(string devName);

        void ResetComm(string devName);
        DateTime getDateTime();

        System.Collections.ArrayList getDeviceNames(string devType);
        void setDeviceStatus(string devName, byte[] hw_status, byte opstatus, byte opmode,bool isConnected);
        string getAllSchduleStatus();
        string getJamRangeString();
        string getEventString();
        void AddAviData(AVIPlateData data);
        void setLSEventData(string devName,DateTime dt, int mon_var, int day_var, int degree);
        void setLS10MinData(string devName,DateTime dt, int mon_var, int day_var, int degree);

        void DoVD_InteropData(string devName, System.DateTime dt);
        void setTemEvent(string devName, object temEventData);
        void setIIDEvent(int camid,int laneid,int iideventid, int action);
        int getTEMLCSStatus(string devName);
        string getRedirectStatusString();
        void setRMS_LTR_Start(string devName);
        void setRMS_LTR_Stop(string devName);
        void setRMS_RampControl_Start(string devName);
        void setRMS_RampControl_Stop(string devName);
        int getCurrentDBQueueCnt();  
        void dbExecute(string sqlcmd);
        string GetAllVDInfo();
        void ReStartFiveMinAvgDataManager();
        void SetDbqMode(DBQueueMode mode);
        string getTIMCCSecManagerStatus();
      //  void setConnecttionStatus(string devName, bool isConnect);
        
    }
    public enum DBQueueMode
    {
        Normal,
        Reject,
        Slow
    }
}
