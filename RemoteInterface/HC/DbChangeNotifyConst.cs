using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HC
{
    public enum DbChangeNotifyConst
    {
        JamEvalTable,  //壅塞評估表的定改變
        TravelSettingData,  //RGS CMS 旅行時間設定改變
        SectionPolygonMapingData, //都會路網對應VD設定改變
        UnitRoadVDMapping  ,  //單位路段指定VD 改變
        AVISampleInterval,
        ETC_IP_Change,
        RediretRoute_Change,
        AID_PARAMETER_Change,
        Reload_Device_Loaction,
        Reload_Section_WeightTable,
        CMSRST_Manual_MessageChange,
        Enable_Weather_Change,
        ETAG_Life_Minutes_Change
    }

}
