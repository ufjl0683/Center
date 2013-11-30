using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime;

namespace RemoteInterface.MFCC
{
    [Serializable]
   public  class TravelTimeData
    {
       public int traverlTime=-1; 
       public int upperLimit=-1;
        public int lowerLimit=-1;

       public TravelTimeData(int traverlTime, int upperLimit, int lowerLimit)
       {
           this.traverlTime = traverlTime;
           this.upperLimit = upperLimit;
           this.lowerLimit = lowerLimit;
       }


     
    }
}
