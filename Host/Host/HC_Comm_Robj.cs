using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;
using RemoteInterface;
using RemoteInterface.MFCC;

using System.Data;
using Host.TC;
using System.Collections;

namespace Host
{
    class HC_Comm_Robj:RemoteInterface.RemoteClassBase,I_HC_Comm
    {
        public static System.Collections.Generic.Dictionary<string, InOutStastic> dictPerformance = new Dictionary<string, InOutStastic>();

      static HC_Comm_Robj()
      {
          dictPerformance.Add("getSendDsByFuncName",new InOutStastic());
          dictPerformance.Add("SetETTUCCTVLock", new InOutStastic());
          dictPerformance.Add("getDateTime", new InOutStastic());
          dictPerformance.Add("getRedirectStatusString", new InOutStastic());
          dictPerformance.Add("setRMS_RampControl_Start", new InOutStastic());
          dictPerformance.Add("setRMS_RampControl_Stop", new InOutStastic());
          dictPerformance.Add("setRMS_LTR_Start", new InOutStastic());
          dictPerformance.Add("setRMS_LTR_Stop", new InOutStastic());
          dictPerformance.Add("setVDFiveMinData", new InOutStastic());
          dictPerformance.Add("setRDEventData", new InOutStastic());
          dictPerformance.Add("setWDEventData", new InOutStastic());
          dictPerformance.Add("getScriptSource", new InOutStastic());
          dictPerformance.Add("getSendPackage", new InOutStastic());
          dictPerformance.Add("setVIFiveMinData", new InOutStastic());
          dictPerformance.Add("ResetComm", new InOutStastic());
          dictPerformance.Add("getTCCommStatusStr", new InOutStastic());
          dictPerformance.Add("getEventString", new InOutStastic());
          dictPerformance.Add("getJamRangeString", new InOutStastic());
          dictPerformance.Add("getDeviceNames", new InOutStastic());
          dictPerformance.Add("setConnecttionStatus", new InOutStastic());
          dictPerformance.Add("setDeviceStatus", new InOutStastic());
          dictPerformance.Add("setIID_EventData", new InOutStastic());
          dictPerformance.Add("getAllSchduleStatus", new InOutStastic());
        //  dictPerformance.Add("setLSEventData", 0);
          dictPerformance.Add("setLS10MinData", new InOutStastic());
          dictPerformance.Add("getTEMLCSStatus", new InOutStastic());
          dictPerformance.Add("setLSEventData", new InOutStastic());
          dictPerformance.Add("getCurrentDBQueueCnt", new InOutStastic());
          dictPerformance.Add("dbExecute", new InOutStastic());
          dictPerformance.Add("AddAviData", new InOutStastic());
          dictPerformance.Add("DoVD_InteropData", new InOutStastic());



      }

      public void SetDbqMode(DBQueueMode mode)
      {
          try
          {
              Program.matrix.dbServer.SetDbQueueMode(mode);
          }
          catch (Exception ex)
          {
              throw new RemoteException(ex.Message);
          }
      }

