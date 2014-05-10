using System;
using System.Collections.Generic;
using System.Text;
using DBConnect;

namespace Execution.Command
{
    internal class GetInsertCmd
    {
        static string schema = ODBC_DB2Connect.schema;

        #region ==== 取得自動、半自動寫入tblIIPEvent表的db命令 ====
        /// <summary>
        /// 取得自動、半自動寫入tblIIPEvent表的欄位
        /// </summary>
        /// <param name="insertCmd">InsertCommand</param>
        /// <returns>InsertCommand</returns>
        private static InsertCommand setTbl_FiledName(InsertCommand insertCmd)
        {
            string str = "";
            str += string.Format(" {0}.{1} ", schema, DB2TableName.tblIIPEvent);
            insertCmd.TblNames = str;


            str = "";
            str += " INC_ID,INC_TYPE_NAME,INC_NAME,INC_CONGESTION,";//反應計劃編號,事件類別,次類別,程度
            str += " INC_LINEID,INC_DIRECTION,INC_LOCATION,FROM_MILEPOST1,TO_MILEPOST1,";//路線,方向,路段,發生里程,結束里程
            str += " INC_LOGIN_MODE,INC_TIME,INC_NOTIFY_TIME,INC_NOTIFY_MODE,INC_NOTIFY_PLANT,";//登入模式,發生時間,通報時間,通報來源,通報設備
            str += " INC_STATUS,EVENTID,INC_BLOCKAGE,INC_MEMO,INC_INTERCHANGE,";//處理狀態,事件編號,阻斷車道,備註,交流道位置            
            str += " originaleventid,MC_ID,Inc_notify,BlockTypeid,MC_Memo,GpsX,GpsY,Execute ";
            //str += " INC_SPREADNEWS,";//是否簡訊發送
            //str += " ROADNET_TURNTO,";//路網轉向
            //str += " INC_STEP_TIMES,INC_STEPNO,";//事件階段產生時間,處理階段
            //str += " END_NOTIFY,END_NOTIFY_MODE,DELAY_TIME,";//結束事件通報者,結束事件通報來源,處理延時時間            
            insertCmd.FiledNames = str.TrimEnd(',') + " ";

            return insertCmd;
        }

        /// <summary>
        /// 取得自動、半自動寫入tblIIPEvent表的db命令
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static InsertCommand setIIPEventCmd(System.Data.Odbc.OdbcDataReader dr, int GpsX, int GpsY)
        {
            int status = (int)EventStatus.Enter;//處理狀態
            int notify = ((int)dr[2]) == 31 ? 8 : (int)Notifier.Device;//通報來源
            int mode = (int)LoginMode.Half;//登入模式
            InsertCommand insertCmd = new InsertCommand();

            insertCmd = setTbl_FiledName(insertCmd);

            insertCmd.RspID = dr[0].ToString();
            
            string str = "";
            str += string.Format("'{0}','{1}',{2},{3},", insertCmd.RspID, dr[1].ToString(), dr[2].ToString(), dr[3].ToString());//反應計劃編號,事件類別,次類別,嚴重程度
            str += string.Format("'{0}','{1}','{2}',{3},{4},", dr[9].ToString(), dr[10].ToString(), dr[11].ToString() == "0" ? "" : dr[11].ToString(), dr[6].ToString(), dr[7].ToString());//路線,方向,路段,發生里程,結束里程

            str += string.Format("{0},timestamp('{1}'),timestamp('{2}'),{3},'{4}',", dr[28], DateTime.Parse(dr[4].ToString()).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Parse(dr[4].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),  notify, dr[8].ToString());//登入模式,發生時間,通報時間,通報來源,通報設備
            str += string.Format("{0},{1},", status, dr[18]);//處理狀態,事件編號
            str += string.Format("'{0}',", dr[25].ToString());//阻斷車道數
            str += string.Format("'{0}',", dr[19].ToString());//備註
            str += string.Format("'{0}',", "");//交流道位置 
            if (string.IsNullOrEmpty(dr[21].ToString()))
                str += string.Format("{0}", dr[18].ToString());//舊事件編號
            else
                str += string.Format("{0}", dr[21].ToString());//舊事件編號

            str += string.Format(",{0},", dr[22].ToString() == "" ? "-1" : dr[22].ToString());//施工編號 
            str += string.Format("'{0}',", dr[23].ToString());//通報者 
            str += string.Format("'{0}',", dr[24].ToString());//阻斷車道型態
            str += string.Format("'{0}',{1},{2},'{3}'", dr[26].ToString(),GpsX,GpsY,dr[27]);//施工訊息 
            insertCmd.WhereCon = str;

            return insertCmd;
        }

