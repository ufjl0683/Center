using System;
using System.Collections.Generic;
using System.Text;

namespace Host
{
    public   class SectionTravelTimeWeightSegnment:IComparable
    {

        public DateTime m_begtime, m_endtime;
       public  int[,] weights = new int[16, 4];

        public SectionTravelTimeWeightSegnment(DateTime begtime,DateTime endtime )
        {
            this.m_begtime = begtime;
            this.m_endtime = endtime;
        }


        public void AddWeight(int weigthid,int vd_percent,int avi_percent,int etc_percent,int his_percent)
        {
             weights[weigthid,0]=vd_percent;
             weights[weigthid,1]=avi_percent;
             weights[weigthid,2]=etc_percent;
             weights[weigthid,3]=his_percent;
        }

        int IComparable.CompareTo(object obj)
        {
            SectionTravelTimeWeightSegnment toCompare = obj as SectionTravelTimeWeightSegnment;
            return (int)((TimeSpan)(this.begtime-toCompare.begtime)).TotalSeconds;
        }

        public DateTime begtime
        {
            get
            {
                return new System.DateTime(1900, 1, 1, m_begtime.Hour, m_begtime.Minute, 0);
            }
        }
        public DateTime endtime
        {
            get
            {
                return new System.DateTime(1900, 1, 1, m_endtime.Hour, m_endtime.Minute, 0);
            }
        }
    
    }
}
