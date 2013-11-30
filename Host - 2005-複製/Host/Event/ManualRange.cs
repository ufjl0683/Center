using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event
{
    public  class ManualRange:Event
    {

        int startMileage, endMileage;
        string lineid, direction;
        public ManualRange(int eventclass, int evenitid, string lineid, string direction, int startMileage, int endMileage, int level)
        {
            this.m_eventmode = EventMode.Manual;
            this.m_class = eventclass;
            this.EventId = evenitid;
            this.m_level = level;
            this.lineid = lineid;
            this.direction = direction;
            this.startMileage = startMileage;
            this.endMileage = endMileage;
         //   this.EventMode = EventMode.Manual;

        }


        //protected override void loadEventIdAndMode()
        //{
        //   // throw new Exception("The method or operation is not implemented.");
        //}

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

        public override int getEndMileM()
        {
          //  throw new Exception("The method or operation is not implemented.");
            return endMileage;
        }

        public override int getDegree()
        {
            //throw new Exception("The method or operation is not implemented.");
            return m_level;
        }

        
    }
}
