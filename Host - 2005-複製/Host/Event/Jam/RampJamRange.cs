using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.Jam
{
    public  class RampJamRange:Range
    {
        public RampVDData rampVDData;
        public RampJamRange(RampVDData rampvddata)
            : base(rampvddata.vd)
        {
            this.rampVDData = rampvddata;
            this.m_class = 43; //都會區出口砸到壅塞
          
            try
            {
                this.m_eventmode = Global.getEventMode(this.m_class);
                this.m_alarm_type = AlarmType.TRAFFIC;
                this.EventId = Global.getEventId();
            }
            catch
            {
                this.m_eventmode = EventMode.DontCare;
            }
        }

        //protected override void loadEventIdAndMode()
        //{
        //    //throw new Exception("The method or operation is not implemented.");
        //    try
        //    {
        //        this.m_alarm_type = AlarmType.TRAFFIC;
        //        this.EventId = Global.getEventId();
        //    }
        //    catch
        //    {
        //        this.m_eventmode = EventMode.DontCare;
        //    }
           
        //}

        public override string getLineId()
        {
            //throw new Exception("The method or operation is not implemented.");
            return this.rampVDData.lineid;
        }

        public override string getDir()
        {
           // throw new Exception("The method or operation is not implemented.");
            return this.rampVDData.direction;
        }

        public override int getStartMileM()
        {
            return this.rampVDData.mile_m;
           // throw new Exception("The method or operation is not implemented.");
        }

        public override int getEndMileM()
        {
           // throw new Exception("The method or operation is not implemented.");
            return this.rampVDData.mile_m;
        }

        public override int getDegree()
        {
            //throw new Exception("The method or operation is not implemented.");
           return  this.rampVDData.vd.jamLevel;
        }

        public override string ToString()
        {
            return "EventID:" + this.EventId + "," + "EventMode:" + this.EventMode + "," + this.rampVDData.divisionName + "(" + (rampVDData.mile_m / 1000.0) + ")出口" + "壅塞," + getEventStatus();
            //return base.ToString();
        }
    }
}
