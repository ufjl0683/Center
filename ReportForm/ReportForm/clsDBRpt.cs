/********************************************************************
 * -----    ----------   ----------  ------------------------------ *
 * 版次     修改日期     修改者      修改內容                       *
 * -----    ----------   ----------  ------------------------------ *
 * v0.1     2009.08.18   林俊傑      Class clsDBRpt Initial         *
 * v0.2     2009.10.14   林俊傑      Add many RPT functions         *
 *                                                                  *
 *                                                                  *
 ********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;    // for SQL Server 2005
using System.Collections;       // for ArrayList
using IBM.Data.DB2;
using IBM.Data.DB2Types;

namespace ReportForm
{
    public class clsDBRpt
    {
        public string sTableName_DeviceConfig = "tblDeviceConfig";
        public string sTableName_DeviceStatusLog = "tblDeviceStatusLog";
        public string sTableName_tblgroupline = "tblgroupline";
        public string sTableName_VD1Min = "tblVdData1Min";
        public string sTableName_VD5Min = "tblVdData5Min";
        public string sTableName_VD1Hr = "tblVdData1Hr";
        public string sTableName_VD1Day = "tblVdData1Day";
        public string sTableName_SysColumns = "SYSIBM.SYSCOLUMNS";
        public string sTableName_vwVDGroup = "vwVDGroup";
        public string sTableName_vwVDRampGroup = "vwVDGroup_Ramp";
        public string sTableName_vwSection = "vwSection";
        public string sTableName_vwRamp = "VWRPT_RAMP";
        public string sTableName_SpotSpeed = "tblVdDataSpotSpeed";
        public string sTableName_Parameter = "tblSysParameter";
        public string sTableName_VWRPT_VD1MIN = "VWRPT_VD1MIN";
        public string sTableName_VWRPT_VD5MIN = "VWRPT_VD5MIN";
        public string sTableName_VWRPT_VD1HR = "VWRPT_VD1HR";
        public string sTableName_VWRPT_VD1DAY = "VWRPT_VD1DAY";
        public string sTableName_VWRPT_VD5MIN_INTERVAL = "VWRPT_VD5MIN_INTERVAl";
        public string sTableName_VWRPT_VD1MIN_INTERVAL = "VWRPT_VD1MIN_INTERVAl";
        public string sTableName_VdConfig = "tblVdConfig";
        public string sTableName_GroupLine = "tblGroupLine";
        public string sTableName_GroupSection = "tblGroupSection";
        public string sTableName_GroupDivision = "tblGroupDivision";
        public string sFunction_MaxVol_Dev = "MaxVolume_DevList";
        public string sFunction_MaxVol_Dev_Ramp = "MaxVolume_DevList_Ramp";
        public string sFunction_DIV = "DIV";
        public string sFunction_DIV2 = "DIV2";
        public string sFunction_ZERO = "ZERO";
        public string sTableName_tblSysUser = "tblSysUser";
        public string sTableName_TBLSYSOPERATION = "TBLSYSOPERATION";
        public string DBNAME = "db2inst1";
        public string sTableName_DeviceStatus = "tblDeviceStatus";
        //shin ADD
        public string sTableName_vwVDGroupnotr = "VWVDGROUPNOTR";
        public string sTableName_vwVDGroup2 = "vwVDGroup2";
        public string sTableName_tblMFCCConfig = "tblMFCCConfig";
        public string sTableName_vwVDRampGroup2 = "vwVDGroup_Ramp2";
        public string sTableName_tbldevicestatelog = "tbldevicestatelog";
        public string sTableName_tblDeviceComparisonLog = "tblDeviceComparisonLog";
        public string sTableName_tblTrafficDataLogSection = "tblTrafficDataLogSection";

        string sErrMsg;
        string sConnstring;

        DB2Connection conn;
        //DB2Command cmd;
        //DB2DataReader dr;
        DB2DataAdapter da;


        #region "取得設備清單，並以 DataTable 方式回傳"
        public DataTable GetDeviceList(string DevType)
        {
            try
            {
                string selectCmd = "";
                selectCmd += "Select DeviceName ";
                selectCmd += "  From " + "db2inst1"  + "." + sTableName_DeviceConfig + " ";
                selectCmd += " Where Lower(Device_Type) = '" + DevType.ToString().Trim().ToLower() + "' ";
                selectCmd += " Order By DeviceName ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "取得系統清單，並以 DataTable 方式回傳"
        public DataTable GetDeviceList()
        {
            try
            {
                string selectCmd = "";
                selectCmd += "Select DISTINCT DEVICE_TYPE as OperationName ";
                selectCmd += "  From " + "db2inst1"  + "." + sTableName_DeviceConfig + " ";
                selectCmd += " Order By DEVICE_TYPE ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "取得人員清單，並以 DataTable 方式回傳"
        public DataTable GetPeopleList()
        {
            try
            {
                string selectCmd = "";
                selectCmd += "Select UserId,UserName ";
                selectCmd += "  From " + sTableName_tblSysUser + " ";
                selectCmd += "  Order By USERID ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "取得欄位清單，並以 DataTable 方式回傳"
        public DataTable GetFieldList()
        {
            try
            {
                string selectCmd = "";
                selectCmd += "Select * ";
                selectCmd += "  From " + sTableName_SysColumns + " ";
                selectCmd += " Where UPPER(TBCreator) = '" + "db2inst1"  + "' ";
                selectCmd += "   And UPPER(TBName) = '" + sTableName_VD5Min.ToString().ToUpper() + "' ";
                selectCmd += " Order By ColNo ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "取得ctrlRPT_OPR1_011操作紀錄報表資料並以 DataTable 方式回傳"
        public DataTable GetctrlRPT_OPR1_01(string sDevList, string sPeopleList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string DevNames = "('CMS-N1-L-250-E-2','CMS-N1-N-251.3','CMS-N1-N-247.6','CMS-N1-N-241.7','CMS-N1-L-240-E-2','CMS-N1-N-231.8','CMS-N1-L-230-E-2','CMS-N1-N-221.3','CMS-N1-L-219-E-3','CMS-N1-L-219-E-2','CMS-N1-N-219.0','CMS-N1-N-212','CMS-N1-L-211-E-2','CMS-N1-N-199.6','CMS-N1-L-198-N-3','CMS-N1-N-190.6','CMS-N1-L-188-N-2','CMS-N1-L-188-N-3','CMS-N1-N-183.2','CMS-N1-L-181-N-2','CMS-N1-N-179.7','CMS-N1-L-178-N-2','CMS-N1-N-175.6','CMS-N1-L-174-N-2','CMS-N1-N-169.1','CMS-N1-L-168-N-2','CMS-N1-N-163.4','CMS-N1-N-161.6','CMS-N1-S-158-N-2','CMS-N1-S-158-N-1','CMS-N1-N-150.8','CMS-N1-N-133.7','CMS-N1-L-132-N-2','CMS-N1-N-118.3','CMS-N1-N-111.4','CMS-N1-L-110-N-3')";
                string selectCmd = "";
                selectCmd += "Select to_char(v.OperationTime,'YYYY-MM-DD HH24:MI:SS') AS OperationTime,v.OperationName,v.DeviceName,v.OperationDesc,COALESCE(l.USERNAME,'') AS USERNAME1 ";
                selectCmd += "       ,case when v.OperationResult=1 then '成功' else '失敗' end as OperationResult ";
                selectCmd += "  From " + DBNAME + "." + "TBLSYSOPERATION v LEFT JOIN " + DBNAME + ".tblSysUser" + " l  ON  v.USERID=l.USERID ";
                selectCmd += " Where v.devicename in " + DevNames + " ";
                selectCmd += " and v.OperationTime between '" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                selectCmd += "                       and '" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                //selectCmd += " AND l.USERNAME in " + GetSelectCmd(user) + " ";
                selectCmd += " Order By USERNAME,OperationTime,DeviceName ";

                //selectCmd += "Select to_char(v.OperationTime,'YYYY-MM-DD HH24:MI:SS') AS OperationTime,v.OperationName,v.DeviceName,v.OperationDesc,COALESCE(l.USERNAME,'') AS USERNAME1 ";
                //selectCmd += "       ,case when v.OperationResult=1 then '成功' else '失敗' end as OperationResult ";
                //selectCmd += "  From " + "db2inst1"  + "." + sTableName_TBLSYSOPERATION + " v LEFT JOIN " + "db2inst1"  + "." + sTableName_tblSysUser + " l  ON  v.USERID=l.USERID ";
                //selectCmd += " Where v.OperationName in " + sDevList + " ";
                //selectCmd += " AND v.OperationTime between '" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                //selectCmd += "                       and '" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                //if (sPeopleList != "")
                //{
                //    selectCmd += " AND l.USERNAME in " + sPeopleList + " ";
                //}
                //selectCmd += " Order By OperationTime,DeviceName ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }

        #endregion

        #region "取得CtrlRPT_HDA_11匝道平均每日交通量統計報表資料並以 DataTable 方式回傳"
        public DataTable GetCtrlRPT_HDA_11(string sList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";


                selectCmd += "Select v.Devicename, ";
                selectCmd += "       date(v.Timestamp) as date, ";
                selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane1)) +  ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane2)) +  ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane3)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane4)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane5)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane6)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane7)) ";
                selectCmd += "       ) as connect_car_volume, ";
                selectCmd += "       cast( ";
                selectCmd += "       " + "db2inst1"  + "." + sFunction_DIV + "( ";
                selectCmd += "                    (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane1)) +  ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane2)) +  ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane3)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane4)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane5)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane6)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane7)) ";
                selectCmd += "                    ), ";
                selectCmd += "                    SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                selectCmd += "                    2 ";
                selectCmd += "                   )*100 as decimal(8,2)) as connect_car_volume_rate, ";
                selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane1)) +  ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane2)) +  ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane3)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane4)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane5)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane6)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane7)) ";
                selectCmd += "       ) as big_car_volume, ";
                selectCmd += "       cast( ";
                selectCmd += "       " + "db2inst1"  + "." + sFunction_DIV + "( ";
                selectCmd += "                    (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane1)) +  ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane2)) +  ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane3)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane4)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane5)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane6)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane7)) ";
                selectCmd += "                    ), ";
                selectCmd += "                    SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                selectCmd += "                    2 ";
                selectCmd += "                    )*100 as decimal(8,2)) as big_car_volume_rate, ";
                selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane1)) +  ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane2)) +  ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane3)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane4)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane5)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane6)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane7)) ";
                selectCmd += "       ) as small_car_volume, ";
                selectCmd += "       cast( ";
                selectCmd += "       " + "db2inst1"  + "." + sFunction_DIV + "( ";
                selectCmd += "                    (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane1)) +  ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane2)) +  ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane3)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane4)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane5)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane6)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane7)) ";
                selectCmd += "                    ), ";
                selectCmd += "                    SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                selectCmd += "                    2 ";
                selectCmd += "                    )*100 as decimal(8,2)) as small_car_volume_rate, ";
                selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume))) as car_volume ";
                selectCmd += " From (Select * From " + "db2inst1"  + "." + sTableName_VD5Min + " v Where v.Timestamp Between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) v ";
                selectCmd += "Where v.Devicename in " + sList + " ";
                selectCmd += "Group By v.Devicename, date(v.Timestamp) ";
                selectCmd += "Order By v.Devicename, date(v.Timestamp) ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }

        #endregion

        #region "取得CtrlRPT_HDA_11匝道平均每日交通量統計報表資料並以 DataTable 方式回傳"
        public DataTable GetCtrlRPT_HDA_11vd(string sList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";


                selectCmd += "Select v.Devicename";
                selectCmd += " From (Select * From " + "db2inst1"  + "." + sTableName_VD5Min + " v Where v.Timestamp Between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) v ";
                selectCmd += "Where v.Devicename in " + sList + " ";
                selectCmd += "Group By v.Devicename ";
                selectCmd += "Order By v.Devicename ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }

        #endregion


        #region "取得報表資料並以 DataTable 方式回傳"
        public DataTable GetReport(string sList, DateTime sStartDate, DateTime sEndDate, string sMyColumns, string sReportKind)
        {
            try
            {
                string selectCmd = "";

                if (sReportKind.ToString().Trim().ToUpper() == "一分鐘交通資料")
                {
                    selectCmd += "Select " + sMyColumns + " ";
                    //2009/12/18:SHIN:edit...改抓五分鐘table
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD1Min + " ";
                    //selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD5Min + " ";
                    selectCmd += " Where DeviceName in " + sList + " ";
                    selectCmd += " and VD5MIN_FLAG=1 and datavalidity = 'V' ";
                    selectCmd += "   And Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += "                     and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += " Order By DeviceName, TimeStamp ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "一分鐘交通資料VD")
                {
                    selectCmd += "Select DeviceName ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD1Min + " ";
                    selectCmd += " Where DeviceName in " + sList + " ";
                    selectCmd += " and VD5MIN_FLAG=1 and datavalidity = 'V' ";
                    selectCmd += "   And Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += "                     and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += " group by DeviceName ";
                    selectCmd += " Order By DeviceName ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "五分鐘交通資料")
                {
                    selectCmd += "Select " + sMyColumns + " ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD5Min + " ";
                    selectCmd += " Where DeviceName in " + sList + " ";
                    selectCmd += "   And Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += "                     and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += " Order By DeviceName, TimeStamp ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "五分鐘交通資料VD")
                {
                    selectCmd += "Select DeviceName ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD5Min + " ";
                    selectCmd += " Where DeviceName in " + sList + " ";
                    selectCmd += "   And Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += "                     and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += " group by DeviceName ";
                    selectCmd += " Order By DeviceName ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "現點速率VD")
                {

                    selectCmd += "select a.DEVICENAME";
                    selectCmd += " From tblVdDataSpotSpeed a  ";
                    selectCmd += " left join tblSysParameter b on  char(a.car_class) = char(b.VariableName) and GroupName = 'car_class'  ";
                    selectCmd += " Where a.DeviceName in" + sList + " ";
                    selectCmd += " And a.Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')   ";
                    selectCmd += " and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')  ";
                    selectCmd += " group by a.DeviceName  ";




                }
                else if (sReportKind.ToString().Trim().ToUpper() == "現點速率記錄")
                {

                    selectCmd += "select a.DEVICENAME,to_char(a.TimeStamp,'YYYY-MM-DD HH24:MI:SS') as TimeStamp,a.lane_id, ";
                    selectCmd += DBNAME + ".ZERO2DASH(a.car_volume)," + DBNAME + ".ZERO2DASH(a.car_speed) ";
                    selectCmd += " ,b.VariableValue," + DBNAME + ".ZERO2DASH(a.car_length) ";
                    selectCmd += " ," + DBNAME + ".ZERO2DASH(a.car_interval) ";
                    selectCmd += " From tblVdDataSpotSpeed a  ";
                    selectCmd += " left join tblSysParameter b on  char(a.car_class) = char(b.VariableName) and GroupName = 'car_class'  ";
                    selectCmd += " Where a.DeviceName in" + sList + " ";
                    selectCmd += " And a.Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')   ";
                    selectCmd += " and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')  ";
                    selectCmd += " Order By a.DeviceName, a.TimeStamp  ";




                }
                else if (sReportKind.ToString().Trim().ToUpper() == "五分鐘車間距資料")
                {
                    selectCmd += "Select " + sMyColumns + " ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD5Min + " ";
                    selectCmd += " Where DeviceName in " + sList + " ";
                    selectCmd += "   And Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += "                     and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += " Order By DeviceName, TimeStamp ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "五分鐘車間距資料VD")
                {
                    selectCmd += "Select DEVICENAME ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD5Min + " ";
                    selectCmd += " Where DeviceName in " + sList + " ";
                    selectCmd += "   And Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += "                     and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += " group by DeviceName  ";
                    selectCmd += " Order By DeviceName ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "一分鐘車道使用率及車間距報表")
                {
                    selectCmd += "Select " + sMyColumns + " ";
                    //2009/12/18:SHIN:edit...改抓五分鐘table
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD1Min + " ";
                    //selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD5Min + " ";
                    selectCmd += " Where DeviceName in " + sList + " ";
                    selectCmd += "   And Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += "                     and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += " Order By DeviceName, TimeStamp ";

                }
                else if (sReportKind.ToString().Trim().ToUpper() == "一分鐘車道使用率及車間距報表VD")
                {
                    selectCmd += "Select DEVICENAME ";
                    //2009/12/18:SHIN:edit...改抓五分鐘table
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD1Min + " ";
                    //selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD5Min + " ";
                    selectCmd += " Where DeviceName in " + sList + " ";
                    selectCmd += "   And Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += "                     and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += " group by DeviceName  ";
                    selectCmd += " Order By DeviceName ";

                }
                else if (sReportKind.ToString().Trim().ToUpper() == "一小時交通資料")
                {
                    selectCmd += "Select " + sMyColumns + " ";
                    //2009/12/18:SHIN:edit...改抓五分鐘table
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD1Hr + " ";
                    //selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD5Min + " ";
                    selectCmd += " Where DeviceName in " + sList + " ";
                    selectCmd += "   And Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += "                     and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += " Order By DeviceName, TimeStamp ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "一小時交通資料VD")
                {
                    selectCmd += "Select DEVICENAME ";
                    //2009/12/18:SHIN:edit...改抓五分鐘table
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD1Hr + " ";
                    //selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD5Min + " ";
                    selectCmd += " Where DeviceName in " + sList + " ";
                    selectCmd += "   And Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += "                     and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += " group by DeviceName  ";
                    selectCmd += " Order By DeviceName ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "一天交通資料")
                {
                    selectCmd += "Select " + sMyColumns + " ";
                    //2009/12/18:SHIN:edit...改抓五分鐘table
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD1Day + " ";
                    //selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD5Min + " ";
                    selectCmd += " Where DeviceName in " + sList + " ";
                    selectCmd += "   And Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += "                     and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += " Order By DeviceName, TimeStamp ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "一天交通資料VD")
                {
                    selectCmd += "Select DEVICENAME ";
                    //2009/12/18:SHIN:edit...改抓五分鐘table
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD1Day + " ";
                    //selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD5Min + " ";
                    selectCmd += " Where DeviceName in " + sList + " ";
                    selectCmd += "   And Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += "                     and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += " group by DeviceName  ";
                    selectCmd += " Order By DeviceName ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "全區主線全日交通量統計")
                {
                    selectCmd += "Select l.LineName, ";
                    selectCmd += "       s.SectionName, ";
                    selectCmd += "       p.VariableValue, ";
                    selectCmd += "       (select " + "db2inst1"  + ".ZERO2DASH((case when MAX(v.car_volume) is null then 0 else MAX(v.car_volume) end)) from " + "db2inst1"  + "." + sTableName_VD1Day + " v, " + "db2inst1"  + "." + sTableName_DeviceConfig + " d where d.Devicename = v.Devicename and d.sectionid = s.sectionid and date(v.timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) ) as car_volume, ";
                    selectCmd += "       " + "db2inst1"  + "." + sFunction_MaxVol_Dev + "((select MAX(v.car_volume) from " + "db2inst1"  + "." + sTableName_VD1Day + " v, " + "db2inst1"  + "." + sTableName_DeviceConfig + " d where d.Devicename = v.Devicename and d.sectionid = s.sectionid and date(v.timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'))), timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'), s.sectionid) as DeviceList ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_GroupSection + " s LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupDivision + " d1 ON d1.DivisionId = s.Start_DivisionId ";
                    selectCmd += "	                                                                                    LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupDivision + " d2 ON d2.DivisionId = s.End_DivisionId ";
                    selectCmd += "                                                                                      LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + " l ON l.LineId = s.LineId ";
                    selectCmd += "                                                                                      LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'DeviceDirection') p ON p.VariableName = s.Direction ";
                    selectCmd += " Where s.SectionId in " + sList + " ";
                    selectCmd += " Order By l.LineId, s.Direction, d1.Mileage ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "全區匝道全日交通量統計")
                {
                    selectCmd += "Select r.LineName, ";
                    selectCmd += "       r.DivisionName, ";
                    selectCmd += "       r.DirectionDesc, ";
                    selectCmd += "       r.Volume, ";
                    selectCmd += "       r.Devicename";
                    selectCmd += "  From ( ";
                    selectCmd += "Select Distinct ";
                    selectCmd += "       r.LineName as LineName, ";
                    selectCmd += "       r.DivisionName as DivisionName, ";
                    selectCmd += "       r.DirectionDesc as DirectionDesc, ";
                    selectCmd += "       (select (case when MAX(v.car_volume) is null then '-' else char(MAX(v.car_volume)) end) ";
                    selectCmd += "          from " + "db2inst1"  + ".tblvddata1day v, ";
                    selectCmd += "               " + "db2inst1"  + ".tbldeviceconfig d ";
                    selectCmd += "         where d.devicename = v.devicename ";
                    selectCmd += "           and d.location = 'R' ";
                    selectCmd += "           and date(v.timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) ";
                    selectCmd += "           and d.SectionId = r.SectionId  ";
                    selectCmd += "           and UPPER(d.Location_R) = UPPER(r.Location_R) ";
                    selectCmd += "        ) as Volume, ";
                    selectCmd += "        " + "db2inst1"  + "." + sFunction_MaxVol_Dev_Ramp + "( ";
                    selectCmd += "        (select MAX(v.car_volume) from " + "db2inst1"  + ".tblvddata1day v, " + "db2inst1"  + ".tbldeviceconfig d ";
                    selectCmd += "          where d.devicename = v.devicename ";
                    selectCmd += "            and d.location = 'R' ";
                    selectCmd += "            and date(v.timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) ";
                    selectCmd += "            and d.SectionId = r.SectionId  ";
                    selectCmd += "            and UPPER(d.Location_R) = UPPER(r.Location_R) ";
                    selectCmd += "        ), timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'), r.SectionId, r.Location_R) as Devicename, ";
                    selectCmd += "        r.LineId, r.Mileage ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_vwRamp + " r ";
                    selectCmd += " Where r.SectionId in " + sList + " ";
                    selectCmd += " ) r ";
                    selectCmd += " Order By r.LineId, r.Mileage ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "現場終端設備狀態記錄統計")
                {
                    selectCmd += "Select l.Devicename as Devicename, ";
                    selectCmd += "       n.LineName as LineName, ";
                    selectCmd += "       (case when d.Location = 'R' and Location_R = 'I' then concat(p.VariableValue,'入口') ";
                    selectCmd += "             when d.Location = 'R' and Location_R = 'O' then concat(p.VariableValue,'出口') ";
                    selectCmd += "        else p.VariableValue end) as Location, ";
                    selectCmd += "       p2.VariableValue as Direction, ";
                    selectCmd += "       cast(round(cast(d.Mile_M as double)/1000,2) as decimal(8,2)) as Mileage, ";
                    selectCmd += "       Replace(l.Memo,':','/') as memo, ";
                    selectCmd += "       l.Timestamp as StartTimestamp, ";
                    selectCmd += "       '' as EndTimestamp ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_DeviceStatusLog + " l LEFT JOIN " + "db2inst1"  + "." + sTableName_DeviceConfig + " d LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + " n ON n.lineid = d.lineid ";
                    selectCmd += "                                                                                         LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'Location') p ON p.VariableName = d.Location ";
                    selectCmd += "                                                                                         LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'DeviceDirection') p2 ON p2.VariableName = d.Direction ";
                    selectCmd += "       ON d.Devicename = l.Devicename ";
                    selectCmd += " Where d.Device_Type in " + sList + " ";
                    selectCmd += "   And l.Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += "   And (l.HW_Status_1 <> 0 OR ";
                    selectCmd += "        l.HW_Status_2 <> 0 OR ";
                    selectCmd += "        l.HW_Status_3 <> 0 OR ";
                    selectCmd += "        l.HW_Status_4 <> 0) ";
                    selectCmd += " Order By l.Timestamp, l.Devicename ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "現場終端設備運作記錄")
                {
                    //selectCmd += "Select l.Devicename as Devicename, ";
                    //selectCmd += "       n.LineName as LineName, ";
                    //selectCmd += "       (case when d.Location = 'R' and Location_R = 'I' then concat(p.VariableValue,'入口') ";
                    //selectCmd += "             when d.Location = 'R' and Location_R = 'O' then concat(p.VariableValue,'出口') ";
                    //selectCmd += "        else p.VariableValue end ";
                    //selectCmd += "       ) as Location, ";
                    //selectCmd += "       p2.VariableValue as Direction, ";
                    //selectCmd += "       cast(round(cast(d.Mile_M as double)/1000,2) as decimal(8,2)) as Mileage, ";
                    //selectCmd += "       l.Timestamp, ";
                    //selectCmd += "       (case when l.connectstatus = 'Y' then '連線' else '離線' end) as connectstatus, ";
                    //selectCmd += "       (case when l.connectstatus <> 'Y' then '-' when l.connectstatus is null then '-' else " + "db2inst1"  + ".OP_STATUS(case when l.op_status is null then 0 else l.op_status end) end) as op_status, ";
                    //selectCmd += "       (case when l.connectstatus <> 'Y' then '-' when l.connectstatus is null then '-' else (case when l.display is null then '-' when l.display = '' then '-' else l.display end) end) as display ";
                    //selectCmd += "  From " + "db2inst1"  + "." + sTableName_DeviceStatusLog + " l LEFT JOIN " + "db2inst1"  + "." + sTableName_DeviceConfig + " d LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + " n ON n.lineid = d.lineid ";
                    //selectCmd += "                                                                                         LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'Location') p ON p.VariableName = d.Location ";
                    //selectCmd += "                                                                                         LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'DeviceDirection') p2 ON p2.VariableName = d.Direction ";
                    //selectCmd += "       ON d.Devicename = l.Devicename ";
                    //selectCmd += " Where d.Device_Type in " + sList + " ";
                    //selectCmd += "   And l.Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    //selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    //selectCmd += " Order By l.DeviceName, l.timestamp ";





                    selectCmd += "Select s.devicename,l.linename, loc.variablevalue,dir.variablevalue, ";
                    selectCmd += "       decimal(round(float(d.mile_m)/1000,2),8,2) as mileage,to_char(s.timestamp,'YYYY-MM-DD HH24:MI:SS'), ";
                    selectCmd += "       (case when s.type = 'C' then (case when s.result = 3 then '斷線' else '連線' end) else '' end) as comm_state, ";
                    selectCmd += "             (case when s.type = 'S' then DB2INST1.OP_STATUS((case when s.result is null then 0 else s.result end)) else '-' end) as op_status, ";
                    selectCmd += "        (case when s.type = 'D' then display else '-' end) as display ";
                    selectCmd += "  From db2inst1.tbldevicestatelog s";
                    selectCmd += " LEFt JOIN db2inst1.tbldeviceconfig d ON d.devicename = s.devicename ";
                    selectCmd += " LEFT JOIN db2inst1.tblgroupline l ON l.lineid = d.lineid ";
                    selectCmd += " LEFT JOIN (select variablename, variablevalue from db2inst1.tblsysparameter where UPPER(groupname) = 'LOCATION') loc ON loc.variablename = d.location  ";
                    selectCmd += " LEFT JOIN (select variablename, variablevalue from db2inst1.tblsysparameter where UPPER(groupname) = 'DEVICEDIRECTION') dir ON dir.variablename = d.direction ";
                    selectCmd += " Where s.type in ('C','S','D') and d.Device_Type in " + sList + " ";
                    selectCmd += "   And s.Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                    selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "設備狀態即時監視")
                {
                    //selectCmd += "Select d.device_type, ";
                    //selectCmd += "       d.Devicename, s.LINENAME, ";
                    //selectCmd += "       (case when l.connectstatus = 'Y' then '連線' when l.connectstatus = 'N' then '離線' else '-' end), ";
                    //selectCmd += "       (case when l.connectstatus <> 'Y' then '-' when l.connectstatus is null then '-' else " + "db2inst1"  + ".OP_MODE(case when d.op_mode is null then 0 else d.op_mode end) end) as op_mode, ";
                    //selectCmd += "       (case when l.connectstatus <> 'Y' then '-' when l.connectstatus is null then '-' else (case when l.memo is null then '-' when l.memo = '' then '-' else l.memo end) end) ";
                    ////selectCmd += "       ,d.DEVICENAME,m.MFCCName  ";
                    //selectCmd += "  From " + "db2inst1"  + "." + sTableName_DeviceConfig + " d LEFT JOIN ";
                    //selectCmd += "       " + "db2inst1"  + "." + sTableName_DeviceStatus + " l ON d.DEVICENAME=l.DEVICENAME LEFT JOIN ";
                    //selectCmd += "       " + "db2inst1"  + "." + sTableName_tblgroupline + " s ON d.LINEID=s.LINEID  ";
                    ////selectCmd += "       " + "db2inst1"  + "." + sTableName_tblMFCCConfig + " m ON d.MFCCId=m.MFCCId ";
                    ////selectCmd += "       " + "db2inst1"  + "." + sTableName_DeviceStatusLog + " l, ";
                    ////selectCmd += "       (select x.devicename, x.timestamp ";
                    ////selectCmd += "          from " + "db2inst1"  + "." + sTableName_DeviceStatusLog + " x ";
                    ////selectCmd += "         where x.timestamp = (select max(timestamp) from " + "db2inst1"  + "." + sTableName_DeviceStatusLog + " where devicename = x.devicename) ";
                    ////selectCmd += "         group by x.devicename, x.timestamp) xl ";
                    ////selectCmd += " Where l.devicename = d.devicename and d.LINEID=s.LINEID ";
                    ////selectCmd += "   And l.devicename = xl.devicename ";
                    ////selectCmd += "   And l.timestamp = xl.timestamp ";
                    //selectCmd += "   Where d.Device_Type in " + sList + " ";
                    //selectCmd += " Order By d.device_type, d.DeviceName,s.LINENAME ";

                    //SHIN add
                    selectCmd += "Select d.device_type, ";
                    selectCmd += "       d.Devicename, s.LINENAME, ";
                    selectCmd += "       (case when d.comm_state = 3 then '斷線' when d.enable = 'N' then '斷線'   else '連線' end), ";
                    selectCmd += "       (case when d.comm_state = 3 then '-' when d.enable = 'N' then '斷線' when d.comm_state is null then '-' else db2inst1.OP_MODE(case when d.op_mode is null then 0 else d.op_mode end) end) as op_mode, ";
                    selectCmd += "       (case when d.comm_state = 3 then '-' when d.enable = 'N' then '斷線' when d.comm_state is null  then '-' else (case when d.hw_status_1=0 and d.hw_status_2 =0 and d.hw_status_3=0 and d.hw_status_4=0  then '正常'  else db2inst1.DEVICESTATUS(d.hw_status_1,d.hw_status_2,d.hw_status_3,d.hw_status_4,d.device_type)  end) end)    ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_DeviceConfig + " d LEFT JOIN ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_tblgroupline + " s ON d.LINEID=s.LINEID  ";
                    selectCmd += "   Where d.Device_Type in " + sList + " ";
                    selectCmd += " Order By d.device_type, d.DeviceName,s.LINENAME ";

                }
                else if (sReportKind.ToString().Trim().ToUpper() == "設備狀態即時監視2")
                {
                    selectCmd += "Select d.DEVICENAME,d.MFCCId   ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_DeviceConfig + " d left join  ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_DeviceStatus + " l ON d.DEVICENAME=l.DEVICENAME left join";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_tblgroupline + " s  ON d.LINEID=s.LINEID";

                    selectCmd += "   Where d.Device_Type in " + sList + " ";
                    selectCmd += " Order By d.device_type, d.DeviceName,s.LINENAME ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "全區主線小時路段平均速度統計")
                {
                    selectCmd += "Select l.LineName, s.SectionName, p.VariableValue, ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(INT(case when ";
                    selectCmd += "                  (select " + "db2inst1"  + ".DIV(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_speed) * " + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), 2) ";
                    selectCmd += "                     from " + "db2inst1"  + "." + sTableName_VD5Min + " v left join " + "db2inst1"  + "." + sTableName_DeviceConfig + " d on d.devicename = v.devicename ";
                    selectCmd += "                    where d.sectionid = s.sectionid ";
                    selectCmd += "                      and date(v.timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) ";
                    selectCmd += "                    group by d.sectionid ";
                    selectCmd += "                  ) is null then 0 ";
                    selectCmd += "        else                       ";
                    selectCmd += "                  (select cast(" + "db2inst1"  + ".DIV(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_speed) * " + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), 2) as decimal(8,2)) ";
                    selectCmd += "                     from " + "db2inst1"  + "." + sTableName_VD5Min + " v left join " + "db2inst1"  + "." + sTableName_DeviceConfig + " d on d.devicename = v.devicename ";
                    selectCmd += "                    where d.sectionid = s.sectionid ";
                    selectCmd += "                      and date(v.timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) ";
                    selectCmd += "                    group by d.sectionid )  ";
                    selectCmd += "        end)) as car_speed ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_GroupSection + " s LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupDivision + " d1 ON d1.DivisionId = s.Start_DivisionId ";
                    selectCmd += "                                                                                      LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupDivision + " d2 ON d2.DivisionId = s.End_DivisionId ";
                    selectCmd += "                                                                                      LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + " l ON l.LineId = s.LineId ";
                    selectCmd += "                                                                                      LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'DeviceDirection') p ON p.VariableName = s.Direction ";
                    selectCmd += " Where s.SectionId in " + sList + " ";
                    selectCmd += " Order By l.LineId, s.Direction, d1.Mileage ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "主線平均每日交通量統計")
                {
                    selectCmd += "Select x.Devicename, ";
                    selectCmd += "       l.LineName, ";
                    selectCmd += "       p.VariableValue as Direction, ";
                    selectCmd += "       cast(cast(d.Mile_M as double)/1000 as decimal(8,2)) as Mileage, ";
                    selectCmd += "       x.date, ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(x.connect_car_volume), ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(x.connect_car_volume_rate), ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(x.big_car_volume), ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(x.big_car_volume_rate), ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(x.small_car_volume), ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(x.small_car_volume_rate), ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(x.car_volume) ";
                    selectCmd += "  From ";
                    selectCmd += "( ";
                    selectCmd += "       Select v.Devicename, ";
                    selectCmd += "              date(v.Timestamp) as date, ";
                    selectCmd += "              (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane1)) +  ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane2)) +  ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane3)) + ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane4)) + ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane5)) + ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane6)) + ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane7)) ";
                    selectCmd += "              ) as connect_car_volume, ";
                    selectCmd += "              int(" + "db2inst1"  + ".DIV( ";
                    selectCmd += "                            (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane1)) +  ";
                    selectCmd += "                             SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane2)) +  ";
                    selectCmd += "                             SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane3)) + ";
                    selectCmd += "                             SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane4)) + ";
                    selectCmd += "                             SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane5)) + ";
                    selectCmd += "                             SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane6)) + ";
                    selectCmd += "                             SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane7)) ";
                    selectCmd += "                             ), ";
                    selectCmd += "                           SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                    selectCmd += "                           2 ";
                    selectCmd += "                          )*100) as connect_car_volume_rate, ";
                    selectCmd += "              (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane1)) +  ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane2)) +  ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane3)) + ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane4)) + ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane5)) + ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane6)) + ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane7)) ";
                    selectCmd += "              ) as big_car_volume, ";
                    selectCmd += "              int(" + "db2inst1"  + ".DIV( ";
                    selectCmd += "                           (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane1)) +  ";
                    selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane2)) +  ";
                    selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane3)) + ";
                    selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane4)) + ";
                    selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane5)) + ";
                    selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane6)) + ";
                    selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane7)) ";
                    selectCmd += "                            ), ";
                    selectCmd += "                           SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                    selectCmd += "                           2 ";
                    selectCmd += "                          )*100) as big_car_volume_rate, ";
                    selectCmd += "              (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane1)) +  ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane2)) +  ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane3)) + ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane4)) + ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane5)) + ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane6)) + ";
                    selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane7)) ";
                    selectCmd += "               ) as small_car_volume, ";
                    selectCmd += "              int(" + "db2inst1"  + ".DIV( ";
                    selectCmd += "                           (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane1)) +  ";
                    selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane2)) +  ";
                    selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane3)) + ";
                    selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane4)) + ";
                    selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane5)) + ";
                    selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane6)) + ";
                    selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane7)) ";
                    selectCmd += "                            ), ";
                    selectCmd += "                           SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                    selectCmd += "                           2 ";
                    selectCmd += "                          )*100) as small_car_volume_rate, ";
                    selectCmd += "              SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)) as car_volume ";
                    selectCmd += "         From " + "db2inst1"  + "." + sTableName_VD5Min + " v ";
                    selectCmd += "        Where date(v.Timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) ";
                    selectCmd += "        Group By v.Devicename, date(v.Timestamp) ";
                    selectCmd += ") x LEFT JOIN " + "db2inst1"  + "." + sTableName_DeviceConfig + " d LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + " l ON l.LineId = d.LineId  ";
                    selectCmd += "                                                                                             LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'DeviceDirection') p ON p.VariableName = d.Direction ";
                    selectCmd += "    ON d.Devicename = x.Devicename  ";
                    selectCmd += " Where x.DeviceName in " + sList + " ";
                    selectCmd += " Order By d.LineId, d.Direction, x.DeviceName ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "匝道平均每日交通量統計")
                {
                    selectCmd += "Select v.Devicename, ";
                    selectCmd += "       date(v.Timestamp) as date, ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane1)) +  ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane2)) +  ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane3)) + ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane4)) + ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane5)) + ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane6)) + ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane7)) ";
                    selectCmd += "       ) as connect_car_volume, ";
                    selectCmd += "       cast( ";
                    selectCmd += "       " + "db2inst1"  + "." + sFunction_DIV + "( ";
                    selectCmd += "                    (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane1)) +  ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane2)) +  ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane3)) + ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane4)) + ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane5)) + ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane6)) + ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane7)) ";
                    selectCmd += "                    ), ";
                    selectCmd += "                    SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                    selectCmd += "                    2 ";
                    selectCmd += "                   )*100 as decimal(8,2)) as connect_car_volume_rate, ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane1)) +  ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane2)) +  ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane3)) + ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane4)) + ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane5)) + ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane6)) + ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane7)) ";
                    selectCmd += "       ) as big_car_volume, ";
                    selectCmd += "       cast( ";
                    selectCmd += "       " + "db2inst1"  + "." + sFunction_DIV + "( ";
                    selectCmd += "                    (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane1)) +  ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane2)) +  ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane3)) + ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane4)) + ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane5)) + ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane6)) + ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane7)) ";
                    selectCmd += "                    ), ";
                    selectCmd += "                    SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                    selectCmd += "                    2 ";
                    selectCmd += "                    )*100 as decimal(8,2)) as big_car_volume_rate, ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane1)) +  ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane2)) +  ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane3)) + ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane4)) + ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane5)) + ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane6)) + ";
                    selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane7)) ";
                    selectCmd += "       ) as small_car_volume, ";
                    selectCmd += "       cast( ";
                    selectCmd += "       " + "db2inst1"  + "." + sFunction_DIV + "( ";
                    selectCmd += "                    (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane1)) +  ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane2)) +  ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane3)) + ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane4)) + ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane5)) + ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane6)) + ";
                    selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane7)) ";
                    selectCmd += "                    ), ";
                    selectCmd += "                    SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                    selectCmd += "                    2 ";
                    selectCmd += "                    )*100 as decimal(8,2)) as small_car_volume_rate, ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume))) as car_volume ";
                    selectCmd += " From (Select * From " + "db2inst1"  + "." + sTableName_VD5Min + " v Where v.Timestamp Between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) v ";
                    selectCmd += "Where v.Devicename in " + sList + " ";
                    selectCmd += "Group By v.Devicename, date(v.Timestamp) ";
                    selectCmd += "Order By v.Devicename, date(v.Timestamp) ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "小時交通流量統計")
                {
                    selectCmd += "Select v.timestamp as timestamp, ";
                    selectCmd += "       v.devicename as devicename, ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH((case when v.connect_car_volume_lane1 < 0 then 0 else v.connect_car_volume_lane1 end) +  ";
                    selectCmd += "       (case when v.big_car_volume_lane1 < 0 then 0 else v.big_car_volume_lane1 end) +  ";
                    selectCmd += "       (case when v.small_car_volume_lane1 < 0 then 0 else v.small_car_volume_lane1 end) + ";
                    selectCmd += "       (case when v.connect_car_volume_lane2 < 0 then 0 else v.connect_car_volume_lane2 end) +  ";
                    selectCmd += "       (case when v.big_car_volume_lane2 < 0 then 0 else v.big_car_volume_lane2 end) +  ";
                    selectCmd += "       (case when v.small_car_volume_lane2 < 0 then 0 else v.small_car_volume_lane2 end) + ";
                    selectCmd += "       (case when v.connect_car_volume_lane3 < 0 then 0 else v.connect_car_volume_lane3 end) +  ";
                    selectCmd += "       (case when v.big_car_volume_lane3 < 0 then 0 else v.big_car_volume_lane3 end) +  ";
                    selectCmd += "       (case when v.small_car_volume_lane3 < 0 then 0 else v.small_car_volume_lane3 end) + ";
                    selectCmd += "       (case when v.connect_car_volume_lane4 < 0 then 0 else v.connect_car_volume_lane4 end) +  ";
                    selectCmd += "       (case when v.big_car_volume_lane4 < 0 then 0 else v.big_car_volume_lane4 end) +  ";
                    selectCmd += "       (case when v.small_car_volume_lane4 < 0 then 0 else v.small_car_volume_lane4 end) + ";
                    selectCmd += "       (case when v.connect_car_volume_lane5 < 0 then 0 else v.connect_car_volume_lane5 end) +  ";
                    selectCmd += "       (case when v.big_car_volume_lane5 < 0 then 0 else v.big_car_volume_lane5 end) +  ";
                    selectCmd += "       (case when v.small_car_volume_lane5 < 0 then 0 else v.small_car_volume_lane5 end) + ";
                    selectCmd += "       (case when v.connect_car_volume_lane6 < 0 then 0 else v.connect_car_volume_lane6 end) +  ";
                    selectCmd += "       (case when v.big_car_volume_lane6 < 0 then 0 else v.big_car_volume_lane6 end) +  ";
                    selectCmd += "       (case when v.small_car_volume_lane6 < 0 then 0 else v.small_car_volume_lane6 end)) ";
                    selectCmd += "       as total, ";
                    selectCmd += " ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH((case when v.connect_car_volume_lane1 < 0 then 0 else v.connect_car_volume_lane1 end) +  ";
                    selectCmd += "       (case when v.connect_car_volume_lane2 < 0 then 0 else v.connect_car_volume_lane2 end) +  ";
                    selectCmd += "       (case when v.connect_car_volume_lane3 < 0 then 0 else v.connect_car_volume_lane3 end) +  ";
                    selectCmd += "       (case when v.connect_car_volume_lane4 < 0 then 0 else v.connect_car_volume_lane4 end) +  ";
                    selectCmd += "       (case when v.connect_car_volume_lane5 < 0 then 0 else v.connect_car_volume_lane5 end) +  ";
                    selectCmd += "       (case when v.connect_car_volume_lane6 < 0 then 0 else v.connect_car_volume_lane6 end)) as total_connect, ";
                    selectCmd += " ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH((case when v.big_car_volume_lane1 < 0 then 0 else v.big_car_volume_lane1 end) +  ";
                    selectCmd += "       (case when v.big_car_volume_lane2 < 0 then 0 else v.big_car_volume_lane2 end) +  ";
                    selectCmd += "       (case when v.big_car_volume_lane3 < 0 then 0 else v.big_car_volume_lane3 end) +  ";
                    selectCmd += "       (case when v.big_car_volume_lane4 < 0 then 0 else v.big_car_volume_lane4 end) +  ";
                    selectCmd += "       (case when v.big_car_volume_lane5 < 0 then 0 else v.big_car_volume_lane5 end) +  ";
                    selectCmd += "       (case when v.big_car_volume_lane6 < 0 then 0 else v.big_car_volume_lane6 end)) as total_big, ";
                    selectCmd += " ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH((case when v.small_car_volume_lane1 < 0 then 0 else v.small_car_volume_lane1 end) +  ";
                    selectCmd += "       (case when v.small_car_volume_lane2 < 0 then 0 else v.small_car_volume_lane2 end) +  ";
                    selectCmd += "       (case when v.small_car_volume_lane3 < 0 then 0 else v.small_car_volume_lane3 end) +  ";
                    selectCmd += "       (case when v.small_car_volume_lane4 < 0 then 0 else v.small_car_volume_lane4 end) +  ";
                    selectCmd += "       (case when v.small_car_volume_lane5 < 0 then 0 else v.small_car_volume_lane5 end) +  ";
                    selectCmd += "       (case when v.small_car_volume_lane6 < 0 then 0 else v.small_car_volume_lane6 end)) as total_small, ";
                    selectCmd += " ";//車道1
                    selectCmd += "       (case when c.lane_count >= 1 then char((case when v.connect_car_volume_lane1 < 0 then 0 else v.connect_car_volume_lane1 end) +  ";
                    selectCmd += "                                         (case when v.big_car_volume_lane1 < 0 then 0 else v.big_car_volume_lane1 end) +  ";
                    selectCmd += "                                         (case when v.small_car_volume_lane1 < 0 then 0 else v.small_car_volume_lane1 end)) ";
                    selectCmd += "        else '-' end) as lane1, ";
                    selectCmd += "       (case when c.lane_count >= 1 then char(case when v.connect_car_volume_lane1 < 0 then 0 else v.connect_car_volume_lane1 end) else '-' end) as lane1_connect, ";
                    selectCmd += "       (case when c.lane_count >= 1 then char(case when v.big_car_volume_lane1 < 0 then 0 else v.big_car_volume_lane1 end) else '-' end) as lane1_big, ";
                    selectCmd += "       (case when c.lane_count >= 1 then char(case when v.small_car_volume_lane1 < 0 then 0 else v.small_car_volume_lane1 end) else '-' end) as lane1_small, ";
                    selectCmd += " ";//車道2
                    selectCmd += "       (case when c.lane_count >= 2 then char((case when v.connect_car_volume_lane2 < 0 then 0 else v.connect_car_volume_lane2 end) +  ";
                    selectCmd += "                                         (case when v.big_car_volume_lane2 < 0 then 0 else v.big_car_volume_lane2 end) +  ";
                    selectCmd += "                                         (case when v.small_car_volume_lane2 < 0 then 0 else v.small_car_volume_lane2 end)) ";
                    selectCmd += "        else '-' end) as lane2, ";
                    selectCmd += "       (case when c.lane_count >= 2 then char(case when v.connect_car_volume_lane2 < 0 then 0 else v.connect_car_volume_lane2 end) else '-' end) as lane2_connect, ";
                    selectCmd += "       (case when c.lane_count >= 2 then char(case when v.big_car_volume_lane2 < 0 then 0 else v.big_car_volume_lane2 end) else '-' end) as lane2_big, ";
                    selectCmd += "       (case when c.lane_count >= 2 then char(case when v.small_car_volume_lane2 < 0 then 0 else v.small_car_volume_lane2 end) else '-' end) as lane2_small, ";
                    selectCmd += " ";//車道3
                    selectCmd += "       (case when c.lane_count >= 3 then char((case when v.connect_car_volume_lane3 < 0 then 0 else v.connect_car_volume_lane3 end) +  ";
                    selectCmd += "                                         (case when v.big_car_volume_lane3 < 0 then 0 else v.big_car_volume_lane3 end) +  ";
                    selectCmd += "                                         (case when v.small_car_volume_lane3 < 0 then 0 else v.small_car_volume_lane3 end)) ";
                    selectCmd += "        else '-' end) as lane3, ";
                    selectCmd += "       (case when c.lane_count >= 3 then char(case when v.connect_car_volume_lane3 < 0 then 0 else v.connect_car_volume_lane3 end) else '-' end) as lane3_connect, ";
                    selectCmd += "       (case when c.lane_count >= 3 then char(case when v.big_car_volume_lane3 < 0 then 0 else v.big_car_volume_lane3 end) else '-' end) as lane3_big, ";
                    selectCmd += "       (case when c.lane_count >= 3 then char(case when v.small_car_volume_lane3 < 0 then 0 else v.small_car_volume_lane3 end) else '-' end) as lane3_small, ";
                    selectCmd += " ";//車道4
                    selectCmd += "       (case when c.lane_count >= 4 then char((case when v.connect_car_volume_lane4 < 0 then 0 else v.connect_car_volume_lane4 end) +  ";
                    selectCmd += "                                         (case when v.big_car_volume_lane4 < 0 then 0 else v.big_car_volume_lane4 end) +  ";
                    selectCmd += "                                         (case when v.small_car_volume_lane4 < 0 then 0 else v.small_car_volume_lane4 end)) ";
                    selectCmd += "        else '-' end) as lane4, ";
                    selectCmd += "       (case when c.lane_count >= 4 then char(case when v.connect_car_volume_lane4 < 0 then 0 else v.connect_car_volume_lane4 end) else '-' end) as lane4_connect, ";
                    selectCmd += "       (case when c.lane_count >= 4 then char(case when v.big_car_volume_lane4 < 0 then 0 else v.big_car_volume_lane4 end) else '-' end) as lane4_big, ";
                    selectCmd += "       (case when c.lane_count >= 4 then char(case when v.small_car_volume_lane4 < 0 then 0 else v.small_car_volume_lane4 end) else '-' end) as lane4_small, ";
                    selectCmd += " ";//車道5
                    selectCmd += "       (case when c.lane_count >= 5 then char((case when v.connect_car_volume_lane5 < 0 then 0 else v.connect_car_volume_lane5 end) +  ";
                    selectCmd += "                                         (case when v.big_car_volume_lane5 < 0 then 0 else v.big_car_volume_lane5 end) +  ";
                    selectCmd += "                                         (case when v.small_car_volume_lane5 < 0 then 0 else v.small_car_volume_lane5 end)) ";
                    selectCmd += "        else '-' end) as lane5, ";
                    selectCmd += "       (case when c.lane_count >= 5 then char(case when v.connect_car_volume_lane5 < 0 then 0 else v.connect_car_volume_lane5 end) else '-' end) as lane5_connect, ";
                    selectCmd += "       (case when c.lane_count >= 5 then char(case when v.big_car_volume_lane5 < 0 then 0 else v.big_car_volume_lane5 end) else '-' end) as lane5_big, ";
                    selectCmd += "       (case when c.lane_count >= 5 then char(case when v.small_car_volume_lane5 < 0 then 0 else v.small_car_volume_lane5 end) else '-' end) as lane5_small, ";
                    selectCmd += " ";//車道6
                    selectCmd += "       (case when c.lane_count >= 6 then char((case when v.connect_car_volume_lane6 < 0 then 0 else v.connect_car_volume_lane6 end) +  ";
                    selectCmd += "                                         (case when v.big_car_volume_lane6 < 0 then 0 else v.big_car_volume_lane6 end) +  ";
                    selectCmd += "                                         (case when v.small_car_volume_lane6 < 0 then 0 else v.small_car_volume_lane6 end)) ";
                    selectCmd += "        else '-' end) as lane6, ";
                    selectCmd += "       (case when c.lane_count >= 6 then char(case when v.connect_car_volume_lane6 < 0 then 0 else v.connect_car_volume_lane6 end) else '-' end) as lane6_connect, ";
                    selectCmd += "       (case when c.lane_count >= 6 then char(case when v.big_car_volume_lane6 < 0 then 0 else v.big_car_volume_lane6 end) else '-' end) as lane6_big, ";
                    selectCmd += "       (case when c.lane_count >= 6 then char(case when v.small_car_volume_lane6 < 0 then 0 else v.small_car_volume_lane6 end) else '-' end) as lane6_small ";
                    selectCmd += " ";
                    //selectCmd += " From (Select * From " + "db2inst1"  + "." + sTableName_VD1Hr + " v Where v.Timestamp Between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) v, ";
                    selectCmd += " From (Select * From " + "db2inst1"  + "." + sTableName_VD5Min + " v Where v.Timestamp Between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) v, ";

                    selectCmd += "      " + "db2inst1"  + "." + sTableName_VdConfig + " c ";
                    selectCmd += "Where v.Devicename = c.Devicename ";
                    selectCmd += "  And v.Devicename in " + sList + " ";
                    selectCmd += "Order By v.Devicename, v.Timestamp ";
                }
                else if (sReportKind.ToString().Trim().ToUpper() == "小時交通平均速度統計")
                {
                    selectCmd += "Select v.timestamp as timestamp, ";
                    selectCmd += "       v.devicename as devicename, ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(ABS(v.car_speed)) as total, ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(case when c.lane_count >= 1 then ";
                    selectCmd += "                                         int(" + "db2inst1"  + ".DIV( " + "db2inst1"  + ".ZERO(v.connect_car_speed_lane1) * " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane1) + ";
                    selectCmd += "                                                                 " + "db2inst1"  + ".ZERO(v.big_car_speed_lane1)     * " + "db2inst1"  + ".ZERO(v.big_car_volume_lane1) + ";
                    selectCmd += "                                                                 " + "db2inst1"  + ".ZERO(v.small_car_speed_lane1)   * " + "db2inst1"  + ".ZERO(v.small_car_volume_lane1) ";
                    selectCmd += "                                                                , ";
                    selectCmd += "                                                                 " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane1) + " + "db2inst1"  + ".ZERO(v.big_car_volume_lane1) + " + "db2inst1"  + ".ZERO(v.small_car_volume_lane1) ";
                    selectCmd += "                                                                , ";
                    selectCmd += "                                                                 0 ";
                    selectCmd += "                                                               )) ";
                    selectCmd += "        else 0 end) as lane1, ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(case when c.lane_count >= 2 then ";
                    selectCmd += "                                          int(" + "db2inst1"  + ".DIV( " + "db2inst1"  + ".ZERO(v.connect_car_speed_lane2) * " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane2) + ";
                    selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.big_car_speed_lane2)     * " + "db2inst1"  + ".ZERO(v.big_car_volume_lane2) + ";
                    selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.small_car_speed_lane2)   * " + "db2inst1"  + ".ZERO(v.small_car_volume_lane2) ";
                    selectCmd += "                                                                 , ";
                    selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane2) + " + "db2inst1"  + ".ZERO(v.big_car_volume_lane2) + " + "db2inst1"  + ".ZERO(v.small_car_volume_lane2) ";
                    selectCmd += "                                                                 , ";
                    selectCmd += "                                                                  0 ";
                    selectCmd += "                                                                ))  ";
                    selectCmd += "        else 0 end) as lane2, ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(case when c.lane_count >= 3 then ";
                    selectCmd += "                                          int(" + "db2inst1"  + ".DIV( " + "db2inst1"  + ".ZERO(v.connect_car_speed_lane3) * " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane3) + ";
                    selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.big_car_speed_lane3)     * " + "db2inst1"  + ".ZERO(v.big_car_volume_lane3) + ";
                    selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.small_car_speed_lane3)   * " + "db2inst1"  + ".ZERO(v.small_car_volume_lane3) ";
                    selectCmd += "                                                                 , ";
                    selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane3) + " + "db2inst1"  + ".ZERO(v.big_car_volume_lane3) + " + "db2inst1"  + ".ZERO(v.small_car_volume_lane3) ";
                    selectCmd += "                                                                 , ";
                    selectCmd += "                                                                  0 ";
                    selectCmd += "                                                                ))  ";
                    selectCmd += "        else 0 end) as lane3, ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(case when c.lane_count >= 4 then  ";
                    selectCmd += "                                          int(" + "db2inst1"  + ".DIV( " + "db2inst1"  + ".ZERO(v.connect_car_speed_lane4) * " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane4) + ";
                    selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.big_car_speed_lane4)     * " + "db2inst1"  + ".ZERO(v.big_car_volume_lane4) + ";
                    selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.small_car_speed_lane4)   * " + "db2inst1"  + ".ZERO(v.small_car_volume_lane4) ";
                    selectCmd += "                                                                 , ";
                    selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane4) + " + "db2inst1"  + ".ZERO(v.big_car_volume_lane4) + " + "db2inst1"  + ".ZERO(v.small_car_volume_lane4) ";
                    selectCmd += "                                                                 , ";
                    selectCmd += "                                                                  0 ";
                    selectCmd += "                                                                )) ";
                    selectCmd += "        else 0 end) as lane4, ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(case when c.lane_count >= 5 then  ";
                    selectCmd += "                                          int(" + "db2inst1"  + ".DIV( " + "db2inst1"  + ".ZERO(v.connect_car_speed_lane5) * " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane5) + ";
                    selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.big_car_speed_lane5)     * " + "db2inst1"  + ".ZERO(v.big_car_volume_lane5) + ";
                    selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.small_car_speed_lane5)   * " + "db2inst1"  + ".ZERO(v.small_car_volume_lane5) ";
                    selectCmd += "                                                                 , ";
                    selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane5) + " + "db2inst1"  + ".ZERO(v.big_car_volume_lane5) + " + "db2inst1"  + ".ZERO(v.small_car_volume_lane5) ";
                    selectCmd += "                                                                 , ";
                    selectCmd += "                                                                  0 ";
                    selectCmd += "                                                                ))  ";
                    selectCmd += "        else 0 end) as lane5, ";
                    selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(case when c.lane_count >= 6 then  ";
                    selectCmd += "                                          int(" + "db2inst1"  + ".DIV( " + "db2inst1"  + ".ZERO(v.connect_car_speed_lane6) * " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane6) + ";
                    selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.big_car_speed_lane6)     * " + "db2inst1"  + ".ZERO(v.big_car_volume_lane6) + ";
                    selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.small_car_speed_lane6)   * " + "db2inst1"  + ".ZERO(v.small_car_volume_lane6) ";
                    selectCmd += "                                                                 , ";
                    selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane6) + " + "db2inst1"  + ".ZERO(v.big_car_volume_lane6) + " + "db2inst1"  + ".ZERO(v.small_car_volume_lane6) ";
                    selectCmd += "                                                                 , ";
                    selectCmd += "                                                                  0 ";
                    selectCmd += "                                                                 ))  ";
                    selectCmd += "        else 0 end) as lane6 ";
                    selectCmd += " From (Select * From " + "db2inst1"  + "." + sTableName_VD1Hr + " v Where v.Timestamp Between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) v, ";
                    selectCmd += "      " + "db2inst1"  + "." + sTableName_VdConfig + " c ";
                    selectCmd += "Where v.Devicename = c.Devicename ";
                    selectCmd += "  And v.Devicename in " + sList + " ";
                    selectCmd += "Order By v.Devicename, v.Timestamp ";
                }
                else
                {
                    // Do Nothing ...
                }

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "資料庫初始化"
        //資料庫初始化
        public void InitDB()
        {
            sConnstring ="Database=TCS;UserID=db2inst1;Password=db2inst1;Server=10.21.50.31:50000";
            conn = new DB2Connection(sConnstring);
            conn.Open();
        }
        #endregion

        #region "關閉資料庫"
        //關閉資料庫
        public void CloseDB()
        {
            conn.Close();
        }
        #endregion

        #region "將 VD by 路段群組的 View 內容以 DataTable 方式回傳"
        public DataTable GetVWSectionDataTable()
        {
            string selectCmd = "";

            selectCmd += "Select * ";
            selectCmd += "  From " + "db2inst1"  + "." + sTableName_vwVDGroup2 + " ";
            selectCmd += " Order By LineId, Direction1Desc, SectionId, DeviceName ";


            InitDB();
            da = new DB2DataAdapter(selectCmd, conn);
            DataTable DT = new DataTable();
            da.Fill(DT);

            da.Dispose();
            CloseDB();
            return DT;
        }
        #endregion

        #region "將 VD by 路段群組的 View (內容不包括匝道的部份,只有主線)內容以 DataTable 方式回傳"
        public DataTable GetVWSectionnotRDataTable()
        {
            string selectCmd = "";

            selectCmd += "Select * ";
            selectCmd += "  From " + "db2inst1"  + "." + sTableName_vwVDGroupnotr + " ";
            selectCmd += " Order By LineId, Direction1Desc, SectionId, DeviceName ";


            InitDB();
            da = new DB2DataAdapter(selectCmd, conn);
            DataTable DT = new DataTable();
            da.Fill(DT);

            da.Dispose();
            CloseDB();
            return DT;
        }
        #endregion

        #region "將 設備 by 路段群組的內容以 DataTable 方式回傳"
        public DataTable GetVWSectionDataTable(string DeviceType)
        {
            string selectCmd = "";

            selectCmd += "Select l.LineId, l.LineName, l.Direction, pl.VariableValue as DirectionDesc, s.SectionId, s.SectionName, vd.DeviceName, s.Direction as Direction1, p.VariableValue as Direction1Desc,m.MFCCNAME  ";
            selectCmd += "From " + "db2inst1"  + "." + sTableName_DeviceConfig + " vd  ";
            selectCmd += "LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + " l LEFT JOIN ( ";
            selectCmd += "Select VariableName, VariableValue ";
            selectCmd += "From " + "db2inst1"  + "." + sTableName_Parameter + "  ";
            selectCmd += "Where GroupName = 'LineDirection') pl ON pl.VariableName = l.Direction ON l.LineId = vd.LineId LEFT JOIN ( ";
            selectCmd += "Select VariableName, VariableValue  ";
            selectCmd += "From DB2INST1.TBLSYSPARAMETER ";
            selectCmd += "Where GroupName = 'DeviceDirection') p ON p.VariableName = vd.Direction LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupSection + " s ON s.SectionId = vd.SectionId  ";
            selectCmd += " LEFT JOIN " + "db2inst1"  + "." + sTableName_tblMFCCConfig + " m on vd.MFCCID=m.MFCCID  ";
            selectCmd += "Where vd.DEVICE_TYPE = '" + DeviceType + " ' ";
            selectCmd += "Order By LineId, Direction1Desc, SectionId, DeviceName ";

            InitDB();
            da = new DB2DataAdapter(selectCmd, conn);
            DataTable DT = new DataTable();
            da.Fill(DT);

            da.Dispose();
            CloseDB();
            return DT;
        }
        #endregion

        #region "將匝道 VD by 路段群組的 View 內容以 DataTable 方式回傳"
        public DataTable GetVWRampVdDataTable()
        {
            string selectCmd = "";

            //selectCmd += "Select * ";
            //selectCmd += "  From " + "db2inst1"  + "." + sTableName_vwVDRampGroup + " ";
            //selectCmd += " Order By LineId, Direction1Desc, SectionId, DeviceName ";


            selectCmd += "Select * ";
            selectCmd += "  From " + "db2inst1"  + "." + sTableName_vwVDRampGroup2 + " ";
            selectCmd += " Order By LineId, Direction1Desc, SectionId, DeviceName ";


            InitDB();
            da = new DB2DataAdapter(selectCmd, conn);
            DataTable DT = new DataTable();
            da.Fill(DT);

            da.Dispose();
            CloseDB();
            return DT;
        }
        #endregion

        #region "取得主線和路段清單"
        public DataTable GetLineSection()
        {
            string selectCmd = "";

            //selectCmd += "Select l.LineId, ";
            //selectCmd += "       l.LineName, ";
            //selectCmd += "       s.SectionId, ";
            //selectCmd += "       s.SectionName, ";
            //selectCmd += "       s.Direction as Direction1, ";
            //selectCmd += "       p.VariableValue as Direction1Desc ";
            //selectCmd += "  From " + "db2inst1"  + "." + sTableName_GroupSection + " s LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupDivision + " d1 ON d1.DivisionId = s.Start_DivisionId ";
            //selectCmd += "	                                                                                    LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupDivision + " d2 ON d2.DivisionId = s.End_DivisionId ";
            //selectCmd += "                                                                                      LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + " l ON l.LineId = s.LineId ";
            //selectCmd += "                                                                                      LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'DeviceDirection') p ON p.VariableName = s.Direction ";
            //selectCmd += " Order By l.LineId, s.Direction, d1.Mileage ";


            selectCmd += "Select l.LineId, ";
            selectCmd += "       l.LineName, ";
            selectCmd += "       s.SectionId, ";
            selectCmd += "       s.SectionName, ";
            selectCmd += "       s.Direction as Direction1, ";
            selectCmd += "       p.VariableValue as Direction1Desc ";
            selectCmd += "  From " + "db2inst1"  + "." + sTableName_GroupSection + " s LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupDivision + " d1 ON d1.DivisionId = s.Start_DivisionId ";
            selectCmd += "	                                                                                    LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupDivision + " d2 ON d2.DivisionId = s.End_DivisionId ";
            selectCmd += "                                                                                      LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + " l ON l.LineId = s.LineId ";
            selectCmd += "                                                                                      LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'DeviceDirection') p ON p.VariableName = s.Direction ";
            selectCmd += "                                                                                      LEFT JOIN " + "db2inst1"  + "." + sTableName_DeviceConfig + " d on  s.SectionId = d.LineId ";
            selectCmd += "                                                                                      LEFT JOIN " + "db2inst1"  + "." + sTableName_tblMFCCConfig + " m on d.MFCCID=m.MFCCID ";
            selectCmd += " Order By l.LineId, s.Direction, d1.Mileage ";


            InitDB();
            da = new DB2DataAdapter(selectCmd, conn);
            DataTable DT = new DataTable();
            da.Fill(DT);

            da.Dispose();
            CloseDB();
            return DT;
        }
        #endregion

        #region "取得交流道和路段清單"
        public DataTable GetRampSection()
        {
            string selectCmd = "";

            selectCmd += "Select r.LineId, ";
            selectCmd += "       r.LineName, ";
            selectCmd += "       r.SectionId, ";
            selectCmd += "       r.SectionName, ";
            selectCmd += "       r.Direction as Direction1, ";
            selectCmd += "       p.VariableValue as Direction1Desc ";
            selectCmd += "  From " + "db2inst1"  + "." + sTableName_vwRamp + " r LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'DeviceDirection') p ON p.VariableName = r.Direction ";
            selectCmd += " Order By r.LineId, r.Direction, r.Mileage ";


            InitDB();
            da = new DB2DataAdapter(selectCmd, conn);
            DataTable DT = new DataTable();
            da.Fill(DT);

            da.Dispose();
            CloseDB();
            return DT;
        }
        #endregion

        #region "取得所有路段內容以 DataTable 方式回傳"
        public DataTable GetAllSectionDev()
        {
            string selectCmd = "";

            selectCmd += "Select l.LineName, ";
            selectCmd += "       s.SectionName, ";
            selectCmd += "       p.VariableValue, ";
            selectCmd += "       (select MAX(v.car_volume) from " + "db2inst1"  + "." + sTableName_VD1Day + " v, " + "db2inst1"  + "." + sTableName_DeviceConfig + " d where d.Devicename = v.Devicename and d.sectionid = s.sectionid and date(v.timestamp) = date(current timestamp - 1 DAYS) ) as car_volume, ";
            selectCmd += "       " + "db2inst1"  + "." + sFunction_MaxVol_Dev + "((select MAX(v.car_volume) from " + "db2inst1"  + "." + sTableName_VD1Day + " v, " + "db2inst1"  + "." + sTableName_DeviceConfig + " d where d.Devicename = v.Devicename and d.sectionid = s.sectionid and date(v.timestamp) = date(current timestamp - 1 DAYS)), current timestamp - 1 DAYS, s.sectionid) as DeviceList ";
            selectCmd += "  From " + "db2inst1"  + "." + sTableName_GroupSection + " s LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupDivision + " d1 ON d1.DivisionId = s.Start_DivisionId ";
            selectCmd += "	                                                                                    LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupDivision + " d2 ON d2.DivisionId = s.End_DivisionId ";
            selectCmd += "                                                                                      LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + " l ON l.LineId = s.LineId ";
            selectCmd += "                                                                                      LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'DeviceDirection') p ON p.VariableName = s.Direction ";
            selectCmd += " Order By l.LineId, s.Direction, d1.Mileage ";

            InitDB();
            da = new DB2DataAdapter(selectCmd, conn);
            DataTable DT = new DataTable();
            da.Fill(DT);

            da.Dispose();
            CloseDB();
            return DT;
        }
        #endregion

        #region "取得所有設備清單"
        public DataTable GetDevList()
        {
            string selectCmd = "";

            selectCmd += "Select VariableName ";
            selectCmd += "  From " + "db2inst1"  + "." + sTableName_Parameter + " ";
            selectCmd += " Where UPPER(GroupName) = 'DEVICE_TYPE' ";
            selectCmd += " Order By GroupName, SysId ";


            InitDB();
            da = new DB2DataAdapter(selectCmd, conn);
            DataTable DT = new DataTable();
            da.Fill(DT);

            da.Dispose();
            CloseDB();
            return DT;
        }
        #endregion



        #region "取得 VD 清單並以 DataTable 傳回 ----------------------------------------------------------------------------------------------------------"
        public DataTable Get_RPT_VD(string sRPT, string sDevList)
        {
            try
            {
                string selectCmd = "";

                if (sRPT == "匝道平均每日交通量統計報表")
                {
                    selectCmd += "Select Distinct v.DeviceName as DEVICENAME , ";
                    selectCmd += "                l.LineName || p1.VariableValue || p2.VariableValue as ROADINFO1, ";
                    selectCmd += "                CONCAT(CONCAT(CONCAT(CONCAT(CONCAT(i.DivisionName,'-'),p2.VariableValue),'-'),(case when d.location_R = 'I' then '入口' else '出口' end)),'     (' || trim(char(v.Lane_Count)) || '車道)') as ROADINFO2 ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VdConfig + " v, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_DeviceConfig + " d, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_GroupLine + " l, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_GroupSection + " s, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_GroupDivision + " i, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_Parameter + " p1, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_Parameter + " p2 ";
                    selectCmd += " Where v.DeviceName in (" + sDevList + " ) ";
                    selectCmd += "   And d.SectionId = s.SectionId ";
                    selectCmd += "   And s.Start_DivisionId = i.DivisionId ";
                    selectCmd += "   And v.DeviceName = d.DeviceName ";
                    selectCmd += "   And d.LineId = l.LineId ";
                    selectCmd += "   And p1.VariableName = d.Location ";
                    selectCmd += "   And p1.GroupName = 'Location' ";
                    selectCmd += "   And p2.VariableName = d.Direction ";
                    selectCmd += "   And p2.GroupName = 'DeviceDirection' ";
                    selectCmd += "Order By v.DeviceName ";
                }
                else if (sRPT == "小時交通流量統計" ||
                         sRPT == "小時交通平均速度統計報表"
                        )
                {
                    selectCmd += "Select Distinct v.DeviceName as DEVICENAME , ";
                    selectCmd += "                l.LineName || p1.VariableValue || p2.VariableValue as ROADINFO1, ";
                    selectCmd += "                (case when d.Location = 'R' then CONCAT(CONCAT(CONCAT(CONCAT(CONCAT(i.DivisionName,'-'),p2.VariableValue),'-'),(case when d.location_R = 'I' then '入口' else '出口' end)),'     (' || trim(char(v.Lane_Count)) || '車道)') ";
                    selectCmd += "                 else '里程:' || char(cast((float(l.EndMileage - l.StartMileage)/1000) as decimal(5,2))) || '(' || trim(char(v.lane_count)) || '車道)' end) as ROADINFO2 ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VdConfig + " v, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_DeviceConfig + " d, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_GroupLine + " l, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_GroupSection + " s, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_GroupDivision + " i, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_Parameter + " p1, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_Parameter + " p2 ";
                    selectCmd += " Where v.DeviceName in (" + sDevList + " )";
                    selectCmd += "   And d.SectionId = s.SectionId ";
                    selectCmd += "   And s.Start_DivisionId = i.DivisionId ";
                    selectCmd += "   And v.DeviceName = d.DeviceName ";
                    selectCmd += "   And d.LineId = l.LineId ";
                    selectCmd += "   And p1.VariableName = d.Location ";
                    selectCmd += "   And p1.GroupName = 'Location' ";
                    selectCmd += "   And p2.VariableName = d.Direction ";
                    selectCmd += "   And p2.GroupName = 'DeviceDirection' ";
                    selectCmd += "Order By v.DeviceName ";
                }
                else if (sRPT == "現點速率調查交通資料記錄報表")
                {
                    selectCmd += "Select Distinct v.DeviceName as DEVICENAME , ";
                    selectCmd += "                l.LineName || p1.VariableValue || p2.VariableValue as ROADINFO1, ";
                    selectCmd += "                '里程:' || char(cast((float(l.EndMileage - l.StartMileage)/1000) as decimal(5,2))) || '(' || trim(char(v.lane_count)) || '車道)' as ROADINFO2 ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VdConfig + " v, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_DeviceConfig + " d, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_GroupLine + " l, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_Parameter + " p1, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_Parameter + " p2 ";
                    selectCmd += " Where v.DeviceName in ( " + sDevList + " ) ";
                    selectCmd += "   And v.DeviceName = d.DeviceName ";
                    selectCmd += "   And d.LineId = l.LineId ";
                    selectCmd += "   And p1.VariableName = d.Location ";
                    selectCmd += "   And p1.GroupName = 'Location' ";
                    selectCmd += "   And p2.VariableName = d.Direction ";
                    selectCmd += "   And p2.GroupName = 'DeviceDirection' ";
                    selectCmd += "Order By v.DeviceName ";
                }
                else if (sRPT == "路段旅行時間記錄報表")
                {
                    selectCmd += "Select Distinct v.DeviceName as DEVICENAME , ";
                    selectCmd += "                l.LineName || p1.VariableValue || p2.VariableValue as ROADINFO1, ";
                    selectCmd += "                '里程:' || char(cast((float(l.EndMileage - l.StartMileage)/1000) as decimal(5,2))) || '(' || trim(char(v.lane_count)) || '車道)' as ROADINFO2 ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VdConfig + " v, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_DeviceConfig + " d, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_GroupLine + " l, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_Parameter + " p1, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_Parameter + " p2 ";
                    selectCmd += " Where v.DeviceName in ( " + sDevList + " ) ";
                    selectCmd += "   And v.DeviceName = d.DeviceName ";
                    selectCmd += "   And d.LineId = l.LineId ";
                    selectCmd += "   And p1.VariableName = d.Location ";
                    selectCmd += "   And p1.GroupName = 'Location' ";
                    selectCmd += "   And p2.VariableName = d.Direction ";
                    selectCmd += "   And p2.GroupName = 'DeviceDirection' ";
                    selectCmd += "Order By v.DeviceName ";


                    selectCmd += " select  DISTINCT  (g.LineName ||  (case when substr(s.lineid,1,1) = 'N' then '高速公路' else '快速道路'  end) || p.variablevalue) as ROADINFO1, ";
                    selectCmd += " ' '  as ROADINFO2  ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_GroupSection + " s, ";
                    selectCmd += " left join (select variablename, variablevalue from db2inst1.tblsysparameter where upper(groupname) = 'DEVICEDIRECTION') p on p.variablename = s.direction  ";
                    selectCmd += " left join " + "db2inst1"  + "." + sTableName_tblgroupline + " g on s.LineId=g.LineId ";
                    selectCmd += " where s.lineid= 'T76' and s.Direction='E'  ";



                }
                else
                {
                    selectCmd += "Select Distinct v.DeviceName as DEVICENAME , ";
                    selectCmd += "                l.LineName || p1.VariableValue || p2.VariableValue as ROADINFO1, ";
                    selectCmd += "                '里程:' || char(cast((float(l.EndMileage - l.StartMileage)/1000) as decimal(5,2))) || '(' || trim(char(v.lane_count)) || '車道)' as ROADINFO2 ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VdConfig + " v, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_DeviceConfig + " d, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_GroupLine + " l, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_Parameter + " p1, ";
                    selectCmd += "       " + "db2inst1"  + "." + sTableName_Parameter + " p2 ";
                    selectCmd += " Where v.DeviceName in (" + sDevList + ") ";
                    selectCmd += "   And v.DeviceName = d.DeviceName ";
                    selectCmd += "   And d.LineId = l.LineId ";
                    selectCmd += "   And p1.VariableName = d.Location ";
                    selectCmd += "   And p1.GroupName = 'Location' ";
                    selectCmd += "   And p2.VariableName = d.Direction ";
                    selectCmd += "   And p2.GroupName = 'DeviceDirection' ";
                    selectCmd += "Order By v.DeviceName ";
                }

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }


        public DataTable Get_RPT_lineName(string sRPT, string lineid, string Direction)
        {
            try
            {
                string selectCmd = "";

               if (sRPT == "路段旅行時間記錄報表")
                {
                    selectCmd += " select  DISTINCT  (g.LineName ||  (case when substr(s.lineid,1,1) = 'N' then '高速公路' else '快速道路'  end) || p.variablevalue) as ROADINFO1, ";
                    selectCmd += " ' '  as ROADINFO2  ";
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_GroupSection + " s ";
                    selectCmd += " left join (select variablename, variablevalue from db2inst1.tblsysparameter where upper(groupname) = 'DEVICEDIRECTION') p on p.variablename = s.direction  ";
                    selectCmd += " left join " + "db2inst1"  + "." + sTableName_tblgroupline + " g on s.LineId=g.LineId ";
                    selectCmd += " where s.lineid= '" + lineid + "' and s.Direction='" + Direction + "'  ";
                }


          

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "========== 報表系統 DataTable =========="

        #region "一分鐘交通資料記錄報表"
        public DataTable Get_RPT_VD1MIN(string sDevList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";
                selectCmd += "Select (case when v.priority = 1 then To_CHAR(v.timestamp,'MM/DD') ";
                selectCmd += "             when v.priority = 2 then To_CHAR(v.timestamp,'HH24:MI')";
                selectCmd += "        else '' end) as timestamp, ";
                selectCmd += "       v.devicename as devicename,  ";
                selectCmd += "       v.degree,  ";
                selectCmd += "       v.total, ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.lane1 <= 0 then '-' else char(v.lane1) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.lane1_connect = '-1' OR v.lane1_connect = '0' then '-' else v.lane1_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.lane1_big = '-1' OR v.lane1_big = '0' then '-' else v.lane1_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.lane1_small = '-1' OR v.lane1_small = '0' then '-' else v.lane1_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.lane2 <= 0 then '-' else char(v.lane2) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.lane2_connect = '-1' OR v.lane2_connect = '0' then '-' else v.lane2_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.lane2_big = '-1' OR v.lane2_big = '0' then '-' else v.lane2_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.lane2_small = '-1' OR v.lane2_small = '0' then '-' else v.lane2_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.lane3 <= 0 then '-' else char(v.lane3) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.lane3_connect = '-1' OR v.lane3_connect = '0' then '-' else v.lane3_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.lane3_big = '-1' OR v.lane3_big = '0' then '-' else v.lane3_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.lane3_small = '-1' OR v.lane3_small = '0' then '-' else v.lane3_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.lane4 <= 0 then '-' else char(v.lane4) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.lane4_connect = '-1' OR v.lane4_connect = '0' then '-' else v.lane4_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.lane4_big = '-1' OR v.lane4_big = '0' then '-' else v.lane4_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.lane4_small = '-1' OR v.lane4_small = '0' then '-' else v.lane4_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.lane5 <= 0 then '-' else char(v.lane5) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.lane5_connect = '-1' OR lane5_connect = '0' then '-' else v.lane5_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.lane5_big = '-1' OR v.lane5_big = '0' then '-' else v.lane5_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.lane5_small = '-1' OR v.lane5_small = '0' then '-' else v.lane5_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.lane6 <= 0 then '-' else char(v.lane6) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.lane6_connect = '-1' OR v.lane6_connect = '0' then '-' else v.lane6_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.lane6_big = '-1' OR v.lane6_big = '0' then '-' else v.lane6_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.lane6_small = '-1' OR v.lane6_small = '0' then '-' else v.lane6_small end) else '-' end),  ";
                selectCmd += " v.priority ";
                //2009/12/18:SHIN:edit...改抓五分鐘table
                selectCmd += "  From " + "db2inst1"  + "." + sTableName_VWRPT_VD1MIN + " v, ";
                //selectCmd += "  From " + "db2inst1"  + "." + sTableName_VWRPT_VD5MIN + " v, ";
                selectCmd += "       " + "db2inst1"  + "." + sTableName_VdConfig + " c ";
                selectCmd += " Where c.devicename = v.devicename";
                selectCmd += "   And c.devicename in " + sDevList + " ";
                selectCmd += "   And v.timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += " Order By v.timestamp, v.devicename, v.priority ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "一分鐘車間距資料記錄報表"
        public DataTable Get_RPT_VD1MIN_INTERVAL(string sDevList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";
                selectCmd += "Select To_CHAR(v.timestamp,'MM/DD HH24:MI') as timestamp,  ";
                selectCmd += "       v.devicename, ";
                selectCmd += "		 (case when c.lane_count >= 1 then char(case when v.average_occupancy_lane1 <= 0 then '-' else char(v.average_occupancy_lane1) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.length_lane1 <= 0 then '-' else char(v.length_lane1) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.average_car_interval_lane1 <= 0 then '-' else char(v.average_car_interval_lane1) end) else '-' end), ";
                selectCmd += "";
                selectCmd += "		 (case when c.lane_count >= 2 then char(case when v.average_occupancy_lane2 <= 0 then '-' else char(v.average_occupancy_lane2) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.length_lane2 <= 0 then '-' else char(v.length_lane2) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.average_car_interval_lane2 <= 0 then '-' else char(v.average_car_interval_lane2) end) else '-' end), ";
                selectCmd += "";
                selectCmd += "		 (case when c.lane_count >= 3 then char(case when v.average_occupancy_lane3 <= 0 then '-' else char(v.average_occupancy_lane3) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.length_lane3 <= 0 then '-' else char(v.length_lane3) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.average_car_interval_lane3 <= 0 then '-' else char(v.average_car_interval_lane3) end) else '-' end), ";
                selectCmd += "";
                selectCmd += "		 (case when c.lane_count >= 4 then char(case when v.average_occupancy_lane4 <= 0 then '-' else char(v.average_occupancy_lane4) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.length_lane4 <= 0 then '-' else char(v.length_lane4) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.average_car_interval_lane4 <= 0 then '-' else char(v.average_car_interval_lane4) end) else '-' end), ";
                selectCmd += "";
                selectCmd += "		 (case when c.lane_count >= 5 then char(case when v.average_occupancy_lane5 <= 0 then '-' else char(v.average_occupancy_lane5) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.length_lane5 <= 0 then '-' else char(v.length_lane5) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.average_car_interval_lane5 <= 0 then '-' else char(v.average_car_interval_lane5) end) else '-' end), ";
                selectCmd += "";
                selectCmd += "		 (case when c.lane_count >= 6 then char(case when v.average_occupancy_lane6 <= 0 then '-' else char(v.average_occupancy_lane6) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.length_lane6 <= 0 then '-' else char(v.length_lane6) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.average_car_interval_lane6 <= 0 then '-' else char(v.average_car_interval_lane6) end) else '-' end), ";
                selectCmd += "";
                selectCmd += "       (case when v.car_length <= 0 then '-' else char(v.car_length) end), ";
                selectCmd += "       (case when v.average_car_interval <= 0 then '-' else char(v.average_car_interval) end) ";
                selectCmd += "      ";
                //2009/12/18:SHIN:edit...改抓五分鐘table
                selectCmd += "  From " + "db2inst1"  + "." + sTableName_VWRPT_VD1MIN_INTERVAL + " v, ";

                selectCmd += "       " + "db2inst1"  + "." + sTableName_VdConfig + " c ";
                selectCmd += " Where c.devicename = v.devicename ";
                selectCmd += "   And c.devicename in " + sDevList + " ";
                selectCmd += "   And v.timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += " Order By v.timestamp, v.devicename ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "五分鐘交通資料記錄報表"
        public DataTable Get_RPT_VD5MIN(string sDevList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";
                selectCmd += "Select (case when v.DataValidityRate< 100 then '*' else '' end),(case when v.priority = 1 then To_CHAR(v.timestamp,'MM/DD') ";
                selectCmd += "             when v.priority = 2 then To_CHAR(v.timestamp,'HH24:MI')";
                selectCmd += "        else '' end) as timestamp, ";
                selectCmd += "       v.devicename as devicename,  ";
                selectCmd += "       v.degree,  ";
                //selectCmd += "       (case when v.total < 0 then 0 else v.total end), ";
                selectCmd += "       DB2INST1.ZERO2DASH(case when v.total < 0 then 0 else v.total end), ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.lane1 <= 0 then '-' else char(v.lane1) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.lane1_connect = '-1' OR v.lane1_connect = '0' then '-' else v.lane1_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.lane1_big = '-1' OR v.lane1_big = '0' then '-' else v.lane1_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.lane1_small = '-1' OR v.lane1_small = '0' then '-' else v.lane1_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.lane2 <= 0 then '-' else char(v.lane2) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.lane2_connect = '-1' OR v.lane2_connect = '0' then '-' else v.lane2_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.lane2_big = '-1' OR v.lane2_big = '0' then '-' else v.lane2_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.lane2_small = '-1' OR v.lane2_small = '0' then '-' else v.lane2_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.lane3 <= 0 then '-' else char(v.lane3) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.lane3_connect = '-1' OR v.lane3_connect = '0' then '-' else v.lane3_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.lane3_big = '-1' OR v.lane3_big = '0' then '-' else v.lane3_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.lane3_small = '-1' OR v.lane3_small = '0' then '-' else v.lane3_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.lane4 <= 0 then '-' else char(v.lane4) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.lane4_connect = '-1' OR v.lane4_connect = '0' then '-' else v.lane4_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.lane4_big = '-1' OR v.lane4_big = '0' then '-' else v.lane4_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.lane4_small = '-1' OR v.lane4_small = '0' then '-' else v.lane4_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.lane5 <= 0 then '-' else char(v.lane5) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.lane5_connect = '-1' OR v.lane5_connect = '0' then '-' else v.lane5_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.lane5_big = '-1' OR v.lane5_big = '0' then '-' else v.lane5_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.lane5_small = '-1' OR v.lane5_small = '0' then '-' else v.lane5_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.lane6 <= 0 then '-' else char(v.lane6) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.lane6_connect = '-1' OR v.lane6_connect = '0' then '-' else v.lane6_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.lane6_big = '-1' OR v.lane6_big = '0' then '-' else v.lane6_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.lane6_small = '-1' OR v.lane6_small = '0' then '-' else v.lane6_small end) else '-' end),  ";
                selectCmd += "  v.priority  ";
                selectCmd += "  From " + "db2inst1"  + "." + sTableName_VWRPT_VD5MIN + " v, ";
                selectCmd += "       " + "db2inst1"  + "." + sTableName_VdConfig + " c ";
                selectCmd += " Where c.devicename = v.devicename ";
                selectCmd += "   And c.devicename in " + sDevList + " ";
                selectCmd += "   And v.timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += " Order By v.timestamp, v.devicename, v.priority ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "五分鐘車間距資料記錄報表"
        public DataTable Get_RPT_VD5MIN_INTERVAL(string sDevList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";
                selectCmd += "Select To_CHAR(v.timestamp,'MM/DD HH24:MI') as timestamp,  ";
                selectCmd += "       v.devicename, ";
                selectCmd += "		 (case when c.lane_count >= 1 then char(case when v.utility_lane1 <= 0 then '-' else char(v.utility_lane1) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.length_lane1 <= 0 then '-' else char(v.length_lane1) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.average_car_interval_lane1 <= 0 then '-' else char(v.average_car_interval_lane1) end) else '-' end), ";
                selectCmd += "";
                selectCmd += "		 (case when c.lane_count >= 2 then char(case when v.utility_lane2 <= 0 then '-' else char(v.utility_lane2) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.length_lane2 <= 0 then '-' else char(v.length_lane2) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.average_car_interval_lane2 <= 0 then '-' else char(v.average_car_interval_lane2) end) else '-' end), ";
                selectCmd += "";
                selectCmd += "		 (case when c.lane_count >= 3 then char(case when v.utility_lane3 <= 0 then '-' else char(v.utility_lane3) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.length_lane3 <= 0 then '-' else char(v.length_lane3) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.average_car_interval_lane3 <= 0 then '-' else char(v.average_car_interval_lane3) end) else '-' end), ";
                selectCmd += "";
                selectCmd += "		 (case when c.lane_count >= 4 then char(case when v.utility_lane4 <= 0 then '-' else char(v.utility_lane4) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.length_lane4 <= 0 then '-' else char(v.length_lane4) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.average_car_interval_lane4 <= 0 then '-' else char(v.average_car_interval_lane4) end) else '-' end), ";
                selectCmd += "";
                selectCmd += "		 (case when c.lane_count >= 5 then char(case when v.utility_lane5 <= 0 then '-' else char(v.utility_lane5) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.length_lane5 <= 0 then '-' else char(v.length_lane5) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.average_car_interval_lane5 <= 0 then '-' else char(v.average_car_interval_lane5) end) else '-' end), ";
                selectCmd += "";
                selectCmd += "		 (case when c.lane_count >= 6 then char(case when v.utility_lane6 <= 0 then '-' else char(v.utility_lane6) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.length_lane6 <= 0 then '-' else char(v.length_lane6) end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.average_car_interval_lane6 <= 0 then '-' else char(v.average_car_interval_lane6) end) else '-' end), ";
                selectCmd += "";
                selectCmd += "       (case when v.car_length <= 0 then '-' else char(v.car_length) end), ";
                selectCmd += "       (case when v.average_car_interval <= 0 then '-' else char(v.average_car_interval) end) ";
                selectCmd += "        ";
                selectCmd += "  From " + "db2inst1"  + "." + sTableName_VWRPT_VD5MIN_INTERVAL + " v, ";
                selectCmd += "       " + "db2inst1"  + "." + sTableName_VdConfig + " c ";
                selectCmd += " Where c.devicename = v.devicename ";
                selectCmd += "   And c.devicename in " + sDevList + " ";
                selectCmd += "   And v.timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += " Order By v.timestamp, v.devicename ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "現點速度記錄報表"
        public DataTable Get_RPT_VDSPOTSPEED(string sDevList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";
                int ilane = 0;
                string sAvgO = "";
                string subSql = "";
                string lane_volume_sql = "";



                selectCmd += "  select (case when x.priority = 3 then '' when x.priority = 2 then TO_char(x.timestamp,'HH24:MI:SS')  else TO_char(x.timestamp,'YYYY/MM/DD') end) AS timestamp, ";
                selectCmd += "  x.DeviceName,x.class,x.total,DB2INST1.ZERO2DASH_CHAR(x.connect_lane1),DB2INST1.ZERO2DASH_CHAR(x.big_lane1),x.small_lane1,x.connect_lane2,x.big_lane2,  ";
                selectCmd += "  x.small_lane2,x.connect_lane3,x.big_lane3,x.small_lane3,x.connect_lane4,x.big_lane4,x.small_lane4, ";
                selectCmd += "   x.connect_lane5,x.big_lane5,x.small_lane5,x.connect_lane6,x.big_lane6,x.small_lane6   ";
                selectCmd += "  from ";
                selectCmd += " (    ";
                selectCmd += "Select timestamp, ";
                selectCmd += "       s.DeviceName, ";
                selectCmd += "       '流量' as class, ";
                selectCmd += "       SUM(s.car_volume) as total, ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 1 then char(SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> 1 then 0 else s.car_volume end)) else '-' end) as connect_lane1, ";
                selectCmd += "       (case when c.lane_count >= 1 then char(SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> 1 then 0 else s.car_volume end)) else '-' end) as big_lane1, ";
                selectCmd += "       (case when c.lane_count >= 1 then char(SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> 1 then 0 else s.car_volume end)) else '-' end) as small_lane1, ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 2 then char(SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> 2 then 0 else s.car_volume end)) else '-' end) as connect_lane2, ";
                selectCmd += "       (case when c.lane_count >= 2 then char(SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> 2 then 0 else s.car_volume end)) else '-' end) as big_lane2, ";
                selectCmd += "       (case when c.lane_count >= 2 then char(SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> 2 then 0 else s.car_volume end)) else '-' end) as small_lane2, ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 3 then char(SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> 3 then 0 else s.car_volume end)) else '-' end) as connect_lane3, ";
                selectCmd += "       (case when c.lane_count >= 3 then char(SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> 3 then 0 else s.car_volume end)) else '-' end) as big_lane3, ";
                selectCmd += "       (case when c.lane_count >= 3 then char(SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> 3 then 0 else s.car_volume end)) else '-' end) as small_lane3, ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 4 then char(SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> 4 then 0 else s.car_volume end)) else '-' end) as connect_lane4, ";
                selectCmd += "       (case when c.lane_count >= 4 then char(SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> 4 then 0 else s.car_volume end)) else '-' end) as big_lane4, ";
                selectCmd += "       (case when c.lane_count >= 4 then char(SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> 4 then 0 else s.car_volume end)) else '-' end) as small_lane4, ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 5 then char(SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> 5 then 0 else s.car_volume end)) else '-' end) as connect_lane5, ";
                selectCmd += "       (case when c.lane_count >= 5 then char(SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> 5 then 0 else s.car_volume end)) else '-' end) as big_lane5, ";
                selectCmd += "       (case when c.lane_count >= 5 then char(SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> 5 then 0 else s.car_volume end)) else '-' end) as small_lane5, ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 6 then char(SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> 6 then 0 else s.car_volume end)) else '-' end) as connect_lane6, ";
                selectCmd += "       (case when c.lane_count >= 6 then char(SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> 6 then 0 else s.car_volume end)) else '-' end) as big_lane6, ";
                selectCmd += "       (case when c.lane_count >= 6 then char(SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> 6 then 0 else s.car_volume end)) else '-' end) as small_lane6, ";
                selectCmd += " ";
                selectCmd += "       1 as priority ";
                selectCmd += " ";
                selectCmd += "  From " + "db2inst1"  + "." + sTableName_SpotSpeed + " s, ";
                selectCmd += "       " + "db2inst1"  + "." + sTableName_VdConfig + " c ";
                selectCmd += " Where s.timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "   And s.devicename = c.devicename ";
                selectCmd += "   And s.devicename in " + sDevList + " ";
                selectCmd += " Group by s.DeviceName, c.lane_count,timestamp ";
                selectCmd += " ";
                selectCmd += "UNION ";
                selectCmd += " ";
                selectCmd += "Select timestamp, ";
                selectCmd += "       s.DeviceName, ";
                selectCmd += "       '速度' as class, ";
                selectCmd += "       (case when SUM(s.car_volume) = 0 then 0 else SUM(s.car_speed) / SUM(s.car_volume) end) as total, ";
                selectCmd += " ";
                ilane = 1;
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 3 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as connect_lane1, ";
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 2 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as big_lane1, ";
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 1 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as small_lane1, ";
                ilane = 2;
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 3 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as connect_lane2, ";
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 2 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as big_lane2, ";
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 1 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as small_lane2, ";
                ilane = 3;
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 3 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as connect_lane3, ";
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 2 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as big_lane3, ";
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 1 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as small_lane3, ";
                ilane = 4;
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 3 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as connect_lane4, ";
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 2 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as big_lane4, ";
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 1 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as small_lane4, ";
                ilane = 5;
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 3 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as connect_lane5, ";
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 2 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as big_lane5, ";
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 1 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as small_lane5, ";
                ilane = 6;
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 3 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as connect_lane6, ";
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 2 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as big_lane6, ";
                selectCmd += "		 (case when c.lane_count >= " + ilane + " then char(case when SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) = 0 then 0 else SUM((case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_speed end) * (case when s.car_class <> 1 then 0 else s.car_volume end)) / SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end) end) else '-' end) as small_lane6, ";
                selectCmd += " ";
                selectCmd += "       2 as priority ";
                selectCmd += " ";
                selectCmd += "  From " + "db2inst1"  + "." + sTableName_SpotSpeed + " s, ";
                selectCmd += "       " + "db2inst1"  + "." + sTableName_VdConfig + " c ";
                selectCmd += " Where s.timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "   And s.devicename = c.devicename ";
                selectCmd += "   And s.devicename in " + sDevList + " ";
                selectCmd += " Group by s.DeviceName, c.lane_count,timestamp ";
                selectCmd += " ";
                selectCmd += "UNION ";
                selectCmd += " ";
                selectCmd += "Select timestamp, ";
                selectCmd += "       s.DeviceName, ";
                selectCmd += "       '占量' as class, ";
                selectCmd += "       (case when count(*) = 0 then 0 else (select SUM(average_occupancy)/count(*) from " + "db2inst1"  + ".tblvddata5min where devicename = s.devicename and timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') group by devicename) end) as total, ";
                selectCmd += " ";

                ilane = 1;
                sAvgO = "average_occupancy_lane1";
                subSql = "(select SUM(" + sAvgO + ")/count(*) from " + "db2inst1"  + "." + sTableName_VD5Min + " where devicename = s.devicename and timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')  group by devicename)";
                lane_volume_sql = "SUM(case when s.lane_id <> " + ilane + " then 0 else s.car_volume end)";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as connect_lane1, ";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as big_lane1, ";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as small_lane1, ";
                selectCmd += " ";

                ilane = 2;
                sAvgO = "average_occupancy_lane2";
                subSql = "(select SUM(" + sAvgO + ")/count(*) from " + "db2inst1"  + "." + sTableName_VD5Min + " where devicename = s.devicename and timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')  group by devicename)";
                lane_volume_sql = "SUM(case when s.lane_id <> " + ilane + " then 0 else s.car_volume end)";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as connect_lane2, ";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as big_lane2, ";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as small_lane2, ";
                selectCmd += " ";

                ilane = 3;
                sAvgO = "average_occupancy_lane3";
                subSql = "(select SUM(" + sAvgO + ")/count(*) from " + "db2inst1"  + "." + sTableName_VD5Min + " where devicename = s.devicename and timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')  group by devicename)";
                lane_volume_sql = "SUM(case when s.lane_id <> " + ilane + " then 0 else s.car_volume end)";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as connect_lane3, ";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as big_lane3, ";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as small_lane3, ";
                selectCmd += " ";

                ilane = 4;
                sAvgO = "average_occupancy_lane4";
                subSql = "(select SUM(" + sAvgO + ")/count(*) from " + "db2inst1"  + "." + sTableName_VD5Min + " where devicename = s.devicename and timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')  group by devicename)";
                lane_volume_sql = "SUM(case when s.lane_id <> " + ilane + " then 0 else s.car_volume end)";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as connect_lane4, ";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as big_lane4, ";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as small_lane4, ";
                selectCmd += " ";

                ilane = 5;
                sAvgO = "average_occupancy_lane5";
                subSql = "(select SUM(" + sAvgO + ")/count(*) from " + "db2inst1"  + "." + sTableName_VD5Min + " where devicename = s.devicename and timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')  group by devicename)";
                lane_volume_sql = "SUM(case when s.lane_id <> " + ilane + " then 0 else s.car_volume end)";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as connect_lane5, ";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as big_lane5, ";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as small_lane5, ";
                selectCmd += " ";

                ilane = 6;
                sAvgO = "average_occupancy_lane6";
                subSql = "(select SUM(" + sAvgO + ")/count(*) from " + "db2inst1"  + "." + sTableName_VD5Min + " where devicename = s.devicename and timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')  group by devicename)";
                lane_volume_sql = "SUM(case when s.lane_id <> " + ilane + " then 0 else s.car_volume end)";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 3 then 0 when s.car_class = 3 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as connect_lane6, ";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 2 then 0 when s.car_class = 2 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as big_lane6, ";
                selectCmd += "       (case when c.lane_count >= " + ilane + " then char(case when ( " + subSql + " * " + lane_volume_sql + ") = 0 then 0 else (" + subSql + " * SUM(case when s.car_class <> 1 then 0 when s.car_class = 1 and s.lane_id <> " + ilane + " then 0 else s.car_volume end)) / (" + subSql + " * " + lane_volume_sql + ") end) else '-' end) as small_lane6, ";
                selectCmd += " ";
                selectCmd += "       3 as priority ";
                selectCmd += " ";
                selectCmd += "  From " + "db2inst1"  + "." + sTableName_SpotSpeed + " s, ";
                selectCmd += "       " + "db2inst1"  + "." + sTableName_VdConfig + " c ";
                selectCmd += " Where s.timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "   And s.devicename = c.devicename ";
                selectCmd += "   And s.devicename in " + sDevList + " ";
                selectCmd += " Group by s.DeviceName, c.lane_count,timestamp  ";
                selectCmd += "  )  x    ";
                selectCmd += "  order by x.devicename, x.timestamp, x.priority ";




                InitDB();
                //舊的做法
                //da = new DB2DataAdapter(selectCmd, conn);
                //DataTable DT = new DataTable();
                //da.Fill(DT);

                //新的做法
                DB2Command da = new DB2Command(selectCmd, conn);
                DataTable DT = new DataTable();
                DT.Load(da.ExecuteReader());


                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }


        #endregion

        #region "一小時交通資料記錄報表"
        public DataTable Get_RPT_VD1HR(string sDevList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";
                selectCmd += "Select (case when v.DataValidityRate< 100 then '*' else '' end),(case when v.priority = 1 then To_CHAR(v.timestamp,'MM/DD') ";
                selectCmd += "             when v.priority = 2 then To_CHAR(v.timestamp,'HH24:MI')";
                selectCmd += "        else '' end) as timestamp, ";
                selectCmd += "       v.devicename as devicename,  ";
                selectCmd += "       v.degree,  ";
                selectCmd += "       DB2INST1.ZERO2DASH(case when v.total < 0 then 0 else v.total end), ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.lane1 <= 0 then '-' else char(v.lane1) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.lane1_connect = '-1' then '-' when v.lane1_connect = '0' then '-' else v.lane1_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.lane1_big = '-1' then '-' when v.lane1_big ='0' then '-' else v.lane1_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.lane1_small = '-1' then '-' when v.lane1_small ='0' then '-' else v.lane1_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.lane2 <= 0 then '-' else char(v.lane2) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.lane2_connect = '-1' then '-' when v.lane2_connect ='0' then '-' else v.lane2_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.lane2_big = '-1' then '-' when v.lane2_big ='0' then '-' else v.lane2_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.lane2_small = '-1' then '-' when v.lane2_small ='0' then '-' else v.lane2_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.lane3 <= 0 then '-' else char(v.lane3) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.lane3_connect = '-1' then '-' when v.lane3_connect ='0' then '-' else v.lane3_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.lane3_big = '-1' then '-' when v.lane3_big ='0' then '-' else v.lane3_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.lane3_small = '-1' then '-' when v.lane3_small ='0' then '-' else v.lane3_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.lane4 <= 0 then '-' else char(v.lane4) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.lane4_connect = '-1' then '-'  when v.lane4_connect ='0' then '-' else v.lane4_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.lane4_big = '-1' then '-' when v.lane4_big ='0' then '-'  else v.lane4_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.lane4_small = '-1' then '-' when v.lane4_small ='0' then '-' else v.lane4_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.lane5 <= 0 then '-' else char(v.lane5) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.lane5_connect = '-1' then '-' when v.lane5_connect ='0' then '-' else v.lane5_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.lane5_big = '-1' then '-' when v.lane5_big ='0' then '-' else v.lane5_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.lane5_small = '-1' then '-' when v.lane5_small ='0' then '-'  else v.lane5_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.lane6 <= 0 then '-' else char(v.lane6) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.lane6_connect = '-1' then '-' when v.lane6_connect ='0' then '-' else v.lane6_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.lane6_big = '-1' then '-' when v.lane6_big ='0' then '-' else v.lane6_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.lane6_small = '-1' then '-' when v.lane6_small ='0' then '-' else v.lane6_small end) else '-' end),  ";
                selectCmd += "  v.priority   ";
                selectCmd += "  From " + "db2inst1"  + "." + sTableName_VWRPT_VD1HR + " v, ";
                //selectCmd += "  From " + "db2inst1"  + "." + sTableName_VWRPT_VD5MIN + " v, ";

                selectCmd += "       " + "db2inst1"  + "." + sTableName_VdConfig + " c ";
                selectCmd += " Where c.devicename = v.devicename ";
                selectCmd += "   And c.devicename in " + sDevList + " ";
                selectCmd += "   And v.timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += " Order By v.timestamp, v.devicename, v.priority ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "一天交通資料記錄報表"
        public DataTable Get_RPT_VD1DAY(string sDevList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";
                selectCmd += "Select (case when v.DataValidityRate< 100 then '*' else '' end),(case when v.priority = 1 then To_CHAR(v.timestamp,'MM/DD') ";
                selectCmd += "             when v.priority = 2 then To_CHAR(v.timestamp,'HH24:MI')";
                selectCmd += "        else '' end) as timestamp, ";
                selectCmd += "       v.devicename as devicename,  ";
                selectCmd += "       v.degree,  ";
                selectCmd += "       DB2INST1.ZERO2DASH(case when v.total < 0 then 0 else v.total end), ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.lane1 <= 0 then '-' else char(v.lane1) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.lane1_connect = '-1' then '-' when v.lane1_connect= '0' then '-' else v.lane1_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.lane1_big = '-1' then '-' when  v.lane1_big='0' then '-'  else v.lane1_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.lane1_small = '-1' then '-' when v.lane1_small='0' then '-' else v.lane1_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.lane2 <= 0 then '-' else char(v.lane2) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.lane2_connect = '-1' then '-' when v.lane2_connect ='0' then '-' else v.lane2_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.lane2_big = '-1' then '-' when v.lane2_big ='0' then '-'  else v.lane2_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.lane2_small = '-1' then '-' when v.lane2_small ='0' then '-'  else v.lane2_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.lane3 < 0 then 0 else v.lane3 end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.lane3_connect = '-1' then '-' when v.lane3_connect ='0' then '-' else v.lane3_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.lane3_big = '-1' then '-' when v.lane3_big ='0' then '-' else v.lane3_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.lane3_small = '-1' then '-' when v.lane3_small ='0' then '-' else v.lane3_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.lane4 <= 0 then '-' else char(v.lane4) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.lane4_connect = '-1' then '-' when v.lane4_connect ='0' then '-' else v.lane4_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.lane4_big = '-1' then '-' when v.lane4_big ='0' then '-'  else v.lane4_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.lane4_small = '-1' then '-' when v.lane4_small ='0' then '-' else v.lane4_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.lane5 < 0 then '-' else char(v.lane5) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.lane5_connect = '-1' then '-' when v.lane5_connect ='0' then '-' else v.lane5_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.lane5_big = '-1' then '-' when v.lane5_big ='0' then '-' else v.lane5_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.lane5_small = '-1' then '-' when v.lane5_small ='0' then '-' else v.lane5_small end) else '-' end),  ";
                selectCmd += " ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.lane6 <= 0 then '-' else char(v.lane6) end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.lane6_connect = '-1' then '-' when v.lane6_connect ='0' then '-' else v.lane6_connect end) else '-' end),  ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.lane6_big = '-1' then '-' when v.lane6_big ='0' then '-'  else v.lane6_big end) else '-' end), ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.lane6_small = '-1' then '-' when v.lane6_small ='0' then '-' else v.lane6_small end) else '-' end),  ";
                selectCmd += " v.priority ";
                //2009/12/18:SHIN:edit...改抓五分鐘table
                selectCmd += "  From " + "db2inst1"  + "." + sTableName_VWRPT_VD1DAY + " v, ";
                //selectCmd += "  From " + "db2inst1"  + "." + sTableName_VWRPT_VD5MIN + " v, ";
                selectCmd += "       " + "db2inst1"  + "." + sTableName_VdConfig + " c ";
                selectCmd += " Where c.devicename = v.devicename ";
                selectCmd += "   And c.devicename in " + sDevList + " ";
                selectCmd += "   And v.timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += " Order By v.timestamp, v.devicename, v.priority ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "全區主線全日交通量統計報表"
        public DataTable Get_RPT_LINEFULLDAY(string sDevList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";
                selectCmd += "Select l.LineName, ";
                selectCmd += "       s.SectionName, ";
                selectCmd += "       p.VariableValue, ";
                selectCmd += "       (select (case when MAX(v.car_volume) is null then '-' when MAX(v.car_volume)=0 then '-' else char(MAX(v.car_volume)) end) from " + "db2inst1"  + "." + sTableName_VD1Day + " v, " + "db2inst1"  + "." + sTableName_DeviceConfig + " d where d.Devicename = v.Devicename and d.sectionid = s.sectionid  and d.location in ('F','H') and date(v.timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) ) as car_volume, ";
                //2009/12/18:SHIN:edit...改抓五分鐘table
                selectCmd += "       " + "db2inst1"  + "." + sFunction_MaxVol_Dev + "((select MAX(v.car_volume) from " + "db2inst1"  + "." + sTableName_VD1Day + " v, " + "db2inst1"  + "." + sTableName_DeviceConfig + " d where d.Devicename = v.Devicename and d.sectionid = s.sectionid  and d.location in ('F','H') and date(v.timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'))), timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'), timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'),s.sectionid) as DeviceList ";
                //selectCmd += "       " + "db2inst1"  + "." + sFunction_MaxVol_Dev + "((select MAX(v.car_volume) from " + "db2inst1"  + "." + sTableName_VD5Min + " v, " + "db2inst1"  + "." + sTableName_DeviceConfig + " d where d.Devicename = v.Devicename and d.sectionid = s.sectionid and date(v.timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'))), timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'), s.sectionid) as DeviceList ";
                selectCmd += "  From " + "db2inst1"  + "." + sTableName_GroupSection + " s LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupDivision + " d1 ON d1.DivisionId = s.Start_DivisionId ";
                selectCmd += "	                                                                                    LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupDivision + " d2 ON d2.DivisionId = s.End_DivisionId ";
                selectCmd += "                                                                                      LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + " l ON l.LineId = s.LineId ";
                selectCmd += "                                                                                      LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'DeviceDirection') p ON p.VariableName = s.Direction ";
                selectCmd += " Where UPPER(s.SectionName) in " + sDevList + " ";
                selectCmd += " Order By l.LineId, s.Direction, d1.Mileage ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "全區匝道全日交通量統計報表"
        public DataTable Get_RPT_RAMPFULLDAY(string sDevList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";
                selectCmd += "Select r.LineName, ";
                selectCmd += "       r.DivisionName, ";
                selectCmd += "       r.DirectionDesc, ";
                selectCmd += "       r.Volume, ";
                selectCmd += "       r.Devicename";
                selectCmd += "  From ( ";
                selectCmd += "Select Distinct ";
                selectCmd += "       r.LineName as LineName, ";
                selectCmd += "       r.DivisionName as DivisionName, ";
                selectCmd += "       r.DirectionDesc as DirectionDesc, ";
                selectCmd += "       (select (case when MAX(v.car_volume) is null then '-' when MAX(v.car_volume)=0  then '-' else char(MAX(v.car_volume)) end) ";
                selectCmd += "          from " + "db2inst1"  + ".tblvddata1day v, ";
                selectCmd += "               " + "db2inst1"  + ".tbldeviceconfig d ";
                selectCmd += "         where d.devicename = v.devicename ";
                selectCmd += "           and d.location = 'R' ";
                selectCmd += "           and date(v.timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) ";
                selectCmd += "           and d.SectionId = r.SectionId  ";
                selectCmd += "           and UPPER(d.Location_R) = UPPER(r.Location_R) ";
                selectCmd += "        ) as Volume, ";
                selectCmd += "        " + "db2inst1"  + "." + sFunction_MaxVol_Dev_Ramp + "( ";
                selectCmd += "        (select MAX(v.car_volume) from " + "db2inst1"  + ".tblvddata1day v, " + "db2inst1"  + ".tbldeviceconfig d ";
                selectCmd += "          where d.devicename = v.devicename ";
                selectCmd += "            and d.location = 'R' ";
                selectCmd += "            and date(v.timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) ";
                selectCmd += "            and d.SectionId = r.SectionId  ";
                selectCmd += "            and UPPER(d.Location_R) = UPPER(r.Location_R) ";
                selectCmd += "        ), timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'), r.SectionId, r.Location_R) as Devicename, ";
                selectCmd += "        r.LineId, r.Mileage ";
                selectCmd += "  From " + "db2inst1"  + "." + sTableName_vwRamp + " r ";
                selectCmd += " Where r.SectionName in " + sDevList + " ";
                selectCmd += " ) r ";
                selectCmd += " Order By r.LineId, r.Mileage ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "現場終端設備狀態記錄統計"
        public DataTable Get_RPT_DeviceStatus(string sDevList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";
                //selectCmd += "Select l.Devicename as Devicename, ";
                //selectCmd += "       n.LineName as LineName, ";
                //selectCmd += "       (case when d.Location = 'R' and Location_R = 'I' then concat(p.VariableValue,'入口') ";
                //selectCmd += "             when d.Location = 'R' and Location_R = 'O' then concat(p.VariableValue,'出口') ";
                //selectCmd += "        else p.VariableValue end) as Location, ";
                //selectCmd += "       p2.VariableValue as Direction, ";
                //selectCmd += "       cast(round(cast(d.Mile_M as double)/1000,2) as decimal(8,2)) as Mileage, ";
                //selectCmd += "       Replace(l.Memo,':','/') as memo, ";
                //selectCmd += "       l.Timestamp as StartTimestamp, ";
                //selectCmd += "       '' as EndTimestamp ";
                //selectCmd += "  From " + "db2inst1"  + "." + sTableName_DeviceStatusLog + " l LEFT JOIN " + "db2inst1"  + "." + sTableName_DeviceConfig + " d LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + " n ON n.lineid = d.lineid ";
                //selectCmd += "                                                                                         LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'Location') p ON p.VariableName = d.Location ";
                //selectCmd += "                                                                                         LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'DeviceDirection') p2 ON p2.VariableName = d.Direction ";
                //selectCmd += "       ON d.Devicename = l.Devicename ";
                //selectCmd += " Where d.Device_Type in " + sDevList + " ";
                //selectCmd += "   And l.Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                //selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                //selectCmd += "   And (l.HW_Status_1 <> 0 OR ";
                //selectCmd += "        l.HW_Status_2 <> 0 OR ";
                //selectCmd += "        l.HW_Status_3 <> 0 OR ";
                //selectCmd += "        l.HW_Status_4 <> 0) ";
                //selectCmd += " Order By l.Timestamp, l.Devicename ";


                //原本table 換成 tblDeviceStatusLogAbnormal edit by SHIN
                selectCmd += " Select l.Devicename as Devicename,n.LineName as LineName,";
                selectCmd += " (case when d.Location = 'R' and Location_R = 'I' then concat(p.VariableValue,'入口') when d.Location = 'R' and Location_R = 'O' then concat(p.VariableValue,'出口') else p.VariableValue end) as Location, p2.VariableValue as Direction,";
                selectCmd += " cast(round(cast(d.Mile_M as double)/1000,2) as decimal(8,2)) as Mileage,";
                selectCmd += " db2inst1.HWSTATUS_BY_BIT(l.bit,d.DEVICE_TYPE),to_char(l.starttime,'YYYY-MM-DD HH24:MI:SS') as StartTimestamp,to_char(l.ENDTIME,'YYYY-MM-DD HH24:MI:SS') as EndTimestamp  ";
                selectCmd += " From db2inst1.tblDeviceStateLogAbnormal l ";
                selectCmd += " LEFT JOIN db2inst1.tblDeviceConfig d ";
                selectCmd += " LEFT JOIN db2inst1.tblGroupLine n ON n.lineid = d.lineid ";
                selectCmd += " LEFT JOIN (select VariableName, VariableValue from db2inst1.tblSysParameter where GroupName = 'Location') p ON p.VariableName = d.Location";
                selectCmd += " LEFT JOIN (select VariableName, VariableValue from db2inst1.tblSysParameter where GroupName = 'DeviceDirection') p2 ON p2.VariableName = d.Direction ";
                selectCmd += " ON d.Devicename = l.Devicename  ";
                selectCmd += " Where d.Device_Type in " + sDevList + "  And l.starttime between ('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                selectCmd += " and ('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')  ";
                selectCmd += " Order By LineName desc, Devicename  ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion


        #region "定時比對記錄報表"
        public DataTable Get_RPT_tblDeviceStatusLog(string sDevList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";
                selectCmd += "Select l.Devicename as Devicename, ";
                selectCmd += "       n.LineName as LineName, ";
                selectCmd += "       (case when d.Location = 'R' and Location_R = 'I' then concat(p.VariableValue,'入口') ";
                selectCmd += "             when d.Location = 'R' and Location_R = 'O' then concat(p.VariableValue,'出口') ";
                selectCmd += "        else p.VariableValue end) as Location, ";
                selectCmd += "       p2.VariableValue as Direction, ";
                selectCmd += "       cast(round(cast(d.Mile_M as double)/1000,2) as decimal(8,2)) as Mileage, ";
                selectCmd += "       to_char(l.TimeStamp,'YYYY-MM-DD HH24:MI:SS') as TimeStamp, l.display as display,l.DEVICE_DISPLAY as DEVICE_DISPLAY,'不符' as comparison ";
                selectCmd += "  From " + "db2inst1"  + "." + sTableName_tblDeviceComparisonLog + " l LEFT JOIN " + "db2inst1"  + "." + sTableName_DeviceConfig + " d LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + " n ON n.lineid = d.lineid ";
                selectCmd += "                                                                                         LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'Location') p ON p.VariableName = d.Location ";
                selectCmd += "                                                                                         LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'DeviceDirection') p2 ON p2.VariableName = d.Direction ";
                selectCmd += "       ON d.Devicename = l.Devicename ";
                selectCmd += " Where d.Device_Type in " + sDevList + " ";
                selectCmd += "   And l.Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                //selectCmd += " AND comparison in('Y','N') ";
                selectCmd += " Order By l.Devicename,l.Timestamp  ";


                InitDB();

                //da = new DB2DataAdapter(selectCmd, conn);
                //DataTable DT = new DataTable();
                //da.Fill(DT);

                //新的做法
                DB2Command da = new DB2Command(selectCmd, conn);
                DataTable DT = new DataTable();
                DT.Load(da.ExecuteReader());


                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "資訊可變標誌即時資料報表"
        public DataTable Get_RPT_tblDeviceStatus(string sDevList)
        {
            try
            {
                string selectCmd = "";
                selectCmd += " SELECT l.Devicename as Devicename,n.LineName as LineName,  ";
                selectCmd += " (case when l.connectstatus='Y' THEN '連線' when l.connectstatus='N' then '離線' ELSE '-' END)  as connectstatus,  ";
                selectCmd += " (case when connectstatus='Y' then db2inst1.OP_MODE(l.op_mode) when connectstatus='N' THEN '-' ELSE '-'  end ) as op_mode,  ";
                selectCmd += " (case when connectstatus='Y' then db2inst1.OP_STATUS(l.op_status) when connectstatus='N' THEN '-' ELSE '-' end) AS op_status,  ";
                selectCmd += " l.display as display  ";
                selectCmd += " FROM " + "db2inst1"  + "." + sTableName_DeviceStatus + " l   ";
                selectCmd += " LEFT JOIN " + "db2inst1"  + "." + sTableName_DeviceConfig + " d ON l.Devicename = d.Devicename  ";
                selectCmd += " LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + "  n ON  d.lineid =n.lineid  ";
                selectCmd += " where d.Device_Type='CMS' AND l.Devicename IN " + sDevList + "  ";
                selectCmd += " order by l.Devicename,l.TimeStamp  ";


                InitDB();

                //da = new DB2DataAdapter(selectCmd, conn);
                //DataTable DT = new DataTable();
                //da.Fill(DT);

                //新的做法
                DB2Command da = new DB2Command(selectCmd, conn);
                DataTable DT = new DataTable();
                DT.Load(da.ExecuteReader());


                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "現場終端設備運作記錄"
        public DataTable Get_RPT_DeviceOpStatus(string sDevList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";

                //selectCmd += "Select l.Devicename as Devicename, ";
                //selectCmd += "       n.LineName as LineName, ";
                //selectCmd += "       (case when d.Location = 'R' and Location_R = 'I' then concat(p.VariableValue,'入口') ";
                //selectCmd += "             when d.Location = 'R' and Location_R = 'O' then concat(p.VariableValue,'出口') ";
                //selectCmd += "        else p.VariableValue end ";
                //selectCmd += "       ) as Location, ";
                //selectCmd += "       p2.VariableValue as Direction, ";
                //selectCmd += "       cast(round(cast(d.Mile_M as double)/1000,2) as decimal(8,2)) as Mileage, ";
                //selectCmd += "       to_char(l.Timestamp,'YYYY-MM-DD HH24:MI:SS'), ";
                //selectCmd += "       (case when l.connectstatus = 'Y' then '連線' else '離線' end) as connectstatus, ";
                //selectCmd += "       (case when l.connectstatus <> 'Y' then '-' when l.connectstatus is null then '-' else " + "db2inst1"  + ".OP_STATUS(case when l.op_status is null then 0 else l.op_status end) end) as op_status, ";
                //selectCmd += "       (case  when l.connectstatus <> 'Y' then '-' when l.connectstatus is null then '-' else (case when l.display is null then '-' when l.display = '' then '-' else l.display end) end) as display ";
                //selectCmd += "  From " + "db2inst1"  + "." + sTableName_DeviceStatusLog + " l LEFT JOIN " + "db2inst1"  + "." + sTableName_DeviceConfig + " d LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + " n ON n.lineid = d.lineid ";
                //selectCmd += "                                                                                         LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'Location') p ON p.VariableName = d.Location ";
                //selectCmd += "                                                                                         LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'DeviceDirection') p2 ON p2.VariableName = d.Direction ";
                //selectCmd += "       ON d.Devicename = l.Devicename ";
                //selectCmd += " Where d.Device_Type in " + sDevList + " ";
                //selectCmd += "   And l.Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                //selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                //selectCmd += " Order By l.DeviceName, l.timestamp ";



                selectCmd += "Select s.devicename,l.linename, loc.variablevalue,dir.variablevalue, ";
                selectCmd += "       decimal(round(float(d.mile_m)/1000,2),8,2) as mileage,to_char(s.timestamp,'YYYY-MM-DD HH24:MI:SS'), ";
                selectCmd += "       (case when s.type = 'C' then (case when s.result = 3 then '斷線' else '連線' end) else '' end) as comm_state, ";
                selectCmd += "             (case when s.type = 'S' then DB2INST1.OP_STATUS((case when s.result is null then 0 else s.result end)) else '-' end) as op_status, ";
                selectCmd += "        DB2INST1.DisplayTOChinese(s.display,d.device_type,s.type) as display   ";
                selectCmd += "  From db2inst1.tbldevicestatelog s";
                selectCmd += " LEFt JOIN db2inst1.tbldeviceconfig d ON d.devicename = s.devicename ";
                selectCmd += " LEFT JOIN db2inst1.tblgroupline l ON l.lineid = d.lineid ";
                selectCmd += " LEFT JOIN (select variablename, variablevalue from db2inst1.tblsysparameter where UPPER(groupname) = 'LOCATION') loc ON loc.variablename = d.location  ";
                selectCmd += " LEFT JOIN (select variablename, variablevalue from db2inst1.tblsysparameter where UPPER(groupname) = 'DEVICEDIRECTION') dir ON dir.variablename = d.direction ";
                selectCmd += " Where s.type in ('C','S','D') and d.Device_Type in " + sDevList + " ";
                selectCmd += "   And s.Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += " Order By s.DeviceName, s.timestamp ";



                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "設備狀態即時監視"
        public DataTable Get_RPT_DeviceMonitor(string sDevList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";
                //selectCmd += "Select d.device_type, ";
                //selectCmd += "       d.Devicename, ";
                //selectCmd += "       (case when l.connectstatus = 'Y' then '連線' else '離線' end) as connectstatus, ";
                //selectCmd += "       " + "db2inst1"  + ".OP_MODE(case when d.op_mode is null then 0 else d.op_mode end) as op_mode, ";
                //selectCmd += "       l.memo ";
                //selectCmd += "  From " + "db2inst1"  + "." + sTableName_DeviceConfig + " d, ";
                //selectCmd += "       " + "db2inst1"  + "." + sTableName_DeviceStatusLog + " l, ";
                //selectCmd += "       (select x.devicename, x.timestamp ";
                //selectCmd += "          from " + "db2inst1"  + "." + sTableName_DeviceStatusLog + " x ";
                //selectCmd += "         where x.timestamp = (select max(timestamp) from " + "db2inst1"  + "." + sTableName_DeviceStatusLog + " where devicename = x.devicename) ";
                //selectCmd += "         group by x.devicename, x.timestamp) xl ";
                //selectCmd += " Where l.devicename = d.devicename ";
                //selectCmd += "   And l.devicename = xl.devicename ";
                //selectCmd += "   And l.timestamp = xl.timestamp ";
                //selectCmd += "   And d.Device_Type in " + sDevList + " ";
                //selectCmd += " Order By d.device_type, d.DeviceName ";
                //SHIN add
                selectCmd += "Select d.device_type, ";
                selectCmd += "       d.Devicename, s.LINENAME, ";
                selectCmd += "       (case when d.comm_state = 3 then '斷線' when d.enable = 'N' then '斷線'   else '連線' end), ";
                selectCmd += "       (case when d.comm_state = 3 then '-' when d.enable = 'N' then '斷線' when d.comm_state is null then '-' else db2inst1.OP_MODE(case when d.op_mode is null then 0 else d.op_mode end) end) as op_mode, ";
                selectCmd += "       (case when d.comm_state = 3 then '-' when d.enable = 'N' then '斷線' when d.comm_state is null  then '-' else (case when d.hw_status_1=0 and d.hw_status_2 =0 and d.hw_status_3=0 and d.hw_status_4=0  then '正常'  else db2inst1.DEVICESTATUS(d.hw_status_1,d.hw_status_2,d.hw_status_3,d.hw_status_4,d.device_type)  end) end)    ";
                selectCmd += "  From " + "db2inst1"  + "." + sTableName_DeviceConfig + " d LEFT JOIN ";
                selectCmd += "       " + "db2inst1"  + "." + sTableName_tblgroupline + " s ON d.LINEID=s.LINEID  ";
                selectCmd += "   Where d.Device_Type in " + sDevList + " ";
                selectCmd += " Order By d.device_type, d.DeviceName,s.LINENAME ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "全區主線小時路段平均速度統計"
        public DataTable Get_RPT_SectionCarSpeed(string sList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";
                //selectCmd += "Select l.LineName, s.SectionName, p.VariableValue, ";
                //selectCmd += "       (case when ";
                //selectCmd += "                  (select " + "db2inst1"  + ".DIV(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_speed) * " + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), 2) ";
                //selectCmd += "                     from " + "db2inst1"  + "." + sTableName_VD5Min + " v left join " + "db2inst1"  + "." + sTableName_DeviceConfig + " d on d.devicename = v.devicename ";
                //selectCmd += "                    where d.sectionid = s.sectionid ";
                //selectCmd += "                      and date(v.timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) ";
                //selectCmd += "                    group by d.sectionid ";
                //selectCmd += "                  ) is null then 0 ";
                //selectCmd += "        else                       ";
                //selectCmd += "                  (select cast(" + "db2inst1"  + ".DIV(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_speed) * " + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), 2) as decimal(8,2)) ";
                //selectCmd += "                     from " + "db2inst1"  + "." + sTableName_VD5Min + " v left join " + "db2inst1"  + "." + sTableName_DeviceConfig + " d on d.devicename = v.devicename ";
                //selectCmd += "                    where d.sectionid = s.sectionid ";
                //selectCmd += "                      and date(v.timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) ";
                //selectCmd += "                    group by d.sectionid )  ";
                //selectCmd += "        end) as car_speed ";
                //selectCmd += "  From " + "db2inst1"  + "." + sTableName_GroupSection + " s LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupDivision + " d1 ON d1.DivisionId = s.Start_DivisionId ";
                //selectCmd += "                                                                                      LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupDivision + " d2 ON d2.DivisionId = s.End_DivisionId ";
                //selectCmd += "                                                                                      LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + " l ON l.LineId = s.LineId ";
                //selectCmd += "                                                                                      LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'DeviceDirection') p ON p.VariableName = s.Direction ";
                //selectCmd += " Where s.SectionName in " + sList + " ";
                //selectCmd += " Order By l.LineId, s.Direction, d1.Mileage ";

                selectCmd += "Select l.LineName, s.SectionName, p.VariableValue, ";
                selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(INT(case when ";
                selectCmd += "                  (select " + "db2inst1"  + ".DIV(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_speed) * " + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), 2) ";
                selectCmd += "                     from " + "db2inst1"  + "." + sTableName_VD5Min + " v left join " + "db2inst1"  + "." + sTableName_DeviceConfig + " d on d.devicename = v.devicename ";
                selectCmd += "                    where d.sectionid = s.sectionid ";
                selectCmd += "                      and date(v.timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) ";
                selectCmd += "                    group by d.sectionid ";
                selectCmd += "                  ) is null then 0 ";
                selectCmd += "        else                       ";
                selectCmd += "                  (select cast(" + "db2inst1"  + ".DIV(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_speed) * " + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), 2) as decimal(8,2)) ";
                selectCmd += "                     from " + "db2inst1"  + "." + sTableName_VD5Min + " v left join " + "db2inst1"  + "." + sTableName_DeviceConfig + " d on d.devicename = v.devicename ";
                selectCmd += "                    where d.sectionid = s.sectionid ";
                selectCmd += "                      and date(v.timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) ";
                selectCmd += "                    group by d.sectionid )  ";
                selectCmd += "        end)) as car_speed ";
                selectCmd += "  From " + "db2inst1"  + "." + sTableName_GroupSection + " s LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupDivision + " d1 ON d1.DivisionId = s.Start_DivisionId ";
                selectCmd += "                                                                                      LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupDivision + " d2 ON d2.DivisionId = s.End_DivisionId ";
                selectCmd += "                                                                                      LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + " l ON l.LineId = s.LineId ";
                selectCmd += "                                                                                      LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'DeviceDirection') p ON p.VariableName = s.Direction ";
                selectCmd += " Where s.SectionName in " + sList + " ";
                selectCmd += " Order By l.LineId, s.Direction, d1.Mileage ";


                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "主線平均每日交通量統計報表"
        public DataTable Get_RPT_LineDayVolume(string sList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";
                selectCmd += "Select x.Devicename, ";
                selectCmd += "       l.LineName, ";
                selectCmd += "       p.VariableValue as Direction, ";
                selectCmd += "       cast(cast(d.Mile_M as double)/1000 as decimal(8,2)) as Mileage, ";
                selectCmd += "       x.date, ";
                selectCmd += "db2inst1"  + ".ZERO2DASH(x.connect_car_volume), ";
                selectCmd += "db2inst1"  + ".ZERO2DASH(x.connect_car_volume_rate), ";
                selectCmd += "db2inst1"  + ".ZERO2DASH(x.big_car_volume), ";
                selectCmd += "db2inst1"  + ".ZERO2DASH(x.big_car_volume_rate), ";
                selectCmd += "db2inst1"  + ".ZERO2DASH(x.small_car_volume), ";
                selectCmd += "db2inst1"  + ".ZERO2DASH(x.small_car_volume_rate), ";
                selectCmd += "db2inst1"  + ".ZERO2DASH(x.car_volume) ";
                selectCmd += "  From ";
                selectCmd += "( ";
                selectCmd += "       Select v.Devicename, ";
                selectCmd += "              date(v.Timestamp) as date, ";
                selectCmd += "              (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane1)) +  ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane2)) +  ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane3)) + ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane4)) + ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane5)) + ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane6)) + ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane7)) ";
                selectCmd += "              ) as connect_car_volume, ";
                selectCmd += "              " + "db2inst1"  + "." + sFunction_DIV2 + "( ";
                selectCmd += "                            (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane1)) +  ";
                selectCmd += "                             SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane2)) +  ";
                selectCmd += "                             SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane3)) + ";
                selectCmd += "                             SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane4)) + ";
                selectCmd += "                             SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane5)) + ";
                selectCmd += "                             SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane6)) + ";
                selectCmd += "                             SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane7)) ";
                selectCmd += "                             ), ";
                selectCmd += "                           SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                selectCmd += "                           2 ";
                selectCmd += "                          )*100 as connect_car_volume_rate, ";
                selectCmd += "              (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane1)) +  ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane2)) +  ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane3)) + ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane4)) + ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane5)) + ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane6)) + ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane7)) ";
                selectCmd += "              ) as big_car_volume, ";
                selectCmd += "              " + "db2inst1"  + "." + sFunction_DIV2 + "( ";
                selectCmd += "                           (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane1)) +  ";
                selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane2)) +  ";
                selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane3)) + ";
                selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane4)) + ";
                selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane5)) + ";
                selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane6)) + ";
                selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane7)) ";
                selectCmd += "                            ), ";
                selectCmd += "                           SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                selectCmd += "                           2 ";
                selectCmd += "                          )*100 as big_car_volume_rate, ";
                selectCmd += "              (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane1)) +  ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane2)) +  ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane3)) + ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane4)) + ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane5)) + ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane6)) + ";
                selectCmd += "               SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane7)) ";
                selectCmd += "               ) as small_car_volume, ";
                selectCmd += "              " + "db2inst1"  + "." + sFunction_DIV2 + "( ";
                selectCmd += "                           (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane1)) +  ";
                selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane2)) +  ";
                selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane3)) + ";
                selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane4)) + ";
                selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane5)) + ";
                selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane6)) + ";
                selectCmd += "                            SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane7)) ";
                selectCmd += "                            ), ";
                selectCmd += "                           SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                selectCmd += "                           2 ";
                selectCmd += "                          )*100 as small_car_volume_rate, ";
                selectCmd += "              SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)) as car_volume ";
                selectCmd += "         From " + "db2inst1"  + "." + sTableName_VD5Min + " v ";
                selectCmd += "        Where date(v.Timestamp) between date(timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) and date(timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) ";
                selectCmd += "        Group By v.Devicename, date(v.Timestamp) ";
                selectCmd += ") x LEFT JOIN " + "db2inst1"  + "." + sTableName_DeviceConfig + " d LEFT JOIN " + "db2inst1"  + "." + sTableName_GroupLine + " l ON l.LineId = d.LineId  ";
                selectCmd += "                                                                                             LEFT JOIN (select VariableName, VariableValue from " + "db2inst1"  + "." + sTableName_Parameter + " where GroupName = 'DeviceDirection') p ON p.VariableName = d.Direction ";
                selectCmd += "    ON d.Devicename = x.Devicename  ";
                selectCmd += " Where x.DeviceName in " + sList + " ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "匝道平均每日交通量統計報表"
        public DataTable Get_RPT_RampDayVolume(string sList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";

                //selectCmd += "Select ROW_NUMBER() over() as RowNum, ";
                //selectCmd += "       v.Devicename, ";
                //selectCmd += "       date(v.Timestamp) as date, ";
                //selectCmd += "       (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane1)) +  ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane2)) +  ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane3)) + ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane4)) + ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane5)) + ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane6)) + ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane7)) ";
                //selectCmd += "       ) as connect_car_volume, ";
                //selectCmd += "       cast( ";
                //selectCmd += "       " + "db2inst1"  + "." + sFunction_DIV + "( ";
                //selectCmd += "                    (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane1)) +  ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane2)) +  ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane3)) + ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane4)) + ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane5)) + ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane6)) + ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane7)) ";
                //selectCmd += "                    ), ";
                //selectCmd += "                    SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                //selectCmd += "                    2 ";
                //selectCmd += "                   )*100 as decimal(8,2)) as connect_car_volume_rate, ";
                //selectCmd += "       (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane1)) +  ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane2)) +  ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane3)) + ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane4)) + ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane5)) + ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane6)) + ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane7)) ";
                //selectCmd += "       ) as big_car_volume, ";
                //selectCmd += "       cast( ";
                //selectCmd += "       " + "db2inst1"  + "." + sFunction_DIV + "( ";
                //selectCmd += "                    (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane1)) +  ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane2)) +  ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane3)) + ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane4)) + ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane5)) + ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane6)) + ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane7)) ";
                //selectCmd += "                    ), ";
                //selectCmd += "                    SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                //selectCmd += "                    2 ";
                //selectCmd += "                    )*100 as decimal(8,2)) as big_car_volume_rate, ";
                //selectCmd += "       (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane1)) +  ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane2)) +  ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane3)) + ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane4)) + ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane5)) + ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane6)) + ";
                //selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane7)) ";
                //selectCmd += "       ) as small_car_volume, ";
                //selectCmd += "       cast( ";
                //selectCmd += "       " + "db2inst1"  + "." + sFunction_DIV + "( ";
                //selectCmd += "                    (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane1)) +  ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane2)) +  ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane3)) + ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane4)) + ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane5)) + ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane6)) + ";
                //selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane7)) ";
                //selectCmd += "                    ), ";
                //selectCmd += "                    SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                //selectCmd += "                    2 ";
                //selectCmd += "                    )*100 as decimal(8,2)) as small_car_volume_rate, ";
                //selectCmd += "       SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)) as car_volume ";
                //selectCmd += " From (Select * From " + "db2inst1"  + "." + sTableName_VD5Min + " v Where v.Timestamp Between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) v ";
                //selectCmd += "Where v.Devicename in " + sList + " ";
                //selectCmd += "Group By v.Devicename, date(v.Timestamp) ";
                //selectCmd += "Order By v.Devicename, date(v.Timestamp) ";

                selectCmd += "Select ROW_NUMBER() over() as RowNum, ";
                selectCmd += "       v.Devicename, ";
                selectCmd += "       date(v.Timestamp) as date, ";
                selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane1)) +  ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane2)) +  ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane3)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane4)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane5)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane6)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane7)) ";
                selectCmd += "       ) as connect_car_volume, ";
                selectCmd += "       cast( ";
                selectCmd += "       " + "db2inst1"  + "." + sFunction_DIV + "( ";
                selectCmd += "                    (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane1)) +  ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane2)) +  ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane3)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane4)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane5)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane6)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.connect_car_volume_lane7)) ";
                selectCmd += "                    ), ";
                selectCmd += "                    SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                selectCmd += "                    2 ";
                selectCmd += "                   )*100 as decimal(8,2)) as connect_car_volume_rate, ";
                selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane1)) +  ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane2)) +  ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane3)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane4)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane5)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane6)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane7)) ";
                selectCmd += "       ) as big_car_volume, ";
                selectCmd += "       cast( ";
                selectCmd += "       " + "db2inst1"  + "." + sFunction_DIV + "( ";
                selectCmd += "                    (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane1)) +  ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane2)) +  ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane3)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane4)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane5)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane6)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.big_car_volume_lane7)) ";
                selectCmd += "                    ), ";
                selectCmd += "                    SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                selectCmd += "                    2 ";
                selectCmd += "                    )*100 as decimal(8,2)) as big_car_volume_rate, ";
                selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane1)) +  ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane2)) +  ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane3)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane4)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane5)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane6)) + ";
                selectCmd += "        SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane7)) ";
                selectCmd += "       ) as small_car_volume, ";
                selectCmd += "       cast( ";
                selectCmd += "       " + "db2inst1"  + "." + sFunction_DIV + "( ";
                selectCmd += "                    (SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane1)) +  ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane2)) +  ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane3)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane4)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane5)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane6)) + ";
                selectCmd += "                     SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.small_car_volume_lane7)) ";
                selectCmd += "                    ), ";
                selectCmd += "                    SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume)), ";
                selectCmd += "                    2 ";
                selectCmd += "                    )*100 as decimal(8,2)) as small_car_volume_rate, ";
                selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(SUM(" + "db2inst1"  + "." + sFunction_ZERO + "(v.car_volume))) as car_volume ";
                selectCmd += " From (Select * From " + "db2inst1"  + "." + sTableName_VD5Min + " v Where v.Timestamp Between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) v ";
                selectCmd += "Where v.Devicename in " + sList + " ";
                selectCmd += "Group By v.Devicename, date(v.Timestamp) ";
                selectCmd += "Order By v.Devicename, date(v.Timestamp) ";


                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "小時交通流量統計報表"
        public DataTable Get_RPT_HourVolume(string sList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";

                selectCmd += "Select to_char(v.timestamp,'YYYY-MM-DD HH24:MI') as timestamp, ";
                selectCmd += "       v.devicename as devicename, ";
                selectCmd += "       (case when v.connect_car_volume_lane1 < 0 then 0 else v.connect_car_volume_lane1 end) +  ";
                selectCmd += "       (case when v.big_car_volume_lane1 < 0 then 0 else v.big_car_volume_lane1 end) +  ";
                selectCmd += "       (case when v.small_car_volume_lane1 < 0 then 0 else v.small_car_volume_lane1 end) + ";
                selectCmd += "       (case when v.connect_car_volume_lane2 < 0 then 0 else v.connect_car_volume_lane2 end) +  ";
                selectCmd += "       (case when v.big_car_volume_lane2 < 0 then 0 else v.big_car_volume_lane2 end) +  ";
                selectCmd += "       (case when v.small_car_volume_lane2 < 0 then 0 else v.small_car_volume_lane2 end) + ";
                selectCmd += "       (case when v.connect_car_volume_lane3 < 0 then 0 else v.connect_car_volume_lane3 end) +  ";
                selectCmd += "       (case when v.big_car_volume_lane3 < 0 then 0 else v.big_car_volume_lane3 end) +  ";
                selectCmd += "       (case when v.small_car_volume_lane3 < 0 then 0 else v.small_car_volume_lane3 end) + ";
                selectCmd += "       (case when v.connect_car_volume_lane4 < 0 then 0 else v.connect_car_volume_lane4 end) +  ";
                selectCmd += "       (case when v.big_car_volume_lane4 < 0 then 0 else v.big_car_volume_lane4 end) +  ";
                selectCmd += "       (case when v.small_car_volume_lane4 < 0 then 0 else v.small_car_volume_lane4 end) + ";
                selectCmd += "       (case when v.connect_car_volume_lane5 < 0 then 0 else v.connect_car_volume_lane5 end) +  ";
                selectCmd += "       (case when v.big_car_volume_lane5 < 0 then 0 else v.big_car_volume_lane5 end) +  ";
                selectCmd += "       (case when v.small_car_volume_lane5 < 0 then 0 else v.small_car_volume_lane5 end) + ";
                selectCmd += "       (case when v.connect_car_volume_lane6 < 0 then 0 else v.connect_car_volume_lane6 end) +  ";
                selectCmd += "       (case when v.big_car_volume_lane6 < 0 then 0 else v.big_car_volume_lane6 end) +  ";
                selectCmd += "       (case when v.small_car_volume_lane6 < 0 then 0 else v.small_car_volume_lane6 end) ";
                selectCmd += "       as total, ";
                selectCmd += " ";
                selectCmd += "       (case when v.connect_car_volume_lane1 < 0 then 0 else v.connect_car_volume_lane1 end) +  ";
                selectCmd += "       (case when v.connect_car_volume_lane2 < 0 then 0 else v.connect_car_volume_lane2 end) +  ";
                selectCmd += "       (case when v.connect_car_volume_lane3 < 0 then 0 else v.connect_car_volume_lane3 end) +  ";
                selectCmd += "       (case when v.connect_car_volume_lane4 < 0 then 0 else v.connect_car_volume_lane4 end) +  ";
                selectCmd += "       (case when v.connect_car_volume_lane5 < 0 then 0 else v.connect_car_volume_lane5 end) +  ";
                selectCmd += "       (case when v.connect_car_volume_lane6 < 0 then 0 else v.connect_car_volume_lane6 end) as total_connect, ";
                selectCmd += " ";
                selectCmd += "       (case when v.big_car_volume_lane1 < 0 then 0 else v.big_car_volume_lane1 end) +  ";
                selectCmd += "       (case when v.big_car_volume_lane2 < 0 then 0 else v.big_car_volume_lane2 end) +  ";
                selectCmd += "       (case when v.big_car_volume_lane3 < 0 then 0 else v.big_car_volume_lane3 end) +  ";
                selectCmd += "       (case when v.big_car_volume_lane4 < 0 then 0 else v.big_car_volume_lane4 end) +  ";
                selectCmd += "       (case when v.big_car_volume_lane5 < 0 then 0 else v.big_car_volume_lane5 end) +  ";
                selectCmd += "       (case when v.big_car_volume_lane6 < 0 then 0 else v.big_car_volume_lane6 end) as total_big, ";
                selectCmd += " ";
                selectCmd += "       (case when v.small_car_volume_lane1 < 0 then 0 else v.small_car_volume_lane1 end) +  ";
                selectCmd += "       (case when v.small_car_volume_lane2 < 0 then 0 else v.small_car_volume_lane2 end) +  ";
                selectCmd += "       (case when v.small_car_volume_lane3 < 0 then 0 else v.small_car_volume_lane3 end) +  ";
                selectCmd += "       (case when v.small_car_volume_lane4 < 0 then 0 else v.small_car_volume_lane4 end) +  ";
                selectCmd += "       (case when v.small_car_volume_lane5 < 0 then 0 else v.small_car_volume_lane5 end) +  ";
                selectCmd += "       (case when v.small_car_volume_lane6 < 0 then 0 else v.small_car_volume_lane6 end) as total_small, ";
                selectCmd += " ";//車道1
                selectCmd += "       (case when c.lane_count >= 1 then char((case when v.connect_car_volume_lane1 < 0 then 0 else v.connect_car_volume_lane1 end) +  ";
                selectCmd += "                                         (case when v.big_car_volume_lane1 < 0 then 0 else v.big_car_volume_lane1 end) +  ";
                selectCmd += "                                         (case when v.small_car_volume_lane1 < 0 then 0 else v.small_car_volume_lane1 end)) ";
                selectCmd += "        else '-' end) as lane1, ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.connect_car_volume_lane1 <= 0 then '-' else char(v.connect_car_volume_lane1) end) else '-' end) as lane1_connect, ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.big_car_volume_lane1 <= 0 then '-' else char(v.big_car_volume_lane1) end) else '-' end) as lane1_big, ";
                selectCmd += "       (case when c.lane_count >= 1 then char(case when v.small_car_volume_lane1 <= 0 then '-' else char(v.small_car_volume_lane1) end) else '-' end) as lane1_small, ";
                selectCmd += " ";//車道2
                selectCmd += "       (case when c.lane_count >= 2 then char((case when v.connect_car_volume_lane2 < 0 then 0 else v.connect_car_volume_lane2 end) +  ";
                selectCmd += "                                         (case when v.big_car_volume_lane2 < 0 then 0 else v.big_car_volume_lane2 end) +  ";
                selectCmd += "                                         (case when v.small_car_volume_lane2 < 0 then 0 else v.small_car_volume_lane2 end)) ";
                selectCmd += "        else '-' end) as lane2, ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.connect_car_volume_lane2 <= 0 then '-' else char(v.connect_car_volume_lane2) end) else '-' end) as lane2_connect, ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.big_car_volume_lane2 <= 0 then '-' else char(v.big_car_volume_lane2) end) else '-' end) as lane2_big, ";
                selectCmd += "       (case when c.lane_count >= 2 then char(case when v.small_car_volume_lane2 <= 0 then '-' else char(v.small_car_volume_lane2) end) else '-' end) as lane2_small, ";
                selectCmd += " ";//車道3
                selectCmd += "       (case when c.lane_count >= 3 then char((case when v.connect_car_volume_lane3 < 0 then 0 else v.connect_car_volume_lane3 end) +  ";
                selectCmd += "                                         (case when v.big_car_volume_lane3 < 0 then 0 else v.big_car_volume_lane3 end) +  ";
                selectCmd += "                                         (case when v.small_car_volume_lane3 < 0 then 0 else v.small_car_volume_lane3 end)) ";
                selectCmd += "        else '-' end) as lane3, ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.connect_car_volume_lane3 <= 0 then '-' else char(v.connect_car_volume_lane3) end) else '-' end) as lane3_connect, ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.big_car_volume_lane3 <= 0 then '-' else char(v.big_car_volume_lane3) end) else '-' end) as lane3_big, ";
                selectCmd += "       (case when c.lane_count >= 3 then char(case when v.small_car_volume_lane3 <= 0 then '-' else char(v.small_car_volume_lane3) end) else '-' end) as lane3_small, ";
                selectCmd += " ";//車道4
                selectCmd += "       (case when c.lane_count >= 4 then char((case when v.connect_car_volume_lane4 < 0 then 0 else v.connect_car_volume_lane4 end) +  ";
                selectCmd += "                                         (case when v.big_car_volume_lane4 < 0 then 0 else v.big_car_volume_lane4 end) +  ";
                selectCmd += "                                         (case when v.small_car_volume_lane4 < 0 then 0 else v.small_car_volume_lane4 end)) ";
                selectCmd += "        else '-' end) as lane4, ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.connect_car_volume_lane4 <= 0 then '-' else char(v.connect_car_volume_lane4) end) else '-' end) as lane4_connect, ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.big_car_volume_lane4 <= 0 then '-' else char(v.big_car_volume_lane4) end) else '-' end) as lane4_big, ";
                selectCmd += "       (case when c.lane_count >= 4 then char(case when v.small_car_volume_lane4 <= 0 then '-' else char(v.small_car_volume_lane4) end) else '-' end) as lane4_small, ";
                selectCmd += " ";//車道5
                selectCmd += "       (case when c.lane_count >= 5 then char((case when v.connect_car_volume_lane5 < 0 then 0 else v.connect_car_volume_lane5 end) +  ";
                selectCmd += "                                         (case when v.big_car_volume_lane5 < 0 then 0 else v.big_car_volume_lane5 end) +  ";
                selectCmd += "                                         (case when v.small_car_volume_lane5 < 0 then 0 else v.small_car_volume_lane5 end)) ";
                selectCmd += "        else '-' end) as lane5, ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.connect_car_volume_lane5 <= 0 then '-' else char(v.connect_car_volume_lane5) end) else '-' end) as lane5_connect, ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.big_car_volume_lane5 <= 0 then '-' else char(v.big_car_volume_lane5) end) else '-' end) as lane5_big, ";
                selectCmd += "       (case when c.lane_count >= 5 then char(case when v.small_car_volume_lane5 <= 0 then '-' else char(v.small_car_volume_lane5) end) else '-' end) as lane5_small, ";
                selectCmd += " ";//車道6
                selectCmd += "       (case when c.lane_count >= 6 then char((case when v.connect_car_volume_lane6 < 0 then 0 else v.connect_car_volume_lane6 end) +  ";
                selectCmd += "                                         (case when v.big_car_volume_lane6 < 0 then 0 else v.big_car_volume_lane6 end) +  ";
                selectCmd += "                                         (case when v.small_car_volume_lane6 < 0 then 0 else v.small_car_volume_lane6 end)) ";
                selectCmd += "        else '-' end) as lane6, ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.connect_car_volume_lane6 <= 0 then '-' else char(v.connect_car_volume_lane6) end) else '-' end) as lane6_connect, ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.big_car_volume_lane6 <= 0 then '-'else char(v.big_car_volume_lane6) end) else '-' end) as lane6_big, ";
                selectCmd += "       (case when c.lane_count >= 6 then char(case when v.small_car_volume_lane6 <= 0 then '-' else char(v.small_car_volume_lane6) end) else '-' end) as lane6_small ";
                selectCmd += " ";
                //2009/12/18:SHIN:edit...改抓五分鐘table
                //selectCmd += " From (Select * From " + "db2inst1"  + "." + sTableName_VD1Hr + " v Where v.Timestamp Between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) v, ";
                selectCmd += " From (Select * From " + "db2inst1"  + "." + sTableName_VD5Min + " v Where v.Timestamp Between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) v, ";
                selectCmd += "      " + "db2inst1"  + "." + sTableName_VdConfig + " c ";
                selectCmd += "Where v.Devicename = c.Devicename ";
                selectCmd += "  And v.Devicename in " + sList + " ";
                selectCmd += "Order By v.Devicename, v.Timestamp ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "小時交通流量統計報表VD"
        public DataTable Get_RPT_HourVolumeVD(string sList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";

                selectCmd += "Select v.devicename ";
                //2009/12/18:SHIN:edit...改抓五分鐘table
                //selectCmd += " From (Select * From " + "db2inst1"  + "." + sTableName_VD1Hr + " v Where v.Timestamp Between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) v, ";
                selectCmd += " From (Select * From " + "db2inst1"  + "." + sTableName_VD5Min + " v Where v.Timestamp Between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) v, ";
                selectCmd += "      " + "db2inst1"  + "." + sTableName_VdConfig + " c ";
                selectCmd += "Where v.Devicename = c.Devicename ";
                selectCmd += "  And v.Devicename in " + sList + " ";
                selectCmd += " Group By v.Devicename ";
                selectCmd += " Order By v.Devicename ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "小時交通平均速度統計報表"
        public DataTable Get_RPT_HourSpeed(string sList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";

                selectCmd += "Select to_char(v.timestamp,'YYYY-MM-DD HH24:MI:SS') as timestamp, ";
                selectCmd += "       v.devicename as devicename, ";
                selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(ABS(v.car_speed)) as total, ";
                selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(case when c.lane_count >= 1 then ";
                selectCmd += "                                         int(" + "db2inst1"  + ".DIV( " + "db2inst1"  + ".ZERO(v.connect_car_speed_lane1) * " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane1) + ";
                selectCmd += "                                                                 " + "db2inst1"  + ".ZERO(v.big_car_speed_lane1)     * " + "db2inst1"  + ".ZERO(v.big_car_volume_lane1) + ";
                selectCmd += "                                                                 " + "db2inst1"  + ".ZERO(v.small_car_speed_lane1)   * " + "db2inst1"  + ".ZERO(v.small_car_volume_lane1) ";
                selectCmd += "                                                                , ";
                selectCmd += "                                                                 " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane1) + " + "db2inst1"  + ".ZERO(v.big_car_volume_lane1) + " + "db2inst1"  + ".ZERO(v.small_car_volume_lane1) ";
                selectCmd += "                                                                , ";
                selectCmd += "                                                                 0 ";
                selectCmd += "                                                               )) ";
                selectCmd += "        else 0 end) as lane1, ";
                selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(case when c.lane_count >= 2 then ";
                selectCmd += "                                          int(" + "db2inst1"  + ".DIV( " + "db2inst1"  + ".ZERO(v.connect_car_speed_lane2) * " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane2) + ";
                selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.big_car_speed_lane2)     * " + "db2inst1"  + ".ZERO(v.big_car_volume_lane2) + ";
                selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.small_car_speed_lane2)   * " + "db2inst1"  + ".ZERO(v.small_car_volume_lane2) ";
                selectCmd += "                                                                 , ";
                selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane2) + " + "db2inst1"  + ".ZERO(v.big_car_volume_lane2) + " + "db2inst1"  + ".ZERO(v.small_car_volume_lane2) ";
                selectCmd += "                                                                 , ";
                selectCmd += "                                                                  0 ";
                selectCmd += "                                                                ))  ";
                selectCmd += "        else 0 end) as lane2, ";
                selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(case when c.lane_count >= 3 then ";
                selectCmd += "                                          int(" + "db2inst1"  + ".DIV( " + "db2inst1"  + ".ZERO(v.connect_car_speed_lane3) * " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane3) + ";
                selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.big_car_speed_lane3)     * " + "db2inst1"  + ".ZERO(v.big_car_volume_lane3) + ";
                selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.small_car_speed_lane3)   * " + "db2inst1"  + ".ZERO(v.small_car_volume_lane3) ";
                selectCmd += "                                                                 , ";
                selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane3) + " + "db2inst1"  + ".ZERO(v.big_car_volume_lane3) + " + "db2inst1"  + ".ZERO(v.small_car_volume_lane3) ";
                selectCmd += "                                                                 , ";
                selectCmd += "                                                                  0 ";
                selectCmd += "                                                                ))  ";
                selectCmd += "        else 0 end) as lane3, ";
                selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(case when c.lane_count >= 4 then  ";
                selectCmd += "                                          int(" + "db2inst1"  + ".DIV( " + "db2inst1"  + ".ZERO(v.connect_car_speed_lane4) * " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane4) + ";
                selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.big_car_speed_lane4)     * " + "db2inst1"  + ".ZERO(v.big_car_volume_lane4) + ";
                selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.small_car_speed_lane4)   * " + "db2inst1"  + ".ZERO(v.small_car_volume_lane4) ";
                selectCmd += "                                                                 , ";
                selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane4) + " + "db2inst1"  + ".ZERO(v.big_car_volume_lane4) + " + "db2inst1"  + ".ZERO(v.small_car_volume_lane4) ";
                selectCmd += "                                                                 , ";
                selectCmd += "                                                                  0 ";
                selectCmd += "                                                                )) ";
                selectCmd += "        else 0 end) as lane4, ";
                selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(case when c.lane_count >= 5 then  ";
                selectCmd += "                                          int(" + "db2inst1"  + ".DIV( " + "db2inst1"  + ".ZERO(v.connect_car_speed_lane5) * " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane5) + ";
                selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.big_car_speed_lane5)     * " + "db2inst1"  + ".ZERO(v.big_car_volume_lane5) + ";
                selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.small_car_speed_lane5)   * " + "db2inst1"  + ".ZERO(v.small_car_volume_lane5) ";
                selectCmd += "                                                                 , ";
                selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane5) + " + "db2inst1"  + ".ZERO(v.big_car_volume_lane5) + " + "db2inst1"  + ".ZERO(v.small_car_volume_lane5) ";
                selectCmd += "                                                                 , ";
                selectCmd += "                                                                  0 ";
                selectCmd += "                                                                ))  ";
                selectCmd += "        else 0 end) as lane5, ";
                selectCmd += "       " + "db2inst1"  + ".ZERO2DASH(case when c.lane_count >= 6 then  ";
                selectCmd += "                                          int(" + "db2inst1"  + ".DIV( " + "db2inst1"  + ".ZERO(v.connect_car_speed_lane6) * " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane6) + ";
                selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.big_car_speed_lane6)     * " + "db2inst1"  + ".ZERO(v.big_car_volume_lane6) + ";
                selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.small_car_speed_lane6)   * " + "db2inst1"  + ".ZERO(v.small_car_volume_lane6) ";
                selectCmd += "                                                                 , ";
                selectCmd += "                                                                  " + "db2inst1"  + ".ZERO(v.connect_car_volume_lane6) + " + "db2inst1"  + ".ZERO(v.big_car_volume_lane6) + " + "db2inst1"  + ".ZERO(v.small_car_volume_lane6) ";
                selectCmd += "                                                                 , ";
                selectCmd += "                                                                  0 ";
                selectCmd += "                                                                 ))  ";
                selectCmd += "        else 0 end) as lane6 ";
                //2009/12/18:SHIN:edit...改抓五分鐘table
                //selectCmd += " From (Select * From " + "db2inst1"  + "." + sTableName_VD1Hr + " v Where v.Timestamp Between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) v, ";
                selectCmd += " From (Select * From " + "db2inst1"  + "." + sTableName_VD5Min + " v Where v.Timestamp Between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) v, ";

                selectCmd += "      " + "db2inst1"  + "." + sTableName_VdConfig + " c ";
                selectCmd += "Where v.Devicename = c.Devicename ";
                selectCmd += "  And v.Devicename in " + sList + " ";
                selectCmd += "Order By v.Devicename, v.Timestamp ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "小時交通平均速度統計報表vd"
        public DataTable Get_RPT_HourSpeedVd(string sList, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";

                selectCmd += "Select v.devicename ";
                selectCmd += " From (Select * From " + "db2inst1"  + "." + sTableName_VD5Min + " v Where v.Timestamp Between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "')) v, ";
                selectCmd += "      " + "db2inst1"  + "." + sTableName_VdConfig + " c ";
                selectCmd += "Where v.Devicename = c.Devicename ";
                selectCmd += "  And v.Devicename in " + sList + " ";
                selectCmd += " Group By v.Devicename ";
                selectCmd += " Order By v.Devicename ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion


        #region "VD記錄報表"
        public DataTable Get_RPT_4VD(string sList, DateTime sStartDate, DateTime sEndDate, string sMyColumns, string rbl)
        {
            try
            {
                string selectCmd = "";

                selectCmd += "Select " + sMyColumns + " ";
                if (rbl == "一天")
                {
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD1Day + " l ";
                }
                else if (rbl == "五分鐘")
                {
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD5Min + " l ";
                }
                else if (rbl == "一分鐘")
                {
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD1Min + " l ";
                }
                else if (rbl == "一小時")
                {
                    selectCmd += "  From " + "db2inst1"  + "." + sTableName_VD1Hr + " l ";
                }
                selectCmd += "  LEFT JOIN " + "db2inst1"  + "." + sTableName_VdConfig + " v ON l.DEVICENAME = v.DEVICENAME ";
                selectCmd += "  LEFT JOIN " + "db2inst1"  + "." + sTableName_DeviceConfig + " d ON l.DEVICENAME = d.DEVICENAME and d.device_type='VD' ";
                selectCmd += "  LEFT JOIN " + "db2inst1"  + "." + sTableName_tblgroupline + " g ON d.LineId = g.LineId ";
                selectCmd += "  LEFT JOIN " + "db2inst1"  + "." + sTableName_Parameter + " s ON s.GroupName='DeviceDirection' and d.Direction=s.VARIABLENAME ";
                selectCmd += "  LEFT JOIN " + "db2inst1"  + "." + sTableName_Parameter + " s1 ON s1.GroupName='Location' and d.Location=s1.VARIABLENAME ";
                selectCmd += "  LEFT JOIN " + "db2inst1"  + "." + sTableName_Parameter + " s2 ON s2.GroupName='VD_Type' and v.type=s2.VARIABLENAME ";
                selectCmd += " Where l.DeviceName in " + sList + " ";
               
                if (rbl == "一分鐘")
                {
                   selectCmd += " and VD5MIN_FLAG=1 and datavalidity = 'V' ";
                }

                selectCmd += "   And l.Timestamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "                     and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += " Order By l.DeviceName, l.TimeStamp ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }


        }
        #endregion

        #region "路段旅行時間記錄報表"
        public DataTable Get_RPT_TrafficDataLogSection(string Lineid, string direction, string start_D, string end_D, DateTime sStartDate, DateTime sEndDate)
        {
            try
            {
                string selectCmd = "";

                selectCmd += " select to_char(d.TimeStamp,'YYYY/MM/DD HH24:MI'),sum(d.TravelTime)/60 ";
                selectCmd += " from " + "db2inst1"  + "." + sTableName_tblTrafficDataLogSection + " d left join " + "db2inst1"  + "." + sTableName_GroupSection + " g on d.SectionId=g.SectionId ";
                selectCmd += "                                                                                                left join " + "db2inst1"  + "." + sTableName_GroupDivision + " gs on g.START_DIVISIONID = gs.DIVISIONID ";
                selectCmd += "  where g.lineid= '" + Lineid + "' and g.Direction='" + direction + "' and gs.Mileage between ( select  Mileage from TBLGROUPDIVISION where  DivisionId='" + start_D + "') ";
                selectCmd += " and ( select  Mileage from TBLGROUPDIVISION where  DivisionId='" + end_D + " ') ";
                selectCmd += " and d.TimeStamp between timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                selectCmd += "group by d.Timestamp ";

                InitDB();

                da = new DB2DataAdapter(selectCmd, conn);
                DataTable DT = new DataTable();
                da.Fill(DT);
                da.Dispose();
                conn.Close();
                return DT;
            }
            catch (Exception ex)
            {
                sErrMsg = ex.Message;
                CloseDB();
                return null;
            }
        }
        #endregion

        #region "取得所有路線名稱"
        public DataTable GetLineName()
        {
            string selectCmd = "";

            selectCmd += "select LineId,LineName ";
            selectCmd += "  From " + "db2inst1"  + "." + sTableName_tblgroupline + " ";
            selectCmd += " Order By LINEID ";


            InitDB();
            da = new DB2DataAdapter(selectCmd, conn);
            DataTable DT = new DataTable();
            da.Fill(DT);

            da.Dispose();
            CloseDB();
            return DT;
        }

        #endregion

        #region "取得東西向名稱"
        public DataTable GetDirectionEW()
        {
            string selectCmd = "";
            selectCmd += "Select  s.Direction as Direction1, p.VariableValue as Direction1Desc  ";
            selectCmd += "From db2inst1.tblGroupSection s  ";
            selectCmd += "LEFT JOIN (select VariableName, VariableValue from db2inst1.tblSysParameter where GroupName = 'DeviceDirection') p ON p.VariableName = s.Direction ";
            selectCmd += "where  s.Direction in " + "('E','W') ";
            selectCmd += "group by s.Direction,p.VariableValue ";
            selectCmd += "Order By  s.Direction ";

            InitDB();
            da = new DB2DataAdapter(selectCmd, conn);
            DataTable DT = new DataTable();
            da.Fill(DT);

            da.Dispose();
            CloseDB();
            return DT;

        }
        #endregion

        #region "取得南北向名稱"
        public DataTable GetDirectionNS()
        {
            string selectCmd = "";
            selectCmd += "Select  s.Direction as Direction1, p.VariableValue as Direction1Desc  ";
            selectCmd += "From db2inst1.tblGroupSection s  ";
            selectCmd += "LEFT JOIN (select VariableName, VariableValue from db2inst1.tblSysParameter where GroupName = 'DeviceDirection') p ON p.VariableName = s.Direction ";
            selectCmd += "where  s.Direction in " + "('N','S') ";
            selectCmd += "group by s.Direction,p.VariableValue ";
            selectCmd += "Order By  s.Direction ";

            InitDB();
            da = new DB2DataAdapter(selectCmd, conn);
            DataTable DT = new DataTable();
            da.Fill(DT);

            da.Dispose();
            CloseDB();
            return DT;

        }
        #endregion

        #region "取得交流道名稱"
        public DataTable GetSectionName(string SectionName)
        {
            string selectCmd = "";
            selectCmd += "select DivisionId,DivisionName   ";
            selectCmd += "from tblGroupDivision ";
            selectCmd += "where DivisionType in('C','I') and LINEID= '" + SectionName + "'";
            selectCmd += "ORDER BY DivisionType ";

            InitDB();
            da = new DB2DataAdapter(selectCmd, conn);
            DataTable DT = new DataTable();
            da.Fill(DT);

            da.Dispose();
            CloseDB();
            return DT;

        }
        #endregion
        #endregion

         #region "取得交流道名稱"
        public DataTable GetNewspaper(string NowTime)
        {
            string selectCmd = "";

            selectCmd += " select  g.LINEID,l.LINENAME,s.VARIABLEVALUE as Direction  ";
             selectCmd += " ,div1.DIVISIONNAME || '-'  ||  div2.DIVISIONNAME as from_location  ";
             selectCmd += " ,(case when t.CAR_SPEED=-1 then '-' else char(t.CAR_SPEED) end) as average_speed   ";
             selectCmd += " ,(case when t.CAR_SPEED >= 0 then  (case when  t.CAR_SPEED >= 80 then '順暢' when  t.CAR_SPEED < 80 and  t.CAR_SPEED >=40  then '車多'    ";
             selectCmd += " when t.CAR_SPEED < 40 then '壅塞'  end) else '-' end)  as congested  ";
             selectCmd += " from TBLTRAFFICDATALOGSECTION t   ";
             selectCmd += " left join db2inst1.tblGroupSection g on t.sectionid=g.sectionid   ";
             selectCmd += " left join db2inst1.tblGroupDivision div1 on g.START_DIVISIONID = div1.DIVISIONID   "; 
             selectCmd += " left join db2inst1.tblGroupDivision div2 on g.END_DIVISIONID = div2.DIVISIONID    "; 
             selectCmd += " left join db2inst1.tblGroupLine  l on g.LINEID = l.LINEID    ";
             selectCmd += "  left join db2inst1.tblSysParameter  s ON s.GroupName='DeviceDirection' and g.Direction=s.VARIABLENAME   ";
             selectCmd += " where   t.CAR_SPEED < 80  and t.CAR_SPEED >= 0 and g.LINEID not in('T72','T74','T76','T78') and  ";
             selectCmd += " Timestamp ='" + NowTime + ":00'    ";
             selectCmd += " order by g.LINEID,Timestamp,g.direction,div1.MILEAGE    ";
            InitDB();
            da = new DB2DataAdapter(selectCmd, conn);
            DataTable DT = new DataTable();
            da.Fill(DT);

            da.Dispose();
            CloseDB();
            return DT;

        }
        #endregion

                 #region "取得交流道名稱"
        public DataTable GetTRAFFICDATALOG(DateTime sStartDate, DateTime sEndDate)
        {
            string selectCmd = "";

            selectCmd += " select l.LINENAME,s.VARIABLEVALUE as Direction,to_char(TimeStamp,'YYYY-MM-DD HH24:MI')  ";
            selectCmd += " ,div1.DIVISIONNAME as from_location,div1.MILEAGE as from_milepost,  ";
            selectCmd += "  div2.DIVISIONNAME as end_location,div2.MILEAGE as end_milepost  ";
            selectCmd += " ,(case when t.CAR_SPEED=-1 then '-' else char(t.CAR_SPEED) end) as average_speed  ";
            selectCmd += " ,(case when t.car_volume=-1 then '-' else char(t.car_volume) end) as average_volume  ";
            selectCmd += " ,(case when t.CAR_SPEED >= 0 then  (case when  t.CAR_SPEED >= 80 then '順暢' when  t.CAR_SPEED < 80 and  t.CAR_SPEED >=40  then '車多'  when t.CAR_SPEED < 40 then '壅塞'  end) else '-' end)  ";
            selectCmd += " from TBLTRAFFICDATALOGSECTION t  ";
            selectCmd += " left join db2inst1.tblGroupSection g on t.sectionid=g.sectionid  ";
            selectCmd += " left join db2inst1.tblGroupDivision  div1 on g.START_DIVISIONID = div1.DIVISIONID  ";
            selectCmd += " left join db2inst1.tblGroupDivision  div2 on g.END_DIVISIONID = div2.DIVISIONID   ";
            selectCmd += " left join db2inst1.tblGroupLine l on g.LINEID = l.LINEID  ";
            selectCmd += " left join db2inst1.tblSysParameter  s ON s.GroupName='DeviceDirection' and g.Direction=s.VARIABLENAME  ";
            selectCmd += " where ";
            selectCmd += "    Timestamp = timestamp('" + sStartDate.ToString("yyyy-MM-dd HH:mm") + ":00') ";
            //selectCmd += "                       and timestamp('" + sEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
            selectCmd += "  order by Timestamp,g.LINEID,g.direction,div1.MILEAGE  ";

            InitDB();
            da = new DB2DataAdapter(selectCmd, conn);
            DataTable DT = new DataTable();
            da.Fill(DT);

            da.Dispose();
            CloseDB();
            return DT;

        }
        #endregion


      

        internal DataTable GetReport(string p, string p_2)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
