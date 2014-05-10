using System;
using System.Collections.Generic;
using System.Text;
using DBConnect;
using Execution.Command;


namespace Execution
{
    public class EasyClient
    {
       
        ODBC_DB2Connect com = null;
        ExecutionObj exeObj = null;
        private static RemoteInterface.HC.I_HC_FWIS hobj;                          //host   

        public EasyClient()
        {
            com = new ODBC_DB2Connect();
            //com = new DB2Connection();
            com.GetReaderData += new GetReaderDataHandler(com_GetReaderData);
        }

        /// <summary>
        /// Execution Table狀態改變
        /// </summary>
        /// <param name="status">狀態(1.確認中,2.等待中,3.執行中,4.結束,5.中止,6.放棄)</param>
        /// <param name="rspID">反應計劃編號</param>
        public void executionStatusChange(int status, string rspID)
        {
            try
            {
                com.update(GetUpdateCmd.setRSPExecutionCmd(status, rspID));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 給定Host
        /// </summary>
        public static RemoteInterface.HC.I_HC_FWIS getHost()
        {
            if (hobj == null)
            {
                lock (typeof(RemoteInterface.HC.I_HC_FWIS))
                {
                    if (hobj == null)
                    {
                        ODBC_DB2Connect com = new ODBC_DB2Connect();
                        com.GetReaderData += new GetReaderDataHandler(com_GetReader);
                        string hostip = ((List<object>)com.select(DataType.Host, Command.GetSelectCmd.getHostIP()))[0].ToString();
                        //string hostip = "10.21.50.217";
                        System.Diagnostics.Trace.WriteLine("Host IP = " + hostip);

                        hobj = (RemoteInterface.HC.I_HC_FWIS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_FWIS),
                                RemoteInterface.RemoteBuilder.getRemoteUri(hostip, 9010, "FWIS"));
                        
                    }
                }
            }
            else
            {
                try
                {
                    ((RemoteInterface.RemoteClassBase)hobj).HelloWorld();
                }
                catch
                {
                    lock (typeof(RemoteInterface.HC.I_HC_FWIS))
                    {
                        ODBC_DB2Connect com = new ODBC_DB2Connect();
                        com.GetReaderData += new GetReaderDataHandler(com_GetReader);
                        string hostip = ((List<object>)com.select(DataType.Host, Command.GetSelectCmd.getHostIP()))[0].ToString();
                        System.Diagnostics.Trace.WriteLine("Host IP = " + hostip);

                        hobj = (RemoteInterface.HC.I_HC_FWIS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_FWIS),
                                RemoteInterface.RemoteBuilder.getRemoteUri(hostip, 9010, "FWIS"));
                    }

                }
            }
            if (hobj == null)
                System.Diagnostics.Trace.WriteLine("Host 連線失敗");
            return hobj;
        }
          /// <summary>
        /// 取得反應計劃DataReader回傳方法
        /// </summary>
        /// <param name="type">資料型態</param>
        /// <param name="dr">DataReader</param>
        /// <returns></returns>
        static object com_GetReader(DataType type, object reader)
        {
            object result = null;
            if (type == DataType.Host)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                result = dr[0].ToString();
            }
            return result;
        }

