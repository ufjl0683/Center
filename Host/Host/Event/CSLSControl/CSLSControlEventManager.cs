using System;
using System.Collections.Generic;
using System.Text;
using Host.TC;
using RemoteInterface;
using RemoteInterface.HC;

namespace Host.Event.CSLSControl
{
  public  class CSLSControlEventManager
    {
       // System.Collections.Generic.Dictionary<string,VDDeviceWrapper> dictVD=new Dictionary<string,VDDeviceWrapper>();
        System.Collections.Generic.Dictionary<string, CSLSControlRange> dictEvent = new Dictionary<string, CSLSControlRange>();
        public CSLSControlEventManager(DevcieManager devMgr)
        {
            loadMainLineVD(devMgr);
        }

        public int GetEventCnt()
        {
            return this.dictEvent.Count;
        }
        private void loadMainLineVD(DevcieManager devMgr)
        {
            foreach (DeviceBaseWrapper dev in devMgr.getDataDeviceEnum())
            {
                try
                {
                    if (dev is VDDeviceWrapper)
                    {
                        VDDeviceWrapper vddev = dev as VDDeviceWrapper;
                        if (dev.location == "F" || dev.location == "H" || dev.location == "T")
                        {
                            //  dictVD.Add(dev.deviceName,  vddev);
                            if (!CanTriggerCSLSEvent(vddev))
                                continue;
                            vddev.OnCSLSControlEvent += new CSLSControlEventHandler(vddev_OnCSLSControlEvent);

                        }
                    }
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                }
               
            }
            ConsoleServer.WriteLine("速限管理啟動完成!");
        }


        bool CanTriggerCSLSEvent(VDDeviceWrapper vddev)
        {
            int fecthstart, fetchend;
            if (vddev.direction == "S" || vddev.direction == "E")
            {
                fecthstart = vddev.start_mileage - 2000;
                fetchend = vddev.start_mileage;
            }
            else
            {
                fecthstart = vddev.start_mileage + 2000;
                fetchend = vddev.start_mileage;

            }
            FetchDeviceData[] fetc_devices = Program.matrix.output_device_fetch_mgr.Fetch(new string[] { "CSLS","MAS" },
                   vddev.lineid, vddev.direction, fecthstart, fetchend);
            if (fetc_devices == null || fetc_devices.Length == 0)
                return false;
            else
                return true;
        }
        void vddev_OnCSLSControlEvent(object sender, int level)
        {
            //throw new NotImplementedException();
            try
            {
                VDDeviceWrapper vddev = sender as VDDeviceWrapper;
                if (!this.dictEvent.ContainsKey(vddev.deviceName))
                {
                    if (level > 0)  //決定是否啟動事件
                    {
                       
                        // add event here
                        CSLSControlRange range = new CSLSControlRange(vddev);
                        range.OnAbortToManager += new EventHandler(range_OnAbortToManager);
                        dictEvent.Add(vddev.deviceName, range);
                        Program.matrix.event_mgr.AddEvent(range);
                    }
                    



                }
                else
                {
                    if (level <= 0)
                    {
                        //close event here
                        CSLSControlRange range = dictEvent[vddev.deviceName];
                        range.invokeStop();
                        dictEvent.Remove(vddev.deviceName);
                    }
                    else
                    {
                        // new range change here
                        CSLSControlRange range = dictEvent[vddev.deviceName];
                        range.invokeDegreeChange();
                    }

                }
            }

           
        catch(Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }

        }

        void range_OnAbortToManager(object sender, EventArgs e)
        {
                //throw new NotImplementedException();
            CSLSControlRange evt = sender as CSLSControlRange;
            evt.OnAbort -= new EventHandler(range_OnAbortToManager);
            dictEvent.Remove(evt.DeviceName);
        }

      
    }
}
