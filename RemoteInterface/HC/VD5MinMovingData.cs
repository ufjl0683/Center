using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HC
{
    [Serializable]
  public   class VD5MinMovingData
    {
      public string devName;
     public  int vol, spd, occ, jamlvl;
      public VD5MinMovingData(string devName,int vol, int spd, int occ, int jamlvl)
      {
          this.devName = devName;
          this.vol = vol;
          this.spd = spd;
          this.occ = occ;
          this.jamlvl = jamlvl;
      }

    }
}
