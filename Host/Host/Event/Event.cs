using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using RemoteInterface.HC;
using RemoteInterface;

namespace Host.Event
{

    public enum EventTransition
    {
        End=0,
        Begin=1,
        RangeChange=2,
        LevelChange=3,
        NoChange=4
    }

    public enum EventMode
    {
        AutoAuto=  0xff , //0,     //IIP Auto , Exeuting Auto  0x ff
        HalfAuto = 0x3f, // 1,     //IIP halfAuto , Exeuting Auto  0x3f
        Manual=3,       
        DontCare= 0x00,//2,             //0x00
        AutoHalf= 0xf3,  //4,       // IIP Auto, Executing HalfAuto  0xf3
        HalfHalf=0x33, //5  ,      //IIP Half Auto  , Executing Half Auto  0x33
        AutoStop=0xf0,   //6,            // AutoStopv  0xF0
        HalfStop= 0x30  //  7                //HalfStop   0x 30
    }

    public enum EventStatus
    {
        Alarm=0,
        Confirm=1,
        PlanCheck=2,
        Executing=3,
        Closed=9,
        Abort=-1,
        ForceAbort=-2,
        Ignore=-3,
        Null=-9


    }


    public enum AlarmType
    {
        DEVICE=1,
        TRAFFIC=2,
        CONTROL=3,
        TUNNEL=4,
        RD=5,
        WD=6,
        VI=7,
        LS=8,
        BS=9,
        GEN,
        OTHER=99
    }


    public abstract class Event
    {

      
    

        private EventStatus _status= EventStatus.Null;
        private  int m_eventid=-1;
        private int m_org_eventid = -1;
        protected AlarmType m_alarm_type =  AlarmType.OTHER;

        protected int m_class=-1;

        protected  EventMode m_eventmode = EventMode.Manual;

        protected int m_level = 0;

        public   event System.EventHandler OnAbort;
        public event System.EventHandler OnAbortToManager;
        public event System.EventHandler OnDegreeChange;
        public event System.EventHandler OnRangeChange;

        public event System.EventHandler OnStop;

        public event System.EventHandler OnReNewEvent;

      //  protected string m_lineid,m_dir;

        protected bool m_isRenew = false;
        System.Collections.Generic.List<RemoteInterface.HC.OutputQueueData> lstOutputQueueData = new List<RemoteInterface.HC.OutputQueueData>();
        public string description;
      //  public string Description;
        public bool IsLock = false;
         public bool IsReload = false; //表試是否重載
       protected  Event()
        {

           //try{
           //  loadEventIdAndMode();
           //  // doModeControl(EventStatus.Alarm);
           //}
           //catch(Exception ex)

           //{
           //    this.m_eventmode= EventMode.DontCare;
           //    RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
           //}

           
        }

       public abstract string ToEventString();
      

        public EventStatus getEventStatus()
        {
            return EventStatus;
        }

        protected EventStatus EventStatus
        {
            get { return _status; }
            set
            {
                if (value != _status)
                {
                    _status = value;

                    
                    string sql = "update tblSysAlarmLog set status={1} where eventid={0}";
                    Program.matrix.dbServer.SendSqlCmd(string.Format(sql,this.EventId,(int)((_status== EventStatus.ForceAbort)?EventStatus.Abort:_status)));
                }
            }
        }


