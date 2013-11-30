using System;
using System.Collections.Generic;
using System.Text;

namespace Comm
{
   public  class NakPackage
    {
     protected  byte[] data;
       
       public NakPackage(byte[] nak)
       {
           this.data = nak;
       }

    virtual public int Seq
       {
           get
           {
               return data[0];
           }
       }
       virtual public int Address
       {
           get
           {
               return data[1] * 256 + data[2];
           }
       }

       virtual public byte[] Err
       {
           get
           {
             return  new byte[] { data[3], data[4] };
           }
       }


       virtual public int LRC
       {
           get
           {
               return data[5];
           }
        }


     public virtual   bool GetErrBit(int inx)
       {
           if (!(inx >= 0 && inx <= 15))
               return false;    
           return ((((data[3]*256+data[4])>>inx) & 1)==1);
       }

       public virtual void SetErrBit(int inx,bool b)
       {
           if (!(inx >= 0 && inx <= 15))
               return ;
           // ((((data[3] * 256 + data[4]) >> inx) & 1) == 1);
           if (inx >= 0 && inx < 8)
           {
               if (b)
                   data[4] |= (byte)(1 << inx);
               else
                   data[4] &= (byte)~(1 << inx);

           }
           else
           {
               inx %= 8;
               if (b)
                   data[3] |= (byte)(1 << inx);
               else
                   data[3] &= (byte)~(1 << inx);

           }
       }

      
        public override string ToString()
        {
            return "NAK:"+Comm.V2DLE.ToHexString(data);
        }
      
    }
}
