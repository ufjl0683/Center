using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    [Serializable]
    public class MoveData
    {
        private int eventid;//�ƥ�s��
        private string notifier;//�q���H���W��
        private DateTime time;//�ƥ�n�J�ɶ�
        private string lineId;//���u�s��
        private string direction;//���u��V
        private int startMileage; //�}�l���{
        private int endMileage; //�������{
        private string blockLane;//���_���D

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
        /// �ƥ�s��
        /// </summary>
        public int Eventid
        {
            get { return eventid; }
            set { eventid = value; }
        }
        /// <summary>
        /// �q���H���W��
        /// </summary>
        public string Notifier
        {
            get { return notifier; }
            set { notifier = value; }
        }
        /// <summary>
        /// �ƥ�n�J�ɶ�
        /// </summary>
        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
        /// <summary>
        /// ���u�s��
        /// </summary>
        public string LineId
        {
            get { return lineId; }
            set { lineId = value; }
        }
        /// <summary>
        /// ���u��V
        /// </summary>
        public string Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        /// <summary>
        /// �}�l���{
        /// </summary>
        public int StartMileage
        {
            get { return startMileage; }
            set { startMileage = value; }
        }
        /// <summary>
        /// �������{
        /// </summary>
        public int EndMileage
        {
            get { return endMileage; }
            set { endMileage = value; }
        }
        /// <summary>
        /// ���_���D
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