        /// <summary>
        /// 車道編號轉換阻斷車道數
        /// </summary>
        /// <param name="lane">阻斷車道</param>
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
        /// 產生反應計劃編號
        /// </summary>
        /// <returns>反應計劃編號</returns>
        private static string getEventID()
        {
            return string.Format("H.{0}", DateTime.Now.ToString("yyyyMMddHHmmss.ffffff"));
        }
        #endregion ==== 取得自動、半自動寫入tblIIPEvent表的db命令 ====

        /// <summary>
        /// 寫入一筆新資料進入RSPExecution表的db命令
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static InsertCommand setRSPExecutionCmd(ExecutionObj obj)
        {        
            int status = (int)EventStatus.Enter;//處理狀態
            InsertCommand insertCmd = new InsertCommand();
            string str = "";
            str += string.Format(" {0}.{1} ", schema, DB2TableName.tblRspExecution);
            insertCmd.TblNames = str;


            insertCmd.WhereCon += string.Format(" '{0}',{1},", obj.RspID, obj.EventID);                                 //反應計劃編號,事件編號
            insertCmd.WhereCon += string.Format(" timestamp('{0}'),", obj.EventTime.ToString("yyyy-MM-dd HH:mm:ss"));   //事件發生時間
            insertCmd.WhereCon += string.Format(" {0},'{1}',", status, obj.Memo);                                       //處理狀態,備註
            insertCmd.WhereCon = insertCmd.WhereCon.TrimEnd(',');
            return insertCmd;
        }

        /// <summary>
        /// 寫入一筆新資料進入tblIIPService表的db命令
        /// </summary>
        /// <param name="rspId">反應計劃編號</param>
        /// <param name="subserviceId">連絡單位編號</param>
        /// <param name="memo">備註</param>
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
        /// 寫入一筆新資料進入tblRSPExecutionOutputData表的db命令
        /// </summary>
        /// <param name="rspId">反應計劃編號</param>
        /// <param name="dev">設備型態</param>
        /// <param name="outputData">顯示內容</param>
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
        /// 寫入一筆移動施工資料進入tblIIPEvent表的db命令
        /// </summary>
        /// <param name="notifier">通報者名稱</param>
        /// <param name="timeStamp">事件時間</param>
        /// <param name="lineID">路線編號(N1,T74...)</param>
        /// <param name="directionID">方向編號(N,S,W,E)</param>
        /// <param name="startMileage">施工開始里程(單位:公尺)</param>
        /// <param name="endMileage">施工結束里程(單位:公尺)</param>
        /// <param name="blocklane">阻斷車道(如第一、第二車道和路肩阻斷為:blocklane="12X")</param>
        /// <param name="eventID">事件編號</param>
        /// <returns>InsertCommand</returns>
        public static InsertCommand setMoveToIIPEvnetCmd(string notifier, DateTime timeStamp, string lineID, string directionID, int startMileage, int endMileage,string blockTypeId,string blocklane,int eventID)
        {
            int status = (int)EventStatus.Enter;//處理狀態
            int notify = (int)Notifier.Device;//通報來源
            int mode = (int)LoginMode.Half;//登入模式

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
        /// 更新移動施工事件
        /// </summary>
        /// <param name="eventid">舊的事件編號</param>
        /// <param name="timeStamp">事件時間</param>
        /// <param name="startMileage">施工開始里程(單位:公尺)</param>
        /// <param name="endMileage">施工結束里程(單位:公尺)</param>
        /// <returns>新的事件編號(-1為輸入錯誤)</returns>
        public static InsertCommand setMoveRenewEventCmd(int oldEventId, string notifier, DateTime timeStamp, string lineID, string directionID, int startMileage, int endMileage, string blocklane, int newEventId)
        {
            int status = (int)EventStatus.Enter;//處理狀態
            int notify = (int)Notifier.Device;//通報來源
            int mode = (int)LoginMode.Half;//登入模式

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
