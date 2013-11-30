using System;
using System.Collections.Generic;
using System.Text;

namespace ReportForm
{
    #region ==== 列舉 ====
    public enum RptHander
    {
        //有主重報表
        一天交通資料 = 0, //ctrlRPT_DATA_00
        五分鐘交通資料記錄報表 = 1, //ctrlRPT_DATA_01
        五分鐘車道使用率及車間距報表 = 2,//ctrlRPT_DATA_02
        一分鐘交通資料記錄報表 = 3,//ctrlRPT_DATA_03
        一分鐘車道使用率及車間距報表 = 4,//ctrlRPT_DATA_04
        現點速率調查交通資料記錄報表 = 5,//ctrlRPT_DATA_06
        VD詳細記錄報表 = 6,//ctrlRPT_DATA_24
        VD總計記錄報表 = 7,//ctrlRPT_DATA_25
        小時交通資料記錄報表 = 8,//ctrlRPT_HDA_01
        小時交通流量統計報表依日期時段彙整 = 9,//ctrlRPT_HDA_03
        小時交通平均速度統計報表 = 10,//ctrlRPT_HDA_05
        匝道平均每日交通量統計報表 = 11,//ctrlRPT_HDA_11

        //無主重報表
        主線平均每日交通量統計報表 = 12,//ctrlRPT_HDA_10
        全區匝道全日交通量統計報表 = 13,//ctrlRPT_HDA_12
        全區主線全日交通量統計報表 = 14,//ctrlRPT_HDA_13
        全區主線小時路段平均速度統計報表 = 15,//ctrlRPT_HDA_14
        資訊可變標誌即時資料報表 = 16,//ctrlRPT_MON_01
        設備狀態即時監視報表 = 17,//ctrlRPT_MON_07
        每日定時路況新聞稿 = 22,//ctrlRPT_MON_07
        路段壅塞狀況一分鐘報表 = 23,//ctrlRPT_HDA_35



        //無主重報表,但有種類
        操作記錄報表 = 18,//ctrlRPT_OPR1_01
        現場終端設備運作記錄報表 = 19,//ctrlRPT_OPR2_06
        定時比對記錄報表 = 20,//ctrlRPT_OPR2_07
        現場終端設備狀態記錄報表 = 21//ctrlRPT_STA_01

    }
    #endregion ==== 列舉 ====
}
