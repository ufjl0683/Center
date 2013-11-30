using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.APID
{

    public enum AIDTYPE
    {
        AID,
        NEURAL
    }
    public   class APIDRangeData:Host.Event.Event
    {
        AIDTYPE AidType= AIDTYPE.AID;
        TC.VDDeviceWrapper vddev;
        public APIDRangeData(TC.VDDeviceWrapper vd,AIDTYPE aidtype):base()
        {
            this.vddev = vd;
           // this.m_level = vd.jamLevel;
            this.AidType = aidtype;
            this.EventId = Global.getEventId();

            this.m_alarm_type = AlarmType.GEN;
            this.m_class = 129;  //一般道路壅塞
            try
            {
                //this.m_eventmode = Global.getEventMode(this.m_class, out this.IsLock,out this.description);
                Global.getEventModeBySectionID(this.getSectionId(), this.m_class, ref this.IsLock, ref this.description);
            }
            catch
            {
                this.m_eventmode = EventMode.DontCare;
            }
        }


      
        public string Key
        {
            get
            {
                return vddev.deviceName;
            }
        }

        public override string getDeviceName()
        {
            //throw new NotImplementedException();
            return vddev.deviceName;
        }
        public override string getLineId()
        {
           // throw new NotImplementedException();
            return vddev.lineid;
        }

        public override string getDir()
        {
           // throw new NotImplementedException();
            return vddev.direction;
        }

        public override int getStartMileM()
        {
          //  throw new NotImplementedException();
            return vddev.start_mileage;
        }

        public override int getEndMileM()
        {
           // throw new NotImplementedException();
            return vddev.end_mileage;
        }

        public override int getDegree()
        {
            int result;
            if (vddev.AID_Result > 2)
                return 1;
            else
                return 0;
         //   return vddev.AID_Result;
           // throw new NotImplementedException();
        }
        public override string ToEventString()
        {
            //throw new NotImplementedException();
            return this.ToString().Replace(',', '_');
        }
        public override string ToString()
        {
            //return base.ToString();
            return "APID" + "," +"mode:"+AidType+","+ this.getDeviceName() + "," + this.EventMode + this.getStartMileM();
        }
    }
}
