using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using RemoteInterface.HC;
using Host.TC;
using RemoteInterface;
//using System.Runtime.Remoting.Messaging;

namespace Host
{
    class HC_FWIS_Robj:RemoteInterface.RemoteClassBase,I_HC_FWIS
    {
        public static System.Collections.Generic.Dictionary<string, InOutStastic> dictPerformance = new Dictionary<string, InOutStastic>();
         static HC_FWIS_Robj()
        {
            dictPerformance.Add("RGS_setManualGenericDisplay",new InOutStastic());
            dictPerformance.Add("SetDbChangeNotify", new InOutStastic());
            dictPerformance.Add("GetAllVD5MinAvgData", new InOutStastic());
            dictPerformance.Add("GetVD5MinAvgData", new InOutStastic());
            dictPerformance.Add("GetRD5MinData", new InOutStastic());
            dictPerformance.Add("AddDevice", new InOutStastic());
            dictPerformance.Add("RemoveDevice", new InOutStastic());
            dictPerformance.Add("CMS_setManualDisplay", new InOutStastic());
            dictPerformance.Add("setManualModeOff", new InOutStastic());
            dictPerformance.Add("MAS_setManualModeOff", new InOutStastic());
            dictPerformance.Add("RMS_setManualModeAndPlan", new InOutStastic());
            dictPerformance.Add("WIS_setManualDisplay", new InOutStastic());
            dictPerformance.Add("getSendDs", new InOutStastic());
            dictPerformance.Add("SetOutput", new InOutStastic());
            dictPerformance.Add("GetCurrentOutput", new InOutStastic());
            dictPerformance.Add("LCS_setManualDisplay", new InOutStastic());
            dictPerformance.Add("GetAllTrafficData", new InOutStastic());
            dictPerformance.Add("GetTravelUpperLimitLowerLimtByRange", new InOutStastic());
            dictPerformance.Add("GetAllTrafficDataByUnit", new InOutStastic());
            dictPerformance.Add("GetDeviceStatus", new InOutStastic());
            dictPerformance.Add("getOutputQueueData", new InOutStastic()); 
            dictPerformance.Add("LoadSchedule", new InOutStastic());
            dictPerformance.Add("LoadScheduleByManualPriority", new InOutStastic());
            dictPerformance.Add("RemoveSchedule", new InOutStastic());
            dictPerformance.Add("Fetch", new InOutStastic());
            dictPerformance.Add("setEventStatus", new InOutStastic());
            dictPerformance.Add("getTravelTimeData", new InOutStastic());
            dictPerformance.Add("InputIIP_Event", new InOutStastic());
            dictPerformance.Add("GenExecutionPlan", new InOutStastic());
            dictPerformance.Add("SetMovingContructEvent", new InOutStastic());
            dictPerformance.Add("CloseMovingConstructEvent", new InOutStastic());
            dictPerformance.Add("Get_VD_TravelTime", new InOutStastic());
            dictPerformance.Add("Get_AVI_TravelTime", new InOutStastic());
            dictPerformance.Add("Get_ETC_TravelTime", new InOutStastic());
            dictPerformance.Add("Get_HIS_TravelTime", new InOutStastic());
            dictPerformance.Add("SetManualEvent", new InOutStastic());
            dictPerformance.Add("ReNewManualEvent", new InOutStastic());
            dictPerformance.Add("getTimccTravelTimeByRange", new InOutStastic());
            dictPerformance.Add("SendSMS", new InOutStastic());
            dictPerformance.Add("LockCCTV", new InOutStastic());
            dictPerformance.Add("SetETTUCCTVLock", new InOutStastic());
            dictPerformance.Add("ReLoadEventExecutionOutput", new InOutStastic());
            dictPerformance.Add("NotifyUserTIMCCPlayData", new InOutStastic());
           
         //   dictPerformance.Add("ReLoadEventExecutionOutput", 0);
        }


        public void RGS_setManualGenericDisplay(string devicename, RGS_GenericDisplay_Data data,bool bForce)
        {
            try
            {
                if (dictPerformance.ContainsKey("RGS_setManualGenericDisplay"))
                {
                    dictPerformance["RGS_setManualGenericDisplay"].CallCount++;
                    dictPerformance["RGS_setManualGenericDisplay"].InCount++;
                }


                OutputQueueData odata = new OutputQueueData(devicename, (bForce) ? OutputModeEnum.ForceManualMode : OutputModeEnum.ManualMode, (bForce) ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID, (bForce) ? OutputQueueData.MANUAL_PRIORITY : OutputQueueData.NORMAL_MANUAL_PRIORITY, data);
                ((RGSDeviceWrapper)Program.matrix.getDeviceWrapper(devicename)).SetOutput(odata);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("RGS_setManualGenericDisplay"))
                    dictPerformance["RGS_setManualGenericDisplay"].InCount--;
            }
        }


