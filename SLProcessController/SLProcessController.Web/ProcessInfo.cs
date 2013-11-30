using System;
using System.Collections.Generic;
using System.Web;

namespace SLProcessController.Web
{
    public class ProcessInfo
    {
        /*

              <DesignColumnRef Name="process_name" />
              <DesignColumnRef Name="console_port" />
              <DesignColumnRef Name="execute_string" />
              <DesignColumnRef Name="state" />
              <DesignColumnRef Name="cpu_time" />
              <DesignColumnRef Name="memory" />
              <DesignColumnRef Name="isAlive" />
              <DesignColumnRef Name="pid" />
         * */

        public string ProcessName
        {
            get;
            set;
        }
        public int ConsolePort
        { get; set; }
        public string ExecutiongString
        { get; set; }
        public int State { get; set; }
        public double CPU_Time { get; set; }
        public long Mermory { get; set; }
        public bool IsAlive { get; set; }
        public int PID { get; set; }
        public int DataQueueCnt { get; set; }
         public string HostIP {get;set;}
         public string MFCC_TYPE { get; set; }

    }
}