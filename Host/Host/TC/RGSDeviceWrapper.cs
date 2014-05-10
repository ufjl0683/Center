using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using System.Drawing;
using RemoteInterface.HC;
using System.Data.Odbc;
using RemoteInterface;

namespace Host.TC
{
 public    class RGSDeviceWrapper:OutPutDeviceBase
    {


     TravelDisplaySettingData[] travelDisplaySettingData;
     System.Timers.Timer tmr1Min = new System.Timers.Timer(1000 * 60);

   
   //  System.Collections.Hashtable outputQueue = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());

     public RGSDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction)
         : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus, direction)
       {

         //  Console.WriteLine("load" + deviceName);
           loadTravelSetting();
           tmr1Min.Elapsed += new System.Timers.ElapsedEventHandler(tmr1Min_Elapsed);
           tmr1Min.Start();
       }

    

     void tmr1Min_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
     {
        // throw new Exception("The method or operation is not implemented.");
         try
         {
             if(  Program.matrix.device_mgr !=null && !Program.matrix.device_mgr.IsInLoadWrapper)
                     DisplayTravelTime();
         }
         catch (Exception ex)
         {
             ConsoleServer.WriteLine(this.deviceName+","+ex.Message + ex.StackTrace);
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
         
         byte []iconids=new byte[]{0,0};
         string [] mesgs=new string[]{"","","",""};
         Color[][] forecolors = new Color[4][] { new Color[0], new Color[0], new Color[0], new Color[0] };

#if DEBUG
         //if (this.deviceName == "RGS-N1-S-242.4")
         //    Console.WriteLine("check!");
#endif 
       
         if (this.travelDisplaySettingData == null || travelDisplaySettingData.Length == 0)
         {
             this.removeOutput(OutputQueueData.TRAVEL_RULE_ID);
             return;
         }
         int travelTime = 0;
         bool[] IsValid = new bool[travelDisplaySettingData.Length];

         for (int i=0;i<IsValid.Length;i++)
             IsValid[i] = true;
         for (int i = 0; i < travelDisplaySettingData.Length; i++) //calc  travel time
         {
             travelTime = 0;
              int upper = 0, lower = 0;
              if (!travelDisplaySettingData[i].enable)
              {
                 // travelDisplaySettingData
                  continue;
              }



             for (int j = 0; j < travelDisplaySettingData[i].detailData.Length; j++)
             {
                 int sec=0;
                
                 TravelDisplayDetailData ddata=travelDisplaySettingData[i].detailData[j];
                 if (!ddata.isXML)
                 {
                     sec = Program.matrix.getLine(ddata.lineid).getTravelTime(ddata.dir, ddata.startMile, ddata.endMile);
                     lower += Program.matrix.getLine(ddata.lineid).getLowerTravelTime(ddata.dir, ddata.startMile, ddata.endMile);
                     upper += Program.matrix.getLine(ddata.lineid).getUpperTravelTime(ddata.dir, ddata.startMile, ddata.endMile);
                 }
                 else
                 {
                     int t_lower = 0, t_upper = 0;
                     try
                     {
                         Program.matrix.timcc_section_mgr.getTravelDataByRange(ddata.lineid, ddata.dir, ddata.startMile, ddata.endMile, ref sec, ref t_upper, ref t_lower);
                     }
                     catch
                     {
                         ConsoleServer.WriteLine(this.deviceName + ",timcc 旅行時間交換錯誤!");
                         sec = -1;
                         t_lower = -1;
                         t_upper = -1;
                         // modi  on 2010/6/5
                         IsValid[i] = false;
                     }
                     lower += t_lower;
                     upper += t_upper;

                 }

                 if (sec < 0)   // exit when one of travel time invalid
                 {
                     ConsoleServer.WriteLine(this.deviceName + "旅行時間無效");
                     IsValid[i] = false;
                     //this.removeOutput(OutputQueueData.TRAVEL_RULE_ID);
                     //this.setAlarmCode(2);
                     alarmCode = 2;
                     travelDisplaySettingData[i].traveltime = -1;
                     travelDisplaySettingData[i].lower = -1;
                     travelDisplaySettingData[i].upper = -1;
                     break;
                 }
                 else
                     travelTime += sec;
             }

             if (IsValid[i])
             {
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
                    // this.setAlarmCode(3);
                     alarmCode = 3;
                     travelDisplaySettingData[i].traveltime = travelTime;
                     travelDisplaySettingData[i].lower = lower;
                     travelDisplaySettingData[i].upper = upper;
                     continue;
                 }

                 travelTime = (int)Math.Ceiling(travelTime / 60.0);  //convert secods to minutes
                 iconids[travelDisplaySettingData[i].displaypart - 1] = (byte)travelDisplaySettingData[i].iconid;
                 mesgs[(travelDisplaySettingData[i].displaypart - 1) * 2] = travelDisplaySettingData[i].message1.Replace("@", (travelTime.ToString().Length == 1) ? " " + travelTime.ToString() : travelTime.ToString());
                 mesgs[(travelDisplaySettingData[i].displaypart - 1) * 2 + 1] = travelDisplaySettingData[i].message2.Replace("@", (travelTime.ToString().Length == 1) ? " " + travelTime.ToString() : travelTime.ToString());
                 forecolors[(travelDisplaySettingData[i].displaypart - 1) * 2] = Global.getColorByPattern(travelDisplaySettingData[i].message1, "@", (travelTime.ToString().Length == 1) ? " " + travelTime.ToString() : travelTime.ToString(), travelDisplaySettingData[i].getForeColors1());
                 forecolors[(travelDisplaySettingData[i].displaypart - 1) * 2 + 1] = Global.getColorByPattern(travelDisplaySettingData[i].message2, "@", (travelTime.ToString().Length == 1) ? " " + travelTime.ToString() : travelTime.ToString(), travelDisplaySettingData[i].getForeColors2());

                 travelDisplaySettingData[i].traveltime = travelTime;
                 travelDisplaySettingData[i].lower = lower;
                 travelDisplaySettingData[i].upper = upper;
             }


         }

         ConsoleServer.WriteLine(deviceName + "," + "上," + iconids[0] + "," + mesgs[0] + "," + mesgs[1]);
         ConsoleServer.WriteLine(deviceName + "," + "下," + iconids[1] + "," + mesgs[2] + "," + mesgs[3]);
         if (alarmCode != 0)
             this.setAlarmCode(alarmCode);
         else
             this.setAlarmCode(1);

         this.SetTravelDisplay(iconids, mesgs, forecolors);


    
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
             cmdMain.CommandText = "select DISPLAY_PART ,g_code_id,message1,message2,msg1forecolor,msg2forecolor,msg1backcolor,msg2backcolor,uppertraveltime,lowertraveltime,offset,enable from tblRGSTravelTime where devicename='" + this.deviceName + "' and enable='Y'  order by display_part";
             rdMain = cmdMain.ExecuteReader();
           
             while (rdMain.Read())
             {
                 ary.Clear();
                 int displaypart = System.Convert.ToInt32(rdMain[0]);
                 int iconid = System.Convert.ToInt32(rdMain[1]);
                 string mesg1 = rdMain[2].ToString();
                 string mesg2 = rdMain[3].ToString();
                 string[] tmp = rdMain[4].ToString().Split(new char[] { ',' });
                 byte[] fcolor1=new byte[tmp.Length];
                 for (int i = 0; i < fcolor1.Length; i++)
                     fcolor1[i] = System.Convert.ToByte(tmp[i]);

                 tmp = rdMain[5].ToString().Split(new char[] { ',' });
                 byte[] fcolor2 = new byte[tmp.Length];
                 for (int i = 0; i < fcolor2.Length; i++)
                     fcolor2[i] = System.Convert.ToByte(tmp[i]);


                 tmp = rdMain[6].ToString().Split(new char[] { ',' });
                 byte[] bcolor1 = new byte[tmp.Length];
                 for (int i = 0; i < bcolor1.Length; i++)
                     bcolor1[i] = System.Convert.ToByte(tmp[i]);

                 tmp = rdMain[7].ToString().Split(new char[] { ',' });
                 byte[] bcolor2 = new byte[tmp.Length];
                 for (int i = 0; i < bcolor2.Length; i++)
                     bcolor2[i] = System.Convert.ToByte(tmp[i]);

                 int upperTravelTime = System.Convert.ToInt32(rdMain[8]);
                 int lowerTraveltime = System.Convert.ToInt32(rdMain[9]);
                 int offset = System.Convert.ToInt32(rdMain[10]);
                 bool enable = rdMain[11].ToString().ToUpper() == "Y" ? true : false;

              

                         #region read detail
                             cmd.CommandText = "select start_lineid, direction,  display_part,start_mileage,end_mileage,isXml from tblRgsTravelTimeDetail where devicename='" + this.deviceName + "'   and Display_part="+displaypart;

                             rd = cmd.ExecuteReader();
                             while (rd.Read())
                             {
                                 string lineid = rd[0].ToString();
                                 string direction = rd[1].ToString();
                                 int displaypartd = System.Convert.ToInt32(rd[2]);
                                 int startmile = System.Convert.ToInt32(rd[3]);
                                 int endmile = System.Convert.ToInt32(rd[4]);
                                 bool isXml = (System.Convert.ToString(rd[5]) == "Y") ? true : false; ;

                                 ary.Add(new TravelDisplayDetailData(displaypartd, lineid, direction, startmile, endmile,isXml));
                             }
                             rd.Close();
                             TravelDisplayDetailData[] ddata = new TravelDisplayDetailData[ary.Count];
                             for (int i = 0; i < ddata.Length; i++)
                                 ddata[i] = (TravelDisplayDetailData)ary.ToArray()[i];



                         #endregion

                     mainary.Add(new TravelDisplaySettingData(displaypart,iconid, mesg1, mesg2, fcolor1, fcolor2, bcolor1, bcolor2,ddata,upperTravelTime,lowerTraveltime,offset,enable));

             }
             rdMain.Close();
             travelDisplaySettingData = new TravelDisplaySettingData[mainary.Count];
             for (int i = 0; i < travelDisplaySettingData.Length; i++)
                 travelDisplaySettingData[i] = (TravelDisplaySettingData)mainary.ToArray()[i];
                    
            
          }
         catch (Exception ex)
         {
             RemoteInterface.ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
         }
         finally
         {
             cn.Close();
         }





     }

    
     //public void setGenericDisplay( RGS_GenericDisplay_Data data,int ruleid,int priority )
     //{


     //    EnOutputQueue(new TC.OutputQueueData(ruleid, priority, data));
     //    output();
     //}

     //public  new   I_MFCC_RGS getRemoteObj()
     //{
     //    return (I_MFCC_RGS)base.getRemoteObj();
     //}

     public override OutputQueueData getOutputdata()
     {
         //return base.getOutputdata();

        // int maxPriority = -1000, maxRuleid = -1000;

         System.Collections.ArrayList ary = new System.Collections.ArrayList();
        // RGS_GenericDisplay_Data MergeGData;
         if (outputQueue.Count == 0)
             return null;
         else
         {

             System.Collections.IEnumerator ie = outputQueue.GetEnumerator();
             while (ie.MoveNext())
             {
                 OutputQueueData quedata = (OutputQueueData)((System.Collections.DictionaryEntry)ie.Current).Value;


                 ary.Add(quedata);
                 //if (data.priority > maxPriority)
                 //{
                 //  //  mergeGerericDisplayData
                 //    maxPriority = data.priority;
                 //    maxRuleid = data.ruleid;
                 //}
             }
         }

         ary.Sort();
         object[] data = ary.ToArray();

         if (data.Length == 1)
             return data[0] as OutputQueueData;
         else
         {
             if (((data[0] as OutputQueueData).data as RGS_GenericDisplay_Data).mode != 2)
                 return data[0] as OutputQueueData;

            // System.Collections.ArrayList upperary = new System.Collections.ArrayList();
           //  System.Collections.ArrayList lowerary = new System.Collections.ArrayList();

             OutputQueueData upperData = null;
             OutputQueueData lowerData = null;
             OutputQueueData traveData = null;
             for (int i = data.Length-1; i >=0 ; i--)
             {
                 OutputQueueData qdata = data[i] as OutputQueueData;
                 RGS_GenericDisplay_Data rgsdata = qdata.data as RGS_GenericDisplay_Data;

                 if (rgsdata.mode != 2)
                     break;
                 if (rgsdata.msgs[0].y / 128 == 0)   //upper part
                 {    
                     if((data[i] as OutputQueueData).mode==  OutputModeEnum.TravelMode)
                         traveData =data[i] as OutputQueueData;

                     else if (upperData == null)
                         upperData = data[i] as OutputQueueData ;
                 }
                 else  //lower part
                 {
                      if(lowerData == null)
                          lowerData=data[i] as OutputQueueData;

                 }
              
                
             }


             if (upperData != null && lowerData != null)
                 return mergeOutputQueueData(upperData, lowerData);
             else if (upperData != null && lowerData == null)
             {
                 if (traveData == null)

                     return upperData;
                 else

                     return mergeOutputQueueData(upperData, traveData);
             }
             else if (upperData == null && lowerData != null)
             {
                 if (traveData == null)
                     return lowerData;
                 else
                     return mergeOutputQueueData(lowerData, traveData);
             }
             else if (data.Length > 0)  //直接輸出 都會路網 等非旅行時間輸出
                 return data[data.Length - 1] as OutputQueueData;
             else

                 return null;


         }

         //if(data.Length>1)
         //return  mergeOutputQueueData((OutputQueueData)data[data.Length-1],(OutputQueueData)data[data.Length-2]);
         //else
         //return (OutputQueueData)data[0];


          

       

     }


     private bool IsCmsModeOutput(int Displaypart,RGS_GenericDisplay_Data data)
     {
         bool ret = false;

         foreach (RGS_Generic_ICON_Data icon in data.icons)
             ret = ret || icon.y == (Displaypart - 1) * 128;
         if (ret)
             return ret;

         foreach (RGS_Generic_Message_Data msg in data.msgs)
             ret = ret ||( msg.y >= (Displaypart - 1) * 128  &&  msg.y < Displaypart*128);

         return ret;
     }

     protected override void EnOutputQueue(OutputQueueData data)
     {
         if (data.data != null)
         {
             RGS_GenericDisplay_Data rgsgendata = data.data as RGS_GenericDisplay_Data;
             if (data.mode == OutputModeEnum.ResponsePlanMode && rgsgendata.mode == 2)
             {
                 if (data.HappenLineID == this.lineid && data.HappenDir == this.direction)
                 {
                     if (rgsgendata.icons.Length > 0)

                         rgsgendata.icons[0].y = 0;

                     for (int i = 0; i < rgsgendata.msgs.Length; i++)
                     {
                         rgsgendata.msgs[i].y = (ushort)(i * 64);
                     }

                 }
                 else
                 {

                     if (rgsgendata.icons.Length > 0)

                         rgsgendata.icons[0].y = 128;

                     for (int i = 0; i < rgsgendata.msgs.Length; i++)
                     {
                         rgsgendata.msgs[i].y = (ushort)(i * 64+128);
                     }
                 }
             }


         }

         base.EnOutputQueue(data);
     }
     private OutputQueueData mergeOutputQueueData(OutputQueueData higher, OutputQueueData lower)  //合併RGS data
     {

         if (higher.data == null)
             return higher;
        // OutputQueueData retData = new OutputQueueData(higher.ruleid, higher.priority, higher.data);
         RGS_GenericDisplay_Data data1 = (RGS_GenericDisplay_Data)higher.data;
         RGS_GenericDisplay_Data data2 = (RGS_GenericDisplay_Data)lower.data;
         RGS_GenericDisplay_Data mergeData = new RGS_GenericDisplay_Data(data1.mode, data1.graph_code_id, data1.icons, data1.msgs, data1.sections);
         if (data1.mode != 2 || data2.mode!=2)  //cms mode
             return new OutputQueueData(this.deviceName,higher.mode,higher.ruleid, higher.priority,mergeData);


         System.Collections.ArrayList ary = new System.Collections.ArrayList();

         ary.Clear();

         for (int i = 0; i < data1.icons.Length; i++)
             ary.Add(data1.icons[i]);

         bool bfind = false;
      
         for (int i = 0; i < data2.icons.Length; i++)
         {
             
             bfind = false;
             foreach (RGS_Generic_ICON_Data icon in ary)
             {
                 if (icon.y == data2.icons[i].y)
                 {
                     bfind = true;
                     break;
                 }
                     
             }
             if (!bfind  && !IsCmsModeOutput(data2.icons[i].y/128+1,data1))
                 ary.Add(data2.icons[i]);


         }

         mergeData.icons = new RGS_Generic_ICON_Data[ary.Count];
         int inx = 0;
         foreach (RGS_Generic_ICON_Data icon in ary)
             mergeData.icons[inx++] = icon;




         ary.Clear();
        



         for (int i = 0; i < data1.msgs.Length; i++)
             ary.Add(data1.msgs[i]);

         for (int i = 0; i < data2.msgs.Length; i++)
         {
             bfind = false;
             foreach (RGS_Generic_Message_Data msg in ary)
             {
                 if (msg.y == data2.msgs[i].y)
                 {
                     bfind = true;
                     break;
                 }
             }

             if (!bfind && !IsCmsModeOutput(data2.msgs[i].y/128+1,data1))
                 ary.Add(data2.msgs[i]);

         }

         inx = 0;

         mergeData.msgs = new RGS_Generic_Message_Data[ary.Count];
       
         foreach (RGS_Generic_Message_Data msg in ary)
             mergeData.msgs[inx++] = msg;

         return new OutputQueueData(this.deviceName,higher.mode,higher.ruleid, higher.priority,mergeData);
     }
  


   public override  void output()
     {
       OutputQueueData data;
       
       data=this.getOutputdata();
       if (this.getRemoteObj() != null && this.getRemoteObj().getConnectionStatus(this.deviceName))
       {
           if (data == null || data.data == null)  //null 代表沒有輸出資料
           {
#if !DEBUG

               ((I_MFCC_RGS)this.getRemoteObj()).setGenericDisplay(this.deviceName, null);
#endif
           }
           else
           {
               RGS_GenericDisplay_Data gnrdata = (RGS_GenericDisplay_Data)data.data;

               //if(this.getRemoteObj()!=null && this.getRemoteObj().getConnectionStatus(deviceName))
               if (gnrdata.mode == 0) //都會路網
               {
                   foreach (RGS_Generic_Section_Data section in gnrdata.sections)
                   {
                       
                       section.status = (byte)Program.matrix.getRGSNetworkModeJamStatus(gnrdata.graph_code_id, section.section_id);
                       ConsoleServer.WriteLine(this.deviceName + "," + section.ToString());
                   }

                  
               } else if (gnrdata.mode == 1  && gnrdata.alarm_class==173  ) //路徑導引 T74 only
               {
                   //if (gnrdata.main_display_template == null || gnrdata.main_display_template == "" || gnrdata.opt_display_template == null || gnrdata.opt_display_template == "")
                   //    return;
                   try
                   {
                       int mainsec = Program.matrix.route_mgr74.GetMainRouteTravelTimes(this.deviceName);
                       int optsec = Program.matrix.route_mgr74.GetMainRouteTravelTimes(this.deviceName);
                       string mainmsg, optmsg;
                       int maininx, optinx;
                       maininx = gnrdata.main_display_template.IndexOf("@");
                       optinx = gnrdata.opt_display_template.IndexOf("@");

                       mainmsg = gnrdata.main_display_template.Replace("@", ((int)Math.Ceiling(mainsec / 60.0)).ToString());
                       optmsg = gnrdata.opt_display_template.Replace("@", ((int)Math.Ceiling(mainsec / 60.0)).ToString());
                       gnrdata.msgs[0].messgae = mainmsg;
                       gnrdata.msgs[0].backcolor = new Color[mainmsg.Length];
                       gnrdata.msgs[0].forecolor = new Color[mainmsg.Length];

                       for (int i = 0; i < mainmsg.Length; i++)
                           gnrdata.msgs[0].backcolor[i] = Color.Black;

                       for (int i = 0; i < mainmsg.Length; i++)
                           gnrdata.msgs[0].forecolor[i] = Color.Red;

                       for (int i = 0; i < mainmsg.Length - gnrdata.main_display_template.Length + 1; i++)
                           gnrdata.msgs[0].forecolor[maininx + i] = Color.Orange;

                       gnrdata.msgs[1].messgae = optmsg;
                       gnrdata.msgs[1].backcolor = new Color[optmsg.Length];
                       for (int i = 0; i < optmsg.Length; i++)
                           gnrdata.msgs[1].backcolor[i] = Color.Black;

                       for (int i = 0; i < optmsg.Length - gnrdata.opt_display_template.Length + 1; i++)
                           gnrdata.msgs[1].forecolor[optinx + i] = Color.Orange;

                       Util.SysLog("RedirectT74.log", DateTime.Now + "," + mainmsg + "," + optmsg);
                   }
                   catch (Exception ex)
                   {
                       Util.SysLog("RedirectT74.log", DateTime.Now+","+ ex.Message + "," + ex.StackTrace);
                   }
                  
               }

#if !DEBUG


               try
               {
                   data.status = 1;
                    this.InvokeOutputDataStatusChange(data);
                   ((I_MFCC_RGS)this.getRemoteObj()).setGenericDisplay(this.deviceName, (RGS_GenericDisplay_Data)data.data);
               }
               catch (Exception ex)
               {
                   data.status = 2;
                this.InvokeOutputDataStatusChange(data);
               }
#endif
           }
       }
       else  //disconnect
       {
           data.IsSuccess = false;
           data.status = 2;
           this.InvokeOutputDataStatusChange(data);
       }

         
     }



     public void SetTravelDisplay(byte[] iconids, string[] messages, Color[][] forecolors)
     {
         byte mode = 2;//cms mode
         byte iconcnt = 0;
         byte msgcnt = 0;




         for (int i = 0; i < iconids.Length; i++)
             if (iconids[i] != 0) iconcnt++;
         for (int i = 0; i < messages.Length; i++)
         {
             if (messages[i] != "") msgcnt++;

         }



         RGS_Generic_ICON_Data[] icons = new RGS_Generic_ICON_Data[iconcnt];
         byte inx = 0;
         for (int i = 0; i < iconids.Length; i++)
         {
             if (iconids[i] != 0)
                 icons[inx++] = new RGS_Generic_ICON_Data(iconids[i], 0, (ushort)(128 * i));
         }

         RGS_Generic_Message_Data[] msgs = new RGS_Generic_Message_Data[msgcnt];
         inx = 0;
         for (int i = 0; i < messages.Length; i++)
         {
             if (messages[i] != "")
             {
                 Color[] backcolor = new Color[messages[i].Length];
                 for (int j = 0; j < messages[i].Length; j++)
                     backcolor[j] = Color.Black;
                 int offset = -128;
                 for (int j = 0; j < icons.Length; j++)
                 {
                     if (icons[j].y / 128 == i / 2)
                     {
                         offset = 0;
                         break;
                     }
                 }
                 msgs[inx++] = new RGS_Generic_Message_Data(messages[i], forecolors[i], backcolor, (ushort)(128 + 4 + offset), (ushort)(64 * i));

             }

         }




         RGS_GenericDisplay_Data data = new RGS_GenericDisplay_Data(mode, 0, icons, msgs, new RGS_Generic_Section_Data[0]);

         this.SetOutput(new OutputQueueData(this.deviceName,OutputModeEnum.TravelMode,OutputQueueData.TRAVEL_RULE_ID, OutputQueueData.TRAVEL_PRIORITY, data));
     }




 }

   
   
}
