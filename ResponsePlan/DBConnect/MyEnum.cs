using System;
using System.Collections.Generic;
using System.Text;

namespace DBConnect
{
    public enum DB2TableName
    {
        tblIIPEvent,            //反應計畫事件資料表
        tblSysParameter,
        tblDeviceConfig,        //設備組態表
        tblSysAlarmConfig,      
        tblSysAlarmList,
        tblSysAlarmLog,         //系統警報記錄檔
        //tblIidStateLog,       //IID偵測紀錄表
        tblSysAlarmType,        //系統警報事件種類表
        tblRspExecution,        //事件反應計劃產生結果
        tblIIPService,          //反應計畫聯絡事項
        tblIIPCommService,
        tblIIPCommServiceEvent,
        tblIIPServiceType,
        tblRSPExecutionOutputData,
        tblCMSConfig,
        tblRSPRule,
        tblRSPDeviceRange,
        tblRSPMessColor,
        tblRSPDefaultMess,
        tblRSPDefaultMessMAS,
        tblGroupSection,
        tblGroupLine,
        tblRSPSerious,
        tblRSPMessRule,
        tblRSPExecutionOutputDataMsg,
        tblRSPBlock,
        tblRSPBlockDetail,
        tblRSPSeriousByAlarmClass,
        tblGroupTunnel,
        tblRmsConfig,
        tblRgsConfig,
        tblRspPriority,
        tblHostConfig,
        tblGroupDivision,
        tblRSPCSLSSpeed,
        tblIIPCSLSParam,
        tblRSPDevice,
        tblRSPServiceRange,
        tblCloverleaf,
        vwGroupSection,
        vwGroupDivision
    }

    public enum SqlServerTableName
    {
        monitor_CONFIGURATION,
        BBSCH_CONFIGURATION,
        SERVER_CONFIGURATION,
        PTZ_PRESET,
        CAMERA_CONFIGURATION
    }

    public enum DataType
    {
        SysAlarmLog,
        IIPEvent,
        Unit,
        Execution,
        OutputData,
        CmsCategory,
        Decorators,
        MessColor,
        LaneCount,
        MessRule,
        EventID,
        Renew,
        AllMoveEvent,
        BlockMeg1,
        BlockMeg2,
        RgsCategory,
        Tunnel,
        CSLS,
        RMS,
        RGS,
        TestExe,
        TestIip,
        Priority,
        Host,
        EndSectionName
    }
}