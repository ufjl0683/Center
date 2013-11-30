using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HC.ScheduleTask
{
   

    [Serializable]
    public   class VDRealTime  //現點速率
    {
        public int[] laneids;
       public  int cycle, durationMin;
        public VDRealTime(int[] laneids, int cycle, int durationMin)
        {
            this.laneids = laneids;
            this.cycle = cycle;
            this.durationMin = durationMin;
        }

    }
    [Serializable]
    public class VDTrigger   //即時觸動
    {

      public  int []landids;
      public   int[] occ_time_limit;

        public VDTrigger(int[] landids, int[] occ_time_limit)
        {
            this.landids = landids;
            this.occ_time_limit = occ_time_limit;
        }

    }
}
