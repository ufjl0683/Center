using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    /// <summary>
    /// �G�D������O
    /// </summary>
    [Serializable]
    class Tunnel : Location
    {
        private Line line;
        /// <summary>
        /// �G�D������O�غc��
        /// </summary>
        /// <param name="tunnelName">���G�D�s��</param>
        /// <param name="tunnelName">���G�D�W��</param>
        /// <param name="direction">���G�D��V</param>        
        /// <param name="startMileage">���G�D�_�I���{</param>
        /// <param name="endMileage">���G�D���I���{</param>
        /// <param name="upstreamTunnel">�W���G�D</param>
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
        /// �ƥ���u�s��
        /// </summary>
        public string Lineid
        {
            get { return lineid; }
        }
        /// <summary>
        /// �ƥ��V
        /// </summary>
        public string Direction
        {
            get { return direction; }
        }
        /// <summary>
        /// �ƥ�}�l���{
        /// </summary>
        public int Mileage
        {
            get { return mileage; }
        }
    }
}
