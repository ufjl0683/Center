using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using RemoteInterface;

namespace Host
{
  public   class XmlWeatherManager
    {
     

        System.Collections.Generic.List<WeatherSetting> list = new List<WeatherSetting>();
        System.Collections.Generic.Dictionary<string,WeartherData> dict = new System.Collections.Generic.Dictionary<string,WeartherData>();
        System.Threading.Timer tmrReadXml;
        string urlbase = "http://10.21.50.100";
        System.Collections.Generic.List<string> lst3X6 = new List<string>();
        System.Collections.Generic.List<string> lst2X8 = new List<string>();
        int index3x6=-1, index2x8=-1;
        System.Timers.Timer tmr10min = new System.Timers.Timer();

        public  bool IsWeatherTurn = true;
        public XmlWeatherManager()
        {
            LoadWeatherSetting();
            LoadAdver();
            tmrReadXml = new System.Threading.Timer(ReadXmlTask);
            tmrReadXml.Change(0, 30 * 60 * 1000);
           // tmrReadXml.Change(0, 10*1000);
            tmr10min.Elapsed += new System.Timers.ElapsedEventHandler(tmr10min_Elapsed);
            tmr10min.Interval = 10 * 60 * 1000;
            tmr10min.Start();
        }

        void tmr10min_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
                //throw new NotImplementedException();

            IsWeatherTurn = !IsWeatherTurn;
            if (!IsWeatherTurn)
            {
               
               if(index2x8 != -1)
                {
                    index2x8 = (index2x8 + 1) % lst2X8.Count;
                }

             
                if(index3x6 !=-1)
                {
                    index3x6 = (index3x6 + 1) % lst3X6.Count;
                }



            }

        }



      public string GetXmlFileNameByLocation(string lineid, string direction, int mile_m)
        {
            WeatherSetting[] settings = list.ToArray();
            foreach (WeatherSetting setting in settings)
            {
                if (lineid == setting.LineID && direction == setting.Direction && (mile_m >= setting.Start_MileLeage && mile_m < setting.End_Mileleage || mile_m >= setting.End_Mileleage && mile_m < setting.Start_MileLeage))
                    return setting.Xml_Name;
            }

            return "";
        }