        public void executePlan()
        {


            OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
            OdbcCommand cmd = new OdbcCommand("select devicename,device_type,priority,outputdata1,outputdata2 from tblrspExecutionOutputData where delmark='N' and  eventid=" + this.EventId);
            OdbcDataReader rd;
            cmd.Connection = cn;
            
            try
            {
                clearLstOutputQueueData();
                cn.Open();
                rd = cmd.ExecuteReader();
              
                bool isIn = false;
                while (rd.Read())
                {
                    try
                    {
                        isIn = true;
                        RemoteInterface.HC.OutputQueueData qdata;
                        int priority = System.Convert.ToInt32(rd[2]);
                        string devName = rd[0].ToString();
                        object OutObj = null;

                        if (!rd.IsDBNull(4))
                        {
                            OutObj = RemoteInterface.Util.getObjectByHexString(rd[4].ToString());
                        }
                        else if (!rd.IsDBNull(3))
                        {
                            OutObj = RemoteInterface.Util.getObjectByHexString(rd[3].ToString());
                        }
                        else
                        {
                            RemoteInterface.Util.SysLog("evterr.log",this.EventId+","+"no outputdata and outputdata1");
                            continue;
                        }


                        Host.TC.OutPutDeviceBase dev = Program.matrix.getDeviceWrapper(devName) as Host.TC.OutPutDeviceBase;
                        qdata = new RemoteInterface.HC.OutputQueueData(devName, RemoteInterface.HC.OutputModeEnum.ResponsePlanMode, this.EventId, priority, OutObj);
                        qdata.HappenDir = this.getDir();
                        qdata.HappenLineID = this.getLineId();
                        qdata.HappenMileage = this.getStartMileM();
                        qdata.DevDir= dev.direction ;
                        qdata.DevLineId = dev.getLineID();
                        qdata.DevMileage = dev.getMileage();
                        (Program.matrix.getDeviceWrapper(devName) as Host.TC.OutPutDeviceBase).SetOutput(qdata);
                    //    RemoteInterface.Util.SysLog( DateTime.Now + "," +  "evterr.log", "output " + devName + ",");

                        lock (this.lstOutputQueueData)   //2013-04-27
                        this.lstOutputQueueData.Add(qdata);
                    }
                    catch (Exception ex)
                    {
                        RemoteInterface.Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);
                    }
                }
                if (!isIn)
                {
                    RemoteInterface.Util.SysLog("evterr.log", this.EventId + "," + "executeing table not found!");
                }
                rd.Close();
            }
            catch (Exception ex)
            {
                RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                RemoteInterface.Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);
            }
            finally
            {
                cn.Close();
            }
            //2011/4/13 for delay remove output
            lock (this.lstOutputQueueData)
            {
                System.Threading.Monitor.PulseAll(this.lstOutputQueueData);
            }


        }


        public  void  ReloadExecutionOutput()
        {
          
                this.clearLstOutputQueueData();
                Execution.Execution.getBuilder().GenerateExecutionTable(this.EventId);
           

        }
       
        public void clearLstOutputQueueData()
        {

            lock (this.lstOutputQueueData)  //2014-4-27
            {
                foreach (OutputQueueData data in lstOutputQueueData)  
                {
                    try
                    {
                        //  2011/4/13  for Delay Remove Output
                        //(Program.matrix.getDevicemanager()[data.DeviceName] as Host.TC.OutPutDeviceBase).removeOutput(data.ruleid);
                        new System.Threading.Thread(RemoveOutputJob).Start(data);
                    }
                    catch (Exception ex)
                    {
                        RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                    }
                }
                lstOutputQueueData.Clear();
            }
        }

        private void RemoveOutputJob(object qdata)
        {
            OutputQueueData data = qdata as OutputQueueData;
            lock (this.lstOutputQueueData)
            {
                try
                {
                    System.Threading.Monitor.Wait(this.lstOutputQueueData, 5000);
                    (Program.matrix.getDevicemanager()[data.DeviceName] as Host.TC.OutPutDeviceBase).removeOutput(data.ruleid);
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine("In RemoveOutputJob," + ex.Message);
                }
            }

        }


        private void doModeControl(EventStatus newStatus)
        {
            try
            {

                if (this.EventMode == EventMode.DontCare)
                    return;
                if (!IsAllowStatus(this.EventMode, EventStatus, newStatus))
                    return;

                if (this.m_class == 60)  //火警事件
                #region
                {
                    string[] cmsDevNames=new string[0];
                    string[] cmsMessages=new string[0];
                    string[] lcsDevNames=new string[0];
                    RemoteInterface.TEM.FireAlarmData fdata = ((TEM.TemRangeData)this).eventData as RemoteInterface.TEM.FireAlarmData;
                    if (Global.IsTunnelGearing(fdata.tunnel, fdata.place,ref cmsDevNames,ref cmsMessages,ref lcsDevNames))
                    {  //處理連動
                        for (int i = 0; i < cmsDevNames.Length; i++)
                        {
                           byte[]colors=new byte[cmsMessages[i].Length];
                            for(int j=0;j<colors.Length;j++)
                                colors[j]=0x20;
                            OutputQueueData outdata=new OutputQueueData(cmsDevNames[i],OutputModeEnum.ManualMode,OutputQueueData.MANUAL_RULE_ID,OutputQueueData.NORMAL_MANUAL_RULE_ID,
                                new CMSOutputData(0,0,0,cmsMessages[i],colors) );
                           
                            lock(this.lstOutputQueueData) // 2013-4-27  
                                 this.lstOutputQueueData.Add(outdata);
                            try
                            {
                                (Program.matrix.getDeviceWrapper(outdata.DeviceName) as TC.CMSDeviceWrapper).SetOutput(outdata);
                            }
                            catch (Exception ex)
                            {
                                Util.SysLog("evterr.log",ex.Message+","+ex.StackTrace);
                            }


                            System.Data.DataSet ds = Program.ScriptMgr["LCS"].GetSendDataSet("set_ctl_sign");
                            System.Data.DataRow r = ds.Tables[0].Rows[0];
                            r["sign_cnt"] = 2;
                          
                            for(int j=0;j<2;j++)
                            {
                             r = ds.Tables[1].NewRow();
                             r["sign_no"] = j;
                             r["sign_status"] = 5;  //1:green 2:red 5: flah green
                             ds.Tables[1].Rows.Add(r);
                            }

                            ds.AcceptChanges();
                            outdata = new OutputQueueData(lcsDevNames[i], OutputModeEnum.ManualMode, OutputQueueData.MANUAL_RULE_ID, OutputQueueData.NORMAL_MANUAL_RULE_ID,
                                new LCSOutputData(ds));
                            lock (this.lstOutputQueueData)     //2013-4-27 
                                     this.lstOutputQueueData.Add(outdata);
                            try
                            {
                                (Program.matrix.getDeviceWrapper(outdata.DeviceName) as TC.LCSDeviceWrapper).SetOutput(outdata);
                            }
                            catch (Exception ex)
                            {
                                Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);
                            }
                         
                        }


                    }
                }
                #endregion
                switch (this.EventMode)
                {

                    case EventMode.Manual:
                        this.executePlan();
                        break;
                    case EventMode.AutoAuto:

//#if DEBUG
                        Util.SysLog("sys.log", DateTime.Now+"prepare to execute:" + this.EventId);    
//#endif

                        Execution.Execution.getBuilder().InputIIP_Event(this.EventId);

//#if DEBUG
                        Util.SysLog("sys.log", DateTime.Now+" generate  plan:" + this.EventId);
//#endif
                            Execution.Execution.getBuilder().GenerateExecutionTable(this.EventId);


                        this.EventStatus = EventStatus.Executing;
//#if DEBUG
                        Util.SysLog("sys.log", DateTime.Now + " begin execute:" + this.EventId);
//#endif
                        this.executePlan();
//#if DEBUG
                        Util.SysLog("sys.log", DateTime.Now + " execute  completed:" + this.EventId);
//#endif
                        break;
                    case EventMode.DontCare:
                        this.EventStatus = EventStatus.Alarm;
                        break;
                    case EventMode.HalfAuto:


                        if (newStatus == EventStatus.Confirm)
                        {
                          //  Execution.Execution.getBuilder().InputIIP_Event(this.EventId);
                          
                                Execution.Execution.getBuilder().GenerateExecutionTable(this.EventId);
                             
                           
                          
                            this.EventStatus = EventStatus.Executing;
                            this.executePlan();

                        }
                        else if (newStatus == EventStatus.Alarm)
                            this.EventStatus = EventStatus.Alarm;


                        break;

                    case EventMode.HalfHalf:
                        if (newStatus == EventStatus.Confirm)
                        {
                           // Execution.Execution.getBuilder().InputIIP_Event(this.EventId);
                           
                                Execution.Execution.getBuilder().GenerateExecutionTable(this.EventId);
                           


                            EventStatus = EventStatus.Confirm;
                        }
                        else if (newStatus == EventStatus.PlanCheck)
                        {

                            this.EventStatus = EventStatus.Executing;
                            this.executePlan();

                        }
                        else if (newStatus == EventStatus.Alarm)
                            this.EventStatus = EventStatus.Alarm;

                        break;

                    case EventMode.AutoHalf:
                        if (newStatus == EventStatus.Alarm)
                        {
                           
                                Execution.Execution.getBuilder().InputIIP_Event(this.EventId);
                                Execution.Execution.getBuilder().GenerateExecutionTable(this.EventId);
                           

                            this.EventStatus = EventStatus.Confirm;
                        }
                        else if (newStatus == EventStatus.PlanCheck)
                        {

                            this.EventStatus = EventStatus.Executing;
                            this.executePlan();

                        }



                        break;

                    case Host.Event.EventMode.AutoStop:
                        if (newStatus == EventStatus.Alarm)
                        {
                           
                                Execution.Execution.getBuilder().InputIIP_Event(this.EventId);
                                Execution.Execution.getBuilder().GenerateExecutionTable(this.EventId);
                           

                            this.EventStatus = EventStatus.Ignore;
                        }
                        else if (newStatus == EventStatus.PlanCheck)
                        {

                          //  this.EventStatus = EventStatus.Executing;
                          //  this.executePlan();

                        }

                        break;

                    case Host.Event.EventMode.HalfStop:
                        if (newStatus == EventStatus.Confirm)
                        {
                            // Execution.Execution.getBuilder().InputIIP_Event(this.EventId);
                          //  Execution.Execution.getBuilder().GenerateExecutionTable(this.EventId);
                            EventStatus = EventStatus.Ignore;
                        }
                        else if (newStatus == EventStatus.PlanCheck)
                        {

                          //  this.EventStatus = EventStatus.Executing;
                          //  this.executePlan();

                        }
                        else if (newStatus == EventStatus.Alarm)
                            this.EventStatus = EventStatus.Alarm;
                        break;

                }
            }
            catch (Exception ex)
            {
                Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);
                throw new Exception(ex.Message + "," + ex.StackTrace);
            }
        }


      
        private static bool  IsAllowStatus(EventMode mode, EventStatus oldstatus, EventStatus newstatus)
        {
             if (newstatus == EventStatus.Abort || newstatus == EventStatus.Closed  || newstatus==EventStatus.Ignore)
                        return false;
             if (newstatus == EventStatus.Executing)
                 return true;
            switch (mode)
            {
              
                default:
                    if (oldstatus >= newstatus)
                        return false;
                    else
                        return true;
                     
                   // break;
            }
        }


        public void setEventStatus(int status)
        {

            if (status ==(int) EventStatus.Abort)
            {
                this.invokeAbort();
                return;

            }
            if (status == (int)EventStatus.ForceAbort)
            {
                this.InvokeForceAbort();
                return;
            }


            doModeControl((EventStatus)status);


           // EventStatus evtstatus = (EventStatus)status;
            //switch ((EventStatus)status)
            //{
            //    case EventStatus.Alarm:
            //        doModeControl((EventStatus) status);
            //        this.m_status = (EventStatus)status;
            //        break;
            //    case EventStatus.Confirm:
            //      //  Execution.Execution.getBuilder().InputIIP_Event(this.EventId); //   .setAutoEvent(this.EventId);
            //        this.m_status = (EventStatus)status;
            //        break;
            //    case EventStatus.Abort:
            //        this.m_status = (EventStatus)status;
            //        //.............. fill later
            //        break;
            //    case EventStatus.PlanCheck:
            //        //jmp to executing
            //        //Execution.Execution.getBuilder().inputEvent(this.EventId);
            //        //this.m_status = (EventStatus)status;
            //        m_status = EventStatus.Executing;
            //        goto case EventStatus.Executing;
            //       // break;
            //    case EventStatus.Executing:
            //        m_status = EventStatus.Executing;
            //       // Execution.Execution.getBuilder().GenerateExecutionTable(this.EventId);   //產生執行表

            //        //....................fill later
            //        break;
               
            //}

        }
        public int EventClass
        {

            get
            {
                return m_class;
            }
        }
        public int EventId
        {
            get
            {
                return m_eventid;
            }

            set
            {
                if (m_org_eventid == -1)
                    m_org_eventid = value;
                m_eventid = value;
            }
        }


        public int OrgEventId
        {

            get
            {
                return m_org_eventid;
            }
            set
            {
                this.m_org_eventid = value;
            }
        }

        public bool IsReNew
        {
            get
            {
                return m_isRenew;
            }

        }
       // public abstract long setEventId();

       
        public  EventMode EventMode
        {
            get
            {
                return m_eventmode;
            }
        }


        public int Level
        {
            get
            {
                return m_level;
            }
        }


        public void ReNewEvent()
        {
           this.m_eventid= Global.getEventId();

           _status = EventStatus.Null;
           m_isRenew = true;

        //   doModeControl((EventStatus)this.EventStatus);  
           if (this.OnReNewEvent != null)
               this.OnReNewEvent(this, null);
           

        }


        public void invokeAbort()
        {

            if (this.OnAbort != null)
                this.OnAbort(this, null);
          
        }

        public void InvokeForceAbort()
        {
            if (this.OnAbort != null)
                this.OnAbort(this, null);
            if (this.OnAbortToManager != null)
                this.OnAbortToManager(this, null);

        }

        public void invokeStop()
        {
            if (this.OnStop != null)
                this.OnStop(this, null);
        }

        public void invokeRangeChange()
        {
            if (this.OnRangeChange != null)
                this.OnRangeChange(this, null);
        }


        public void invokeDegreeChange()
        {
            if (this.OnDegreeChange != null)
                this.OnDegreeChange(this, null);
        }
      //  protected abstract void loadEventIdAndMode();



       
        
        public abstract string getLineId();

        public abstract string getDir();

       

        public abstract int getStartMileM();

        public abstract int getEndMileM();

        public abstract int getDegree();

        public abstract string getDeviceName();

        public virtual string getSectionId()
        {
            return Program.matrix.line_mgr[this.getLineId()].getSectionByMile(this.getDir().Trim()[0].ToString(), this.getStartMileM()).sectionId;
        }
     //   public abstract int getDeviceName();
       
       

    }
}
