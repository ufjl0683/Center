using System;
using System.Collections.Generic;
using System.Text;

namespace Comm
{
   public  class TCDLE30NakPackage:NakPackage
    {
      // byte[] data;
       //System.Collections.BitArray Errs;
       public TCDLE30NakPackage(byte[] nak):base(nak)
       {
         //  this.data = nak;
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

       public int Len
       {
           get
           {
               return data[3] * 256 + data[4];
           }
       }


       public override byte[] Err
       {
           get
           {
             return  new byte[] { data[5] };
           }
       }


       public override int LRC
       {
           get
           {
               return data[6];
           }
        }


       public override bool GetErrBit(int inx)
       {
           //return base.GetErrBit(inx);
           if (((this.Err[0] >> inx) & 0x01) == 0)
               return false;
           else
               return true;
       }


       public override void SetErrBit(int inx, bool b)
       {
           if (b)
               this.Err[0] = (byte)(this.Err[0] | (1 << inx));
           else
               this.Err[0] = (byte)(this.Err[0] & (~(1 << inx)));
       }

   //public    bool GetErrBit(int inx)
   //    {
   //        if (!(inx >= 0 && inx <= 15))
   //            return false;    
   //        return ((((data[3]*256+data[4])>>inx) & 1)==1);
   //    }

   //    public void SetErrBit(int inx,bool b)
   //    {
   //        if (!(inx >= 0 && inx <= 15))
   //            return ;
   //        // ((((data[3] * 256 + data[4]) >> inx) & 1) == 1);
   //        if (inx >= 0 && inx < 8)
   //        {
   //            if (b)
   //                data[4] |= (byte)(1 << inx);
   //            else
   //                data[4] &= (byte)~(1 << inx);

   //        }
   //        else
   //        {
   //            inx %= 8;
   //            if (b)
   //                data[3] |= (byte)(1 << inx);
   //            else
   //                data[3] &= (byte)~(1 << inx);

   //        }
   //    }

      
        public override string ToString()
        {
            return "NAK:"+Comm.V2DLE.ToHexString(data);
        }
      
    }
}
