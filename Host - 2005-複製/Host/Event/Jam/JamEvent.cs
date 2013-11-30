using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.Jam
{
  public   class JamEvent:Event
    {

      JamRange jamRange;
        public  JamEvent(long eventid,JamRange jamrange,EventMode mode):base(eventid,mode)
        {
            this.jamRange = jamrange;
        }
    }
}
