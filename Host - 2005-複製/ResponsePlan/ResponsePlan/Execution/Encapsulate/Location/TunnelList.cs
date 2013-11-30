using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    class TunnelList : MyList<Tunnel>
    {
        public override Tunnel Find(object sender)
        {
            EventObj obj = (EventObj)sender;
            foreach (Tunnel t in getList())
            {
                if (t.compare(obj)) return t;
            }
            return null;
        }
    }
}
