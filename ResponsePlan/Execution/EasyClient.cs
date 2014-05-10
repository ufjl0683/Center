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
        /// Execution Table���A����
        /// </summary>
        /// <param name="status">���A(1.�T�{��,2.���ݤ�,3.���椤,4.����,5.����,6.���)</param>
        /// <param name="rspID">�����p���s��</param>
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
        /// ���wHost
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
                System.Diagnostics.Trace.WriteLine("Host �s�u����");
            return hobj;
        }
          /// <summary>
        /// ���o�����p��DataReader�^�Ǥ�k
        /// </summary>
        /// <param name="type">��ƫ��A</param>
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

        #region ==== ���oOutputData��ܤ��e ====
        /// <summary>
        /// ���oOutputData��ܤ��e
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
                result = string.Format("���ϡG{0},ICON�G{1},�T���G{2}", output.g_code_id, output.icon_id, output.mesg);
            }
            else if (outputData is RemoteInterface.HC.FSOutputData)
            {
                RemoteInterface.HC.FSOutputData output = (RemoteInterface.HC.FSOutputData)outputData;
                result = string.Format("�T���G{0}", getFSDisplay(output.type));
            }
            else if (outputData is RemoteInterface.HC.CSLSOutputData)
            {
                RemoteInterface.HC.CSLSOutputData output = (RemoteInterface.HC.CSLSOutputData)outputData;
                result = string.Format("�T���G{0}", output.dataset.Tables[0].Rows[0]["speed"].ToString());
            }
            else if (outputData is RemoteInterface.MFCC.RGS_GenericDisplay_Data)
            {
                RemoteInterface.MFCC.RGS_GenericDisplay_Data output = (RemoteInterface.MFCC.RGS_GenericDisplay_Data)outputData;
                byte icon1 = 0;
                byte icon2 = 0;
                byte graph_code_id = 0;
                string meg = "";
                getRGSDisplay(output, ref  graph_code_id, ref  icon1, ref  icon2, ref  meg);
                result = string.Format("���ϡG{0},ICON1�G{1},ICON2�G{2},�T���G{3}", graph_code_id, icon1, icon2, meg);
            }
            else if (outputData is RemoteInterface.HC.LCSOutputData)
            {
                RemoteInterface.HC.LCSOutputData output = (RemoteInterface.HC.LCSOutputData)outputData;
                result = string.Format("�T���G{0}", getLCSDisplay(output));
            }
            else if (outputData is RemoteInterface.HC.RMSOutputData)
            {
                RemoteInterface.HC.RMSOutputData output = (RemoteInterface.HC.RMSOutputData)outputData;
                result = string.Format("�T���G{0}", getRMSDisplay(output));
            }
            return result;
        }

        /// <summary>
        /// ���oFS��ܤ��e
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string getFSDisplay(byte type)
        {
            switch (type)
            {
                case 1:    //�@��
                    return "��";
                case 2:    //�j��
                    return "��";
                case 3:    //���B
                    return "�B";
                default:    //��L
                    return "����";
            }
        }

        /// <summary>
        /// ���oRGS��ܤ��e
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
        /// ���oLCS��ܤ��e
        /// </summary>
        /// <param name="output">LCSOutputData</param>
        /// <returns></returns>
        public static string getLCSDisplay(RemoteInterface.HC.LCSOutputData output)
        {
            string display = "";
            foreach (System.Data.DataRow row in output.dataset.Tables[1].Rows)
            {
                display += string.Format("{0}��ܤ��e:{1}   ", row["sign_no"].ToString() == "0" ? "�����D" : "�~���D", Byte2String(Convert.ToByte(row["sign_status"])));
            }
            return display;
        }

        /// <summary>
        /// ���oRMS��ܤ��e
        /// </summary>
        /// <param name="output">RMSOutputData</param>
        /// <returns></returns>
        public static string getRMSDisplay(RemoteInterface.HC.RMSOutputData output)
        {
            string display = getRmsModeString(output.mode);
            if (output.mode == 3)
                display += string.Format(",�ɨ{0}", output.planno);
            return display;
        }

        public static string getMASDisplay(RemoteInterface.HC.MASOutputData output)
        {
            StringBuilder sb = new StringBuilder();
            for(int i =0;i<output.displays.Length;i++)
            {
                if (output.displays[i] is int)
                {
                    sb.AppendFormat("��{0}���D�t��:{1} ",i+1,output.displays[i]);
                }
                else if (output.displays[i] is RemoteInterface.HC.CMSOutputData)
                {
                    RemoteInterface.HC.CMSOutputData cms = (RemoteInterface.HC.CMSOutputData)output.displays[i];
                    sb.AppendFormat("��{0}���D�ϥ�:{1} ", i + 1, cms.g_code_id);
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
                case 0: returnStr = "����"; break;
                case 1: returnStr = "��"; break;
                case 2: returnStr = "��"; break;
                case 3: returnStr = "��"; break;
                case 4: returnStr = "��"; break;
                case 5: returnStr = "��(�{�{)"; break;
                case 6: returnStr = "��(�{�{)"; break;
                case 7: returnStr = "��(�{�{)"; break;
                case 8: returnStr = "��(�{�{)"; break;
                default: returnStr = "����"; break;
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
            // 0�G�T�w�ɨ�Ҧ�
            // 1�G�ϰ��q�����Ҧ�
            // 2�G�w�]�ɨ�Ҧ�
            // 3�G�`�D����
            // 4�G��X����q�����Ҧ�
            // 5�G��ʾާ@�Ҧ��w(���Ҧ��L�k�]�w�A��TC�i�J���Ҧ��ɷ|�D
            //                  �ʦ^���B�i�H04H+80H�d��)
            // 6�G�����פ�w(���Ҧ��L�k�]�w�A��TC�i�J���Ҧ��ɷ|�D�ʦ^
            //���B�i�H04H+80H�d��)
            switch (mode)
            {
                case 0:
                    return "�T�w�ɨ�Ҧ�";
                case 1:
                    return "�ϰ��q�Ҧ�";
                case 2:
                    return "�w�]�ɨ�Ҧ�";
                case 3:
                    return "�`�D����";
                case 4:
                    return "��X����q�����Ҧ�";
                case 5:
                    return "��ʾާ@�Ҧ�";
                case 6:
                    return "�����פ�";
                default:
                    return string.Format("{0}�L���Ҧ�", mode);
            }
        }
        #endregion ==== ���oOutputData��ܤ��e ====

        #region ==== ���o�����p�� ====
        /// <summary>
        ///  ���o�����p��
        /// </summary>
        /// <param name="rspId">�����p���s��</param>
        /// <returns>�����p��</returns>
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
                //���oĵ�i�s��
                com.select(DataType.IIPEvent, Command.GetSelectCmd.getIIPEventCmd(rspId, false));
                //���o"�����p���s��","�ƥ�o�ͮɶ�","�Ƶ�","�B�z���A"
                com.select(DataType.Execution, Command.GetSelectCmd.getRspExecution(rspId));
                //���o"�B�z�����"
                com.select(DataType.Unit, Command.GetSelectCmd.getExeUnitData(rspId, exeObj.AlarmClass));
                //���o"�]�ƿ�X���"
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
        /// ���o�����p��DataReader�^�Ǥ�k
        /// </summary>
        /// <param name="type">��ƫ��A</param>
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
        #endregion ==== ���o�����p�� ====

        #region ==== �x�s��ƶi�JExecution table ====
        /// <summary>
        /// �x�s��ƶi�JExecution table
        /// </summary>
        /// <param name="sender"></param>
        public void saveExecution(string rspId, int evenId, ExecutionObj sender, bool isUserChange)
        {      
            try
            {
                //�x�s"�����p���s��","�ƥ�o�ͮɶ�","�Ƶ�","�B�z���A"
                InsertCommand cmd;
                if (!isUserChange)
                {
                    cmd = Command.GetInsertCmd.setRSPExecutionCmd(sender);
                    com.insert(cmd);
                }

                //�x�s�B�z�����
                if (!isUserChange && com.Select(string.Format("Select * from {0}.{1} where INC_ID = '{2}';", RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblIIPService, rspId)).Rows.Count == 0)
                {   //�ϥΪ̤w�]�w���s
                    foreach (LiaiseUnit unit in sender.Units)
                    {
                        if (unit == null) break;
                        if (isUserChange)
                        {//�Y�����D�{���϶�
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

                //�x�sWIS���
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

                //�x�sFS���
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

                //�x�sCSLS���
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

                //�x�sRGS���
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

                //�x�sCMS���
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

                //�x�sLCS���
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


                //�x�sRMS���
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

                //�x�sCCTV���
                if (sender.CCTVOutputData != null)
                {
                    foreach (System.Collections.DictionaryEntry de in sender.CCTVOutputData)
                    {
                        cmd = Command.GetInsertCmd.setRSPExecutionOutputDataCmd(rspId, evenId, (string)de.Key, DeviceType.CCTV, de.Value, false);
                        com.insert(cmd);
                        //serMeg.setAlarmMeg("�x�sRMS���(������)");
                    }
                }

                //serMeg.setServerMeg("�[�J�@���ƥ��tblRSPExecution");
            }
            catch (System.Exception ex)
            {
                //serMeg.setAlarmMeg("�[�J�@���ƥ��tblRSPExecution���~!!");
                throw ex;
            }
        }
        #endregion ==== �x�s��ƶi�JExecution table ====
    }
}
