using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using RemoteInterface.HC;

namespace RemoteInterface.HC
{
    public   interface I_HC_FWIS
    {
        void RGS_setManualGenericDisplay(string devicename, RGS_GenericDisplay_Data data,bool bForce);
      
        void CMS_setManualDisplay(string devicename, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors,bool bForce);
        void CMS_setManualDisplay(string devicename, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors,byte[] vspaces, bool bForce);
       
        void setManualModeOff(string deviceName);
        void setManualModeOff(string deviceName, bool bForce);

        void TTS_setManualModeOff(string devName,int boardid, bool bForce);
        void MAS_setManualModeOff(string devName, int laneid, bool bForce);
        void RMS_setManualModeAndPlan(string devicename, int mode, int planno,bool bForce);
      
        void WIS_setManualDisplay(string devicename, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors, bool bForce);
        void SetOutput(string deviceName,object  OutputData,bool bForce);

        VD5MinMovingData[] GetAllVD5MinAvgData();
        VD5MinMovingData GetVD5MinAvgData(string devName);

        void SetDbChangeNotify(DbChangeNotifyConst notifyConst, params object[] args);
        object GetCurrentOutput(string devname,ref int priority);

        void GetAllTrafficData(string lineid, string dir, int startMile,int endMile, ref int volume, ref int speed, ref int occupancy, ref int  jameLevel, ref int travelSec);
       // void GetAllTrafficData(string lineid, string dir, int startMile, int endMile, ref int volume, ref int speed, ref int occupancy, ref int jameLevel, ref int travelSec,int travelUpper);
         void GetTravelUpperLimitLowerLimtByRange(string lineid, string dir, int startMile, int endMile, ref int lowerTravelTime, ref int upperTravelTime);
        void GetAllTrafficDataByUnit(string lineid, string dir, int unitStartKm, ref int volume, ref int speed, ref int occupancy, ref int jameLevel, ref int travelSec, ref string[] vdList);
        
        void AddDevice(string devName);
        void RemoveDevice(string devName);
        void GetDeviceStatus(string devName, ref byte[] hw_status, ref byte opmode, ref byte opstatus, ref bool isConnected);
        void GetRD5MinData(string devName, ref System.DateTime lastReportTIme, ref int amount, ref int acc_amount, ref int degree);
        void LoadSchedule(int schid);
        void LoadScheduleByManualPriority(int schid);
        void RemoveSchedule(int schid);

        OutputQueueData[] getOutputQueueData(string devName);
       
        FetchDeviceData[] Fetch(string[] deviceTypes, string lineId, string direction, int mileage, int segCnt, int sysSegCnt, bool IsBranch);
        FetchDeviceData[] Fetch(string[] deviceTypes, string lineId, int startMileage, int endMileage);
        FetchDeviceData[] Fetch(string[] deviceTypes, string lineId,string direction, int startMileage, int endMileage);
        System.Data.DataSet getSendDs(string DeviceType, string func_name);
        void setEventStatus(int evtid,int status);

        TravelTimeData[] getTravelTimeData(string devName);
        void InputIIP_Event(int evtid);
        void GenExecutionPlan(int evtid);
        void SetMovingContructEvent(int id, string notifier, DateTime timeStamp, string lineID, string directionID, int startMileage, int endMileage, int blockTypeId, string blocklane, string description);
        void SetMovingContructEvent(int id, string notifier, DateTime timeStamp, string lineID, string directionID, int startMileage, int endMileage, int blockTypeId, string blocklane, string description, string isExecute);

        void CloseMovingConstructEvent(int id);
        int Get_VD_TravelTime(string lineid, string dir, int startmile_m, int endmile_m);
        int Get_AVI_TravelTime(string lineid, string dir, int startmile_m, int endmile_m);
        int Get_ETC_TravelTime(string lineid, string dir, int startmile_m, int endmile_m);
        int Get_HIS_TravelTime(string lineid, string dir, int startmile_m, int endmile_m);
        void SetManualEvent(int eventclass, int evenitid, string lineid, string direction, int startMileage, int endMileage, int level);
        void ReNewManualEvent(int eventid, int newevtid, int startMileage, int endMileage, int level);
        int getTimccTravelTimeByRange(string lineid, string dir, int startmile, int endmile);
        int SendSMS(string phoneNo, string body);
        void LockCCTV(int cctvid, string desc, string desc2, int preset);
        void ReLoadEventExecutionOutput(int evtid);
        void NotifyUserTIMCCPlayData(TIMCC_RespInstruction instruction);
        void SetETTUCCTVLock(string etid);
    }
}