        #region ==== 取得OutputData顯示內容 ====
        /// <summary>
        /// 取得OutputData顯示內容
        /// </summary>
        /// <param name="outputDataStr"></param>
        /// <returns></returns>
        public static string getOutputDataMeg(string outputDataStr)
        {
            object outputData = RemoteInterface.Utils.Util.StringToObj(outputDataStr);
            string result = "";
            if (outputData is RemoteInterface.HC.CMSOutputData)
            {
                RemoteInterface.HC.CMSOutputData output = (RemoteInterface.HC.CMSOutputData)outputData;
                result = string.Format("底圖：{0},ICON：{1},訊息：{2}", output.g_code_id, output.icon_id, output.mesg);
            }
            else if (outputData is RemoteInterface.HC.FSOutputData)
            {
                RemoteInterface.HC.FSOutputData output = (RemoteInterface.HC.FSOutputData)outputData;
                result = string.Format("訊息：{0}", getFSDisplay(output.type));
            }
            else if (outputData is RemoteInterface.HC.CSLSOutputData)
            {
                RemoteInterface.HC.CSLSOutputData output = (RemoteInterface.HC.CSLSOutputData)outputData;
                result = string.Format("訊息：{0}", output.dataset.Tables[0].Rows[0]["speed"].ToString());
            }
            else if (outputData is RemoteInterface.MFCC.RGS_GenericDisplay_Data)
            {
                RemoteInterface.MFCC.RGS_GenericDisplay_Data output = (RemoteInterface.MFCC.RGS_GenericDisplay_Data)outputData;
                byte icon1 = 0;
                byte icon2 = 0;
                byte graph_code_id = 0;
                string meg = "";
                getRGSDisplay(output, ref  graph_code_id, ref  icon1, ref  icon2, ref  meg);
                result = string.Format("底圖：{0},ICON1：{1},ICON2：{2},訊息：{3}", graph_code_id, icon1, icon2, meg);
            }
            else if (outputData is RemoteInterface.HC.LCSOutputData)
            {
                RemoteInterface.HC.LCSOutputData output = (RemoteInterface.HC.LCSOutputData)outputData;
                result = string.Format("訊息：{0}", getLCSDisplay(output));
            }
            else if (outputData is RemoteInterface.HC.RMSOutputData)
            {
                RemoteInterface.HC.RMSOutputData output = (RemoteInterface.HC.RMSOutputData)outputData;
                result = string.Format("訊息：{0}", getRMSDisplay(output));
            }
            return result;
        }

        /// <summary>
        /// 取得FS顯示內容
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string getFSDisplay(byte type)
        {
            switch (type)
            {
                case 1:    //濃霧
                    return "霧";
                case 2:    //強風
                    return "風";
                case 3:    //豪雨
                    return "雨";
                default:    //其他
                    return "熄滅";
            }
        }

        /// <summary>
        /// 取得RGS顯示內容
        /// </summary>
        /// <param name="output"></param>
        /// <param name="graph_code_id"></param>
        /// <param name="icon1"></param>
        /// <param name="icon2"></param>
        /// <param name="meg"></param>
        public static void getRGSDisplay(RemoteInterface.MFCC.RGS_GenericDisplay_Data output, ref byte graph_code_id, ref byte icon1, ref byte icon2, ref string meg)
        {
            for (int i = 0; i < output.icons.Length; i++)
            {
                if (i == 1)
                    icon1 = output.icons[i].icon_code_id;
                else if (i == 2)
                    icon2 = output.icons[i].icon_code_id;
            }
            foreach (RemoteInterface.MFCC.RGS_Generic_Message_Data message in output.msgs)
            {
                if (message == null) continue;
                meg += message.messgae + "\r";
            }
            graph_code_id = output.graph_code_id;
            meg = meg.Substring(0, meg.Length - 1 > 0 ? meg.Length - 1 : 0);
        }

        /// <summary>
        /// 取得LCS顯示內容
        /// </summary>
        /// <param name="output">LCSOutputData</param>
        /// <returns></returns>
        public static string getLCSDisplay(RemoteInterface.HC.LCSOutputData output)
        {
            string display = "";
            foreach (System.Data.DataRow row in output.dataset.Tables[1].Rows)
            {
                display += string.Format("{0}顯示內容:{1}   ", row["sign_no"].ToString() == "0" ? "內車道" : "外車道", Byte2String(Convert.ToByte(row["sign_status"])));
            }
            return display;
        }

        /// <summary>
        /// 取得RMS顯示內容
        /// </summary>
        /// <param name="output">RMSOutputData</param>
        /// <returns></returns>
        public static string getRMSDisplay(RemoteInterface.HC.RMSOutputData output)
        {
            string display = getRmsModeString(output.mode);
            if (output.mode == 3)
                display += string.Format(",時制為{0}", output.planno);
            return display;
        }

