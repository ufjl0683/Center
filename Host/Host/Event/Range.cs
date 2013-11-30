using System;
using System.Collections.Generic;
using System.Text;
using Host.TC;
using System.Collections;

namespace Host.Event
{
    public  abstract class Range:Event
    {
      protected  ArrayList devlist = ArrayList.Synchronized(new ArrayList());
        
        public Range(DeviceBaseWrapper dev):base()
        {
            this.devlist.Add(dev);
        }


        public virtual bool IsInRange(DeviceBaseWrapper dev)
        {
            return dev.mile_m>= StartMile && dev.mile_m < EndMile;
        }
        public string LineId
        {
            get
            {
                return ((DeviceBaseWrapper)devlist[0]).lineid;
            }
        }


        public string Direction
        {
            get
            {
                return ((DeviceBaseWrapper)devlist[0]).direction;
            }
        }


        public virtual int StartMile
        {
            get
            {

                throw new System.Exception("StartMile Must be Overwrite!");
                ////return 0;
              // return ((VDDeviceWrapper)devlist[0]).mile_m;
            }
        }


        public virtual int EndMile
        {
            get
            {
               // return ((VDDeviceWrapper)devlist[devlist.Count - 1]).mile_m;
                throw new System.Exception("EndMile Must be Overwrite!");
            }
        }

        public int StartIndex
        {
            get
            {
                return (devlist[0] as DeviceBaseWrapper).AryInx;
            }
        }


        public int EndIndex
        {
            get
            {
                return ((VDDeviceWrapper)devlist[devlist.Count - 1]).AryInx;
            }
        }
    }
}
