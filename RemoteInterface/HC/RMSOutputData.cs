using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HC
{
    [Serializable]
    public class RMSOutputData
    {
        public int mode, planno;
        public bool IsChangeDownStreamCapacity = false;
        public int newCapDown=0, oldCapDown = 0;
        public int main_occupy_threshold,  max_rms_rate,  min_rms_rate,  ramp_threshold,  ramp_termination_count_threshold;
        public RMSOutputData(int mode, int planno)
        {
            this.mode = mode;
            this.planno = planno;
        }
        public RMSOutputData(int mode, int planno, int newCapDown, int oldCapDown, int main_occupy_threshold, int max_rms_rate, int min_rms_rate, int ramp_threshold, int ramp_termination_count_threshold)
        {

            this.IsChangeDownStreamCapacity = true;
            this.newCapDown = newCapDown;
            this.oldCapDown = oldCapDown;
            this.main_occupy_threshold = main_occupy_threshold;
            this.max_rms_rate = max_rms_rate;
            this.min_rms_rate = min_rms_rate;
            this.ramp_termination_count_threshold = ramp_termination_count_threshold;
            this.ramp_threshold = ramp_threshold;
        }
    }
}
