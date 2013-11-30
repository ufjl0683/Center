using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.Redirect
{
    class RouteRedirectRange:Event
    {

        public string devName;
        RedirectSettingGroup group;
        public   RouteRedirectRange(RedirectSettingGroup  group ):base()
        {
            this.group = group;
            this.devName = group.devName;
            this.m_alarm_type = AlarmType.TRAFFIC;
            this.m_class = 48;
          
            try
            {
                this.m_eventmode = Global.getEventMode(m_class);
                this.EventId = Global.getEventId();
            }
            catch(Exception ex)
            {
                this.m_eventmode = EventMode.DontCare;
                RemoteInterface.Util.SysLog("evterr.txt", ex.Message + "," + ex.StackTrace);
            }
        }

        public override string getLineId()
        {
            return Program.matrix.device_mgr[this.devName].lineid;
            //throw new Exception("The method or operation is not implemented.");
        }

        public override string getDir()
        {

            return Program.matrix.device_mgr[this.devName].direction;
        }

        public override int getStartMileM()
        {
            // throw new Exception("The method or operation is not implemented.");
            return Program.matrix.device_mgr[this.devName].start_mileage;
        }

        public override int getEndMileM()
        {
            // throw new Exception("The method or operation is not implemented.");
            return Program.matrix.device_mgr[this.devName].end_mileage;
        }

        public override int getDegree()
        {
            return group.Degree;
        }

        public override string ToString()
        {
            //return base.ToString();

            return "RouteRedirectEvent," + this.devName + ", degree:" + this.getDegree();
        }

    }
}
