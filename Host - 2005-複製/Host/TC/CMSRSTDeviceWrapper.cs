using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;
using System.Data.Odbc;
using System.Drawing;
using RemoteInterface;

namespace Host.TC
{
   public  class CMSRSTDeviceWrapper:OutPutDeviceBase
    {
       TravelDisplaySettingData[] travelDisplaySettingData;
       System.Timers.Timer tmr1Min;
       public CMSRSTDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction)
          : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus, direction)
       {
           loadTravelSetting();
           tmr1Min = new System.Timers.Timer(1000 * 60);
           tmr1Min.Elapsed += new System.Timers.ElapsedEventHandler(tmr1Min_Elapsed);
           tmr1Min.Start();
       }

       void tmr1Min_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
       {
           try
           {
               if (Program.matrix.device_mgr != null && !Program.matrix.device_mgr.IsInLoadWrapper)
                DisplayTravelTime();
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(this.deviceName+","+ ex.Message + ex.StackTrace);
           }
           
           //throw new Exception("The method or operation is not implemented.");
       }


       public void setCMS_Dispaly(string devicename,OutputModeEnum mode, int ruleid, int priority, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors)
       {
           CMSOutputData cmsdata = new CMSOutputData(icon_id, g_code_id, hor_space, mesg, colors);
           OutputQueueData data = new OutputQueueData(this.deviceName,mode,ruleid, priority, cmsdata);
           this.SetOutput(data);
         //  output();
       }
       public void setCMS_Dispaly(string devicename, OutputModeEnum mode, int ruleid, int priority, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors,byte[]vspaces)
       {
           CMSOutputData cmsdata = new CMSOutputData(icon_id, g_code_id, hor_space, mesg, colors,vspaces);
           OutputQueueData data = new OutputQueueData(this.deviceName,mode, ruleid, priority, cmsdata);
           this.SetOutput(data);
           //  output();
       }

       
#if DEBUG
      public override void output()
      {
      }
#else
       public override void output()
       {
           OutputQueueData data = this.getOutputdata();
         
           if(this.getRemoteObj()!=null  && this.getRemoteObj().getConnectionStatus(this.deviceName))
           if (data==null || data.data == null )
           {
              // this.getRemoteObj().setDisplayOff(this.deviceName);
            
           }
           else
           {
               CMSOutputData cmddata = (CMSOutputData)data.data;
             //  if(cmddata.dataType==0)
               this.getRemoteObj().SendDisplay(0,this.deviceName, cmddata.g_code_id, cmddata.hor_space, cmddata.mesg, cmddata.colors);
                
               //else
               //this.getRemoteObj().SendDisplay(0,this.deviceName,cmddata.dataType, cmddata.g_code_id, cmddata.hor_space, cmddata.mesg, cmddata.colors);

           }
               
          // throw new Exception("The method or operation is not implemented.");
       }
#endif

       public new  RemoteInterface.MFCC.I_MFCC_CMSRST getRemoteObj()
       {
           return (RemoteInterface.MFCC.I_MFCC_CMSRST)base.getRemoteObj();
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

       public void DisplayTravelTime()
       {

           byte[] iconids = new byte[] { 0, 0 };
           string[] mesgs = new string[] { "", "", "", "" };
           byte[][] forecolors = new byte[4][] { new byte[0], new byte[0], new byte[0], new byte[0] };


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
                           }
                           catch 
                           {
                               ConsoleServer.WriteLine(this.deviceName+",timcc 旅行時間交換錯誤!");
                               sec = -1;
                               t_lower = -1;
                               t_upper = -1;
                           }
                           lower += t_lower;
                           upper += t_upper;

                       }

                     
                       if (sec < 0)   // exit when one of travel time invalid
                       {
                          this.removeOutput(OutputQueueData.TRAVEL_RULE_ID);
                           ConsoleServer.WriteLine(this.deviceName + "旅行時間無效");
                           IsValid[i] = false;
                           this.setAlarmCode(2); //invalid
                           travelDisplaySettingData[i].traveltime = -1;
                           travelDisplaySettingData[i].lower = -1;
                           travelDisplaySettingData[i].upper = -1;
                           break;
                       }
                       else
                           travelTime += sec;
                   }

                   travelTime += travelDisplaySettingData[i].offset;
                   if (travelDisplaySettingData[i].lowerTravelTimeLimit != -1)
                       lower = travelDisplaySettingData[i].lowerTravelTimeLimit;
                   if (travelTime < lower)
                   {
                       travelTime = lower;
                    
                       ConsoleServer.WriteLine(this.deviceName + " 旅行時間低於下限!");
                      
                   }

                   if (travelDisplaySettingData[i].upperTravelTimeLimit != -1)
                       upper = travelDisplaySettingData[i].upperTravelTimeLimit;
                   if (travelTime > upper)
                   {
                       this.removeOutput(OutputQueueData.TRAVEL_RULE_ID);
                       ConsoleServer.WriteLine(this.deviceName + " 旅行時間" + travelTime + "高於於上限" + upper + "，熄滅!");
                       this.setAlarmCode(3);
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

                   travelDisplaySettingData[i].traveltime = travelTime;
                   travelDisplaySettingData[i].lower = lower;
                   travelDisplaySettingData[i].upper = upper;

              



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





               this.setCMS_Dispaly(this.deviceName, OutputModeEnum.TravelMode, (int)OutputQueueData.TRAVEL_RULE_ID, (int)OutputQueueData.TRAVEL_PRIORITY,
               iconids[0], 0, 0, msg, fbcolor);
          

         
       }




   }

   
}

