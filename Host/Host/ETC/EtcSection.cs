using System;
using System.Collections.Generic;
using System.Text;

namespace Host.ETC
{
    public  class EtcSection
    {
        public string lineid, direction, startDivision, endDivision;
        public int startMile, endMile,flowid;

       // 31小客，32小貨，41大客，42大貨，5連結
         public System.DateTime timestamp=new DateTime();
        public int small_car_time=-1, small_vehicle_time=-1, bus_time=-1, big_car_time=-1, link_car_time=-1;
        public int small_car_vol = -1, small_vehicle_vol = -1, bus_vol = -1, big_car_vol = -1, link_car_vol = -1;
        public int traveltime = -1;
       
           
        public EtcSection(string lineid,string direction, string startDivision,string endDivision,int startMile,int endMile, int flowid)
        {
             this.lineid = lineid;
             this.direction = direction;
             this.startDivision = startDivision;
             this.endDivision = endDivision;
             this.startMile = startMile;
             this.endMile = endMile;
             this.flowid = flowid;
        }


        public string Key
        {

            get
            {
                return direction+"-"+startDivision + "-" + endDivision;
            }
        }

        public void setTravelTime(DateTime dt,int time)
        {
            if (((TimeSpan)(dt - timestamp)).TotalMinutes > 30)
            {
                traveltime = -1;
            }

            if (dt > timestamp)
            {
                traveltime = time;
                timestamp = dt;
                return;       
            }

            if (dt == timestamp && time < traveltime)
                traveltime = time;




            return;

        }

        public int Speed
        {
            get
            {
                if (traveltime == -1)
                    return -1;

                if (((TimeSpan)(System.DateTime.Now - timestamp)).TotalMinutes > 40)
                {
                    return -1;
                }
                return System.Math.Abs(endMile - startMile) / traveltime * 60 / 1000;
            }
            
           
        }

     


    }
}
