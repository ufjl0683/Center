using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    [Serializable]
    public class ExecutionObj
    {
        private string rspID = "";              //反應計劃編號
        private string eventID = "";            //事件編號
        private System.DateTime eventTime;      //事件發生或改變時間
        private int alarmClass;                 //警告編號
        private List<LiaiseUnit> units;         //連絡單位資料        
        private System.Collections.Hashtable wisOutputDatas;       //WIS輸出資料
        private System.Collections.Hashtable cmsOutputDatas;       //CMS輸出資料
        private System.Collections.Hashtable rgsOutputDatas;       //RGS輸出資料
        private System.Collections.Hashtable rmsOutputDatas;       //RMS輸出資料
        private System.Collections.Hashtable cctvOutputDatas;      //CCTV輸出資料
        private System.Collections.Hashtable cslsOutputDatas;      //CSLS輸出資料
        private System.Collections.Hashtable lcsOutputDatas;       //LCS輸出資料
        private System.Collections.Hashtable fsOutputDatas;        //LCS輸出資料
        private string memo = "";                                  //測試用字串
        private int mode=2;                                        //模式        
        /// <summary>
        /// 反應計劃編號
        /// </summary>
        public string RspID
        {
            get { return rspID; }
            set { rspID = value; }
        }

        /// <summary>
        /// 事件編號
        /// </summary>
        public string EventID
        {
            get { return eventID; }
            set { eventID = value; }
        }

        /// <summary>
        /// 事件發生或改變時間
        /// </summary>
        public DateTime EventTime
        {
            get { return eventTime; }
            set { eventTime = value; }
        }

        /// <summary>
        /// 警告編號
        /// </summary>
        public int AlarmClass
        {
            get { return alarmClass; }
            set { alarmClass = value; }
        }

        /// <summary>
        /// 連絡單位資料
        /// </summary>
        public List<LiaiseUnit> Units
        {
            get { return units; }
            set { units = value; }
        }        

        /// <summary>
        ///  WIS輸出資料
        /// </summary>
        public System.Collections.Hashtable WISOutputData
        {
            get { return wisOutputDatas; }
            set { wisOutputDatas = value; }
        }

        /// <summary>
        ///  CMS輸出資料
        /// </summary>
        public System.Collections.Hashtable CMSOutputData
        {
            get { return cmsOutputDatas; }
            set { cmsOutputDatas = value; }
        }

        /// <summary>
        ///  RGS輸出資料
        /// </summary>
        public System.Collections.Hashtable RGSOutputData
        {
            get { return rgsOutputDatas; }
            set { rgsOutputDatas = value; }
        }

        /// <summary>
        ///  CSLS輸出資料
        /// </summary>
        public System.Collections.Hashtable CSLSOutputData
        {
            get { return cslsOutputDatas; }
            set { cslsOutputDatas = value; }
        }

        /// <summary>
        ///  FS輸出資料
        /// </summary>
        public System.Collections.Hashtable FSOutputData
        {
            get { return fsOutputDatas; }
            set { fsOutputDatas = value; }
        }

        /// <summary>
        ///  LCS輸出資料
        /// </summary>
        public System.Collections.Hashtable LCSOutputData
        {
            get { return lcsOutputDatas; }
            set { lcsOutputDatas = value; }
        }

        /// <summary>
        ///  CCTV輸出資料
        /// </summary>
        public System.Collections.Hashtable CCTVOutputData
        {
            get { return cctvOutputDatas; }
            set { cctvOutputDatas = value; }
        }

        /// <summary>
        ///  RMS輸出資料
        /// </summary>
        public System.Collections.Hashtable RMSOutputData
        {
            get { return rmsOutputDatas; }
            set { rmsOutputDatas = value; }
        }

        /// <summary>
        /// 備註
        /// </summary>
        public string Memo
        {
            get { return memo; }
            set { memo = value; }
        }

        public int RGS_Mode
        {
            get { return mode; }
            set { mode = value; }
        }
    }
}
