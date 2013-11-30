using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    class Section : Location
    {
        /// <summary>
        /// ���q������O�غc��
        /// </summary>
        /// <param name="id">���q�s��</param>
        /// <param name="secName">���q�W��</param>
        /// <param name="startMileage">���q�}�l���{</param>
        /// <param name="endMileage">���q�������{</param>
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
