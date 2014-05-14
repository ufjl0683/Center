using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    [Serializable]
    public class MoveData
    {
        private int eventid;//事件編號
        private string notifier;//通報人員名稱
        private DateTime time;//事件登入時間
        private string lineId;//路線編號
        private string direction;//路線方向
        private int startMileage; //開始里程
        private int endMileage; //結束里程
        private string blockLane;//阻斷車道

        public MoveData(int eventid, string notifier, DateTime time, string lineId, string direction, int startMileage, int endMileage, string blockLane)
        {
            this.eventid = eventid;
            this.notifier = notifier;
            this.time = time;
            this.lineId = lineId;
            this.direction = direction;
            this.startMileage = startMileage;
            this.endMileage = endMileage;
            this.blockLane = blockLane;
        }

        /// <summary>
        /// 事件編號
        /// </summary>
        public int Eventid
        {
            get { return eventid; }
            set { eventid = value; }
        }
        /// <summary>
        /// 通報人員名稱
        /// </summary>
        public string Notifier
        {
            get { return notifier; }
            set { notifier = value; }
        }
        /// <summary>
        /// 事件登入時間
        /// </summary>
        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
        /// <summary>
        /// 路線編號
        /// </summary>
        public string LineId
        {
            get { return lineId; }
            set { lineId = value; }
        }
        /// <summary>
        /// 路線方向
        /// </summary>
        public string Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        /// <summary>
        /// 開始里程
        /// </summary>
        public int StartMileage
        {
            get { return startMileage; }
            set { startMileage = value; }
        }
        /// <summary>
        /// 結束里程
        /// </summary>
        public int EndMileage
        {
            get { return endMileage; }
            set { endMileage = value; }
        }
        /// <summary>
        /// 阻斷車道
        /// </summary>
        public string BlockLane
        {
            get { return blockLane; }
            set { blockLane = value; }
        }

        public override string ToString()
        {
            return this.eventid.ToString();
        }

        public override bool Equals(object obj)
        {
            MoveData data = (MoveData)obj;          
            return (data.eventid==this.eventid);
        }

        public override int GetHashCode()
        {
            return this.eventid.GetHashCode();
        }
    }
}
