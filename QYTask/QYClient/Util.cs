using System;
using System.Collections.Generic;
using System.Text;

namespace QYClient
{
  public   class Util
    {

      public static RemoteInterface.IQYCommands robj =  (RemoteInterface.IQYCommands)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.IQYCommands), QYClient.Properties.Settings.Default.RemomeUri);

      public static string ToColorString(System.Drawing.Color[] colors)
      {
          string ret = "";
          for (int i = 0; i < colors.Length; i++)
          {
              ret += colors[i].R.ToString() + "," + colors[i].G.ToString() + "," + colors[i].B.ToString() + ",";
          }
          return ret.TrimEnd(new char[] { ',' });

      }

      public static System.Drawing.Color[] ToColors(string colorstr)
      {
          string[] colorStr = colorstr.Split(new char[] { ',' });
          System.Drawing.Color[] colors = new System.Drawing.Color[colorStr.Length / 3];
          for (int i = 0; i < colors.Length; i++)

              colors[i] = System.Drawing.Color.FromArgb(System.Convert.ToInt32(colorStr[i * 3]), System.Convert.ToInt32(colorStr[i * 3 + 1]), System.Convert.ToInt32(colorStr[i * 3 + 2]));


          return colors;
      }


     

  }
}
