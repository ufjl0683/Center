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
                    int GpsX =0, GpsY=0;
                    try
                    {
                        if ((int)dr[2] == 49)
                        {
                            getInterchangePoint((string)dr[9], ((string)dr[10])[0].ToString(), (int)dr[7], ref GpsX, ref GpsY);
                        }
                        else
                        {
                            GetNearDevice((string)dr[9], (int)dr[7], ref GpsX, ref GpsY);
                        }
                    }
                    catch
                    {
                        ;
                    }

                    InsertCommand cmd = GetInsertCmd.setIIPEventCmd(dr,GpsX,GpsY);
                    dbCmd.insert(cmd);
                    //ServerMeg.setServerMeg("加入一筆自動或半自動事件到tblIIPEvent");
                    //if (InsertEvent != null)
                    //{
                    //    InsertEvent(cmd.RspID);
                    //}
                }
                catch (System.Exception ex)
                {
                    //扔出例外會導致老師開機時啟動CMS反應計畫錯誤
                    //ServerMeg.setAlarmMeg("加入一筆自動或半自動事件到tblIIPEvent錯誤!!");
                    //throw new Exception("加入一筆自動或半自動事件到tblIIPEvent錯誤!!"  + "EventID=" + dr[18].ToString() + ",alarmclass" + dr[2].ToString() 
                    //    + "\r\n" + ex.Message);
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
                System.Collections.Generic.List<object> list = (System.Collections.Generic.List<object>)dbCmd.select(DataType.SysAlarmLog, GetSelectCmd.getLogCmd(eventID));
                if (list.Count == 0)
                {
                    throw new Exception("Not Found at IIPEvent " +  eventID);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                //ServerMeg.setAlarmMeg("無此事件編號!!");
                throw ex;
                //return false;
            }
        }


        #region "取得最近的設備位置"
        private void GetNearDevice(string LineID, int Mile_M,ref int GpsX,ref int GpsY)
        {
            int StartMile = 0, EndMile = 0;
            int StartPoint = 30, EndPoint = 30;
            int GPS = 0;
            int GetType = 0;  //判斷起迄點該如何抓取
            System.Data.DataTable DT = null;

            #region "內插法"
            string Cmd = string.Format("(mile_M in (select min(mile_m) from tbldeviceconfig where lineid = '{0}' and MILE_M > {1} and location != 'L' and GPSX != 30 )  or " +
                                       " mile_M in (select max(mile_m) from tbldeviceconfig where lineid = '{0}' and MILE_M < {1} and location != 'L' and GPSX != 30 )) and " +
                                       " lineid = '{0}'  and location != 'L' and GPSX != 30 order by mile_M", LineID, Mile_M);

            DT = dbCmd.Select("Select MIle_M,GPSX,GPSY from tbldeviceconfig where " + Cmd);

            if (DT.Rows.Count < 2)  //若上下游找不到時，則找上游二支
            {
                Cmd = string.Format("Lineid='{0}'  and GPSX!=30 and location!='L' and Mile_M >{1} order by mile_M ", LineID, Mile_M);
                DT = dbCmd.Select("Select MIle_M,GPSX,GPSY from tbldeviceconfig where " + Cmd);
                if (DT.Rows.Count < 2)  //若上游找不到二支時，則找下游二支
                {
                    Cmd = string.Format("Lineid='{0}'  and GPSX!=30 and location!='L' and Mile_M <{1} order by mile_M ", LineID, Mile_M);
                    DT = dbCmd.Select("Select MIle_M,GPSX,GPSY from tbldeviceconfig where " + Cmd);
                    GetType = 1;  //倒著抓
                }
            }

            int k = 0;

            if (DT.Rows.Count >= 2)
            {
                switch (GetType)
                {
                    case 0:
                        StartMile = Convert.ToInt32(DT.Rows[0][0].ToString());
                        do
                        {
                            EndMile = Convert.ToInt32(DT.Rows[k + 1][0].ToString());
                            k++;
                        } while (StartMile == EndMile && k < DT.Rows.Count);

                        break;
                    case 1:                        
                        EndMile = Convert.ToInt32(DT.Rows[DT.Rows.Count - 1][0].ToString());
                        do
                        {
                            StartMile = Convert.ToInt32(DT.Rows[DT.Rows.Count - (k + 2)][0].ToString());
                            k++;
                        } while (StartMile == EndMile && k < DT.Rows.Count);
                        break;
                }
                
                switch (GetType)
                {
                    case 0:
                        StartPoint = Convert.ToInt32(DT.Rows[0][1].ToString());
                        EndPoint = Convert.ToInt32(DT.Rows[k][1].ToString());
                        break;
                    case 1:
                        StartPoint = Convert.ToInt32(DT.Rows[DT.Rows.Count - (k + 1)][1].ToString());
                        EndPoint = Convert.ToInt32(DT.Rows[DT.Rows.Count - 1][1].ToString());
                        break;
                }

                if ((StartPoint != 30) || (EndPoint != 30))
                {
                    GpsX = InterPol(StartMile, EndMile, StartPoint, EndPoint, Mile_M);
                }

                switch (GetType)
                {
                    case 0:
                        StartPoint = Convert.ToInt32(DT.Rows[0][2].ToString());
                        EndPoint = Convert.ToInt32(DT.Rows[k][2].ToString());
                        break;
                    case 1:
                        StartPoint = Convert.ToInt32(DT.Rows[DT.Rows.Count - (k +1)][2].ToString());
                        EndPoint = Convert.ToInt32(DT.Rows[DT.Rows.Count - 1][2].ToString());
                        break;
                }                

                if ((StartPoint != 30) || (EndPoint != 30))
                {
                    GpsY = InterPol(StartMile, EndMile, StartPoint, EndPoint, Mile_M);
                }
            }
            #endregion

        }
        #endregion

        #region "由內插法取得經緯度座標"
        private int InterPol(int Mile1, int Mile2, int Point1, int Point2, int LocatMile)
        {
            int LocatPoint = 0;

            try
            {
                if ((Mile2 - Mile1) > 0)
                {
                    LocatPoint = Point2 - ((Point2 - Point1) * (Mile2 - LocatMile)) / (Mile2 - Mile1);
                }
                else { LocatPoint = 0; }
            }
            catch (Exception)
            { 
                ;
            }

            return LocatPoint;
        }
        #endregion

        #region 取得閘道儀控VD設備GPS座標
        private void getInterchangePoint(string LineID,string direction,int Mile,ref int GPSX,ref int GPSY)
        {
            try
            {
                System.Data.DataRow DivisionDR = null;
                foreach (System.Data.DataRow dr in RSPGlobal.GetDivisionDT().Rows)
                {
                    if ((string)dr[3] == LineID)
                    {
                        if ((string)dr[1] == "I" || (string)dr[1] == "C")
                        {
                            int Distance = Mile - (int)dr[2];
                            if (Distance <= 500 && Distance >= -500)
                            {
                                DivisionDR = dr;
                            }
                        }
                    }
                }                

                if (DivisionDR != null)
                {
                    string cmd = string.Format("Select GPSX,GPSY from {0}.{1} where LineID = '{2}' and Device_Type = 'VD' and Location = 'R' and Location_R = 'I' "
                        + "and direction = '{3}' and mile_m >= {4} and mile_m <= {5};", RSPGlobal.GlobaSchema, DB2TableName.tblDeviceConfig, LineID, direction, Mile - 500, Mile + 500);
                    System.Data.DataTable DT = dbCmd.Select(cmd);

                    if (DT.Rows.Count != 0)
                    {
                        if (DT.Rows[0][0].ToString() != "30")
                        {
                            GPSX = (int)DT.Rows[0][0];
                            GPSY = (int)DT.Rows[0][1];
                        }
                        else
                        {
                            GPSX = 30;
                            GPSY = 30;
                        }
                    }
                    else
                    {
                        GPSX = 30;
                        GPSY = 30;
                    }
                }
            }
            catch (Exception ex)
            { 
                ;
            }
        }
        #endregion
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
