using System;
using System.Collections.Generic;
using System.Text;
/*
 * 
 * byte 2:
bit 0: 車道1之顯示設備故障
bit 1: 車道1之終端控制器與上層連線異常
bit 2: 車道1之終端控制器與下層連線異常
bit 3: 車道1之LED 或燈泡模組故障
bit 4: 車道2之顯示設備故障
bit 5: 車道2之終端控制器與上層連線異常
bit 6: 車道2之終端控制器與下層連線異常
bit 7: 車道2之LED 或燈泡模組故障
byte 3:
bit 0: 車道3之顯示設備故障
bit 1: 車道3之終端控制器與上層連線異常
bit 2: 車道3之終端控制器與下層連線異常
bit 3: 車道3之LED 或燈泡模組故障
bit 4: 車道4之顯示設備故障
bit 5: 車道4之終端控制器與上層連線異常
bit 6: 車道4之終端控制器與下層連線異常
bit 7: 車道4之LED 或燈泡模組故障
byte 4:
bit 0: 車道5之顯示設備故障
bit 1: 車道5之終端控制器與上層連線異常
bit 2: 車道5之終端控制器與下層連線異常
bit 3: 車道5之LED 或燈泡模組故障
bit 4: 車道6之顯示設備故障
bit 5: 車道6之終端控制器與上層連線異常
bit 6: 車道6之終端控制器與下層連線異常
bit 7: 車道6之LED 或燈泡模組故障
*/
namespace RemoteInterface.HWStatus
{
    public enum MAS_HW_Status_Bit_Enum
    {
        DeviceErr = 0,
        CabineteOpen = 1,
        PortableTest = 2,
        PanelOperate = 3,
        ParameterRequest = 4,
        AutoRestart = 5,
        LightSignalOff = 6,
        IO_unitFail = 7,
        Lane1DisplayErr = 8,
        Lane1UpLinkErr = 9,
        Lane1DownLinkErr = 10,
        Lane1_LED_ModuleErr = 11,
        Lane2DisplayErr = 12,
        Lane2UpLinkErr = 13,
        Lane2DownLinkErr = 14,
        Lane2_LED_ModuleErr =15,

        Lane3DisplayErr = 16,
        Lane3UpLinkErr = 17,
        Lane3DownLinkErr = 18,
        Lane3_LED_ModuleErr = 19,
        Lane4DisplayErr = 20,
        Lane4UpLinkErr = 21,
        Lane4DownLinkErr = 22,
        Lane4_LED_ModuleErr = 23,

        Lane5DisplayErr = 24,
        Lane5UpLinkErr = 25,
        Lane5DownLinkErr = 26,
        Lane5_LED_ModuleErr = 27,
        Lane6DisplayErr = 28,
        Lane6UpLinkErr = 29,
        Lane6DownLinkErr = 30,
        Lane6_LED_ModuleErr = 31

    }
  public   class MAS_HW_StatusDesc:I_HW_Status_Desc
    {
        public static string[] hw_status_desc = new string[]
           {    "設備故障","箱門開啟","手提測試機操作","現場操作","要求下傳基本參數","自行重新起動","燈號熄減","輸入單元故障",
                "車道1之顯示設備故障","車道1之終端控制器與上層連線異常","車道1之終端控制器與下層連線異常","車道1之LED 或燈泡模組故障","車道2之顯示設備故障","車道2之終端控制器與上層連線異常","車道2之終端控制器與下層連線異常","車道2之LED 或燈泡模組故障",
                "車道3之顯示設備故障","車道3之終端控制器與上層連線異常","車道3之終端控制器與下層連線異常","車道3之LED 或燈泡模組故障","車道4之顯示設備故障","車道4之終端控制器與上層連線異常","車道4之終端控制器與下層連線異常","車道4之LED 或燈泡模組故障",
                "車道5之顯示設備故障","車道5之終端控制器與上層連線異常","車道5之終端控制器與下層連線異常","車道5之LED 或燈泡模組故障","車道6之顯示設備故障","車道6之終端控制器與上層連線異常","車道6之終端控制器與下層連線異常","車道6之LED 或燈泡模組故障"
           };

        System.Collections.BitArray ArrayhwStatus;
        byte[] diff;
      public string devName;
      byte[] m_status;
         public MAS_HW_StatusDesc(string devName,byte[] hw_status, byte[] diff)
        {
            ArrayhwStatus = new System.Collections.BitArray(hw_status);
            this.diff = diff;
            this.devName = devName;
            m_status = hw_status;
        }
        public MAS_HW_StatusDesc(string devName,byte[] hw_status)
            : this(devName,hw_status, new byte[] { 0xff, 0xff, 0xff, 0xff })
        {

        }
        public string getDesc(int bitinx)
        {
            return ((MAS_HW_Status_Bit_Enum)bitinx).ToString();
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
                    yield return (MAS_HW_Status_Bit_Enum)i;
            }
        }
        public System.Collections.IEnumerable getEnum()
        {
            System.Collections.BitArray aryInx = new System.Collections.BitArray(diff);
            for (int i = 0; i < aryInx.Count; i++)
            {
                if (aryInx.Get(i))
                    yield return (MAS_HW_Status_Bit_Enum)i;
            }
        }

    


        public string getDeviceName()
        {
            return this.devName;
        }

      

    


        public byte[] getHW_status()
        {
            return m_status;
        }

       
    }
}
