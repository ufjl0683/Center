using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    /// <summary>
    /// �ѭԨƥ����O
    /// </summary>
    internal class WeaEvent : AEvent
    {
        public WeaEvent(System.Collections.Hashtable ht, CategoryType type)
        {
            Initialize(ht, type);
        }
    }
}
