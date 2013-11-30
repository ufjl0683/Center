using System;
using System.Collections.Generic;
using System.Text;

namespace Comm.TC
{
    public enum RGS_HW_Status_Bit_Enum
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
        DisplayOverLoad = 12,
        CGS_LightErr = 13,
        CGS_AlarmLightErr = 14,
        RTU_ReactErr = 15

    }

    [Serializable]
    public class RGS_HW_StatusDesc : I_HW_Status_Desc
    {
        public static string[] hw_status_desc = new string[]
           {
               "設備故障","箱門開啟","手提測試機操作","現場操作","要求下傳基本參數","自行重新起動","燈號熄減","輸入單元故障",
               "顯示設備故障","終端控制器與上層連線異常","終端控制器與下層連線異常","LED故障模組","顯示板過電流/過電壓","CGS 照明燈異常"," CGS 警示黃燈故障","RTU 連動通訊異常"
           };

        System.Collections.BitArray ArrayhwStatus;
        byte[] diff;


        public RGS_HW_StatusDesc(byte[] hw_status, byte[] diff)
        {
            ArrayhwStatus = new System.Collections.BitArray(hw_status);
            this.diff = diff;
        }
        public RGS_HW_StatusDesc(byte[] hw_status)
            : this(hw_status, new byte[] { 0xff, 0xff, 0xff, 0xff })
        {

        }
        public string getDesc(int bitinx)
        {
            return ((RGS_HW_Status_Bit_Enum)bitinx).ToString();
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
                    yield return (RGS_HW_Status_Bit_Enum)i;
            }
        }
        public System.Collections.IEnumerable getEnum()
        {
            System.Collections.BitArray aryInx = new System.Collections.BitArray(diff);
            for (int i = 0; i < aryInx.Count; i++)
            {
                if (aryInx.Get(i))
                    yield return (RGS_HW_Status_Bit_Enum)i;
            }
        }



    }

}
