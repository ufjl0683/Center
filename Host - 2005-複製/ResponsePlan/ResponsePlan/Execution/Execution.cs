using DBConnect;
using Execution.Command;
using Execution.Category;

namespace Execution
{
    public class Execution 
    {
        //private static System.Collections.Generic.List<string> runThread;   //正在執行的執行緒名稱集合
        //private static Execution execution;                                 //獨體建構物件 
        private EvenInput input = null;                                     //輸入事件物件
        private RSPDataTbl rspData;                                         //反應計劃資料
        private ODBC_DB2Connect dbcmd;                                            //資料庫物件          
        private TunnelData tunnelData;

        //public delegate void ConfirmEventHandler(LoginMode type, string sender);
        //public delegate void NotConfirmEventHandler(System.Collections.Generic.List<string> rspIDs);
        ///// <summary>
        ///// 反應計劃產生給使用者
        ///// </summary>
        //public event ConfirmEventHandler ConfirmEvent;
        #region ==== 建構元 ====
        /// <summary>
        /// 建構元
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
        /// 建構者
        /// </summary>
        /// <returns>建構元</returns>
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
        #endregion ==== 建構元 ====

        #region ==== 私有方法 ====
        /// <summary>
        /// 將手動、半自動、自動事件資料帶入執行緒
        /// </summary>
        /// <param name="dataType">資料型態</param>
        /// <param name="dr">DataReader</param>
        object dbcmd_GetReaderData(DataType dataType, object reader)
        {
            if (dataType == DataType.IIPEvent)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                System.Collections.Hashtable ht = getIIPEventData(dr);
                if (IsSpecialEvent(dr[2].ToString()))//判斷是否為"特殊事件"
                {
                    //當有"特殊事件處理規則表"時取得對應規則加入Execution Table(未完成)
                    //ServerMeg.setAlarmMeg("當有\"特殊事件處理規則表\"時取得對應規則加入Execution Table(未完成)。");
                    throw new System.Exception("特殊事件處理未完成");
                }
                else
                {
                    //ServerMeg.setServerMeg("確認中資料為非特殊事件，進入執行緒。");
                    CategoryType type = (CategoryType)System.Enum.Parse(typeof(CategoryType), dr[1].ToString().ToUpper());
                    return categoryFactory(type, ht).getExecution(dr[0].ToString());
                    //執行緒
                    //System.Threading.ParameterizedThreadStart start = new System.Threading.ParameterizedThreadStart(categoryFactory(type, ht).getExecution);
                    //System.Threading.Thread t = new System.Threading.Thread(start);
                    //runThread.Add(dr[0].ToString());
                    //t.Start(runThread);
                }
            }
            return null;
        }
        /// <summary>
        /// 將事件資料由datareader放入hashtable
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
        /// 判斷是否為"特殊事件"
        /// </summary>
        /// <param name="inc_name">事件次類別</param>
        /// <returns></returns>
        private bool IsSpecialEvent(string inc_name)
        {
            //還無"特殊事件對應表"可以比對,以後新增(未完成)
            //ServerMeg.setAlarmMeg("還無\"特殊事件對應表\"可以比對,以後新增(未完成)。");
            return false;
        }

        /// <summary>
        /// 事件類別工廠
        /// </summary>
        /// <param name="type">事件類別型態</param>
        /// <param name="row">事件資料</param>
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
        #endregion ==== 私有方法 ====

        #region ==== 公眾方法 ====
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
            //    throw new System.Exception("加入新的事件失敗!!");
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
        ///// 手動、半自動事件回報使用者
        ///// </summary>
        ///// <param name="sender">反應計劃編號</param>
        //public void reportEvent(LoginMode type, string rspId)
        //{
        //    if (ConfirmEvent != null)
        //    {
        //        ConfirmEvent(type, rspId);
        //    }
        //}

        ///// <summary>
        /////  更新反應計劃資料
        ///// </summary>
        ///// <param name="dataType">資料型態</param>
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
        #endregion ==== 公眾方法 ====
    }
}
