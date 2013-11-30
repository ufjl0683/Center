using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;
using System.Data.Odbc;
using RemoteInterface;

namespace Host.TC
{
    public  abstract class OutPutDeviceBase:TC.DeviceBaseWrapper
    {

         int alarmCode = 1;
        System.Timers.Timer tmrChkRSP_Output = new System.Timers.Timer(1000*60);
     protected   System.Collections.Hashtable outputQueue = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());

        public OutPutDeviceBase(string mfccid, string devicename, string deviceType, string ip, int port, string location,string lineid, int mile_m,byte[]hw_status,byte opmode,byte opstatus,string direction)
         : base(mfccid, devicename, deviceType, ip, port, location, lineid,mile_m,hw_status,opmode,opstatus,direction)
           
        {


            LoadManualData();


            tmrChkRSP_Output.Elapsed += new System.Timers.ElapsedEventHandler(tmrChkRSP_Output_Elapsed);
            tmrChkRSP_Output.Start();

        }


   protected   virtual   void tmrChkRSP_Output_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            object[] qdatas;
#if DEBUG
            
            //if (this.deviceName != "CMS-N6-E-17.3")
            //    return;
#endif
           qdatas=new object[outputQueue.Count];

           outputQueue.CopyTo(qdatas, 0);
          // System.Collections.IDictionaryEnumerator ie = outputQueue.GetEnumerator();
          //  while(ie.MoveNext())
            foreach( System.Collections.DictionaryEntry entity in qdatas)
            {
                OutputQueueData data = entity.Value as OutputQueueData;
                if (data.mode == OutputModeEnum.ResponsePlanMode)
                {
                    OdbcConnection cn= new OdbcConnection(Global.Db2ConnectionString);
                    try
                    {
                       
                        OdbcCommand cmd = new OdbcCommand(string.Format("select count(*)  from tblrspexecutionoutputdata where devicename='{0}' and eventid={1}", this.deviceName, data.ruleid));
                        cmd.Connection = cn;
                        cn.Open();
                        int cnt = System.Convert.ToInt32(cmd.ExecuteScalar());
                        if (cnt == 0)
                        {
                            this.outputQueue.Remove(data.ruleid);
                            this.output();
                            break;

                        }


                    }
                    catch (Exception ex)
                    {
                        RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                    }
                    finally
                    {
                        cn.Close();
                    }



                }




            }



        }



        protected  void  setAlarmCode(int alarmCode)
        {

           // return;
            if (alarmCode == 1)
            {
                this.alarmCode = 1;  //normal
                return;
            }
            if(this.alarmCode ==alarmCode)
                return;

            this.alarmCode = alarmCode;
            string sql = "insert into  tblDeviceComparisonLog (devicename,timestamp,alarmcode,display,device_display) values('{0}','{1}',{2},'','')";

           Program.matrix.dbServer.SendSqlCmd(string.Format(sql,this.deviceName,RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now),alarmCode));


        }
        
        protected virtual void EnOutputQueue(OutputQueueData data)
        {
            if (outputQueue.Contains(data.ruleid))
                outputQueue.Remove(data.ruleid);
            outputQueue.Add(data.ruleid,data);
            //候對中
            if (data.mode == OutputModeEnum.ManualMode || data.mode == OutputModeEnum.ForceManualMode)
                SaveManualData(data);

        }

        public virtual OutputQueueData[] getAllOutputQueueData()
        {

            if (this.deviceType == "RMS")
            {
                return this.GetPriorityQueueData(new RMS_Comparer());
            }
            else if (this.deviceType == "CSLS")
                return this.GetPriorityQueueData(new CSLS_Comparer());
            else if (this.deviceType == "FS")
                return this.GetPriorityQueueData(new FS_Comparer());
            else if (this.deviceType == "MAS")
                return this.GetPriorityQueueData(new MAS_Comparer());
            else if (this.deviceType == "WIS")
                return this.GetPriorityQueueData(new WIS_Comparer());
            else

                return this.GetPriorityQueueData();
            //System.Collections.ArrayList ary = new System.Collections.ArrayList();

           
            //if (outputQueue.Count == 0)
            //    return null;
            //else
            //{

            //    System.Collections.IEnumerator ie = outputQueue.GetEnumerator();
            //    while (ie.MoveNext())
            //    {
            //        OutputQueueData quedata = (OutputQueueData)((System.Collections.DictionaryEntry)ie.Current).Value;


            //        ary.Add(quedata);
                  
            //    }
            //}

            //ary.Sort();
            //object[] data = ary.ToArray();

            //OutputQueueData[] retobjs = new OutputQueueData[data.Length];

            //for (int i = 0; i < data.Length; i++)
            //    retobjs[i] =(OutputQueueData)data[i];

            //return retobjs;

        }
        public OutputQueueData GetQueueData(int ruleid)
        {
            if(!outputQueue.Contains(ruleid))
                return null;
            return outputQueue[ruleid] as OutputQueueData;

        }


        private void LoadManualData()
        {
            OutputQueueData data;

            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();
            System.Data.Odbc.OdbcDataReader rd;
            cmd.Connection = cn;
            cmd.CommandText = "select manualData,ForcemanualData from tbldeviceManualOutputData where devicename='" + this.deviceName + "'";
            try
            {
                cn.Open();
                rd = cmd.ExecuteReader();
                rd.Read();
                if (!rd.IsDBNull(1))
                {
                    data =(OutputQueueData) RemoteInterface.Util.getObjectByHexString(rd[1].ToString());
                    this.SetOutput(data);
                }
                else if(!rd.IsDBNull(0))
                {
                    data = (OutputQueueData)RemoteInterface.Util.getObjectByHexString(rd[0].ToString());
                    this.SetOutput(data);
                }
                   

            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message);
            }
            finally
            {
                cn.Close();
            }

        }


        private void SaveManualData(OutputQueueData data)
        {

            string sql = "";

            if (data.mode == OutputModeEnum.ManualMode)
           
                sql = "update tblDeviceManualOutputData set Manualdata='" + RemoteInterface.Util.getObjectHexString(data) + "' where devicename='" + this.deviceName + "'";
           
            else
                sql = "update tblDeviceManualOutputData set ForceManualdata='" + RemoteInterface.Util.getObjectHexString(data) + "' where devicename='" + this.deviceName + "'";

#if !DEBUG

            Program.matrix.dbServer.SendSqlCmd(sql);
#endif

        }

       
        public   void  SetOutput(OutputQueueData data)
        {
            data.OnStatusChange += (sender, arg) =>
            {

                OutputQueueData qdata = sender as OutputQueueData;
                if (qdata.mode != OutputModeEnum.ResponsePlanMode)
                    return;

                string sql = "update tblrspexecutionoutputdata set status={0} ,Success='{1}' where eventid={2} and devicename='{3}'";
                Program.matrix.dbServer.SendSqlCmd(string.Format(sql, qdata.status, qdata.IsSuccess ? "Y" : "N", qdata.ruleid, this.deviceName));
            };
            data.status = 0;
            EnOutputQueue( data);
          
            if (this.IsRemoteObjectConnect())
                output();
        }

        public void InvokeOutputDataStatusChange(OutputQueueData qdata)
        {
            return;
             if (qdata.mode != OutputModeEnum.ResponsePlanMode)
                    return;
            string sql = "update tblrspexecutionoutputdata set status={0} ,Success='{1}' where eventid={2} and devicename='{3}'";
            Program.matrix.dbServer.SendSqlCmd(string.Format(sql, qdata.status, qdata.IsSuccess ? "Y" : "N", qdata.ruleid, this.deviceName));
            
        }


        //public void SetOutputOff()
        //{
        //}


        public  virtual void removeOutput(int ruleId)
        {
            if (outputQueue.Contains(ruleId))
            {
                this.outputQueue.Remove(ruleId);
                output();
            }
        }

        //public virtual void RemoveOutputWithoutRefresh(int ruleId)
        //{
        //    if (outputQueue.Contains(ruleId))
        //    {
        //        this.outputQueue.Remove(ruleId);
             
        //    }
        //}

        protected OutputQueueData[] GetPriorityQueueData()
        {

            System.Collections.ArrayList ary = new System.Collections.ArrayList();
            if (this.outputQueue.Count == 0)
                return new OutputQueueData[0];
            else
            {

                System.Collections.IEnumerator ie = outputQueue.GetEnumerator();
                while (ie.MoveNext())
                {
                    OutputQueueData quedata = (OutputQueueData)((System.Collections.DictionaryEntry)ie.Current).Value;


                    ary.Add(quedata);
                }

            }

         
            ary.Sort();
            OutputQueueData[] data = new OutputQueueData[ary.Count];
            for (int i = 0; i < ary.Count; i++)
                data[i] = ary[i] as OutputQueueData;


            return data;
        }
        protected OutputQueueData[] GetPriorityQueueData(System.Collections.IComparer I_compare)
        {

            System.Collections.ArrayList ary = new System.Collections.ArrayList();
            if (outputQueue.Count == 0)
                return new OutputQueueData[0];
            else
            {
                
                System.Collections.IEnumerator ie = outputQueue.GetEnumerator();
                while (ie.MoveNext())
                {
                    OutputQueueData quedata = (OutputQueueData)((System.Collections.DictionaryEntry)ie.Current).Value;


                    ary.Add(quedata);
                }

            }


            ary.Sort(I_compare);
            OutputQueueData[] data = new OutputQueueData[ary.Count];
            for (int i = 0; i < ary.Count; i++)
                data[i] = ary[i] as OutputQueueData;


            return data;
        }



        public virtual OutputQueueData getOutputdata()
        {
            //System.Collections.ArrayList ary = new System.Collections.ArrayList();




            if (outputQueue.Count == 0)
                return null;

            OutputQueueData[] data = this.getAllOutputQueueData();
          
                return data[data.Length-1];

        }
        public abstract void output();  // output to mfcc
    }
}