        public void SetDbChangeNotify(DbChangeNotifyConst notifyConst,params object[] args)
        {
            DeviceBaseWrapper dev;
            try
            {
                if (dictPerformance.ContainsKey("SetDbChangeNotify"))
                {
                    dictPerformance["SetDbChangeNotify"].CallCount++;
                    dictPerformance["SetDbChangeNotify"].InCount++;
                }

                switch (notifyConst)
                {
                    case DbChangeNotifyConst.ETAG_Life_Minutes_Change:
                        if (args==null||args.Length != 1)
                              throw new Exception("引數錯誤");
                        int minute = System.Convert.ToInt32(args[0]);
                        Host.TC.AVIDeviceWrapper.ETAGLiveTime = minute;
                        break;
                    case DbChangeNotifyConst.JamEvalTable:
                        Program.matrix.vd_jam_eval.LoatTable();
                        break;
                    case DbChangeNotifyConst.AID_PARAMETER_Change:
                        if (args != null && args[0] != null)
                        {
                            dev = Program.matrix.getDeviceWrapper(args[0].ToString());
                            AID.AIDobject.LoadParameters();
                            VDDeviceWrapper vddev = dev as VDDeviceWrapper;
                            vddev.NotifyAidObjSettingChange();
                        }
                        break;
                    case DbChangeNotifyConst.TravelSettingData:
                        dev = Program.matrix.getDeviceWrapper(args[0].ToString());
                        if (dev is RGSDeviceWrapper)
                        {
                            ((RGSDeviceWrapper)dev).loadTravelSetting();
                            try
                            {
                                ((RGSDeviceWrapper)dev).DisplayTravelTime();
                            }
                            catch { ;}
                            ConsoleServer.WriteLine(args[0].ToString() + "旅行時間設定改變!");
                        }
                        else if (dev is CMSDeviceWrapper)
                        {
                            ((CMSDeviceWrapper)dev).loadTravelSetting();
                            try
                            {
                                ((CMSDeviceWrapper)dev).DisplayTravelTime();
                            }
                            catch { ;}
                            ConsoleServer.WriteLine(args[0].ToString() + "旅行時間設定改變!");
                        }
                        else if (dev is TTSDeviceWrapper)
                        {
                            ((TTSDeviceWrapper)dev).loadTravelSetting();
                            try
                            {
                                ((TTSDeviceWrapper)dev).DisplayTravelTime();
                            }
                            catch { ;}
                            ConsoleServer.WriteLine(args[0].ToString() + "旅行時間設定改變!");
                        }
                        else
                            throw new Exception("設備不支援旅行時間設定");

                        break;

                    case DbChangeNotifyConst.SectionPolygonMapingData:

                        Program.matrix.rgs_polygon_section_mapping.loadPolyGonSessionMapping();
                        if (args != null && args[0] != null)
                            ((OutPutDeviceBase)Program.matrix.getDeviceWrapper(args[0].ToString())).output();

                        break;
                    case DbChangeNotifyConst.UnitRoadVDMapping:  //args 0:lineid,1:dir ,2: km
                        if (args == null || args.Length != 3)
                            throw new Exception("引數錯誤");
                        Program.matrix.line_mgr[args[0].ToString()][args[1].ToString(), System.Convert.ToInt32(args[2])].load_vd_travel_mapping_table();
                        System.Console.WriteLine(Program.matrix.line_mgr[args[0].ToString()][args[1].ToString(), System.Convert.ToInt32(args[2])].unitid + "VD 對應表改變!");
                        break;

                    case DbChangeNotifyConst.AVISampleInterval:
                        if (args == null || args.Length != 1)
                            throw new Exception("引數錯誤");

                        Program.matrix.avimgr[args[0].ToString()].LoadAVISectionEffective();
                        break;
                    case DbChangeNotifyConst.ETC_IP_Change:
                        Program.matrix.etcmgr.loadETCIPs();
                        break;
                    case DbChangeNotifyConst.RediretRoute_Change:
                        Program.matrix.route_mgr.loadAllRoutSetting();
                        break;
                    case DbChangeNotifyConst.T74RediretRoute_Change:
                        Program.matrix.route_mgr74.loadAllRoutSetting();
                        break;
                    case DbChangeNotifyConst.Reload_Device_Loaction:
                        if (args == null || args.Length != 1)
                            throw new Exception("引數錯誤");
                        Program.matrix.device_mgr[args[0].ToString()].ReloadDeviceLocation();
                        break;
                    case DbChangeNotifyConst.Reload_Section_WeightTable:
                        if (args == null || args.Length != 3)
                            throw new Exception("引數錯誤");
                        Program.matrix.line_mgr[args[0].ToString()].getSection(args[1].ToString(), args[2].ToString()).LoadSectionTravelTimeWeight();
                        break;

                    case DbChangeNotifyConst.CMSRST_Manual_MessageChange:
                        if (args == null || args.Length != 2)
                            throw new Exception("引數錯誤");
                        //   Program.matrix.line_mgr[args[0].ToString()].getSection(args[1].ToString(),args[2].ToString()).LoadSectionTravelTimeWeight();
                        (Program.matrix.getDeviceWrapper(args[0].ToString()) as CMSRSTDeviceWrapper).loadManualTurnData(System.Convert.ToInt32(args[1]));
                        break;
                    case DbChangeNotifyConst.Enable_Weather_Change:
                        if (args == null || args.Length != 2)
                            throw new Exception("引數錯誤");

                        (Program.matrix.getDeviceWrapper(args[0].ToString()) as CMSDeviceWrapper).EnableWeather = args[1].ToString().Trim();
                        break;

                }
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("SetDbChangeNotify"))
                         dictPerformance["SetDbChangeNotify"].InCount--;
            }
        }

        public RemoteInterface.HC.VD5MinMovingData[]  GetAllVD5MinAvgData()
       {
          //  Program.matrix.
           try
           {
               if (dictPerformance.ContainsKey("GetAllVD5MinAvgData"))
               {
                   dictPerformance["GetAllVD5MinAvgData"].CallCount++;
                   dictPerformance["GetAllVD5MinAvgData"].InCount++;
               }

               System.Collections.ArrayList ary = new System.Collections.ArrayList();
               Host.TC.DevcieManager devmgr = Program.matrix.getDevicemanager();
               foreach (TC.DeviceBaseWrapper dev in devmgr.getAllDeviceEnum())
               {
                   if (dev is TC.VDDeviceWrapper)
                   {
                       VDDeviceWrapper vd = (VDDeviceWrapper)dev;
                       ary.Add(vd.ToFWIS_Get5minMovingAvgData());

                   }

               }
               object[] objs = ary.ToArray();
               RemoteInterface.HC.VD5MinMovingData[] retdata = new VD5MinMovingData[objs.Length];
               for (int i = 0; i < retdata.Length; i++)
                   retdata[i] = (RemoteInterface.HC.VD5MinMovingData)objs[i];

               return retdata;
           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
           }
           finally
           {
               if (dictPerformance.ContainsKey("GetAllVD5MinAvgData"))
               {
                 //  dictPerformance["GetAllVD5MinAvgData"].CallCount++;
                   dictPerformance["GetAllVD5MinAvgData"].InCount--;
               }
           }


       }
        public RemoteInterface.HC.VD5MinMovingData GetVD5MinAvgData(string devName)
        {
            try
            {
                if (dictPerformance.ContainsKey("GetVD5MinAvgData"))
                {
                    dictPerformance["GetVD5MinAvgData"].CallCount++;
                    dictPerformance["GetVD5MinAvgData"].InCount++;
                }
                Host.TC.DevcieManager devmgr = Program.matrix.getDevicemanager();
                VDDeviceWrapper vd = (VDDeviceWrapper)devmgr[devName];
                return vd.ToFWIS_Get5minMovingAvgData();
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("GetVD5MinAvgData"))
                {
                   // dictPerformance["GetVD5MinAvgData"].CallCount++;
                    dictPerformance["GetVD5MinAvgData"].InCount--;
                }
            }
        }

        public void GetRD5MinData(string devName,ref System.DateTime lastReportTIme,ref int amount,ref int acc_amount,ref int degree)
        {
            try
            {
                if (dictPerformance.ContainsKey("GetRD5MinData"))
                {
                    dictPerformance["GetRD5MinData"].CallCount++;
                    dictPerformance["GetRD5MinData"].InCount++;
                }
                Host.TC.DevcieManager devmgr = Program.matrix.getDevicemanager();
                RDDeviceWrapper rd = (RDDeviceWrapper)devmgr[devName];

                amount = rd.curr_amount;
                acc_amount = rd.curr_acc_amount;
                degree = rd.curr_degree;
                lastReportTIme = rd.lastReportTime;
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("GetRD5MinData"))
                {
                   // dictPerformance["GetRD5MinData"].CallCount++;
                    dictPerformance["GetRD5MinData"].InCount--;
                }
            }
        }

        public void AddDevice(string devName)
        {
            try
            {
                if (dictPerformance.ContainsKey("AddDevice"))
                {
                    dictPerformance["AddDevice"].CallCount++;
                    dictPerformance["AddDevice"].InCount++;
                }
                Host.TC.DevcieManager devmgr = Program.matrix.getDevicemanager();
                devmgr.AddDeviceWrapper(devName, true, null);

            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("AddDevice"))
                {
                  //  dictPerformance["AddDevice"].CallCount++;
                    dictPerformance["AddDevice"].InCount--;
                }
            }
        }
        public void RemoveDevice(string devName)
        {
            try
            {
                if (dictPerformance.ContainsKey("RemoveDevice"))
                {
                    dictPerformance["RemoveDevice"].CallCount++;
                    dictPerformance["RemoveDevice"].InCount++;
                }
                Host.TC.DevcieManager devmgr = Program.matrix.getDevicemanager();
                devmgr.RemoveDeviceWrapper(devName);

            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("RemoveDevice"))
                {
                  //  dictPerformance["RemoveDevice"].CallCount++;
                    dictPerformance["RemoveDevice"].InCount--;
                }
            }
        }
        public void CMS_setManualDisplay(string devicename, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors,bool bForce)
        {

            try
            {
                if (dictPerformance.ContainsKey("CMS_setManualDisplay"))
                {
                    dictPerformance["CMS_setManualDisplay"].CallCount++;
                    dictPerformance["CMS_setManualDisplay"].InCount++;
                }
                ((CMSDeviceWrapper)Program.matrix.getDeviceWrapper(devicename)).setCMS_Dispaly(devicename, (bForce) ? OutputModeEnum.ForceManualMode : OutputModeEnum.ManualMode, (bForce) ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID, (bForce) ? OutputQueueData.MANUAL_PRIORITY : OutputQueueData.NORMAL_MANUAL_PRIORITY,
                    icon_id, g_code_id, hor_space, mesg, colors);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("CMS_setManualDisplay"))
                {
                   // dictPerformance["CMS_setManualDisplay"].CallCount++;
                    dictPerformance["CMS_setManualDisplay"].InCount--;
                }
            }
            
        }
        public void CMS_setManualDisplay(string devicename, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors,byte[] vspaces, bool bForce)
        {
            try
            {
                if (dictPerformance.ContainsKey("CMS_setManualDisplay"))
                {
                    dictPerformance["CMS_setManualDisplay"].CallCount++;
                    dictPerformance["CMS_setManualDisplay"].InCount++;
                }

                ((CMSDeviceWrapper)Program.matrix.getDeviceWrapper(devicename)).setCMS_Dispaly(devicename, (bForce) ? OutputModeEnum.ForceManualMode : OutputModeEnum.ManualMode, (bForce) ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID, (bForce) ? OutputQueueData.MANUAL_PRIORITY : OutputQueueData.NORMAL_MANUAL_PRIORITY,
                    icon_id, g_code_id, hor_space, mesg, colors, vspaces);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("CMS_setManualDisplay"))
                {
                  //  dictPerformance["CMS_setManualDisplay"].CallCount++;
                    dictPerformance["CMS_setManualDisplay"].InCount--;
                }
            }


        }

        //public void CMS_setManualModeOff(string devicename)
        //{
        //    try
        //    {
        //        ((CMSDeviceWrapper)Program.matrix.getDeviceWrapper(devicename)).removeOutput(OutPutDeviceBase.MANUAL_RULE_ID);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new RemoteException(ex.Message + ex.StackTrace);
        //    }
        //}

        public void setManualModeOff(string devicename)
        {
            try
            {
                if (dictPerformance.ContainsKey("setManualModeOff"))
                {
                    dictPerformance["setManualModeOff"].CallCount++;
                    dictPerformance["setManualModeOff"].InCount++;
                }
                ((OutPutDeviceBase)Program.matrix.getDeviceWrapper(devicename)).removeOutput(OutputQueueData.MANUAL_RULE_ID);
                ((OutPutDeviceBase)Program.matrix.getDeviceWrapper(devicename)).removeOutput(OutputQueueData.NORMAL_MANUAL_RULE_ID);
                string sql = "update tblDeviceManualOutputData set ManualData=null,ForceManualData=null  where deviceName='" + devicename + "'";
                Program.matrix.dbServer.SendSqlCmd(sql);
                ((OutPutDeviceBase)Program.matrix.getDeviceWrapper(devicename)).output();
                //sql = "update tblDeviceManualOutputData set data=null  where deviceName='" + devicename + "'";
                //Program.matrix.dbServer.SendSqlCmd(sql);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setManualModeOff"))
                {
                   // dictPerformance["setManualModeOff"].CallCount++;
                    dictPerformance["setManualModeOff"].InCount--;
                }
            }
        }
        public void setManualModeOff(string deviceName, bool bForce)
        {
             string sql="";
             try
             {
                 if (dictPerformance.ContainsKey("setManualModeOff"))
                 {
                     dictPerformance["setManualModeOff"].CallCount++;
                     dictPerformance["setManualModeOff"].InCount++;
                 }
                 if (bForce)
                 {
                     ((OutPutDeviceBase)Program.matrix.getDeviceWrapper(deviceName)).removeOutput(OutputQueueData.MANUAL_RULE_ID);
                     sql = "update tblDeviceManualOutputData set ForceManualData=null  where deviceName='" + deviceName + "'";
                 }
                 else
                 {
                     ((OutPutDeviceBase)Program.matrix.getDeviceWrapper(deviceName)).removeOutput(OutputQueueData.NORMAL_MANUAL_RULE_ID);
                     sql = "update tblDeviceManualOutputData set ManualData=null where deviceName='" + deviceName + "'";
                 }

                 Program.matrix.dbServer.SendSqlCmd(sql);
                 //sql = "update tblDeviceManualOutputData set data=null  where deviceName='" + devicename + "'";
                 //Program.matrix.dbServer.SendSqlCmd(sql);

             }
             catch (Exception ex)
             {
                 throw new RemoteException(ex.Message + ex.StackTrace);
             }
             finally
             {
                 if (dictPerformance.ContainsKey("setManualModeOff"))
                 {
                    // dictPerformance["setManualModeOff"].CallCount++;
                     dictPerformance["setManualModeOff"].InCount--;
                 }
             }
        }

        public void TTS_setManualModeOff(string devName, int boardid, bool bForce)
        {
            string sql = "";

            try
            {
                if (dictPerformance.ContainsKey("TTS_setManualModeOff"))
                {
                    dictPerformance["TTS_setManualModeOff"].CallCount++;
                    dictPerformance["TTS_setManualModeOff"].InCount++;
                }
                TTSDeviceWrapper tc = (TTSDeviceWrapper)Program.matrix.getDeviceWrapper(devName);
                int ruleid = bForce ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID;
                OutputQueueData qdata = tc.GetQueueData(ruleid);

                if (qdata == null)
                    return;

                TTSOutputData tdata = (TTSOutputData)qdata.data;
                if (tdata == null)
                {
                    tc.removeOutput(ruleid);
                    if (bForce)
                        sql = "update tblDeviceManualOutputData set ForceManualData=null  where deviceName='" + devName + "'";
                    else
                        sql = "update tblDeviceManualOutputData set ManualData=null where deviceName='" + devName + "'";

                    Program.matrix.dbServer.SendSqlCmd(sql);
                    return;
                }

                for (int i = 0; i < tdata.boardid.Length; i++)
                {
                    if (tdata.boardid[i] == boardid)  //找到
                    {
                        if (tdata.boardid.Length == 1)  //移除 OutputQueueData
                        {
                            tc.removeOutput(ruleid);
                            if (bForce)
                                sql = "update tblDeviceManualOutputData set ForceManualData=null  where deviceName='" + devName + "'";
                            else
                                sql = "update tblDeviceManualOutputData set ManualData=null where deviceName='" + devName + "'";

                            Program.matrix.dbServer.SendSqlCmd(sql);
                        }

                        else  //移除 boardid　
                        {
                            byte[] bdata = new byte[tdata.boardid.Length - 1], cdata = new byte[tdata.boardid.Length - 1];
                            string[] mdata = new string[tdata.boardid.Length - 1];
                            int inx = 0;
                            for (int j = 0; j < tdata.boardid.Length; j++)  //複製
                            {
                                if (tdata.boardid[j] == boardid)
                                    continue;
                                bdata[inx] = tdata.boardid[j];
                                mdata[inx] = tdata.traveltime[j];
                                cdata[inx] = tdata.color[j];
                                inx++;
                            }

                            tdata.boardid = bdata;
                            tdata.traveltime = mdata;
                            tdata.color = cdata;
                            tc.output();
                        }

                        break;
                    }
                }



            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("TTS_setManualModeOff"))
                {
                  //  dictPerformance["TTS_setManualModeOff"].CallCount++;
                    dictPerformance["TTS_setManualModeOff"].InCount--;
                }
            }
        }
        public void MAS_setManualModeOff(string devName, int laneid, bool bForce)
        {
            string sql = "";

            try
            {
                if (dictPerformance.ContainsKey("MAS_setManualModeOff"))
                {
                    dictPerformance["MAS_setManualModeOff"].CallCount++;
                    dictPerformance["MAS_setManualModeOff"].InCount++;
                }
                MASDeviceWrapper tc = (MASDeviceWrapper)Program.matrix.getDeviceWrapper(devName);
                int ruleid = bForce ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID;
                OutputQueueData qdata = tc.GetQueueData(ruleid);

                if (qdata == null)
                    return;

                MASOutputData tdata = (MASOutputData)qdata.data;
                if (tdata == null)
                {
                    tc.removeOutput(ruleid);
                    if (bForce)
                        sql = "update tblDeviceManualOutputData set ForceManualData=null  where deviceName='" + devName + "'";
                    else
                        sql = "update tblDeviceManualOutputData set ManualData=null where deviceName='" + devName + "'";

                    Program.matrix.dbServer.SendSqlCmd(sql);
                    return;
                }

                for (int i = 0; i < tdata.laneids.Length; i++)
                {
                    if (tdata.laneids[i] == laneid)  //找到
                    {
                        if (tdata.laneids.Length == 1)  //移除 OutputQueueData
                        {
                            tc.removeOutput(ruleid);
                            if (bForce)
                                sql = "update tblDeviceManualOutputData set ForceManualData=null  where deviceName='" + devName + "'";
                            else
                                sql = "update tblDeviceManualOutputData set ManualData=null where deviceName='" + devName + "'";

                            Program.matrix.dbServer.SendSqlCmd(sql);
                        }

                        else  //移除 boardid　
                        {
                            byte[] bdata = new byte[tdata.laneids.Length - 1];
                            object[] mdata = new object[tdata.laneids.Length - 1];
                            int inx = 0;
                            for (int j = 0; j < tdata.laneids.Length; j++)  //複製
                            {
                                if (tdata.laneids[j] == laneid)
                                    continue;
                                bdata[inx] = tdata.laneids[j];
                                mdata[inx] = tdata.displays[j];
                                // cdata[inx] = tdata.color[j];
                                inx++;
                            }

                            tdata.laneids = bdata;
                            tdata.displays = mdata;
                            //  tdata.color = cdata;
                            tc.output();
                        }

                        break;
                    }
                }



            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("MAS_setManualModeOff"))
                {
                    //dictPerformance["MAS_setManualModeOff"].CallCount++;
                    dictPerformance["MAS_setManualModeOff"].InCount--;
                }
            }
        }

        public void RMS_setManualModeAndPlan(string devicename, int mode, int planno, bool bForce)
        {

            try
            {
                if (dictPerformance.ContainsKey("RMS_setManualModeAndPlan"))
                {
                    dictPerformance["RMS_setManualModeAndPlan"].CallCount++;
                    dictPerformance["RMS_setManualModeAndPlan"].InCount++;
                }
                ((RMSDeviceWrapper)Program.matrix.getDeviceWrapper(devicename)).SetModeAndPlanno(devicename, (bForce) ? OutputModeEnum.ForceManualMode : OutputModeEnum.ManualMode, (bForce) ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID, (bForce) ? OutputQueueData.MANUAL_PRIORITY : OutputQueueData.NORMAL_MANUAL_PRIORITY, (byte)mode, (byte)planno);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("RMS_setManualModeAndPlan"))
                {
                    //dictPerformance["RMS_setManualModeAndPlan"].CallCount++;
                    dictPerformance["RMS_setManualModeAndPlan"].InCount--;
                }
            }
        }

        
        public void WIS_setManualDisplay(string devicename,int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors, bool bForce)
        {
            try
            {
                if (dictPerformance.ContainsKey("WIS_setManualDisplay"))
                {
                    dictPerformance["WIS_setManualDisplay"].CallCount++;
                    dictPerformance["WIS_setManualDisplay"].InCount++;
                }
                ((WISDeviceWrapper)Program.matrix.getDeviceWrapper(devicename)).setWIS_Dispaly(devicename, (bForce) ? OutputModeEnum.ForceManualMode : OutputModeEnum.ManualMode, (bForce) ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID, (bForce) ? OutputQueueData.MANUAL_PRIORITY : OutputQueueData.NORMAL_MANUAL_PRIORITY, icon_id, g_code_id, hor_space, mesg, colors);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("WIS_setManualDisplay"))
                {
                   // dictPerformance["WIS_setManualDisplay"].CallCount++;
                    dictPerformance["WIS_setManualDisplay"].InCount--;
                }
            }
        }

        public System.Data.DataSet getSendDs(string DeviceType, string func_name)
        {

            try
            {
                if (dictPerformance.ContainsKey("getSendDs"))
                {
                    dictPerformance["getSendDs"].CallCount++;
                    dictPerformance["getSendDs"].InCount++;
                }
                return Program.ScriptMgr[DeviceType].GetSendDataSet(func_name);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("getSendDs"))
                {
                   // dictPerformance["getSendDs"].CallCount++;
                    dictPerformance["getSendDs"].InCount--;
                }
            }
        }

      
        public void SetOutput(string devName,object  OutputData,bool bForce)
        {
            try
            {
                if (dictPerformance.ContainsKey("SetOutput"))
                {
                    dictPerformance["SetOutput"].CallCount++;
                    dictPerformance["SetOutput"].InCount++;
                }
                OutputQueueData data = new OutputQueueData(devName, (bForce) ? OutputModeEnum.ForceManualMode : OutputModeEnum.ManualMode, (bForce) ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID, (bForce) ? OutputQueueData.MANUAL_PRIORITY : OutputQueueData.NORMAL_MANUAL_PRIORITY, OutputData);
                ((OutPutDeviceBase)Program.matrix.getDeviceWrapper(devName)).SetOutput(data);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("SetOutput"))
                {
                   // dictPerformance["SetOutput"].CallCount++;
                    dictPerformance["SetOutput"].InCount--;
                }
            }
        }


        public object GetCurrentOutput(string devName,ref int priority)
        {
            try
            {
                if (dictPerformance.ContainsKey("GetCurrentOutput"))
                {
                    dictPerformance["GetCurrentOutput"].CallCount++;
                    dictPerformance["GetCurrentOutput"].InCount++;
                }

                Host.TC.OutPutDeviceBase dev = (Host.TC.OutPutDeviceBase)Program.matrix.getDevicemanager()[devName];
                OutputQueueData data = dev.getOutputdata();

                if (data == null) return null;

                priority = data.priority;

                return data.data;
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("GetCurrentOutput"))
                {
                  //  dictPerformance["GetCurrentOutput"].CallCount++;
                    dictPerformance["GetCurrentOutput"].InCount--;
                }
            }

        }


        public void LCS_setManualDisplay(string deviceName,System.Data.DataSet ds,bool bForce)
        {

            try
            {
                if (dictPerformance.ContainsKey("LCS_setManualDisplay"))
                {
                    dictPerformance["LCS_setManualDisplay"].CallCount++;
                    dictPerformance["LCS_setManualDisplay"].InCount++;
                }
                ((LCSDeviceWrapper)Program.matrix.getDeviceWrapper(deviceName)).SetOutput(new OutputQueueData(deviceName, (bForce) ? OutputModeEnum.ForceManualMode : OutputModeEnum.ManualMode, (bForce) ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID, (bForce) ? OutputQueueData.MANUAL_PRIORITY : OutputQueueData.NORMAL_MANUAL_PRIORITY, ds));
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("LCS_setManualDisplay"))
                {
                   // dictPerformance["LCS_setManualDisplay"].CallCount++;
                    dictPerformance["LCS_setManualDisplay"].InCount--;
                }
            }
        }


        

        public void GetAllTrafficData(string lineid, string dir, int startMile,int endMile, ref int volume, ref int speed, ref int occupancy, ref int  jameLevel, ref int travelSec)
        {

         int lowerTravelTime=0, upperTravelTime=0;
         try
         {
             if (dictPerformance.ContainsKey("GetAllTrafficData"))
             {
                 dictPerformance["GetAllTrafficData"].CallCount++;
                 dictPerformance["GetAllTrafficData"].InCount++;
             }
             Program.matrix.line_mgr[lineid].getAllTrafficData(dir, startMile, endMile, ref volume, ref speed, ref occupancy, ref jameLevel, ref travelSec, ref lowerTravelTime, ref upperTravelTime);

         }
         catch (Exception ex)
         {
             throw new RemoteException(ex.Message + ex.StackTrace);
         }
         finally
         {
             if (dictPerformance.ContainsKey("GetAllTrafficData"))
             {
                 //dictPerformance["GetAllTrafficData"].CallCount++;
                 dictPerformance["GetAllTrafficData"].InCount--;
             }
         }

        }

        public void GetTravelUpperLimitLowerLimtByRange(string lineid, string dir, int startMile, int endMile,  ref int lowerTravelTime, ref int upperTravelTime)
        {
            int volume = 0, speed = 0, jameLevel = 0, travelSec = 0, occupancy=0;
           // int lowerTravelTime = 0, upperTravelTime = 0;
            try
            {
                if (dictPerformance.ContainsKey("GetTravelUpperLimitLowerLimtByRange"))
                {
                    dictPerformance["GetTravelUpperLimitLowerLimtByRange"].CallCount++;
                    dictPerformance["GetTravelUpperLimitLowerLimtByRange"].InCount++;
                }
                Program.matrix.line_mgr[lineid].getAllTrafficData(dir, startMile, endMile, ref volume, ref speed, ref occupancy, ref jameLevel, ref travelSec, ref lowerTravelTime, ref upperTravelTime);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("GetTravelUpperLimitLowerLimtByRange"))
                {
                  //  dictPerformance["GetTravelUpperLimitLowerLimtByRange"].CallCount++;
                    dictPerformance["GetTravelUpperLimitLowerLimtByRange"].InCount--;
                }
            }

        }


        public void GetAllTrafficDataByUnit(string lineid, string dir, int unitStartKm, ref int volume, ref int speed, ref int occupancy, ref int jameLevel, ref int travelSec, ref string[] vdList)
        {
          //  int lowerTravelTime = 0, upperTravelTime = 0;
            try
            {
                if (dictPerformance.ContainsKey("GetAllTrafficDataByUnit"))
                {
                    dictPerformance["GetAllTrafficDataByUnit"].CallCount++;
                    dictPerformance["GetAllTrafficDataByUnit"].InCount++;
                }
                Program.matrix.line_mgr[lineid][dir, unitStartKm].getAllVDTrafficData(ref vdList, ref volume,
                    ref speed, ref occupancy, ref jameLevel/*, ref travelSec*/, -1, -1);
                travelSec = Program.matrix.line_mgr[lineid][dir, unitStartKm].getTravelTime();

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("GetAllTrafficDataByUnit"))
                {
                   // dictPerformance["GetAllTrafficDataByUnit"].CallCount++;
                    dictPerformance["GetAllTrafficDataByUnit"].InCount--;
                }
            }
        }

        public void GetDeviceStatus(string devName, ref byte[] hw_status, ref byte opmode, ref byte opstatus,ref bool isConnected)
        {

            try
            {
                if (dictPerformance.ContainsKey("GetDeviceStatus"))
                {
                    dictPerformance["GetDeviceStatus"].CallCount++;
                    dictPerformance["GetDeviceStatus"].InCount++;
                }
                DeviceBaseWrapper dev = Program.matrix.getDeviceWrapper(devName);
                hw_status = dev.hw_status;
                opmode = dev.opMode;
                opstatus = dev.opStatus;
                isConnected = dev.IsConnected;
            }

            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("GetDeviceStatus"))
                {
                   // dictPerformance["GetDeviceStatus"].CallCount++;
                    dictPerformance["GetDeviceStatus"].InCount--;
                }
            }

        }

        

        public void LoadSchedule(int schid)
        {
            try
            {
                if (dictPerformance.ContainsKey("LoadSchedule"))
                {
                    dictPerformance["LoadSchedule"].CallCount++;
                    dictPerformance["LoadSchedule"].InCount++;
                }
                Schedule.ScheduleManager.AddSchedule(schid);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("LoadSchedule"))
                {
                   // dictPerformance["LoadSchedule"].CallCount++;
                    dictPerformance["LoadSchedule"].InCount--;
                }
            }
        }

        public void LoadScheduleByManualPriority(int schid)
        {
            try
            {
                if (dictPerformance.ContainsKey("LoadScheduleByManualPriority"))
                {
                    dictPerformance["LoadScheduleByManualPriority"].CallCount++;
                    dictPerformance["LoadScheduleByManualPriority"].InCount++;
                }
                Schedule.ScheduleManager.AddSchedule(schid);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("LoadScheduleByManualPriority"))
                {
                   // dictPerformance["LoadScheduleByManualPriority"].CallCount++;
                    dictPerformance["LoadScheduleByManualPriority"].InCount--;
                }
            }
        }

        public void RemoveSchedule(int schid)
        {
            try
            {
                if (dictPerformance.ContainsKey("RemoveSchedule"))
                {
                    dictPerformance["RemoveSchedule"].CallCount++;
                    dictPerformance["RemoveSchedule"].InCount++;
                }
                
                Schedule.ScheduleManager.RemoveSchedule(schid);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("RemoveSchedule"))
                {
                   // dictPerformance["RemoveSchedule"].CallCount++;
                    dictPerformance["RemoveSchedule"].InCount--;
                }
            }
        }

        public OutputQueueData[] getOutputQueueData(string devName)
        {
            try
            {
                if (dictPerformance.ContainsKey("getOutputQueueData"))
                {
                    dictPerformance["getOutputQueueData"].CallCount++;
                    dictPerformance["getOutputQueueData"].InCount++;
                }
                Host.TC.OutPutDeviceBase outdev = Program.matrix.getDevicemanager()[devName] as Host.TC.OutPutDeviceBase;
                return outdev.getAllOutputQueueData();
                // devmgr.RemoveDeviceWrapper(devName);

            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("getOutputQueueData"))
                {
                  //  dictPerformance["getOutputQueueData"].CallCount++;
                    dictPerformance["getOutputQueueData"].InCount--;
                }
            }
        }


        public FetchDeviceData[] Fetch(string[] deviceTypes, string lineId, string direction, int mileage, int segCnt, int sysSegCnt, bool IsBranch)
        {

            try
            {
                if (dictPerformance.ContainsKey("Fetch"))
                {
                    dictPerformance["Fetch"].CallCount++;
                    dictPerformance["Fetch"].InCount++;
                }
                return Program.matrix.output_device_fetch_mgr.Fetch(deviceTypes, lineId, direction, mileage, segCnt, sysSegCnt, IsBranch);

            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("Fetch"))
                {
                   // dictPerformance["Fetch"].CallCount++;
                    dictPerformance["Fetch"].InCount--;
                }
            }


        }
        public FetchDeviceData[] Fetch(string[] deviceTypes, string lineId, int startMileage, int endMileage)
        {
            try
            {
                if (dictPerformance.ContainsKey("Fetch"))
                {
                    dictPerformance["Fetch"].CallCount++;
                    dictPerformance["Fetch"].InCount++;
                }
                return Program.matrix.output_device_fetch_mgr.Fetch(deviceTypes, lineId, startMileage, endMileage);

            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("Fetch"))
                {
                   // dictPerformance["Fetch"].CallCount++;
                    dictPerformance["Fetch"].InCount--;
                }
            }
        }
        public FetchDeviceData[] Fetch(string[] deviceTypes, string lineId,string direction, int startMileage, int endMileage)
        {
            try
            {
                if (dictPerformance.ContainsKey("Fetch"))
                {
                    dictPerformance["Fetch"].CallCount++;
                    dictPerformance["Fetch"].InCount++;
                }
                return Program.matrix.output_device_fetch_mgr.Fetch(deviceTypes, lineId, direction, startMileage, endMileage);

            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("Fetch"))
                {
                   // dictPerformance["Fetch"].CallCount++;
                    dictPerformance["Fetch"].InCount--;
                }
            }
        }


        public void setEventStatus(int evtid,int status)
        {
            try
            {
                if (dictPerformance.ContainsKey("setEventStatus"))
                {
                    dictPerformance["setEventStatus"].CallCount++;
                    dictPerformance["setEventStatus"].InCount++;
                }
                switch ((Event.EventStatus)status)
                {
                    case Event.EventStatus.Confirm:
                        break;
                    case Event.EventStatus.Executing:
                        break;
                    case Event.EventStatus.PlanCheck:
                        break;
                    case Event.EventStatus.ForceAbort:
                        break;
                    default:
                        throw new Exception("do not support status:" + status);
                }
                Program.matrix.event_mgr.setEventStatus(evtid, status);
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setEventStatus"))
                {
                   // dictPerformance["setEventStatus"].CallCount++;
                    dictPerformance["setEventStatus"].InCount--;
                }
            }
        }


        public TravelTimeData[] getTravelTimeData(string devName)
        {

            try
            {
                if (dictPerformance.ContainsKey("getTravelTimeData"))
                {
                    dictPerformance["getTravelTimeData"].CallCount++;
                    dictPerformance["getTravelTimeData"].InCount++;
                }
                Host.TC.OutPutDeviceBase outdev = Program.matrix.getDevicemanager()[devName] as Host.TC.OutPutDeviceBase;
                if (outdev.deviceType == "RGS")
                {
                    return ((RGSDeviceWrapper)outdev).getTravelTimeData();
                }
                else if (outdev.deviceType == "CMS")
                {
                    return ((CMSDeviceWrapper)outdev).getTravelTimeData();
                }
                else if (outdev.deviceType == "TTS")
                {
                    return ((TTSDeviceWrapper)outdev).getTravelTimeData();
                }
                else
                    throw new Exception(devName + " do not support this method!");


                //  return outdev.getAllOutputQueueData();
                // devmgr.RemoveDeviceWrapper(devName);

            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("getTravelTimeData"))
                {
                    //dictPerformance["getTravelTimeData"].CallCount++;
                    dictPerformance["getTravelTimeData"].InCount--;
                }
            }
        }

        public void InputIIP_Event(int evtid)
        {
            try
            {
                //  Execution.Execution
                if (dictPerformance.ContainsKey("InputIIP_Event"))
                {
                    dictPerformance["InputIIP_Event"].CallCount++;
                    dictPerformance["InputIIP_Event"].InCount++;
                }
                Execution.Execution.getBuilder().InputIIP_Event(evtid);
            }
            catch (Exception ex)
            {
                Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("InputIIP_Event"))
                {
                  //  dictPerformance["InputIIP_Event"].CallCount++;
                    dictPerformance["InputIIP_Event"].InCount--;
                }
            }
        }
        public void GenExecutionPlan(int evtid)
        {
            try
            {
                //  Execution.Execution
                if (dictPerformance.ContainsKey("GenExecutionPlan"))
                {
                    dictPerformance["GenExecutionPlan"].CallCount++;
                    dictPerformance["GenExecutionPlan"].InCount++;
                }
                Execution.Execution.getBuilder().GenerateExecutionTable(evtid);
            }
            catch (Exception ex)
            {
                Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("GenExecutionPlan"))
                {
                    //dictPerformance["GenExecutionPlan"].CallCount++;
                    dictPerformance["GenExecutionPlan"].InCount--;
                }
            }
        }

        
         /// <summary>
        /// 移動施工輸入
        /// </summary>
        /// <param name="notifier">通報者名稱</param>
        /// <param name="timeStamp">事件時間</param>
        /// <param name="lineID">路線編號(N1,T74...)</param>
        /// <param name="directionID">方向編號(N,S,W,E)</param>
        /// <param name="startMileage">施工開始里程(單位:公尺)</param>
        /// <param name="endMileage">施工結束里程(單位:公尺)</param>
        /// <param name="blocklane">阻斷車道(如第一、第二車道和路肩阻斷為:blocklane="110000")</param>
        /// <returns>事件編號(-1為輸入錯誤)</returns>

        public void SetMovingContructEvent(int id, string notifier, DateTime timeStamp, string lineID, string directionID, int startMileage, int endMileage, int blockTypeId, string blocklane,string description)
        {

            try
            {
                //  Execution.Execution
                if (dictPerformance.ContainsKey("SetMovingContructEvent"))
                {
                    dictPerformance["SetMovingContructEvent"].CallCount++;
                    dictPerformance["SetMovingContructEvent"].InCount++;
                }
                Program.matrix.moving_construct_mgr.setEvent(id, notifier, timeStamp, lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, description, "Y");
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("SetMovingContructEvent"))
                {
                  //  dictPerformance["SetMovingContructEvent"].CallCount++;
                    dictPerformance["SetMovingContructEvent"].InCount--;
                }
            }
        }

        public void SetMovingContructEvent(int id, string notifier, DateTime timeStamp, string lineID, string directionID, int startMileage, int endMileage, int blockTypeId, string blocklane, string description,string isExecute)
        {

            try
            {
                if (dictPerformance.ContainsKey("SetMovingContructEvent"))
                {
                    dictPerformance["SetMovingContructEvent"].CallCount++;
                    dictPerformance["SetMovingContructEvent"].InCount++;
                }
                //  Execution.Execution
                Program.matrix.moving_construct_mgr.setEvent(id, notifier, timeStamp, lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, description, isExecute);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("SetMovingContructEvent"))
                {
                   // dictPerformance["SetMovingContructEvent"].CallCount++;
                    dictPerformance["SetMovingContructEvent"].InCount--;
                }
            }
        }


        public void CloseMovingConstructEvent(int id)
        {

            try
            {
                if (dictPerformance.ContainsKey("CloseMovingConstructEvent"))
                {
                    dictPerformance["CloseMovingConstructEvent"].CallCount++;
                    dictPerformance["CloseMovingConstructEvent"].InCount++;
                }
                Program.matrix.moving_construct_mgr.CloseMovingConstructEvent(id);
                // Program.matrix.moving_construct_mgr.setEvent(id, notifier, timeStamp, lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, description);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("CloseMovingConstructEvent"))
                {
                  //  dictPerformance["CloseMovingConstructEvent"].CallCount++;
                    dictPerformance["CloseMovingConstructEvent"].InCount--;
                }
            }
        }


        public int Get_VD_TravelTime(string lineid, string dir, int startmile_m, int endmile_m)
        {
            try
            {
                if (dictPerformance.ContainsKey("Get_VD_TravelTime"))
                {
                    dictPerformance["Get_VD_TravelTime"].CallCount++;
                    dictPerformance["Get_VD_TravelTime"].InCount++;
                }
                return Program.matrix.line_mgr[lineid].getVD_TravelTime(dir, startmile_m, endmile_m);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("Get_VD_TravelTime"))
                {
                   // dictPerformance["Get_VD_TravelTime"].CallCount++;
                    dictPerformance["Get_VD_TravelTime"].InCount--;
                }
            }
        }
        public int Get_AVI_TravelTime(string lineid, string dir, int startmile_m, int endmile_m)
        {

            try
            {
                if (dictPerformance.ContainsKey("Get_AVI_TravelTime"))
                {
                    dictPerformance["Get_AVI_TravelTime"].CallCount++;
                    dictPerformance["Get_AVI_TravelTime"].InCount++;
                }
                return Program.matrix.line_mgr[lineid].getAVI_TravelTime(dir, startmile_m, endmile_m);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("Get_AVI_TravelTime"))
                {
                    //dictPerformance["Get_AVI_TravelTime"].CallCount++;
                    dictPerformance["Get_AVI_TravelTime"].InCount--;
                }
            }
        }

        public int Get_ETC_TravelTime(string lineid, string dir, int startmile_m, int endmile_m)
        {
            try
            {
                if (dictPerformance.ContainsKey("Get_ETC_TravelTime"))
                {
                    dictPerformance["Get_ETC_TravelTime"].CallCount++;
                    dictPerformance["Get_ETC_TravelTime"].InCount++;
                }
                return Program.matrix.line_mgr[lineid].getETC_TravelTime(dir, startmile_m, endmile_m);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("Get_ETC_TravelTime"))
                {
                  //  dictPerformance["Get_ETC_TravelTime"].CallCount++;
                    dictPerformance["Get_ETC_TravelTime"].InCount--;
                }
            }
        }

        public int Get_HIS_TravelTime(string lineid, string dir, int startmile_m, int endmile_m)
        {
            try
            {
                if (dictPerformance.ContainsKey("Get_HIS_TravelTime"))
                {
                    dictPerformance["Get_HIS_TravelTime"].CallCount++;
                    dictPerformance["Get_HIS_TravelTime"].InCount++;
                }
                return Program.matrix.line_mgr[lineid].getHIS_TravelTime(dir, startmile_m, endmile_m);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("Get_HIS_TravelTime"))
                {
                    //dictPerformance["Get_HIS_TravelTime"].CallCount++;
                    dictPerformance["Get_HIS_TravelTime"].InCount--;
                }
            }
        }


        public void SetManualEvent(int eventclass, int evenitid, string lineid, string direction, int startMileage, int endMileage, int level)
        {
          // Program.matrix.event_mgr
            try
            {
                if (dictPerformance.ContainsKey("SetManualEvent"))
                {

                    dictPerformance["SetManualEvent"].CallCount++;
                    dictPerformance["SetManualEvent"].InCount++;
                }
                Program.matrix.event_mgr.AddEvent(new Event.ManualRange(eventclass, evenitid, lineid, direction,
                    startMileage, endMileage, level));

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("SetManualEvent"))
                {

                  //  dictPerformance["SetManualEvent"].CallCount++;
                    dictPerformance["SetManualEvent"].InCount--;
                }
            }
        }

        public void ReNewManualEvent(int eventid, int newevtid, int startMileage, int endMileage, int level)
        {
            try
            {
                if (dictPerformance.ContainsKey("ReNewManualEvent"))
                {
                    dictPerformance["ReNewManualEvent"].CallCount++;
                    dictPerformance["ReNewManualEvent"].InCount++;
                }
                Host.Event.ManualRange evt = Program.matrix.event_mgr.getEventByID(eventid) as Host.Event.ManualRange;

                evt.setNewEventId(newevtid);
                evt.setStartMuileM(startMileage);
                evt.setEndMuileM(endMileage);
                evt.setDegree(level);
                evt.invokeRangeChange();

                // evt.InvokeForceAbort();


            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("ReNewManualEvent"))
                {
                   // dictPerformance["ReNewManualEvent"].CallCount++;
                    dictPerformance["ReNewManualEvent"].InCount--;
                }
            }
        }

        public int  getTimccTravelTimeByRange(string lineid,string dir,int startmile,int endmile)
        {
          int sec=-1,t_upper=-1,t_lower=-1;
          try
          {
              if (dictPerformance.ContainsKey("getTimccTravelTimeByRange"))
              {
                  dictPerformance["getTimccTravelTimeByRange"].CallCount++;
                  dictPerformance["getTimccTravelTimeByRange"].InCount++;
              }
              Program.matrix.timcc_section_mgr.getTravelDataByRange(lineid, dir, startmile, endmile, ref sec, ref t_upper, ref t_lower);
              return sec;
          }
          catch (Exception ex)
          {
              throw new RemoteException(ex.Message);
          }
          finally
          {
              if (dictPerformance.ContainsKey("getTimccTravelTimeByRange"))
              {
                 // dictPerformance["getTimccTravelTimeByRange"].CallCount++;
                  dictPerformance["getTimccTravelTimeByRange"].InCount--;
              }
          }
        }

        public  int SendSMS(string phoneNo, string body)
        {

            try
            {
                if (dictPerformance.ContainsKey("SendSMS"))
                {
                    dictPerformance["SendSMS"].CallCount++;
                    dictPerformance["SendSMS"].InCount++;
                }
                Host.WebReference.Service svr = new WebReference.Service();
                return svr.SendSMS(phoneNo, body);
                // isLock = true;
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("SendSMS"))
                {
                   // dictPerformance["SendSMS"].CallCount++;
                    dictPerformance["SendSMS"].InCount--;
                }
            }
        }

        public void LockCCTV(int cctvid, string desc,string desc2,int preset)
        {
            try
            {
                if (dictPerformance.ContainsKey("LockCCTV"))
                {
                    dictPerformance["LockCCTV"].CallCount++;
                    dictPerformance["LockCCTV"].InCount++;
                }
                Program.matrix.cctvmgr.setLock(cctvid, desc, desc2, preset);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("LockCCTV"))
                {
                   // dictPerformance["LockCCTV"].CallCount++;
                    dictPerformance["LockCCTV"].InCount--;
                }
            }
        }

        public void SetETTUCCTVLock(string etid)
        {
            try
            {
                if (dictPerformance.ContainsKey("SetETTUCCTVLock"))
                {
                    dictPerformance["SetETTUCCTVLock"].CallCount++;
                    dictPerformance["SetETTUCCTVLock"].InCount++;
                }
                Program.matrix.cctvmgr.setETTULock(etid);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("SetETTUCCTVLock"))
                {
                    //dictPerformance["SetETTUCCTVLock"].CallCount++;
                    dictPerformance["SetETTUCCTVLock"].InCount--;
                }
            }
        }

        public void ReLoadEventExecutionOutput(int evtid)
        {
            try
            {
                if (dictPerformance.ContainsKey("ReLoadEventExecutionOutput"))
                {
                    dictPerformance["ReLoadEventExecutionOutput"].CallCount++;
                    dictPerformance["ReLoadEventExecutionOutput"].InCount++;
                }
                Program.matrix.event_mgr.ReloadEventExecutionOutput(evtid);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("ReLoadEventExecutionOutput"))
                {
                   // dictPerformance["ReLoadEventExecutionOutput"].CallCount++;
                    dictPerformance["ReLoadEventExecutionOutput"].InCount--;
                }
            }
        }

        public void NotifyUserTIMCCPlayData(TIMCC_RespInstruction instruction)
        {
            try
            {
                if (dictPerformance.ContainsKey("NotifyUserTIMCCPlayData"))
                {
                    dictPerformance["NotifyUserTIMCCPlayData"].CallCount++;
                    dictPerformance["NotifyUserTIMCCPlayData"].InCount++;
                }
                Program.notifyServer.NotifyAll(new RemoteInterface.NotifyEventObject(EventEnumType.TIMCC_REP_INSTRUCTION, "TIMCC", instruction));
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("NotifyUserTIMCCPlayData"))
                {
                  //  dictPerformance["NotifyUserTIMCCPlayData"].CallCount++;
                    dictPerformance["NotifyUserTIMCCPlayData"].InCount--;
                }
            }

        }
    }
}
