using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{
    [Serializable]
    public class RGS_Generic_Section_Data
    {
        public byte section_id, status;
        public RGS_Generic_Section_Data(byte section_id, byte status)
        {
            this.section_id = section_id;
            this.status = status;
        }

        public bool Equals(RGS_Generic_Section_Data data)
        {
          //  Console.WriteLine(section_id + "," + data.section_id + "," + status + "," + data.status);
            return this.section_id == data.section_id && this.status == data.status;
        }

        public override string ToString()
        {
            ///return base.ToString();
            ///
            return string.Format("sec_id:{0} status:{1}",section_id,status);
        }
    }
}
