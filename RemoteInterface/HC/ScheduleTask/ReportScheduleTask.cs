using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HC.ScheduleTask
{
    [Serializable]
    public class ReportScheduleTask
    {
        public int reportId;
        public ReportScheduleTask(int reportId)
        {
            this.reportId = reportId;
        }
    }
}
