using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    [Serializable]
    public class ExecutionObj
    {
        private string rspID = "";              //�����p���s��
        private string eventID = "";            //�ƥ�s��
        private System.DateTime eventTime;      //�ƥ�o�ͩΧ��ܮɶ�
        private int alarmClass;                 //ĵ�i�s��
        private List<LiaiseUnit> units;         //�s�������        
        private System.Collections.Hashtable wisOutputDatas;       //WIS��X���
        private System.Collections.Hashtable cmsOutputDatas;       //CMS��X���
        private System.Collections.Hashtable rgsOutputDatas;       //RGS��X���
        private System.Collections.Hashtable rmsOutputDatas;       //RMS��X���
        private System.Collections.Hashtable cctvOutputDatas;      //CCTV��X���
        private System.Collections.Hashtable cslsOutputDatas;      //CSLS��X���
        private System.Collections.Hashtable lcsOutputDatas;       //LCS��X���
        private System.Collections.Hashtable fsOutputDatas;        //LCS��X���
        private string memo = "";                                  //���եΦr��
        private int mode=2;                                        //�Ҧ�        
        /// <summary>
        /// �����p���s��
        /// </summary>
        public string RspID
        {
            get { return rspID; }
            set { rspID = value; }
        }

        /// <summary>
        /// �ƥ�s��
        /// </summary>
        public string EventID
        {
            get { return eventID; }
            set { eventID = value; }
        }

        /// <summary>
        /// �ƥ�o�ͩΧ��ܮɶ�
        /// </summary>
        public DateTime EventTime
        {
            get { return eventTime; }
            set { eventTime = value; }
        }

        /// <summary>
        /// ĵ�i�s��
        /// </summary>
        public int AlarmClass
        {
            get { return alarmClass; }
            set { alarmClass = value; }
        }

        /// <summary>
        /// �s�������
        /// </summary>
        public List<LiaiseUnit> Units
        {
            get { return units; }
            set { units = value; }
        }        

        /// <summary>
        ///  WIS��X���
        /// </summary>
        public System.Collections.Hashtable WISOutputData
        {
            get { return wisOutputDatas; }
            set { wisOutputDatas = value; }
        }

        /// <summary>
        ///  CMS��X���
        /// </summary>
        public System.Collections.Hashtable CMSOutputData
        {
            get { return cmsOutputDatas; }
            set { cmsOutputDatas = value; }
        }

        /// <summary>
        ///  RGS��X���
        /// </summary>
        public System.Collections.Hashtable RGSOutputData
        {
            get { return rgsOutputDatas; }
            set { rgsOutputDatas = value; }
        }

        /// <summary>
        ///  CSLS��X���
        /// </summary>
        public System.Collections.Hashtable CSLSOutputData
        {
            get { return cslsOutputDatas; }
            set { cslsOutputDatas = value; }
        }

        /// <summary>
        ///  FS��X���
        /// </summary>
        public System.Collections.Hashtable FSOutputData
        {
            get { return fsOutputDatas; }
            set { fsOutputDatas = value; }
        }

        /// <summary>
        ///  LCS��X���
        /// </summary>
        public System.Collections.Hashtable LCSOutputData
        {
            get { return lcsOutputDatas; }
            set { lcsOutputDatas = value; }
        }

        /// <summary>
        ///  CCTV��X���
        /// </summary>
        public System.Collections.Hashtable CCTVOutputData
        {
            get { return cctvOutputDatas; }
            set { cctvOutputDatas = value; }
        }

        /// <summary>
        ///  RMS��X���
        /// </summary>
        public System.Collections.Hashtable RMSOutputData
        {
            get { return rmsOutputDatas; }
            set { rmsOutputDatas = value; }
        }

        /// <summary>
        /// �Ƶ�
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
