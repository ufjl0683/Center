using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    public interface I_Event
    {
        void Confirm(string sender);
        void TestConnect();
    }
}
