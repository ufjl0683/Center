using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.MovingConstruct
{
         public  class MovingConstructManager
        {

             System.Collections.Hashtable hsMovingEvent = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
             public MovingConstructManager()
             {

                 System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
              //  System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select id, notifier, timeStamp, lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, description,Execution,originalEventid from TBLIIPMCNSLOG");
               System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select id, notifier, t1.timeStamp, t1.lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, description,t1.Execution,t1.originalEventid,t2.eventid,t2.status"+
               " from TBLIIPMCNSLOG t1 inner join tblsysAlarmlog t2 on t2.originaleventid=t1.originaleventid  where t2.ifclose=0");
                 System.Data.Odbc.OdbcDataReader rd;
                 cmd.Connection = cn;
                 int id,originalEventid,evtid, status;
                 string notifier;
                 DateTime timeStamp;
                 string lineID, directionID;
                 int startMileage,  endMileage,  blockTypeId; 
                 string blocklane,  description,IsExecution;
                 try
                 {
                     cn.Open();
                     rd = cmd.ExecuteReader();
                     while (rd.Read())
                     {

                         id = System.Convert.ToInt32( rd[0]);
                         notifier= rd[1].ToString();
                         timeStamp=System.Convert.ToDateTime(rd[2]);
                         lineID=rd[3].ToString();
                         directionID=rd[4].ToString();
                         startMileage=System.Convert.ToInt32(rd[5]);
                         endMileage= System.Convert.ToInt32(rd[6]);
                         blockTypeId=System.Convert.ToInt32(rd[7]);
                         blocklane=rd[8].ToString();
                         description=rd[9].ToString();
                         IsExecution = rd[10].ToString();
                         originalEventid = System.Convert.ToInt32(rd[11]);
                         evtid = System.Convert.ToInt32(rd[12]);
                         status=System.Convert.ToInt32(rd[13]);
                         this.setEvent(id, notifier, timeStamp, lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, description, IsExecution, originalEventid,evtid,status);
                     }
                 }
                 catch(Exception ex)
                 {
                     RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                 }
                 finally
                 {
                     cn.Close();
                 }

             }
             public int GetEventCnt()
             {


                 return this.hsMovingEvent.Count;
             }

            public void setEvent(int id, string notifier, DateTime timeStamp, string lineID, string directionID, int startMileage, int endMileage, int blockTypeId, string blocklane, string description,string IsExecute)
             {

                 if (!hsMovingEvent.Contains(id))
                 {
                     RemoteInterface.Util.SysLog("construct.log", id + " begin!");
                     MovingConstructRange evt = new MovingConstructRange(id, notifier, timeStamp, lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, description, IsExecute);
                     this.hsMovingEvent.Add(id,evt);
                     Program.matrix.event_mgr.AddEvent(evt);
                     string sql="insert into TBLIIPMCNSLOG (id, notifier, timeStamp, lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, description,originaleventid,execution) values({0},'{1}','{2}','{3}','{4}',{5},{6},{7},'{8}','{9}',{10},'{11}')";
                     Program.matrix.dbServer.SendSqlCmd(
                         string.Format(sql,id,notifier,RemoteInterface.DbCmdServer.getTimeStampString(timeStamp),lineID,directionID,startMileage,endMileage,blockTypeId,blocklane,description,evt.OrgEventId,IsExecute));
                 }
                 else
                 {

                     MovingConstructRange evt = hsMovingEvent[id] as MovingConstructRange;
                     evt.setDir(directionID);
                     RemoteInterface.Util.SysLog("construct.log", id + " mile: " + startMileage +"~"+ endMileage);
                     evt.setStartMileage(startMileage);
                     evt.setEndMileage(endMileage);
                     
                     evt.invokeRangeChange();
                     string sql = "update TBLIIPMCNSLOG set startMileage={0},endMileage={1},directionID='{2}'  where id={3}";
                     Program.matrix.dbServer.SendSqlCmd(string.Format(sql, startMileage, endMileage, directionID,id));

                    // ((MovingConstructRange)).invokeRangeChange();
                     
                 }

             }


             // for reload 
            public void setEvent(int id, string notifier, DateTime timeStamp, string lineID, string directionID, int startMileage, int endMileage, int blockTypeId, string blocklane, string description, string IsExecute, int originalEvtid,int evtid,int status)
            {

                if (!hsMovingEvent.Contains(id))
                {
                    RemoteInterface.Util.SysLog("construct.log", id + " begin!");
                    MovingConstructRange evt = new MovingConstructRange(id, notifier, timeStamp, lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, description,IsExecute,originalEvtid,(EventStatus) status);
                    this.hsMovingEvent.Add(id, evt);
                    evt.EventId = evtid;
                    Program.matrix.event_mgr.AddEvent(evt);
                    string sql = "insert into TBLIIPMCNSLOG (id, notifier, timeStamp, lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, description,originaleventid,execution) values({0},'{1}','{2}','{3}','{4}',{5},{6},{7},'{8}','{9}',{10},'{11}')";
                    Program.matrix.dbServer.SendSqlCmd(
                        string.Format(sql, id, notifier, RemoteInterface.DbCmdServer.getTimeStampString(timeStamp), lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, description,originalEvtid,IsExecute));
                }
                else
                {

                    MovingConstructRange evt = hsMovingEvent[id] as MovingConstructRange;
                    RemoteInterface.Util.SysLog("construct.log", id + " mile: " + startMileage + "~" + endMileage);
                    evt.setStartMileage(startMileage);
                    evt.setEndMileage(endMileage);
                    evt.invokeRangeChange();
                    string sql = "update TBLIIPMCNSLOG set startMileage={0},endMileage={1},directionID='{2}' where id={3}";
                    Program.matrix.dbServer.SendSqlCmd(string.Format(sql, startMileage, endMileage, directionID, id));

                    // ((MovingConstructRange)).invokeRangeChange();

                }

            }



             public void CloseMovingConstructEvent(int id)
             {

                 RemoteInterface.Util.SysLog("construct.log", id + " close!");
                 try
                 {
                     if (!this.hsMovingEvent.Contains(id))
                         return;
                     MovingConstructRange evt = hsMovingEvent[id] as MovingConstructRange;
                     evt.invokeStop();
                     //  Program.matrix.event_mgr.
                     string sql = "delete from TBLIIPMCNSLOG where id=" + id ;
                     Program.matrix.dbServer.SendSqlCmd(sql);
                     hsMovingEvent.Remove(id);
                 }
                 catch (Exception ex)
                 {
                     RemoteInterface.Util.SysLog("evterr.log",ex.Message + "," + ex.StackTrace);
                 }

             }


            


        }
}
