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
                    //ServerMeg.setServerMeg("�[�J�@���۰ʩΥb�۰ʨƥ��tblIIPEvent");
                    //if (InsertEvent != null)
                    //{
                    //    InsertEvent(cmd.RspID);
                    //}
                }
                catch //(System.Exception ex)
                {
                    //ServerMeg.setAlarmMeg("�[�J�@���۰ʩΥb�۰ʨƥ��tblIIPEvent���~!!");
                    throw new Exception("�[�J�@���۰ʩΥb�۰ʨƥ��tblIIPEvent���~!!");
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
                dbCmd.select(DataType.SysAlarmLog, GetSelectCmd.getLogCmd(eventID));
                return true;
            }
            catch (System.Exception ex)
            {
                //ServerMeg.setAlarmMeg("�L���ƥ�s��!!");
                throw ex;
                //return false;
            }
        }

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
