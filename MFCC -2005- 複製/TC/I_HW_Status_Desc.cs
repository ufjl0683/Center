using System;
using System.Collections.Generic;
using System.Text;

namespace Comm
{
 public interface I_HW_Status_Desc
    {
     
       
        string getDesc(int  bitinx);
        string getChiDesc(int inx);
        bool getStatus(int bitinx);
       

        System.Collections.IEnumerable  getEnum(byte[] indexs);
        System.Collections.IEnumerable getEnum();
        
    }
}
