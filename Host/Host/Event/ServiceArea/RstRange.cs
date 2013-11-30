using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.ServiceArea
{
    public   class RstRange:Event
    {
        TC.DeviceBaseWrapper dev;
        public RstRange()
        {

            this.dev = new TC.DeviceBaseWrapper("", "QingShuiServiceArea", "ServiceArea", "192.168.78.6", 1001, "F", "N3", 172400, new byte[] { 0, 0, 0, 0 }, 0, 0, "N");
            this.m_alarm_type = AlarmType.OTHER;
            this.m_class = 154;

            this.EventId = Global.getEventId();
            try
            {
                this.m_eventmode = Global.getEventModeBySectionID(this.getSectionId(), this.m_class, ref this.IsLock, ref this.description);

            }
            catch
            {
                this.m_eventmode = EventMode.DontCare;
            }

        }


        public override string ToEventString()
        {
           // throw new NotImplementedException();
            return this.ToString().Replace(',', '_');
        }

        public override string getLineId()
        {
            return dev.getLineID();
        }

        public override string getDir()
        {
          //  throw new NotImplementedException();
            return dev.getDirection();
        }

        public override int getStartMileM()
        {
           // throw new NotImplementedException();
            return dev.getMileage();
        }

        public override int getEndMileM()
        {
           // throw new NotImplementedException();
             return dev.getMileage();
        }

        public override int getDegree()
        {
            return 1;
        }

        public override string getDeviceName()
        {
            return dev.deviceName;
        }
    }
}