        public static string getMASDisplay(RemoteInterface.HC.MASOutputData output)
        {
            StringBuilder sb = new StringBuilder();
            for(int i =0;i<output.displays.Length;i++)
            {
                if (output.displays[i] is int)
                {
                    sb.AppendFormat("第{0}車道速限:{1} ",i+1,output.displays[i]);
                }
                else if (output.displays[i] is RemoteInterface.HC.CMSOutputData)
                {
                    RemoteInterface.HC.CMSOutputData cms = (RemoteInterface.HC.CMSOutputData)output.displays[i];
                    sb.AppendFormat("第{0}車道圖示:{1} ", i + 1, cms.g_code_id);
                }
            }
            return sb.ToString();

        }


        /// <summary>
        /// LCS byte to string
        /// </summary>
        /// <param name="inputByte"></param>
        /// <returns></returns>
        public static string Byte2String(byte inputByte)
        {
            string returnStr = string.Empty;
            switch (inputByte)
            {
                case 0: returnStr = "熄滅"; break;
                case 1: returnStr = "↓"; break;
                case 2: returnStr = "╳"; break;
                case 3: returnStr = "↘"; break;
                case 4: returnStr = "↙"; break;
                case 5: returnStr = "↓(閃爍)"; break;
                case 6: returnStr = "╳(閃爍)"; break;
                case 7: returnStr = "↘(閃爍)"; break;
                case 8: returnStr = "↙(閃爍)"; break;
                default: returnStr = "熄滅"; break;
            }
            return returnStr;
        }

        /// <summary>
        /// RMS mode to string
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string getRmsModeString(int mode)
        {
            // 0：固定時制模式
            // 1：區域交通反應模式
            // 2：預設時制模式
            // 3：匝道關閉
            // 4：整合式交通反應模式
            // 5：手動操作模式─(此模式無法設定，唯TC進入此模式時會主
            //                  動回報且可以04H+80H查詢)
            // 6：儀控終止─(此模式無法設定，唯TC進入此模式時會主動回
            //報且可以04H+80H查詢)
            switch (mode)
            {
                case 0:
                    return "固定時制模式";
                case 1:
                    return "區域交通模式";
                case 2:
                    return "預設時制模式";
                case 3:
                    return "匝道關閉";
                case 4:
                    return "整合式交通反應模式";
                case 5:
                    return "手動操作模式";
                case 6:
                    return "儀控終止";
                default:
                    return string.Format("{0}無此模式", mode);
            }
        }
        #endregion ==== 取得OutputData顯示內容 ====

