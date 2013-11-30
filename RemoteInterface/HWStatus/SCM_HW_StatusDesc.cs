using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HWStatus
{
    public enum SCM_HW_Status_Bit_Enum
    {
        CPU_Err = 0,
        MemoryErr = 1,
        TimerErr = 2,
        WatchDogErr = 3,
        PowerErr = 4,
        IO_UnitErr = 5,
        SignalDriveUnitErr = 6,
        SignalHeadErr = 7,
        Connected = 8,
        CaninatedOpen = 9,
        TimePlanErr = 10,
        SigConflictErr = 11,
        SigPowerErr = 12,
        TimingPlanOnTransition = 13,
        ControllerReady = 14,
        CommLineBad = 15,
        Bit16 = 16,
        Bit17 = 17,
        Bit18 = 18,
        Bit19 = 19,
        Bit20 = 20,
        Bit21 = 21,
        Bit22 = 22,
        Bit23 = 23,
        Bit24 = 24,
        Bit25 = 25,
        Bit26 = 26,
        Bit27 = 27,
        Bit28 = 28,
        Bit219 = 29,
        Bit30 = 30,
        Bit31 = 31
    }
  public   class SCM_HW_StatusDesc:I_HW_Status_Desc
    {
        public static string[] hw_status_desc = new string[]
           {
               "CPU故障","記憶體故障","計時單元故障","WatchDog 故障","電源異常","IO單元故障","燈號驅動單元故障","燈泡故障",
               "連線狀況","箱門開啟","時制異常","訊號衝突","輸出電源異常","時制轉換","控制器正常","通訊故障",
               "未定義","未定義","未定義","未定義","未定義","未定義","未定義","未定義",
               "未定義","未定義","未定義","未定義","未定義","未定義","未定義","未定義"
           };

        System.Collections.BitArray ArrayhwStatus;
        byte[] diff;
      public string devName;
      byte[] m_status;
         public SCM_HW_StatusDesc(string devName,byte[] hw_status, byte[] diff)
        {
            ArrayhwStatus = new System.Collections.BitArray(hw_status);
            this.diff = diff;
            this.devName = devName;
            m_status = hw_status;
        }
        public SCM_HW_StatusDesc(string devName,byte[] hw_status)
            : this(devName,hw_status, new byte[] { 0xff, 0xff, 0xff, 0xff })
        {

        }
        public string getDesc(int bitinx)
        {
            return ((SCM_HW_Status_Bit_Enum)bitinx).ToString();
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
                    yield return (SCM_HW_Status_Bit_Enum)i;
            }
        }
        public System.Collections.IEnumerable getEnum()
        {
            System.Collections.BitArray aryInx = new System.Collections.BitArray(diff);
            for (int i = 0; i < aryInx.Count; i++)
            {
                if (aryInx.Get(i))
                    yield return (SCM_HW_Status_Bit_Enum)i;
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
