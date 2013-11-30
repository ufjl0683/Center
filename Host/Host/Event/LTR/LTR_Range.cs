using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.LTR
{
    class LTR_Range:Event
    {
        TC.RMSDeviceWrapper dev;

        public LTR_Range(TC.RMSDeviceWrapper dev)
        {
            this.dev = dev;
            //this.group = group;
         //   this.devName = group.devName;
            this.m_alarm_type = AlarmType.TRAFFIC;
            this.m_class = 142;

            try
            {
            //    this.m_eventmode = Global.getEventMode(m_class);
                this.m_eventmode = Global.getEventModeBySectionID(this.getSectionId(), this.m_class, ref this.IsLock, ref this.description);
                this.EventId = Global.getEventId();

            }
            catch (Exception ex)
            {
                this.m_eventmode = EventMode.DontCare;
                RemoteInterface.Util.SysLog("evterr.txt", ex.Message + "," + ex.StackTrace);
            }

        }


        public override string getLineId()
        {
            return dev.lineid;
           // throw new NotImplementedException();
        }

        public override string getDir()
        {
            //throw new NotImplementedException();
            return this.dev.direction;
        }
        public override string ToEventString()
        {
            //throw new NotImplementedException();
            return this.ToString().Replace(',', '_');
        }

        public override int getStartMileM()
        {
          //  throw new NotImplementedException();
            return this.dev.mile_m;
        }

        public override int getEndMileM()
        {
           // throw new NotImplementedException();
            return this.dev.mile_m;
        }

        public override int getDegree()
        {
           // throw new NotImplementedException();
            return 1;
        }

        public override string getDeviceName()
        {
            //throw new NotImplementedException();
            return this.dev.deviceName;
        }
        public override string ToString()
        {

            return getDeviceName() + "," + "LTR_Start Event!";
            //return base.ToString();
        }
    }
}
