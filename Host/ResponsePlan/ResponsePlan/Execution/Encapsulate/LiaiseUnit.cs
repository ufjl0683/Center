using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    [Serializable]
    public class LiaiseUnit
    {
        public int serviceID;
        public int subServiceID;
        public string serviceName;
        public string subserviceName;
        public int alarmclass;
        public bool ifAlarm;
        public string phone;
        public string fax;
        public int startMile;
        public int endMile;
        public string memo;
    }
}
