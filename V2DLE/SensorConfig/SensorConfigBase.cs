using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Comm.SensorConfig
{
    // [XmlInclude(typeof(SensorRuleConfigBase))]
    public class SensorConfigBase
    {
        public int id{get;set;}
        public string device_name { get; set; }
        public string com_type { get; set; } //TCP ot COM
        public string ip_comport { get; set; }
        public int port_baud { get; set; }
        public byte execution_mode { get; set; }
        public SensorValueConfigBase[] sensor_values { get; set; }     
        
       
        
    }
}
