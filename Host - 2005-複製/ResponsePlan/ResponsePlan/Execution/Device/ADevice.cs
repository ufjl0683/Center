using System;
using System.Collections.Generic;
using System.Text;
using Execution.Category;

namespace Execution.Category
{
    /// <summary>
    /// �˹����󪺩�H���O
    /// </summary>
    internal abstract class ADevice : AEvent
    {
        protected AEvent aDevice;
        protected MegType megType;
        protected DeviceType devType;
        protected System.Collections.Hashtable DevRange;
        protected Degree degree;
        protected delegate object GetMessRuleDataHandler(DBConnect.DataType type, object dr);
        protected int mode = 2;
        protected Tunnel tunnel;

        protected void Initialize(AEvent aDevice, CategoryType type, System.Collections.Hashtable ht, DeviceType devType, System.Collections.Hashtable DevRange,Degree degree)
        {
            this.type = type;
            this.secType = Convert.ToByte(ht["INC_NAME"]);
            this.aDevice = aDevice;
            this.ht = ht;
            this.com = new DBConnect.ODBC_DB2Connect();
            this.devType = devType;
            this.DevRange = DevRange;
            this.degree = degree;
            com.GetReaderData += new DBConnect.GetReaderDataHandler(com_GetMessRuleData);
        }

