using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;

namespace RemoteInterface.HC
{
    public enum OutputModeEnum
    {
         ForceManualMode=100,
         ResponsePlanMode=50,
         BelowResponseMode=40,
        
         ScheduleMode = 30,
         ManualMode = 20,
         WeatherMode=15,
         UnderWeatherMode=12,
         TravelMode=10
    }
    [Serializable]
  public   class OutputQueueData:IComparable,System.Runtime.Serialization.ISerializable
    {
       
        public static int MANUAL_RULE_ID = -1;

        public static int SCHEDULE_RULE_ID = -30;
        public static int NORMAL_MANUAL_RULE_ID = -40;
      
        public static int WEATHER_RULE_ID = -45;
        public static int TRAVEL_RULE_ID = -50;
        
        public static int MANUAL_PRIORITY = 1000;

        public static int SCHEDULE_PRIORITY = -30;
        public static int NORMAL_MANUAL_PRIORITY = -40;
     
        public static int WEATHER_PRIORITY = -45;
        public static int UMDER_WEATHER_PRIORITY = -47;
        public static int TRAVEL_PRIORITY = -50;
       
        public   int ruleid;
        public   int priority;
        public    object data;
        public OutputModeEnum mode;
        private  string  _devName="";
        private int _status = -1;  //未執行
        private bool _IsSuccess = true;

        public string HappenLineID="";
        public string HappenDir="";
        public int HappenMileage=0;
        public string DevLineId="";
        public string DevDir="";
        public int DevMileage=0;
        public event EventHandler OnStatusChange;
      //  public event EventHandler OnStatusChange;
      public OutputQueueData(string devName,OutputModeEnum mode, int ruleid, int priority, object displaydata)
        {

            this.mode = mode;
            this.ruleid = ruleid;
            this.priority = priority;
            this.data = displaydata;
            this.DeviceName = devName;
        }


      private OutputQueueData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      {
          try
          {
              ruleid = info.GetInt32("ruleid");
              priority = info.GetInt32("priority");
              mode = (OutputModeEnum)info.GetValue("mode", typeof(OutputModeEnum));
           
              _IsSuccess = info.GetBoolean("_IsSuccess");
              _devName = info.GetString("_devName");
              _status = info.GetInt32("_status");

              data = info.GetValue("data", typeof(object));
          }
          catch (Exception ex)
          {
              Console.WriteLine(ex.Message + "," + ex.StackTrace);
          }
      }
      void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      {
          try
          {
              info.AddValue("ruleid", ruleid);
              info.AddValue("priority", priority);
              info.AddValue("mode", mode);
              info.AddValue("data", data);
              //info.AddValue("HappenLineID", HappenLineID);
              //info.AddValue("HappenDir", HappenDir);
              //info.AddValue("HappenMileage", HappenMileage);
              //info.AddValue("DevLineId", DevLineId);

             // info.AddValue("DevDir", DevDir);

             // info.AddValue("DevMileage", DevMileage);
              info.AddValue("_devName", _devName);
              info.AddValue("_IsSuccess", _IsSuccess);
              info.AddValue("_status", _status);
          }
          catch (Exception ex)
          {
              Console.WriteLine(ex.Message + "," + ex.StackTrace);
          }

      }

      public int status
      {
          get{
              return _status;
            }
          set
          {

              if (_status != value)
              {
                  _status = value;

                  if (this.OnStatusChange != null)
                      this.OnStatusChange(this, null);

              }

              
          }

      }


      public bool IsSuccess
      {
          get
          {
              return _IsSuccess;
          }
          set
          {
              if (this._IsSuccess != value)
              {
                  _IsSuccess = value;
                  if (this.OnStatusChange != null)
                      this.OnStatusChange(this, null);
              }
          }
      }
        public string DeviceName
        {
            get{
                return _devName;
               }

               set
               {
                   _devName = value;
               }
        }


          
      

        int IComparable.CompareTo(object obj)
        {
            OutputQueueData toCompare = (OutputQueueData)obj;

            if (this.mode != toCompare.mode)
                return this.mode - toCompare.mode;
            else if(this.priority!=toCompare.priority)
                    return this.priority - toCompare.priority;
            else if (this.HappenLineID != toCompare.HappenLineID)  //不同路線
            {
                if (this.HappenLineID == this.DevLineId)
                    return 1;
                else if (toCompare.HappenLineID == toCompare.DevLineId)
                    return -1;
                return 0;
            }
            else  // 路線相同
            {
             //   int ret = 0;
                if (HappenDir == "S" || HappenDir == "E")
                {
                    return -(this.HappenMileage - this.DevMileage) + (toCompare.HappenMileage - toCompare.DevMileage);
                }
                else if (HappenDir == "N" || HappenDir == "W")
                {
                    return (this.HappenMileage - this.DevMileage) - (toCompare.HappenMileage - toCompare.DevMileage);
                }
                else
                {
                    return -System.Math.Abs(this.HappenMileage - this.DevMileage) + System.Math.Abs(toCompare.HappenMileage - toCompare.DevMileage);
                }
            }


           

           // throw new Exception("The method or operation is not implemented.");
        }



      

       

      
    }
}
