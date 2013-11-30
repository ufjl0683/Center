using System;
using System.Collections.Generic;
using System.Text;
using DBConnect;

namespace Execution.Category
{
    /// <summary>
    /// ��H�ƥ����O
    /// </summary>
    internal abstract class AEvent
    {
        protected System.Collections.Hashtable ht;
        protected CategoryType type;            //�ƥ����O
        protected byte secType;
        protected ODBC_DB2Connect com = null;           
        private int mode = 2;
        
        virtual protected void Initialize(System.Collections.Hashtable ht, CategoryType type)
        {
            this.type = type;
            this.secType = Convert.ToByte(ht["INC_NAME"]);
            this.ht = ht;
            if (ht["INC_BLOCKAGE"].ToString() == "") ht["INC_BLOCKAGE"] = "0000000000";//�p�G�S��"���_���D�r��"�A�w�]��"0000000000"
            this.com = new DBConnect.ODBC_DB2Connect();
        }

        /// <summary>
        /// �����p���D�n��k�]������^
        /// </summary>
        /// <param name="sender">�b�۰ʩM��ʽs���Ȧs��Hashtable</param>
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
        /// ��o��ܩҦ���Ƹ��
        /// </summary>
        /// <param name="sender">��ܩҦ���Ƹ��</param>
        /// <returns>��ܩҦ���Ƹ��</returns>
        public virtual ExecutionObj produceExecution(ExecutionObj sender)
        {
            com.GetReaderData += new GetReaderDataHandler(com_GetReaderData);
            sender.EventTime = DateTime.Parse(ht["INC_TIME"].ToString());
            sender.Memo += "==>>Get Unit";
            List<LiaiseUnit> units = new List<LiaiseUnit>();

            foreach (object obj in getDisplayContent(Convert.ToByte(ht["INC_NAME"]), Convert.ToInt32(ht["FROM_MILEPOST1"]), Convert.ToInt32(ht["TO_MILEPOST1"])))
            {
                if (obj != null) units.Add((LiaiseUnit)obj);
            }
            sender.Units = units;
            return sender;
        }

        /// <summary>
        /// ��o�s�������
        /// </summary>
        /// <param name="type">�ƥ󦸤���</param>
        /// <returns>�s�������</returns>
        private List<object> getDisplayContent(byte secType, int start_mile,int end_mile)
        {
            try
            {
                List<object> units = (List<object>)com.select(DataType.Unit, Command.GetSelectCmd.getUnitData(secType, start_mile, end_mile));
                com.GetReaderData -= new GetReaderDataHandler(com_GetReaderData);
                return units;
            }
            catch//(Exception ex)
            {
                throw new Exception("��o�s������ƥ���");
            }
        }

        /// <summary>
        /// Select��ƪ�DataReader�^��
        /// </summary>
        /// <param name="type">��ƫ��A</param>
        /// <param name="dr">DataReader</param>
        object com_GetReaderData(DBConnect.DataType type, object reader)
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
        /// �˹��̤u�t
        /// </summary>
        /// <returns></returns>
        protected virtual AEvent factory()
        {
            AEvent myObj = this;
            com.GetReaderData += new GetReaderDataHandler(com_GetReaderData);
            Degree degree = getDegree();
            //this.serMeg.setServerMeg(string.Format("�ƥ��Y���{�׬�:{0}", degree.ToString()));
            //this.serMeg.setServerMeg(string.Format("�ƥ�ö�{�׬�:{0}", ht["INC_CONGESTION"].ToString()));  
            if (degree != Degree.N)
            {
                List<object> Decorators = (List<object>)com.select(DataType.Decorators, Command.GetSelectCmd.getDisplyDevTypes(degree, secType, ht["INC_LOCATION"].ToString().Trim()));
                com.GetReaderData -= new GetReaderDataHandler(com_GetReaderData);
                List<DeviceType> adorns = new List<DeviceType>();
                foreach (object obj in Decorators)
                {
                    System.Collections.Hashtable decHT = (System.Collections.Hashtable)obj;
                    if (Convert.ToInt32(decHT["DEVSTART"]) == 1)
                        adorns.Add((DeviceType)Enum.Parse(typeof(DeviceType), decHT["DEVICETYPE"].ToString()));
                }
                return getAdorns(adorns, myObj, Decorators, degree);
            }
            else
                return null;
        }

        /// <summary>
        /// �˹��̤u�t�ϥΤ�k
        /// </summary>
        /// <param name="adorns">�˹�����O�_�˹�</param>
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
                    default:
                        myObj = new CCTV(myObj, type, ht, DeviceType.CCTV, (System.Collections.Hashtable)Decorators[i], degree);
                        break;
                }
            }
            return myObj;
        }

        /// <summary>
        /// ��o�Y���{��
        /// </summary>
        /// <returns>�Y���{��</returns>
        protected virtual Degree getDegree()
        {
            if (!string.IsNullOrEmpty(ht["INC_SERVERITY"].ToString()) && ht["INC_SERVERITY"].ToString() != "0"  )
            {
                return (Degree)Enum.Parse(typeof(Degree), ht["INC_SERVERITY"].ToString());
            }
            else
            {
                List<object> list = (List<object>)com.select(DBConnect.DataType.LaneCount, Command.GetSelectCmd.getSeriousByAlarmClass(secType));
                if (list.Count == 0)
                {
                    //this.serMeg.setAlarmMeg("�ƥ��Y���{�װѼƪ�P�_�S���{��(�Ьd���q���D�ƬO�_�M���_���D�ƹ���)!!!");
                    return Degree.N;
                }
                System.Collections.Hashtable myht = (System.Collections.Hashtable)list[0];

                switch (myht[(int)ht["INC_CONGESTION"]].ToString())
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
                            //this.serMeg.setAlarmMeg("�ƥ��Y���{�װѼƪ�P�_�S���{��(�Ьd���q���D�ƬO�_�M���_���D�ƹ���)!!!");
                            return Degree.N;
                        }
                }
            }
        }
    }
}
