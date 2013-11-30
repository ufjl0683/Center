using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{
    [Serializable]
   public  class VI_5Min_Data
    {
       public string devName;
       public DateTime dt;
       public int distance, degree;
       public VI_5Min_Data(string devName, DateTime dt, int distance, int degree)
       {
           this.degree = degree;
           this.devName = devName;
           this.dt = dt;
           this.distance = distance;
           
       }
    }
}
