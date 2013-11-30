using System;
using System.Collections.Generic;
using System.Text;
using DBConnect;

namespace Execution.Command
{
    internal class GetSelectCmd
    {
        static string schema = ODBC_DB2Connect.schema;

        /// <summary>
        /// 取得自動、半自動資料表
        /// </summary>
        /// <param name="eventID">事件編號</param>
        /// <returns>SQL命令</returns>
        public static ICommand getLogCmd(int eventID)
        {
            SelectCommand selectCmd = new SelectCommand();

            string str = "";
            str += " list.FLOWID,cty.alarmtype,list.alarmclass,list.DEGREE, ";
            str += " list.TIMESTAMP as startTime,list.endTime, list.STARTMILEAGE,";
            str += " list.ENDMILEAGE, list.DEVICENAME,list.LINEID,list.DIRECTION,list.SectionId, ";
            str += " list.IFHANDLE,list.IFCLOSE,list.IFERROR, ";
            str += " log.Lane_id,log.Cam_id,log.DivisionID,log.eventid,log.memo, ";
            str += " cty.login_mode,log.originaleventid, ";
            str += " log.MC_ID,log.MC_Notifier,log.MC_blocktypeid,log.MC_blocklane,mc_memo ";
            selectCmd.FiledNames = str;

            str = "";
            str += string.Format(" {0}.{1} list left join {0}.{2} cty on cty.alarmclass = list.alarmclass ", schema, DB2TableName.tblSysAlarmList, DB2TableName.tblSysAlarmConfig);
            str += string.Format("              left join {0}.{1} log on list.ORIGINALKEY = log.eventid ", schema, DB2TableName.tblSysAlarmLog);
            selectCmd.TblNames = str;

            str = "";
            str += string.Format(" type='S' and ORIGINALKEY={0}   ", eventID);
            selectCmd.WhereCon = str;

            return selectCmd;
        }


        /// <summary>
        /// 取得反應計劃事件資料表
        /// </summary>
        /// <param name="devName">設備名稱</param>
        /// <returns>命令共同介面</returns>
        public static ICommand getIIPEventCmd(string id,bool isEventId)
        {
            SelectCommand com = new SelectCommand();
            //com.FiledNames = " * ";

            com.FiledNames += " eve.INC_ID,eve.inc_type_name,eve.inc_name,eve.incidentid, ";
			com.FiledNames += " eve.inc_congestion,inc_serverity,inc_lineid,inc_direction, ";
            com.FiledNames += " eve.inc_location,eve.inc_interchange,eve.from_milepost1, ";
            com.FiledNames += " eve.to_milepost1,inc_blockage,eve.inc_login_mode,eve.inc_time, ";
            com.FiledNames += " eve.inc_step_times,eve.inc_stepno,eve.inc_status,eve.inc_spreadnews, ";
            com.FiledNames += " eve.control_volumn_level,eve.roadnet_turnto,eve.delay_time, ";
            com.FiledNames += " eve.entrance,eve.eventid,sec.SECTIONNAME,line.LINENAME,type.DESCRIPTION as alarmTypename, ";
            com.FiledNames += " con.DESCRIPTION as alarmclassName,con.shortname,line.g_code_id as LINE_ICON, ";
            com.FiledNames += " con.G_CODE_ID as ACC_ICON,sec.G_CODE_ID_CITY,sec.G_CODE_ID_PATH,sec.Lane_count,eve.BlockTypeId, ";
            com.FiledNames += " line.DIRECTION as line_Direction,line.STARTMILEAGE as line_s_mile,line.ENDMILEAGE  as line_e_mile ";
            com.TblNames += string.Format(" {0}.{1} eve left join(select * from {0}.{2}) sec on sec.sectionid=eve.inc_location ", schema, DB2TableName.tblIIPEvent, DB2TableName.tblGroupSection);
            com.TblNames += string.Format(" left join(select * from {0}.{1}) line on line.lineid=eve.inc_lineid ", schema, DB2TableName.tblGroupLine);
            com.TblNames += string.Format(" left join(select * from {0}.{1}) type on type.alarmtype=eve.Inc_type_name ", schema, DB2TableName.tblSysAlarmType);
            com.TblNames += string.Format(" left join(select * from {0}.{1}) con on con.alarmclass=eve.Inc_name ", schema, DB2TableName.tblSysAlarmConfig);

            com.WhereCon += string.Format(" eve.INC_STATUS = {0} ", (int)EventStatus.Enter);
            if (!string.IsNullOrEmpty(id))
            {
                if (!isEventId)
                    com.WhereCon += string.Format(" and eve.INC_ID = '{0}'", id);
                else
                    com.WhereCon += string.Format(" and eve.EventID = {0}", id);
            }
            return com;
        }

