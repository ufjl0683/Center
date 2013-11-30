using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using RemoteInterface.MFCC;

namespace RemoteInterface.MFCC
{
   public  interface I_MFCC_RGS:I_MFCC_Base
    {


    //   void setBackGroundPic(string ip, int port, int mode, int g_code_id, string desc, Color[][] pic);
      
       void setBackGroundPic(string devName, int mode, int g_code_id, string desc, Color[][] pic);
    //   void setIconPic(string ip, int port, int icon_id, string desc, Color[][] pic);
       void setIconPic(string devName, int icon_id, string desc, Color[][] pic);
     //  Color[,] getIconPic(string ip, int port, int icon_id, ref string desc);
       Color[,] getIconPic(string devName, int icon_id,ref string desc);
    //   Color[][] getBackGroundPic(string ip, int port, int mode, int g_code_id, ref string desc);
       Color[,] getBackGroundPic(string devName, int mode, int g_code_id, ref string desc);
       void setGenericDisplay(string devName, RGS_GenericDisplay_Data data);
       RGS_GenericDisplay_Data getCurrentGenericDisplay(string devName);
     //  void setGenericDisplay(string ip,int port, RGS_GenericDisplay_Data data);
       void setDisplayOff(string devName);
       void setPolygonData(string devName,byte g_code_id, RGS_PolygonData pdata);
       RGS_PolygonData getPolygonData(string devName, byte g_code_id);



    }
}
