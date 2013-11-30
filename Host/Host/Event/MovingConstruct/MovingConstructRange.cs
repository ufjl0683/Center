using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.MovingConstruct
{
     public  class MovingConstructRange:Event
    {
        public   int id, blockTypeId;
        public new string description;
         public string notifier, blocklane;
         string lineID, directionID;
         System.DateTime  timeStamp;
         int startMileage,  endMileage;
         public string IsExecution;
       
         
         public MovingConstructRange(int id, string notifier, DateTime timeStamp, string lineID, string directionID, int startMileage, int endMileage, int blockTypeId, string blocklane, string description,string IsExecution)
         {
             this.id = id;
             this.notifier = notifier;
             this.timeStamp = timeStamp;
             this.lineID = lineID;
             this.directionID = directionID;
             this.startMileage = startMileage;
             this.endMileage=endMileage;
             this.blocklane=blocklane;
             this.blockTypeId = blockTypeId;
             this.description = description;
             this.EventId = Global.getEventId();
             this.m_class = 31;
             this.m_alarm_type = AlarmType.GEN;
             this.IsExecution = IsExecution;
             try
             {
               //  this.m_eventmode = Global.getEventMode(this.m_class);
                 this.m_eventmode = Global.getEventModeBySectionID(this.getSectionId(), this.m_class, ref this.IsLock, ref this.description);
             }
             catch
             {
                 this.m_eventmode = EventMode.DontCare;
             }
             
         }


         // for reload

         public MovingConstructRange(int id, string notifier, DateTime timeStamp, string lineID, string directionID, int startMileage, int endMileage, int blockTypeId, string blocklane, string description, string IsExecute, int originalEvtid, EventStatus status)
             :this(id,notifier,timeStamp,lineID,directionID,startMileage,endMileage,blockTypeId,blocklane,description,IsExecute)
         {

            
             this.OrgEventId = originalEvtid;
             this.EventStatus = status;
             this.IsReload = true;

         }

         public override string ToEventString()
         {
             //throw new NotImplementedException();
             return this.ToString().Replace(',', '_');
         }
         public override string getDeviceName()
         {
             return null;
             //throw new NotImplementedException();
         }
         public void setStartMileage(int mile_m)
         {
             this.startMileage = mile_m;
         }
         public void setEndMileage(int mile_m)
         {
             this.endMileage = mile_m;
         }
         //protected override void loadEventIdAndMode()
         //{
         //   // throw new Exception("The method or operation is not implemented.");
         //    //this.EventId = Global.getEventId();
         //    //this.m_class = 31;
         //    //this.m_alarm_type = AlarmType.GEN;
         //    //try
         //    //{
         //    //    this.m_eventmode = Global.getEventMode(this.m_class);
         //    //}
         //    //catch
         //    //{
         //    //    this.m_eventmode = EventMode.DontCare;
         //    //}
         //}
         public override int getDegree()
         {
             return -1;
             //throw new Exception("The method or operation is not implemented.");
         }
         public override string getDir()
         {
             //throw new Exception("The method or operation is not implemented.");
             return directionID;
         }

         public void setDir(string dir)
         {

             this.directionID = dir;
         }
            

         public override bool Equals(object obj)
         {
             //return base.Equals(obj);
             return this.id == (obj as MovingConstructRange).id;
         }


         public override int getStartMileM()
         {
             //throw new Exception("The method or operation is not implemented.");
             return this.startMileage;

         }

         public override int getEndMileM()
         {
             //throw new Exception("The method or operation is not implemented.");
             return this.endMileage;
         }

         public override string getLineId()
         {
             //throw new Exception("The method or operation is not implemented.");
             return this.lineID;
         }

         public override string ToString()
         {
             return "Moving Construct EventID:" + this.EventId + "," + "EventMode:" + this.EventMode + "," + "evtid:" + this.EventId + "," + "id:" + id + "," + "lineid:" + lineID + "," + "dir:" + directionID + ", startmileage:" + startMileage + ",endmile:" + endMileage +","+ getEventStatus(); ;
         }

    }
}
