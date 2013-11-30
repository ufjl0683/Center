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
        AutoAuto=0,     //IIP Auto , Exeuting Auto
        HalfAuto = 1,     //IIP halfAuto , Exeuting Auto
        Manual=3,       
        DontCare=2,
        AutoHalf=4,       // IIP Auto, Executing HalfAuto
        HalfHalf=5        //IIP Half Auto  , Executing Half Auto
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

        public EventStatus getEventStatus()
        {
            return EventStatus;
        }

        private EventStatus EventStatus
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
            OdbcCommand cmd = new OdbcCommand("select devicename,device_type,priority,outputdata1,outputdata2 from tblrspExecutionOutputData where eventid=" + this.EventId);
            OdbcDataReader rd;
            cmd.Connection = cn;
            
            try
            {
                cn.Open();
                rd = cmd.ExecuteReader();
                clearLstOutputQueueData();
                while (rd.Read())
                {
                    RemoteInterface.HC.OutputQueueData qdata;
                    int priority= System.Convert.ToInt32(rd[2]);
                    string devName=rd[0].ToString();
                    object OutObj = null;
                   
                    if (!rd.IsDBNull(4))
                    {
                       OutObj=RemoteInterface.Util.getObjectByHexString(rd[4].ToString());
                    }
                    else if (!rd.IsDBNull(3))
                    {
                        OutObj = RemoteInterface.Util.getObjectByHexString(rd[3].ToString());
                    }
                    else
                        return;


                    qdata = new RemoteInterface.HC.OutputQueueData(devName,RemoteInterface.HC.OutputModeEnum.ResponsePlanMode, this.EventId, priority,OutObj );

                    (Program.matrix.getDeviceWrapper(devName) as Host.TC.OutPutDeviceBase).SetOutput(qdata);
                    this.lstOutputQueueData.Add(qdata);
                }
                rd.Close();
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

        public void clearLstOutputQueueData()
        {
            foreach (OutputQueueData data in lstOutputQueueData)
            {
                try
                {
                    (Program.matrix.getDevicemanager()[data.DeviceName] as Host.TC.OutPutDeviceBase).removeOutput(data.ruleid);
                }
                catch(Exception ex)
                {
                    RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                }
            }
            lstOutputQueueData.Clear();
        }

        private void doModeControl(EventStatus newStatus)
        {
            try
            {

                if (this.EventMode == EventMode.DontCare)
                    return;
                if (!IsAllowStatus(this.EventMode, EventStatus, newStatus))
                    return;
                switch (this.EventMode)
                {
                    case EventMode.AutoAuto:

                        Execution.Execution.getBuilder().InputIIP_Event(this.EventId);
                        //Execution.Execution.getBuilder().
                        Execution.Execution.getBuilder().GenerateExecutionTable(this.EventId);
                        this.EventStatus = EventStatus.Executing;
                        this.executePlan();

                        break;
                    case EventMode.DontCare:
                        this.EventStatus = EventStatus.Alarm;
                        break;
                    case EventMode.HalfAuto:


                        if (newStatus == EventStatus.Confirm)
                        {
                            Execution.Execution.getBuilder().InputIIP_Event(this.EventId);
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
                            Execution.Execution.getBuilder().InputIIP_Event(this.EventId);
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
             if (newstatus == EventStatus.Abort || newstatus == EventStatus.Closed)
                        return false;

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

     //   public abstract int getDeviceName();
       
       

    }
}
