using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    /// <summary>
    /// 隧道資料類別
    /// </summary>
    [Serializable]
    class Tunnel : Location
    {
        private Line line;
        /// <summary>
        /// 隧道資料類別建構元
        /// </summary>
        /// <param name="tunnelName">本隧道編號</param>
        /// <param name="tunnelName">本隧道名稱</param>
        /// <param name="direction">本隧道方向</param>        
        /// <param name="startMileage">本隧道起點里程</param>
        /// <param name="endMileage">本隧道終點里程</param>
        /// <param name="upstreamTunnel">上游隧道</param>
        public Tunnel(string id, string tunnelName, Line line, string direction, int startMileage, int endMileage, Tunnel upstreamTunnel)
        {
            this.id = id;
            this.startMileage = startMileage;
            this.endMileage = endMileage;
            this.name = tunnelName;
            this.direction = direction;
            if (upstreamTunnel == null || !upstreamTunnel.Line.Equals(line))
            {
                this.upstreamLocation = null;
            }
            else
            {
                this.upstreamLocation = upstreamTunnel;
            }
            this.line = line;
        }

        public override bool compare(object sender)
        {
            EventObj obj = (EventObj)sender;
            if (this.line.Equals(obj.Lineid))
            {
                if (this.direction == obj.Direction)
                {
                    switch (this.direction)
                    {
                        case "E":
                        case "N":
                            if (this.startMileage <= obj.Mileage && obj.Mileage <= endMileage)
                                return true;
                            break;
                        case "W":
                        case "S":
                            if (this.startMileage >= obj.Mileage && obj.Mileage >= endMileage)
                                return true;
                            break;
                    }
                }
            }
            return false;
        }

        public Line Line
        {
            get { return line; }
        }

        public override bool Equals(object obj)
        {
            Tunnel tunnel = (Tunnel)obj;
            if (this.id == tunnel.id && this.line == tunnel.line)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }
    }

    [Serializable]
    class EventObj
    {
        string lineid;
        string direction;
        int mileage;
        public EventObj(string lineid, string direction, int mileage)
        {
            this.direction = direction;
            this.mileage = mileage;
            this.lineid = lineid;
        }
        /// <summary>
        /// 事件路線編號
        /// </summary>
        public string Lineid
        {
            get { return lineid; }
        }
        /// <summary>
        /// 事件方向
        /// </summary>
        public string Direction
        {
            get { return direction; }
        }
        /// <summary>
        /// 事件開始里程
        /// </summary>
        public int Mileage
        {
            get { return mileage; }
        }
    }
}
