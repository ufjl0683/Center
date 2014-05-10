using System;
using System.Collections.Generic;
using System.Text;

namespace DBConnect
{
    public enum DB2TableName
    {
        tblIIPEvent,            //�����p�e�ƥ��ƪ�
        tblSysParameter,
        tblDeviceConfig,        //�]�ƲպA��
        tblSysAlarmConfig,      
        tblSysAlarmList,
        tblSysAlarmLog,         //�t��ĵ���O����
        //tblIidStateLog,       //IID����������
        tblSysAlarmType,        //�t��ĵ���ƥ������
        tblRspExecution,        //�ƥ�����p�����͵��G
        tblIIPService,          //�����p�e�p���ƶ�
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