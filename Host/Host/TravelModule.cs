using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using System.Data;
using System.Drawing;
namespace Host
{
    public delegate void new_travel_time_handler(RGS_TravelDisplayData[] displayData);
   public   class TravelModule
    {
       Ds RGSConfDs = new Ds();
       Ds Curr5minSecDs;
       public static System.Collections.Hashtable ip_hash = new System.Collections.Hashtable();
       public  event new_travel_time_handler OnNewTravelData;
       System.Threading.Thread thOneMinCalTask;
      public  TravelModule()
       {
           load_RGS_table();

           thOneMinCalTask = new System.Threading.Thread(section_data_1min_task);
           thOneMinCalTask.Start();
       }

       public  void section_data_1min_task()
       {

           ConsoleServer.WriteLine("section_data_1min_task started!");

           while (true)
           {
               try
               {
                   System.Net.WebRequest req = System.Net.HttpWebRequest.Create("http://192.168.4.4/section_traffic_data.xml");

                   System.IO.Stream stream = req.GetResponse().GetResponseStream();
                   System.IO.TextReader rd = new System.IO.StreamReader(stream);
                   System.IO.TextWriter wr = new System.IO.StreamWriter(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "tmp.xml"));
                   wr.Write(rd.ReadToEnd());
                   wr.Flush();
                   wr.Close();
                   rd.Close();
                   stream.Close();
                   wr.Dispose();
                   rd.Dispose();
                   stream.Dispose();


                   Ds ds = five_min_section_parser(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "tmp.xml"));
                   // Curr5minSecDs.Dispose();
                   Curr5minSecDs = ds;


                   string dest;
                   if ((dest = getSaveDirFileName(ds.tblFileAttr[0].time)) != "")   // new 5 min data
                   {
                      ConsoleServer.WriteLine("section_data_1min_task: new section data->" + ds.tblFileAttr[0][0].ToString());


                       System.IO.File.Copy(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "tmp.xml"), dest);
                       //if (OnNewTravelData != null)
                       //{
                           
                           calcuate_travel_time();

                           RGS_TravelDisplayData[] displayDatas = new RGS_TravelDisplayData[RGSConfDs.tblRGSMain.Rows.Count];
                           for (int i = 0; i < RGSConfDs.tblRGSMain.Rows.Count; i++)
                           {
                               displayDatas[i] = getRgsDisplay(RGSConfDs.tblRGSMain[i]);
                           }

                          if(OnNewTravelData!=null)
                              OnNewTravelData(displayDatas);
                           //try
                           //{
                           //    NotifyDisplayTask();
                           //}
                           //catch (Exception ex)
                           //{
                           //    Console.WriteLine("section_data_1min_task:" + ex.Message);
                           //}
                       //}


                   }



               }
               catch (Exception ex)
               {
                   ConsoleServer.WriteLine("section_data_1min_task:" + ex.Message + ex.StackTrace);

                   try
                   {
                       if (Curr5minSecDs == null || System.Math.Abs(((TimeSpan)(System.DateTime.Now - Curr5minSecDs.tblFileAttr[0].time)).TotalMinutes) >= 20)
                       {
                           lock (RGSConfDs.tblRGSMain)
                           {

                               RGS_TravelDisplayData[] displayDatas = new RGS_TravelDisplayData[RGSConfDs.tblRGSMain.Rows.Count];
                               for (int i = 0; i < RGSConfDs.tblRGSMain.Rows.Count; i++)
                               {
                                   displayDatas[i] =new RGS_TravelDisplayData(RGSConfDs.tblRGSMain[0].rgs_name,null,null,null);
                               }

                               if (OnNewTravelData != null)
                                   OnNewTravelData(displayDatas);
                               //foreach (Ds.tblRGS_ConfigRow r in RGSConfDs.tblRGS_Config.Rows)
                               //{
                               //    r.RowError = "Timcc 連線資料異常!";
                               //    r.mode = 1; //手動
                               //    Console.WriteLine("寫入 RowErr");



                               //}
                               //if (OnNewTravelData != null)
                               //{

                               //    try
                               //    {
                               //        DataSet ds1 = new DataSet();

                               //        ds1.Tables.Add(Util.getPureDataTable(RGSConfDs.tblRGS_Config));
                               //        OnNewTravelData(ds1);
                               //        NotifyDisplayTask();
                               //    }
                               //    catch (Exception ex1)
                               //    {
                               //        Console.WriteLine("section_data_1min_task:" + ex1.StackTrace);
                               //    }
                               //}
                           }
                           ConsoleServer.WriteLine("Timcc 連線異常!");
                       }
                   }
                   catch (Exception ex1)
                   {
                       ConsoleServer.WriteLine(ex1.Message);
                   }

               }



