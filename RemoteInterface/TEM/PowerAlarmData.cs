using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
namespace RemoteInterface.TEM
{

    [Serializable]
  public   class PowerAlarmData
    {
        public DateTime dt;
        public string tunnel,devName;
        public  int place,  status;
        public event OnEventHandler OnEvent;
        public event OnEventHandler OnEventStop;
      public PowerAlarmData(string devName,DateTime dt, string tunnel, int place, int status)
      {

          this.dt = dt;
          this.tunnel = tunnel;
          this.place = place;
         // this.div = div;
       //   this.status = status;
          this.devName = devName;
      }


      public bool setStatus(int status)
      {

          this.dt=DateTime.Now;
          if (this.status != status)
          {
              this.status = status;
              PowerAlarmData data = new PowerAlarmData(devName, dt, tunnel, place, status);
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
              return "P-"+tunnel + "-" + place;
          }
      }

      public override string ToString()
      {
          //return base.ToString();

          string ret ="隧道配電"+ devName + "tunnel:{0},place:{1},status:{2}";
          return string.Format(ret, tunnel, place, status);
      }
    }
}
