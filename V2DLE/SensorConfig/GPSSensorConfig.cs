using System;
using System.Collections.Generic;
using System.Text;

namespace Comm.SensorConfig
{
    public class GPSSensorConfig : SensorConfigBase
    {
        //public int id { get; set; }
        //public string device_name { get; set; }
        //public string com_type { get; set; } //TCP ot COM
        //public string ip_comport { get; set; }
        //public int port_baud { get; set; }
        //public string device_type { get { return "GPS"; } }
        public double refx, refy, refz;
        public bool is_reference;

    }
}
