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

            if (isNear && (int)DevRange["NORMAL"] > 0) //交接路段顯示範圍 - 1
            {
                DevRange["NORMAL"] = (int)DevRange["NORMAL"] - 1;
            }          
            
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
                        
                        //範圍內
                        //System.Collections.Hashtable DevHTIn;
                        //if (mile_s != mile_e)
                        //{
                        //    DevHTIn = new System.Collections.Hashtable();
                        //    if (devType != DeviceType.RMS && devType != DeviceType.LCS)
                        //    {
                        //        DevHTIn = setGENDisplay(getInDeviceName(ref  maxSegId, ref  devsMeg, mile_s, mile_e, lineid), -99);                 
                        //    }
                        //}

                        if (direction.Length > 1)
                        {
                            #region 雙向
                            RemoteInterface.HC.FetchDeviceData[] fetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, direction[0].ToString());
                            System.Collections.Hashtable DevHT1 = setGENDisplay(fetchDevs, maxSegId, direction[0].ToString());
                            if (DevHT1 == null)
                                DevHT1 = new System.Collections.Hashtable();
                            fetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, direction[1].ToString());
                            System.Collections.Hashtable DevHT2 = setGENDisplay(fetchDevs, maxSegId, direction[1].ToString());
                            if (DevHT2 != null)
                            {
                                foreach (System.Collections.DictionaryEntry Dev in DevHT2)
                                {
                                    if (!DevHT1.Contains(Dev.Key))
                                        DevHT1.Add(Dev.Key, Dev.Value);                                    
                                }
                            }
                            if (mile_s != mile_e )//&& (int)ht["INC_NAME"] == 31) //施工範圍
                            {
                                RemoteInterface.HC.I_HC_FWIS hobj = EasyClient.getHost();
                                if (devType != DeviceType.LCS)//(devType != DeviceType.RMS && devType != DeviceType.LCS)
                                {
                                    fetchDevs = hobj.Fetch(new string[] { devType.ToString() }, lineid, direction[0].ToString(), mile_s, mile_e);
                                    foreach (RemoteInterface.HC.FetchDeviceData dev in fetchDevs)
                                    {
                                        dev.SegId = -99;
                                    }
                                    DevHT2 = setGENDisplay(fetchDevs,maxSegId,direction[0].ToString());
                                    foreach (System.Collections.DictionaryEntry Dev in DevHT2)
                                    {
                                        if (!DevHT1.Contains(Dev.Key))
                                        {
                                            DevHT1.Add(Dev.Key, Dev.Value);
                                        }
                                    }
                                    fetchDevs = hobj.Fetch(new string[] { devType.ToString() }, lineid, direction[1].ToString(), mile_s, mile_e);
                                    foreach (RemoteInterface.HC.FetchDeviceData dev in fetchDevs)
                                    {
                                        dev.SegId = -99;
                                    }
                                    DevHT2 = setGENDisplay(fetchDevs,maxSegId,direction[1].ToString());
                                    foreach (System.Collections.DictionaryEntry Dev in DevHT2)
                                    {
                                        if (!DevHT1.Contains(Dev.Key))
                                        {
                                            DevHT1.Add(Dev.Key, Dev.Value);
                                        }
                                    }

                                    if (devType == DeviceType.CMS || devType == DeviceType.RGS)
                                    {
                                        DevHT2 = setGENDisplay(GetRangeBrachCMS(lineid, mile_s, mile_e), maxSegId, direction);

                                        foreach (System.Collections.DictionaryEntry Dev in DevHT2)
                                        {
                                            if (!DevHT1.Contains(Dev.Key))
                                            {
                                                DevHT1.Add(Dev.Key, Dev.Value);
                                            }
                                        }
                                    }                                   
                                }
                            }
                            //if (DevHTIn != null)
                            //{
                            //    foreach (System.Collections.DictionaryEntry Dev in DevHTIn)
                            //    {
                            //        if (!DevHT1.Contains(Dev.Key))
                            //            DevHT1.Add(Dev.Key, Dev.Value);
                            //    }
                            //}
                            return DevHT1;
                            #endregion
                        }
                        else
                        {
                            RemoteInterface.HC.FetchDeviceData[] fetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, direction);
                            System.Collections.Hashtable DevHT1 = setGENDisplay(fetchDevs, maxSegId, direction);                            
                            if (mile_s != mile_e )//&& (int)ht["INC_NAME"] == 31) //施工範圍
                            {
                                RemoteInterface.HC.I_HC_FWIS hobj = EasyClient.getHost();
                                //if (devType != DeviceType.LCS)//(devType != DeviceType.RMS && devType != DeviceType.LCS)
                                //{
                                    System.Collections.Hashtable DevHT2 = new System.Collections.Hashtable();
                                    fetchDevs = hobj.Fetch(new string[] { devType.ToString() }, lineid, direction, mile_s, mile_e);
                                    if (devType == DeviceType.LCS) //LCS 需驅動終點隧道後面設備
                                    {
                                        RemoteInterface.HC.FetchDeviceData[] tmpfetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, mile_e, lineid, direction);
                                        if (tmpfetchDevs != null)
                                        {
                                            fetchDevs = tmpfetchDevs;
                                        }
                                        //int tmpSegId = maxSegId;
                                        //string tmpDevsMeg = devsMeg;
                                        //RemoteInterface.HC.FetchDeviceData[] fetchDevs2 = getOutDeviceName(ref tmpSegId, ref tmpDevsMeg, mile_e, lineid, direction);
                                        //if (fetchDevs2 != null)
                                        //{
                                        //    System.Collections.Generic.Dictionary<string, RemoteInterface.HC.FetchDeviceData> tmpDict = new Dictionary<string, RemoteInterface.HC.FetchDeviceData>(fetchDevs.Length + fetchDevs2.Length);
                                        //    for (int i = 0; i < fetchDevs.Length; i++)
                                        //    {
                                        //        if (!tmpDict.ContainsKey(fetchDevs[i].DevName))
                                        //        {
                                        //            tmpDict.Add(fetchDevs[i].DevName, fetchDevs[i]);
                                        //        }
                                        //    }
                                        //    for (int i = 0; i < fetchDevs2.Length; i++)
                                        //    {
                                        //        if (!tmpDict.ContainsKey(fetchDevs2[i].DevName))
                                        //        {
                                        //            tmpDict.Add(fetchDevs2[i].DevName, fetchDevs2[i]);
                                        //        }
                                        //    }
                                        //    fetchDevs = new RemoteInterface.HC.FetchDeviceData[tmpDict.Count];
                                        //    int j = 0;
                                        //    foreach (System.Collections.Generic.KeyValuePair<string, RemoteInterface.HC.FetchDeviceData> tmpDev in tmpDict)
                                        //    {
                                        //        fetchDevs[j++] = tmpDev.Value;
                                        //    }
                                        //}
                                    }
                                    
                                    foreach (RemoteInterface.HC.FetchDeviceData dev in fetchDevs)
                                    {
                                        dev.SegId = -99;
                                    }
                                    
                                    DevHT2 = setGENDisplay(fetchDevs, maxSegId, direction);
                                    if (DevHT1 == null)
                                        DevHT1 = new System.Collections.Hashtable();

                                    foreach (System.Collections.DictionaryEntry Dev in DevHT2)
                                    {
                                        if (!DevHT1.Contains(Dev.Key))
                                        {
                                            DevHT1.Add(Dev.Key, Dev.Value);
                                        }
                                    }

                                    if (devType == DeviceType.CMS || devType == DeviceType.RGS)
                                    {
                                        DevHT2 = setGENDisplay(GetRangeBrachCMS(lineid, mile_s, mile_e), maxSegId, direction);

                                        foreach (System.Collections.DictionaryEntry Dev in DevHT2)
                                        {
                                            if (!DevHT1.Contains(Dev.Key))
                                            {
                                                DevHT1.Add(Dev.Key, Dev.Value);
                                            }
                                        }
                                    }
                                //}
                            }
                            return DevHT1;
                        }
                    }
                case CategoryType.OBS:          //壅塞事件
                    {
                        RemoteInterface.HC.FetchDeviceData[] fetchDevs;
                        if ((int)ht["INC_NAME"] == 133 && devType != DeviceType.CCTV) //入口匝道壅塞
                        {
                            if (devType == DeviceType.CMS)
                            {
                                string where = string.Format("mileage > {0} and mileage < {1}", mile_s - 500, mile_s + 500);
                                string cmd = string.Format("Select mileage,DivisionType,DivisionID From {0}.{1} where LineID = '{2}' and DivisionType in ('I','C') and {3}  fetch first 1 rows only; "
                                    , RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblGroupDivision, lineid,where);
                                System.Data.DataTable DT = com.Select(cmd);

                                if (DT.Rows.Count == 0 )//
                                {
                                    return setOBSDisplay(new RemoteInterface.HC.FetchDeviceData[0], maxSegId);
                                }
                                else if ((string)DT.Rows[0][1] != "I")//系統交流道
                                {
                                    string DivisionID = (string)DT.Rows[0][2];
                                    cmd = string.Format("select LineId2,mileage2,direction2 from {0}.VWCLOVERLEAF where DivisionType = 'C' "
                                        + "and LineID1 = '{1}' and Mileage1 = {2} and Direction1 = '{3}';", RSPGlobal.GlobaSchema, lineid, DT.Rows[0][0], direction);
                                    DT = com.Select(cmd);
                                    if (DT.Rows.Count == 0)
                                    {
                                        return setOBSDisplay(new RemoteInterface.HC.FetchDeviceData[0], maxSegId);
                                    }
                                    else
                                    {
                                        System.Collections.Hashtable DevHT = new System.Collections.Hashtable();
                                        foreach (System.Data.DataRow dr in DT.Rows)
                                        {
                                            fetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, (int)dr[1], (string)dr[0], (string)dr[2]);
                                            System.Collections.Hashtable tmpDevHT = setOBSDisplay(fetchDevs, maxSegId);

                                            foreach (System.Collections.DictionaryEntry Dev in tmpDevHT)
                                            {
                                                if (!DevHT.Contains(Dev.Key))
                                                {
                                                    DevHT.Add(Dev.Key, Dev.Value);
                                                }
                                            }
                                        }
                                        return DevHT;
                                    }
                                }
                                else//交流道
                                {
                                    fetchDevs = LoadLCMS(lineid, direction, mile_s);
                                    return setOBSDisplay(fetchDevs, maxSegId);
                                }
                            }                                
                            
                        }
                       
                        fetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, direction);
                        if ((devType == DeviceType.CMS || devType == DeviceType.RMS || devType == DeviceType.CCTV) && mile_s != mile_e)
                        {
                            RemoteInterface.HC.I_HC_FWIS hobj = EasyClient.getHost();
                            RemoteInterface.HC.FetchDeviceData[] fetchDevsIn = hobj.Fetch(new string[] { devType.ToString() }, lineid, direction, mile_s, mile_e);
                            System.Data.DataTable DT = RSPGlobal.GetDeviceDT();
                            for (int i = 0; i < fetchDevsIn.Length; i++)
                            {
                                System.Data.DataRow dr = DT.Rows.Find(fetchDevsIn[i].DevName);
                                if (dr != null)
                                {
                                    fetchDevsIn[i].Location = dr[RSPGlobal.Location].ToString();
                                    fetchDevsIn[i].SegId = -99;
                                }
                            }
                            List<RemoteInterface.HC.FetchDeviceData> fetchList = new List<RemoteInterface.HC.FetchDeviceData>(fetchDevs.Length + fetchDevsIn.Length);
                            fetchList.AddRange(fetchDevs);
                            fetchList.AddRange(fetchDevsIn);
                            return setOBSDisplay(fetchList.ToArray(), maxSegId);
                        }
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

                        string dir1 = string.Empty;
                        string dir2 = string.Empty;
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
                                if (!displayHT.Contains(de.Key))
                                    displayHT.Add(de.Key, de.Value);
                            }
                        }
                        //devHT = setWEADisplay(getOutDeviceName(ref maxSegId, ref devsMeg, mile_s > mile_e ? mile_s : mile_e, lineid, dir2), maxSegId);
                        devHT = setWEADisplay(getOutDeviceName(ref maxSegId, ref devsMeg, mile_s < mile_e ? mile_s : mile_e, lineid, dir2), maxSegId); // 8/31昭毅
                        if (devHT != null)
                        {
                            foreach (System.Collections.DictionaryEntry de in devHT)
                            {
                                if (!displayHT.Contains(de.Key))
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
                case CategoryType.RMS: //匝道儀控
                    {
                        if (devType == DeviceType.CMS)
                        {
                            string cmd = string.Format("Select cfg.DeviceName,cfg.Mile_M,section.MaxSpeed,section.MinSpeed From {0}.{1} cfg ,{0}.{4} section "
                                    + " where cfg.LineID = '{2}' and cfg.device_Type = 'CMS'  and cfg.Location = 'L' "
                                    + " and cfg.mile_m > ({3} - 500) and cfg.mile_m < ({3} + 500) and cfg.sectionID = section.sectionID;"
                                    , RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblDeviceConfig, lineid, mile_s, DBConnect.DB2TableName.tblGroupSection);
                            System.Data.DataTable DT = com.Select(cmd);
                            RemoteInterface.HC.FetchDeviceData[] fetchDevs = new RemoteInterface.HC.FetchDeviceData[DT.Rows.Count];
                            int j = 0;
                            foreach (System.Data.DataRow dr in DT.Rows)
                            {
                                fetchDevs[j] = new RemoteInterface.HC.FetchDeviceData((string)dr[0], 0, lineid, direction, (int)dr[1], (int)dr[2], (int)dr[3], "RMS");
                                fetchDevs[j].Location = "L";
                                j++;
                            }
                            return setRESDisplay(fetchDevs, maxSegId);
                        }
                        else
                        {
                            return setRESDisplay(new RemoteInterface.HC.FetchDeviceData[0], 0);
                        }
                    }
                case CategoryType.RGS:
                    {
                        RemoteInterface.HC.FetchDeviceData[] fetchDevs = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, direction);
                        return setRESDisplay(fetchDevs, maxSegId);
                    }
                    
                case CategoryType.TUNFire:
                    {
                        RemoteInterface.HC.FetchDeviceData[] fetchDevs1, fetchDevs2;
                        List<RemoteInterface.HC.FetchDeviceData> fetchDevs = new List<RemoteInterface.HC.FetchDeviceData>();          
                        switch (direction)
                        {
                            //case "N":
                            //case "S":
                            //    fetchDevs1 = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, "N");
                            //    fetchDevs2 = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, "S");
                            //    break;
                            default:
                                fetchDevs1 = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, "W");
                                fetchDevs2 = getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, "E");
                                break;
                        }
                        if (fetchDevs1 != null)
                            fetchDevs.AddRange(fetchDevs1);
                        if (fetchDevs2 != null)
                            fetchDevs.AddRange(fetchDevs2);
                        return setTUNDisplay(fetchDevs.ToArray(), maxSegId);
                    }
                case CategoryType.PARK:
                    {
                        List<RemoteInterface.HC.FetchDeviceData> DevList = new List<RemoteInterface.HC.FetchDeviceData>();
                        DevList.AddRange(getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, "N"));
                        DevList.AddRange(getOutDeviceName(ref maxSegId, ref devsMeg, mile_s, lineid, "S"));
                        return setOTHDisplay(DevList.ToArray(), maxSegId);                       
                    }

                default:
                    return null;
            }
        }

        #region ==== 獲得要顯示的設備名稱 ====

        private RemoteInterface.HC.FetchDeviceData[] GetRangeBrachCMS(string Line, int mile_s, int mile_e)
        {
            if ((int)DevRange["NORMAL"] == 0)
            {
                return new RemoteInterface.HC.FetchDeviceData[0];
            }

            if (mile_s > mile_e)
            {
                int k = mile_e;
                mile_e = mile_s;
                mile_s = k;
            }
            
            RemoteInterface.HC.I_HC_FWIS hobj = EasyClient.getHost();
            System.Data.DataTable DivDT = RSPGlobal.GetDivisionDT();
            System.Data.DataTable CloverleafDT = RSPGlobal.GetCloverleafDT();
            System.Collections.Hashtable tmpHT = new System.Collections.Hashtable();
            //List<System.Data.DataRow> DRList = new List<System.Data.DataRow>();
            foreach (System.Data.DataRow dr in DivDT.Rows)
            {
                if ((string)dr[1] == "C" && (string)dr[3] == Line && (int)dr[2] >= mile_s && (int)dr[2] <= mile_e)
                {
                    int k = 0;
                    foreach (System.Data.DataRow CloverDR in CloverleafDT.Rows)
                    {
                        if ((string)CloverDR[0] == Line && (int)CloverDR[1] == (int)dr[2])
                        {
                            k++;
                            RemoteInterface.HC.FetchDeviceData[] fetDevs 
                                = hobj.Fetch(new string[] { "CMS","RGS" }, (string)CloverDR[2], (string)CloverDR[4], (int)CloverDR[3], (int)DevRange["NORMAL"] - 1, 0, false);
                            foreach (RemoteInterface.HC.FetchDeviceData dev in fetDevs)
                            {                        
                                if (!tmpHT.Contains(dev.DevName))
                                {
                                    tmpHT.Add(dev.DevName, dev);
                                }
                                if (dev.SegId == (int)DevRange["NORMAL"] -1)
                                    break;
                            }
                            if (k == 2)
                            {
                                break;
                            }                        
                        }
                    }
                }
            }

            List<RemoteInterface.HC.FetchDeviceData> DevList = new List<RemoteInterface.HC.FetchDeviceData>(tmpHT.Count);
            foreach (System.Collections.DictionaryEntry de in tmpHT)
            {
                if (((RemoteInterface.HC.FetchDeviceData)de.Value).DeviceType == devType.ToString())
                {
                    DevList.Add((RemoteInterface.HC.FetchDeviceData)de.Value);
                }
            }
            return DevList.ToArray();
        }


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
                if (mile_s == mile_e)
                {
                    fetchDevs = new RemoteInterface.HC.FetchDeviceData[0];
                    return fetchDevs;
                }

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
            //try
            //{
                RemoteInterface.HC.I_HC_FWIS hobj = EasyClient.getHost();

                if (!string.IsNullOrEmpty(ht["INC_INTERCHANGE"].ToString()) && !ht["INC_INTERCHANGE"].ToString().Contains("系統交流道"))  //匝道入口
                {
                    string tmpStr = (string)ht["INC_INTERCHANGE"];
                    if (tmpStr.Length > 1 && tmpStr.Substring(tmpStr.Length - 2).Equals("入口"))
                    {
                        if (devType != DeviceType.CMS)
                        {
                            return new RemoteInterface.HC.FetchDeviceData[0];
                        }
                        else
                        {
                            fetchDevs = LoadNearLCMS(lineid, mile);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(ht["BLOCKTYPEID"].ToString()))
                {
                    int blocktypeID = Convert.ToInt32(ht["BLOCKTYPEID"]);
                    if (blocktypeID == 6)
                    {
                        if (devType != DeviceType.CMS)
                        {
                            return new RemoteInterface.HC.FetchDeviceData[0];
                        }
                        else
                        {
                            fetchDevs = LoadNearLCMS(lineid, mile);
                        }
                    }                    
                }              
                

                switch (devType)
                {
                    case DeviceType.LCS:                        
                        tunnel = TunnelData.getBuilder().tunList.Find(new EventObj(lineid, direction, mile));
                        if (tunnel != null)
                        {
                            int startMileage = 0;
                            //if (tunnel.UpstreamLocation != null)
                            //    startMileage = tunnel.UpstreamLocation.EndMileage;
                            //else
                            //{
                            if (tunnel.Direction == "E" || tunnel.Direction == "S")
                            {
                                startMileage = tunnel.Line.StartMileage;
                            }
                            else
                            {
                                startMileage = tunnel.Line.EndMileage;
                            }
                            //}                            

                            //Host fuction
                            fetchDevs = hobj.Fetch(new string[] { devType.ToString() }, lineid, direction, startMileage, tunnel.EndMileage);
                        }
                        break;
                    case DeviceType.CMS:
                        {
                            if (fetchDevs != null) //已載入入口CMS
                                break;

                            string DevDir = string.Empty;
                            bool isExtend = Convert.ToInt32(DevRange["ISEXTEND"]) == 1 ? true : false;
                            //Host fuction
                            if ((int)DevRange["NORMAL"] == 0 && !isNear)
                            {
                                fetchDevs = new RemoteInterface.HC.FetchDeviceData[0];
                            }
                            else
                            {
                                if (lineid != "T74甲" || direction != "W")
                                {
                                    fetchDevs = hobj.Fetch(new string[] { devType.ToString(), DeviceType.RGS.ToString() }, lineid, direction, mile, Convert.ToInt32(DevRange["NORMAL"]), Convert.ToInt32(DevRange["SYSTEM"]), isExtend);
                                }
                                else
                                    fetchDevs = new RemoteInterface.HC.FetchDeviceData[0];
                            }                    
                             

                            System.Data.DataTable DT = RSPGlobal.GetDeviceDT();
                            for (int i = 0; i < fetchDevs.Length; i++)
                            {
                                System.Data.DataRow dr = DT.Rows.Find(fetchDevs[i].DevName);
                                if (dr != null)
                                {
                                    fetchDevs[i].Location = dr[RSPGlobal.Location].ToString();
                                }
                            }

                            List<RemoteInterface.HC.FetchDeviceData> oldFetchDevs = new List<RemoteInterface.HC.FetchDeviceData>();
                            int MaxSeg = 0;
                            foreach (RemoteInterface.HC.FetchDeviceData dev in fetchDevs)
                            {
                                if (dev.SegId > MaxSeg)
                                {
                                    MaxSeg = dev.SegId;
                                }
                            }

                            #region RGS排序
                            foreach (RemoteInterface.HC.FetchDeviceData Dev in fetchDevs) //RGS排序
                            {
                                if (Dev.DeviceType == "CMS")
                                {
                                    oldFetchDevs.Add(Dev);
                                }
                                else
                                {
                                    if (Dev.SegId == (int)DevRange["NORMAL"] || Dev.SegId == (int)DevRange["SYSTEM"])
                                    {
                                        for (int j = 0; j < oldFetchDevs.Count; j++)
                                        {
                                            if (oldFetchDevs[j].LineId == Dev.LineId && Dev.SegId == oldFetchDevs[j].SegId && Dev.Direction == oldFetchDevs[j].Direction)
                                            {
                                                if (Dev.Direction == "S" || Dev.Direction == "E")
                                                {
                                                    if (Dev.Mileage < oldFetchDevs[j].Mileage)
                                                    {
                                                        oldFetchDevs.Insert(j, Dev);
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    if (Dev.Mileage > oldFetchDevs[j].Mileage)
                                                    {
                                                        oldFetchDevs.Insert(j, Dev);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            fetchDevs = oldFetchDevs.ToArray();
                            #endregion

                            oldFetchDevs.Clear();
                            string DevDirN = string.Empty;
                            string DevLine = string.Empty;
                            DevDir = string.Empty;

                            string DevDirNS = string.Empty, DevLineS = string.Empty;
                            int DevSeg = 0;

                            for (int i = 0; i < fetchDevs.Length; i++)
                            {
                                //if (fetchDevs[i].Location != "L") //移除平面道路 || fetchDevs[i].LineId == lineid) //移除非主線平面道路
                                //{
                                if ((int)DevRange["SYSTEM"] > 0)
                                {
                                    if (fetchDevs[i].SegId <= (int)DevRange["NORMAL"])
                                    {
                                        if (fetchDevs[i].SegId == (int)DevRange["NORMAL"])
                                        {
                                            if (DevDirN != fetchDevs[i].Direction || DevLine != fetchDevs[i].LineId)
                                            {
                                                if (fetchDevs[i].Location != "L")
                                                {
                                                    oldFetchDevs.Add(fetchDevs[i]);
                                                    DevDirN = fetchDevs[i].Direction;
                                                    DevLine = fetchDevs[i].LineId;
                                                }
                                                if (DevDirNS != fetchDevs[i].Direction || DevLineS != fetchDevs[i].LineId || DevSeg != fetchDevs[i].SegId)
                                                {
                                                    oldFetchDevs.AddRange(LoadLCMS(fetchDevs[i].LineId, fetchDevs[i].Direction, fetchDevs[i].Mileage));
                                                    DevDirNS = fetchDevs[i].Direction;
                                                    DevLineS = fetchDevs[i].LineId;
                                                    DevSeg = fetchDevs[i].SegId;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            oldFetchDevs.Add(fetchDevs[i]);
                                            if (DevDirNS != fetchDevs[i].Direction || DevLineS != fetchDevs[i].LineId || DevSeg != fetchDevs[i].SegId)
                                            {
                                                oldFetchDevs.AddRange(LoadLCMS(fetchDevs[i].LineId, fetchDevs[i].Direction, fetchDevs[i].Mileage));
                                                DevDirNS = fetchDevs[i].Direction;
                                                DevLineS = fetchDevs[i].LineId;
                                                DevSeg = fetchDevs[i].SegId;
                                            }
                                        }
                                    }
                                    else if (fetchDevs[i].SegId == MaxSeg)  // 系統不需載入平面
                                    {
                                        if ((DevDir != fetchDevs[i].Direction || DevLine != fetchDevs[i].LineId) && fetchDevs[i].Location != "L")
                                        {
                                            oldFetchDevs.Add(fetchDevs[i]);
                                            DevDir = fetchDevs[i].Direction;
                                            DevLine = fetchDevs[i].LineId;
                                        }
                                    }
                                }
                                else
                                {
                                    if (fetchDevs[i].SegId < (int)DevRange["NORMAL"])
                                    {
                                        oldFetchDevs.Add(fetchDevs[i]);
                                        //for add LCMS 2011.03.14
                                        //DevDir = fetchDevs[i].Direction;
                                        //DevLine = fetchDevs[i].LineId;
                                        //
                                        if (DevDirNS != fetchDevs[i].Direction || DevLineS != fetchDevs[i].LineId || DevSeg != fetchDevs[i].SegId)
                                        {
                                            oldFetchDevs.AddRange(LoadLCMS(fetchDevs[i].LineId, fetchDevs[i].Direction, fetchDevs[i].Mileage));
                                            DevDirNS = fetchDevs[i].Direction;
                                            DevLineS = fetchDevs[i].LineId;
                                            DevSeg = fetchDevs[i].SegId;
                                        }
                                    }
                                    else
                                    {
                                        if (DevDir != fetchDevs[i].Direction || DevLine != fetchDevs[i].LineId)
                                        {
                                            if (fetchDevs[i].Location != "L")
                                            {
                                                oldFetchDevs.Add(fetchDevs[i]);
                                                DevDir = fetchDevs[i].Direction;
                                                DevLine = fetchDevs[i].LineId;
                                            }
                                            if (DevDirNS != fetchDevs[i].Direction || DevLineS != fetchDevs[i].LineId || DevSeg != fetchDevs[i].SegId)
                                            {
                                                oldFetchDevs.AddRange(LoadLCMS(fetchDevs[i].LineId, fetchDevs[i].Direction, fetchDevs[i].Mileage));
                                                DevDirNS = fetchDevs[i].Direction;
                                                DevLineS = fetchDevs[i].LineId;
                                                DevSeg = fetchDevs[i].SegId;
                                            }
                                        }
                                    }
                                }
                                //}
                            }

                            for (int i = 0; i < oldFetchDevs.Count; ) //過濾CMS以外設備
                            {
                                if (oldFetchDevs[i].DeviceType != "CMS")
                                {
                                    oldFetchDevs.Remove(oldFetchDevs[i]);
                                }
                                else
                                {
                                    i++;
                                }
                            }

                            try     //服務區
                            {
                                string cmd = string.Format("Select s.divisionid,div.lineid,div.mileage from {0}.{1} s,{0}.{5} div Where s.LineID = '{2}' and s.Direction = '{3}' and "
                                    + " ((s.Start_mile >= {4} and s.END_Mile <= {4}) or (s.Start_mile <= {4} and s.End_Mile >= {4})) and s.divisionid = div.divisionid;"
                                    , RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblRSPServiceRange, lineid, direction, mile, DBConnect.DB2TableName.tblGroupDivision);
                                DT = com.Select(cmd);
                                foreach (System.Data.DataRow dr in DT.Rows)
                                {
                                    if ((string)dr[1] == "N1") //LineID
                                    {
                                        int N1_Mile;
                                        if (lineid == "N1")
                                        {
                                            N1_Mile = mile;
                                        }
                                        else
                                        {
                                            cmd = string.Format("Select mileage1 from {0}.{1} where LineID1 = 'N1' and LineID2 = '{2}';"
                                                , RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblCloverleaf, lineid);
                                            System.Data.DataTable TmpDT = com.Select(cmd);
                                            N1_Mile = (int)TmpDT.Rows[0][0];
                                        }

                                        string EventDir;
                                        if ((int)dr[2] > N1_Mile)
                                        {
                                            EventDir = "N";
                                        }
                                        else
                                        {
                                            EventDir = "S";
                                        }
                                        if (lineid == "N1" && EventDir != direction)
                                        {
                                            continue;
                                        }

                                        cmd = string.Format("select cfg.DeviceName,cfg.LineID,cfg.Direction,cfg.mile_M from {0}.{1} cfg , {0}.{2} sec "
                                         + " where (sec.START_DIVISIONID = '{3}' or sec.END_DIVISIONID = '{3}') "
                                         + " and sec.SECTIONID = cfg.SECTIONID and cfg.LOCATION = 'S' and cfg.Device_Type = 'CMSRST' and cfg.Direction = '{4}';"
                                         , RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblDeviceConfig, DBConnect.DB2TableName.tblGroupSection, dr[0], EventDir);

                                    }
                                    else
                                    {
                                        cmd = string.Format("select cfg.DeviceName,cfg.LineID,cfg.Direction,cfg.mile_M from {0}.{1} cfg , {0}.{2} sec "
                                            + " where (sec.START_DIVISIONID = '{3}' or sec.END_DIVISIONID = '{3}') "
                                            + " and sec.SECTIONID = cfg.SECTIONID and cfg.LOCATION = 'S' and cfg.Device_Type = 'CMSRST';", RSPGlobal.GlobaSchema
                                            , DBConnect.DB2TableName.tblDeviceConfig, DBConnect.DB2TableName.tblGroupSection, dr[0]);
                                    }
                                    System.Data.DataTable CMSDT = com.Select(cmd);
                                    foreach (System.Data.DataRow CMSdr in CMSDT.Rows)
                                    {
                                        RemoteInterface.HC.FetchDeviceData newDev
                                            = new RemoteInterface.HC.FetchDeviceData((string)CMSdr[0], 0, (string)CMSdr[1], (string)CMSdr[2], (int)CMSdr[3], 90, 30, "CMS");
                                        newDev.Location = "S";
                                        oldFetchDevs.Add(newDev);
                                    }
                                }
                            }
                            catch
                            {
                                //載入服務區CMS時發生例外
                            }

                            if (ht["INC_NAME"].Equals(43))
                            {
                                fetchDevs = LoadNearLCMS(lineid, mile);
                                foreach (RemoteInterface.HC.FetchDeviceData dev in fetchDevs)
                                {
                                    for (int k = 0; k < oldFetchDevs.Count; k++)
                                    {
                                        if (oldFetchDevs[k].DevName == dev.DevName)
                                        {
                                            oldFetchDevs.RemoveAt(k);
                                            k--;
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (!string.IsNullOrEmpty(ht["INC_INTERCHANGE"].ToString()))  //匝道出口
                            {
                                string tmpStr = (string)ht["INC_INTERCHANGE"];
                                if (tmpStr.Length > 1 && tmpStr.Substring(tmpStr.Length - 2).Equals("出口"))
                                {
                                    fetchDevs = LoadNearLCMS(lineid, mile);
                                    foreach (RemoteInterface.HC.FetchDeviceData dev in fetchDevs)
                                    {
                                        for (int k = 0; k < oldFetchDevs.Count; k++)
                                        {
                                            if (oldFetchDevs[k].DevName == dev.DevName)
                                            {
                                                oldFetchDevs.RemoveAt(k);
                                                k--;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (!string.IsNullOrEmpty(ht["BLOCKTYPEID"].ToString()))
                            {
                                int blocktypeID = Convert.ToInt32(ht["BLOCKTYPEID"]);
                                if ( blocktypeID == 5)
                                {
                                    fetchDevs = LoadNearLCMS(lineid, mile);
                                    foreach (RemoteInterface.HC.FetchDeviceData dev in fetchDevs)
                                    {
                                        for (int k = 0; k < oldFetchDevs.Count; k++)
                                        {
                                            if (oldFetchDevs[k].DevName == dev.DevName)
                                            {
                                                oldFetchDevs.RemoveAt(k);
                                                k--;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            if (lineid == "N1" && direction == "N" && ((mile > 174200 && mile < 192800) || ((int)ht["TO_MILEPOST1"] > 174200 && (int)ht["TO_MILEPOST1"] < 192800)))
                            {
                                oldFetchDevs.Add(new RemoteInterface.HC.FetchDeviceData("CMS-N3-S-196.7", 5, "N3", "S", 196700, 100, 60, "CMS"));
                                oldFetchDevs.Add(new RemoteInterface.HC.FetchDeviceData("CMS-N3-S-196.8", 5, "N3", "S", 196850, 100, 60, "CMS"));
                            }
                            else
                            {
                                for (int i = 0; i < oldFetchDevs.Count;i++ )
                                {
                                    if (oldFetchDevs[i].DevName == "CMS-N3-S-196.7" || oldFetchDevs[i].DevName == "CMS-N3-S-196.8")
                                    {
                                        oldFetchDevs.RemoveAt(i);
                                        i--;
                                        continue;
                                    }
                                }
                            }

                            //for (int i = 0; i < oldFetchDevs.Count; ) //過濾CMS以外設備
                            //{
                            //    System.Data.DataRow dr = DT.Rows.Find(oldFetchDevs[i].DevName);
                            //    if (dr == null)
                            //    {
                            //        oldFetchDevs.Remove(oldFetchDevs[i]);
                            //    }
                            //    else
                            //    {
                            //        if (((string)dr[2]) != "CMS")
                            //        {
                            //            oldFetchDevs.Remove(oldFetchDevs[i]);
                            //        }
                            //        else
                            //        {
                            //            i++;
                            //        }
                            //    }
                            //}
                            fetchDevs = oldFetchDevs.ToArray();
                            /////
                            //上游平面道路所屬所有CMS
                            //try
                            //{
                            //string where;
                            //if (direction == "S" || direction == "E")
                            //{
                            //    where = string.Format(" mileage <= {0} order by mileage desc", mile);
                            //}
                            //else
                            //{
                            //    where = " mileage >= " + mile.ToString() + " order by mileage asc ";
                            //}

                            //string cmd = string.Format("Select mileage From {0}.{1} where LineID = '{2}' and {3} fetch first 1 rows only; "
                            //    , RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblGroupDivision, lineid, where);

                            //DT = com.Select(cmd);
                            //int DivMile = (int)DT.Rows[0][0];

                            //cmd = string.Format("Select cfg.DeviceName,cfg.Mile_M,section.MaxSpeed,section.MinSpeed From {0}.{1} cfg ,{0}.{4} section " 
                            //    + " where cfg.LineID = '{2}' and cfg.device_Type = 'CMS'  and cfg.Location = 'L' " 
                            //    + " and cfg.mile_m > ({3} - 200) and cfg.mile_m < ({3} + 200) and cfg.sectionID = section.sectionID;"
                            //    , RSPGlobal.GlobaSchema,DBConnect.DB2TableName.tblDeviceConfig,lineid,DivMile,DBConnect.DB2TableName.tblGroupSection);
                            //DT = com.Select(cmd);


                            //fetchDevs = new RemoteInterface.HC.FetchDeviceData[oldFetchDevs.Count + DT.Rows.Count];
                            //for (int i = 0; i < oldFetchDevs.Count; i++)
                            //{
                            //    fetchDevs[i] = oldFetchDevs[i];
                            //}

                            //int j = 0;
                            //foreach (System.Data.DataRow dr in DT.Rows)
                            //{
                            //    fetchDevs[oldFetchDevs.Count + j] = new RemoteInterface.HC.FetchDeviceData((string)dr[0], 0, lineid, direction, (int)dr[1], (int)dr[2], (int)dr[3]);
                            //    fetchDevs[oldFetchDevs.Count + j].Location = "L";
                            //    j++;
                            //}
                            //    fetchDevs = oldFetchDevs.ToArray();

                            //}
                            //catch(Exception ex)
                            //{
                            //    throw ex;
                            //}
                            /////

                        }
                        break;
                    case DeviceType.CSLS:
                    case DeviceType.MAS:
                        List<RemoteInterface.HC.FetchDeviceData> Devs = new List<RemoteInterface.HC.FetchDeviceData>();
                        fetchDevs = hobj.Fetch(new string[] { devType.ToString() }, lineid, direction, mile, 5, 0, false);
                        Devs.AddRange(fetchDevs);

                        int mile_e = (int)ht["TO_MILEPOST1"];
                        int mile_o = mile_e;
                        switch (direction) // 需更改為讀取資料庫
                        {
                            case "N":
                            case "W":
                                mile_o = mile_e - 2000;
                                break;
                            case "S":
                            case "E":
                                mile_o = mile_e + 2000;
                                break;
                        }
                        fetchDevs = hobj.Fetch(new string[] { devType.ToString() }, lineid, direction, mile_e, mile_o);
                        if (fetchDevs.Length > 0)
                        {
                            fetchDevs[0].Location = "D";
                            Devs.Insert(0, fetchDevs[0]);
                        }
                        fetchDevs = Devs.ToArray();
                        break;
                    //case DeviceType.FS:  無影響區域外設備
                    //    break;
                    case DeviceType.RGS:
                        {
                            if (type == CategoryType.RGS)
                            {
                                fetchDevs
                                    = new RemoteInterface.HC.FetchDeviceData[] { new RemoteInterface.HC.FetchDeviceData(ht["INC_NOTIFY_PLANT"].ToString(), 0, lineid, direction, mile, 100, 40, "RGS") };
                            }
                            else
                            {
                                bool isExtend = Convert.ToInt32(DevRange["ISEXTEND"]) == 1 ? true : false;
                                if ((int)DevRange["NORMAL"] == 0)
                                {
                                    fetchDevs = new RemoteInterface.HC.FetchDeviceData[0];
                                }
                                else
                                {
                                    fetchDevs = hobj.Fetch(new string[] { devType.ToString() }, lineid, direction, mile, Convert.ToInt32(DevRange["NORMAL"]), Convert.ToInt32(DevRange["SYSTEM"]), isExtend);
                                }
                                int MaxSeg = 0;
                                foreach (RemoteInterface.HC.FetchDeviceData dev in fetchDevs)
                                {
                                    if (dev.SegId > MaxSeg)
                                    {
                                        MaxSeg = dev.SegId;
                                    }
                                }
                                if ((int)DevRange["SYSTEM"] > 0)
                                {
                                    string DevDir = string.Empty;
                                    string DevLine = string.Empty;
                                    List<RemoteInterface.HC.FetchDeviceData> FiltDevs = new List<RemoteInterface.HC.FetchDeviceData>(fetchDevs.Length);
                                    for (int i = 0; i < fetchDevs.Length; i++)
                                    {
                                        if (fetchDevs[i].SegId < (int)DevRange["NORMAL"])
                                        {
                                            FiltDevs.Add(fetchDevs[i]);
                                        }
                                        else
                                        {
                                            if (fetchDevs[i].SegId == MaxSeg && (fetchDevs[i].Direction != DevDir || fetchDevs[i].LineId != DevLine))
                                            {
                                                FiltDevs.Add(fetchDevs[i]);
                                                DevDir = fetchDevs[i].Direction;
                                                DevLine = fetchDevs[i].LineId;
                                            }
                                        }
                                    }
                                    fetchDevs = FiltDevs.ToArray();
                                }
                            }
                        }
                        break;
                    case DeviceType.RMS:
                        fetchDevs = hobj.Fetch(new string[] { devType.ToString() }, lineid, direction, mile, 1, 0, false);//
                        if (fetchDevs.Length > 1)
                        {
                            fetchDevs = new RemoteInterface.HC.FetchDeviceData[1] { fetchDevs[0] };
                        }
                        break;
                    //case DeviceType.WIS: 無影響區域外設備
                    //    break;
                    case DeviceType.CCTV:
                        {
                            //List<RemoteInterface.HC.FetchDeviceData> CCTVList = new List<RemoteInterface.HC.FetchDeviceData>(2);
                            //string cmd = string.Format("select Top 1 camera_name,cast(milepost as int) as mile from vw_user_CCTV "
                            //    + " where category_name = '{0}' and cast(milepost as int) >= {1} and cast(milepost as int) < {2} order by cast(milepost as int)", lineid, mile,mile + 3000);
                            //DBConnect.ODBC_SQLServerConnect SQLconn = new DBConnect.ODBC_SQLServerConnect();
                            //System.Data.DataTable DT = SQLconn.Select(cmd);
                            //if (DT != null && DT.Rows.Count > 0)
                            //{
                            //    CCTVList.Add(new RemoteInterface.HC.FetchDeviceData((string)DT.Rows[0][0], 0, lineid, direction, (int)DT.Rows[0][1], 90, 30, "CCTV"));
                            //}
                            //cmd = string.Format("select Top 1 camera_name,cast(milepost as int) as mile from vw_user_CCTV "
                            //    + " where category_name = '{0}' and cast(milepost as int) <= {1} and cast(milepost as int) > {2} order by cast(milepost as int) desc", lineid, mile,mile - 3000);
                            //DT = SQLconn.Select(cmd);
                            //if (DT != null && DT.Rows.Count > 0)
                            //{
                            //    CCTVList.Add(new RemoteInterface.HC.FetchDeviceData((string)DT.Rows[0][0], 0, lineid, direction, (int)DT.Rows[0][1], 90, 30, "CCTV"));
                            //}

                            //fetchDevs = CCTVList.ToArray();

                            List<RemoteInterface.HC.FetchDeviceData> CCTVList = new List<RemoteInterface.HC.FetchDeviceData>(2);

                            string cmd = string.Format("Select Camera_Name,mile_m from {0}.{1} where category_name = '{2}' and mile_m > {3} and mile_m <{4} order by mile_m fetch first 1 rows only;"
                                , RSPGlobal.GlobaSchema, "TBLCCTVCONFIG", lineid, mile, mile + 3000);
                            System.Data.DataTable DT = com.Select(cmd);
                            if (DT != null && DT.Rows.Count > 0)
                            {
                                CCTVList.Add(new RemoteInterface.HC.FetchDeviceData((string)DT.Rows[0][0], 0, lineid, direction, (int)DT.Rows[0][1], 90, 30, "CCTV"));
                            }
                            cmd = string.Format("Select Camera_Name,mile_m from {0}.{1} where category_name = '{2}' and mile_m > {3} and mile_m <{4} order by mile_m desc fetch first 1 rows only;"
                            , RSPGlobal.GlobaSchema, "TBLCCTVCONFIG", lineid, mile - 3000, mile);
                            DT = com.Select(cmd);
                            if (DT != null && DT.Rows.Count > 0)
                            {
                                CCTVList.Add(new RemoteInterface.HC.FetchDeviceData((string)DT.Rows[0][0], 0, lineid, direction, (int)DT.Rows[0][1], 90, 30, "CCTV"));
                            }

                            //System.Data.DataRow dr = RSPGlobal.GetLineNameDT().Rows.Find(lineid);
                            //int StartMile = (int)dr[2];
                            //int EndMile = (int)dr[3];

                            //StartMile = StartMile > mile - 3000 ? StartMile : mile - 3000;
                            //EndMile = EndMile < mile + 3000 ? EndMile : mile + 3000;

                            //fetchDevs = hobj.Fetch(new string[] { "CCTV" }, lineid, mile, StartMile);
                            //if (fetchDevs.Length > 0)
                            //    CCTVList.Add(fetchDevs[0]);
                            //fetchDevs = hobj.Fetch(new string[] { "CCTV" }, lineid, mile, EndMile);
                            //if (fetchDevs.Length > 0)
                            //    CCTVList.Add(fetchDevs[0]);
                            fetchDevs = CCTVList.ToArray();
                        }
                        break;
                }
            

                if (fetchDevs == null) return fetchDevs;
                foreach (RemoteInterface.HC.FetchDeviceData fetchDev in fetchDevs)
                {
                    devsMeg += fetchDev.DevName + "<<==";
                    if (maxSegId < fetchDev.SegId) maxSegId = fetchDev.SegId;
                }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            return fetchDevs;
        }

        RemoteInterface.HC.FetchDeviceData[] LoadLCMS(string lineid,string direction,int mile)
        {
            string where;
            if (direction == "S" || direction == "E")
            {
                where = string.Format(" mileage <= {0} order by mileage desc", mile);
            }
            else
            {
                where = " mileage >= " + mile.ToString() + " order by mileage asc ";
            }

            string cmd = string.Format("Select mileage,DivisionType From {0}.{1} where LineID = '{2}' and DivisionType in ('I','C') and {3}  fetch first 1 rows only; "
                , RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblGroupDivision, lineid, where);
            System.Data.DataTable DT = com.Select(cmd);

            if (DT.Rows.Count == 0 || (string)DT.Rows[0][1] != "I")
            {
                return new RemoteInterface.HC.FetchDeviceData[0];
            }

            int DivMile = (int)DT.Rows[0][0];

            cmd = string.Format("Select cfg.DeviceName,cfg.Mile_M,section.MaxSpeed,section.MinSpeed From {0}.{1} cfg ,{0}.{4} section "
                + " where cfg.LineID = '{2}' and cfg.device_Type = 'CMS'  and cfg.Location = 'L' "
                + " and cfg.mile_m >= ({3} - 500) and cfg.mile_m <= ({3} + 500) and cfg.sectionID = section.sectionID;"
                , RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblDeviceConfig, lineid, DivMile, DBConnect.DB2TableName.tblGroupSection);
            DT = com.Select(cmd);

            RemoteInterface.HC.FetchDeviceData[] fetchDevs = new RemoteInterface.HC.FetchDeviceData[DT.Rows.Count];
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                System.Data.DataRow dr = DT.Rows[i];
                fetchDevs[i] = new RemoteInterface.HC.FetchDeviceData((string)dr[0], (int)DevRange["NORMAL"], lineid, direction, (int)dr[1], (int)dr[2], (int)dr[3], "CMS");
                fetchDevs[i].Location = "L";
            }
            return fetchDevs;
        }

        RemoteInterface.HC.FetchDeviceData[] LoadNearLCMS(string lineid, int mile)
        {
            string cmd = string.Format("Select cfg.DeviceName,cfg.Mile_M,section.MaxSpeed,section.MinSpeed,cfg.Direction From {0}.{1} cfg ,{0}.{4} section "
                + " where cfg.LineID = '{2}' and cfg.device_Type = 'CMS'  and cfg.Location = 'L' "
                + " and cfg.mile_m >= ({3} - 500) and cfg.mile_m <= ({3} + 500) and cfg.sectionID = section.sectionID;"
                , RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblDeviceConfig, lineid, mile, DBConnect.DB2TableName.tblGroupSection);
            System.Data.DataTable DT = com.Select(cmd);

            RemoteInterface.HC.FetchDeviceData[] fetchDevs = new RemoteInterface.HC.FetchDeviceData[DT.Rows.Count];
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                System.Data.DataRow dr = DT.Rows[i];
                fetchDevs[i] = new RemoteInterface.HC.FetchDeviceData((string)dr[0], (int)DevRange["NORMAL"], lineid, "", (int)dr[1], (int)dr[2], (int)dr[3], "CMS");
                fetchDevs[i].Location = "L";
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
        virtual protected System.Collections.Hashtable setGENDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId,string direction) 
        {
            int unit_Start = findUnitSection(Convert.ToInt32(ht["FROM_MILEPOST1"]), direction);
            megType = getGeneralMsgType(degree, Convert.ToInt32(ht["INC_CONGESTION"]), unit_Start,direction);
            //this.serMeg.setServerMeg(string.Format("事件訊息型態:{0}", megType.ToString()));
            return setDisplay(devNames, maxSegId, megType);
        }

        private int findUnitSection(int startMileage, string direction) //?
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
        virtual protected MegType getGeneralMsgType(Degree degree, int jammed, int unit_Start,string direction)
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
                    hobj.GetAllTrafficDataByUnit(ht["INC_LINEID"].ToString(), direction, unit_Start, ref  volume, ref  speed, ref  occupancy, ref  jameLevel, ref  travelSec, ref  vdList);
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
