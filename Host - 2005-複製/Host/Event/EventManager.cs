using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using Host.Event;

namespace Host.Event
{
   public  class EventManager
    {

       System.Collections.Hashtable hsEvent = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
       public EventManager()
       {

#if !DEBUG
           string sql = "update tblSysAlarmlog set status=-1,ifclose=1 where ifclose=0";
           Program.matrix.dbServer.SendSqlCmd(sql);
#endif

           ConsoleServer.WriteLine("事件管理啟動完成!");
       }

       public void setEventStatus(int evtid, int status)
       {

           if (!hsEvent.Contains(evtid))
               throw new Exception(evtid + " not found!");

           ((Event)hsEvent[evtid]).setEventStatus(status);
          
       }
       void eventobj_OnDegreeChange(object sender, EventArgs e)
       {
           //throw new Exception("The method or operation is not implemented.");
           try
           {
               Event evt = sender as Event;
             //  Util.Log(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "sys.log"), DateTime.Now + "," + evt + ",degree change!\r\n");

               string sql = "update tblSysAlarmLog set status=-1,ifclose=1,memo='Degree Changed',endtime='{1}' where eventid={0}";
               Program.matrix.dbServer.SendSqlCmd(string.Format(sql, evt.EventId, RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now)));

           
               
               //this.RemoveEvent(evt);
           
               //try
               //{
               //    evt.setEventStatus((int)EventStatus.Abort);
               //}
               //catch(Exception ex) {
               //    Util.SysLog("evterr.log",ex.Message+","+ex.StackTrace) ;
               
               //}
               //evt.ReNewEvent();
            
               //this.AddEvent(evt);
               try
               {
                   //  this.hsEvent.Remove(evt.EventId);
                   this.RemoveEvent(evt);
               }
               catch (Exception ex)
               {
                   Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);

               }

