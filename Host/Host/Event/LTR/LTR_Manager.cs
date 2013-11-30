using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.LTR
{
     public  class LTR_Manager
    {

         System.Collections.Generic.Dictionary<string, LTR_Range> hsEvents = new Dictionary<string, LTR_Range>();
         public LTR_Manager()
         {



         }

         public int GetEventCnt()
         {


             return this.hsEvents.Count;
         }

         public  void  setLTR_start(TC.RMSDeviceWrapper dev)
         {
             if (hsEvents.ContainsKey(dev.deviceName))
                 return;

             LTR_Range evt= new LTR_Range(dev);
             hsEvents.Add(dev.deviceName,evt);
             Program.matrix.event_mgr.AddEvent(evt);

         }

         public void setLTR_stop(TC.RMSDeviceWrapper dev)
         {
             if (!hsEvents.ContainsKey(dev.deviceName))
                 return;

             hsEvents[dev.deviceName].invokeStop();
            
             hsEvents.Remove(dev.deviceName);
          
             
         }


    }
}
