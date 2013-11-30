using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.TEM;

namespace Host.Event.TEM
{
    public   class TemRangeData:Event
    {

        object eventData;
        Host.TC.TEMDeviceWrapper dev;
        public TemRangeData(Host.TC.TEMDeviceWrapper dev,object data)
        {
            this.eventData = data;
            this.dev=dev;
            this.m_alarm_type = AlarmType.TUNNEL;

            if (data is RemoteInterface.TEM.AirAlarmData)
                this.m_class = 52;
            else if (data is RemoteInterface.TEM.FireAlarmData)
                this.m_class = 60;
            else if (data is RemoteInterface.TEM.LightAlarmData)
                this.m_class = 53;
            else if (data is RemoteInterface.TEM.PowerAlarmData)
                this.m_class = 54;
            else if (data is RemoteInterface.TEM.SecurityAlarmData)
                this.m_class = 76;
            else if (data is RemoteInterface.TEM.MonitorAlarmData)
                this.m_class = 77;
            else
                throw new Exception("unknown TemEventData!");

            this.m_eventmode = Global.getEventMode(this.m_class);
            try
            {
                this.EventId = Global.getEventId();
            }
            catch
            {
                this.m_eventmode = EventMode.DontCare;
            }

        }


        //protected override void loadEventIdAndMode()
        //{
        // //   throw new Exception("The method or operation is not implemented.");

          
        //    try
        //    {
        //        this.EventId = Global.getEventId();
        //    }
        //    catch
        //    {
        //        this.m_eventmode = EventMode.DontCare;
        //    }
        //   // EventStatus = EventStatus.Alarm;
        //}
        public override int getDegree()
        {
          if(eventData is  AirAlarmData)
          {
              AirAlarmData airdata= eventData as AirAlarmData;
              return airdata.level;
          }
          else if (eventData is SecurityAlarmData)
          {
              SecurityAlarmData secdata = eventData as SecurityAlarmData;

              return secdata.status;
          }
          else if (eventData is  RemoteInterface.TEM.FireAlarmData)
          {
              FireAlarmData firedata = eventData as FireAlarmData;

              return firedata.status;
          }
          else if (eventData is RemoteInterface.TEM.LightAlarmData)
          {
              LightAlarmData lightdata = eventData as LightAlarmData;

              return lightdata.damaged==0?0:1;
          }
          else if (eventData is RemoteInterface.TEM.PowerAlarmData)
          {
              PowerAlarmData powerdata = eventData as PowerAlarmData;

              return powerdata.status;
          }
          else if (eventData is RemoteInterface.TEM.MonitorAlarmData)
          {
              MonitorAlarmData monoitordata = eventData as MonitorAlarmData;

              return monoitordata.status;
          }
          else
              return 0;
        }

        public override string getDir()
        {
          return dev.direction;
        }

        public override int  getStartMileM()
        {
            return this.dev.start_mileage;
 	       // throw new Exception("The method or operation is not implemented.");
        }

        public override int  getEndMileM()
        {
            return this.dev.end_mileage;
 	        //throw new Exception("The method or operation is not implemented.");
        }

        public override string  getLineId()
        {
            return this.dev.lineid;
 	       // throw new Exception("The method or operation is not implemented.");
        }

        public override string ToString()
        {
            //return base.ToString();

            return  "EventID:" + this.EventId + "," + "EventMode:" + this.EventMode + "," + eventData.ToString()+","+getEventStatus();
        }

