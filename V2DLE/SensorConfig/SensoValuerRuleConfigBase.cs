using System;
using System.Collections.Generic;
using System.Text;

namespace Comm.SensorConfig
{
    public class SensorValueRuleConfigBase
    {
        public  int  level {get;set;}  //1~3
        public int hour_ma { get; set; } 
        public double lower_limit { get; set; } //注意值，警戒值，行動值 下限
        public double upper_limit { get; set; } //注意值，警戒值，行動值 上限
        public int left_hour_ma1 { get; set; } 
        public int right_hour_ma1 { get; set; }
        public int left_hour_ma2 { get; set; }
        public int right_hour_ma2 { get; set; }

    }
}
