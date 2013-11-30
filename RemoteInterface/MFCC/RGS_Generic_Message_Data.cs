using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace RemoteInterface.MFCC
{
    [Serializable]
    public class RGS_Generic_Message_Data
    {
        public ushort x, y;
        public Color[] forecolor;
        public Color[] backcolor;
        public string messgae;
        public RGS_Generic_Message_Data(string msg, Color[] foreColor, Color[] backColor, ushort x, ushort y)
        {
            this.x = x;
            this.y = y;
            this.forecolor = foreColor;
            this.backcolor = backColor;
            this.messgae = msg;
            if (messgae.Length != forecolor.Length || messgae.Length != backcolor.Length)
                throw new Exception("messagge legth and  color length are not equal!");
        }

        public bool Equals(RGS_Generic_Message_Data data)
        {
            if (this.x != data.x)
                return false;
            if (this.y != data.y)
                return false;
            if (this.forecolor.Length != data.forecolor.Length) return false;
            if (backcolor.Length != data.backcolor.Length) return false;
            if (messgae != data.messgae) return false;
            for (int i = 0; i < forecolor.Length; i++)
                if (forecolor[i].ToArgb() != data.forecolor[i].ToArgb()) return false;
            for (int i = 0; i < backcolor.Length; i++)
                if (backcolor[i].ToArgb() != backcolor[i].ToArgb()) return false;

            return true;


           
        }


        public override string ToString()
        {
            //return base.ToString();
            return string.Format("x:{0} y:{1} mesg:{2}", x, y, messgae);
        }

    }
}
