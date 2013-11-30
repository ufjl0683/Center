using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
namespace RemoteInterface.TEM
{

    [Serializable]
  public   class LightAlarmData
    {
        public DateTime dt;
        public string tunnel,devName;
        public  int place,  div,required,damaged;
        public event OnEventHandler OnEvent;
        public event OnEventHandler OnEventStop;
      public LightAlarmData(string devName, DateTime dt, string tunnel, int place, int div, int required,int damaged)
      {

          this.dt = dt;
          this.tunnel = tunnel;
          this.place = place;
         // this.div = div;
        //  this.status = status;
          this.div = div;
          this.required = required;
          //this.damaged = damaged;
          this.devName = devName;
      }


      public bool  setStatus(int required,int damaged)
      {

          this.dt=DateTime.Now;
          if (this.damaged != damaged)
          {
              this.damaged = damaged;
              LightAlarmData data = new LightAlarmData(devName, dt, tunnel, place, div, required, damaged);
              data.damaged = damaged;
              if (damaged == 0 && this.OnEventStop != null)
              {

                  this.OnEventStop(data);
              }

              if (damaged != 0 && this.OnEvent != null)
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
              return "L-"+tunnel + "-" + place+"-"+div;
          }
      }

      public override string ToString()
      {
          //return base.ToString();

          string ret = "隧道照明"+   devName + "tunnel:{0},place:{1},div:{2},required:{3},damaged:{4}";
          return string.Format(ret, tunnel, place,div ,required,damaged);
      }
    }
}
