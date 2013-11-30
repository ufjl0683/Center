using System;
using System.Collections.Generic;
using System.Text;

namespace Comm
{
  public   class TCDLE30AckPackage:AckPackage
    {
      //  byte[] data;

      public TCDLE30AckPackage(byte[] ack):base(ack)
        {
          // data = ack;
            
        }

        public override int Seq
        {
            get
            {
                return data[0];
            }
            
        }

      public override int Address
        {
            get
            {
                return data[1] * 256 + data[2];
            }
        }

      public  int Len
      {
          get
          {
              return data[3] * 256 + data[4];
          }
      }
       
        public override int LRC
        {
            get
            {
                return data[5];
            }
        }

        public override void SetErrBit(int inx,bool b)
       {
           throw new System.NotImplementedException();
           //if (!(inx >= 0 && inx <= 15))
           //    return ;
           //// ((((data[3] * 256 + data[4]) >> inx) & 1) == 1);
           //if (inx >= 0 && inx < 8)
           //{
           //    if (b)
           //        data[4] |= (byte)(1 << inx);
           //    else
           //        data[4] &= (byte)~(1 << inx);

           //}
           //else
           //{
           //    inx %= 8;
           //    if (b)
           //        data[3] |= (byte)(1 << inx);
           //    else
           //        data[3] &= (byte)~(1 << inx);

           //}
       }

        public override string ToString()
        {
            return "ACK:"+Comm.V2DLE.ToHexString(data);
        }
        

    }
}
