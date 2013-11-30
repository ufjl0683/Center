using System;
using System.Collections.Generic;
using System.Text;
using Host.TC;

namespace Host.Event.CSLSControl
{
    class CSLSControlRange:Range
    {

        public CSLSControlRange(VDDeviceWrapper vd)
            : base(vd)
        {

            this.m_level = vd.cslsControlLevel;

            this.EventId = Global.getEventId();

            this.m_alarm_type = AlarmType.TRAFFIC;
            this.m_class = 136;  //csls 主線速率控制
            try
            {
                // this.m_eventmode = Global.getEventMode(this.m_class);
                this.m_eventmode = Global.getEventModeBySectionID(this.getSectionId(), this.m_class, ref this.IsLock, ref this.description);
            }
            catch
            {
                this.m_eventmode = EventMode.DontCare;
            }
        }

      

        public override string getDeviceName()
        {
            //throw new NotImplementedException();
            return (this.devlist[0] as DeviceBaseWrapper).deviceName;
        }
        public string DeviceName
        {
            get
            {
                return (this.devlist[0] as DeviceBaseWrapper).deviceName;
            }
        }

        public override int StartMile
        {
            get
            {
                return (devlist[0] as VDDeviceWrapper).start_mileage;
            }
        }

        public override int EndMile
        {
            get
            {
                return (devlist[devlist.Count - 1] as VDDeviceWrapper).end_mileage;
            }
        }

        public override int getDegree()
        {
            //throw new Exception("The method or operation is not implemented.");
            return this.m_level;
        }

        public override string getLineId()
        {
            // throw new Exception("The method or operation is not implemented.");
            return LineId;
        }

        public override string getDir()
        {
            //  throw new Exception("The method or operation is not implemented.");
            return Direction;
        }

        public override int getStartMileM()
        {
            //  throw new Exception("The method or operation is not implemented.");
            return StartMile;
        }

        public override int getEndMileM()
        {
            //throw new Exception("The method or operation is not implemented.");
            return this.EndMile;
        }
        public override string ToString()
        {
            if (this.devlist.Count > 0)

                return "EventID:" + this.EventId + ",速率控制," + "EventMode:" + this.EventMode + "," + this.LineId + "," + this.Direction + "," + StartMile + "~" + EndMile + "壅塞," + this.getDegree() + "," + getEventStatus();

            else
                return this.EventId + ",clear";
        }
        public override string ToEventString()
        {
            //throw new NotImplementedException();
            return this.ToString().Replace(',', '_');
        }
    }
}
