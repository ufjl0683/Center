using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.TEM;

namespace Host.TC
{

   
  public   class TEMDeviceWrapper: DeviceBaseWrapper
    {

       System.Collections.Hashtable hsTemEventData=System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
      public TEMDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction)
          : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus, direction)
      {
      }


      public Event.EventTransition getTransition(int oldval, int newval)
      {

          if (oldval == newval)
              return Event.EventTransition.NoChange;
          else  // not equal
          {
              if (oldval == 0)
                  return Event.EventTransition.Begin;

              else if (newval == 0)
                  return Event.EventTransition.End;

              else

                  return Event.EventTransition.RangeChange;

              
          }

      }
      public Event.EventTransition setTemEventData(object eventData)
      {
          Event.EventTransition ret;
          string key = "";

          if (eventData is AirAlarmData)
          {
              AirAlarmData newdata = eventData as AirAlarmData;
              AirAlarmData olddata;
              key = newdata.Key;
              if (hsTemEventData.Contains(key))
              {
                  olddata = hsTemEventData[key] as AirAlarmData;
                  

                  ret= getTransition(olddata.level,newdata.level);

                 

              }
              else  // not contain
              {
                  this.hsTemEventData.Add(key, newdata);
                  if (newdata.level > 0)
                      ret= Host.Event.EventTransition.Begin;
                  else
                      ret= Host.Event.EventTransition.NoChange;
              }
              
          }
          else if (eventData is SecurityAlarmData)
          {
              SecurityAlarmData newdata = eventData as SecurityAlarmData;
              SecurityAlarmData olddata;
              key = newdata.Key;
              if (hsTemEventData.Contains(key))
              {
                  olddata = hsTemEventData[key] as SecurityAlarmData;

                  ret= getTransition(olddata.status, newdata.status);


              }
              else  // not contain
              {
                  this.hsTemEventData.Add(key, newdata);
                  if (newdata.status > 0)
                      ret= Host.Event.EventTransition.Begin;
                  else
                      ret= Host.Event.EventTransition.NoChange;
              }

            
          }
          else if (eventData is RemoteInterface.TEM.FireAlarmData)
          {
              FireAlarmData newdata = eventData as FireAlarmData;
              FireAlarmData olddata;
              key = newdata.Key;
              if (hsTemEventData.Contains(key))
              {
                  olddata = hsTemEventData[key] as FireAlarmData;

                  ret= getTransition(olddata.status, newdata.status);


              }
              else  // not contain
              {
                  this.hsTemEventData.Add(key, newdata);
                  if (newdata.status > 0)
                      ret= Host.Event.EventTransition.Begin;
                  else
                      ret= Host.Event.EventTransition.NoChange;
              }

            
          }
          else if (eventData is RemoteInterface.TEM.LightAlarmData)
          {
              LightAlarmData newdata = eventData as LightAlarmData;
              LightAlarmData olddata;
              key = newdata.Key;
              if (hsTemEventData.Contains(key))
              {
                  olddata = hsTemEventData[key] as LightAlarmData;

                  ret= getTransition(olddata.damaged, newdata.damaged);


              }
              else  // not contain
              {
                  this.hsTemEventData.Add(key, newdata);
                  if (newdata.damaged > 0)
                      ret= Host.Event.EventTransition.Begin;
                  else
                      ret= Host.Event.EventTransition.NoChange;
              }

              
          }
          else if (eventData is RemoteInterface.TEM.PowerAlarmData)
          {
              PowerAlarmData newdata = eventData as PowerAlarmData;
              PowerAlarmData olddata;
              key = newdata.Key;
              if (hsTemEventData.Contains(key))
              {
                  olddata = hsTemEventData[key] as PowerAlarmData;

                  ret= getTransition(olddata.status, newdata.status);


              }
              else  // not contain
              {
                  this.hsTemEventData.Add(key, newdata);
                  if (newdata.status > 0)
                      ret= Host.Event.EventTransition.Begin;
                  else
                      ret= Host.Event.EventTransition.NoChange;
              }

            
          }
          else
              ret= Event.EventTransition.NoChange;

          if (key != "")
          {
              hsTemEventData.Remove(key);
              hsTemEventData.Add(key, eventData);
          }
         //else if(eventData is RemoteInterface.TEM.FireAlarmData)
         //else if(eventData is RemoteInterface.TEM.LightAlarmData)
         //else if(eventData is RemoteInterface.TEM.PowerAlarmData)
         //else if(eventData is RemoteInterface.TEM.SecurityAlarmData)
          return ret;

      }
    }
}
