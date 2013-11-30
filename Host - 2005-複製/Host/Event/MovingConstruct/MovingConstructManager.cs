using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.MovingConstruct
{
         public  class MovingConstructManager
        {

             System.Collections.Hashtable hsMovingEvent = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
             public MovingConstructManager()
             {

             }

            public void setEvent(int id, string notifier, DateTime timeStamp, string lineID, string directionID, int startMileage, int endMileage, int blockTypeId, string blocklane, string description)
             {

                 if (!hsMovingEvent.Contains(id))
                 {
                     MovingConstructRange evt = new MovingConstructRange(id, notifier, timeStamp, lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, description);
                     this.hsMovingEvent.Add(id,evt);
                     Program.matrix.event_mgr.AddEvent(evt);

                 }
                 else
                 {

                     MovingConstructRange evt = hsMovingEvent[id] as MovingConstructRange;
                     evt.setStartMileage(startMileage);
                     evt.setEndMileage(endMileage);
                     evt.invokeRangeChange();
                    // ((MovingConstructRange)).invokeRangeChange();
                     
                 }

             }



             public void CloseMovingConstructEvent(int id)
             {
                 if (!this.hsMovingEvent.Contains(id))
                     return;
                 MovingConstructRange evt = hsMovingEvent[id] as MovingConstructRange;
                 evt.invokeStop();
               //  Program.matrix.event_mgr.
                 hsMovingEvent.Remove(id);

             }


            


        }
}
