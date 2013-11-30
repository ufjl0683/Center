using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{
    [Serializable]
     public   class AVIPlateData
    {
        public string DevName;
        public  System.DateTime dt;
         public string plate;
         public AVIPlateData(string DevName, System.DateTime dt, string plate)
         {
             this.DevName = DevName;
             this.dt = dt;
             this.plate = plate;
         }

        
    }
}