        public static ICommand getIIPEventCmd(int eventid)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += " * ";
            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblIIPEvent);
            com.WhereCon += string.Format(" eventid = {0} ", eventid);
            return com;
        }

        public static ICommand getMoveEventCmd(int alarmClass)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += " * ";
            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblIIPEvent);
            com.WhereCon += string.Format(" INC_NAME={0} and INC_STATUS in ({1},{2},{3}) ", alarmClass, (int)EventStatus.Enter, (int)EventStatus.Execute, (int)EventStatus.Wait);
            return com;
        }

        /// <summary>
        /// 取得連絡單位資料表
        /// </summary>
        /// <param name="alarmclass">次類別編號</param>
        /// <param name="sectionid">路段編號</param>
        /// <returns></returns>
        public static ICommand getUnitData(byte alarmclass, int start_mile, int end_mile)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" ser.serviceid,type.subserviceid,servicename, ", 0);
            com.FiledNames += string.Format(" event.alarmclass,event.ifalarm,type.phone,type.fax,type.start_mileage,type.END_MILEAGE ", 0);

            com.TblNames += string.Format(" {0}.{1} ser, ", schema,DB2TableName.tblIIPCommService);
            com.TblNames += string.Format(" {0}.{1} event, ", schema, DB2TableName.tblIIPCommServiceEvent);
            com.TblNames += string.Format(" {0}.{1} type ", schema, DB2TableName.tblIIPServiceType);
            
            com.WhereCon += string.Format("     ser.SERVICEID=event.SERVICEID ");
            com.WhereCon += string.Format(" and type.SERVICEID=event.SERVICEID ");
            com.WhereCon += string.Format(" and event.alarmclass = {0}", alarmclass);
            com.WhereCon += string.Format(" and (type.start_mileage<={0} or type.start_mileage<={1}) ", start_mile, end_mile);
            com.WhereCon += string.Format(" and (type.END_MILEAGE>{0} or type.END_MILEAGE>{1}) ", start_mile, end_mile);
            return com;
        }

        public static ICommand getRspExecution(string rspId)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" * ", 0);
            com.TblNames += string.Format(" {0}.{1} exe ", schema,DB2TableName.tblRspExecution);
            com.WhereCon += string.Format("  exe.exe_id='{0}' ", rspId);
            return com;
        }

        public static ICommand getExeUnitData(string rspId, int alarmClass)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" iip.INC_ID,iip.COMM_TIME,eve.SERVICEID,iip.SUBSERVICEID,type.SUBSERVICENAME,type.PHONE,type.FAX,ser.SERVICENAME,iip.MEMO,eve.IFALARM ");

            com.TblNames += string.Format(" {0}.{1} iip left join(select * from {0}.{2}) type on type.subserviceid=iip.subserviceid ", schema, DB2TableName.tblIIPService, DB2TableName.tblIIPServiceType);
            com.TblNames += string.Format("             left join(select * from {0}.{1}) ser on ser.serviceid =type.serviceid  ", schema, DB2TableName.tblIIPCommService);
            com.TblNames += string.Format("             left join(select * from {0}.{1}) eve on ser.serviceid =eve.serviceid  ", schema, DB2TableName.tblIIPCommServiceEvent);

            com.WhereCon += string.Format("      iip.INC_ID='{0}' ", rspId);
            com.WhereCon += string.Format("  and eve.alarmclass={0} ", alarmClass);
            return com;
        }

        public static ICommand getOutputData(string rspId)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" device_type,outputdata1,devicename,priority ");
            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblRSPExecutionOutputData);
            com.WhereCon += string.Format(" exe_id='{0}' ", rspId);
            return com;
        }

        public static ICommand getOutputData(string rspId,string devName)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" device_type,outputdata1,devicename,priority ");
            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblRSPExecutionOutputData);
            com.WhereCon += string.Format(" exe_id='{0}' and deviceName='{1}' ", rspId, devName);
            return com;
        }

        public static ICommand getCMSCategory(int ruleid,int alarmclass,string devtype,int distance,string devName,string MegType,string lineid)
        {
            SelectCommand com = new SelectCommand();

            com.FiledNames += string.Format(" * ");

            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblRSPDefaultMess);

            com.WhereCon += string.Format(" ruleid={0} ", ruleid);
            com.WhereCon += string.Format(" and alarmclass={0} ", alarmclass);
            com.WhereCon += string.Format(" and devicetype='{0}' ", devtype);

            com.WhereCon += string.Format("and distance = case when (select sec.lineid ", 0);
            com.WhereCon += string.Format("                            from {0}.{1} dev, ", schema,DB2TableName.tblDeviceConfig);
            com.WhereCon += string.Format("                                 {0}.{1} sec  ", schema,DB2TableName.tblGroupSection);
            com.WhereCon += string.Format("                           where devicename='{0}' ", devName);
            com.WhereCon += string.Format("                             and sec.sectionid=dev.sectionid )  = '{0}' then {1}  else 3 end ", lineid, distance);

            com.WhereCon += string.Format(" and cmstype in (select category from {0}.{1} where Devicename='{2}') ",schema,DB2TableName.tblCMSConfig, devName);
            com.WhereCon += string.Format(" and level = '{0}' ", MegType);
            return com;
        }

        public static ICommand getRGSCategory(int ruleid, int alarmclass, string devtype, int distance, string devName, string MegType, string lineid)
        {
            SelectCommand com = new SelectCommand();

            com.FiledNames += string.Format(" * ");

            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblRSPDefaultMess);

            com.WhereCon += string.Format(" ruleid={0} ", ruleid);
            com.WhereCon += string.Format(" and alarmclass={0} ", alarmclass);
            com.WhereCon += string.Format(" and devicetype='{0}' ", devtype);

            com.WhereCon += string.Format("and distance = case when (select sec.lineid ", 0);
            com.WhereCon += string.Format("                            from {0}.{1} dev, ", schema, DB2TableName.tblDeviceConfig);
            com.WhereCon += string.Format("                                 {0}.{1} sec  ", schema, DB2TableName.tblGroupSection);
            com.WhereCon += string.Format("                           where devicename='{0}' ", devName);
            com.WhereCon += string.Format("                             and sec.sectionid=dev.sectionid )  = '{0}' then {1}  else 3 end ", lineid, distance);

            com.WhereCon += string.Format(" and level = '{0}' ", MegType);
            return com;
        }

        public static ICommand getDisplyDevTypes(Degree degree, int alarmClass, string sectionid)
        {
            SelectCommand com = new SelectCommand();
            if (degree == Degree.L)
                com.FiledNames += string.Format(" Ruleid,devicetype ,lowdevicestart as devStart,lownormal as normal,lowsystem as system,lowisextend as isextend ");
            else if (degree == Degree.M)
                com.FiledNames += string.Format(" Ruleid,devicetype ,middevicestart as devStart,midnormal as normal,midsystem as system,midisextend as isextend ");
            else if (degree == Degree.H)
                com.FiledNames += string.Format(" Ruleid,devicetype ,highdevicestart as devStart,highnormal as normal,highsystem as system,highisextend as isextend ");
            else if (degree == Degree.S)
                com.FiledNames += string.Format(" Ruleid,devicetype ,superdevicestart as devStart,supernormal as normal,supersystem as system,superisextend as isextend ");
            else
                com.FiledNames += string.Format(" Ruleid,devicetype ");

            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblRSPDeviceRange);

            com.WhereCon += string.Format(" Ruleid in (select Ruleid from {0}.{1} where RUNING='Y') ", schema, DB2TableName.tblRSPRule);
            com.WhereCon += string.Format(" and ALARMCLASS={0} ", alarmClass);
            com.WhereCon += string.Format(" and SECTIONID='{0}' ", sectionid);

            return com;
        }

        public static ICommand getMessColor()
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" * ");

            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblRSPMessColor);

            return com;
        }


        public static ICommand getSerious(int laneCount)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" * ");

            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblRSPSerious);

            com.WhereCon += string.Format(" lanecount = {0} ", laneCount);
            com.WhereCon += string.Format(" and Ruleid in (select Ruleid from {0}.{1} where RUNING='Y') ", schema,DB2TableName.tblRSPRule);
            return com;
        }

        public static ICommand getSeriousByAlarmClass(int alarmClass)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" * ");

            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblRSPSeriousByAlarmClass);

            com.WhereCon += string.Format(" alarmclass = {0} ", alarmClass);
            com.WhereCon += string.Format(" and Ruleid in (select Ruleid from {0}.{1} where RUNING='Y') ", schema, DB2TableName.tblRSPRule);
            return com;
        }

        public static ICommand getMessRule(string serverity,int type)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" * ");

            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblRSPMessRule);

            com.WhereCon += string.Format(" serverity = '{0}' ", serverity);
            com.WhereCon += string.Format(" and Ruleid in (select Ruleid from {0}.{1} where RUNING='Y') ", schema,DB2TableName.tblRSPRule);
            com.WhereCon += string.Format(" and type = {0}", type);
            return com;
        }

        public static ICommand gettblRSPExecutionOutputdataFlowID(string exe_id, string devicename, string device_type)
        {
            SelectCommand com = new SelectCommand();

            com.FiledNames += string.Format(" flowid ");

            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblRSPExecutionOutputData);

            com.WhereCon += string.Format(" exe_id='{0}'  ", exe_id);
            com.WhereCon += string.Format(" and devicename='{0}' ", devicename);
            com.WhereCon += string.Format(" and device_type='{0}' ", device_type);

            return com;
        }

        /// <summary>
        /// 取得事件編號
        /// </summary>
        /// <returns></returns>
        public static ICommand getEventID()
        {
            SelectCommand com = new SelectCommand();
            //select NEXTVAL FOR DB2INST1.SEQ_EVENT FROM SYSIBM.SYSDUMMY1
            com.FiledNames += string.Format(" NEXTVAL FOR {0}.SEQ_EVENT ", schema);

            com.TblNames += "SYSIBM.SYSDUMMY1";

            return com;
        }

        public static ICommand getBlockMeg1(string typeList)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" * ");

            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblRSPBlock);

            com.WhereCon += string.Format(" typeid in ({0}) ", typeList);
            return com;
        }

        public static ICommand getBlockMeg2(int typeId,int laneCount,string composition)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" * ");

            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblRSPBlockDetail);

            com.WhereCon += string.Format("     typeid = {0} ", typeId);
            com.WhereCon += string.Format(" and lanecount = {0} ", laneCount);
            com.WhereCon += string.Format(" and composition = '{0}' ", composition.Trim());
            return com;
        }

        public static ICommand getTunnelData(string id,string direction)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" * ");
            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblGroupTunnel);
            com.WhereCon += string.Format("     Tunnelid = '{0}' ", id.Trim());
            return com;
        }

        public static ICommand getTunnelData()
        {
            SelectCommand com = new SelectCommand();

            com.FiledNames += string.Format(" * ");
            com.TblNames += string.Format(" ( ");
            com.TblNames += string.Format(" ({0}) ", getTunnelData("S").getCommand());
            com.TblNames += string.Format(" union ");
            com.TblNames += string.Format(" ({0} {1}) ", getTunnelData("N").getCommand(), "desc");
            com.TblNames += string.Format(" union ");
            com.TblNames += string.Format(" ({0}) ", getTunnelData("E").getCommand());
            com.TblNames += string.Format(" union ");
            com.TblNames += string.Format(" ({0} {1}) ", getTunnelData("W").getCommand(), "desc");
            com.TblNames += string.Format(" )  a ");
            com.OrderBy += string.Format("  a.direction,a.row ");
            return com;
        }


        private static ICommand getTunnelData(string direction)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" row_number() over() as row,tun.tunnelid,tun.originalkey, ");
            com.FiledNames += string.Format(" tun.tunnelname,line.lineid,line.linename,sec.SECTIONID,sec.DIRECTION,line.startmileage as line_start, ");
            com.FiledNames += string.Format(" tun.startmileage as tun_Start,tun.endmileage as tun_end,line.endmileage as line_end ");

            com.TblNames += string.Format(" {0}.{1} tun left join (select lineid,sectionid,direction from {0}.{2}) sec on sec.sectionid=tun.sectionid, ", schema, DB2TableName.tblGroupTunnel,DB2TableName.tblGroupSection);
            com.TblNames += string.Format(" {0}.{1} line ", schema, DB2TableName.tblGroupLine);

            com.WhereCon += string.Format("     line.lineid=sec.lineid ");
            com.WhereCon += string.Format(" and sec.direction='{0}' ", direction.Trim());

            com.OrderBy += string.Format(" sec.direction,tun.startmileage ");
            return com;
        }

        private static ICommand getLineData()
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" * ");
            com.TblNames += string.Format(" {0}.{1}  ", schema, DB2TableName.tblGroupLine);
            return com;
        }

        public static ICommand getSectionSpeed(string sectionid)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" Rsp_CSLS_Decrease,Rsp_CSLS_Range,Rsp_MaxSpeed_Low,Rsp_MinSpeed_Low, ");
            com.FiledNames += string.Format(" Rsp_MaxSpeed_Mid,Rsp_MinSpeed_Mid,Rsp_MaxSpeed_High,Rsp_MinSpeed_High,Rsp_MaxSpeed_Super,Rsp_MinSpeed_Super ");
            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblGroupSection);
            com.WhereCon += string.Format("     sectionid = '{0}' ", sectionid.Trim());
            return com;
        }

        public static ICommand getRMSMode()
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" dev.devicename,dev.mile_m,dev.lineid,dev.sectionid,dev.direction, ");
            com.FiledNames += string.Format(" rsp_controlmode_low,rsp_planno_low, ");
            com.FiledNames += string.Format(" rsp_controlmode_mid,rsp_planno_mid, ");
            com.FiledNames += string.Format(" rsp_controlmode_high,rsp_planno_high, ");
            com.FiledNames += string.Format(" rsp_controlmode_super,rsp_planno_super ");
            com.TblNames += string.Format(" {0}.{1} dev, ", schema, DB2TableName.tblDeviceConfig);
            com.TblNames += string.Format(" {0}.{1} rms ", schema, DB2TableName.tblRmsConfig);
            com.WhereCon += string.Format("     dev.devicename=rms.devicename ");
            return com;
        }


        public static ICommand getRGSMode()
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" dev.devicename,dev.mile_m,dev.lineid,dev.sectionid,dev.direction,sec.G_CODE_ID_CITY,sec.G_CODE_ID_PATH ");
            com.TblNames += string.Format(" {0}.{1} dev left join(select * from {0}.{2}) sec on sec.sectionid=dev.sectionid, ", schema, DB2TableName.tblDeviceConfig,DB2TableName.tblGroupSection);
            com.TblNames += string.Format(" {0}.{1} rms ", schema, DB2TableName.tblRgsConfig);
            com.WhereCon += string.Format("     dev.devicename=rms.devicename ");
            return com;
        }

        public static ICommand getPriority(byte alarmclass,MegType type)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" * ");
            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblRspPriority);
            com.WhereCon += string.Format("     alarmclass={0} and level='{1}' ",alarmclass,type.ToString());
            return com;
        }

        public static ICommand getHostIP()
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" hostip ");
            com.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblHostConfig);
            com.WhereCon += "     HOSTID='HOST' ";
            return com;
        }

        public static ICommand getEndSectionName(string lineid,string direction,int mile)
        {
            SelectCommand com = new SelectCommand();
            com.FiledNames += string.Format(" s.sectionname ");
            com.TblNames += string.Format(" {0}.{1} s  ", schema, DB2TableName.tblGroupSection);
            com.TblNames += string.Format(" left join(select Divisionid,Divisionname,mileage from {0}.{1}) g1 on s.start_divisionid=g1.divisionid  ", schema, DB2TableName.tblGroupDivision);
            com.TblNames += string.Format(" left join(select Divisionid,Divisionname,mileage from {0}.{1}) g2 on s.end_divisionid=g2.divisionid ", schema, DB2TableName.tblGroupDivision);
            com.WhereCon += string.Format(" s.lineid='{0}' ", lineid);
            com.WhereCon += string.Format(" and s.direction='{0}' ", direction);
            com.WhereCon += string.Format(" and (g1.mileage-{0}<=0 and g2.mileage-{0}>0 ", mile);
            com.WhereCon += string.Format(" or   g1.mileage-{0}>0 and g2.mileage-{0}<=0) ", mile);
            return com;
        }
    }
}
