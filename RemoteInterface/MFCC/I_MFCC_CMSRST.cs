using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{
    public   interface I_MFCC_CMSRST : RemoteInterface.MFCC.I_MFCC_Base
    {
        //void SendDisplay(string devName,int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors);
        //void SendDisplay(string devName, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors,  byte[] v_spaces);
        void SendDisplay(int inx,string devName, int g_code_id, int hor_space, string mesg, byte[] colors, byte[] v_spaces);
        void SendDisplay(int inx,string devName, int g_code_id, int hor_space, string mesg, byte[] colors);
    //    void SendDisplay(string ip,int port, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors);
           //color length= mesg length execept '\r'
        void setDisplayOff(string devName);
        //void SetIconPic(string DevName, int icon_id, string desc, System.Drawing.Color[][] bmp);
        //System.Drawing.Color[,]  GetIconPic(string DevName, int icon_id, ref string desc);
        //void GetCurrentTCDisplay(string devname, ref int icon_id, ref int g_code_id, ref int hor_space, ref string mesg, ref byte[] colors);
        //void GetCurrentTCDisplay(string devname, ref int icon_id, ref int g_code_id, ref int hor_space, ref string mesg, ref byte[] colors,ref byte[] vspaces);
        void GetCurrentTCDisplay(string devname,  int inx, ref int dataType, ref int g_code_id, ref int hor_space, ref string mesg, ref byte[] colors, ref byte[] vspaces);
        void GetCurrentTCDisplay(string devname, ref int icon_id, ref int g_code_id, ref int hor_space, ref string mesg, ref byte[] colors);
        void GetCurrentTCDisplay(string devname, ref int icon_id, ref int g_code_id, ref int hor_space, ref string mesg, ref byte[] colors, ref byte[] vspaces);
        void GetCurrentTCDisplay(string devname, ref int dataType, ref int icon_id, ref int g_code_id, ref int hor_space, ref string mesg, ref byte[] colors, ref byte[] vspaces);

    }
}
