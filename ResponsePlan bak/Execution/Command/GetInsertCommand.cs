using System;
using System.Collections.Generic;
using System.Text;
using DBConnect;

namespace Execution.Command
{
    internal class GetInsertCmd
    {
        static string schema = ODBC_DB2Connect.schema;

        #region ==== ���o�۰ʡB�b�۰ʼg�JtblIIPEvent��db�R�O ====
        /// <summary>
        /// ���o�۰ʡB�b�۰ʼg�JtblIIPEvent�����
        /// </summary>
        /// <param name="insertCmd">InsertCommand</param>
        /// <returns>InsertCommand</returns>
        private static InsertCommand setTbl_FiledName(InsertCommand insertCmd)
        {
            string str = "";
            str += string.Format(" {0}.{1} ", schema, DB2TableName.tblIIPEvent);
            insertCmd.TblNames = str;


            str = "";
            str += " INC_ID,INC_TYPE_NAME,INC_NAME,INC_CONGESTION,";//�����p���s��,�ƥ����O,�����O,�{��
            str += " INC_LINEID,INC_DIRECTION,INC_LOCATION,FROM_MILEPOST1,TO_MILEPOST1,";//���u,��V,���q,�o�ͨ��{,�������{
            str += " INC_LOGIN_MODE,INC_TIME,INC_NOTIFY_TIME,INC_NOTIFY_MODE,INC_NOTIFY_PLANT,";//�n�J�Ҧ�,�o�ͮɶ�,�q���ɶ�,�q���ӷ�,�q���]��
            str += " INC_STATUS,EVENTID,INC_BLOCKAGE,INC_MEMO,INC_INTERCHANGE,";//�B�z���A,�ƥ�s��,���_���D,�Ƶ�,��y�D��m            
            str += " originaleventid,MC_ID,Inc_notify,BlockTypeid,MC_Memo,GpsX,GpsY,Execute ";
            //str += " INC_SPREADNEWS,";//�O�_²�T�o�e
            //str += " ROADNET_TURNTO,";//������V
            //str += " INC_STEP_TIMES,INC_STEPNO,";//�ƥ󶥬q���ͮɶ�,�B�z���q
            //str += " END_NOTIFY,END_NOTIFY_MODE,DELAY_TIME,";//�����ƥ�q����,�����ƥ�q���ӷ�,�B�z���ɮɶ�            
            insertCmd.FiledNames = str.TrimEnd(',') + " ";

            return insertCmd;
        }

        /// <summary>
        /// ���o�۰ʡB�b�۰ʼg�JtblIIPEvent��db�R�O
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static InsertCommand setIIPEventCmd(System.Data.Odbc.OdbcDataReader dr, int GpsX, int GpsY)
        {
            int status = (int)EventStatus.Enter;//�B�z���A
            int notify = ((int)dr[2]) == 31 ? 8 : (int)Notifier.Device;//�q���ӷ�
            int mode = (int)LoginMode.Half;//�n�J�Ҧ�
            InsertCommand insertCmd = new InsertCommand();

            insertCmd = setTbl_FiledName(insertCmd);

            insertCmd.RspID = dr[0].ToString();
            
            string str = "";
            str += string.Format("'{0}','{1}',{2},{3},", insertCmd.RspID, dr[1].ToString(), dr[2].ToString(), dr[3].ToString());//�����p���s��,�ƥ����O,�����O,�Y���{��
            str += string.Format("'{0}','{1}','{2}',{3},{4},", dr[9].ToString(), dr[10].ToString(), dr[11].ToString() == "0" ? "" : dr[11].ToString(), dr[6].ToString(), dr[7].ToString());//���u,��V,���q,�o�ͨ��{,�������{

            str += string.Format("{0},timestamp('{1}'),timestamp('{2}'),{3},'{4}',", dr[28], DateTime.Parse(dr[4].ToString()).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Parse(dr[4].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),  notify, dr[8].ToString());//�n�J�Ҧ�,�o�ͮɶ�,�q���ɶ�,�q���ӷ�,�q���]��
            str += string.Format("{0},{1},", status, dr[18]);//�B�z���A,�ƥ�s��
            str += string.Format("'{0}',", dr[25].ToString());//���_���D��
            str += string.Format("'{0}',", dr[19].ToString());//�Ƶ�
            str += string.Format("'{0}',", "");//��y�D��m 
            if (string.IsNullOrEmpty(dr[21].ToString()))
                str += string.Format("{0}", dr[18].ToString());//�¨ƥ�s��
            else
                str += string.Format("{0}", dr[21].ToString());//�¨ƥ�s��

            str += string.Format(",{0},", dr[22].ToString() == "" ? "-1" : dr[22].ToString());//�I�u�s�� 
            str += string.Format("'{0}',", dr[23].ToString());//�q���� 
            str += string.Format("'{0}',", dr[24].ToString());//���_���D���A
            str += string.Format("'{0}',{1},{2},'{3}'", dr[26].ToString(),GpsX,GpsY,dr[27]);//�I�u�T�� 
            insertCmd.WhereCon = str;

            return insertCmd;
        }

