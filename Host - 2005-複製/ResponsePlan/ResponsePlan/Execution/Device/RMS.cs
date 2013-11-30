using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    /// <summary>
    /// 杆耿ンRMS
    /// </summary>
    internal class RMS : ADevice 
    {
        RMSDeviceList rmslist;
        public RMS(AEvent aDevice, CategoryType type, System.Collections.Hashtable ht, DeviceType devType, System.Collections.Hashtable DevRange, Degree degree)
        {
            Initialize(aDevice, type, ht, devType, DevRange, degree);
            rmslist = new RMSDeviceList();
            this.GetMessRuleData += new GetMessRuleDataHandler(RMS_GetMessRuleData);
        }

        public override ExecutionObj produceExecution(ExecutionObj sender)
        {
            sender.Memo += "==>> RMS";
            sender.RMSOutputData = getDisplayContent(type);
            aDevice.produceExecution(sender);
            return sender;
        }

        protected override System.Collections.Hashtable setDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId, MegType megType)
        {
            System.Collections.Hashtable displayht = new System.Collections.Hashtable();

            List<object> outputs = new List<object>();
            if (devNames == null) return displayht;

            com.select(DBConnect.DataType.RMS, Command.GetSelectCmd.getRMSMode());

            foreach (RemoteInterface.HC.FetchDeviceData devName in devNames)
            {
                RMSData data = rmslist.Find(devName.DevName);
                RMSMode mode = new RMSMode(65535, 30);
                if (data != null)
                {
                    switch (this.degree)
                    {
                        case Degree.L:
                            mode = data.Mode_L;
                            break;
                        case Degree.M:
                            mode = data.Mode_M;
                            break;
                        case Degree.H:
                            mode = data.Mode_H;
                            break;
                        case Degree.S:
                            mode = data.Mode_S;
                            break;
                        default:
                            break;
                    }
                }
                List<object> output = new List<object>();
                output.AddRange(new object[] { this.getPriority(), new RemoteInterface.HC.RMSOutputData(mode.Mode, mode.PlanNo) });
                displayht.Add(devName.DevName, output);
            }
            return displayht;      
        }

        object RMS_GetMessRuleData(DBConnect.DataType type, object reader)
        {
            if (type == DBConnect.DataType.RMS)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                RMSData data = new RMSData(dr[0].ToString(), Convert.ToInt32(dr[1]), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), 
                                           Convert.ToInt32(dr[5]), Convert.ToInt32(dr[6]), Convert.ToInt32(dr[7]), Convert.ToInt32(dr[8]), 
                                           Convert.ToInt32(dr[9]), Convert.ToInt32(dr[10]), Convert.ToInt32(dr[11]), Convert.ToInt32(dr[12]));
                rmslist.Add(data);
                return null;
            }
            else
            {
                return null;
            }
        }
    }

    abstract class DeviceData
    {
        protected string devName = "";
        protected int mileage = 0;
        protected string lineId;
        protected string secId;
        protected string direction;

        public string DeviceName
        {
            get { return devName; }
        }

        public int Mileage
        {
            get { return mileage; }
        }

        public string LineId
        {
            get { return lineId; }
        }

        public string SectionId
        {
            get { return secId; }
        }

        public string Direction
        {
            get { return direction; }
        }
    }

    class RMSData : DeviceData
    {
        RMSMode mode_L;
        RMSMode mode_M;
        RMSMode mode_H;
        RMSMode mode_S;
        public RMSData(string devName, int mileage, string lineId, string secId, string direction, int mode_L, int planNo_L, int mode_M, int planNo_M, int mode_H, int planNo_H, int mode_S, int planNo_S)
        {
            this.devName = devName;
            this.mileage = mileage;
            this.lineId = lineId;
            this.secId = secId;
            this.direction = direction;
            this.mode_L = new RMSMode(mode_L, planNo_L);
            this.mode_M = new RMSMode(mode_M, planNo_M);
            this.mode_H = new RMSMode(mode_H, planNo_H);
            this.mode_S = new RMSMode(mode_S, planNo_S);
        }

        /// <summary>
        /// 单家Α
        /// </summary>
        public RMSMode Mode_L
        {
            get { return mode_L; }
        }
        /// <summary>
        /// 单い家Α
        /// </summary>
        public RMSMode Mode_M
        {
            get { return mode_M; }
        }
        /// <summary>
        /// 单蔼家Α
        /// </summary>
        public RMSMode Mode_H
        {
            get { return mode_H; }
        }
        /// <summary>
        /// 单禬蔼家Α
        /// </summary>
        public RMSMode Mode_S
        {
            get { return mode_S; }
        }

        public override bool Equals(object obj)
        {
            return this.devName == (string)obj;
        }

        public override int GetHashCode()
        {
            return this.devName.GetHashCode();
        }

        public override string ToString()
        {
            return this.devName;
        }
    }

    /// <summary>
    /// RMS家Αン
    /// </summary>
    class RMSMode
    {
        int mode = 0;
        int planNo = 0;
        public RMSMode(int mode, int planNo)
        {
            this.mode = mode;
            this.planNo = planNo;
        }

        /// <summary>
        /// RMS家Α
        /// </summary>
        public int Mode
        {
            get { return mode; }
        }
        /// <summary>
        /// RSM磅
        /// </summary>
        public int PlanNo
        {
            get { return planNo; }
        }
    }

    class RMSDeviceList : MyList<RMSData>
    {
        public override RMSData Find(object sender)
        {
            string devName = (string)sender;
            foreach (RMSData rms in this.getList())
            {
                if (rms.Equals(devName)) return rms;
            }
            return null;
        }
    }
}
