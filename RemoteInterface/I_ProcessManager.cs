using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace RemoteInterface
{
  public   interface I_ProcessManager
    {
      DataTable getProcessStatus();

      void setDateTime(System.DateTime dt);
      bool IsAllProcessOk();
      void SetProcessRunningState(string pName, int state);
    }
}
