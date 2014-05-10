using System;
using System.Collections.Generic;
using System.Text;
using DBConnect;

namespace Execution.Category
{
    /// <summary>
    /// 抽象事件類別
    /// </summary>
    internal abstract class AEvent
    {
        protected System.Collections.Hashtable ht;
        protected CategoryType type;            //事件類別
        protected byte secType;
        protected ODBC_DB2Connect com = null;           
        //private int mode = 2;
        protected bool isNear = false;
        protected bool NearDirError = false;
        
        protected void Initialize(System.Collections.Hashtable ht, CategoryType type)
        {
            this.type = type;
            this.secType = Convert.ToByte(ht["INC_NAME"]);
            this.ht = ht;
            if (ht["INC_BLOCKAGE"].ToString() == "") ht["INC_BLOCKAGE"] = "0000000000";//如果沒有"阻斷車道字串"，預設為"0000000000"
            this.com = new DBConnect.ODBC_DB2Connect();
            
            System.Data.DataRow DR = RSPGlobal.GetLineNameDT().Rows.Find(ht["INC_LINEID"]);
            ht.Add("ORIGINAL_FROM_MILEPOST1", (int)ht["FROM_MILEPOST1"]);
            ht.Add("ORIGINAL_TO_MILEPOST1", (int)ht["TO_MILEPOST1"]);
            ht.Add("ORIGINAL_INC_LOCATION", (string)ht["INC_LOCATION"]);

            if (DR != null)
            {
                string SecCmd = string.Empty;
                if ((int)ht["FROM_MILEPOST1"] < (int)DR[2])
                {                   
                    ht["FROM_MILEPOST1"] = (int)DR[2];
                    
                    if ((int)ht["TO_MILEPOST1"] < (int)DR[2])
                    {                        
                        ht["TO_MILEPOST1"] = (int)DR[2];
                    }

                    SecCmd = string.Format("Select SectionID from {0}.{1} sec Left Join  {0}.{2} div on sec.Start_DivisionID = div.divisionID "
                        + " where sec.LineID = '{3}' and sec.Direction = '{4}' order by div.mileage fetch first 1 rows only;"
                        , RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblGroupSection, DBConnect.DB2TableName.tblGroupDivision, ht["INC_LINEID"], ht["INC_DIRECTION"]);
                    System.Data.DataTable DT = com.Select(SecCmd);
                    ht["INC_LOCATION"] = DT.Rows[0][0];
                    if (ht["INC_DIRECTION"].Equals("S") || ht["INC_DIRECTION"].Equals("W"))
                    {
                        NearDirError = true;
                    }

                    isNear = true;                    
                }
                else if ((int)ht["FROM_MILEPOST1"] > (int)DR[3])
                {
                    ht["FROM_MILEPOST1"] = (int)DR[3];

                    if ((int)ht["TO_MILEPOST1"] > (int)DR[3])
                    {
                        ht["TO_MILEPOST1"] = (int)DR[3];
                    }
                    SecCmd = string.Format("Select SectionID from {0}.{1} sec Left Join  {0}.{2} div on sec.Start_DivisionID = div.divisionID "
                        + " where sec.LineID = '{3}' and sec.Direction = '{4}' order by div.mileage desc fetch first 1 rows only;"
                        , RSPGlobal.GlobaSchema,DBConnect.DB2TableName.tblGroupSection, DBConnect.DB2TableName.tblGroupDivision, ht["INC_LINEID"], ht["INC_DIRECTION"]);
                    System.Data.DataTable DT = com.Select(SecCmd);
                    ht["INC_LOCATION"] = DT.Rows[0][0];
                    if (ht["INC_DIRECTION"].Equals("N") || ht["INC_DIRECTION"].Equals("E"))
                    {
                        NearDirError = true;
                    }

                    isNear = true;
                }
            }
            
        }

