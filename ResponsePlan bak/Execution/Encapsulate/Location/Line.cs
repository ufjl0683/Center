using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    /// <summary>
    /// ���u������O
    /// </summary>
    [Serializable]
    class Line : Location
    {
        /// <summary>
        /// ���u������O�غc��
        /// </summary>
        /// <param name="id">���u�s��</param>
        /// <param name="lineName">���u�W��</param>
        /// <param name="startMileage">���u�}�l���{</param>
        /// <param name="endMileage">���u�������{</param>
        public Line(string id, string lineName, int startMileage, int endMileage)
        {
            this.id = id;
            this.name = lineName;
            this.startMileage = startMileage;
            this.endMileage = endMileage;
        }

        public override bool compare(object sender)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool Equals(object obj)
        {
            string lineid = obj.ToString();
            return this.id == lineid;
        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }
    }
}
