using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.MovingConstruct
{
     public  class MovingConstructRange:Event
    {
        public   int id, blockTypeId;
         public string description, notifier, blocklane;
         string lineID, directionID;
         System.DateTime  timeStamp;
         int startMileage,  endMileage;
         
         public MovingConstructRange(int id, string notifier, DateTime timeStamp, string lineID, string directionID, int startMileage, int endMileage, int blockTypeId, string blocklane, string description)
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
             try
             {
                 this.m_eventmode = Global.getEventMode(this.m_class);
             }
             catch
             {
                 this.m_eventmode = EventMode.DontCare;
             }
             
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
             return "EventID:" + this.EventId + "," + "EventMode:" + this.EventMode + "," + "evtid:" + this.EventId + "," + "id:" + id + "," + "lineid:" + lineID + "," + "dir:" + directionID + ", startmileage:" + startMileage + ",endmile:" + endMileage +","+ getEventStatus(); ;
         }

    }
}