        /// <summary>
        /// ���D�s���ഫ���_���D��
        /// </summary>
        /// <param name="lane">���_���D</param>
        /// <returns></returns>
        private static string getBlockage(string lane)
        {
            byte Lane=0;
            if (lane == "") return "";
            else Lane = Convert.ToByte(lane);
            string result = "";
            for (byte i = 0; i < 8; i++)
            {
                if (8-i == Lane)
                    result += "1";
                else
                    result += "0";
            }
            return result;
        }

        /// <summary>
        /// ���ͤ����p���s��
        /// </summary>
        /// <returns>�����p���s��</returns>
        private static string getEventID()
        {
            return string.Format("H.{0}", DateTime.Now.ToString("yyyyMMddHHmmss.ffffff"));
        }
        #endregion ==== ���o�۰ʡB�b�۰ʼg�JtblIIPEvent��db�R�O ====

        /// <summary>
        /// �g�J�@���s��ƶi�JRSPExecution��db�R�O
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static InsertCommand setRSPExecutionCmd(ExecutionObj obj)
        {        
            int status = (int)EventStatus.Enter;//�B�z���A
            InsertCommand insertCmd = new InsertCommand();
            string str = "";
            str += string.Format(" {0}.{1} ", schema, DB2TableName.tblRspExecution);
            insertCmd.TblNames = str;


            insertCmd.WhereCon += string.Format(" '{0}',{1},", obj.RspID, obj.EventID);                                 //�����p���s��,�ƥ�s��
            insertCmd.WhereCon += string.Format(" timestamp('{0}'),", obj.EventTime.ToString("yyyy-MM-dd HH:mm:ss"));   //�ƥ�o�ͮɶ�
            insertCmd.WhereCon += string.Format(" {0},'{1}',", status, obj.Memo);                                       //�B�z���A,�Ƶ�
            insertCmd.WhereCon = insertCmd.WhereCon.TrimEnd(',');
            return insertCmd;
        }

        /// <summary>
        /// �g�J�@���s��ƶi�JtblIIPService��db�R�O
        /// </summary>
        /// <param name="rspId">�����p���s��</param>
        /// <param name="subserviceId">�s�����s��</param>
        /// <param name="memo">�Ƶ�</param>
        /// <returns></returns>
        public static InsertCommand setIIPServiceCmd(string rspId,int subserviceId,string memo)
        {
            InsertCommand insertCmd = new InsertCommand();
            insertCmd.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblIIPService);
            insertCmd.FiledNames += string.Format(" inc_id,subserviceid,memo,comm_time ");
            insertCmd.WhereCon += string.Format(" '{0}',{1},'{2}',timestamp('{3}') ", rspId, subserviceId, memo,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return insertCmd;
        }

        /// <summary>
        /// �g�J�@���s��ƶi�JtblRSPExecutionOutputData��db�R�O
        /// </summary>
        /// <param name="rspId">�����p���s��</param>
        /// <param name="dev">�]�ƫ��A</param>
        /// <param name="outputData">��ܤ��e</param>
        /// <returns></returns>
        public static InsertCommand setRSPExecutionOutputDataCmd(string rspId,int eventid, string devName,DeviceType dev, object outputData,bool isUserChange)
        {
            InsertCommand insertCmd = new InsertCommand();
            insertCmd.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblRSPExecutionOutputData);
            if (isUserChange)
                insertCmd.FiledNames += string.Format(" Exe_Id,DEVICENAME,Device_Type,OutputData2,priority,eventid  ");
            else
                insertCmd.FiledNames += string.Format(" Exe_Id,DEVICENAME,Device_Type,OutputData1,priority,eventid ");
            if (dev == DeviceType.CCTV)
            {
                insertCmd.WhereCon += string.Format(" '{0}','{1}','{2}',null,0,{3} ", rspId, devName, dev.ToString(), eventid);
            }
            else if (dev == DeviceType.CMS || dev == DeviceType.LCS || dev == DeviceType.CSLS || dev == DeviceType.RMS || dev == DeviceType.RGS)
            {
                List<object> outputs = (List<object>)outputData;
                insertCmd.WhereCon += string.Format(" '{0}','{1}','{2}','{3}',{4},{5} ", rspId, devName, dev.ToString(), RemoteInterface.Utils.Util.ObjToString(outputs[1]), (int)outputs[0], eventid);
            }
            else
                insertCmd.WhereCon += string.Format(" '{0}','{1}','{2}','{3}',{4},{5} ", rspId, devName, dev.ToString(), RemoteInterface.Utils.Util.ObjToString(outputData), 0, eventid);

            return insertCmd;
        }

