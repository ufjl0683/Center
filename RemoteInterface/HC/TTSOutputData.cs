using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HC
{
    [Serializable]
   public class TTSOutputData
    {

       public  byte[] boardid;
       public string[] traveltime;
       public byte[] color;

        //  解除手動時 travelTime 及 color 均為 0xff
       public TTSOutputData(byte[] boardid, string[] traveltime, byte[] color)
       {
           this.boardid = boardid;
           this.traveltime = traveltime;
           this.color = color;

           if (boardid.Length > 3 || traveltime.Length > 3 || color.Length > 3)
               throw new Exception("array length can not greater than 3!");
           if (!(boardid.Length == traveltime.Length && traveltime.Length == color.Length))
               throw new Exception("Array Dimension are not equal!");
       }
    }
}
