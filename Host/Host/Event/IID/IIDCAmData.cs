using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.IID
{
  public   class IID_CAM_Data
    {

      public string lineid, direction, devName;
      public int camid,laneid,mileage;
      public int eventid,action;
      public event EventHandler OnEvent;
      public Host.TC.DeviceBaseWrapper devWrapper;
      public IID_CAM_Data(string lineid, string direction, string devName, int camid, int laneid, int mileage)
      {
          this.lineid = lineid;
          this.direction = direction;
          this.devName = devName;
          this.camid = camid;
          this.laneid = laneid;
          this.mileage = mileage;
          devWrapper = new Host.TC.DeviceBaseWrapper("MFCC_IID", devName, "IID", "127.0.0.1", 0, "F", "N6", mileage, new byte[] { 0, 0, 0, 0 }, 0, 0, direction);
      }

      public string Key
      {
          get
          {
              return camid + "-" + laneid;
          }
      }

    public  void setEvent(int eventid, int action)
      {
          this.eventid = eventid;
          this.action = action;
          if (this.OnEvent != null)
              this.OnEvent(this, null);
      }


   

    }
}