        void LoadAdver()
        {
            OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
            OdbcCommand cmd = new OdbcCommand();
            cmd.CommandText = "Select Category ,Text  from tblFullText  where FullTextType=2  ";
            cmd.Connection = cn;
            try
            {
                cn.Open();
                System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    if (rd[0].ToString().StartsWith("2X8"))
                        this.lst2X8.Add(rd[1].ToString());

                    if (rd[0].ToString().StartsWith("3X6"))
                        lst3X6.Add(rd[1].ToString());
                }

                rd.Close();

                if (lst2X8.Count == 0)
                    index2x8 = -1;
                else
                    index2x8 = 0;
                if (lst3X6.Count == 0)
                    index3x6 = -1;
                else
                    index3x6 = 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }
            finally
            {
                cn.Close();
            }
        }


      void LoadWeatherSetting()
        {
            OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
            OdbcCommand cmd = new OdbcCommand();
            cmd.CommandText = "Select LineID,Xml_Name,Direction,Start_Mileage,End_Mileage from tblweather";
            cmd.Connection = cn;
            try
            {
                cn.Open();
                OdbcDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    WeatherSetting setting = new WeatherSetting();
                    setting.LineID = rd[0].ToString();
                    setting.Xml_Name = rd[1].ToString();
                    setting.Direction = rd[2].ToString();
                    setting.Start_MileLeage = System.Convert.ToInt32(rd[3])*1000;
                    setting.End_Mileleage = System.Convert.ToInt32(rd[4])*1000;
                    list.Add(setting);
                    if(!dict.ContainsKey(setting.Xml_Name))
                        dict.Add(setting.Xml_Name,new WeartherData(){xml_file_name=setting.Xml_Name});
                }
                rd.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }
            finally
            {
                
                cn.Close();
            }
        }
      public string  GetAdvert2X8()
      {
          if (index2x8 == -1)
              return "";
         return lst2X8[index2x8];
      }
      public string GetAdvert3X6()
      {
          if (index3x6 == -1)
              return "";
          return lst3X6[index3x6];

      }
      public WeartherData getWeather(string xmlname)
      {
          lock (dict)
          {
              if (dict.ContainsKey(xmlname))
                  return dict[xmlname];
              else
                  throw  new Exception("讀取氣象資料錯誤!");
          }
      }

      void ReadXmlTask(object state)
      {
          try
          {
              lock (dict)
              {

                  foreach (WeartherData wdata in dict.Values)
                  {
                      try
                      {
                          ReadXmlWeatherData(urlbase + "/" + wdata.xml_file_name, wdata);
                          Console.WriteLine(wdata);
                      }
                      catch (Exception ex)
                      {
                          Console.WriteLine(wdata.xml_file_name + " read error!");
                      }
                  }
              }
          }
          catch(Exception ex)
          {
              ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
          }
              
          }
      

        void ReadXmlWeatherData(string url, WeartherData wdata )
      {
          System.Net.WebClient client = new System.Net.WebClient();
          
         // System.IO.Stream stream= client.OpenRead(url);
          System.Xml.XmlReader rd = System.Xml.XmlReader.Create(url);
          int cdcnt = 0;
          bool found = false;
          while (rd.Read())
          {
             
              if (rd.NodeType == System.Xml.XmlNodeType.CDATA && rd.Depth == 4)
              {
                  cdcnt++;

                  if (cdcnt == 5)  //get district title
                  {
                     string str= rd.Value.Trim();
                     int pos=  str.IndexOf(' ',0);
                     wdata.district=str.Substring(0,pos-1)+" " ; //去掉縣市 家空白
                  }
                  if (cdcnt == 6)  // get weather array
                  {
                      string str = rd.Value.Replace('\n',' ').Replace('\t',' ').Trim();
                      string[] weatherstrs = str.Split(new string[] { "<BR>" } , StringSplitOptions.RemoveEmptyEntries);

                    
                      foreach (string wstr in weatherstrs)
                      {
                         // Console.WriteLine(wstr.Trim());
                          string[] items = wstr.Trim().Split(new char[] { ' ' });
                          string strdate = items[0];
                          string dayOrnight = items[1];
                          int lowtemp = System.Convert.ToInt32(items[2].Split(new char[] { ':' })[1]);
                          int highttemp = System.Convert.ToInt32(items[4]);
                          string desc = items[5];
                          if (strdate == string.Format("{0:00}/{1:00}", DateTime.Now.Month, DateTime.Now.Day))
                          {
                             
                              if (dayOrnight == "白天")
                              {
                                  wdata.day_low_temp = lowtemp;
                                  wdata.day_high_temp = highttemp;
                                  wdata.day_weatrher_description = desc;
                              }
                              else if (dayOrnight == "晚上")
                              {
                                  wdata.night_low_temp = lowtemp;
                                  wdata.night_high_temp = highttemp;
                                  wdata.night_weatrher_description = desc;
                              }
                              else
                                  throw new Exception("中央氣象局資料剖析錯誤");
                              found = true;    
                          }
                         
                      }

                  }
                  //if(cdcnt==5 || cdcnt==6)
                  //Console.WriteLine(rd.Name + "_" + rd.NodeType + "_" + rd.Depth + rd.Value);
              }
          }

          rd.Close();
          client.Dispose();
          if (found)
              wdata.IsValid = true;
          else
              wdata.IsValid = false;
          
       
      }

    }

  public class WeartherData
  {
     public string xml_file_name;
     public  string district;
     public  int day_low_temp;
     public  int day_high_temp;
     public  string day_weatrher_description;
     public int night_low_temp;
     public int night_high_temp;
     public string night_weatrher_description;
     public bool IsValid = false;

     public string To2X8String()
     {
          string display1,display2;
         if (DateTime.Now.Hour <= 18)
         {
             display1 = district + day_low_temp + "~" + day_high_temp+"度";
             display2 = day_weatrher_description;
            
         }
         else
         {
             display1 = district + night_low_temp + "~" + night_high_temp + "度";
             display2 = night_weatrher_description;
         }
         if (display2.Length > 8)
             return display1;
         else
             return display1 + "\r" + PadCenter(8,display2);

     }

     public string To3X6String()
     {
         string display1="", display2="", display3 = ""; 
         display1 = district;
         if (DateTime.Now.Hour < 18)
         {
            // display2 = day_low_temp + "~" + day_high_temp + "度";
             display1 += day_low_temp + "~" + day_high_temp + "度";
             if (day_weatrher_description.Length <= 6)
             
                 display2 = day_weatrher_description;
             else
             {
                 
                 display2 = day_weatrher_description.Substring(0, 6);
                 display3 = day_weatrher_description.Substring(6, day_weatrher_description.Length % 6); 
             }
         }
         else
         {
             //display2 = night_low_temp + "~" + night_high_temp + "度";
             //display3 = night_weatrher_description;
             display1 += night_low_temp + "~" + night_high_temp + "度";
             if (night_weatrher_description.Length <= 6)

                 display2 = night_weatrher_description;
             else
             {
                  
                 display2 = night_weatrher_description.Substring(0, 6);
                 display3 = night_weatrher_description.Substring(6, night_weatrher_description.Length % 6);

             }
         }
         if (display3.Length>0)
         {
             return display1+"\r"+display2+"\r"+PadCenter(6,display3);
         }
         else
             return display1 + "\r" + PadCenter(6,display2) ;
     }

     public string PadCenter(int totalWidth, string display)
     {
         return display.PadLeft((totalWidth - display.Length)   + display.Length);
     }

     public override string ToString()
     {
         //return base.ToString();
         if (DateTime.Now.Hour <= 18)
             return district + day_low_temp + "~" + day_high_temp + "度\r\n" + day_weatrher_description;
         else
             return district + night_low_temp + "~" + night_high_temp + "度\r\n" + night_weatrher_description;
     }
    
  }
  public class WeatherSetting
  {
      public string LineID;
      public string Xml_Name;
      public string Direction;
      public int Start_MileLeage;
      public int End_Mileleage;
    
     
  }
}
