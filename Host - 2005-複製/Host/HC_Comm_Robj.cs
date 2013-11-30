using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;
using RemoteInterface;
using RemoteInterface.MFCC;

using System.Data;
using Host.TC;

namespace Host
{
    class HC_Comm_Robj:RemoteInterface.RemoteClassBase,I_HC_Comm
    {
      
      public   System.Data.DataSet getSendDsByFuncName(string devType,string funcName)
        {
            try{

               return   ((Comm.Protocol)Host.Program.ScriptMgr[devType]).GetSendDataSet(funcName);

            }catch(Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }


        public DateTime getDateTime()
        {
            return System.DateTime.Now;
        }

        public void setVDFiveMinData(string devName, RemoteInterface.MFCC.VD1MinCycleEventData data)
        {


            try
            {

                ((VDDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).Set5MinAvgData(data);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
           
        }

        public void setRDEventData(string devName, DateTime dt, int pluviometric, int degree)
        {
            try
            {

                ((RDDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).setEventData(dt,  pluviometric, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }

        public void setWDEventData(string devName, DateTime dt, int average_wind_speed, int average_wind_direction, int max_wind_speed, int max_wind_direction, int degree)
        {
            try
            {

                ((WDDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).setEventData(dt,average_wind_speed,average_wind_direction,max_wind_speed,max_wind_direction, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }

        public void setVIEventData(string devName, DateTime dt, int distance, int degree)
        {
            try
            {

                ((VIDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).setEventData(dt,distance , degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }

        public void setBS_EventData(string devName, DateTime dt, int slope, int shift, int sink, int degree)
        {

            try
            {

                ((BSDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).setEventData(dt, slope,shift,sink, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }

        }



        public void setRDFiveMinData(string devName, System.DateTime dt,int amount, int acc_amount, int degree)
        {
            try
            {

                ((RDDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).set5MinData(dt, amount, acc_amount, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }

        public void setVIFiveMinData(string devName, System.DateTime dt, int distance,  int degree)
        {
            try
            {

                ((VIDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).set5MinData(dt, distance, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }

        public void setWDTenMinData(string devname, System.DateTime dt, int average_wind_speed, int average_wind_direction, int max_wind_speed, int max_wind_direction, int degree)
        {
            try
            {

                ((WDDeviceWrapper)Program.matrix.getDeviceWrapper(devname)).set10MinData(dt, average_wind_speed,average_wind_direction,max_wind_speed,max_wind_direction, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }

        public string getScriptSource(string devType)//取得script source 文字
        {
            try
            {
                return Program.ScriptMgr[devType].getScriptSource();
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }


        public Comm.SendPackage getSendPackage(string devType, DataSet ds, int address)
        {
            try
            {
                return Program.ScriptMgr[devType].GetSendPackage(ds, address);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }

        public void ResetComm(string devName)
        {
            try
            {

                Program.matrix.getDeviceWrapper(devName).getRemoteObj().ResetComm(devName);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }
        public string getTCCommStatusStr(string devName)
        {

            try
            {

               return  Program.matrix.getDeviceWrapper(devName).getRemoteObj().getCurrentTcCommStatusStr(devName);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
           
        }


        public string getEventString()
        {
            try
            {

                if (Program.matrix.event_mgr == null)
                    throw new Exception(" Event Manager 尚未啟動!");
                return Program.matrix.event_mgr.ToString();

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message + "," + ex.StackTrace);
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
        public string getJamRangeString()
        {
            try
            {

                if (Program.matrix.jammgr == null)
                    throw new Exception(" Jam Manager 尚未啟動!");
                return Program.matrix.jammgr.ToString();

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message+","+ex.StackTrace);
            }
        }


        public System.Collections.ArrayList getDeviceNames(string devType)
        {
            try
            {
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
        }

        public void setConnecttionStatus(string devName,bool isConnect)
        {
            try
            {

                Program.matrix.getDeviceWrapper(devName).IsConnected = isConnect;
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                throw new RemoteException(ex.Message + ex.StackTrace);
            }

        }
        public void setDeviceStatus(string devName,byte[]hw_status,byte opstatus,byte opmode,bool isConnected)
        {
            try
            {

                DeviceBaseWrapper dev = Program.matrix.getDeviceWrapper(devName); 
                if(dev!=null)
                  dev.set_HW_status(hw_status, opmode, opmode,isConnected);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
        }

        public void setIID_EventData(int  laneid,int  camid,int eventid,int action_type )
        {
            try
            {
                Program.matrix.iid_mgr.setCamEvent(camid, laneid, eventid, action_type);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
        }

      
        public string getAllSchduleStatus()
        {
            try
            {
              return  Schedule.ScheduleManager.getAllScheduleStatus();
               
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                throw new RemoteException(ex.Message + ex.StackTrace);
            }
        }

              


        public void AddAviData(AVIPlateData data)
        {
            try
            {

                if (Program.matrix.avimgr == null)
                    return;

                Program.matrix.avimgr.AddAviData(data);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }


        public void DoVD_InteropData(string devName, System.DateTime dt)
        {
            try
            {

                Program.matrix.jammgr.DoVD_InteropData(devName, dt);

                System.Console.WriteLine(devName + " ," + dt.ToShortTimeString() + " 資料差補");

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
            
        }


        public void setTemEvent(string devName, object temEventData)
        {
            try
            {

                Program.matrix.tem_mgr.setEventData(devName, temEventData);

              
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }


        }




         public  void setIIDEvent(int camid,int laneid,int iideventid, int action)
        {

            try
            {

                Program.matrix.iid_mgr.setCamEvent(camid, laneid, iideventid, action);
            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
          


        }







   

        public void setLSEventData(string devName,DateTime dt, int mon_var, int day_var, int degree)
        {
            try
            {

                ((LSDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).setEventData(dt, mon_var, day_var, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }

        public void setLS10MinData(string devName,DateTime dt, int mon_var, int day_var, int degree)
        {
            try
            {

                ((LSDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).set10MinData(dt, mon_var, day_var, degree);

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }

        public int getTEMLCSStatus(string devName)
        {

            try
            {

                return ((LCSDeviceWrapper)Program.matrix.getDeviceWrapper(devName)).getTEMStatus();

            }
            catch (Exception ex)
            {
                throw new RemoteException(ex.Message);
            }
        }
    }
}
