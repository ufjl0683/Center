using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace RemoteInterface.TEM
{
    public delegate void OnEventHandler(object sender);
   [Serializable]
   
 public   class FireAlarmData
    {
        public DateTime dt;
     public string tunnel,devName;
       public  int  place, div, status;
        public event OnEventHandler OnEvent;
        public event OnEventHandler OnEventStop;
      
        public FireAlarmData(string devName,DateTime dt, string tunnel, int place, int div, int status)
        {
            this.dt = dt;
            this.tunnel = tunnel;
            this.place = place;
            this.div = div;
        //    this.status = status;
            this.devName = devName;

        }

        public bool setStatus(int status)
        {
            this.dt = DateTime.Now;
            if (this.status != status)
            {

                this.status = status;
                FireAlarmData data = new FireAlarmData(devName, dt, tunnel, place, div, status);
                data.status = status;
                if (status == 0 && this.OnEventStop != null)
                    this.OnEventStop(data);


                if (status != 0 && this.OnEvent != null)
                    this.OnEvent(data);


                return true;

            }
            else
                return false;
        }


        public string Key
        {
            get { return "F-"+tunnel.ToString() + "-" + place + "-" + div; }

        }

        public override string ToString()
        {
            //return base.ToString();

            string ret = devName + "隧道火警 tunnid:{0} place:{1} div:{2} status:{3}";
            return string.Format(ret, tunnel, place, div, status);
        }




    }
}
