using System;
using System.Collections.Generic;
using System.Text;
using Execution.Category;

namespace Execution.Category
{
    /// <summary>
    /// 裝飾物件的抽象類別
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
        /// 獲得顯示內容樣板方法
        /// </summary>
        /// <param name="type">事件分類</param>
        /// <returns>顯示內容</returns>
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
                case CategoryType.GEN:          //一般事件
                    {
                        RemoteInterface.HC.FetchDeviceData[] fetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, direction);
                        return setGENDisplay(fetchDevs, maxSegId);
                    }
                case CategoryType.OBS:          //壅塞事件
                    {
                        RemoteInterface.HC.FetchDeviceData[] fetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, direction);
                        return setOBSDisplay(fetchDevs, maxSegId);
                    }
                case CategoryType.WEA:          //天候事件
                    {
                        //RemoteInterface.HC.FetchDeviceData[] fetchDevs = getDeviceName(ref maxSegId);
                        //return setWEADisplay(fetchDevs, maxSegId);
                        System.Collections.Hashtable devHT = new System.Collections.Hashtable();
                        System.Collections.Hashtable displayHT = new System.Collections.Hashtable();

                        //範圍內
                        if (devType != DeviceType.RMS && devType != DeviceType.LCS)
                        {
                            devHT = setWEADisplay(getInDeviceName(ref  maxSegId, ref  devsMeg, mile_s, mile_e, lineid), -99);
                            if (devHT != null)
                            {
                                foreach (System.Collections.DictionaryEntry de in devHT)//-99為範圍內
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
                        devHT = setWEADisplay(getOutDeviceName(ref maxSegId, ref devsMeg, mile_s < mile_e ? mile_e : mile_s, lineid, dir1), maxSegId); // 8/31昭毅
                        if (devHT != null)
                        {
                            foreach (System.Collections.DictionaryEntry de in devHT)
                            {
                                    displayHT.Add(de.Key, de.Value);
                            }
                        }
                        //devHT = setWEADisplay(getOutDeviceName(ref maxSegId, ref devsMeg, mile_s > mile_e ? mile_s : mile_e, lineid, dir2), maxSegId);
                        devHT = setWEADisplay(getOutDeviceName(ref maxSegId, ref devsMeg, mile_s < mile_e ? mile_s : mile_e, lineid, dir2), maxSegId); // 8/31昭毅
                        if (devHT != null)
                        {
                            foreach (System.Collections.DictionaryEntry de in devHT)
                            {
                                displayHT.Add(de.Key, de.Value);
                            }
                        }

                        return displayHT;
                    }
                case CategoryType.TUN:          //隧道機電事件
                    {
                        RemoteInterface.HC.FetchDeviceData[] fetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, direction);
                        return setTUNDisplay(fetchDevs, maxSegId);
                    }
                case CategoryType.RES:          //管制事件
                    {
                        RemoteInterface.HC.FetchDeviceData[] fetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, direction);
                        return setRESDisplay(fetchDevs, maxSegId);
                    }
                case CategoryType.OTH:          //其他事件
                    {
                        RemoteInterface.HC.FetchDeviceData[] fetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, direction);
                        return setOTHDisplay(fetchDevs, maxSegId);
                    }
                default:
                    return null;
            }
        }

        #region ==== 獲得要顯示的設備名稱 ====
        /// <summary>
        /// 獲得範圍內要顯示的設備名稱
        /// </summary>
        /// <param name="maxSegId">最遠的路段</param>
        /// <param name="devsMeg">組合設備訊息</param>
        /// <param name="mile_s">開始里程</param>
        /// <param name="mile_e">始終里程</param>
        /// <param name="lineid">路線編號</param>
        /// <returns>設備名稱集合</returns>
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
        /// 獲得範圍外要顯示的設備名稱
        /// </summary>
        /// <param name="maxSegId">最遠的路段</param>
        /// <param name="devsMeg">組合設備訊息</param>
        /// <param name="mile">開始里程</param>
        /// <param name="lineid">路線編號</param>
        /// <param name="direction">方向</param>
        /// <returns>設備名稱集合</returns>
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
        ///// 獲得要顯示的設備名稱
        ///// </summary>
        ///// <returns>設備名稱</returns>
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
        //            devs = "路段：" + ht["INC_LOCATION"].ToString().Trim() + "，起點：" + Convert.ToInt32(ht["FROM_MILEPOST1"]) + "，終點：" + Convert.ToString(ht["TO_MILEPOST1"]).Trim() + "，";
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
        //                devs = "路段：" + ht["INC_LOCATION"].ToString().Trim() + "，起點：" + Convert.ToInt32(ht["FROM_MILEPOST1"]) + "，方向：" + direction + "，";
        //            }
        //        }
        //        else
        //        {
        //            bool isExtend = Convert.ToInt32(DevRange["ISEXTEND"]) == 1 ? true : false;
        //            if (devType == DeviceType.CSLS)
        //                isExtend = false;

        //            fetchDevs = hobj.Fetch(new string[] { devType.ToString() }, lindid, direction, mileage, Convert.ToInt32(DevRange["NORMAL"]), Convert.ToInt32(DevRange["SYSTEM"]), isExtend);
        //            devs = "路段：" + ht["INC_LOCATION"].ToString().Trim() + "，起點：" + Convert.ToInt32(ht["FROM_MILEPOST1"]) + "，方向：" + direction + "，";
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
        #endregion ==== 獲得要顯示的設備名稱 ====

        /// <summary>
        /// 設定一般事件顯示內容
        /// </summary>
        /// <param name="devNames">要顯示之設備</param>
        /// <param name="maxSegId">最遠路段的編碼</param>
        /// <returns>設備實施內容</returns>
        virtual protected System.Collections.Hashtable setGENDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId) 
        {
            int unit_Start = findUnitSection(Convert.ToInt32(ht["FROM_MILEPOST1"]), Convert.ToString(ht["INC_DIRECTION"]));
            megType = getGeneralMsgType(degree, Convert.ToInt32(ht["INC_CONGESTION"]), unit_Start);
            //this.serMeg.setServerMeg(string.Format("事件訊息型態:{0}", megType.ToString()));
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
        /// 設定壅塞事件顯示內容
        /// </summary>
        /// <param name="devNames">要顯示之設備</param>
        /// <param name="maxSegId">最遠路段的編碼</param>
        /// <returns>設備實施內容</returns>
        virtual protected System.Collections.Hashtable setOBSDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId) 
        {
            megType = getNongeneralMsgType(degree, Convert.ToInt32(ht["INC_CONGESTION"]));
            //this.serMeg.setServerMeg(string.Format("事件訊息型態:{0}", megType.ToString())); 
            return setDisplay(devNames, maxSegId, megType);
        }

        /// <summary>
        /// 設定天候事件顯示內容
        /// </summary>
        /// <param name="devNames">要顯示之設備</param>
        /// <param name="maxSegId">最遠路段的編碼</param>
        /// <returns>設備實施內容</returns>
        virtual protected System.Collections.Hashtable setWEADisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId)
        {
            megType = getNongeneralMsgType(degree, Convert.ToInt32(ht["INC_CONGESTION"]));
            //this.serMeg.setServerMeg(string.Format("事件訊息型態:{0}", megType.ToString()));
            return setDisplay(devNames, maxSegId, megType);
        }

        /// <summary>
        /// 設定隧道機電事件顯示內容
        /// </summary>
        /// <param name="devNames">要顯示之設備</param>
        /// <param name="maxSegId">最遠路段的編碼</param>
        /// <returns>設備實施內容</returns>
        virtual protected System.Collections.Hashtable setTUNDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId)
        {
            megType = getNongeneralMsgType(degree, Convert.ToInt32(ht["INC_CONGESTION"]));
            //this.serMeg.setServerMeg(string.Format("事件訊息型態:{0}", megType.ToString()));
            return setDisplay(devNames, maxSegId, megType);
        }

        /// <summary>
        /// 設定管制事件顯示內容
        /// </summary>
        /// <param name="devNames">要顯示之設備</param>
        /// <param name="maxSegId">最遠路段的編碼</param>
        /// <returns>設備實施內容</returns>
        virtual protected System.Collections.Hashtable setRESDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId)
        {
            megType = getNongeneralMsgType(degree, Convert.ToInt32(ht["INC_CONGESTION"]));
            //this.serMeg.setServerMeg(string.Format("事件訊息型態:{0}", megType.ToString()));
            return setDisplay(devNames, maxSegId, megType);
        }

        /// <summary>
        /// 設定管制事件顯示內容
        /// </summary>
        /// <param name="devNames">要顯示之設備</param>
        /// <param name="maxSegId">最遠路段的編碼</param>
        /// <returns>設備實施內容</returns>
        virtual protected System.Collections.Hashtable setOTHDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId)
        {
            megType = getNongeneralMsgType(degree, Convert.ToInt32(ht["INC_CONGESTION"]));
            //this.serMeg.setServerMeg(string.Format("事件訊息型態:{0}", megType.ToString()));
            return setDisplay(devNames, maxSegId, megType);
        }

        /// <summary>
        /// 設定所有顯示內容
        /// </summary>
        /// <param name="devNames">要顯示之設備</param>
        /// <param name="maxSegId">最遠路段的編碼</param>
        /// <param name="megType">取得一般事件訊息型態</param>
        /// <returns>設備實施內容</returns>
        virtual protected System.Collections.Hashtable setDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId, MegType megType) { return null; }

        /// <summary>
        /// 取得優先度
        /// </summary>
        /// <returns>優先度</returns>
        virtual protected int getPriority()
        {
            return -1;
        }

        /// <summary>
        /// 取得一般事件訊息型態
        /// </summary>
        /// <param name="degree">嚴重程度</param>
        /// <param name="jammed">壅塞程度</param>
        /// <returns>訊息型態</returns>
        virtual protected MegType getGeneralMsgType(Degree degree, int jammed, int unit_Start)
        {
            //serMeg.setAlarmMeg("使用Host去抓壅塞程度!!!");
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
        /// 取得非一般事件訊息型態
        /// </summary>
        /// <param name="degree">嚴重程度</param>
        /// <param name="jammed">壅塞程度</param>
        /// <returns>訊息型態</returns>
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
