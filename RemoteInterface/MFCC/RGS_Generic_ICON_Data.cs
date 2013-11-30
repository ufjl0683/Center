using System;
using System.Collections.Generic;
using System.Text;


namespace RemoteInterface.MFCC
{
    [Serializable]
    public class RGS_Generic_ICON_Data
    {
        public ushort x, y;
        public byte icon_code_id;
        public RGS_Generic_ICON_Data(byte icon_code_id, ushort x, ushort y)
        {
            this.x = x;
            this.y = y;
            this.icon_code_id = icon_code_id;
        }

        public bool Equals(RGS_Generic_ICON_Data data)
        {
            return this.x == data.x && this.y == data.y && this.icon_code_id == data.icon_code_id;
        }

        public override string ToString()
        {
            return string.Format("x:{0} y:{1} icondid:{2} ", x, y, icon_code_id);
            //return base.ToString();
        }
    }

}
