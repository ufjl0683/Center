using System;
using System.Collections.Generic;
using System.Text;

namespace Comm.SensorConfig
{
   public  class SensorValueConfigBase
    {
       public int id { get; set; }
       public string desc { get; set; }
       public double offset { get; set; }
       public double coefficient { get; set; }
     //  public SensorValueRuleConfigBase[] rules{get;set;}
       public double SIGMA { get; set; }
       public double INITMEAN {get;set;}
       public double MeanThreshold { get; set; }
       public string ConvertFormula { get; set; }

    }
}
