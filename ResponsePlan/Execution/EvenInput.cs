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
                    //ServerMeg.setServerMeg("�[�J�@���۰ʩΥb�۰ʨƥ��tblIIPEvent");
                    //if (InsertEvent != null)
                    //{
                    //    InsertEvent(cmd.RspID);
                    //}
                }
                catch (System.Exception ex)
                {
                    //���X�ҥ~�|�ɭP�Ѯv�}���ɱҰ�CMS�����p�e���~
                    //ServerMeg.setAlarmMeg("�[�J�@���۰ʩΥb�۰ʨƥ��tblIIPEvent���~!!");
                    //throw new Exception("�[�J�@���۰ʩΥb�۰ʨƥ��tblIIPEvent���~!!"  + "EventID=" + dr[18].ToString() + ",alarmclass" + dr[2].ToString() 
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
        /// �غc��
        /// </summary>
        /// <returns>�غc��</returns>
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
        /// �Ѯv��J(�b�۰�,�۰�)
        /// </summary>
        /// <param name="eventID">�ƥ�s��</param>
        /// <returns>�O�_��J���\</returns>
        public bool setAutoEvent(int eventID)
        {
            try
            {
                //ServerMeg.setServerMeg("��J�@���b�۰ʩΦ۰ʨƥ�");
                System.Collections.Generic.List<object> list = (System.Collections.Generic.List<object>)dbCmd.select(DataType.SysAlarmLog, GetSelectCmd.getLogCmd(eventID));
                if (list.Count == 0)
                {
                    throw new Exception("Not Found at IIPEvent " +  eventID);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                //ServerMeg.setAlarmMeg("�L���ƥ�s��!!");
                throw ex;
                //return false;
            }
        }


        #region "���o�̪񪺳]�Ʀ�m"
        private void GetNearDevice(string LineID, int Mile_M,ref int GpsX,ref int GpsY)
        {
            int StartMile = 0, EndMile = 0;
            int StartPoint = 30, EndPoint = 30;
            int GPS = 0;
            int GetType = 0;  //�P�_�_���I�Ӧp����
            System.Data.DataTable DT = null;

            #region "�����k"
            string Cmd = string.Format("(mile_M in (select min(mile_m) from tbldeviceconfig where lineid = '{0}' and MILE_M > {1} and location != 'L' and GPSX != 30 )  or " +
                                       " mile_M in (select max(mile_m) from tbldeviceconfig where lineid = '{0}' and MILE_M < {1} and location != 'L' and GPSX != 30 )) and " +
                                       " lineid = '{0}'  and location != 'L' and GPSX != 30 order by mile_M", LineID, Mile_M);

            DT = dbCmd.Select("Select MIle_M,GPSX,GPSY from tbldeviceconfig where " + Cmd);

            if (DT.Rows.Count < 2)  //�Y�W�U��䤣��ɡA�h��W��G��
            {
                Cmd = string.Format("Lineid='{0}'  and GPSX!=30 and location!='L' and Mile_M >{1} order by mile_M ", LineID, Mile_M);
                DT = dbCmd.Select("Select MIle_M,GPSX,GPSY from tbldeviceconfig where " + Cmd);
                if (DT.Rows.Count < 2)  //�Y�W��䤣��G��ɡA�h��U��G��
                {
                    Cmd = string.Format("Lineid='{0}'  and GPSX!=30 and location!='L' and Mile_M <{1} order by mile_M ", LineID, Mile_M);
                    DT = dbCmd.Select("Select MIle_M,GPSX,GPSY from tbldeviceconfig where " + Cmd);
                    GetType = 1;  //�˵ۧ�
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

        #region "�Ѥ����k���o�g�n�׮y��"
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

        #region ���o�h�D����VD�]��GPS�y��
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
        ///// �ϥΪ̿�J
        ///// </summary>
        ///// <param name="eventID">�ƥ�s��</param>
        //public bool setUserEvent(string eventID)
        //{
        //    try
        //    {
        //        //ServerMeg.setServerMeg("User��J�@����ʨƥ�");
        //        dbCmd.select(DataType.IIPEvent, GetSelectCmd.getIIPEventCmd(eventID, true));
        //        return true;
        //    }
        //    catch //(System.Exception ex)
        //    {
        //        //ServerMeg.setAlarmMeg("�L���ƥ�s��!!");
        //        //throw new Exception("�L���ƥ�s��!!");
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// ���ʬI�u��J
        ///// </summary>
        ///// <param name="id">�I�u�s��</param>
        ///// <param name="notifier">�q���̦W��</param>
        ///// <param name="timeStamp">�ƥ�ɶ�</param>
        ///// <param name="lineID">���u�s��(N1,T74...)</param>
        ///// <param name="directionID">��V�s��(N,S,W,E)</param>
        ///// <param name="startMileage">�I�u�}�l���{(���:����)</param>
        ///// <param name="endMileage">�I�u�������{(���:����)</param>
        ///// <param name="blockTypeId">���_���D���A</param>
        ///// <param name="blocklane">���_���D(���A��1�ϥ�)</param>
        ///// <returns>�ƥ�s��(-1����J���~)</returns>  
        //public int setMoveConstruction(int id,string notifier, DateTime timeStamp, string lineID, string directionID, int startMileage, int endMileage,string blockTypeId,string blocklane)
        //{
        //    try
        //    {
        //        //ServerMeg.setServerMeg("���ʬI�u��J�@���ƥ�");

        //        int result = (int)((System.Collections.Generic.List<object>)dbCmd.select(DataType.EventID, GetSelectCmd.getEventID()))[0];

        //        dbCmd.insert(GetInsertCmd.setMoveToIIPEvnetCmd(notifier, timeStamp, lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, result));
        //        setUserEvent(result.ToString());

        //        return result;
        //    }
        //    catch //(System.Exception ex)
        //    {
        //        //ServerMeg.setAlarmMeg("���ʬI�u��J�@���ƥ���~!!");
        //        return -1;
        //    }
        //}

        ///// <summary>
        ///// ��s���ʬI�u�ƥ�
        ///// </summary>
        ///// <param name="eventid">�ª��ƥ�s��</param>
        ///// <param name="timeStamp">�ƥ�ɶ�</param>
        ///// <param name="startMileage">�I�u�}�l���{(���:����)</param>
        ///// <param name="endMileage">�I�u�������{(���:����)</param>
        ///// <returns>�s���ƥ�s��(-1����J���~)</returns>
        //public int RenewEvent(int eventid, DateTime timeStamp, int startMileage, int endMileage)
        //{
        //    try
        //    {
        //        int status = (int)EventStatus.End;//�B�z���A
        //        //ServerMeg.setServerMeg("��s���ʬI�u�ƥ�");

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
        //        //ServerMeg.setAlarmMeg("��s���ʬI�u�ƥ���~!!");
        //        return -1;
        //    }
        //}

        ///// <summary>
        ///// ���o�Ҧ�(������)���ʬI�u�ƥ�
        ///// </summary>
        ///// <returns>���ʬI�u�ƥ���</returns>
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
        /// �I�u�ƥ󵲧�
        /// </summary>
        /// <param name="movingId">�I�u�s��</param>
        /// <returns>�O�_���\</returns>
        public bool closeEvent(int movingId)
        {
            try
            {
                dbCmd.update(GetUpdateCmd.setRSPExecutionCmd((int)EventStatus.End, movingId));                
                return true;
            }
            catch (Exception ex)
            {
                //ServerMeg.setServerMeg("���ʬI�u�ƥ󵲧����~!!");
                throw new Exception("���ʬI�u�ƥ󵲧����~!!" + ex.Message);
                //return false;
            }
        }
    }
}
