using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    /// <summary>
    /// �G�D���q�ƥ����O
    /// </summary>
    internal class TunEvent : AEvent
    {
        public TunEvent(System.Collections.Hashtable ht, CategoryType type)
        {
            Initialize(ht, type);
        }
    }
}
