using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.TEM
{
    [Serializable]
  public   class AirAlarmData
    {
      public string devName;
      public int tunnel, place, mileage_m;
      public string type;

      public double density;
      public int level;
      int odd;
        public event OnEventHandler OnEvent;
        public event OnEventHandler OnEventStop;

        public AirAlarmData(string devName, int tunnel, int place, int mile_k, int mile_m, string type, int  density ,int odd, int level)
        {
            this.devName = devName;
            this.tunnel = tunnel;
            this.place = place;
            this.odd = odd;
            this.mileage_m = mile_k * 1000 + mile_m;
            this.density = (double)density/(Math.Pow(1,odd));
          //  this.level = level;
            this.type = type;
            //if (level != 0 && this.OnEvent != null)
            //    this.OnEvent(this);


        }
          
      
        public bool  setData( double  density , int level)
        {
            this.density = density;

            if (this.level != level)
            {

                this.level = level;
                AirAlarmData data = new AirAlarmData(this.devName, tunnel, place, mileage_m / 1000, mileage_m % 1000, type, (int)(density * Math.Pow(1, odd)), odd, level);
                data.level = level;
                if (level == 0 && this.OnEventStop != null)
                    this.OnEventStop(data);

                if (level != 0 && this.OnEvent != null)
                    this.OnEvent(data);

                return true;

            }
            else

                return false;

                 
        }

        public string Key
        {
            get{
                return  "A+"+ tunnel.ToString()+"-"+place+"-"+mileage_m+"-"+type;
                }
        }

        public override string ToString()
        {
            //return base.ToString();

            return string.Format("隧道空氣品質 tunnel:{0} place:{1} type:{2} density:{3} level:{4}", tunnel, place, type, density, level);

        }
    }
}
