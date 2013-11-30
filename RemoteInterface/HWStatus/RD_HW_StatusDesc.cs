using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HWStatus
{

    public enum RD_HW_Status_Bit_Enum
    {
        DeviceErr = 0,
        CabineteOpen = 1,
        PortableTest = 2,
        PanelOperate = 3,
        ParameterRequest = 4,
        AutoRestart = 5,
        LightSignalOff = 6,
        IO_unitFail = 7,

        CksErr = 8,
        SysMemErr = 9,
        DataMemErr = 10,
        Bit11 = 11,
        Bit12 = 12,
        Bit13 = 13,
        Bit14 = 14,
        Bit15 = 15,

        DetectUint1 = 16,
        DetectUnit2 = 17,
        DetectUnit3 = 18,
        DetectUnit4 = 19,
        DetectUnit5 = 20,
        DetectUnit6 = 21,
        Bit22 = 22,
        Bit23 = 23,

        Loop1 = 24,
        Loop2 = 25,
        Loop3 = 26,
        Loop4 = 27,
        Loop5 = 28,
        Loop6 = 29,

        Bit30 = 30,
        Bit31 = 31


    }

    /*byte 2:
bit 0 : 雨量偵測器故障
bit 1 – bit 7: 保留
byte 3-byte 4:保留
     * */
    [Serializable]
 public   class RD_HW_StatusDesc : I_HW_Status_Desc
    {
            public static string[] hw_status_desc = new string[]
           {
               "設備故障","箱門開啟","手提測試機操作","現場操作","要求下傳基本參數","自行重新起動","燈號熄減","輸入單元故障",
               "雨量偵測器故障","未定義","未定義","未定義","未定義","未定義","未定義","未定義",
               "未定義","未定義","未定義","未定義","未定義","未定義","未定義","未定義",
               "未定義","未定義", "未定義","未定義", "未定義","未定義","未定義","未定義"
           };

        System.Collections.BitArray ArrayhwStatus;
        byte[] diff;
        string devName;
       byte[] m_status;


        public RD_HW_StatusDesc(string devName,byte[] status, byte[] diff)
        {
           
            ArrayhwStatus = new System.Collections.BitArray(status);
            this.diff = diff;
            this.devName = devName;
            m_status = status;
        }
        public RD_HW_StatusDesc(string devName,byte[] status)
            : this(devName,status, new byte[] { 0xff, 0xff, 0xff, 0xff })
        {
        }
        #region I_HW_Status_Desc 成員

       public byte[] getHW_status()
       {
           return this.m_status;
       }

         public string getDesc(int bitinx)
        {
          //  throw new Exception("The method or operation is not implemented.");
            return ((RD_HW_Status_Bit_Enum)bitinx).ToString();
        }

        public  string getChiDesc(int inx)
        {
           // throw new Exception("The method or operation is not implemented.");
            return hw_status_desc[inx];
        }

       public   bool getStatus(int bitinx)
        {
          //  throw new Exception("The method or operation is not implemented.");
            return ArrayhwStatus.Get(bitinx);
        }

       public   System.Collections.IEnumerable getEnum(byte[] indexs)  //取得所有錯誤訊息的位元咧舉值
        {
          //  throw new Exception("The method or operation is not implemented.");
            System.Collections.BitArray aryInx = new System.Collections.BitArray(indexs);
            for (int i = 0; i < aryInx.Count; i++)
            {
                if (aryInx.Get(i))
                    yield return (RD_HW_Status_Bit_Enum)i;
            }
        }

        public  System.Collections.IEnumerable getEnum()  //取得  diff  指示的列舉值
        {
          //  throw new Exception("The method or operation is not implemented.");
            System.Collections.BitArray aryInx = new System.Collections.BitArray(diff);
            for (int i = 0; i < aryInx.Count; i++)
            {
                if (aryInx.Get(i))
                    yield return (RD_HW_Status_Bit_Enum)i;
            }
        }

        #endregion









        #region I_HW_Status_Desc 成員


        public string getDeviceName()
        {
            return this.devName;
        }

        #endregion


    }
}
