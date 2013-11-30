using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    class Section : Location
    {
        /// <summary>
        /// 路段資料類別建構元
        /// </summary>
        /// <param name="id">路段編號</param>
        /// <param name="secName">路段名稱</param>
        /// <param name="startMileage">路段開始里程</param>
        /// <param name="endMileage">路段結束里程</param>
        public Section(string id, string secName, int startMileage, int endMileage)
        {
            this.id = id;
            this.name = secName;
            this.startMileage = startMileage;
            this.endMileage = endMileage;
        }

        public override bool compare(object sender)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool Equals(object obj)
        {
            string secid = (string)obj;
            return this.id == secid;
        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }
    }
}
