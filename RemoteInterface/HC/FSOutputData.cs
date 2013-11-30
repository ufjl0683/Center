using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HC
{
    [Serializable]
   public  class FSOutputData
    {
      public  byte type;
       public FSOutputData(byte type)
       {
           this.type = type;
       }
    }
}
