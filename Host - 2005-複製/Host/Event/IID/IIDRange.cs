using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.IID
{
    public  class IIDRange:Range
    {
        IID_CAM_Data camdata;
       
        public IIDRange(IID_CAM_Data camdata):base(camdata.devWrapper)
        {
            this.camdata = camdata;
            this.m_alarm_type = AlarmType.TUNNEL;
            this.m_class = 69 + camdata.eventid;  //一般道路壅塞

            this.m_eventmode = Global.getEventMode(this.m_class);
            try
            {
                this.EventId = Global.getEventId();
            }
            catch
            {
                this.m_eventmode = EventMode.DontCare;
            }
        }

        public int Cam_ID
        {
            get
            {
                return camdata.camid;
            }
        }

        public int Lane_Id
        {
            get
            {
                return camdata.laneid;
            }
        }

        public override int getDegree()
        {
            return 0;
            //throw new Exception("The method or operation is not implemented.");
        }

        public override string getDir()
        {
            return Direction;

            //throw new Exception("The method or operation is not implemented.");
        }

        public override int StartMile
        {
            get
            {
                return camdata.mileage;
                //return base.StartMile;
            }
        }

        public override int  EndMile
        {
            get
            {
                return camdata.mileage;
            }
           
        }

        public override int getEndMileM()
        {
            return this.EndMile;
            //throw new Exception("The method or operation is not implemented.");
        }

        public override string getLineId()
        {
            return this.LineId;
            //throw new Exception("The method or operation is not implemented.");
        }

        public override int getStartMileM()
        {
            return this.StartMile;
            //throw new Exception("The method or operation is not implemented.");
        }

        //protected override void loadEventIdAndMode()
        //{
           
        //   // this.m_eventmode = EventMode.Manual;
          
        //    try
        //    {
        //        this.EventId = Global.getEventId();
        //    }
        //    catch
        //    {
        //        this.m_eventmode = EventMode.DontCare;
        //    }
        //    //throw new Exception("The method or operation is not implemented.");
        //}


        public override string ToString()
        {

            string ret = "IID Event,m_class={0},iid_eventid={1},action={2},camid={3},laneid={4}";
            return "EventID:" + this.EventId + "," + "EventMode:" + this.EventMode + "," + string.Format(ret, this.m_class, this.camdata.eventid, this.camdata.action, this.Cam_ID, this.Lane_Id)+","+getEventStatus();;
        }
       

     
    }
}
