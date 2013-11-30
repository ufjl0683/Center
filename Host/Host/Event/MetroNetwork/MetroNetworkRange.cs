using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.MetroNetwork
{
   public  class MetroNetworkRange:Event
    {
        TC.RGSDeviceWrapper RgsDevice;
        int g_code_id;
        public bool IsRampJam = false;
        public bool IsMainVDJamp = false;
       
       public MetroNetworkRange(TC.RGSDeviceWrapper rgsdev, int g_code_id):base()
       {
           this.RgsDevice = rgsdev;
           this.g_code_id = g_code_id;


           this.m_alarm_type = AlarmType.TRAFFIC;
           this.m_class = 42;

           try
           {
               //  this.m_eventmode = Global.getEventMode(m_class);
               this.m_eventmode = Global.getEventModeBySectionID(this.getSectionId(), this.m_class, ref this.IsLock, ref this.description);
               this.EventId = Global.getEventId();
           }
           catch (Exception ex)
           {
               this.m_eventmode = EventMode.DontCare;
               RemoteInterface.Util.SysLog("evterr.log", ex.Message + "," + ex.StackTrace);
           }
       }

       public override string ToEventString()
       {
           // throw new NotImplementedException();
           return string.Format("MetropNetwork Event,evtid:{0} EventMode:{1} Class:{2} device:{3} g_code_id:{4} ,{5}",
                    this.EventId, this.EventMode, this.EventClass, this.getDeviceName(), g_code_id, this.EventStatus);

       }

       public bool CanStop()
       {
           return !(IsRampJam || IsMainVDJamp);
       }



       public override string getLineId()
       {
           // throw new NotImplementedException();
           return RgsDevice.lineid;
       }

       public override string getDir()
       {
           //  throw new NotImplementedException();
           return RgsDevice.direction;
       }

       public override int getStartMileM()
       {
           // throw new NotImplementedException();
           return RgsDevice.mile_m;
       }

       public override int getEndMileM()
       {
           return RgsDevice.mile_m;
           // throw new NotImplementedException();
       }

       public override int getDegree()
       {
           // throw new NotImplementedException();
           return 1;
       }

       public override string getDeviceName()
       {
           return RgsDevice.deviceName;
           // throw new NotImplementedException();
       }

       public override string ToString()
       {
           return this.ToEventString();
       }
    }
}