        #region ==== 取得反應計劃 ====
        /// <summary>
        ///  取得反應計劃
        /// </summary>
        /// <param name="rspId">反應計劃編號</param>
        /// <returns>反應計劃</returns>
        public ExecutionObj getExecutionObj(string rspId)
        {
            exeObj = new ExecutionObj();
            exeObj.Units = new List<LiaiseUnit>();
            exeObj.CCTVOutputData = new System.Collections.Hashtable();
            exeObj.CMSOutputData = new System.Collections.Hashtable();
            exeObj.CSLSOutputData = new System.Collections.Hashtable();
            exeObj.FSOutputData = new System.Collections.Hashtable();
            exeObj.RGSOutputData = new System.Collections.Hashtable();
            exeObj.RMSOutputData = new System.Collections.Hashtable();
            exeObj.WISOutputData = new System.Collections.Hashtable();
            exeObj.LCSOutputData = new System.Collections.Hashtable();
            exeObj.MASOutputData = new System.Collections.Hashtable();
            //try
            //{
                //取得警告編號
                com.select(DataType.IIPEvent, Command.GetSelectCmd.getIIPEventCmd(rspId, false));
                //取得"反應計劃編號","事件發生時間","備註","處理狀態"
                com.select(DataType.Execution, Command.GetSelectCmd.getRspExecution(rspId));
                //取得"處理單位資料"
                com.select(DataType.Unit, Command.GetSelectCmd.getExeUnitData(rspId, exeObj.AlarmClass));
                //取得"設備輸出資料"
                com.select(DataType.OutputData, Command.GetSelectCmd.getOutputData(rspId));

                com.GetReaderData -= new DBConnect.GetReaderDataHandler(com_GetReaderData);
                return exeObj;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        /// <summary>
        /// 取得反應計劃DataReader回傳方法
        /// </summary>
        /// <param name="type">資料型態</param>
        /// <param name="dr">DataReader</param>
        /// <returns></returns>
        object com_GetReaderData(DataType type, object reader)
        {
            object result = null;
            if (type == DataType.IIPEvent)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                exeObj.AlarmClass = Convert.ToInt32(dr[2].ToString());
            }
            else if (type == DataType.Execution)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                exeObj.RspID = dr[0].ToString();
                exeObj.EventID = dr[1].ToString();
                exeObj.EventTime = DateTime.Parse(dr[2].ToString());
                exeObj.Memo = dr[4].ToString();
            }
            else if (type == DataType.Unit)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                LiaiseUnit unit = new LiaiseUnit();
                unit.serviceID = Convert.ToInt32(dr[2].ToString());
                unit.subServiceID = Convert.ToInt32(dr[3].ToString());
                unit.serviceName = dr[7].ToString();
                unit.subserviceName = dr[4].ToString();
                unit.phone = dr[5].ToString();
                unit.fax = dr[6].ToString();
                unit.memo = dr[8].ToString();
                unit.ifAlarm = dr[9].ToString() == "0" ? false : true;
                exeObj.Units.Add(unit);
            }
            else if (type == DataType.OutputData)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                DeviceType dev = (DeviceType)Enum.Parse(typeof(DeviceType), dr[0].ToString());
                List<object> objs = new List<object>();
                switch (dev)
                {
                    case DeviceType.CCTV:
                        break;
                    case DeviceType.CMS:
                        objs.Clear();
                        objs.AddRange(new object[] { dr[3], RemoteInterface.Utils.Util.StringToObj(dr[1].ToString()) });
                        exeObj.CMSOutputData.Add(dr[2].ToString(), objs);
                        break;
                    case DeviceType.CSLS:
                        objs.Clear();
                        objs.AddRange(new object[] { dr[3], RemoteInterface.Utils.Util.StringToObj(dr[1].ToString()) });
                        exeObj.CSLSOutputData.Add(dr[2].ToString(), objs);
                        break;
                    case DeviceType.FS:
                        objs.Clear();
                        objs.AddRange(new object[] { dr[3], RemoteInterface.Utils.Util.StringToObj(dr[1].ToString()) });
                        exeObj.FSOutputData.Add(dr[2].ToString(), objs);
                        break;
                    case DeviceType.LCS:
                        objs.Clear();
                        objs.AddRange(new object[] { dr[3], RemoteInterface.Utils.Util.StringToObj(dr[1].ToString()) });
                        exeObj.LCSOutputData.Add(dr[2].ToString(), objs);
                        break;
                    case DeviceType.RGS:
                        objs.Clear();
                        objs.AddRange(new object[] { dr[3], RemoteInterface.Utils.Util.StringToObj(dr[1].ToString()) });
                        exeObj.RGSOutputData.Add(dr[2].ToString(), objs); 
                        break;
                    case DeviceType.RMS:
                        objs.Clear();
                        objs.AddRange(new object[] { dr[3], RemoteInterface.Utils.Util.StringToObj(dr[1].ToString()) });
                        exeObj.RMSOutputData.Add(dr[2].ToString(), objs);
                        break;
                    case DeviceType.WIS:
                        objs.Clear();
                        objs.AddRange(new object[] { dr[3], RemoteInterface.Utils.Util.StringToObj(dr[1].ToString()) });
                        exeObj.WISOutputData.Add(dr[2].ToString(), objs);
                        break; 
                    case DeviceType.MAS:
                        objs.Clear();
                        objs.AddRange(new object[] { dr[3], RemoteInterface.Utils.Util.StringToObj(dr[1].ToString()) });
                        exeObj.MASOutputData.Add(dr[2].ToString(), objs);
                        break;
                }               
            }
            return result;
        }
        #endregion ==== 取得反應計劃 ====

        #region ==== 儲存資料進入Execution table ====
        /// <summary>
        /// 儲存資料進入Execution table
        /// </summary>
        /// <param name="sender"></param>
        public void saveExecution(string rspId, int evenId, ExecutionObj sender, bool isUserChange)
        {      
            try
            {
                //儲存"反應計劃編號","事件發生時間","備註","處理狀態"
                InsertCommand cmd;
                if (!isUserChange)
                {
                    cmd = Command.GetInsertCmd.setRSPExecutionCmd(sender);
                    com.insert(cmd);
                }

                //儲存處理單位資料
                if (!isUserChange && com.Select(string.Format("Select * from {0}.{1} where INC_ID = '{2}';", RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblIIPService, rspId)).Rows.Count == 0)
                {   //使用者已設定不存
                    foreach (LiaiseUnit unit in sender.Units)
                    {
                        if (unit == null) break;
                        if (isUserChange)
                        {//嚴重問題程式區塊
                            if (com.select(DataType.Unit, Command.GetSelectCmd.getExeUnitData(rspId, exeObj.AlarmClass)) != null)
                                com.insert(Command.GetInsertCmd.setIIPServiceCmd(rspId, unit.subServiceID, unit.memo));
                            else
                                com.update(Command.GetUpdateCmd.setIIPServiceCmd(rspId, unit.subServiceID, unit.memo, DateTime.Now));
                        }//
                        else
                        {
                            cmd = Command.GetInsertCmd.setIIPServiceCmd(rspId, unit.subServiceID, unit.memo);
                            com.insert(cmd);
                        }
                    }
                }

                //儲存WIS資料
                if (sender.WISOutputData != null)
                {
                    foreach (System.Collections.DictionaryEntry de in sender.WISOutputData)
                    {
                        if (de.Value == null) break;
                        RemoteInterface.HC.CMSOutputData output = (RemoteInterface.HC.CMSOutputData)de.Value;
                        if (isUserChange)
                        {
                            if (com.select(DataType.OutputData, Command.GetSelectCmd.getOutputData(rspId, (string)de.Key)) != null)
                            {
                                com.update(Command.GetUpdateCmd.setRSPExecutionOutputDataCmd(evenId, (string)de.Key, 0, output));
                            }
                            else
                            {
                                cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.WIS, output, isUserChange);
                                com.insert(cmd);
                            }
                            com.update(Command.GetUpdateCmd.setRSPExecutionMessage2(rspId, (string)de.Key, 2, DeviceType.WIS.ToString(), output.g_code_id, output.icon_id, 0, output.mesg));
                        }
                        else
                        {
                            cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.WIS, output, isUserChange);
                            com.insert(cmd);
                            com.update(Command.GetUpdateCmd.setRSPExecutionMessage1(rspId, (string)de.Key, 2, DeviceType.WIS.ToString(), output.g_code_id, output.icon_id, 0, output.mesg));
                        }
                    }
                }

                //儲存FS資料
                if (sender.FSOutputData != null)
                {
                    foreach (System.Collections.DictionaryEntry de in sender.FSOutputData)
                    {
                        if (de.Value == null) break;
                        RemoteInterface.HC.FSOutputData output = (RemoteInterface.HC.FSOutputData)de.Value;
                        string display = EasyClient.getFSDisplay(output.type);
                        if (isUserChange)
                        {
                            if (com.select(DataType.OutputData, Command.GetSelectCmd.getOutputData(rspId, (string)de.Key)) != null)
                            {
                                com.update(Command.GetUpdateCmd.setRSPExecutionOutputDataCmd(evenId, (string)de.Key, 0, output));
                            }
                            else
                            {
                                cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.FS, de.Value, isUserChange);
                                com.insert(cmd);
                            }
                            com.update(Command.GetUpdateCmd.setRSPExecutionMessage2(rspId, (string)de.Key, 2, DeviceType.FS.ToString(), 0, 0, 0, display));
                        }
                        else
                        {
                            cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.FS, de.Value, isUserChange);
                            com.insert(cmd);
                            com.update(Command.GetUpdateCmd.setRSPExecutionMessage1(rspId, (string)de.Key, 2, DeviceType.FS.ToString(), 0, 0, 0, display));
                        }
                    }
                }

                //儲存CSLS資料
                if (sender.CSLSOutputData != null)
                {
                    foreach (System.Collections.DictionaryEntry de in sender.CSLSOutputData)
                    {
                        if (de.Value == null) break;
                        RemoteInterface.HC.CSLSOutputData output = (RemoteInterface.HC.CSLSOutputData)((List<object>)de.Value)[1];
                        if (isUserChange)
                        {
                            if (com.select(DataType.OutputData, Command.GetSelectCmd.getOutputData(rspId, (string)de.Key)) != null)
                            {
                                com.update(Command.GetUpdateCmd.setRSPExecutionOutputDataCmd(evenId, (string)de.Key, (int)((List<object>)de.Value)[0], output));
                            }
                            else
                            {
                                cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.CSLS, de.Value, isUserChange);
                                com.insert(cmd);
                            }
                            com.update(Command.GetUpdateCmd.setRSPExecutionMessage2(rspId, (string)de.Key, 2, DeviceType.CSLS.ToString(), 0, 0, 0, output.dataset.Tables[0].Rows[0]["speed"].ToString()));
                        }
                        else
                        {
                            cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.CSLS, de.Value, isUserChange);
                            com.insert(cmd);
                            com.update(Command.GetUpdateCmd.setRSPExecutionMessage1(rspId, (string)de.Key, 2, DeviceType.CSLS.ToString(), 0, 0, 0, output.dataset.Tables[0].Rows[0]["speed"].ToString()));
                        }
                    }
                }

                //儲存RGS資料
                if (sender.RGSOutputData != null)
                {
                    foreach (System.Collections.DictionaryEntry de in sender.RGSOutputData)
                    {
                        if (de.Value == null) break;
                        RemoteInterface.MFCC.RGS_GenericDisplay_Data output = (RemoteInterface.MFCC.RGS_GenericDisplay_Data)((List<object>)de.Value)[1];
                        byte icon1 = 0;
                        byte icon2 = 0;
                        byte graph_code_id = 0;
                        string meg = "";
                        EasyClient.getRGSDisplay(output, ref  graph_code_id, ref  icon1, ref  icon2, ref  meg);

                        if (isUserChange)
                        {
                            if (com.select(DataType.OutputData, Command.GetSelectCmd.getOutputData(rspId, (string)de.Key)) != null)
                            {
                                com.update(Command.GetUpdateCmd.setRSPExecutionOutputDataCmd(evenId, (string)de.Key, (int)((List<object>)de.Value)[0], output));
                            }
                            else
                            {
                                cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.RGS, de.Value, isUserChange); 
                                com.insert(cmd);
                            }
                            com.update(Command.GetUpdateCmd.setRSPExecutionMessage2(rspId, (string)de.Key, 2, DeviceType.RGS.ToString(), graph_code_id, icon1, icon2, meg));
                        }
                        else
                        {
                            cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.RGS, de.Value, isUserChange); 
                            com.insert(cmd);
                            com.update(Command.GetUpdateCmd.setRSPExecutionMessage1(rspId, (string)de.Key, 2, DeviceType.RGS.ToString(), graph_code_id, icon1, icon2, meg));
                        }
                    }
                }

                //儲存CMS資料
                if (sender.CMSOutputData != null)
                {
                    foreach (System.Collections.DictionaryEntry de in sender.CMSOutputData)
                    {
                        if (de.Value == null) break;
                        RemoteInterface.HC.CMSOutputData output = (RemoteInterface.HC.CMSOutputData)((List<object>)de.Value)[1];

                        if (isUserChange)
                        {
                            if (com.select(DataType.OutputData, Command.GetSelectCmd.getOutputData(rspId, (string)de.Key)) != null)
                            {
                                com.update(Command.GetUpdateCmd.setRSPExecutionOutputDataCmd(evenId, (string)de.Key, (int)((List<object>)de.Value)[0], output));
                            }
                            else
                            {
                                cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.CMS, de.Value, isUserChange);
                                com.insert(cmd);
                            }
                            com.update(Command.GetUpdateCmd.setRSPExecutionMessage2(rspId, (string)de.Key, 2, DeviceType.CMS.ToString(), output.g_code_id, output.icon_id, 0, output.mesg));
                        }
                        else
                        {
                            cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.CMS, de.Value, isUserChange);
                            com.insert(cmd);
                            com.update(Command.GetUpdateCmd.setRSPExecutionMessage1(rspId, (string)de.Key, 2, DeviceType.CMS.ToString(), output.g_code_id, output.icon_id, 0, output.mesg));
                        }
                    }
                }

                //儲存LCS資料
                if (sender.LCSOutputData != null)
                {
                    foreach (System.Collections.DictionaryEntry de in sender.LCSOutputData)
                    {
                        if (de.Value == null) break;
                        RemoteInterface.HC.LCSOutputData output = (RemoteInterface.HC.LCSOutputData)((List<object>)de.Value)[1];
                        if (isUserChange)
                        {
                            if (com.select(DataType.OutputData, Command.GetSelectCmd.getOutputData(rspId, (string)de.Key)) != null)
                            {
                                com.update(Command.GetUpdateCmd.setRSPExecutionOutputDataCmd(evenId, (string)de.Key, (int)((List<object>)de.Value)[0], output));
                            }
                            else
                            {
                                cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.LCS, de.Value, isUserChange);
                                com.insert(cmd);
                            }
                            com.update(Command.GetUpdateCmd.setRSPExecutionMessage2(rspId, (string)de.Key, 0, DeviceType.LCS.ToString(), 0, 0, 0, getLCSDisplay(output)));
                        }
                        else
                        {
                            cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.LCS, de.Value, isUserChange);
                            com.insert(cmd);
                            com.update(Command.GetUpdateCmd.setRSPExecutionMessage1(rspId, (string)de.Key, 0, DeviceType.LCS.ToString(), 0, 0, 0, getLCSDisplay(output)));
                        }
                    }
                }


                //儲存RMS資料
                if (sender.RMSOutputData != null)
                {
                    foreach (System.Collections.DictionaryEntry de in sender.RMSOutputData)
                    {
                        if (de.Value == null) break;
                        RemoteInterface.HC.RMSOutputData output = (RemoteInterface.HC.RMSOutputData)((List<object>)de.Value)[1];
                        if (isUserChange)
                        {
                            if (com.select(DataType.OutputData, Command.GetSelectCmd.getOutputData(rspId, (string)de.Key)) != null)
                            {
                                com.update(Command.GetUpdateCmd.setRSPExecutionOutputDataCmd(evenId, (string)de.Key, (int)((List<object>)de.Value)[0], output));
                            }
                            else
                            {
                                cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.RMS, de.Value, isUserChange);
                                com.insert(cmd);
                            }
                            com.update(Command.GetUpdateCmd.setRSPExecutionMessage2(rspId, (string)de.Key, 2, DeviceType.RMS.ToString(), 0, 0, 0, EasyClient.getRMSDisplay(output)));
                        }
                        else
                        {
                            cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.RMS, de.Value, isUserChange);
                            com.insert(cmd);
                            com.update(Command.GetUpdateCmd.setRSPExecutionMessage1(rspId, (string)de.Key, 2, DeviceType.RMS.ToString(), 0, 0, 0, EasyClient.getRMSDisplay(output)));
                        }
                    }
                }

                if (sender.MASOutputData != null)
                {
                    foreach (System.Collections.DictionaryEntry de in sender.MASOutputData)
                    {
                        if (de.Value == null) break;
                        if (!isUserChange)
                        {
                            RemoteInterface.HC.MASOutputData output = (RemoteInterface.HC.MASOutputData)de.Value;
                            cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.MAS, de.Value, isUserChange);
                            com.insert(cmd);
                            com.update(Command.GetUpdateCmd.setRSPExecutionMessage1(rspId, (string)de.Key, 2, DeviceType.MAS.ToString(), 0, 0, 0, EasyClient.getMASDisplay(output)));
                        }                        
                    }
                }

                //儲存CCTV資料
                if (sender.CCTVOutputData != null)
                {
                    foreach (System.Collections.DictionaryEntry de in sender.CCTVOutputData)
                    {
                        cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.CCTV, de.Value, false);
                        com.insert(cmd);
                        //serMeg.setAlarmMeg("儲存RMS資料(未完成)");
                    }
                }

                //serMeg.setServerMeg("加入一筆事件到tblRSPExecution");
            }
            catch (System.Exception ex)
            {
                //serMeg.setAlarmMeg("加入一筆事件到tblRSPExecution錯誤!!");
                throw ex;
            }
        }
        #endregion ==== 儲存資料進入Execution table ====
    }
}
