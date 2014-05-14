using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    /// <summary>
    /// 天候事件類別
    /// </summary>
    internal class WeaEvent : AEvent
    {
        public WeaEvent(System.Collections.Hashtable ht, CategoryType type)
        {
            Initialize(ht, type);
        }
    }
}
