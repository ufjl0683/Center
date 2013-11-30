using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using RemoteInterface.HC;
using Host.TC;
using RemoteInterface;

namespace Host
{
    class HC_FWIS_Robj:RemoteInterface.RemoteClassBase,I_HC_FWIS
    {
        public void RGS_setManualGenericDisplay(string devicename, RGS_GenericDisplay_Data data,bool bForce)
        {
            try
            {
                OutputQueueData odata = new OutputQueueData(devicename,(bForce) ?OutputModeEnum.ForceManualMode:OutputModeEnum.ManualMode,        (bForce) ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID, (bForce) ? OutputQueueData.MANUAL_PRIORITY : OutputQueueData.NORMAL_MANUAL_PRIORITY, data);
                ((RGSDeviceWrapper)Program.matrix.getDeviceWrapper(devicename)).SetOutput(odata);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
        }


        public void SetDbChangeNotify(DbChangeNotifyConst notifyConst,params object[] args)
        {
            DeviceBaseWrapper dev;
          try{
                    switch (notifyConst)
                    {
                        case DbChangeNotifyConst.JamEvalTable:
                            Program.matrix.vd_jam_eval.LoatTable();
                            break;
                        case DbChangeNotifyConst.TravelSettingData:
                            dev = Program.matrix.getDeviceWrapper(args[0].ToString());
                            if (dev is RGSDeviceWrapper)
                            {
                                ((RGSDeviceWrapper)dev).loadTravelSetting();
                                ((RGSDeviceWrapper)dev).DisplayTravelTime();
                                ConsoleServer.WriteLine(args[0].ToString() + "旅行時間設定改變!");
                            }
                            else if (dev is CMSDeviceWrapper)
                            {
                                ((CMSDeviceWrapper)dev).loadTravelSetting();
                                ((CMSDeviceWrapper)dev).DisplayTravelTime();
                                ConsoleServer.WriteLine(args[0].ToString() + "旅行時間設定改變!");
                            }
                            else if (dev is TTSDeviceWrapper)
                            {
                                ((TTSDeviceWrapper)dev).loadTravelSetting();
                                ((TTSDeviceWrapper)dev).DisplayTravelTime();
                                ConsoleServer.WriteLine(args[0].ToString() + "旅行時間設定改變!");
                            }
                            else
                                throw new Exception("設備不支援旅行時間設定");

                            break;

                        case DbChangeNotifyConst.SectionPolygonMapingData:

                            Program.matrix.rgs_polygon_section_mapping.loadPolyGonSessionMapping();
                            if (args != null  && args[0]!=null)
                                ((OutPutDeviceBase)Program.matrix.getDeviceWrapper(args[0].ToString())).output();

                            break;
                        case DbChangeNotifyConst.UnitRoadVDMapping:  //args 0:lineid,1:dir ,2: km
                            if (args == null || args.Length != 3)
                                throw new Exception("引數錯誤");
                            Program.matrix.line_mgr[args[0].ToString()][args[1].ToString(),System.Convert.ToInt32(args[2])].load_vd_travel_mapping_table();
                            System.Console.WriteLine(Program.matrix.line_mgr[args[0].ToString()][args[1].ToString(),System.Convert.ToInt32(args[2])].unitid+"VD 對應表改變!");
                            break;

                        case DbChangeNotifyConst.AVISampleInterval:
                            if(args==null || args.Length!=1)
                                throw new Exception("引數錯誤");

                            Program.matrix.avimgr[args[0].ToString()].LoadAVISectionEffective();
                            break;
                        case  DbChangeNotifyConst.ETC_IP_Change:
                            Program.matrix.etcmgr.loadETCIPs();
                            break;
                        case DbChangeNotifyConst.RediretRoute_Change:
                            Program.matrix.route_mgr.loadAllRoutSetting();
                            break;
                    }
            } catch(Exception ex)
            {
                throw new RemoteException(ex.Message+ex.StackTrace);
            }
        }

        public RemoteInterface.HC.VD5MinMovingData[]  GetAllVD5MinAvgData()
       {
          //  Program.matrix.
           try
           {
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


       }
        public RemoteInterface.HC.VD5MinMovingData GetVD5MinAvgData(string devName)
        {
            try
            {
                Host.TC.DevcieManager devmgr = Program.matrix.getDevicemanager();
                VDDeviceWrapper vd = (VDDeviceWrapper)devmgr[devName];
                return vd.ToFWIS_Get5minMovingAvgData();
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }
        }

        public void GetRD5MinData(string devName,ref System.DateTime lastReportTIme,ref int amount,ref int acc_amount,ref int degree)
        {
            try
            {
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
        }

        public void AddDevice(string devName)
        {
            try
            {
                Host.TC.DevcieManager devmgr = Program.matrix.getDevicemanager();
                devmgr.AddDeviceWrapper(devName,true,null);
              
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }
        }
        public void RemoveDevice(string devName)
        {
            try
            {
                Host.TC.DevcieManager devmgr = Program.matrix.getDevicemanager();
                devmgr.RemoveDeviceWrapper(devName);
              
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }
        }
        public void CMS_setManualDisplay(string devicename, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors,bool bForce)
        {
            try
            {
                ((CMSDeviceWrapper)Program.matrix.getDeviceWrapper(devicename)).setCMS_Dispaly(devicename, (bForce) ? OutputModeEnum.ForceManualMode:OutputModeEnum.ManualMode, (bForce) ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID, (bForce) ? OutputQueueData.MANUAL_PRIORITY : OutputQueueData.NORMAL_MANUAL_PRIORITY,
                    icon_id, g_code_id, hor_space, mesg, colors);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            
        }
        public void CMS_setManualDisplay(string devicename, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors,byte[] vspaces, bool bForce)
        {
            try
            {
                ((CMSDeviceWrapper)Program.matrix.getDeviceWrapper(devicename)).setCMS_Dispaly(devicename, (bForce) ? OutputModeEnum.ForceManualMode : OutputModeEnum.ManualMode, (bForce) ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID, (bForce) ? OutputQueueData.MANUAL_PRIORITY : OutputQueueData.NORMAL_MANUAL_PRIORITY,
                    icon_id, g_code_id, hor_space, mesg, colors,vspaces);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
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
                ((OutPutDeviceBase)Program.matrix.getDeviceWrapper(devicename)).removeOutput(OutputQueueData.MANUAL_RULE_ID);
                ((OutPutDeviceBase)Program.matrix.getDeviceWrapper(devicename)).removeOutput(OutputQueueData.NORMAL_MANUAL_RULE_ID);
                string sql = "update tblDeviceManualOutputData set ManualData=null,ForceManualData=null  where deviceName='" + devicename + "'";
                Program.matrix.dbServer.SendSqlCmd(sql);
                //sql = "update tblDeviceManualOutputData set data=null  where deviceName='" + devicename + "'";
                //Program.matrix.dbServer.SendSqlCmd(sql);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
        }
        public void setManualModeOff(string deviceName, bool bForce)
        {
             string sql="";
            try
            {
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
        }

        public void TTS_setManualModeOff(string devName, int boardid, bool bForce)
        {
            string sql = "";

            try
            {
                TTSDeviceWrapper tc = (TTSDeviceWrapper)Program.matrix.getDeviceWrapper(devName);
                int ruleid=bForce ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID;
                 OutputQueueData  qdata=tc.GetQueueData(ruleid);

                 if (qdata == null)
                     return;

                 TTSOutputData tdata =(TTSOutputData)qdata.data;
                 if (tdata == null)
                 {
                     tc.removeOutput(ruleid);
                    if(bForce)
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
                             int inx=0;
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
        }
        public void MAS_setManualModeOff(string devName, int laneid, bool bForce)
        {
            string sql = "";

            try
            {
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
        }

        public void RMS_setManualModeAndPlan(string devicename, int mode, int planno, bool bForce)
        {

            try
            {
                ((RMSDeviceWrapper)Program.matrix.getDeviceWrapper(devicename)).SetModeAndPlanno(devicename,(bForce)?OutputModeEnum.ForceManualMode:OutputModeEnum.ManualMode, (bForce) ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID, (bForce) ? OutputQueueData.MANUAL_PRIORITY : OutputQueueData.NORMAL_MANUAL_PRIORITY, (byte)mode, (byte)planno);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
        }

        
        public void WIS_setManualDisplay(string devicename,int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors, bool bForce)
        {
            try
            {
                ((WISDeviceWrapper)Program.matrix.getDeviceWrapper(devicename)).setWIS_Dispaly(devicename, (bForce) ?OutputModeEnum.ForceManualMode:OutputModeEnum.ManualMode,(bForce) ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID, (bForce) ? OutputQueueData.MANUAL_PRIORITY : OutputQueueData.NORMAL_MANUAL_PRIORITY,icon_id, g_code_id, hor_space, mesg, colors);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
        }

        public System.Data.DataSet getSendDs(string DeviceType, string func_name)
        {

            try
            {
              return  Program.ScriptMgr[DeviceType].GetSendDataSet(func_name);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
        }


        public void SetOutput(string devName,object  OutputData,bool bForce)
        {
            try
            {
                OutputQueueData data = new OutputQueueData(devName,(bForce) ? OutputModeEnum.ForceManualMode : OutputModeEnum.ManualMode,(bForce) ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID, (bForce) ? OutputQueueData.MANUAL_PRIORITY : OutputQueueData.NORMAL_MANUAL_PRIORITY, OutputData);
                ((OutPutDeviceBase)Program.matrix.getDeviceWrapper(devName)).SetOutput(data);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
        }


        public object GetCurrentOutput(string devName,ref int priority)
        {
            try
            {
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

        }


        public void LCS_setManualDisplay(string deviceName,System.Data.DataSet ds,bool bForce)
        {

            try{
                ((LCSDeviceWrapper)Program.matrix.getDeviceWrapper(deviceName)).SetOutput(new OutputQueueData(deviceName,(bForce) ? OutputModeEnum.ForceManualMode : OutputModeEnum.ManualMode, (bForce) ? OutputQueueData.MANUAL_RULE_ID : OutputQueueData.NORMAL_MANUAL_RULE_ID, (bForce) ? OutputQueueData.MANUAL_PRIORITY : OutputQueueData.NORMAL_MANUAL_PRIORITY, ds));
               }
            catch(Exception ex)
            {
                 throw new RemoteException(ex.Message + ex.StackTrace);
            }
        }


        

        public void GetAllTrafficData(string lineid, string dir, int startMile,int endMile, ref int volume, ref int speed, ref int occupancy, ref int  jameLevel, ref int travelSec)
        {

         int lowerTravelTime=0, upperTravelTime=0;
            try
            {

                Program.matrix.line_mgr[lineid].getAllTrafficData(dir, startMile, endMile,ref volume, ref speed,ref occupancy,ref jameLevel,ref travelSec,ref lowerTravelTime,ref upperTravelTime);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message+ex.StackTrace);
            }

        }

        public void GetTravelUpperLimitLowerLimtByRange(string lineid, string dir, int startMile, int endMile,  ref int lowerTravelTime, ref int upperTravelTime)
        {
            int volume = 0, speed = 0, jameLevel = 0, travelSec = 0, occupancy=0;
           // int lowerTravelTime = 0, upperTravelTime = 0;
            try
            {

                Program.matrix.line_mgr[lineid].getAllTrafficData(dir, startMile, endMile, ref volume, ref speed, ref occupancy, ref jameLevel, ref travelSec, ref lowerTravelTime, ref upperTravelTime);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }

        }


        public void GetAllTrafficDataByUnit(string lineid, string dir, int unitStartKm, ref int volume, ref int speed, ref int occupancy, ref int jameLevel, ref int travelSec, ref string[] vdList)
        {
          //  int lowerTravelTime = 0, upperTravelTime = 0;
            try
            {

                Program.matrix.line_mgr[lineid][dir, unitStartKm].getAllVDTrafficData(ref vdList, ref volume,
                    ref speed, ref occupancy, ref jameLevel/*, ref travelSec*/);
                travelSec = Program.matrix.line_mgr[lineid][dir, unitStartKm].getTravelTime();

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }

        public void GetDeviceStatus(string devName, ref byte[] hw_status, ref byte opmode, ref byte opstatus,ref bool isConnected)
        {
            DeviceBaseWrapper dev = Program.matrix.getDeviceWrapper(devName);
            hw_status = dev.hw_status;
            opmode = dev.opMode;
            opstatus = dev.opStatus;
            isConnected = dev.IsConnected;

        }

        

        public void LoadSchedule(int schid)
        {
            try
            {
                Schedule.ScheduleManager.AddSchedule(schid);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }

        public void RemoveSchedule(int schid)
        {
            try
            {
                Schedule.ScheduleManager.RemoveSchedule(schid);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }

        public OutputQueueData[] getOutputQueueData(string devName)
        {
            try
            {
                Host.TC.OutPutDeviceBase outdev = Program.matrix.getDevicemanager()[devName] as Host.TC.OutPutDeviceBase;
                 return outdev.getAllOutputQueueData();
               // devmgr.RemoveDeviceWrapper(devName);

            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }
        }


        public FetchDeviceData[] Fetch(string[] deviceTypes, string lineId, string direction, int mileage, int segCnt, int sysSegCnt, bool IsBranch)
        {

            try
            {
                return Program.matrix.output_device_fetch_mgr.Fetch(deviceTypes, lineId, direction, mileage, segCnt, sysSegCnt, IsBranch);

            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }


        }
        public FetchDeviceData[] Fetch(string[] deviceTypes, string lineId, int startMileage, int endMileage)
        {
            try
            {
                return Program.matrix.output_device_fetch_mgr.Fetch(deviceTypes, lineId, startMileage, endMileage);

            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }
        }
        public FetchDeviceData[] Fetch(string[] deviceTypes, string lineId,string direction, int startMileage, int endMileage)
        {
            try
            {
                return Program.matrix.output_device_fetch_mgr.Fetch(deviceTypes, lineId,direction, startMileage, endMileage);

            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message + ex.StackTrace);
            }
        }


        public void setEventStatus(int evtid,int status)
        {
            try
            {
                switch ((Event.EventStatus)status)
                {
                    case Event.EventStatus.Confirm:
                        break;
                    case Event.EventStatus.Executing:
                        break;
                    case Event.EventStatus.PlanCheck:
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
        }


        public TravelTimeData[] getTravelTimeData(string devName)
        {

            try
            {
                Host.TC.OutPutDeviceBase outdev = Program.matrix.getDevicemanager()[devName] as Host.TC.OutPutDeviceBase;
                if (outdev.deviceType=="RGS")
                {
                   return ((RGSDeviceWrapper)outdev).getTravelTimeData();
                }
                else if (outdev.deviceType=="CMS")
                {
                    return ((CMSDeviceWrapper)outdev).getTravelTimeData();
                }
                else if (outdev.deviceType=="TTS")
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
        }

        public void InputIIP_Event(int evtid)
        {
            try
            {
             //  Execution.Execution
                Execution.Execution.getBuilder().InputIIP_Event(evtid);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }
        public void GenExecutionPlan(int evtid)
        {
            try
            {
                //  Execution.Execution
                Execution.Execution.getBuilder().GenerateExecutionTable(evtid);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
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
                Program.matrix.moving_construct_mgr.setEvent(id, notifier, timeStamp, lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, description);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }


        public void CloseMovingConstructEvent(int id)
        {

            try
            {
                Program.matrix.moving_construct_mgr.CloseMovingConstructEvent(id);
               // Program.matrix.moving_construct_mgr.setEvent(id, notifier, timeStamp, lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, description);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }
      
    }
}