        public string getSQL_tInsertAlarmData()
        {
            string ret = "";

            if (eventData is AirAlarmData)
            {
                AirAlarmData evtdata = eventData as AirAlarmData;
                int tem_type;
                if (evtdata.type == "CO")
                    tem_type = 1;
                else if (evtdata.type == "VI")
                    tem_type = 2;
                else if (evtdata.type == "NO")
                    tem_type = 3;
                else if (evtdata.type == "NO2")
                    tem_type = 4;
                else
                    tem_type = 5;

                string sql = "insert into tblSysAlarmLog (EVENTID,ALARMCLASS,TIMESTAMP,LINEID,DIRECTION,START_MILEAGE,END_MILEAGE,DEGREE,OriginalEventID,Tunid,placeid,tem_status_level,TEM_TYPE,density) values({0},{1},'{2}','{3}','{4}',{5},{6},{7},{8},'{9}',{10},{11},{12},{13})";

                ret = string.Format(sql, this.EventId, this.m_class, RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now), this.getLineId(), this.getDir(),
                    this.getStartMileM(), this.getEndMileM(), this.getDegree(), this.OrgEventId, evtdata.tunnel, evtdata.place, this.getDegree(), tem_type,evtdata.density);
            }
            else if (eventData is SecurityAlarmData)
            {

               SecurityAlarmData evtdata= eventData as   SecurityAlarmData;
              string   sql = "insert into tblSysAlarmLog (EVENTID,ALARMCLASS,TIMESTAMP,LINEID,DIRECTION,START_MILEAGE,END_MILEAGE,DEGREE,OriginalEventID,Tunid,placeid,cardid,tem_status_level) values({0},{1},'{2}','{3}','{4}',{5},{6},{7},{8},'{9}',{10},{11},{12})";

              ret = string.Format(sql, this.EventId, this.m_class, RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now), this.getLineId(), this.getDir(),
                  this.getStartMileM(),  this.getEndMileM(),this.getDegree(), this.OrgEventId, evtdata.tunnel, evtdata.place, evtdata.cardid,this.getDegree());

            }
            else if (eventData is RemoteInterface.TEM.FireAlarmData)
            {
                FireAlarmData evtdata = eventData as FireAlarmData;
                string sql = "insert into tblSysAlarmLog (EVENTID,ALARMCLASS,TIMESTAMP,LINEID,DIRECTION,START_MILEAGE,END_MILEAGE,DEGREE,OriginalEventID,Tunid,placeid,divid,tem_status_level) values({0},{1},'{2}','{3}','{4}',{5},{6},{7},{8},'{9}',{10},{11},{12})";

                ret = string.Format(sql, this.EventId, this.m_class, RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now), this.getLineId(), this.getDir(),
                    this.getStartMileM(), this.getEndMileM(), this.getDegree(), this.OrgEventId, evtdata.tunnel, evtdata.place, evtdata.div,evtdata.status);

            }
            else if (eventData is RemoteInterface.TEM.LightAlarmData)
            {
                LightAlarmData evtdata = eventData as LightAlarmData;
                string sql = "insert into tblSysAlarmLog (EVENTID,ALARMCLASS,TIMESTAMP,LINEID,DIRECTION,START_MILEAGE,END_MILEAGE,DEGREE,OriginalEventID,Tunid,placeid,divid,required,damaged,tem_status_level) values({0},{1},'{2}','{3}','{4}',{5},{6},{7},{8},'{9}',{10},{11},{12},{13},{14})";

                ret = string.Format(sql, this.EventId, this.m_class, RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now), this.getLineId(), this.getDir(),
                    this.getStartMileM(), this.getEndMileM(), this.getDegree(), this.OrgEventId, evtdata.tunnel, evtdata.place, evtdata.div,evtdata.required,evtdata.damaged,this.getDegree());
            }
            else if (eventData is RemoteInterface.TEM.PowerAlarmData)
            {
                PowerAlarmData evtdata = eventData as PowerAlarmData;
                string sql = "insert into tblSysAlarmLog (EVENTID,ALARMCLASS,TIMESTAMP,LINEID,DIRECTION,START_MILEAGE,END_MILEAGE,DEGREE,OriginalEventID,Tunid,placeid,tem_status_level) values({0},{1},'{2}','{3}','{4}',{5},{6},{7},{8},'{9}',{10},{11})";

                ret = string.Format(sql, this.EventId, this.m_class, RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now), this.getLineId(), this.getDir(),
                    this.getStartMileM(), this.getEndMileM(), this.getDegree(), this.OrgEventId, evtdata.tunnel, evtdata.place,evtdata.status);

            }
            else if (eventData is RemoteInterface.TEM.MonitorAlarmData)
            {
                MonitorAlarmData evtdata = eventData as MonitorAlarmData;
                string sql = "insert into tblSysAlarmLog (EVENTID,ALARMCLASS,TIMESTAMP,LINEID,DIRECTION,START_MILEAGE,END_MILEAGE,DEGREE,OriginalEventID,Tunid,placeid,loc,tem_status_level) values({0},{1},'{2}','{3}','{4}',{5},{6},{7},{8},'{9}',{10},{11},{12})";

                ret = string.Format(sql, this.EventId, this.m_class, RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now), this.getLineId(), this.getDir(),
                    this.getStartMileM(), this.getEndMileM(), this.getDegree(), this.OrgEventId, evtdata.tunnel, evtdata.place, evtdata.location, this.getDegree());


            }
            else
                throw new Exception("unknow TEMEventData Type");

            return ret;
        }

        
    }
}