               try
               {
                   //  evt.invokeAbort();
                   evt.setEventStatus((int)EventStatus.Abort);
               }
               catch (Exception ex)
               {
                   Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);

               }
               try
               {
                   evt.ReNewEvent();
               }
               catch (Exception ex)
               {
                   Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);

               }
               try
               {
                   this.AddEvent(evt);
                   //this.hsEvent.Add(evt.EventId, evt);
               }
               catch (Exception ex)
               {
                   Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);

               }

            
           
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
           }

       }

       void eventobj_OnRangeChange(object sender, EventArgs e)
       {
           try
           {
               Event evt = sender as Event;
           

               string sql = "update tblSysAlarmLog set status=-1,ifclose=1 ,memo='Range changed',endtime='{1}' where eventid={0}";
               Program.matrix.dbServer.SendSqlCmd(string.Format(sql, evt.EventId, RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now)));
            
               try
               {
                 //  this.hsEvent.Remove(evt.EventId);
                   this.RemoveEvent(evt);
               }
               catch (Exception ex)
               {
                   Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);

               }

               try
               {
                 //  evt.invokeAbort();
                   evt.setEventStatus((int)EventStatus.Abort);
               }
               catch (Exception ex)
               {
                   Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);

               }
               try
               {
                   evt.ReNewEvent();
               }
               catch (Exception ex)
               {
                   Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);

               }
               try
               {
                   this.AddEvent(evt);
                   //this.hsEvent.Add(evt.EventId, evt);
               }
               catch (Exception ex)
               {
                   Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);

               }
              // InsertAlarmTable(evt);
               //try
               //{
               //    evt.setEventStatus((int)EventStatus.Alarm);
               //}
               //catch (Exception ex)
               //{
               //    ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
               //}

           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
           }
        
       }
       void evt_OnReNewEvent(object sender, EventArgs e)
       {

         
           //throw new Exception("The method or operation is not implemented.");
       }

       void eventobj_OnStop(object sender, EventArgs e)
       {
         //  throw new Exception("The method or operation is not implemented.");
           try
           {
           Event evt = sender as Event;
         //  Util.Log(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "sys.log"), DateTime.Now + "," + evt + ",stop!\r\n");
           try
           {
               RemoveEvent(evt);
           }
           catch (Exception ex)
           {
               Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);

           }

           try
           {
               evt.setEventStatus((int)EventStatus.Closed);
           }
           catch (Exception ex)
           {
               Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);

           }
           string sql = "update tblSysAlarmLog set status=9,ifclose=1,memo='close',endtime='{1}'  where eventid={0}";
           Program.matrix.dbServer.SendSqlCmd(string.Format(sql,evt.EventId,RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now)));
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
           }
       }

       void evt_OnAbort(object sender, EventArgs e)
       {
       
           try
           {
               Event evt = sender as Event;
                 
              
             //  Util.Log(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "sys.log"), DateTime.Now + "," + evt + ",abort!\r\n");
               try
               {
                   RemoveEvent(evt);
               }
               catch (Exception ex)
               {
                   Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);

               }
               try
               {
                   evt.setEventStatus((int)EventStatus.Abort);
               }
               catch (Exception ex)
               {
                   Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);

               }
               string sql = "update tblSysAlarmLog set status=-1,ifclose=1,endtime='{1}' where eventid={0}";
               Program.matrix.dbServer.SendSqlCmd(string.Format(sql, evt.EventId, RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now)));

           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
              
           }
       }

       public void AddEvent(Event evt)
       {
       
           try
           {
               hsEvent.Add(evt.EventId, evt);
               evt.OnStop += new EventHandler(eventobj_OnStop);
               evt.OnRangeChange += new EventHandler(eventobj_OnRangeChange);
               evt.OnDegreeChange += new EventHandler(eventobj_OnDegreeChange);
               evt.OnReNewEvent += new EventHandler(evt_OnReNewEvent);
               evt.OnAbort += new EventHandler(evt_OnAbort);
               InsertAlarmTable(evt);
               evt.setEventStatus((int)EventStatus.Alarm);
               Program.notifyServer.NotifyAll(new NotifyEventObject( EventEnumType.NEW_RSP_EVENT,"HOST",evt.EventId));
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
           }
       }

   


        void RemoveEvent(Event evt)
       {
           //try
           //{
               
                
            if(hsEvent.Contains(evt.EventId))
            {
                evt.clearLstOutputQueueData();

               hsEvent.Remove(evt.EventId);
               evt.OnStop -= new EventHandler(eventobj_OnStop);
               evt.OnRangeChange -= new EventHandler(eventobj_OnRangeChange);
               evt.OnDegreeChange -= new EventHandler(eventobj_OnDegreeChange);
               evt.OnReNewEvent -= new EventHandler(evt_OnReNewEvent);
               evt.OnAbort -= new EventHandler(evt_OnAbort);
            }
             
           //}
           //catch (Exception ex)
           //{
           //    ConsoleServer.WriteLine(ex.Message);
           //}
          // InsertAlarm(evt);
       }
       
      

       void InsertAlarmTable(Event evt)
       {
           string sql;
        //   if(! evt.IsReNew)
           if (evt is Host.Event.Jam.JamRange )
           {

               sql = "insert into tblSysAlarmLog (EVENTID,ALARMCLASS,TIMESTAMP,LINEID,DIRECTION,START_MILEAGE,END_MILEAGE,DEGREE,OriginalEventID,DeviceName) values({0},{1},'{2}','{3}','{4}',{5},{6},{7},{8},'{9}')";
               sql = string.Format(sql, evt.EventId, evt.EventClass, RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now), evt.getLineId(),
                   evt.getDir(), evt.getStartMileM(), evt.getEndMileM(), evt.getDegree(), evt.OrgEventId,(evt  as  Jam.JamRange).DeviceName);
           }
           else if(evt is Host.Event.Jam.RampJamRange)
           {
               sql = "insert into tblSysAlarmLog (EVENTID,ALARMCLASS,TIMESTAMP,LINEID,DIRECTION,START_MILEAGE,END_MILEAGE,DEGREE,OriginalEventID,DivisionID,DeviceName) values({0},{1},'{2}','{3}','{4}',{5},{6},{7},{8},'{9}','{10}')";
               sql =string.Format(sql, evt.EventId, evt.EventClass, RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now), evt.getLineId(),
                   evt.getDir(), evt.getStartMileM(), evt.getEndMileM(), evt.getDegree(), evt.OrgEventId, (evt as Jam.RampJamRange).rampVDData.divisionId, (evt as Jam.RampJamRange).rampVDData.deviceName);
           }

           else if(evt is Host.Event.MovingConstruct.MovingConstructRange)
           {
                 sql = "insert into tblSysAlarmLog (EVENTID,ALARMCLASS,TIMESTAMP,LINEID,DIRECTION,START_MILEAGE,END_MILEAGE,OriginalEventID,mc_id,mc_notifier,mc_blocktypeid,mc_blocklane,mc_memo) values({0},{1},'{2}','{3}','{4}',{5},{6},{7},{8},'{9}',{10},'{11}','{12}')";
               sql =string.Format(sql, evt.EventId, evt.EventClass, RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now), evt.getLineId(),
                   evt.getDir(), evt.getStartMileM(), evt.getEndMileM(),  evt.OrgEventId,
                   (evt as MovingConstruct.MovingConstructRange).id, (evt as MovingConstruct.MovingConstructRange).notifier, (evt as MovingConstruct.MovingConstructRange).blockTypeId,

                    (evt as MovingConstruct.MovingConstructRange).blocklane, (evt as MovingConstruct.MovingConstructRange).description);
           }

           else if(evt is Host.Event.Weather.WeatherRange)

           {
              
            sql = "insert into tblSysAlarmLog (EVENTID,ALARMCLASS,TIMESTAMP,LINEID,DIRECTION,START_MILEAGE,END_MILEAGE,DEGREE,OriginalEventID,DeviceName) values({0},{1},'{2}','{3}','{4}',{5},{6},{7},{8},'{9}')";
            sql=string.Format(sql,evt.EventId, evt.EventClass, RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now), evt.getLineId(),
                   evt.getDir(), evt.getStartMileM(), evt.getEndMileM(), evt.getDegree(), evt.OrgEventId,((Host.Event.Weather.WeatherRange)evt).DeviceName);
              

           } 
           else if (evt is Host.Event.TEM.TemRangeData)
           {
               sql = (evt as Host.Event.TEM.TemRangeData).getSQL_tInsertAlarmData();

           }
           else if(evt is Host.Event.IID.IIDRange)
           {
               Host.Event.IID.IIDRange iidevt = evt as Host.Event.IID.IIDRange;
               sql = "insert into tblSysAlarmLog (EVENTID,ALARMCLASS,TIMESTAMP,LINEID,DIRECTION,START_MILEAGE,END_MILEAGE,DEGREE,OriginalEventID,CAM_ID,LANE_ID) values({0},{1},'{2}','{3}','{4}',{5},{6},{7},{8},{9},{10})";
               sql = string.Format(sql, iidevt.EventId, iidevt.EventClass, RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now), iidevt.getLineId(),
                          iidevt.getDir(), iidevt.getStartMileM(), iidevt.getEndMileM(), iidevt.getDegree(), iidevt.OrgEventId,iidevt.Cam_ID,iidevt.Lane_Id);
           }
           else
           {

               sql = "insert into tblSysAlarmLog (EVENTID,ALARMCLASS,TIMESTAMP,LINEID,DIRECTION,START_MILEAGE,END_MILEAGE,DEGREE,OriginalEventID) values({0},{1},'{2}','{3}','{4}',{5},{6},{7},{8})";
               sql = string.Format(sql, evt.EventId, evt.EventClass, RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now), evt.getLineId(),
                      evt.getDir(), evt.getStartMileM(), evt.getEndMileM(), evt.getDegree(), evt.OrgEventId);


           }
           try
           {
               System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
               System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand(sql);
             
               cmd.Connection = cn;
               try
               {
                   cn.Open();
                   cmd.ExecuteNonQuery();



               }
               catch(Exception ex)
               {
                   Util.SysLog("evtlerr.log",ex.Message+","+ex.StackTrace);
               }

               finally
               {
                   cn.Close();
               }

            //   Program.matrix.dbServer.SendSqlCmd(sql);
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
           }

       }


       public override string ToString()
       {
           string ret="";

           foreach (Event evt in this.hsEvent.Values)
               ret += evt.ToString() + "\r\n";

           return ret;

           //return base.ToString();
       }

    }
}
