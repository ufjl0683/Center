using DBConnect;
using Execution.Command;
using Execution.Category;

namespace Execution
{
    public class Execution 
    {
        //private static System.Collections.Generic.List<string> runThread;   //���b���檺������W�ٶ��X
        //private static Execution execution;                                 //�W��غc���� 
        private EvenInput input = null;                                     //��J�ƥ󪫥�
        private RSPDataTbl rspData;                                         //�����p�����
        private ODBC_DB2Connect dbcmd;                                            //��Ʈw����          
        private TunnelData tunnelData;

        //public delegate void ConfirmEventHandler(LoginMode type, string sender);
        //public delegate void NotConfirmEventHandler(System.Collections.Generic.List<string> rspIDs);
        ///// <summary>
        ///// �����p�����͵��ϥΪ�
        ///// </summary>
        //public event ConfirmEventHandler ConfirmEvent;
        #region ==== �غc�� ====
        /// <summary>
        /// �غc��
        /// </summary>
        public Execution()
        {
            try
            {
                tunnelData = TunnelData.getBuilder();
                //ServerMeg = ServerMessage.getBuilder();
                input = EvenInput.getBuilder();
                rspData = RSPDataTbl.getBuilder();
                dbcmd = new ODBC_DB2Connect();
                //runThread = new System.Collections.Generic.List<string>();
                //input.InsertEvent += new EvenInput.InsertEventHandler(input_InsertEvent);
                dbcmd.GetReaderData += new GetReaderDataHandler(dbcmd_GetReaderData);
            }
            catch (System.Exception ex)
            {
                //ServerMeg.setAlarmMeg(ex.Message);
                throw new System.Exception(ex.StackTrace);
            }
        }

        static Execution myExceution = new Execution();

        /// <summary>
        /// �غc��
        /// </summary>
        /// <returns>�غc��</returns>
        /// 
        ///
        public static Execution getBuilder()
        {
            //if (execution == null)
            //{
            //    lock (typeof(Execution))
            //    {
            //        if (execution == null)
            //            execution = new Execution();
            //    }
            //}

            //return execution;         
            return myExceution;
        }
        #endregion ==== �غc�� ====

        #region ==== �p����k ====
        /// <summary>
        /// �N��ʡB�b�۰ʡB�۰ʨƥ��Ʊa�J�����
        /// </summary>
        /// <param name="dataType">��ƫ��A</param>
        /// <param name="dr">DataReader</param>
        object dbcmd_GetReaderData(DataType dataType, object reader)
        {
            if (dataType == DataType.IIPEvent)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                System.Collections.Hashtable ht = getIIPEventData(dr);
                if (IsSpecialEvent(dr[2].ToString()))//�P�_�O�_��"�S��ƥ�"
                {
                    //��"�S��ƥ�B�z�W�h��"�ɨ��o�����W�h�[�JExecution Table(������)
                    //ServerMeg.setAlarmMeg("��\"�S��ƥ�B�z�W�h��\"�ɨ��o�����W�h�[�JExecution Table(������)�C");
                    throw new System.Exception("�S��ƥ�B�z������");
                }
                else
                {
                    //ServerMeg.setServerMeg("�T�{����Ƭ��D�S��ƥ�A�i�J������C");
                    CategoryType type = (CategoryType)System.Enum.Parse(typeof(CategoryType), dr[1].ToString().ToUpper());
                    return categoryFactory(type, ht).getExecution(dr[0].ToString());
                    //�����
                    //System.Threading.ParameterizedThreadStart start = new System.Threading.ParameterizedThreadStart(categoryFactory(type, ht).getExecution);
                    //System.Threading.Thread t = new System.Threading.Thread(start);
                    //runThread.Add(dr[0].ToString());
                    //t.Start(runThread);
                }
            }
            return null;
        }
        /// <summary>
        /// �N�ƥ��ƥ�datareader��Jhashtable
        /// </summary>
        /// <param name="dr">DataReader</param>
        /// <returns>Hashtable</returns>
        private System.Collections.Hashtable getIIPEventData(System.Data.Odbc.OdbcDataReader dr)
        {
            System.Collections.Hashtable obj = new System.Collections.Hashtable();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                obj.Add(dr.GetName(i), dr[i]);
            }
            return obj;
        }        


        /// <summary>
        /// �P�_�O�_��"�S��ƥ�"
        /// </summary>
        /// <param name="inc_name">�ƥ����O</param>
        /// <returns></returns>
        private bool IsSpecialEvent(string inc_name)
        {
            //�ٵL"�S��ƥ������"�i�H���,�H��s�W(������)
            //ServerMeg.setAlarmMeg("�ٵL\"�S��ƥ������\"�i�H���,�H��s�W(������)�C");
            return false;
        }

        /// <summary>
        /// �ƥ����O�u�t
        /// </summary>
        /// <param name="type">�ƥ����O���A</param>
        /// <param name="row">�ƥ���</param>
        /// <returns></returns>
        private AEvent categoryFactory(CategoryType type,System.Collections.Hashtable ht)
        {
            AEvent category = null;
            switch (type)
            {
                case CategoryType.GEN:
                    category = new GenEvent(ht, type);
                    break;
                case CategoryType.OBS:
                    category = new ObsEvent(ht, type);
                    break;
                case CategoryType.OTH:
                    category = new OthEvent(ht, type);
                    break;
                case CategoryType.RES:
                    category = new ResEvent(ht, type);
                    break;
                case CategoryType.TUN:
                    category = new TunEvent(ht, type);
                    break;
                case CategoryType.WEA:
                    category = new WeaEvent(ht, type);
                    break;
            }
            return category;
        }
        #endregion ==== �p����k ====

        #region ==== ������k ====
        public void InputIIP_Event(int eventId)
        {
            input.setAutoEvent(eventId);
        }

        public void GenerateExecutionTable(int eventId)
        {
            //try
            //{
            System.Collections.Generic.List<object> objs = (System.Collections.Generic.List<object>)dbcmd.select(DataType.IIPEvent, GetSelectCmd.getIIPEventCmd(eventId.ToString(), true));
            //return (int)objs[0];
            //}
            //catch //(System.Exception ex)
            //{
            //    //ServerMeg.setAlarmMeg(ex.Message);
            //    throw new System.Exception("�[�J�s���ƥ󥢱�!!");
            //}
        }

        public void CloseMoving(int movingId)
        {
            input.closeEvent(movingId);
            EasyClient.getHost().CloseMovingConstructEvent(movingId);
        }


        public void Close()
        {
            if (DBConnect.ODBC_DB2Connect.conn != null)
                DBConnect.ODBC_DB2Connect.conn.Close();
        }
        ///// <summary>
        ///// ��ʡB�b�۰ʨƥ�^���ϥΪ�
        ///// </summary>
        ///// <param name="sender">�����p���s��</param>
        //public void reportEvent(LoginMode type, string rspId)
        //{
        //    if (ConfirmEvent != null)
        //    {
        //        ConfirmEvent(type, rspId);
        //    }
        //}

        ///// <summary>
        /////  ��s�����p�����
        ///// </summary>
        ///// <param name="dataType">��ƫ��A</param>
        //public void renewTable(RenewData dataType)
        //{
        //    while (true)
        //    {
        //        if (runThread.Count == 0)
        //        {
        //            rspData.setDBData(dataType);
        //            break;
        //        }
        //    }
        //}
        #endregion ==== ������k ====
    }
}
