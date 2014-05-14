using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    class LtrEvent : AEvent
    {
        internal LtrEvent(System.Collections.Hashtable ht, CategoryType type)
        {
            Initialize(ht, type);
        }

        protected override AEvent factory()
        {
            AEvent myObj = this;
            List<object> Decorators = new List<object>();
            Degree degree = getDegree();
            System.Collections.Hashtable HT = new System.Collections.Hashtable();
            Decorators.Clear();
            List<DeviceType> adorns = new List<DeviceType>();
            return getAdorns(adorns, myObj, Decorators, degree);
        }
    }
}
