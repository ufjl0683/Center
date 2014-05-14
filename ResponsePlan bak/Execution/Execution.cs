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



                if ((string)ht["INC_DIRECTION"] == "S" || (string)ht["INC_DIRECTION"] == "E")
                {
                    if ((int)ht["FROM_MILEPOST1"] > (int)ht["TO_MILEPOST1"])
                    {
                        int k = (int)ht["FROM_MILEPOST1"];
                        ht["FROM_MILEPOST1"] = ht["TO_MILEPOST1"];
                        ht["TO_MILEPOST1"] = k;
                    }
                }
                else if ((string)ht["INC_DIRECTION"] == "N" || (string)ht["INC_DIRECTION"] == "W")
                {
                    if ((int)ht["FROM_MILEPOST1"] < (int)ht["TO_MILEPOST1"])
                    {
                        int k = (int)ht["FROM_MILEPOST1"];
                        ht["FROM_MILEPOST1"] = ht["TO_MILEPOST1"];
                        ht["TO_MILEPOST1"] = k;
                    }
                }

                SetFireDrangeItemData(ht);

                //Lane_Count改為參照tbllanecount
                string cmd = string.Format("Select Lane_Count from {0}.{1} where LineID = '{2}' "
                    + "and ((Start_Mile < {3} and End_Mile >= {4}) or (End_Mile < {3} and Start_Mile >= {4})) and Direction = '{5}';"
                    , RSPGlobal.GlobaSchema, "tbllanecount", ht["INC_LINEID"], ht["FROM_MILEPOST1"], ht["TO_MILEPOST1"], ((string)ht["INC_DIRECTION"])[0]);
                System.Data.DataTable tmpDT = dbcmd.Select(cmd);
                if (tmpDT != null && tmpDT.Rows.Count > 0)
                {
                    ht["LANE_COUNT"] = tmpDT.Rows[0][0];
                }


                if (IsSpecialEvent(dr[2].ToString()))//判斷是否為"特殊事件"
                {
                    //當有"特殊事件處理規則表"時取得對應規則加入Execution Table(未完成)
                    //ServerMeg.setAlarmMeg("當有\"特殊事件處理規則表\"時取得對應規則加入Execution Table(未完成)。");
                    switch (dr[2].ToString())
                    {
                        case "48":
                        case "42":
                            return categoryFactory(CategoryType.RGS, ht).getExecution(dr[0].ToString());
                        case "49":
                            return categoryFactory(CategoryType.RMS, ht).getExecution(dr[0].ToString());
                        case "142":
                            return categoryFactory(CategoryType.LTR, ht).getExecution(dr[0].ToString());
                        case "60":
                            return categoryFactory(CategoryType.TUNFire, ht).getExecution(dr[0].ToString());
                        case "154":
                            return categoryFactory(CategoryType.PARK, ht).getExecution(dr[0].ToString());
                        default:
                            throw new System.Exception("特殊事件處理未完成");
                    }
                    
                }
                else
                {
                    //ServerMeg.setServerMeg("確認中資料為非特殊事件，進入執行緒。");
                    if (dr[1].ToString().ToUpper() == "IID")//林於總驗要求
                    {
                        return categoryFactory(CategoryType.GEN, ht).getExecution(dr[0].ToString());
                    }

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

        private void SetFireDrangeItemData(System.Collections.Hashtable HT)
        {
            if (HT["INC_NAME"].Equals(35) || HT["INC_NAME"].Equals(37)) //35火警 37危險品散落
            {
                int Start = (int)HT["FROM_MILEPOST1"];
                int End = (int)HT["TO_MILEPOST1"];
                System.Data.DataTable DT = dbcmd.Select(string.Format("Select tun.STARTMILEAGE,tun.ENDMILEAGE " 
                    + " from TBLGROUPTUNNEL tun,TBLGROUPSECTION sec where tun.SECTIONID = sec.SECTIONID and sec.Lineid = '{0}'; ",HT["INC_LINEID"]));
                bool InTun = false;
                foreach (System.Data.DataRow dr in DT.Rows)
                {
                    int Tun1 = (int)dr[0];
                    int Tun2 = (int)dr[1];

                    if ((Start >= Tun1 && Start <= Tun2) || (End >= Tun1 && End <= Tun2) || (Start >= Tun1 && End <= Tun2))
                    {
                        InTun = true;
                        break;
                    }
                }

                if (InTun)
                {
                    if (HT["INC_DIRECTION"].Equals("N") || HT["INC_DIRECTION"].Equals("S"))
                    {
                        HT["INC_DIRECTION"] = "NS";
                    }
                    else if (HT["INC_DIRECTION"].Equals("W") || HT["INC_DIRECTION"].Equals("E"))
                    {
                        HT["INC_DIRECTION"] = "WE";
                    }

                    HT["BLOCKTYPEID"] = 1;
                    string INC_BLOCKAGE = "0000000000";
                    try
                    {
                        for (int i = 0; i < System.Convert.ToInt32(HT["LANE_COUNT"]); i++)
                        {
                            INC_BLOCKAGE = INC_BLOCKAGE.Substring(0, i) + "1" + INC_BLOCKAGE.Substring(i + 1);
                        }
                        HT["INC_BLOCKAGE"] = INC_BLOCKAGE;
                    }
                    catch
                    {
                        ;
                    }                    
                }
            }
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
            switch (inc_name)
            {
                case "48":
                case "42":
                    return true;
                case "49":
                    return true;
                case "60":
                    return true;
                case "142":
                    return true;
                case "154":
                    return true;
                default:
                    return false;

            }
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
                case CategoryType.RMS:
                    category = new RmsEvent(ht, type);
                    break;
                case CategoryType.LTR:
                    category = new LtrEvent(ht, type);
                    break;
                case CategoryType.RGS:
                    category = new RgsEvent(ht, type);
                    break;
                case CategoryType.TUNFire:
                    category = new TunFireEvent(ht, type);
                    break;
                case CategoryType.PARK:
                    category = new ParkEvent(ht, type);
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
            if (objs.Count == 0)
            {
                if (dbcmd.Select("Select * from TBLIIPEVENT where eventID = " + eventId + ";").Rows.Count > 0)
                {
                    return;
                }

                throw new System.Exception("Not Found at GenerateExecutionTable " + eventId);
            }

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
