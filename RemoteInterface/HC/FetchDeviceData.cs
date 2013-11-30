using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HC
{
    [Serializable]
     public  class FetchDeviceData
    {
         public string DevName;
         public int SegId;

         public string Location;
         public int Mileage;
         public string LineId;
         public string Direction;
         public int maxSpd, minSpd;
         public string DeviceType;
         public FetchDeviceData(string DevName, int SegId, string LineId, string Direction, int Mileage, int maxSpd, int minSpd, string DeviceType)
         {
             this.DevName = DevName;
             this.SegId = SegId;
             this.LineId = LineId;
             this.Direction = Direction;
             this.Mileage = Mileage;
             this.maxSpd = maxSpd;
             this.minSpd = minSpd;
             this.DeviceType = DeviceType;
         }
    }
}