        /// <summary>
        /// ��o��ܤ��e�˪O��k
        /// </summary>
        /// <param name="type">�ƥ����</param>
        /// <returns>��ܤ��e</returns>
        virtual protected System.Collections.Hashtable getDisplayContent(CategoryType type)
        {
            int maxSegId = 0;
            string devsMeg = "";
            int mile_s = Convert.ToInt32(ht["FROM_MILEPOST1"]);
            int mile_e = Convert.ToInt32(ht["TO_MILEPOST1"]);
            string lineid = Convert.ToString(ht["INC_LINEID"]).Trim();
            string direction = Convert.ToString(ht["INC_DIRECTION"]).Trim();

            switch (type)
            {
                case CategoryType.GEN:          //�@��ƥ�
                    {
                        RemoteInterface.HC.FetchDeviceData[] fetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, direction);
                        return setGENDisplay(fetchDevs, maxSegId);
                    }
                case CategoryType.OBS:          //�ö�ƥ�
                    {
                        RemoteInterface.HC.FetchDeviceData[] fetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, direction);
                        return setOBSDisplay(fetchDevs, maxSegId);
                    }
                case CategoryType.WEA:          //�ѭԨƥ�
                    {
                        //RemoteInterface.HC.FetchDeviceData[] fetchDevs = getDeviceName(ref maxSegId);
                        //return setWEADisplay(fetchDevs, maxSegId);
                        System.Collections.Hashtable devHT = new System.Collections.Hashtable();
                        System.Collections.Hashtable displayHT = new System.Collections.Hashtable();

                        //�d��
                        if (devType != DeviceType.RMS && devType != DeviceType.LCS)
                        {
                            devHT = setWEADisplay(getInDeviceName(ref  maxSegId, ref  devsMeg, mile_s, mile_e, lineid), -99);
                            if (devHT != null)
                            {
                                foreach (System.Collections.DictionaryEntry de in devHT)//-99���d��
                                {
                                    displayHT.Add(de.Key, de.Value);
                                }
                            }
                        }

                        string dir1 = "";
                        string dir2 = "";
                        if (ht["LINE_DIRECTION"].ToString() == "NS")
                        {
                            dir1 = "N";
                            dir2 = "S";
                        }
                        else
                        {
                            dir1 = "W";
                            dir2 = "E";
                        }
                        //devHT = setWEADisplay(getOutDeviceName(ref maxSegId, ref devsMeg, mile_s > mile_e ? mile_e : mile_s, lineid, dir1), maxSegId);
                        devHT = setWEADisplay(getOutDeviceName(ref maxSegId, ref devsMeg, mile_s < mile_e ? mile_e : mile_s, lineid, dir1), maxSegId); // 8/31�L��
                        if (devHT != null)
                        {
                            foreach (System.Collections.DictionaryEntry de in devHT)
                            {
                                    displayHT.Add(de.Key, de.Value);
                            }
                        }
                        //devHT = setWEADisplay(getOutDeviceName(ref maxSegId, ref devsMeg, mile_s > mile_e ? mile_s : mile_e, lineid, dir2), maxSegId);
                        devHT = setWEADisplay(getOutDeviceName(ref maxSegId, ref devsMeg, mile_s < mile_e ? mile_s : mile_e, lineid, dir2), maxSegId); // 8/31�L��
                        if (devHT != null)
                        {
                            foreach (System.Collections.DictionaryEntry de in devHT)
                            {
                                displayHT.Add(de.Key, de.Value);
                            }
                        }

                        return displayHT;
                    }
                case CategoryType.TUN:          //�G�D���q�ƥ�
                    {
                        RemoteInterface.HC.FetchDeviceData[] fetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, direction);
                        return setTUNDisplay(fetchDevs, maxSegId);
                    }
                case CategoryType.RES:          //�ި�ƥ�
                    {
                        RemoteInterface.HC.FetchDeviceData[] fetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, direction);
                        return setRESDisplay(fetchDevs, maxSegId);
                    }
                case CategoryType.OTH:          //��L�ƥ�
                    {
                        RemoteInterface.HC.FetchDeviceData[] fetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, direction);
                        return setOTHDisplay(fetchDevs, maxSegId);
                    }
                default:
                    return null;
            }
        }

        #region ==== ��o�n��ܪ��]�ƦW�� ====
        /// <summary>
        /// ��o�d�򤺭n��ܪ��]�ƦW��
        /// </summary>
        /// <param name="maxSegId">�̻������q</param>
        /// <param name="devsMeg">�զX�]�ưT��</param>
        /// <param name="mile_s">�}�l���{</param>
        /// <param name="mile_e">�l�ר��{</param>
        /// <param name="lineid">���u�s��</param>
        /// <returns>�]�ƦW�ٶ��X</returns>
        virtual protected RemoteInterface.HC.FetchDeviceData[] getInDeviceName(ref int maxSegId,ref string devsMeg,int mile_s,int mile_e, string lineid)
        {
            RemoteInterface.HC.FetchDeviceData[] fetchDevs = null;
            try
            {
                RemoteInterface.HC.I_HC_FWIS hobj = EasyClient.getHost();
                //Host fuction
                fetchDevs = hobj.Fetch(new string[] { devType.ToString() }, lineid, mile_s, mile_e);

                if (fetchDevs == null) return fetchDevs;
                foreach (RemoteInterface.HC.FetchDeviceData fetchDev in fetchDevs)
                {
                    devsMeg += fetchDev.DevName + "<<==";
                    if (maxSegId < fetchDev.SegId) maxSegId = fetchDev.SegId;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return fetchDevs;
        }

        /// <summary>
        /// ��o�d��~�n��ܪ��]�ƦW��
        /// </summary>
        /// <param name="maxSegId">�̻������q</param>
        /// <param name="devsMeg">�զX�]�ưT��</param>
        /// <param name="mile">�}�l���{</param>
        /// <param name="lineid">���u�s��</param>
        /// <param name="direction">��V</param>
        /// <returns>�]�ƦW�ٶ��X</returns>
        virtual protected RemoteInterface.HC.FetchDeviceData[] getOutDeviceName(ref int maxSegId, ref string devsMeg, int mile, string lineid, string direction)
        {
            RemoteInterface.HC.FetchDeviceData[] fetchDevs = null;
            try
            {
                RemoteInterface.HC.I_HC_FWIS hobj = EasyClient.getHost();
                
                if (devType == DeviceType.LCS)
                {
                    tunnel = TunnelData.getBuilder().tunList.Find(new EventObj(lineid, direction, mile));
                    if (tunnel != null)
                    {
                        int startMileage = 0;
                        if (tunnel.UpstreamLocation != null)
                            startMileage = tunnel.UpstreamLocation.EndMileage;
                        else
                        {
                            if (tunnel.Direction == "E" || tunnel.Direction == "S")
                            {
                                startMileage = tunnel.Line.StartMileage;
                            }
                            else
                            {
                                startMileage = tunnel.Line.EndMileage;
                            }
                        }
                        //Host fuction
                        fetchDevs = hobj.Fetch(new string[] { devType.ToString() }, lineid, direction, startMileage, tunnel.EndMileage);
                    }
                }
                else
                {
                    bool isExtend = Convert.ToInt32(DevRange["ISEXTEND"]) == 1 ? true : false;
                    if (devType == DeviceType.CSLS)
                        isExtend = false;
                    //Host fuction
                    fetchDevs = hobj.Fetch(new string[] { devType.ToString() }, lineid, direction, mile, Convert.ToInt32(DevRange["NORMAL"]), Convert.ToInt32(DevRange["SYSTEM"]), isExtend);
                }

                if (fetchDevs == null) return fetchDevs;
                foreach (RemoteInterface.HC.FetchDeviceData fetchDev in fetchDevs)
                {
                    devsMeg += fetchDev.DevName + "<<==";
                    if (maxSegId < fetchDev.SegId) maxSegId = fetchDev.SegId;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return fetchDevs;
        }

        
        ///// <summary>
        ///// ��o�n��ܪ��]�ƦW��
        ///// </summary>
        ///// <returns>�]�ƦW��</returns>
        //virtual protected RemoteInterface.HC.FetchDeviceData[] getDeviceName(ref int maxSegId)
        //{
        //    RemoteInterface.HC.FetchDeviceData[] fetchDevs = null;
        //    try
        //    {
        //        RemoteInterface.HC.I_HC_FWIS hobj = EasyClient.getHost();

        //        string devs = "";
        //        int mileage = Convert.ToInt32(ht["FROM_MILEPOST1"]);
        //        string lindid = Convert.ToString(ht["INC_LINEID"]).Trim();
        //        string direction = Convert.ToString(ht["INC_DIRECTION"]).Trim();
        //        string sectionid = Convert.ToString(ht["SECTIONID"]).Trim();

        //        if (devType == DeviceType.WIS)
        //        {
        //            fetchDevs = hobj.Fetch(new string[] { devType.ToString() }, lindid, mileage, Convert.ToInt32(ht["TO_MILEPOST1"]));
        //            devs = "���q�G" + ht["INC_LOCATION"].ToString().Trim() + "�A�_�I�G" + Convert.ToInt32(ht["FROM_MILEPOST1"]) + "�A���I�G" + Convert.ToString(ht["TO_MILEPOST1"]).Trim() + "�A";
        //        }
        //        else if (devType == DeviceType.LCS)
        //        {
        //            tunnel = TunnelData.getBuilder().tunList.Find(new EventObj(lindid, direction, mileage));
        //            if (tunnel != null)
        //            {
        //                int startMileage = 0;
        //                if (tunnel.UpstreamLocation != null)
        //                    startMileage = tunnel.UpstreamLocation.EndMileage;
        //                else
        //                {
        //                    if (tunnel.Direction == "E" || tunnel.Direction == "S")
        //                    {
        //                        startMileage = tunnel.Line.StartMileage;
        //                    }
        //                    else
        //                    {
        //                        startMileage = tunnel.Line.EndMileage;
        //                    }
        //                }
        //                fetchDevs = hobj.Fetch(new string[] { devType.ToString() }, lindid, direction, startMileage, tunnel.EndMileage);
        //                devs = "���q�G" + ht["INC_LOCATION"].ToString().Trim() + "�A�_�I�G" + Convert.ToInt32(ht["FROM_MILEPOST1"]) + "�A��V�G" + direction + "�A";
        //            }
        //        }
        //        else
        //        {
        //            bool isExtend = Convert.ToInt32(DevRange["ISEXTEND"]) == 1 ? true : false;
        //            if (devType == DeviceType.CSLS)
        //                isExtend = false;

        //            fetchDevs = hobj.Fetch(new string[] { devType.ToString() }, lindid, direction, mileage, Convert.ToInt32(DevRange["NORMAL"]), Convert.ToInt32(DevRange["SYSTEM"]), isExtend);
        //            devs = "���q�G" + ht["INC_LOCATION"].ToString().Trim() + "�A�_�I�G" + Convert.ToInt32(ht["FROM_MILEPOST1"]) + "�A��V�G" + direction + "�A";
        //        }
        //        if (fetchDevs == null) return fetchDevs;
        //        foreach (RemoteInterface.HC.FetchDeviceData fetchDev in fetchDevs)
        //        {
        //            devs += fetchDev.DevName + "<<==";
        //            if (maxSegId < fetchDev.SegId) maxSegId = fetchDev.SegId;
        //        }
        //        //serMeg.setServerMeg(devs.TrimEnd(new char[] { '=', '<' }));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return fetchDevs;
        //}
        #endregion ==== ��o�n��ܪ��]�ƦW�� ====

        /// <summary>
        /// �]�w�@��ƥ���ܤ��e
        /// </summary>
        /// <param name="devNames">�n��ܤ��]��</param>
        /// <param name="maxSegId">�̻����q���s�X</param>
        /// <returns>�]�ƹ�I���e</returns>
        virtual protected System.Collections.Hashtable setGENDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId) 
        {
            int unit_Start = findUnitSection(Convert.ToInt32(ht["FROM_MILEPOST1"]), Convert.ToString(ht["INC_DIRECTION"]));
            megType = getGeneralMsgType(degree, Convert.ToInt32(ht["INC_CONGESTION"]), unit_Start);
            //this.serMeg.setServerMeg(string.Format("�ƥ�T�����A:{0}", megType.ToString()));
            return setDisplay(devNames, maxSegId, megType);
        }

        private int findUnitSection(int startMileage, string direction)
        {
            int unit_Start=0;
            int mileage = startMileage / 1000;
            int remainder = startMileage % 1000;
            if (direction == "N" || direction == "W")//+
            {
                if (remainder > 200)
                {
                    unit_Start = mileage - 1;
                }
                else
                {
                    unit_Start = mileage;
                }
            }
            else
            {
                if (remainder > 800)
                {
                    unit_Start = mileage;
                }
                else
                {
                    unit_Start = mileage - 1;
                }
            }
            return unit_Start;
        }

        /// <summary>
        /// �]�w�ö�ƥ���ܤ��e
        /// </summary>
        /// <param name="devNames">�n��ܤ��]��</param>
        /// <param name="maxSegId">�̻����q���s�X</param>
        /// <returns>�]�ƹ�I���e</returns>
        virtual protected System.Collections.Hashtable setOBSDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId) 
        {
            megType = getNongeneralMsgType(degree, Convert.ToInt32(ht["INC_CONGESTION"]));
            //this.serMeg.setServerMeg(string.Format("�ƥ�T�����A:{0}", megType.ToString())); 
            return setDisplay(devNames, maxSegId, megType);
        }

        /// <summary>
        /// �]�w�ѭԨƥ���ܤ��e
        /// </summary>
        /// <param name="devNames">�n��ܤ��]��</param>
        /// <param name="maxSegId">�̻����q���s�X</param>
        /// <returns>�]�ƹ�I���e</returns>
        virtual protected System.Collections.Hashtable setWEADisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId)
        {
            megType = getNongeneralMsgType(degree, Convert.ToInt32(ht["INC_CONGESTION"]));
            //this.serMeg.setServerMeg(string.Format("�ƥ�T�����A:{0}", megType.ToString()));
            return setDisplay(devNames, maxSegId, megType);
        }

        /// <summary>
        /// �]�w�G�D���q�ƥ���ܤ��e
        /// </summary>
        /// <param name="devNames">�n��ܤ��]��</param>
        /// <param name="maxSegId">�̻����q���s�X</param>
        /// <returns>�]�ƹ�I���e</returns>
        virtual protected System.Collections.Hashtable setTUNDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId)
        {
            megType = getNongeneralMsgType(degree, Convert.ToInt32(ht["INC_CONGESTION"]));
            //this.serMeg.setServerMeg(string.Format("�ƥ�T�����A:{0}", megType.ToString()));
            return setDisplay(devNames, maxSegId, megType);
        }

        /// <summary>
        /// �]�w�ި�ƥ���ܤ��e
        /// </summary>
        /// <param name="devNames">�n��ܤ��]��</param>
        /// <param name="maxSegId">�̻����q���s�X</param>
        /// <returns>�]�ƹ�I���e</returns>
        virtual protected System.Collections.Hashtable setRESDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId)
        {
            megType = getNongeneralMsgType(degree, Convert.ToInt32(ht["INC_CONGESTION"]));
            //this.serMeg.setServerMeg(string.Format("�ƥ�T�����A:{0}", megType.ToString()));
            return setDisplay(devNames, maxSegId, megType);
        }

        /// <summary>
        /// �]�w�ި�ƥ���ܤ��e
        /// </summary>
        /// <param name="devNames">�n��ܤ��]��</param>
        /// <param name="maxSegId">�̻����q���s�X</param>
        /// <returns>�]�ƹ�I���e</returns>
        virtual protected System.Collections.Hashtable setOTHDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId)
        {
            megType = getNongeneralMsgType(degree, Convert.ToInt32(ht["INC_CONGESTION"]));
            //this.serMeg.setServerMeg(string.Format("�ƥ�T�����A:{0}", megType.ToString()));
            return setDisplay(devNames, maxSegId, megType);
        }

        /// <summary>
        /// �]�w�Ҧ���ܤ��e
        /// </summary>
        /// <param name="devNames">�n��ܤ��]��</param>
        /// <param name="maxSegId">�̻����q���s�X</param>
        /// <param name="megType">���o�@��ƥ�T�����A</param>
        /// <returns>�]�ƹ�I���e</returns>
        virtual protected System.Collections.Hashtable setDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId, MegType megType) { return null; }

        /// <summary>
        /// ���o�u����
        /// </summary>
        /// <returns>�u����</returns>
        virtual protected int getPriority()
        {
            return -1;
        }

        /// <summary>
        /// ���o�@��ƥ�T�����A
        /// </summary>
        /// <param name="degree">�Y���{��</param>
        /// <param name="jammed">�ö�{��</param>
        /// <returns>�T�����A</returns>
        virtual protected MegType getGeneralMsgType(Degree degree, int jammed, int unit_Start)
        {
            //serMeg.setAlarmMeg("�ϥ�Host�h��ö�{��!!!");
            RemoteInterface.HC.I_HC_FWIS hobj = EasyClient.getHost();
            if (jammed == 0)
            {
                try
                {
                    int volume = 0;
                    int speed = 0;
                    int occupancy = 0;
                    int jameLevel = 0;
                    int travelSec = 0;
                    string[] vdList = null;
                    hobj.GetAllTrafficDataByUnit(ht["INC_LINEID"].ToString(), ht["INC_DIRECTION"].ToString(), unit_Start, ref  volume, ref  speed, ref  occupancy, ref  jameLevel, ref  travelSec, ref  vdList);
                    if (jameLevel != -1)
                        jammed = jameLevel;
                }
                catch
                {
                }
            }

            List<object> list = (List<object>)com.select(DBConnect.DataType.MessRule, Command.GetSelectCmd.getMessRule(degree.ToString(), 0));
            if (list.Count == 0) return MegType.A;
            System.Collections.Hashtable myht = (System.Collections.Hashtable)list[0];
            if (myht.Contains(jammed))
                return (MegType)Enum.Parse(typeof(MegType), myht[jammed].ToString());
            else
                return MegType.A;
        }

        /// <summary>
        /// ���o�D�@��ƥ�T�����A
        /// </summary>
        /// <param name="degree">�Y���{��</param>
        /// <param name="jammed">�ö�{��</param>
        /// <returns>�T�����A</returns>
        virtual protected MegType getNongeneralMsgType(Degree degree, int jammed)
        {
            List<object> list = (List<object>)com.select(DBConnect.DataType.MessRule, Command.GetSelectCmd.getMessRule(degree.ToString(), 1));
            if (list.Count == 0) return MegType.A;
            System.Collections.Hashtable myht = (System.Collections.Hashtable)list[0];
            if (myht.Contains(jammed))
                return (MegType)Enum.Parse(typeof(MegType), myht[jammed].ToString() == "-" ? "A" : myht[jammed].ToString());
            else
                return MegType.A;
        }

        protected event GetMessRuleDataHandler GetMessRuleData;
        object com_GetMessRuleData(DBConnect.DataType type, object reader)
        {
            if (type == DBConnect.DataType.MessRule)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                System.Collections.Hashtable laneCount = new System.Collections.Hashtable();
                laneCount.Add(0, Convert.ToString(dr[3]));
                laneCount.Add(1, Convert.ToString(dr[4]));
                laneCount.Add(2, Convert.ToString(dr[5]));
                laneCount.Add(3, Convert.ToString(dr[6]));
                laneCount.Add(4, Convert.ToString(dr[7]));
                laneCount.Add(5, Convert.ToString(dr[8]));
                laneCount.Add(6, Convert.ToString(dr[9]));
                laneCount.Add(7, Convert.ToString(dr[10]));
                laneCount.Add(8, Convert.ToString(dr[11]));
                laneCount.Add(9, Convert.ToString(dr[12]));
                laneCount.Add(10, Convert.ToString(dr[13]));
                laneCount.Add(11, Convert.ToString(dr[14]));
                laneCount.Add(12, Convert.ToString(dr[15]));
                laneCount.Add(13, Convert.ToString(dr[16]));
                laneCount.Add(14, Convert.ToString(dr[17]));
                return laneCount;
            }
            else if (type == DBConnect.DataType.EndSectionName)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                return dr[0].ToString();
            }
            else if (type == DBConnect.DataType.Priority)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                return Convert.ToInt32(dr[2]);
            }
            else
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                if (GetMessRuleData != null)
                {
                    return GetMessRuleData(type, dr);
                }
                else
                    return null;
            }
        }
    }
}
