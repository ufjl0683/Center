using System;
using System.Collections.Generic;
using System.Text;
using DBConnect;

namespace Execution.Command
{
    internal class GetUpdateCmd
    {
        static string schema = ODBC_DB2Connect.schema;

        /// <summary>
        /// ����tbRSPExecution���B�z���A
        /// </summary>
        /// <param name="status">�B�z���A</param>
        /// <param name="rspID">RSP�s��</param>
        /// <returns></returns>
        public static ICommand setRSPExecutionCmd(int status,string rspID)
        {
            UpdateCommand com = new UpdateCommand();
            com.TblNames = string.Format(" {0}.{1} ", schema, DB2TableName.tblRspExecution);

            com.FiledNames = string.Format(" state = {0}", status);

            com.WhereCon = string.Format(" Exe_ID = '{0}' ", rspID);

            return com;
        }

        /// <summary>
        /// ����tbRSPExecution���B�z���A
        /// </summary>
        /// <param name="status">�B�z���A</param>
        /// <param name="rspID">�ƥ�s��</param>
        /// <returns></returns>
        public static ICommand setRSPExecutionCmd(int status, int eventid)
        {
            //UpdateCommand com = new UpdateCommand();
            //com.TblNames = string.Format(" {0}.{1} ", schema, tableName.tblRspExecution);

            //com.FiledNames = string.Format(" state = {0}", status);

            //com.WhereCon = string.Format(" EVENTID = {0} ", eventid);

            //return com;


            UpdateCommand com = new UpdateCommand();
            com.TblNames = string.Format(" {0}.{1} ", schema, DB2TableName.tblIIPEvent);

            com.FiledNames = string.Format(" INC_STATUS = {0}", status);

            com.WhereCon = string.Format(" MC_ID = {0} ", eventid);

            return com;
        }


        public static ICommand setRSPExecutionMessage1(string exe_id, string devicename, int mode, string device_type, int backgroud, int icon1, int icon2, string message)
        {
            UpdateCommand com = new UpdateCommand();

            com.TblNames = string.Format(" {0}.{1} ", schema, DB2TableName.tblRSPExecutionOutputDataMsg);

            com.FiledNames = string.Format(" DeviceName='{0}',Mode={1},outputdata1_background={2},outputdata1_icon1={3},outputdata1_icon2={4},outputdata1_msg='{5}' ", devicename, mode, backgroud, icon1, icon2, message);

            com.WhereCon = string.Format(" FLOWID = ( {0} ) ", GetSelectCmd.gettblRSPExecutionOutputdataFlowID(exe_id, devicename, device_type).getCommand());

            return com;
        }

        public static ICommand setRSPExecutionMessage2(string exe_id, string devicename, int mode, string device_type, int backgroud, int icon1, int icon2, string message)
        {
            UpdateCommand com = new UpdateCommand();

            com.TblNames = string.Format(" {0}.{1} ", schema, DB2TableName.tblRSPExecutionOutputDataMsg);

            com.FiledNames = string.Format(" DeviceName='{0}',Mode={1},outputdata2_background={2},outputdata1_icon2={3},outputdata2_icon2={4},outputdata2_msg='{5}' ", devicename, mode, backgroud, icon1, icon2, message);

            com.WhereCon = string.Format(" FLOWID = ( {0} ) ", GetSelectCmd.gettblRSPExecutionOutputdataFlowID(exe_id, devicename, device_type).getCommand());

            return com;
        }


        /// <summary>
        /// �ק�@����ƶi�JtblIIPService��db�R�O
        /// </summary>
        /// <param name="rspId">�����p���s��</param>
        /// <param name="subserviceId">�s�����s��</param>
        /// <param name="memo">�Ƶ�</param>
        /// <returns></returns>
        public static UpdateCommand setIIPServiceCmd(string rspId, int subserviceId, string memo, DateTime commTime)
        {
            UpdateCommand updateCmd = new UpdateCommand();
            updateCmd.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblIIPService);
            updateCmd.FiledNames += string.Format(" subserviceid={0}, memo = '{1}', comm_time = timestamp('{2}') ", subserviceId, memo, commTime.ToString("yyyy-MM-dd HH:mm:ss"));
            updateCmd.WhereCon += string.Format(" inc_id = '{0}' and subserviceid={1} ", rspId, subserviceId);
            return updateCmd;
        }


        /// <summary>
        /// �ק�@���s��ƶi�JtblRSPExecutionOutputData��db�R�O
        /// </summary>
        /// <param name="rspId">�����p���s��</param>
        /// <param name="dev">�]�ƫ��A</param>
        /// <param name="outputData">��ܤ��e</param>
        /// <returns></returns>
        public static UpdateCommand setRSPExecutionOutputDataCmd(int eventid, string devName, int priority, object outputData)
        {
            UpdateCommand updateCmd = new UpdateCommand();
            updateCmd.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblRSPExecutionOutputData);
            updateCmd.FiledNames += string.Format(" Priority = {0}, OutputData2 = '{1}' ", priority, RemoteInterface.Utils.Util.ObjToString(outputData));
            updateCmd.WhereCon += string.Format(" EventId = {0} and DeviceName='{1}' ", eventid, devName);
            return updateCmd;
        }
    }
}
