using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HC
{
     [Serializable]
    public   class WISOutputData:CMSOutputData
    {
         public int WisMesgType;
         public WISOutputData(int WisMesgType,int dataType, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors, byte[] vspaces):
             base(dataType,icon_id,g_code_id,hor_space,mesg,colors,vspaces)
         {
             this.WisMesgType = WisMesgType;
         }
    }
}
