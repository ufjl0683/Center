using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.TEM;
namespace Host.Event.TEM
{
  public   class TemManager
    {
      System.Collections.Hashtable hsTemRanges = new System.Collections.Hashtable();
      public TemManager()
      {

        

      }



     public  void setEventData(string devName, object eventdata)
      {
          string key = "";
          Host.Event.Event evt;
          if(eventdata is  RemoteInterface.TEM.AirAlarmData)
              key=( eventdata as AirAlarmData).Key;
          else if (eventdata is RemoteInterface.TEM.FireAlarmData)
                 key=( eventdata as FireAlarmData).Key;
          else if (eventdata is RemoteInterface.TEM.LightAlarmData)
                 key=( eventdata as LightAlarmData).Key;
          else if (eventdata is RemoteInterface.TEM.PowerAlarmData)
                 key = (eventdata as PowerAlarmData).Key;
          else if (eventdata is RemoteInterface.TEM.SecurityAlarmData)
                 key = key = (eventdata as SecurityAlarmData).Key;
          else if (eventdata is RemoteInterface.TEM.MonitorAlarmData)
                key = key = (eventdata as MonitorAlarmData).Key;

             if (key == "")
                 return;

             TC.TEMDeviceWrapper dev = Program.matrix.device_mgr[devName] as TC.TEMDeviceWrapper;

          switch (dev.setTemEventData(eventdata))
          {
              case EventTransition.Begin:
                  evt=new Host.Event.TEM.TemRangeData(dev,eventdata);
                  hsTemRanges.Add(key,evt );
                  Program.matrix.event_mgr.AddEvent(evt);
                  break;

              case EventTransition.End:
                  evt = hsTemRanges[key] as Host.Event.Event;
                  evt.invokeStop();
                  hsTemRanges.Remove(key);
                  break;
              case EventTransition.LevelChange:

                  evt = hsTemRanges[key] as Host.Event.Event;
                  evt.invokeDegreeChange();

                  break;

              case EventTransition.NoChange:
                  break;
              case EventTransition.RangeChange:
                  evt = hsTemRanges[key] as Host.Event.Event;
                  evt.invokeRangeChange();
                  break;
          }



      }

      //else if(eventData is RemoteInterface.TEM.FireAlarmData)
      //else if(eventData is RemoteInterface.TEM.LightAlarmData)
      //else if(eventData is RemoteInterface.TEM.PowerAlarmData)
      //else if(eventData is RemoteInterface.TEM.SecurityAlarmData)
     

    }
}
