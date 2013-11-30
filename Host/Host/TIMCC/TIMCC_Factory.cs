using System;
using System.Collections.Generic;
using System.Text;
//using System.Collections.Generic;

namespace Host.TIMCC
{
   public  class TIMCC_Factory
    {
     
       public  static  SectionTrafficData[] getSectionData(String urlstr) 
	{
		//try {
        string url = urlstr;

        System.Collections.ArrayList ary = new System.Collections.ArrayList();

        System.Xml.XmlReader rd = null;
       // Ds ds = new Ds();

        using (rd = System.Xml.XmlTextReader.Create(url))
        {
            while (rd.Read())
            {

                if (rd.Name == "traffic_data" && rd.NodeType == System.Xml.XmlNodeType.Element)
                {
                   // Ds.tblSecTrafficDataRow r = ds.tblSecTrafficData.NewtblSecTrafficDataRow();
                   
                    string dir = "";
                    switch (System.Convert.ToString(rd["directionId"]))
                    {
                        case "1":
                            dir = "E";
                            break;
                        case "2":
                            dir = "W";
                            break;
                        case "3":
                            dir = "S";
                            break;
                        case "4":
                            dir = "N";
                            break;
                    }
                    //r.directionId = dir;
                    //r.end_location = System.Convert.ToString(rd["end_location"]);
                    //r.end_milepost = System.Convert.ToUInt32(rd["end_milepost"]);
                    //r.expresswayId = rd["expresswayId"].ToString();
                    //r.freewayId = rd["freewayId"].ToString();
                    //r.from_location = rd["from_location"].ToString();
                    //r.from_milepost = System.Convert.ToUInt32(rd["from_milepost"]);
                    //r.section_lower_limit = System.Convert.ToUInt32(rd["section_lower_limit"]);
                    //r.section_upper_limit = System.Convert.ToUInt32(rd["section_upper_limit"]);
                    //r.travel_time = System.Convert.ToSingle(rd["travel_time"]);

                    SectionTrafficData sec = new SectionTrafficData(rd["freewayId"].ToString().Trim() == "" ? rd["expresswayId"].ToString() : rd["freewayId"].ToString().Trim(),
                        dir, System.Convert.ToInt32(rd["from_milepost"]), System.Convert.ToInt32(rd["end_milepost"]),rd["from_location"].ToString(),
                        System.Convert.ToString(rd["end_location"]), System.Convert.ToInt32(rd["travel_time"]), System.Convert.ToInt32(rd["section_upper_limit"]),
                        System.Convert.ToInt32(rd["section_lower_limit"]));
                    ary.Add(sec);
                   // ds.tblSecTrafficData.AddtblSecTrafficDataRow(r);

                }
                else if (rd.Name == "file_attribute" && rd.NodeType == System.Xml.XmlNodeType.Element)

                   SectionTrafficData.timeStamp=System.Convert.ToDateTime(rd["time"]);


            }
            rd.Close();

        }

        SectionTrafficData[] ret = new SectionTrafficData[ary.Count];

        for (int i = 0; i < ary.Count; i++)
            ret[i] =(SectionTrafficData) ary[i];

        return ret;

       
		
	}


   }
}
