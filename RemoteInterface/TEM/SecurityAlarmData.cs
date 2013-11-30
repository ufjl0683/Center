using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
namespace RemoteInterface.TEM
{

    [Serializable]
  public   class SecurityAlarmData
    {
        public DateTime dt;
        public string tunnel,devName;
         public int place,  status,cardid;
        public event OnEventHandler OnEvent;
        public event OnEventHandler OnEventStop;
      public SecurityAlarmData(string devName, DateTime dt, string tunnel, int place,int cardid, int status)
      {

          this.dt = dt;
          this.tunnel = tunnel;
          this.place = place;
         // this.div = div;
        //  this.status = status;
          this.cardid = cardid;
          this.devName = devName;
      }


      public bool setStatus(int status)
      {

          this.dt=DateTime.Now;
          if (this.status != status)
          {

              this.status = status;
              SecurityAlarmData data = new SecurityAlarmData(devName, dt, tunnel, place, cardid, status);
              data.status = status; ;
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
              return  "S-"+tunnel + "-" + place+"-"+cardid;
          }
      }

      public override string ToString()
      {
          //return base.ToString();

          string ret ="隧道門禁"+ devName + "tunnel:{0},place:{1},cardid:{2},status:{3}";
          return string.Format(ret, tunnel, place, cardid,status);
      }
    }
}
