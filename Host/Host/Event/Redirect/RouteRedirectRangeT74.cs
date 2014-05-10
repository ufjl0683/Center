using Host.Event.Redirect74;
using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.RedirectT74
{
    class RouteRedirectRangeT74:Event
    {

        public string devName;
        RedirectSettingGroup group;
        public   RouteRedirectRangeT74(RedirectSettingGroup  group ):base()
        {
            this.group = group;
            this.devName = group.devName;
            this.m_alarm_type = AlarmType.TRAFFIC;
            this.m_class = 173;
          
            try
            {
              //  this.m_eventmode = Global.getEventMode(m_class);
                this.m_eventmode = Global.getEventModeBySectionID(this.getSectionId(), this.m_class, ref this.IsLock, ref this.description);
                this.EventId = Global.getEventId();
            }
            catch(Exception ex)
            {
                this.m_eventmode = EventMode.DontCare;
                RemoteInterface.Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);
            }
        }

        public override string ToEventString()
        {
            //throw new NotImplementedException();
            return this.ToString().Replace(',', '_');
        }

        public override string getDeviceName()
        {
          return   this.devName;
            //throw new NotImplementedException();
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
            return Program.matrix.device_mgr[this.devName].mile_m;

        }

        public override int getEndMileM()
        {
            // throw new Exception("The method or operation is not implemented.");
            return Program.matrix.device_mgr[this.devName].mile_m;
        }

        public override int getDegree()
        {
            return group.Degree;
        }

        public override string ToString()
        {
            //return base.ToString();

            return "RouteRedirectEvent," + this.devName + ", degree:" + this.getDegree()+","+this.EventMode+","+this.EventId;
        }

    }
}