        /// <summary>
        /// �g�J�@�����ʬI�u��ƶi�JtblIIPEvent��db�R�O
        /// </summary>
        /// <param name="notifier">�q���̦W��</param>
        /// <param name="timeStamp">�ƥ�ɶ�</param>
        /// <param name="lineID">���u�s��(N1,T74...)</param>
        /// <param name="directionID">��V�s��(N,S,W,E)</param>
        /// <param name="startMileage">�I�u�}�l���{(���:����)</param>
        /// <param name="endMileage">�I�u�������{(���:����)</param>
        /// <param name="blocklane">���_���D(�p�Ĥ@�B�ĤG���D�M���Ӫ��_��:blocklane="12X")</param>
        /// <param name="eventID">�ƥ�s��</param>
        /// <returns>InsertCommand</returns>
        public static InsertCommand setMoveToIIPEvnetCmd(string notifier, DateTime timeStamp, string lineID, string directionID, int startMileage, int endMileage,string blockTypeId,string blocklane,int eventID)
        {
            int status = (int)EventStatus.Enter;//�B�z���A
            int notify = (int)Notifier.Device;//�q���ӷ�
            int mode = (int)LoginMode.Half;//�n�J�Ҧ�

            InsertCommand insertCmd = new InsertCommand();
            insertCmd.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblIIPEvent);

            insertCmd.FiledNames += "INC_ID,INC_TYPE_NAME,INC_NAME,INC_CONGESTION,";
            insertCmd.FiledNames += "INC_LINEID,INC_DIRECTION,FROM_MILEPOST1,";
            insertCmd.FiledNames += "TO_MILEPOST1,INC_LOGIN_MODE,INC_TIME,INC_NOTIFY_TIME,INC_NOTIFY,";
            insertCmd.FiledNames += "INC_NOTIFY_MODE,INC_NOTIFY_NAME,INC_STATUS,INC_SPREADNEWS,EVENTID,INC_BLOCKAGE,";
            insertCmd.FiledNames += "OriginalEventID,BlockTypeId,INC_MEMO ";
            
            insertCmd.WhereCon += string.Format(" '{0}','{1}',{2},{3}, ", getEventID(), "GEN", 31, 0);
            insertCmd.WhereCon += string.Format(" '{0}','{1}',{2}, ", lineID, directionID, startMileage);
            insertCmd.WhereCon += string.Format(" {0},{1},'{2}','{2}','{3}',", endMileage, mode, timeStamp.ToString("yyyy-MM-dd HH:mm:ss"), notifier);

            insertCmd.WhereCon += string.Format(" {0},'{1}',{2},'{3}',{4},'{5}', ", notify, notifier, status, "Y", eventID, blocklane);
            insertCmd.WhereCon += string.Format(" {0},'{1}','RSP TEST' ", eventID, blockTypeId);
            return insertCmd;
        } 
       
        /// <summary>
        /// ��s���ʬI�u�ƥ�
        /// </summary>
        /// <param name="eventid">�ª��ƥ�s��</param>
        /// <param name="timeStamp">�ƥ�ɶ�</param>
        /// <param name="startMileage">�I�u�}�l���{(���:����)</param>
        /// <param name="endMileage">�I�u�������{(���:����)</param>
        /// <returns>�s���ƥ�s��(-1����J���~)</returns>
        public static InsertCommand setMoveRenewEventCmd(int oldEventId, string notifier, DateTime timeStamp, string lineID, string directionID, int startMileage, int endMileage, string blocklane, int newEventId)
        {
            int status = (int)EventStatus.Enter;//�B�z���A
            int notify = (int)Notifier.Device;//�q���ӷ�
            int mode = (int)LoginMode.Half;//�n�J�Ҧ�

            InsertCommand insertCmd = new InsertCommand();
            insertCmd.TblNames += string.Format(" {0}.{1} ", schema, DB2TableName.tblIIPEvent);

            insertCmd.FiledNames += "INC_ID,INC_TYPE_NAME,INC_NAME,INC_CONGESTION,";
            insertCmd.FiledNames += "INC_LINEID,INC_DIRECTION,FROM_MILEPOST1,";
            insertCmd.FiledNames += "TO_MILEPOST1,INC_LOGIN_MODE,INC_TIME,INC_NOTIFY_TIME,INC_NOTIFY,";
            insertCmd.FiledNames += "INC_NOTIFY_MODE,INC_NOTIFY_NAME,INC_STATUS,INC_SPREADNEWS,EVENTID,INC_BLOCKAGE,";
            insertCmd.FiledNames += "OriginalEventID ";

            insertCmd.WhereCon += string.Format(" '{0}','{1}',{2},{3}, ", getEventID(), "GEN", 31, 0);
            insertCmd.WhereCon += string.Format(" '{0}','{1}',{2}, ", lineID, directionID, startMileage);
            insertCmd.WhereCon += string.Format(" {0},{1},'{2}','{2}','{3}',", endMileage, mode, timeStamp.ToString("yyyy-MM-dd HH:mm:ss"), notifier);

            insertCmd.WhereCon += string.Format(" {0},'{1}',{2},'{3}',{4},'{5}', ", notify, notifier, status, "Y", newEventId, blocklane);
            insertCmd.WhereCon += string.Format(" {0} ", oldEventId);
            return insertCmd;
        }
    }
}
