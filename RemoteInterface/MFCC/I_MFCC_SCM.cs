using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{
    public   interface I_MFCC_SCM : RemoteInterface.MFCC.I_MFCC_Base
    {
        //void SendDisplay(string devName, int g_code_id, int hor_space, string mesg, byte[] colors);
        ////g_code_id =0 文字模式, g_code_id=1 圖形模式
        //void SendDisplay(string ip,int port,  int g_code_id, int hor_space, string mesg, byte[] colors);
        //void setDisplayOff(string devName);
        //void GetCurrentTCDisplay(string devName, ref int g_code_id, ref int hor_space, ref string mesg, ref byte[] colors);
        //    //color length= mesg length execept '\r'
        //g_code_id =0 文字模式, g_code_id=1 圖形模式
    }
}
