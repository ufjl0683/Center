using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HC
{
    [Serializable]
  public   class CMSOutputDataOld
    {
        public int icon_id;
        public int g_code_id;
        public int hor_space;
        public string mesg;
        public byte[] colors;
        public  byte[] vspaces;
        public int dataType=0;
        

      public CMSOutputDataOld(int dataType, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors, byte[] vspaces)
          : this(icon_id, g_code_id, hor_space, mesg, colors, vspaces)
      {
        
          this.dataType=dataType;

      }
      public CMSOutputDataOld(int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors, byte[] vspaces)
      {
          this.icon_id = icon_id;
          this.g_code_id = g_code_id;
          this.hor_space = hor_space;
          this.mesg = mesg;
          this.colors = colors;
          this.vspaces = vspaces;
      }
        public CMSOutputDataOld(int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors):this(icon_id,g_code_id,hor_space,mesg,colors,new byte[0])
        {
            
        }

    }

 [Serializable]
  public   class CMSOutputData 
    {
        public int icon_id;
        public int g_code_id;
        public int hor_space;
        public string mesg;
        public byte[] colors;
        public  byte[] vspaces;
        public int dataType=0;
        public int AlarmClass=-1;

      public CMSOutputData(int dataType, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors, byte[] vspaces)
          : this(icon_id, g_code_id, hor_space, mesg, colors, vspaces)
      {
        
          this.dataType=dataType;

      }
      public CMSOutputData(int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors, byte[] vspaces)
      {
          this.icon_id = icon_id;
          this.g_code_id = g_code_id;
          this.hor_space = hor_space;
          this.mesg = mesg;
          this.colors = colors;
          this.vspaces = vspaces;
      }
        public CMSOutputData(int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors):this(icon_id,g_code_id,hor_space,mesg,colors,new byte[0])
        {
            
        }

    }
}
