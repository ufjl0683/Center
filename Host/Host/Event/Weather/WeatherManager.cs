using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace Host.Event.Weather
{
    public  class WeatherManager
    {

        System.Collections.Hashtable hsEvent = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
       
        public WeatherManager()
        {

            loadDevice();

        }

        public int GetEventCnt()
        {


            return this.hsEvent.Count;
        }
        void loadDevice()
        {
            System.Collections.IEnumerator ie = Program.matrix.device_mgr.getDataDeviceEnum().GetEnumerator();
            while (ie.MoveNext())
            {
                try
                {
                    if (ie.Current is TC.RDDeviceWrapper)
                    {
                        TC.RDDeviceWrapper rd = ie.Current as TC.RDDeviceWrapper;
                        rd.OnEvent += new EventHandler(Weather_OnEvent);
                    }
                    else if (ie.Current is TC.VIDeviceWrapper)
                    {
                        TC.VIDeviceWrapper vi = ie.Current as TC.VIDeviceWrapper;
                        vi.OnEvent += new EventHandler(Weather_OnEvent);
                    }
                    else if (ie.Current is TC.WDDeviceWrapper)
                    {
                        TC.WDDeviceWrapper wd = ie.Current as TC.WDDeviceWrapper;
                        wd.OnEvent += new EventHandler(Weather_OnEvent);
                    }
                    else if (ie.Current is TC.LSDeviceWrapper)
                    {
                        TC.LSDeviceWrapper ls = ie.Current as TC.LSDeviceWrapper;
                        ls.OnEvent += new EventHandler(Weather_OnEvent);
                    }
                    else if (ie.Current is TC.BSDeviceWrapper)
                    {
                        TC.BSDeviceWrapper bs = ie.Current as TC.BSDeviceWrapper;
                        bs.OnEvent += new EventHandler(Weather_OnEvent);
                    }
                }
                catch (Exception ex)
                {
                    RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                }
            }
        }

        void Weather_OnEvent(object sender, EventArgs e)
        {


            try
            {
                TC.DeviceBaseWrapper dev = sender as TC.DeviceBaseWrapper;


                if (!hsEvent.Contains(dev.deviceName))
                {
                    if (dev.event_degree == 0 || dev.event_degree == -1)
                        return;

                    Weather.WeatherRange range;
                    if (dev is TC.RDDeviceWrapper)

                        range = new WeatherRange(dev, AlarmType.RD);
                    else if (dev is TC.VIDeviceWrapper)
                        range = new WeatherRange(dev, AlarmType.VI);
                    else if (dev is TC.WDDeviceWrapper)
                        range = new WeatherRange(dev, AlarmType.WD);
                    else if (dev is TC.BSDeviceWrapper)
                        range = new WeatherRange(dev, AlarmType.BS);
                    else
                        range = new WeatherRange(dev, AlarmType.LS);
                    

                    hsEvent.Add(dev.deviceName, range);
                   // range.OnAbort += new EventHandler(range_OnAbort);
                    range.OnAbortToManager += new EventHandler(range_OnAbortToManager);
                    Program.matrix.event_mgr.AddEvent(range);


                }
                else   // 已經存在事件
                {

                    Host.Event.Event evt = (Event)hsEvent[dev.deviceName];

                    if (dev.event_degree == 0 || dev.event_degree == -1)
                    {
                        evt.invokeStop();
                        hsEvent.Remove(dev.deviceName);
                    }
                    else
                    {
                        evt.invokeDegreeChange();

                    }




                }

            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }
            


        }

        void range_OnAbortToManager(object sender, EventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
            WeatherRange evt = sender as WeatherRange;
            evt.OnAbort -= new EventHandler(range_OnAbortToManager);
            hsEvent.Remove(evt.DeviceName);
        }

       
    }
}
