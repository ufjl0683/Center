using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    /// <summary>
    /// 路線資料類別
    /// </summary>
    [Serializable]
    class Line : Location
    {
        /// <summary>
        /// 路線資料類別建構元
        /// </summary>
        /// <param name="id">路線編號</param>
        /// <param name="lineName">路線名稱</param>
        /// <param name="startMileage">路線開始里程</param>
        /// <param name="endMileage">路線結束里程</param>
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