        /// <summary>
        /// 反應計劃主要方法（執行緒）
        /// </summary>
        /// <param name="sender">半自動和手動編號暫存的Hashtable</param>
        public int getExecution(object sender)
        {
            //System.Collections.Generic.List<string> runThread = (System.Collections.Generic.List<string>)sender;
            ExecutionObj exObj = new ExecutionObj();

            exObj.RspID = ht["INC_ID"].ToString();
            exObj.EventID = ht["EVENTID"].ToString();
            AEvent myEvent = factory();
            if (myEvent == null) return -1;
            myEvent.produceExecution(exObj);
            lock (exObj)
            {
                EasyClient easy = new EasyClient();
                easy.saveExecution(ht["INC_ID"].ToString(), Convert.ToInt32(ht["EVENTID"]), exObj, false);
                //saveExecution(exObj);
            }
            return Convert.ToInt32(exObj.EventID);
            //if (Execution.getBuilder() != null)
            //{
            //    switch (Convert.ToInt32(ht["INC_LOGIN_MODE"].ToString()))
            //    {
            //        case 1:
            //            Execution.getBuilder().reportEvent(LoginMode.Auto, exObj.EventID);
            //            break;
            //        case 2:
            //            Execution.getBuilder().reportEvent(LoginMode.Half, exObj.RspID);
            //            break;
            //        default:
            //            Execution.getBuilder().reportEvent(LoginMode.Manual, exObj.RspID);
            //            break;                       
            //    }
            //}
            //runThread.Remove(ht["INC_ID"].ToString());
        }

        /// <summary>
        /// 獲得顯示所有資料資料
        /// </summary>
        /// <param name="sender">顯示所有資料資料</param>
        /// <returns>顯示所有資料資料</returns>
        public virtual ExecutionObj produceExecution(ExecutionObj sender)
        {
            com.GetReaderData += new GetReaderDataHandler(com_GetReaderData);
            sender.EventTime = DateTime.Parse(ht["INC_TIME"].ToString());
            sender.Memo += "==>>Get Unit";
            List<LiaiseUnit> units = new List<LiaiseUnit>();

            foreach (object obj in getDisplayContent(Convert.ToByte(ht["INC_NAME"]), Convert.ToInt32(ht["FROM_MILEPOST1"]), Convert.ToInt32(ht["TO_MILEPOST1"]),ht["INC_LINEID"].ToString()))
            {
                if (obj != null) units.Add((LiaiseUnit)obj);
            }
            sender.Units = units;
            return sender;
        }

        /// <summary>
        /// 獲得連絡單位資料
        /// </summary>
        /// <param name="type">事件次分類</param>
        /// <returns>連絡單位資料</returns>
        private List<object> getDisplayContent(byte secType, int start_mile,int end_mile,string LineID)
        {
            try
            {
                List<object> units = (List<object>)com.select(DataType.Unit, Command.GetSelectCmd.getUnitData(secType, start_mile, end_mile,LineID));
                com.GetReaderData -= new GetReaderDataHandler(com_GetReaderData);
                return units;
            }
            catch//(Exception ex)
            {
                throw new Exception("獲得連絡單位資料失敗");
            }
        }

