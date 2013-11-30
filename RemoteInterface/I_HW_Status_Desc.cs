using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface
{
 public interface I_HW_Status_Desc
    {
     
       
        string getDesc(int  bitinx);  //取得英文說明
        string getChiDesc(int inx);    //取得中文說明
        bool getStatus(int bitinx);    // 取得  bitinx 所指的狀態內容
       
        
        System.Collections.IEnumerable getEnum(byte[] indexs); //  取得indexs所有欄位列舉值
        System.Collections.IEnumerable getEnum();  // 取得 diff 所指的欄位列舉值
         string getDeviceName();
        byte[] getHW_status();
    }
}