         //   Util.GC();

               System.Threading.Thread.Sleep(60 * 1000);
           }





       }
        void load_RGS_table()
       {

           int rowcnt = 0;

           System.IO.TextReader tr = new System.IO.StreamReader(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "RGS_Config.csv"), System.Text.Encoding.GetEncoding("big5"));

           //Host.Ds.RGSModeRow rr;

           //rr = RGSConfDs.tblRGSMode.NewtblRGSModeRow();
           //rr.value = 0;
           //rr.display = "旅行時間模式";
           //RGSConfDs.tblRGSMode.AddtblRGSModeRow(rr);
           //rr = RGSConfDs.tblRGSMode.NewtblRGSModeRow();
           //rr.value = 1;
           //rr.display = "手動輸入模式";
           //RGSConfDs.tblRGSMode.AddtblRGSModeRow(rr);




           string s = "";
           string[] fields;
           s = tr.ReadLine();

           fields = s.Split(new char[] { ',' });

           for (int i = 0; i < fields.Length; i++)
               fields[i] = fields[i].Trim();



           while ((s = tr.ReadLine()) != null)
           {


               string[] tmp = s.Split(new char[] { ',' });

               System.Data.DataRow r = RGSConfDs.tblRGS_Config.NewRow();

               for (int i = 0; i < tmp.Length; i++)
               {
                   r[i] = System.Convert.ChangeType(tmp[i], RGSConfDs.tblRGS_Config.Columns[i].DataType);
               }
               if (r["freewayId"].ToString() == "0")
                   r["mode"] = 1;
               else
                   r["mode"] = 0;
               r["ficon"] = 0;

               r["message1"] = "";
               r["message2"] = "";

               r["finput1"] = "";
               r["finput2"] = "";
               r["traveltime"] = 0;
               r["lowerlimit"] = 0;
               r["upperlimit"] = 0;
               lock (RGSConfDs.tblRGSMain)
               {
                   if (ip_hash[r["ip"].ToString()] == null)
                   {
                        Ds.tblRGSMainRow rmain = RGSConfDs.tblRGSMain.NewtblRGSMainRow();
                       ip_hash.Add(r["ip"].ToString(), rmain);
                       rmain.ip = r["ip"].ToString();
                       rmain.rgs_name = r["rgs_name"].ToString();
                       rmain.location = System.Convert.ToUInt32(r["location"]);
                       rmain.freewayId = r["freewayId"].ToString();
                       rmain.direction = r["direction"].ToString();
                       rmain.deviec_id = System.Convert.ToInt32(r["deviec_id"].ToString(), 16);
                       rmain.port = (uint)r["port"];
                       rmain.connected = false;
                       rmain.hwstatus1 = rmain.hwstatus2 = rmain.hwstatus3 = rmain.hwstatus4 = 0;
                       rmain.DisplayErr = rmain.DownLinkErr = rmain.DeviceErr = rmain.LED_ModuleErr = rmain.UplinkErr = rmain.CabineteOpen = false;
                       rmain.EndEdit();
                       RGSConfDs.tblRGSMain.AddtblRGSMainRow(rmain);
                   }
               }
               RGSConfDs.tblRGS_Config.Rows.Add(r);
               rowcnt++;
               ////----------------------------just for test
               //if (rowcnt == 2)
               //    break;
           }



       }
       public  Ds five_min_section_parser(string uri)
       {

           System.Xml.XmlReader rd = null;
           Ds ds = new Ds();

           using (rd = System.Xml.XmlTextReader.Create(uri))
           {
               while (rd.Read())
               {

                   if (rd.Name == "traffic_data" && rd.NodeType == System.Xml.XmlNodeType.Element)
                   {
                       Ds.tblSecTrafficDataRow r = ds.tblSecTrafficData.NewtblSecTrafficDataRow();

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
                       r.directionId = dir;
                       r.end_location = System.Convert.ToString(rd["end_location"]);
                       r.end_milepost = System.Convert.ToUInt32(rd["end_milepost"]);
                       r.expresswayId = rd["expresswayId"].ToString();
                       r.freewayId = rd["freewayId"].ToString();
                       r.from_location = rd["from_location"].ToString();
                       r.from_milepost = System.Convert.ToUInt32(rd["from_milepost"]);
                       r.section_lower_limit = System.Convert.ToUInt32(rd["section_lower_limit"]);
                       r.section_upper_limit = System.Convert.ToUInt32(rd["section_upper_limit"]);
                       r.travel_time = System.Convert.ToSingle(rd["travel_time"]);

                       ds.tblSecTrafficData.AddtblSecTrafficDataRow(r);

                   }
                   else if (rd.Name == "file_attribute" && rd.NodeType == System.Xml.XmlNodeType.Element)

                       ds.tblFileAttr.AddtblFileAttrRow(System.Convert.ToDateTime(rd["time"]));


               }
               rd.Close();

           }
           return ds;

       }
       public  string getSaveDirFileName(System.DateTime dt)
       {
           if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "SecData"))
               System.IO.Directory.CreateDirectory(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "SecData"));
           string dir = string.Format(AppDomain.CurrentDomain.BaseDirectory + @"SecData\{0:0000}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);

           string filename = string.Format(@"{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}.xml", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);

           if (!System.IO.Directory.Exists(Util.CPath(dir)))
               System.IO.Directory.CreateDirectory(Util.CPath(dir));

           if (System.IO.File.Exists(Util.CPath(dir + @"\" + filename)))
               return "";
           else
               return Util.CPath(dir + @"\" + filename);


       }
       public  void calcuate_travel_time()
       {
           lock (RGSConfDs.tblRGS_Config)
           {
               foreach (Ds.tblRGS_ConfigRow r in RGSConfDs.tblRGS_Config.Rows)
               {
                   System.Data.DataView vw = null;

                   vw = get_travel_sections_detail_data(r.from_milepost, r.end_milepost, r.freewayId, r.direction);
                   //if(r.direction=="S")
                   //     vw =new DataView(Curr5minSecDs.tblSecTrafficData,
                   //    string.Format("from_milepost>={0} and  from_milepost < {1} and freewayId={2} and directionId='S'", r.from_milepost, r.end_milepost-1000, r.freewayId)
                   //    ,"",DataViewRowState.CurrentRows);
                   //else if(r.direction=="N")
                   //    vw =new DataView(Curr5minSecDs.tblSecTrafficData,
                   //    string.Format("from_milepost<={0} and  from_milepost > {1} and freewayId={2}  and directionId='N'", r.from_milepost, r.end_milepost+1000, r.freewayId)
                   //, "", DataViewRowState.CurrentRows);

                   r.lowerlimit = r.upperlimit = 0;
                   r.traveltime = 0;
                   r.RowError = "";
                   for (int i = 0; i < vw.Count; i++)
                   {

                       float traveltime = (float)vw[i]["travel_time"];
                       if (traveltime == 65535)
                           r.RowError = "無效的旅行時間";

                       r.traveltime += traveltime;
                       r.lowerlimit += System.Convert.ToUInt16(vw[i]["section_upper_limit"]);
                       r.upperlimit += System.Convert.ToUInt16(vw[i]["section_lower_limit"]);

                   }
                   r.lowerlimit = (ushort)(r.lowerlimit / 60);
                   r.upperlimit = (ushort)(r.upperlimit / 60);
                   r.traveltime = System.Convert.ToInt16(r.traveltime / 60);
                   r.message1 = r.msg_temp1;

                   if (r.HasErrors)
                   {
                       r.message1 = r.message2 = "";
                       r.messageColor2 = r.messageColor1 = "";

                   }
                   else
                   {

                       if (r.traveltime < r.upperlimit)
                       {

                           r.message1 = r.msg_temp1;
                           r.message2 = r.msg_temp2.Replace("@", System.Convert.ToUInt16(r.upperlimit).ToString());
                           r.RowError = "旅行時間超過上限值";



                       }
                       else
                       {
                           r.message2 = r.msg_temp2.Replace("@", System.Convert.ToUInt16(r.traveltime).ToString());
                           // set display color

                       }

                       if (r.traveltime > r.lowerlimit)
                       {
                           r.message1 = "";
                           r.message2 = "";
                           r.messageColor2 = r.messageColor1 = "";
                           r.RowError = "旅行時間超過下限值";
                       }

                       // set display color
                       Color[] colors = new Color[r.message1.Length];
                       for (int i = 0; i < colors.Length; i++)
                           colors[i] = Color.Red;
                       r.messageColor1 = Util.ToColorString(colors);

                       colors = new Color[r.message2.Length];
                       for (int i = 0; i < colors.Length; i++)
                       {
                           int colorinx = r.msg_temp2.IndexOf('@');
                           if (i >= colorinx && i <= System.Convert.ToUInt16(r.upperlimit).ToString().Length + colorinx - 1)
                               colors[i] = Color.Orange;
                           else

                               colors[i] = Color.Red;
                       }
                       r.messageColor2 = Util.ToColorString(colors);

                   }





               }
           }
       }

       public  System.Data.DataView get_travel_sections_detail_data(uint from_milepost, uint end_milepost, string freewayId, string direction)
       {
           System.Data.DataView vw = null;
           if (direction == "S")
               vw = new DataView(Curr5minSecDs.tblSecTrafficData,
              string.Format("from_milepost>={0} and  from_milepost < {1} and freewayId='{2}' and directionId='S'", from_milepost - 500, end_milepost - 500, freewayId)
              , "", DataViewRowState.CurrentRows);
           else if (direction == "N")
               vw = new DataView(Curr5minSecDs.tblSecTrafficData,
               string.Format("from_milepost<={0} and  from_milepost > {1} and freewayId='{2}' and directionId='N'", from_milepost + 500, end_milepost + 500, freewayId)
           , "", DataViewRowState.CurrentRows);

           return vw;
       }
       private RGS_TravelDisplayData getRgsDisplay(Ds.tblRGSMainRow r)
       {



           try
           {
               

               //  System.Console.WriteLine(r.rgs_name + "," + r.ip + ","+r.port+"," + ((r.display_part == 1) ? "上" : "下") + ",icon:" + r.iconid + "," + r.message1 + "," + r.message2);

               Ds.tblRGS_ConfigRow[] rows = (Ds.tblRGS_ConfigRow[])r.GetChildRows("tblRGSMain_tblRGS_Config");
              
                   string[] msgs = new string[4];
                   byte[] icons;
                   Color[][] colors = new Color[4][];
                   icons = new byte[2];
                   string[] strcolors;
                   lock (RGSConfDs.tblRGS_Config)
                   {
                       for (int i = 0; i < 2; i++)
                       {
                           //icons[i] = rows[i].iconid;
                           if (rows[i].display_part == 1)  //上
                           {



                               //if (rows[i].mode == 0)  //travel mode
                               //{
                                   msgs[0] = rows[i].message1;
                                   msgs[1] = rows[i].message2;
                                   icons[0] = rows[i].iconid;

                               //}
                               //else
                               //{
                               //    msgs[0] = rows[i].finput1;
                               //    msgs[1] = rows[i].finput2;
                               //    icons[0] = rows[i].ficon;

                               //}

                               if (msgs[0] != "")
                               {
                                   strcolors = ((string)((rows[i].mode == 0) ? rows[i].messageColor1 : rows[i].finputColor1)).Split(new char[] { ',' });
                                   colors[0] = new Color[strcolors.Length / 3];
                                   for (int cinx = 0; cinx < strcolors.Length / 3; cinx++)
                                       colors[0][cinx] = System.Drawing.Color.FromArgb(System.Convert.ToInt32(strcolors[cinx * 3]), System.Convert.ToInt32(strcolors[cinx * 3 + 1]), System.Convert.ToInt32(strcolors[cinx * 3 + 2]));
                               }
                               else
                               {
                                   colors[0] = new Color[0];
                               }
                               if (msgs[1] != "")
                               {
                                   strcolors = ((string)((rows[i].mode == 0) ? rows[i].messageColor2 : rows[i].finputColor2)).Split(new char[] { ',' });
                                   colors[1] = new Color[strcolors.Length / 3];
                                   for (int cinx = 0; cinx < strcolors.Length / 3; cinx++)
                                       colors[1][cinx] = System.Drawing.Color.FromArgb(System.Convert.ToInt32(strcolors[cinx * 3]), System.Convert.ToInt32(strcolors[cinx * 3 + 1]), System.Convert.ToInt32(strcolors[cinx * 3 + 2]));

                               }
                               else
                               {
                                   colors[1] = new Color[0];
                               }

                               rows[i].curr_icon = icons[0];
                               rows[i].curr_msg1 = msgs[0];
                               rows[i].curr_msg2 = msgs[1];

                           }

                           else  //下
                           {

                               //if (rows[i].mode == 0)
                               //{
                                   msgs[2] = rows[i].message1;
                                   msgs[3] = rows[i].message2;
                                   icons[1] = rows[i].iconid;
                               //}
                               //else
                               //{
                               //    msgs[2] = rows[i].finput1;
                               //    msgs[3] = rows[i].finput2;
                               //    icons[1] = rows[i].ficon;
                               //}


                               if (msgs[2] != "")
                               {
                                   strcolors = ((string)((rows[i].mode == 0) ? rows[i].messageColor1 : rows[i].finputColor1)).Split(new char[] { ',' });
                                   colors[2] = new Color[strcolors.Length / 3];
                                   for (int cinx = 0; cinx < strcolors.Length / 3; cinx++)
                                       colors[2][cinx] = System.Drawing.Color.FromArgb(System.Convert.ToInt32(strcolors[cinx * 3]), System.Convert.ToInt32(strcolors[cinx * 3 + 1]), System.Convert.ToInt32(strcolors[cinx * 3 + 2]));

                               }
                               else
                               {
                                   colors[2] = new Color[0];
                               }

                               if (msgs[3] != "")
                               {
                                   strcolors = ((string)((rows[i].mode == 0) ? rows[i].messageColor2 : rows[i].finputColor2)).Split(new char[] { ',' });
                                   colors[3] = new Color[strcolors.Length / 3];
                                   for (int cinx = 0; cinx < strcolors.Length / 3; cinx++)
                                       colors[3][cinx] = System.Drawing.Color.FromArgb(System.Convert.ToInt32(strcolors[cinx * 3]), System.Convert.ToInt32(strcolors[cinx * 3 + 1]), System.Convert.ToInt32(strcolors[cinx * 3 + 2]));

                               }
                               else
                               {
                                   colors[3] = new Color[0];
                               }
                               rows[i].curr_icon = icons[1];
                               rows[i].curr_msg1 = msgs[2];
                               rows[i].curr_msg2 = msgs[3];
                           }


                       }
                   }

                   //if (eventDispatcher != null)
                   //    eventDispatcher.NotifyAll(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.RGS_Display_Event, r.ip, Util.getPureDataTable(rows)));
                //   rgs_manager[r.ip].SetTravelDisplay(icons, msgs, colors);

                   for (int i = 0; i < 2; i++)
                   {
                       ConsoleServer.WriteLine(r.rgs_name + "," + ((rows[i].display_part == 1) ? "上" : "下") + "," + icons[i] + "," + msgs[i * 2] + "," + msgs[i * 2 + 1]);
                   }

                   return  new RGS_TravelDisplayData(r.rgs_name,icons,msgs,colors);
              







               // System.Console.WriteLine(r.rgs_name + "," + r.ip + "," + r.port + "," + ((r.display_part == 1) ? "上" : "下") + ",icon:" + r.ficon + "," + r.finput1 + "," + r.finput2);

               //}
               //if (eventDispatcher != null)
               //    eventDispatcher.NotifyAll(new RemoteInterface.NotifyEventObject(RemoteInterface.EventEnumType.RGS_Display_Event, r.ip, Util.getPureDataTable(rows)));


           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message);
               return null;
           }
           //finally
           //{
           //  //  Util.GC();
           //}




       }
    }

    public class RGS_TravelDisplayData
    {
      public  byte[] icons;
      public   string[] msgs;
      public   Color[][] colors;
        public string devname;
       public  RGS_TravelDisplayData(string devname,byte[] icons, string[] msgs, Color[][] colors)
        {
            this.icons = icons;
            this.msgs = msgs;
            this.colors = colors;
            this.devname = devname;
        }
    }
}
