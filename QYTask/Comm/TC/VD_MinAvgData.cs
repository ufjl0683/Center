using System;
using System.Collections.Generic;
using System.Text;

namespace Comm.TC
{
    public class VD_MinAvgData
    {

        public    int vol=0;
       // private int m_vol_cnt;
        public  int speed=0;
        public int  occupancy = 0;
        public int interval = 0;
       public    int year, month, day,hour,min;
        public string vdName;
        private VDTC vdtc;
        //public VD_MinAvgData()
        //{
        //    m_vol = m_speed = m_occupancy = -1;
        //    m_vol_cnt = 0;
        //}
      
        public VD_MinAvgData(VDTC vdtc)
        {
            this.vdName = vdtc.DeviceName;
            this.vdtc = vdtc;
        }

        public DateTime dateTime
        {
            get
            {
                return new System.DateTime(year, month, day, hour, min, 0);
            }
        }
        public bool IsValid
        {
            get
            {
                if (vol == -1 || speed == -1 || occupancy == -1)
                    return false;
                else if (vol == 0 && (speed != 0 || occupancy != 0))
                    return false;
                else if (speed > VDTC.conflict_speed && occupancy > VDTC.conflict_occupancy)
                    return false;
                else
                    return true;
                   
            }
        }

        public override string ToString()
        {
            //return base.ToString();
            return this.vdName+","+this.dateTime+string.Format(",vol:{0},speed:{1},occpancy:{2}", vol, speed, occupancy);
        }
    }
}