        /// <summary>
        /// Select資料的DataReader回報
        /// </summary>
        /// <param name="type">資料型態</param>
        /// <param name="dr">DataReader</param>
        protected object com_GetReaderData(DBConnect.DataType type, object reader)
        {
            if (type == DataType.Unit)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                LiaiseUnit units = new LiaiseUnit();
                units.serviceID = Convert.ToInt32(dr[0]);
                units.subServiceID = Convert.ToInt32(dr[1]);
                units.serviceName = dr[2].ToString();
                units.alarmclass = Convert.ToInt32(dr[3]);
                units.ifAlarm = Convert.ToInt32(dr[4]) == 0 ? false : true;
                units.phone = dr[5].ToString();
                units.fax = dr[6].ToString();
                units.startMile = Convert.ToInt32(dr[7]);
                units.endMile = Convert.ToInt32(dr[8]);
                return units;
            }
            else if (type == DBConnect.DataType.LaneCount)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                System.Collections.Hashtable laneCount = new System.Collections.Hashtable();
                laneCount.Add(0, Convert.ToString(dr[2]));
                laneCount.Add(1, Convert.ToString(dr[3]));
                laneCount.Add(2, Convert.ToString(dr[4]));
                laneCount.Add(3, Convert.ToString(dr[5]));
                laneCount.Add(4, Convert.ToString(dr[6]));
                laneCount.Add(5, Convert.ToString(dr[7]));
                laneCount.Add(6, Convert.ToString(dr[8]));
                laneCount.Add(7, Convert.ToString(dr[9]));
                laneCount.Add(8, Convert.ToString(dr[10]));
                laneCount.Add(9, Convert.ToString(dr[11]));
                laneCount.Add(10, Convert.ToString(dr[12]));
                laneCount.Add(11, Convert.ToString(dr[13]));
                laneCount.Add(12, Convert.ToString(dr[14]));
                laneCount.Add(13, Convert.ToString(dr[15]));
                laneCount.Add(14, Convert.ToString(dr[16]));
                return laneCount;
            }
            else if (type == DataType.Decorators)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                System.Collections.Hashtable decHT = new System.Collections.Hashtable();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (!decHT.ContainsKey(dr.GetName(i))) decHT.Add(dr.GetName(i), dr[i]);
                }
                return decHT;
            }
            else
                return null;
        }

        /// <summary>
        /// 裝飾者工廠
        /// </summary>
        /// <returns></returns>
        protected virtual AEvent factory()
        {
            AEvent myObj = this;
            com.GetReaderData += new DBConnect.GetReaderDataHandler(com_GetReaderData);
            Degree degree = getDegree();
            //this.serMeg.setServerMeg(string.Format("事件嚴重程度為:{0}", degree.ToString()));
            //this.serMeg.setServerMeg(string.Format("事件壅塞程度為:{0}", ht["INC_CONGESTION"].ToString()));  
            if (degree != Degree.N)
            {
                string cmd = string.Format("Select * from {0}.{1} where alarmType = '{2}'", RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblRSPDevice, type);
                System.Data.DataTable DeviceTypeDT = com.Select(cmd);

 
                List<object> Decorators = (List<object>)com.select(DataType.Decorators, Command.GetSelectCmd.getDisplyDevTypes(degree, secType, ht["INC_LOCATION"].ToString().Trim()));
                com.GetReaderData -= new GetReaderDataHandler(com_GetReaderData);
                
                System.Collections.Hashtable HT = (System.Collections.Hashtable)Decorators[0];
                Decorators.Clear();

                List<DeviceType> adorns = new List<DeviceType>();
                System.Data.DataRow dr = DeviceTypeDT.Rows[0];
                foreach (System.Data.DataColumn dc in DeviceTypeDT.Columns)
                {
                    if (dr[dc].Equals(1))
                    {
                        if (Enum.IsDefined(typeof(DeviceType), dc.ColumnName))
                        {
                            adorns.Add((DeviceType)Enum.Parse(typeof(DeviceType),dc.ColumnName));
                            Decorators.Add(HT.Clone());
                        }
                    }
                }

                //foreach (object obj in Decorators)
                //{
                //    System.Collections.Hashtable decHT = (System.Collections.Hashtable)obj;
                //    if (Convert.ToInt32(decHT["DEVSTART"]) == 1)
                //        adorns.Add((DeviceType)Enum.Parse(typeof(DeviceType), decHT["DEVICETYPE"].ToString()));
                //}
                return getAdorns(adorns, myObj, Decorators, degree);
            }
            else
                return null;
        }

        /// <summary>
        /// 裝飾者工廠使用方法
        /// </summary>
        /// <param name="adorns">裝飾物件是否裝飾</param>
        /// <param name="myObj">AEvent</param>
        /// <returns></returns>
        protected AEvent getAdorns(List<DeviceType> adorns, AEvent myObj, List<object> Decorators,Degree degree)
        {
            for (int i = 0; i < adorns.Count;i++ )
            {
                switch (adorns[i])
                {
                    case DeviceType.CMS:
                        myObj = new CMS(myObj, type, ht, DeviceType.CMS, (System.Collections.Hashtable)Decorators[i], degree);
                        break;
                    case DeviceType.WIS:
                        myObj = new WIS(myObj, type, ht, DeviceType.WIS, (System.Collections.Hashtable)Decorators[i], degree);
                        break;
                    case DeviceType.FS:
                        myObj = new FS(myObj, type, ht, DeviceType.FS, (System.Collections.Hashtable)Decorators[i], degree);
                        break;
                    case DeviceType.RGS:
                        myObj = new RGS(myObj, type, ht, DeviceType.RGS, (System.Collections.Hashtable)Decorators[i], degree);
                        break;
                    case DeviceType.LCS:
                        myObj = new LCS(myObj, type, ht, DeviceType.LCS, (System.Collections.Hashtable)Decorators[i], degree);
                        break;
                    case DeviceType.CSLS:
                        myObj = new CSLS(myObj, type, ht, DeviceType.CSLS, (System.Collections.Hashtable)Decorators[i], degree);
                        break;
                    case DeviceType.RMS:
                        myObj = new RMS(myObj, type, ht, DeviceType.RMS, (System.Collections.Hashtable)Decorators[i], degree);
                        break;
                    case DeviceType.MAS:
                        myObj = new MAS(myObj, type, ht, DeviceType.MAS, (System.Collections.Hashtable)Decorators[i], degree);
                        break;
                    default:
                        myObj = new CCTV(myObj, type, ht, DeviceType.CCTV, (System.Collections.Hashtable)Decorators[i], degree);
                        break;
                }
            }
            return myObj;
        }

        /// <summary>
        /// 獲得嚴重程度
        /// </summary>
        /// <returns>嚴重程度</returns>
        protected virtual Degree getDegree()
        {
            if (NearDirError)
            {
                return Degree.N;
            }
            else if (!string.IsNullOrEmpty(ht["INC_SERVERITY"].ToString()) && ht["INC_SERVERITY"].ToString() != "0"  )
            {
                return (Degree)Enum.Parse(typeof(Degree), ht["INC_SERVERITY"].ToString());
            }
            else
            {
                if ((int)ht["INC_NAME"] == 31)
                {
                    try
                    {
                        int k = Convert.ToInt32(ht["BLOCKTYPEID"]);
                        if (k == 11)
                        {
                            return Degree.N;
                        }
                    }
                    catch
                    {
                    }
                }

                List<object> list = (List<object>)com.select(DBConnect.DataType.LaneCount, Command.GetSelectCmd.getSeriousByAlarmClass(secType));
                if (list.Count == 0)
                {
                    //this.serMeg.setAlarmMeg("事件嚴重程度參數表判斷沒有程度(請查路段車道數是否和阻斷車道數對應)!!!");
                    return Degree.N;
                }
                System.Collections.Hashtable myht = (System.Collections.Hashtable)list[0];
                

                switch (myht[(int)ht["INC_CONGESTION"] - 1].ToString())
                {
                    case "L":
                        return Degree.L;
                    case "M":
                        return Degree.M;
                    case "H":
                        return Degree.H;
                    case "S":
                        return Degree.S;
                    default:
                        {
                            //this.serMeg.setAlarmMeg("事件嚴重程度參數表判斷沒有程度(請查路段車道數是否和阻斷車道數對應)!!!");
                            return Degree.N;
                        }
                }
            }
        }
    }
}
