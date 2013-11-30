using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.RampControl
{
     public  class RampControl_Manager
    {

         System.Collections.Generic.Dictionary<string, RampControlRange> hsEvents = new Dictionary<string, RampControlRange>();
         public RampControl_Manager()
         {



         }

         public int GetEventCnt()
         {


             return this.hsEvents.Count;
         }
         public  void  set_start(TC.RMSDeviceWrapper dev)
         {
             if (hsEvents.ContainsKey(dev.deviceName))
                 return;
             //非反應計畫
             if ((Program.matrix.getDeviceWrapper(dev.deviceName) as TC.OutPutDeviceBase).getOutputdata().mode == RemoteInterface.HC.OutputModeEnum.ResponsePlanMode)
                 return;


             RampControlRange evt= new RampControlRange(dev);
             hsEvents.Add(dev.deviceName,evt);
             Program.matrix.event_mgr.AddEvent(evt);

         }

         public void setRampControl_stop(TC.RMSDeviceWrapper dev)
         {
             if (!hsEvents.ContainsKey(dev.deviceName))
                 return;

             hsEvents[dev.deviceName].invokeStop();
            
             hsEvents.Remove(dev.deviceName);
          
             
         }


    }
}
