using System;
using System.Collections.Generic;
using System.Text;
using Host.Event;
using Host.TC;

namespace Host.Event.Weather
{
    public  class WeatherRange:Range
    {

     public     WeatherRange(DeviceBaseWrapper dev,AlarmType alarm_type)  : base(dev)
        {
            this.m_alarm_type = alarm_type;
            switch (m_alarm_type)
            {
                case AlarmType.RD:
                    this.m_class = 47; 
                    break;
                case AlarmType.VI:
                    this.m_class = 45; 
                    break;
                case AlarmType.WD:
                    this.m_class = 46; 
                    break;
                case AlarmType.LS:
                    this.m_class = 50;
                    break;

                case AlarmType.BS:
                    this.m_class = 51;
                
                    break;
            }

           // this.m_eventmode = Global.getEventMode(this.m_class);
            this.m_eventmode = Global.getEventModeBySectionID(this.getSectionId(), this.m_class, ref this.IsLock, ref this.description);

            try
            {
                this.EventId = Global.getEventId();
            }
            catch
            {
                this.m_eventmode = EventMode.DontCare;
            }
           
        }

        //protected override void loadEventIdAndMode()
        //{

          
        //    try
        //    {
        //        this.EventId = Global.getEventId();
        //    }
        //    catch
        //    {
        //        this.m_eventmode = EventMode.DontCare;
        //    }
        //  //  EventStatus = EventStatus.Alarm;
           
           
           
          
            
        //    //throw new Exception("The method or operation is not implemented.");
        //}

     public override string getDeviceName()
     {
         //throw new NotImplementedException();
         return (devlist[0] as DeviceBaseWrapper).deviceName;
     }

        public override string getDir()
        {
            return Direction;
            //throw new Exception("The method or operation is not implemented.");
        }

        public override int getStartMileM()
        {
            //throw new Exception("The method or operation is not implemented.");
            if (this.m_alarm_type == AlarmType.BS)
                return (devlist[0] as DeviceBaseWrapper).mile_m;
            else
                return ((DeviceBaseWrapper)devlist[0]).start_mileage;
        }

        public override int getEndMileM()
        {

            if (this.m_alarm_type == AlarmType.BS)
                return (devlist[0] as DeviceBaseWrapper).mile_m;

            return ((DeviceBaseWrapper)devlist[0]).end_mileage;
            //throw new Exception("The method or operation is not implemented.");
        }

        public override string getLineId()
        {
            return LineId;
            //throw new Exception("The method or operation is not implemented.");
        }

        public override int getDegree()
        {
            
                switch (this.m_alarm_type)
                {
                    case AlarmType.RD:
                        return ((DeviceBaseWrapper)devlist[0]).event_degree;
                        
                    case AlarmType.VI:
                        return ((DeviceBaseWrapper)devlist[0]).event_degree;
                        
                    case AlarmType.WD:
                        return ((DeviceBaseWrapper)devlist[0]).event_degree;
                    case AlarmType.LS:
                        return ((DeviceBaseWrapper)devlist[0]).event_degree;

                    case AlarmType.BS:
                        return ((DeviceBaseWrapper)devlist[0]).event_degree;
                        
                    default:
                        throw new Exception("Unknown Weather Event Source!");

                }
            
        }

        public string DeviceName
        {
            get
            {
                return ((Host.TC.DeviceBaseWrapper)devlist[0]).deviceName;
            }
        }

        public override string ToString()
        {
            //return base.ToString();

            return "EventID:"+this.EventId+","+"EventMode:"+this.EventMode+","+ ((DeviceBaseWrapper)this.devlist[0]).deviceName + "," + this.getLineId() + "," + this.getDir() + "," + this.getStartMileM() + "~" + this.getEndMileM() + "," + this.getDegree() + "," + getEventStatus(); 

        }
        public override string ToEventString()
        {
           //throw new NotImplementedException();
            return this.ToString().Replace(',', '_');
        }

    }
}
