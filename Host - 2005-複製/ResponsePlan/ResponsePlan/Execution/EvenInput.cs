using System;
using DBConnect;
using Execution.Command;

namespace Execution
{
    class EvenInput 
    {
        private ODBC_DB2Connect dbCmd;
        //private static EvenInput evenInput = null;
        //public delegate void InsertEventHandler(string insertID);
        //public event InsertEventHandler InsertEvent;

        private EvenInput()
        {
            dbCmd = new ODBC_DB2Connect();
            //ServerMeg = ServerMessage.getBuilder();
            dbCmd.GetReaderData += new GetReaderDataHandler(dbCmd_GetReaderData);
        }

        object dbCmd_GetReaderData(DataType type, object reader)
        {
            if (type == DataType.SysAlarmLog)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                try
                {
                    InsertCommand cmd = GetInsertCmd.setIIPEventCmd(dr);
                    dbCmd.insert(cmd);
                    //ServerMeg.setServerMeg("加入一筆自動或半自動事件到tblIIPEvent");
                    //if (InsertEvent != null)
                    //{
                    //    InsertEvent(cmd.RspID);
                    //}
                }
                catch //(System.Exception ex)
                {
                    //ServerMeg.setAlarmMeg("加入一筆自動或半自動事件到tblIIPEvent錯誤!!");
                    throw new Exception("加入一筆自動或半自動事件到tblIIPEvent錯誤!!");
                }
            }
            else if (type == DataType.IIPEvent)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                //if (InsertEvent != null)
                //{
                //    InsertEvent(dr[0].ToString());
                //}
            }
            else if (type == DataType.EventID)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                return dr[0];
            }
            else if (type == DataType.Renew)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                System.Collections.Hashtable ht = new System.Collections.Hashtable();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (!ht.ContainsKey(dr.GetName(i))) ht.Add(dr.GetName(i), dr[i]);
                }
                return ht;
            }
            else if (type == DataType.AllMoveEvent)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                return new MoveData(Convert.ToInt32(dr[39]), dr[16].ToString(), DateTime.Parse(dr[13].ToString()), dr[6].ToString(), dr[7].ToString(), Convert.ToInt32(dr[10]), Convert.ToInt32(dr[11]), dr[25].ToString());
            }
            return null;
        }

        /// <summary>
        /// 建構者
        /// </summary>
        /// <returns>建構元</returns>
        public static EvenInput getBuilder()
        {
            //if (evenInput == null)
            //{
            //    lock (typeof(EvenInput))
            //    {
            //        if (evenInput == null)
            //            evenInput = new EvenInput();
            //    }
            //}
            //return evenInput;
            return new EvenInput();
        }

        /// <summary>
        /// 老師輸入(半自動,自動)
        /// </summary>
        /// <param name="eventID">事件編號</param>
        /// <returns>是否輸入成功</returns>
        public bool setAutoEvent(int eventID)
        {
            try
            {
                //ServerMeg.setServerMeg("輸入一筆半自動或自動事件");
                dbCmd.select(DataType.SysAlarmLog, GetSelectCmd.getLogCmd(eventID));
                return true;
            }
            catch (System.Exception ex)
            {
                //ServerMeg.setAlarmMeg("無此事件編號!!");
                throw ex;
                //return false;
            }
        }

        ///// <summary>
        ///// 使用者輸入
        ///// </summary>
        ///// <param name="eventID">事件編號</param>
        //public bool setUserEvent(string eventID)
        //{
        //    try
        //    {
        //        //ServerMeg.setServerMeg("User輸入一筆手動事件");
        //        dbCmd.select(DataType.IIPEvent, GetSelectCmd.getIIPEventCmd(eventID, true));
        //        return true;
        //    }
        //    catch //(System.Exception ex)
        //    {
        //        //ServerMeg.setAlarmMeg("無此事件編號!!");
        //        //throw new Exception("無此事件編號!!");
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// 移動施工輸入
        ///// </summary>
        ///// <param name="id">施工編號</param>
        ///// <param name="notifier">通報者名稱</param>
        ///// <param name="timeStamp">事件時間</param>
        ///// <param name="lineID">路線編號(N1,T74...)</param>
        ///// <param name="directionID">方向編號(N,S,W,E)</param>
        ///// <param name="startMileage">施工開始里程(單位:公尺)</param>
        ///// <param name="endMileage">施工結束里程(單位:公尺)</param>
        ///// <param name="blockTypeId">阻斷車道型態</param>
        ///// <param name="blocklane">阻斷車道(型態為1使用)</param>
        ///// <returns>事件編號(-1為輸入錯誤)</returns>  
        //public int setMoveConstruction(int id,string notifier, DateTime timeStamp, string lineID, string directionID, int startMileage, int endMileage,string blockTypeId,string blocklane)
        //{
        //    try
        //    {
        //        //ServerMeg.setServerMeg("移動施工輸入一筆事件");

        //        int result = (int)((System.Collections.Generic.List<object>)dbCmd.select(DataType.EventID, GetSelectCmd.getEventID()))[0];

        //        dbCmd.insert(GetInsertCmd.setMoveToIIPEvnetCmd(notifier, timeStamp, lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, result));
        //        setUserEvent(result.ToString());

        //        return result;
        //    }
        //    catch //(System.Exception ex)
        //    {
        //        //ServerMeg.setAlarmMeg("移動施工輸入一筆事件錯誤!!");
        //        return -1;
        //    }
        //}

        ///// <summary>
        ///// 更新移動施工事件
        ///// </summary>
        ///// <param name="eventid">舊的事件編號</param>
        ///// <param name="timeStamp">事件時間</param>
        ///// <param name="startMileage">施工開始里程(單位:公尺)</param>
        ///// <param name="endMileage">施工結束里程(單位:公尺)</param>
        ///// <returns>新的事件編號(-1為輸入錯誤)</returns>
        //public int RenewEvent(int eventid, DateTime timeStamp, int startMileage, int endMileage)
        //{
        //    try
        //    {
        //        int status = (int)EventStatus.End;//處理狀態
        //        //ServerMeg.setServerMeg("更新移動施工事件");

        //        int result = (int)((System.Collections.Generic.List<object>)dbCmd.select(DataType.EventID, GetSelectCmd.getEventID()))[0];

        //        System.Collections.Generic.List<object> list = (System.Collections.Generic.List<object>)dbCmd.select(DataType.Renew, GetSelectCmd.getIIPEventCmd(eventid));

        //        dbCmd.update(GetUpdateCmd.setRSPExecutionCmd(status, eventid));

        //        System.Collections.Hashtable ht = (System.Collections.Hashtable)list[0];

        //        dbCmd.insert(GetInsertCmd.setMoveRenewEventCmd(eventid, ht["INC_NOTIFY"].ToString(), timeStamp, ht["INC_LINEID"].ToString(), ht["INC_DIRECTION"].ToString(), startMileage, endMileage, ht["INC_BLOCKAGE"].ToString(), result));
        //        setUserEvent(result.ToString());

        //        return result;
        //    }
        //    catch //(System.Exception ex)
        //    {
        //        //ServerMeg.setAlarmMeg("更新移動施工事件錯誤!!");
        //        return -1;
        //    }
        //}

        ///// <summary>
        ///// 取得所有(未結束)移動施工事件
        ///// </summary>
        ///// <returns>移動施工事件資料</returns>
        //public MoveData[] getAllMoveEventID()
        //{
        //    System.Collections.Generic.List<object> list = (System.Collections.Generic.List<object>)dbCmd.select(DataType.AllMoveEvent, GetSelectCmd.getMoveEventCmd(31));
        //    MoveData[] datas = new MoveData[list.Count];

        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        datas[i] = (MoveData)list[i];
        //    }

        //    return datas;
        //}

        /// <summary>
        /// 施工事件結束
        /// </summary>
        /// <param name="movingId">施工編號</param>
        /// <returns>是否成功</returns>
        public bool closeEvent(int movingId)
        {
            try
            {
                dbCmd.update(GetUpdateCmd.setRSPExecutionCmd((int)EventStatus.End, movingId));                
                return true;
            }
            catch (Exception ex)
            {
                //ServerMeg.setServerMeg("移動施工事件結束錯誤!!");
                throw new Exception("移動施工事件結束錯誤!!" + ex.Message);
                //return false;
            }
        }
    }
}
