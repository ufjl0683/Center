using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HWStatus
{
    public enum TTS_HW_Status_Bit_Enum
    {
        DeviceErr = 0,
        CabineteOpen = 1,
        PortableTest = 2,
        PanelOperate = 3,
        ParameterRequest = 4,
        AutoRestart = 5,
        LightSignalOff = 6,
        IO_unitFail = 7,
        DisplayErr = 8,
        UplinkErr = 9,
        DownLinkErr = 10,
        LED_ModuleErr = 11,
        Reserve = 12,
        Ligth_Err = 13,
        Bit14 = 14,
        Bit15 = 15,
        Bit16 = 16,
        Bit17 = 17,
        Bit18 = 18,
        Bit19 = 19,
        Bit20 = 20,
        Bit21 = 21,
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
  public   class TTS_HW_StatusDesc:I_HW_Status_Desc
    {
        public static string[] hw_status_desc = new string[]
           {    "設備故障","箱門開啟","手提測試機操作","現場操作","要求下傳基本參數","自行重新起動","燈號熄減","輸入單元故障",
               "顯示設備故障","終端控制器與上層連線異常","終端控制器與下層連線異常","LED故障模組","未定義","照明燈故障","未定義","Bit15",
                "未定義","未定義","未定義","未定義","未定義","未定義","未定義","未定義",
               "未定義","未定義","未定義","未定義","未定義","未定義","未定義","未定義"
           };

        System.Collections.BitArray ArrayhwStatus;
        byte[] diff;
      public string devName;
      byte[] m_status;
         public TTS_HW_StatusDesc(string devName,byte[] hw_status, byte[] diff)
        {
            ArrayhwStatus = new System.Collections.BitArray(hw_status);
            this.diff = diff;
            this.devName = devName;
            m_status = hw_status;
        }
        public TTS_HW_StatusDesc(string devName,byte[] hw_status)
            : this(devName,hw_status, new byte[] { 0xff, 0xff, 0xff, 0xff })
        {

        }
        public string getDesc(int bitinx)
        {
            return ((TTS_HW_Status_Bit_Enum)bitinx).ToString();
        }
        public string getChiDesc(int inx)
        {
            return hw_status_desc[inx];
        }
        public bool getStatus(int bitinx)
        {
            return ArrayhwStatus.Get(bitinx);
        }

        public System.Collections.IEnumerable getEnum(byte[] indexs)
        {
            System.Collections.BitArray aryInx = new System.Collections.BitArray(indexs);
            for (int i = 0; i < aryInx.Count; i++)
            {
                if (aryInx.Get(i))
                    yield return (TTS_HW_Status_Bit_Enum)i;
            }
        }
        public System.Collections.IEnumerable getEnum()
        {
            System.Collections.BitArray aryInx = new System.Collections.BitArray(diff);
            for (int i = 0; i < aryInx.Count; i++)
            {
                if (aryInx.Get(i))
                    yield return (TTS_HW_Status_Bit_Enum)i;
            }
        }

        #region I_HW_Status_Desc 成員


        public string getDeviceName()
        {
            return this.devName;
        }

        #endregion

        #region I_HW_Status_Desc 成員


        public byte[] getHW_status()
        {
            return m_status;
        }

        #endregion
    }
}
