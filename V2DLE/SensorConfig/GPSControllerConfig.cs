using System;
using System.Collections.Generic;
using System.Text;
using Comm.SensorConfig;
using System.Xml.Serialization;

namespace Comm.SensorConfig
{
  // [XmlInclude(typeof(GPSSensorConfig))]
    public class GPSControllerConfig:ControllerConfigBase
    {
      public  GPSSensorConfig ref_gps;

      public new  void Serialize()
      {
          Serialize(AppDomain.CurrentDomain.BaseDirectory + "config.xml");
      }
      public static new  ControllerConfigBase Deserialize(string pathfile)
      {
          System.IO.FileStream fs = System.IO.File.OpenRead(pathfile);
          System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(GPSControllerConfig));
          return x.Deserialize(fs) as GPSControllerConfig;
      }
    }
}
