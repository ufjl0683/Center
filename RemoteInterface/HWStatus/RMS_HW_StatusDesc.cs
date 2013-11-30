using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace RemoteInterface.HWStatus
{

    public enum RMS_HW_Status_Bit_Enum
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
        MainVD_Err = 12,
        DelayVD_Err = 13,
        ArrivalLeaveVD_Err = 14,
        Bit15 = 15,
        RedLightCountDownInDisplay=16,
        GreenLightCountDownInDisplay=17,
        RedLightErr=18,
        GreenLightErr=19,
        RedLightNotMatch=20,
        GreenLightNotMatch=21,
        LightPutOff=22


    }

    [Serializable]
   public class RMS_HW_StatusDesc:I_HW_Status_Desc
    {
        public static string[] hw_status_desc = new string[]
           {
               "設備故障","箱門開啟","手提測試機操作","現場操作","要求下傳基本參數","自行重新起動","燈號熄減","輸入單元故障",
               "顯示設備故障","終端控制器與上層連線異常","終端控制器與下層連線異常","LED故障模組","主線車輛偵測器故障","延滯車輛偵測器故障","到達/駛離車輛偵測器故障","Bit15",
               "倒數燈紅燈顯示中","倒數燈綠燈顯示中","倒數燈紅燈故障","倒數燈綠燈故障","倒數燈紅燈不符","倒數燈綠燈不符","熄滅中","未定義",
               "未定義","未定義","未定義","未定義","未定義","未定義","未定義","未定義"
           };

        System.Collections.BitArray ArrayhwStatus;
        byte[] diff;
        string devName;
       byte[] m_status;


        public RMS_HW_StatusDesc(string devName,byte[] status, byte[] diff)
        {
            ArrayhwStatus = new System.Collections.BitArray(status);
            this.diff = diff;
            this.devName = devName;
            this.m_status = status;
        }
        public RMS_HW_StatusDesc(string devName,byte[] status):this(devName,status,new byte[]{0xff,0xff,0xff,0xff})
        {
        }
        #region I_HW_Status_Desc 成員

         public string getDesc(int bitinx)
        {
          //  throw new Exception("The method or operation is not implemented.");
            return ((RMS_HW_Status_Bit_Enum)bitinx).ToString();
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
                    yield return (RMS_HW_Status_Bit_Enum)i;
            }
        }

        public  System.Collections.IEnumerable getEnum()  //取得  diff  指示的列舉值
        {
          //  throw new Exception("The method or operation is not implemented.");
            System.Collections.BitArray aryInx = new System.Collections.BitArray(diff);
            for (int i = 0; i < aryInx.Count; i++)
            {
                if (aryInx.Get(i))
                    yield return (RMS_HW_Status_Bit_Enum)i;
            }
        }

        #endregion

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
