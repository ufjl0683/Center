using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
namespace RemoteInterface.TEM
{

    [Serializable]
  public   class MonitorAlarmData
    {
        public DateTime dt;
        public string tunnel,devName;
         public int place,  status,location;
        public event OnEventHandler OnEvent;
        public event OnEventHandler OnEventStop;
      public MonitorAlarmData(string devName, DateTime dt, string tunnel, int place, int location, int status)
      {

          this.dt = dt;
          this.tunnel = tunnel;
          this.place = place;
         // this.div = div;
         // this.status = status;
          this.location = location;
          this.devName = devName;
      }


      public bool setStatus(int status)
      {

          this.dt=DateTime.Now;
          if (this.status != status)
          {

              this.status = status;
              MonitorAlarmData data = new MonitorAlarmData(devName, dt, tunnel, place, location, status);
              data.status = status;
              if (status == 0 && this.OnEventStop != null)
              {

                  this.OnEventStop(data);
              }

              if (status != 0 && this.OnEvent != null)
                  this.OnEvent(data);

              return true;
          }
          else

              return false;
      }

  

      public string Key
      {
          get
          {
              return  "M-"+tunnel + "-" + place+"-"+location;
          }
      }

      public override string ToString()
      {
          //return base.ToString();

          string ret ="隧道監視"+ devName + "tunnel:{0},place:{1},location:{2},status:{3}";
          return string.Format(ret, tunnel, place, location,status);
      }
    }
}
