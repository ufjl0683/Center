using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HC.ScheduleTask
{

    [Serializable]
    public class SetTravelModeTask
    {
        public bool  Enable;
      
        public SetTravelModeTask(bool benable)
        {
            this.Enable = benable;
           
        }


    }
}
