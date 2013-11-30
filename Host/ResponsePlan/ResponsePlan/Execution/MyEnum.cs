using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    internal enum LCSLight
    {
        /// <summary>
        /// 熄滅
        /// </summary>
        snuff = 0,
        /// <summary>
        /// ↓
        /// </summary>
        down = 1,
        /// <summary>
        /// ╳
        /// </summary>
        forbid = 2,
        /// <summary>
        /// ↘
        /// </summary>
        rightDown = 3,
        /// <summary>
        /// ↙
        /// </summary>
        leftDown = 4
    }

    internal enum DeviceType
    {
        CMS,
        RGS,
        WIS,
        CSLS,
        LCS,
        RMS,
        CCTV,
        FS
    }

    /// <summary>
    /// 事件分類
    /// </summary>
    enum CategoryType
    {
        /// <summary>
        /// 一般事件
        /// </summary>
        GEN,    //一般事件
        /// <summary>
        /// 壅塞事件
        /// </summary>
        OBS,    //壅塞事件
        /// <summary>
        /// 天候事件
        /// </summary>
        WEA,    //天候事件
        /// <summary>
        /// 隧道機電事件
        /// </summary>
        TUN,    //隧道機電事件
        /// <summary>
        /// 管制事件
        /// </summary>
        RES,    //管制事件
        /// <summary>
        /// 其他事件
        /// </summary>
        OTH,    //其他事件
    }

    /// <summary>
    /// 等級
    /// </summary>
    internal enum Degree
    {
        /// <summary>
        /// 無
        /// </summary>
        N = 0,   //無
        /// <summary>
        /// 低
        /// </summary>
        L = 1,    //低
        /// <summary>
        /// 中
        /// </summary>
        M = 2, //中
        /// <summary>
        /// 高
        /// </summary>
        H = 3,  //高
        /// <summary>
        /// 超高
        /// </summary>
        S = 4  //超高
    }

    /// <summary>
    /// 訊息型態
    /// </summary>
    internal enum MegType
    {
        /// <summary>
        /// 警告
        /// </summary>
        A,    //警告
        /// <summary>
        /// 非強制
        /// </summary>
        U, //非強制
        /// <summary>
        /// 強制
        /// </summary>
        F   //強制
    }

    public enum EventStatus
    {
        /// <summary>
        /// 確認中
        /// </summary>
        Enter = 1,              //確認
        /// <summary>
        /// 等待中
        /// </summary>
        Wait = 2,               //等待
        /// <summary>
        /// 放棄
        /// </summary>
        Abort = 6,              //放棄
        /// <summary>
        /// 中止
        /// </summary>
        Terminate = 5,          //中止
        /// <summary>
        /// 結束
        /// </summary>
        End = 4,                //結束
        /// <summary>
        /// 執行中
        /// </summary>
        Execute = 3             //執行
    }

    public enum Notifier
    {
        /// <summary>
        /// 緊急電話
        /// </summary>
        ExyTel = 1,     //緊急電話
        /// <summary>
        /// 專線電話
        /// </summary>
        SpyTel = 2,     //專線電話
        /// <summary>
        /// 無線電話
        /// </summary>
        Wireless = 3,   //無線電話
        /// <summary>
        /// 市內電話
        /// </summary>
        LocalTel = 4,   //市內電話
        /// <summary>
        /// 手機
        /// </summary>
        Mobile = 5,     //手機
        /// <summary>
        /// 偵測器
        /// </summary>
        Device = 6,     //偵測器
        /// <summary>
        /// 自動偵測
        /// </summary>
        Auto = 7        //自動偵測
    }

    /// <summary>
    /// 更新反應計劃資料
    /// </summary>
    public enum RenewData
    {
        All
    }


    public enum LoginMode
    {
        /// <summary>
        /// 自動
        /// </summary>
        Auto = 1,       //自動
        /// <summary>
        /// 半自動
        /// </summary>
        Half = 2,       //半自動
        /// <summary>
        /// 手動
        /// </summary>
        Manual = 3      //手動
    }
}
