using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;
using System.Data.Odbc;
using System.Drawing;
using RemoteInterface;
using RemoteInterface.MFCC;

namespace Host.TC
{
   public  class CMSDeviceWrapper:OutPutDeviceBase
    {





       public string category;
       public string EnableWeather ="N";
       string xmlWeatherFileName;
       TravelDisplaySettingData[] travelDisplaySettingData;
       System.Timers.Timer tmr1Min;
       public CMSDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction)
          : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus, direction)
       {
           LoadCmsExtenSetting();
           loadTravelSetting();
           try
           {
               xmlWeatherFileName = Program.matrix.xml_weather_mgr.GetXmlFileNameByLocation(lineid, direction, mile_m);
           }
           catch { ;}

           tmr1Min = new System.Timers.Timer(1000 * 15);
           tmr1Min.Elapsed += new System.Timers.ElapsedEventHandler(tmr1Min_Elapsed);
           tmr1Min.Start();
          
#if DEBUG
          //CMSOutputData d31= new CMSOutputData(0,0,0,"A",new byte[]{30});
          //d31.AlarmClass = 31;
          //CMSOutputData d41 = new CMSOutputData(0, 0, 0, "A", new byte[] { 30 });
          //d41.AlarmClass = 41;
          //if (this.deviceName == "CMS-N1-N-241.7")
          //{
          //    OutputQueueData oqd31 = new OutputQueueData("CMS-N1-N-241.7", OutputModeEnum.ResponsePlanMode, 111, 0, d31);
          //    OutputQueueData oqd41 = new OutputQueueData("CMS-N1-N-241.7", OutputModeEnum.ResponsePlanMode, 112, 0, d41);
          //    this.SetOutput(oqd31);
          //    this.SetOutput(oqd41);
          //}
#endif

       }
       void LoadCmsExtenSetting()
       {
           OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
           OdbcDataReader  rd;
           OdbcCommand cmd = new OdbcCommand();
           cmd.CommandText = "select category,EnableWeather from tblcmsconfig where devicename='" + deviceName + "'";
           cmd.Connection = cn;
           try
           {
              cn.Open();
              rd= cmd.ExecuteReader();
              if (rd.Read())
              {
                  this.category = rd[0].ToString().Trim();
                  this.EnableWeather = rd[1].ToString().Trim();
                  if (this.EnableWeather == "Y")
                      Console.WriteLine(this.deviceName + ", enable weather!");
              }

           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(this.deviceName + "," + ex.Message + ex.StackTrace);
           }
           finally
           {
               cn.Close();
           }
               
         
       }

       void DisplayWeatherTask()
       {
           if (!(this.category.StartsWith("3X6") || this.category.StartsWith("2X8")) || xmlWeatherFileName=="")
               return; 
           WeartherData wdata = Program.matrix.xml_weather_mgr.getWeather(this.xmlWeatherFileName);
           if (!wdata.IsValid || EnableWeather=="N")
           {
               this.removeOutput(OutputQueueData.WEATHER_RULE_ID);
               return;
           }

           string mesg;
           if (this.category.StartsWith("3X6"))
           {
               if (!Program.matrix.xml_weather_mgr.IsWeatherTurn)
               {
                   mesg = Program.matrix.xml_weather_mgr.GetAdvert3X6();
                   if(mesg=="")
                       mesg = wdata.To3X6String();
               }
               else
                   mesg = wdata.To3X6String();
           }
           else  //2x8
           { 
               if (!Program.matrix.xml_weather_mgr.IsWeatherTurn)
               {
                   mesg = Program.matrix.xml_weather_mgr.GetAdvert2X8();
                   if (mesg == "")
                       mesg = wdata.To2X8String();
               }
               else
                 mesg = wdata.To2X8String();
           }
           int wordcnt = 0;
           byte[] colors;
           for (int i = 0; i < mesg.Length; i++)
           {
               if (mesg[i] != '\r')
                   wordcnt++;

           }
           colors = new byte[wordcnt];

           for (int i = 0; i < colors.Length; i++)
               colors[i] = 0x30;
           this.setCMS_Dispaly(this.deviceName,
               OutputModeEnum.WeatherMode,
               OutputQueueData.WEATHER_RULE_ID,
               OutputQueueData.WEATHER_PRIORITY, 0, 0,  0,
               mesg, colors);

       }



       int mincnt = 0;



       void tmr1Min_Elapsed(object sender, System.Timers.ElapsedEventArgs e)  //15sec cycle
       {

           if (mincnt % 4 == 0)
           {
               try
               {
                   if (Program.matrix.device_mgr != null && !Program.matrix.device_mgr.IsInLoadWrapper)
                       DisplayTravelTime();
               }
               catch (Exception ex)
               {
                   ConsoleServer.WriteLine(this.deviceName + "," + ex.Message + ex.StackTrace);
               }

               try
               {
                   // if(mincnt%10==0)
                   if (this.EnableWeather == "Y")
                   {
                       //OutputQueueData qdata = this.GetQueueData(RemoteInterface.HC.OutputQueueData.NORMAL_MANUAL_RULE_ID);
                       //if (qdata != null)
                       //{

                       //    if (qdata.mode == OutputModeEnum.ManualMode)
                       //        qdata.mode = OutputModeEnum.UnderWeatherMode;
                       //    else
                       //        qdata.mode = OutputModeEnum.ManualMode;

                       //}
                       try
                       {
                           DisplayWeatherTask();
                       }
                       catch (Exception ex)
                       {
                           ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                       }



                   }
                   else
                       this.removeOutput(OutputQueueData.WEATHER_RULE_ID);


                 

                   //else
                   //{  //還原
                   //     OutputQueueData qdata = this.GetQueueData(RemoteInterface.HC.OutputQueueData.NORMAL_MANUAL_RULE_ID);
                   //     if (qdata != null && qdata.mode != OutputModeEnum.ManualMode)
                   //     {
                   //         qdata.mode = OutputModeEnum.ManualMode;
                   //         output();
                   //     }
                   //}
               }
               catch (Exception ex)
               {
                   ConsoleServer.WriteLine(this.deviceName + "," + ex.Message + ex.StackTrace);

               };
           }

           if (AlarmClass41And31Process()) //同時有 壅塞及施工，那麼每隔15sec 兩者互相倫播
           {
               try
               {
                   output();
               }
               catch (Exception ex)
               {
                   ConsoleServer.WriteLine(this.deviceName + "," + ex.Message + ex.StackTrace);


               }
           }
           else if (mincnt % 4 == 0)  //正常一分鐘輸出
               {
                   try
                   {
                       output();
                   }
                   catch (Exception ex)
                   {
                       ConsoleServer.WriteLine(this.deviceName + "," + ex.Message + ex.StackTrace);


                   }
               }
           //try
           //{
           //    if (mincnt % 30 == 0) // 30 min task
           //        DisplayWeatherTask();


           //}
           //catch(Exception ex)
           //{
           //    ConsoleServer.WriteLine(this.deviceName + "," + ex.Message + ex.StackTrace);

           //}

              mincnt=(mincnt+1)%1000;
           //throw new Exception("The method or operation is not implemented.");
       }

       private bool AlarmClass41And31Process()
           //同時有 壅塞及施工，那麼每隔30sec 兩者互相倫播
       {
           //throw new NotImplementedException();
           OutputQueueData  qClass31=null, qClass41=null;
           OutputQueueData[]  qdatas = this.GetPriorityQueueData();
#if DEBUG
           //if (this.deviceName != "CMS-N1-N-241.7")
           //    return;
#endif
           try
           {
               foreach (OutputQueueData qdata in qdatas)
               {
                   if (qdata.data is CMSOutputData)
                   {
                       if (((CMSOutputData)qdata.data).AlarmClass == 31)
                           qClass31 = qdata;
                       if (((CMSOutputData)qdata.data).AlarmClass == 41)
                           qClass41 = qdata;


                   }
               }

               if (qClass31!=null &&qClass41!=null)
               {
                   if(mincnt%2==0)
                   {
                       qClass31.mode= OutputModeEnum.BelowResponseMode;
                       qClass41.mode = OutputModeEnum.ResponsePlanMode;
                   }
                   else
                   {
                       qClass31.mode= OutputModeEnum.ResponsePlanMode;
                       qClass41.mode = OutputModeEnum.BelowResponseMode;
                   }

                   return true;
               }
               else if (qClass31 != null)
               {
                   if (qClass31.mode == OutputModeEnum.BelowResponseMode)
                   {
                       qClass31.mode = OutputModeEnum.ResponsePlanMode;
                       return true;
                   }
                   return false;
               }
               else if (qClass41 != null)
               {
                   if (qClass41.mode == OutputModeEnum.BelowResponseMode)
                   {
                       qClass41.mode = OutputModeEnum.ResponsePlanMode;
                       return true;
                   }
                   return false;
               }

           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
              
           }


           return false;
       }


       public void setCMS_Dispaly(string devicename,OutputModeEnum mode, int ruleid, int priority, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors)
       {
           CMSOutputData cmsdata = new CMSOutputData(icon_id, g_code_id, hor_space, mesg, colors);
           OutputQueueData data = new OutputQueueData(this.deviceName,mode,ruleid, priority, cmsdata);
           this.SetOutput(data);
         //  output();
       }
       public void setCMS_Dispaly(string devicename,OutputModeEnum mode, int ruleid, int priority, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors,byte[]vspaces)
       {
           CMSOutputData cmsdata = new CMSOutputData(icon_id, g_code_id, hor_space, mesg, colors,vspaces);
           OutputQueueData data = new OutputQueueData(this.deviceName,mode, ruleid, priority, cmsdata);
           this.SetOutput(data);
           //  output();
       }

       

       public override void output()
       {
          
               OutputQueueData data = this.getOutputdata();
                  
           if(data==null)
            {
#if !DEBUG
                if (this.IsConnected)
               
                    ((RemoteInterface.MFCC.I_MFCC_CMS)this.getRemoteObj()).setDisplayOff(this.deviceName);
#endif
               return;
           }

               if (this.getRemoteObj() != null && this.IsConnected  /*this.getRemoteObj().getConnectionStatus(this.deviceName)*/)
               {
                   if (data == null || data.data == null)
                   {
#if !DEBUG
                       this.getRemoteObj().setDisplayOff(this.deviceName);
#endif
                   }
                   else
                   {
                       CMSOutputData cmddata = (CMSOutputData)data.data;
                       data.status = 1;
                       this.InvokeOutputDataStatusChange(data);
                       try
                       {
#if !DEBUG


                           if (cmddata.dataType == 0)
                               this.getRemoteObj().SendDisplay(this.deviceName, cmddata.icon_id, cmddata.g_code_id, cmddata.hor_space, cmddata.mesg, cmddata.colors);

                           else
                               this.getRemoteObj().SendDisplay(this.deviceName, cmddata.dataType, cmddata.icon_id, cmddata.g_code_id, cmddata.hor_space, cmddata.mesg, cmddata.colors);
#endif
                       }
                       catch (Exception ex)
                       {
                           data.IsSuccess = false;
                           this.InvokeOutputDataStatusChange(data);
                           Console.WriteLine(ex.Message + "," + ex.StackTrace);

                       }

                   }
               }
               else
               {
                   data.IsSuccess = false;
                   data.status = 2;
                  
                   this.InvokeOutputDataStatusChange(data);
               }
           
          
               
          // throw new Exception("The method or operation is not implemented.");
       }


       public new  RemoteInterface.MFCC.I_MFCC_CMS getRemoteObj()
       {
           return (RemoteInterface.MFCC.I_MFCC_CMS)base.getRemoteObj();
       }

       public void loadTravelSetting()
       {
           System.Collections.ArrayList ary = new System.Collections.ArrayList();
           System.Collections.ArrayList mainary = new System.Collections.ArrayList();
           OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
           OdbcDataReader rdMain, rd;
           OdbcCommand cmd = new OdbcCommand();
           OdbcCommand cmdMain = new OdbcCommand();
           cmd.Connection = cn;
           cmdMain.Connection = cn;
           try
           {

               cn.Open();
               cmdMain.CommandText = "select DISPLAY_PART ,g_code_id,message1,message2,msg1forecolor,msg2forecolor,msg1backcolor,msg2backcolor,uppertravelTime,lowerTravelTime,offset,enable from tblRGSTravelTime where devicename='" + this.deviceName + "'  and enable='Y' order by display_part";
               //if (deviceName == "CMS-T78-E-28.5")
               //    Console.WriteLine();
               rdMain = cmdMain.ExecuteReader();

               while (rdMain.Read())
               {
                   ary.Clear();
                   int displaypart = System.Convert.ToInt32(rdMain[0]);
                   int iconid = System.Convert.ToInt32(rdMain[1]);
                   string mesg1 = rdMain[2].ToString();
                   string mesg2 = rdMain[3].ToString();
                   string[] tmp = rdMain[4].ToString().Split(new char[] { ',' });
                   byte[] fcolor1 = new byte[mesg1.Length];

                  // if (mesg1.Length != 0) 
                   for (int i = 0; i < fcolor1.Length; i++)
                       fcolor1[i] = System.Convert.ToByte(tmp[i]);

                   tmp = rdMain[5].ToString().Split(new char[] { ',' });

                   byte[] fcolor2 = new byte[mesg2.Length];
                   for (int i = 0; i < fcolor2.Length; i++)
                       fcolor2[i] = System.Convert.ToByte(tmp[i]);


                   tmp = rdMain[6].ToString().Split(new char[] { ',' });
                   int upperTravelTime = System.Convert.ToInt32(rdMain[8]);
                   int lowerTraveltime = System.Convert.ToInt32(rdMain[9]);
                   int offset = System.Convert.ToInt32(rdMain[10]);
                   bool enable = rdMain[11].ToString().ToUpper() == "Y" ? true : false;

                   //byte[] bcolor1 = new byte[tmp.Length];
                   //for (int i = 0; i < bcolor1.Length; i++)
                   //    bcolor1[i] = System.Convert.ToByte(tmp[i]);

                   //tmp = rdMain[7].ToString().Split(new char[] { ',' });
                   //byte[] bcolor2 = new byte[tmp.Length];
                   //for (int i = 0; i < bcolor2.Length; i++)
                   //    bcolor2[i] = System.Convert.ToByte(tmp[i]);

                   #region read detail
                   cmd.CommandText = "select start_lineid, direction,  display_part,start_mileage,end_mileage, isXml from tblRgsTravelTimeDetail where devicename='" + this.deviceName + "'  and Display_part=" + displaypart;

                   rd = cmd.ExecuteReader();
                   while (rd.Read())
                   {
                       string lineid = rd[0].ToString();
                       string direction = rd[1].ToString();
                       int displaypartd = System.Convert.ToInt32(rd[2]);
                       int startmile = System.Convert.ToInt32(rd[3]);
                       int endmile = System.Convert.ToInt32(rd[4]);
                       bool isXml = (System.Convert.ToString(rd[5])=="Y")?true:false;;

                       ary.Add(new TravelDisplayDetailData(displaypartd, lineid, direction, startmile, endmile,isXml));
                   }
                   rd.Close();
                   TravelDisplayDetailData[] ddata = new TravelDisplayDetailData[ary.Count];
                   for (int i = 0; i < ddata.Length; i++)
                       ddata[i] = (TravelDisplayDetailData)ary.ToArray()[i];



                   #endregion

                   mainary.Add(new TravelDisplaySettingData(displaypart, iconid, mesg1, mesg2, fcolor1, fcolor2, null, null, ddata,upperTravelTime,lowerTraveltime,offset,enable));

               }
               rdMain.Close();
               travelDisplaySettingData = new TravelDisplaySettingData[mainary.Count];
               for (int i = 0; i < travelDisplaySettingData.Length; i++)
                   travelDisplaySettingData[i] = (TravelDisplaySettingData)mainary.ToArray()[i];


           }
           catch (Exception ex)
           {
               RemoteInterface.ConsoleServer.WriteLine(this.deviceName+ex.Message + ex.StackTrace);
           }
           finally
           {
               cn.Close();
           }





       }


       public TravelTimeData[] getTravelTimeData()
       {
           TravelTimeData[] data = new TravelTimeData[travelDisplaySettingData.Length];
           for (int i = 0; i < travelDisplaySettingData.Length; i++)
           {
               data[i] = new TravelTimeData(travelDisplaySettingData[i].traveltime, travelDisplaySettingData[i].upper, travelDisplaySettingData[i].lower);
           }

           return data;
       }

       public void DisplayTravelTime()
       {
           int alarmCode = 0;

           byte[] iconids = new byte[] { 0, 0 };
           string[] mesgs = new string[] { "", "", "", "" };
           byte[][] forecolors = new byte[4][] { new byte[0], new byte[0], new byte[0], new byte[0] };

#if DEBUG
           //if (this.deviceName != "CMS-N3-N-111.5")
           //{
           //    return;
           //}
#endif
           if (this.travelDisplaySettingData == null || travelDisplaySettingData.Length == 0)
           {
               this.removeOutput(OutputQueueData.TRAVEL_RULE_ID);
               return;
           }
           int travelTime = 0;
           bool[] IsValid = new bool[travelDisplaySettingData.Length];

           for (int i = 0; i < IsValid.Length; i++)
               IsValid[i] = true;
              for (int i = 0; i < travelDisplaySettingData.Length; i++) //calc  travel time
               {
                   travelTime = 0;
                   int upper = 0, lower = 0;

                   if (!travelDisplaySettingData[i].enable)
                       continue;



                   for (int j = 0; j < travelDisplaySettingData[i].detailData.Length; j++)
                   {
                       int sec = 0;
                       TravelDisplayDetailData ddata = travelDisplaySettingData[i].detailData[j];

                       if (!ddata.isXML)
                       {
                           sec = Program.matrix.getLine(ddata.lineid).getTravelTime(ddata.dir, ddata.startMile, ddata.endMile);
                           lower += Program.matrix.getLine(ddata.lineid).getLowerTravelTime(ddata.dir, ddata.startMile, ddata.endMile);
                           upper += Program.matrix.getLine(ddata.lineid).getUpperTravelTime(ddata.dir, ddata.startMile, ddata.endMile);
                       }
                       else
                       {
                           int t_lower=0, t_upper=0;
                           try
                           {

                               Program.matrix.timcc_section_mgr.getTravelDataByRange(ddata.lineid, ddata.dir, ddata.startMile, ddata.endMile, ref sec, ref t_upper, ref t_lower);
                               //modi 2012/6/5
                               if (sec <0)
                               {
                                   IsValid[i] = false;
                                   //t_lower = -1;
                                   //t_upper = -1;
                               }

                           }
                           catch 
                           {
                               ConsoleServer.WriteLine(this.deviceName+",timcc 旅行時間交換錯誤!");
                              //modu 2012/6/5
                               IsValid[i] = false;
                               sec = -1;
                               t_lower = -1;
                               t_upper = -1;
                           }
                           lower += t_lower;
                           upper += t_upper;

                       }

                     
                       if (sec < 0 || ! IsValid[i])   // exit when one of travel time invalid
                       {
                          this.removeOutput(OutputQueueData.TRAVEL_RULE_ID);
                           ConsoleServer.WriteLine(this.deviceName + "旅行時間無效");
                           IsValid[i] = false;
                           alarmCode = 2;
                           //this.setAlarmCode(2); //invalid
                           travelDisplaySettingData[i].traveltime = -1;
                           travelDisplaySettingData[i].lower = -1;
                           travelDisplaySettingData[i].upper = -1;
                           break;
                       }
                       else
                           travelTime += sec;
                   }  //for

                 

                   travelTime += travelDisplaySettingData[i].offset;
                   if (travelDisplaySettingData[i].lowerTravelTimeLimit != -1   &&  IsValid[i])
                       lower = travelDisplaySettingData[i].lowerTravelTimeLimit;
                   if (travelTime < lower  &&  IsValid[i]  )
                   {
                       travelTime = lower;
                    
                       ConsoleServer.WriteLine(this.deviceName + " 旅行時間低於下限!");
                      
                   }

                   if (travelDisplaySettingData[i].upperTravelTimeLimit != -1 && IsValid[i] )
                       upper = travelDisplaySettingData[i].upperTravelTimeLimit;
                   if (travelTime > upper && IsValid[i]  )
                   {
                       this.removeOutput(OutputQueueData.TRAVEL_RULE_ID);
                       ConsoleServer.WriteLine(this.deviceName + " 旅行時間" + travelTime + "高於於上限" + upper + "，熄滅!");
                       alarmCode = 3;
                      // this.setAlarmCode(3);
                       travelDisplaySettingData[i].traveltime = travelTime;
                       travelDisplaySettingData[i].lower = lower;
                       travelDisplaySettingData[i].upper = upper;
                       continue ;
                   }

                 
                  


                   travelTime =(int)Math.Ceiling(travelTime / 60.0);  //convert secods to minutes
                   iconids[travelDisplaySettingData[i].displaypart - 1] = (byte)travelDisplaySettingData[i].iconid;
                   mesgs[(travelDisplaySettingData[i].displaypart - 1) * 2] = travelDisplaySettingData[i].message1.Replace("@", (travelTime.ToString().Length == 1) ? " " + travelTime.ToString() : travelTime.ToString());
                   mesgs[(travelDisplaySettingData[i].displaypart - 1) * 2 + 1] = travelDisplaySettingData[i].message2.Replace("@", (travelTime.ToString().Length == 1) ? " " + travelTime.ToString() : travelTime.ToString());
                   forecolors[(travelDisplaySettingData[i].displaypart - 1) * 2] = Global.getColorBytesByPattern(travelDisplaySettingData[i].message1, "@", (travelTime.ToString().Length == 1) ? " " + travelTime.ToString() : travelTime.ToString(), travelDisplaySettingData[i].fcolorbyte1);
                   forecolors[(travelDisplaySettingData[i].displaypart - 1) * 2 + 1] = Global.getColorBytesByPattern(travelDisplaySettingData[i].message2, "@", (travelTime.ToString().Length == 1) ? " " + travelTime.ToString() : travelTime.ToString(), travelDisplaySettingData[i].fcolorbyte2);

                   travelDisplaySettingData[i].traveltime = travelTime * 60 ;
                   travelDisplaySettingData[i].lower = lower;
                   travelDisplaySettingData[i].upper = upper;

                 



               }  // for

              for (int i = 0; i < IsValid.Length; i++)
              {
                  if (!IsValid[i])
                  {
                      this.removeOutput(OutputQueueData.TRAVEL_RULE_ID);
                      return;
                  }
              }


               ConsoleServer.WriteLine(deviceName + "," + iconids[0] + "," + mesgs[0] + "," + mesgs[1]+","+mesgs[2]+","+mesgs[3]);
               //  ConsoleServer.WriteLine(deviceName + "," + "下," + iconids[1] + "," + mesgs[2] + "," + mesgs[3]);

               byte[] fbcolor = new byte[forecolors[0].Length + forecolors[1].Length+forecolors[2].Length+forecolors[3].Length];
               
               //System.Array.Copy(forecolors[0], fbcolor, forecolors[0].Length);
              // System.Array.Copy(forecolors[1], 0, fbcolor, forecolors[0].Length, forecolors[1].Length);
               int inx = 0;
               string msg = "";
               for (int i = 0; i < 4; i++)
               {   
                   System.Array.Copy(forecolors[i],0,fbcolor,inx, forecolors[i].Length);
                   inx += forecolors[i].Length;
                   if (i % 2 == 0) 
                       msg += mesgs[i]+"\r";
                   else   //奇數行
                   {
                       if (mesgs[i] != "")
                           msg += mesgs[i] + "\r";

                   }
               }

               msg = msg.Trim(new char[] { '\r' });


               if (alarmCode == 0)
                   this.setAlarmCode(1);
               else
                   this.setAlarmCode(alarmCode);


               this.setCMS_Dispaly(this.deviceName, OutputModeEnum.TravelMode, (int)OutputQueueData.TRAVEL_RULE_ID, (int)OutputQueueData.TRAVEL_PRIORITY,
               iconids[0], 0, 0, msg, fbcolor);
          

         
       }




   }

   
}

