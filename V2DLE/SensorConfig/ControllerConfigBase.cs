using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Comm.SensorConfig
{
   [XmlInclude(typeof(TiltSensorConfig)), XmlInclude(typeof(GPSSensorConfig))     ]
   public class ControllerConfigBase
    {
        public ushort controller_id = 0xffff;
        public int listen_port;

        public SensorConfigBase[] sensors;// = new PiltConfig[16];
        public string device_type;
     //  public byte execution_mode;//0:investigation mode 1:running mode
        public byte version_no;
        public DateTime build_date;

       public  void Serialize(string pathfile)
        {
            System.IO.FileStream fs = System.IO.File.Create(pathfile);
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(ControllerConfigBase));
            x.Serialize(fs, this);
            fs.Close();
            fs.Dispose();

        }
       public void Serialize()
       {
           Serialize(AppDomain.CurrentDomain.BaseDirectory + "config.xml");
       }
       public static  ControllerConfigBase Deserialize(string pathfile)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(pathfile);
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(ControllerConfigBase));
            return x.Deserialize(fs) as ControllerConfigBase;
        }
       public static ControllerConfigBase Deserialize(System.IO.Stream stream)
       {
           //System.IO.FileStream fs = System.IO.File.OpenRead(pathfile);
           System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(ControllerConfigBase));
           return x.Deserialize(stream) as ControllerConfigBase;
       }
    }
}
