using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event
{
    public  class ManualRange:Event
    {

        int startMileage, endMileage;
        string lineid, direction;
        public int newEventId;
        public ManualRange(int eventclass, int evenitid, string lineid, string direction, int startMileage, int endMileage, int level)
        {
            this.m_eventmode = EventMode.Manual;
            this.m_class = eventclass;
            this.EventId = evenitid;
          //  this.OrgEventId = evenitid;
            this.m_level = level;
            this.lineid = lineid;
            this.direction = direction;
            this.startMileage = startMileage;
            this.endMileage = endMileage;
         //   this.EventMode = EventMode.Manual;

        }
        public override string ToEventString()
        {
            //throw new NotImplementedException();
            return this.ToString().Replace(',', '_');
        }
        public override string getDeviceName()
        {
            //throw new NotImplementedException();
            return null;
        }
        //protected override void loadEventIdAndMode()
        //{
        //   // throw new Exception("The method or operation is not implemented.");
        //}

        public void setNewEventId(int neweventId)
        {
            this.newEventId=neweventId;
        }
        public override string getLineId()
        {
            return lineid;
           // throw new Exception("The method or operation is not implemented.");
        }

        public override string getDir()
        {
            //throw new Exception("The method or operation is not implemented.");
            return direction;
        }

        public override int getStartMileM()
        {
            //throw new Exception("The method or operation is not implemented.");
            return startMileage;
        }
        public void setStartMuileM(int mile_m)
        {
            this.startMileage = mile_m;
        }
        public override int getEndMileM()
        {
          //  throw new Exception("The method or operation is not implemented.");
            return endMileage;
        }
        public void setEndMuileM(int mile_m)
        {
            this.endMileage = mile_m;
        }

        public override int getDegree()
        {
            //throw new Exception("The method or operation is not implemented.");
            return m_level;
        }
        public void setDegree(int degree)
        {
            this.m_level = degree;
        }

        
    }
}
