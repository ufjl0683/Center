using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    /// <summary>
    /// 壅塞事件類別
    /// </summary>
    internal class ObsEvent : AEvent
    {
        public ObsEvent(System.Collections.Hashtable ht, CategoryType type)
        {
            Initialize(ht, type);
        }
    }
}