        public   System.Data.DataSet getSendDsByFuncName(string devType,string funcName)
        {
            try
            {
                if (dictPerformance.ContainsKey("getSendDsByFuncName"))
                {
                    dictPerformance["getSendDsByFuncName"].CallCount++;
                    dictPerformance["getSendDsByFuncName"].InCount++;
                }

                return ((Comm.Protocol)Host.Program.ScriptMgr[devType]).GetSendDataSet(funcName);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("getSendDsByFuncName"))
                {
                   // dictPerformance["getSendDsByFuncName"].CallCount++;
                    dictPerformance["getSendDsByFuncName"].InCount--;
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

        public DateTime getDateTime()
      {
          try
          {
              if (dictPerformance.ContainsKey("getDateTime"))
              {
                  dictPerformance["getDateTime"].CallCount++;
                  dictPerformance["getDateTime"].InCount++;
              }

              return System.DateTime.Now;
          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
              throw new RemoteException(ex.Message + ex.StackTrace);
          }
          finally
          {
              if (dictPerformance.ContainsKey("getDateTime"))
              {
                  dictPerformance["getDateTime"].CallCount++;
                  dictPerformance["getDateTime"].InCount--;
              }
          }
        }


        public string getRedirectStatusString()
        {
            try
            {
                if (dictPerformance.ContainsKey("getRedirectStatusString"))
                {
                    dictPerformance["getRedirectStatusString"].CallCount++;
                    dictPerformance["getRedirectStatusString"].InCount++;
                }

                return Program.matrix.route_mgr.ToString();
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("getRedirectStatusString"))
                {
                    //dictPerformance["getRedirectStatusString"].CallCount++;
                    dictPerformance["getRedirectStatusString"].InCount--;
                }
            }
        }


        public void setRMS_RampControl_Start(string devName)
        {
            try
            {

                if (dictPerformance.ContainsKey("setRMS_RampControl_Start"))
                {
                    dictPerformance["setRMS_RampControl_Start"].CallCount++;
                    dictPerformance["setRMS_RampControl_Start"].InCount++;
                }

                Program.matrix.rampctl_mgr.set_start(Program.matrix.getDeviceWrapper(devName) as RMSDeviceWrapper);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setRMS_RampControl_Start"))
                {
                    //dictPerformance["setRMS_RampControl_Start"].CallCount++;
                    dictPerformance["setRMS_RampControl_Start"].InCount--;
                }
            }
            //  Program.matrix.ltr_mgr.setLTR_start(devName);
        }

        public void setRMS_RampControl_Stop(string devName)
        {
            try
            {
                if (dictPerformance.ContainsKey("setRMS_RampControl_Stop"))
                {
                    dictPerformance["setRMS_RampControl_Stop"].CallCount++;
                    dictPerformance["setRMS_RampControl_Stop"].InCount++;
                }

                Program.matrix.rampctl_mgr.setRampControl_stop(Program.matrix.getDeviceWrapper(devName) as RMSDeviceWrapper);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setRMS_RampControl_Stop"))
                {
                    //dictPerformance["setRMS_RampControl_Stop"].CallCount++;
                    dictPerformance["setRMS_RampControl_Stop"].InCount--;
                }
            }
        }

        public void setRMS_LTR_Start(string devName)
        {
            try
            {
                if (dictPerformance.ContainsKey("setRMS_LTR_Start"))
                {
                    dictPerformance["setRMS_LTR_Start"].CallCount++;
                    dictPerformance["setRMS_LTR_Start"].InCount++;
                }

                Program.matrix.ltr_mgr.setLTR_start(Program.matrix.getDeviceWrapper(devName) as RMSDeviceWrapper);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setRMS_LTR_Start"))
                {
                   // dictPerformance["setRMS_LTR_Start"].CallCount++;
                    dictPerformance["setRMS_LTR_Start"].InCount--;
                }

            }
          //  Program.matrix.ltr_mgr.setLTR_start(devName);
        }

        public void setRMS_LTR_Stop(string devName)
        {
            try
            {
                if (dictPerformance.ContainsKey("setRMS_LTR_Stop"))
                {
                    dictPerformance["setRMS_LTR_Stop"].CallCount++;
                    dictPerformance["setRMS_LTR_Stop"].InCount++;
                }

                Program.matrix.ltr_mgr.setLTR_stop(Program.matrix.getDeviceWrapper(devName) as RMSDeviceWrapper);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setRMS_LTR_Stop"))
                {
                    //dictPerformance["setRMS_LTR_Stop"].CallCount++;
                    dictPerformance["setRMS_LTR_Stop"].InCount--;
                }
            }
        }
    
        public void setVDFiveMinData(string devName, RemoteInterface.MFCC.VD1MinCycleEventData data)
        {


            try
            {
                if (dictPerformance.ContainsKey("setVDFiveMinData"))
                {
                    dictPerformance["setVDFiveMinData"].CallCount++;
                    dictPerformance["setVDFiveMinData"].InCount++;
                }
                //((VDDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).Set5MinAvgData(data);
                Program.matrix.vd5minavg_mgr.Set5MinAvgData(data);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setVDFiveMinData"))
                {
                  //  dictPerformance["setVDFiveMinData"].CallCount++;
                    dictPerformance["setVDFiveMinData"].InCount--;
                }
            }
           
        }

        public void setRDEventData(string devName, DateTime dt, int pluviometric, int degree)
        {
            try
            {
                if (dictPerformance.ContainsKey("setRDEventData"))
                {
                    dictPerformance["setRDEventData"].CallCount++;
                    dictPerformance["setRDEventData"].InCount++;
                }

                ((RDDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).setEventData(dt, pluviometric, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setRDEventData"))
                {
                   // dictPerformance["setRDEventData"].CallCount++;
                    dictPerformance["setRDEventData"].InCount--;
                }
            }
        }

        public void setWDEventData(string devName, DateTime dt, int average_wind_speed, int average_wind_direction, int max_wind_speed, int max_wind_direction, int degree)
        {
            try
            {
                if (dictPerformance.ContainsKey("setWDEventData"))
                {
                    dictPerformance["setWDEventData"].CallCount++;
                    dictPerformance["setWDEventData"].InCount++;
                }
                ((WDDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).setEventData(dt, average_wind_speed, average_wind_direction, max_wind_speed, max_wind_direction, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setWDEventData"))
                {
                   // dictPerformance["setWDEventData"].CallCount++;
                    dictPerformance["setWDEventData"].InCount--;
                }
            }
        }

        public void setVIEventData(string devName, DateTime dt, int distance, int degree)
        {
            try
            {
                if (dictPerformance.ContainsKey("setVIEventData"))
                {
                    dictPerformance["setVIEventData"].CallCount++;
                    dictPerformance["setVIEventData"].InCount++;

                }
                ((VIDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).setEventData(dt, distance, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setVIEventData"))
                {
                    //dictPerformance["setVIEventData"].CallCount++;
                    dictPerformance["setVIEventData"].InCount--;

                }
            }
        }

        public void setBS_EventData(string devName, DateTime dt, int slope, int shift, int sink, int degree)
        {

            try
            {
                if (dictPerformance.ContainsKey("setBS_EventData"))
                {
                    dictPerformance["setBS_EventData"].CallCount++;
                    dictPerformance["setBS_EventData"].InCount++;
                }
                ((BSDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).setEventData(dt, slope, shift, sink, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setBS_EventData"))
                {
                   // dictPerformance["setBS_EventData"].CallCount++;
                    dictPerformance["setBS_EventData"].InCount--;
                }
            }

        }



        public void setRDFiveMinData(string devName, System.DateTime dt,int amount, int acc_amount, int degree)
        {
            try
            {
                if (dictPerformance.ContainsKey("setRDFiveMinData"))
                {
                    dictPerformance["setRDFiveMinData"].CallCount++;
                    dictPerformance["setRDFiveMinData"].InCount++;
                }
                ((RDDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).set5MinData(dt, amount, acc_amount, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setRDFiveMinData"))
                {
                   // dictPerformance["setRDFiveMinData"].CallCount++;
                    dictPerformance["setRDFiveMinData"].InCount--;
                }
            }
        }

        public void setVIFiveMinData(string devName, System.DateTime dt, int distance,  int degree)
        {
            try
            {

                if (dictPerformance.ContainsKey("setVIFiveMinData"))
                {
                    dictPerformance["setVIFiveMinData"].CallCount++;
                    dictPerformance["setVIFiveMinData"].InCount++;
                }
                ((VIDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).set5MinData(dt, distance, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setVIFiveMinData"))
                {
                   // dictPerformance["setVIFiveMinData"].CallCount++;
                    dictPerformance["setVIFiveMinData"].InCount--;
                }
            }
        }

        public void setWDTenMinData(string devname, System.DateTime dt, int average_wind_speed, int average_wind_direction, int max_wind_speed, int max_wind_direction, int degree)
        {
            try
            {

                if (dictPerformance.ContainsKey("setWDTenMinData"))
                {
                    dictPerformance["setWDTenMinData"].CallCount++;
                    dictPerformance["setWDTenMinData"].InCount++;
                }

                ((WDDeviceWrapper)Program.matrix.getDeviceWrapper(devname)).set10MinData(dt, average_wind_speed, average_wind_direction, max_wind_speed, max_wind_direction, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {

                if (dictPerformance.ContainsKey("setWDTenMinData"))
                {
                   // dictPerformance["setWDTenMinData"].CallCount++;
                    dictPerformance["setWDTenMinData"].InCount--;
                }
            }
        }

        public string getScriptSource(string devType)//取得script source 文字
        {
            try
            {
                if (dictPerformance.ContainsKey("getScriptSource"))
                {
                    dictPerformance["getScriptSource"].CallCount++;
                    dictPerformance["getScriptSource"].InCount++;
                }
                return Program.ScriptMgr[devType].getScriptSource();
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("getScriptSource"))
                {
                    //dictPerformance["getScriptSource"].CallCount++;
                    dictPerformance["getScriptSource"].InCount--;
                }
            }
        }


        public Comm.SendPackage getSendPackage(string devType, DataSet ds, int address)
        {
            try
            {
                if (dictPerformance.ContainsKey("getSendPackage"))
                {

                    dictPerformance["getSendPackage"].CallCount++;
                    dictPerformance["getSendPackage"].InCount++;
                }
                return Program.ScriptMgr[devType].GetSendPackage(ds, address);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("getSendPackage"))
                {

                    //dictPerformance["getSendPackage"].CallCount++;
                    dictPerformance["getSendPackage"].InCount--;
                }
            }
        }

        public void ResetComm(string devName)
        {
            try
            {
                if (dictPerformance.ContainsKey("ResetComm"))
                {
                    dictPerformance["ResetComm"].CallCount++;
                    dictPerformance["ResetComm"].InCount++;
                }

                Program.matrix.getDeviceWrapper(devName).getRemoteObj().ResetComm(devName);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("ResetComm"))
                {
                    //dictPerformance["ResetComm"].CallCount++;
                    dictPerformance["ResetComm"].InCount--;
                }
            }
        }
        public string getTCCommStatusStr(string devName)
        {

            try
            {
                if (dictPerformance.ContainsKey("getTCCommStatusStr"))
                {
                    dictPerformance["getTCCommStatusStr"].CallCount++;
                    dictPerformance["getTCCommStatusStr"].InCount++;
                }


                return Program.matrix.getDeviceWrapper(devName).getRemoteObj().getCurrentTcCommStatusStr(devName);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("getTCCommStatusStr"))
                {
                   // dictPerformance["getTCCommStatusStr"].CallCount++;
                    dictPerformance["getTCCommStatusStr"].InCount--;
                }
            }
           
        }


        public string getEventString()
        {
            try
            {
                if (dictPerformance.ContainsKey("getEventString"))
                {
                    dictPerformance["getEventString"].CallCount++;
                    dictPerformance["getEventString"].InCount++;

                }
                if (Program.matrix.event_mgr == null)
                    throw new Exception(" Event Manager 尚未啟動!");
                return Program.matrix.event_mgr.ToString();

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + "," + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("getEventString"))
                {
                   // dictPerformance["getEventString"].CallCount++;
                    dictPerformance["getEventString"].InCount--;

                }
            }
        }

        //public override object InitializeLifetimeService()
        //{
        //    return null;
        //}


        public void ReStartFiveMinAvgDataManager()
        {
            try
            {
                if (Program.matrix.vd5minavg_mgr != null)
                    Program.matrix.vd5minavg_mgr.Close();
                Program.matrix.vd5minavg_mgr = new FiveMinVDAVGDataManager();
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + "," + ex.StackTrace);
            }

        }

        public string getTIMCCSecManagerStatus()
        {
            try
            {
               return Program.matrix.timcc_section_mgr.ToString();
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + "," + ex.StackTrace);
            }
        }

        public string GetAllVDInfo()
        {
            string ret = "";
            try
            {
                ret = "FiveMinQueueCnt:" + Program.matrix.vd5minavg_mgr.VDFiveMinQueueCnt + "\r\n";
                ret += "state:" + Program.matrix.vd5minavg_mgr.state+"\r\n";
                ret += "LastDeviceName:" + Program.matrix.vd5minavg_mgr.LastDeviceName + "\r\n";
                ret += "LastVDDateTime:" + Program.matrix.vd5minavg_mgr.LastVDDateTime + "\r\n";
                ret += "LastErrorMsg:" + Program.matrix.vd5minavg_mgr.LastErrorMsg + "\r\n";
                foreach (DeviceBaseWrapper dev in Program.matrix.device_mgr.getDataDeviceEnum())
                {
                    if (dev is VDDeviceWrapper)
                    {
                        VDDeviceWrapper vddev = dev as VDDeviceWrapper;
                        ret +=   vddev.ToString()+"\r\n";

                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + "," + ex.StackTrace);
            }
            
        }
        public string getJamRangeString()
        {
            try
            {
                if (dictPerformance.ContainsKey("getJamRangeString"))
                {
                    dictPerformance["getJamRangeString"].CallCount++;
                    dictPerformance["getJamRangeString"].InCount++;
                }
                if (Program.matrix.jammgr == null)
                    throw new Exception(" Jam Manager 尚未啟動!");
                return Program.matrix.jammgr.ToString();

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + "," + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("getJamRangeString"))
                {
                  //  dictPerformance["getJamRangeString"].CallCount++;
                    dictPerformance["getJamRangeString"].InCount--;
                }
            }
        }


        public System.Collections.ArrayList getDeviceNames(string devType)
        {
            try
            {
                if (dictPerformance.ContainsKey("getDeviceNames"))
                {
                    dictPerformance["getDeviceNames"].CallCount++;
                    dictPerformance["getDeviceNames"].InCount++;
                }


                System.Collections.ArrayList ary = new System.Collections.ArrayList();
                foreach (DeviceBaseWrapper dev in Program.matrix.getDevicemanager().getAllDeviceEnum())
                {
                    if (dev.deviceType == devType)
                        ary.Add(dev.deviceName);

                }
                return ary;
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("getDeviceNames"))
                {
                  //  dictPerformance["getDeviceNames"].CallCount++;
                    dictPerformance["getDeviceNames"].InCount--;
                }
            }
        }

        public void setConnecttionStatus(string devName,bool isConnect)
        {
            try
            {
                if (dictPerformance.ContainsKey("setConnecttionStatus"))
                {
                    dictPerformance["setConnecttionStatus"].CallCount++;
                    dictPerformance["setConnecttionStatus"].InCount++;

                }
                Program.matrix.getDeviceWrapper(devName).IsConnected = isConnect;
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setConnecttionStatus"))
                {
                  //  dictPerformance["setConnecttionStatus"].CallCount++;
                    dictPerformance["setConnecttionStatus"].InCount--;

                }
            }

        }
        public void setDeviceStatus(string devName,byte[]hw_status,byte opstatus,byte opmode,bool isConnected)
        {
            try
            {
                if (dictPerformance.ContainsKey("setDeviceStatus"))
                {

                    dictPerformance["setDeviceStatus"].CallCount++;
                    dictPerformance["setDeviceStatus"].InCount++;
                }
                DeviceBaseWrapper dev = Program.matrix.getDeviceWrapper(devName);
                if (dev != null)
                    dev.set_HW_status(hw_status, opmode, opmode, isConnected);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setDeviceStatus"))
                {

                  //  dictPerformance["setDeviceStatus"].CallCount++;
                    dictPerformance["setDeviceStatus"].InCount--;
                }
            }
        }

        public void setIID_EventData(int  laneid,int  camid,int eventid,int action_type )
        {
            try
            {
                if (dictPerformance.ContainsKey("setIID_EventData"))
                {
                    dictPerformance["setIID_EventData"].CallCount++;
                    dictPerformance["setIID_EventData"].InCount++;
                }
                Program.matrix.iid_mgr.setCamEvent(camid, laneid, eventid, action_type);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setIID_EventData"))
                {
                    //dictPerformance["setIID_EventData"].CallCount++;
                    dictPerformance["setIID_EventData"].InCount--;
                }
            }
        }

      
        public string getAllSchduleStatus()
        {
            try
            {
                if (dictPerformance.ContainsKey("getAllSchduleStatus"))
                {
                    dictPerformance["getAllSchduleStatus"].CallCount++;
                    dictPerformance["getAllSchduleStatus"].InCount++;
                }
                return Schedule.ScheduleManager.getAllScheduleStatus();

            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (dictPerformance.ContainsKey("getAllSchduleStatus"))
                {
                   // dictPerformance["getAllSchduleStatus"].CallCount++;
                    dictPerformance["getAllSchduleStatus"].InCount--;
                }
            }
        }

              


        public void AddAviData(AVIPlateData data)
        {
            try
            {
                if (dictPerformance.ContainsKey("AddAviData"))
                {
                    dictPerformance["AddAviData"].CallCount++;
                    dictPerformance["AddAviData"].InCount++;
                }
                if (Program.matrix.avimgr == null)
                    return;

                Program.matrix.avimgr.AddAviData(data);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("AddAviData"))
                {
                   // dictPerformance["AddAviData"].CallCount++;
                    dictPerformance["AddAviData"].InCount--;
                }
            }
        }


        public void DoVD_InteropData(string devName, System.DateTime dt)
        {
            try
            {
               // return;
                if (dictPerformance.ContainsKey("DoVD_InteropData"))
                {
                    dictPerformance["DoVD_InteropData"].CallCount++;
                    dictPerformance["DoVD_InteropData"].InCount++;
                }

                Program.matrix.jammgr.DoVD_InteropData(devName, dt);

                System.Console.WriteLine(devName + " ," + dt.ToShortTimeString() + " 資料差補");

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("DoVD_InteropData"))
                {
                   // dictPerformance["DoVD_InteropData"].CallCount++;
                    dictPerformance["DoVD_InteropData"].InCount--;
                }
            }
            
        }


        public void setTemEvent(string devName, object temEventData)
        {
            try
            {
                if (dictPerformance.ContainsKey("setTemEvent"))
                {
                    dictPerformance["setTemEvent"].CallCount++;
                    dictPerformance["setTemEvent"].InCount++;
                }
                Program.matrix.tem_mgr.setEventData(devName, temEventData);


            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setTemEvent"))
                {
                    //dictPerformance["setTemEvent"].CallCount++;
                    dictPerformance["setTemEvent"].InCount--;
                }
            }


        }




         public  void setIIDEvent(int camid,int laneid,int iideventid, int action)
        {

            try
            {
                if (dictPerformance.ContainsKey("setIIDEvent"))
                {
                    dictPerformance["setIIDEvent"].CallCount++;
                    dictPerformance["setIIDEvent"].InCount++;
                }
                Program.matrix.iid_mgr.setCamEvent(camid, laneid, iideventid, action);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setIIDEvent"))
                {
                   // dictPerformance["setIIDEvent"].CallCount++;
                    dictPerformance["setIIDEvent"].InCount--;
                }
            }
          


        }







   

        public void setLSEventData(string devName,DateTime dt, int mon_var, int day_var, int degree)
        {
            try
            {
                if (dictPerformance.ContainsKey("setLSEventData"))
                {
                    dictPerformance["setLSEventData"].CallCount++;
                    dictPerformance["setLSEventData"].InCount++;
                }
                ((LSDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).setEventData(dt, mon_var, day_var, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setLSEventData"))
                {
                    //dictPerformance["setLSEventData"].CallCount++;
                    dictPerformance["setLSEventData"].InCount--;
                }
            }
        }

        public void setLS10MinData(string devName,DateTime dt, int mon_var, int day_var, int degree)
        {
            try
            {
                if (dictPerformance.ContainsKey("setLS10MinData"))
                {
                    dictPerformance["setLS10MinData"].CallCount++;
                    dictPerformance["setLS10MinData"].InCount++;
                }
                ((LSDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).set10MinData(dt, mon_var, day_var, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("setLS10MinData"))
                {
                   // dictPerformance["setLS10MinData"].CallCount++;
                    dictPerformance["setLS10MinData"].InCount--;
                }
            }
        }

        public int getTEMLCSStatus(string devName)
        {

            try
            {
                if (dictPerformance.ContainsKey("getTEMLCSStatus"))
                {
                    dictPerformance["getTEMLCSStatus"].CallCount++;
                    dictPerformance["getTEMLCSStatus"].InCount++;
                }
                return ((LCSDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).getTEMStatus();

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("getTEMLCSStatus"))
                {
                    //dictPerformance["getTEMLCSStatus"].CallCount++;
                    dictPerformance["getTEMLCSStatus"].InCount--;
                }
            }
        }


       public   int getCurrentDBQueueCnt()
        {
            try
            {
                if (dictPerformance.ContainsKey("getCurrentDBQueueCnt"))
                {
                    dictPerformance["getCurrentDBQueueCnt"].CallCount++;
                    dictPerformance["getCurrentDBQueueCnt"].InCount++;
                }
                return Program.matrix.dbServer.getCurrentQueueCnt();

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            finally
            {
                if (dictPerformance.ContainsKey("getCurrentDBQueueCnt"))
                {
                   // dictPerformance["getCurrentDBQueueCnt"].CallCount++;
                    dictPerformance["getCurrentDBQueueCnt"].InCount--;
                }
            }
        }
       public  void dbExecute(string sqlcmd)
       {
           try
           {
               if (dictPerformance.ContainsKey("dbExecute"))
               {
                   dictPerformance["dbExecute"].CallCount++;
                   dictPerformance["dbExecute"].InCount++;
               }
               Program.matrix.dbServer.SendSqlCmd(sqlcmd);

           }
           catch (Exception ex)
           {
               throw new RemoteException(ex.Message);
           }
           finally
           {
               if (dictPerformance.ContainsKey("dbExecute"))
               {
                  // dictPerformance["dbExecute"].CallCount++;
                   dictPerformance["dbExecute"].InCount--;
               }
           }
       }
    }
}
