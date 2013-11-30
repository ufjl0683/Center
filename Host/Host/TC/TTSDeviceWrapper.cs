using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;
using Host.TC;
using RemoteInterface;
using System.Data.Odbc;
using System.Drawing;
using RemoteInterface.MFCC;


namespace Host.TC
{
    public class TTSDeviceWrapper : OutPutDeviceBase
    {
        TravelDisplaySettingData[] travelDisplaySettingData;
        System.Timers.Timer tmr1Min = new System.Timers.Timer(1000 * 60);
     //   protected System.Collections.Hashtable[] TTSOutputQueue = new System.Collections.Hashtable[3];
        public TTSDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction)
           : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus, direction)
       {
           //for (int i = 0; i < 3; i++)
           //    TTSOutputQueue[i] = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
           loadTravelSetting();
           tmr1Min.Elapsed += new System.Timers.ElapsedEventHandler(tmr1Min_Elapsed);
           tmr1Min.Start();

       }

         volatile bool isInOneMin = false;
        void tmr1Min_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // throw new Exception("The method or operation is not implemented.");

            try
            {
                if (isInOneMin)
                    return;
                isInOneMin = true;
                if (Program.matrix.device_mgr != null && !Program.matrix.device_mgr.IsInLoadWrapper)
                    DisplayTravelTime();
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(this.deviceName + "," + ex.Message + ex.StackTrace);
            }
            finally
            {
                isInOneMin = false;
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

            byte[] iconids = new byte[] { 1, 2 ,3};
            string[] mesgs = new string[] { " ", " ", " " };
            byte[] forecolors = new byte[] { 255, 255, 255};
            bool[] isValid = new bool[] { true, true, true };
            int[] avitime = new int[] { 0, 0, 0 };

#if DEBUG
            //if (this.deviceName != "TTS-N1-S-248.2")
            //    return;
#endif

           
            if (this.travelDisplaySettingData == null || travelDisplaySettingData.Length == 0)
            {
                this.removeOutput(OutputQueueData.TRAVEL_RULE_ID);
                return;
            }
            int travelTime = 0;

            for (int i = 0; i < travelDisplaySettingData.Length; i++) //calc  travel time
            {
                travelTime = 0;
                int upper = 0, lower = 0;

                //if (!travelDisplaySettingData[i].enable)
                //{
                   
                //    continue;
                //}

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
                        }
                        lower += t_lower;
                        upper += t_upper;

                    }


                    if (sec < 0)   // exit when one of travel time invalid
                    {
                        this.removeOutput(OutputQueueData.TRAVEL_RULE_ID);
                        ConsoleServer.WriteLine(this.deviceName + "旅行時間無效");
                        isValid[i] = false;
                      //  this.setAlarmCode(2);
                        alarmCode = 2;
                        travelDisplaySettingData[i].traveltime = -1;
                        travelDisplaySettingData[i].lower = -1;
                        travelDisplaySettingData[i].upper = -1;
                        break ;
                    }
                    else
                        travelTime += sec;

                     int tmpsec=0;
                     if (!ddata.isXML)
                         tmpsec = Program.matrix.line_mgr[ddata.lineid].getAVI_TravelTime(ddata.dir, ddata.startMile, ddata.endMile);
                     else
                         tmpsec = -1;
                     if (tmpsec < 0)
                         avitime[i] = -1;
                     else if (avitime[i] != -1)
                         avitime[i] += tmpsec;

                }


                if (isValid[i])
                {
                    travelTime += travelDisplaySettingData[i].offset;
                    if (travelDisplaySettingData[i].lowerTravelTimeLimit != -1)
                        lower = travelDisplaySettingData[i].lowerTravelTimeLimit;
                    if (travelTime < lower)
                    {
                        travelTime = lower;
                        ConsoleServer.WriteLine(this.deviceName + " 旅行時間低於下限!");
                        travelDisplaySettingData[i].traveltime = travelTime;
                        travelDisplaySettingData[i].lower = lower;
                        travelDisplaySettingData[i].upper = upper;
                    }

                    if (travelDisplaySettingData[i].upperTravelTimeLimit != -1)
                        upper = travelDisplaySettingData[i].upperTravelTimeLimit;
                    if (travelTime > upper)
                    {
                        this.removeOutput(OutputQueueData.TRAVEL_RULE_ID);
                        ConsoleServer.WriteLine(this.deviceName + " 旅行時間" + travelTime + "高於於上限" + upper + "，熄滅!");
                        isValid[i] = false;
                       // this.setAlarmCode(3);
                        alarmCode = 3;
                        travelDisplaySettingData[i].traveltime = travelTime;
                        travelDisplaySettingData[i].lower = lower;
                        travelDisplaySettingData[i].upper = upper;
                        //continue;
                    }


                    travelTime = (int)Math.Ceiling(travelTime / 60.0);  //convert secods to minutes
                    iconids[travelDisplaySettingData[i].displaypart - 1] = (byte)travelDisplaySettingData[i].iconid;
                }
                if (isValid[i])
                    mesgs[travelDisplaySettingData[i].displaypart - 1] = travelDisplaySettingData[i].message1.Replace("@", travelTime.ToString().PadLeft(3, ' '));
                else
                    mesgs[travelDisplaySettingData[i].displaypart - 1] = "   ";
        
               // mesgs[(travelDisplaySettingData[i].displaypart - 1) * 2 + 1] = travelDisplaySettingData[i].message2.Replace("@", (travelTime.ToString().Length == 1) ? " " + travelTime.ToString() : travelTime.ToString());
                forecolors[travelDisplaySettingData[i].displaypart - 1] = travelDisplaySettingData[i].fcolorbyte1[0]; ; // Global.getColorBytesByPattern(travelDisplaySettingData[i].message1, "@", (travelTime.ToString().Length == 1) ? " " + travelTime.ToString() : travelTime.ToString(), travelDisplaySettingData[i].fcolorbyte1);
              //  forecolors[(travelDisplaySettingData[i].displaypart - 1) * 2 + 1] = Global.getColorBytesByPattern(travelDisplaySettingData[i].message2, "@", (travelTime.ToString().Length == 1) ? " " + travelTime.ToString() : travelTime.ToString(), travelDisplaySettingData[i].fcolorbyte2);

                if (travelDisplaySettingData[i].enable)
                {
                    iconids[i] = (byte)(i + 1);  // replace for boardid
                    travelDisplaySettingData[i].traveltime = travelTime;
                    travelDisplaySettingData[i].lower = lower;
                    travelDisplaySettingData[i].upper = upper;
                }
                //else
                //{
                //    iconids[i] = (byte)(i + 1);  // replace for boardid
                //    travelDisplaySettingData[i].traveltime = -1;
                //    travelDisplaySettingData[i].lower = lower;
                //    travelDisplaySettingData[i].upper = upper;
                //}
               
            }

          
            try
            {
                if (alarmCode != 0)
                    this.setAlarmCode(alarmCode);
                else
                    this.setAlarmCode(1);
                this.SetOutput(new OutputQueueData(this.deviceName, OutputModeEnum.TravelMode, OutputQueueData.TRAVEL_RULE_ID, OutputQueueData.TRAVEL_PRIORITY,
                    new TTSOutputData(iconids, mesgs, forecolors)));
            }
            catch (Exception ex)
            {
              //  ConsoleServer.
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }
         
            for (int i = 0; i < 3; i++)
            {
                ConsoleServer.WriteLine(deviceName+","+"board"+(i+1)+" 旅行時間"+ mesgs[i]+"分");
               

            }
            if (DateTime.Now.Minute % 5 == 0)
            {
                DateTime dt = DateTime.Now.AddMinutes(-DateTime.Now.Minute % 5);
               dt= dt.AddSeconds(-dt.Second);

                string sql = "update tblTTSTravelTimeCheck5Min set DIV1_OPERATIONTIME='{0}',DIV2_OPERATIONTIME='{1}',DIV3_OPERATIONTIME='{2}',Datavalidity='V'  where   devicename='{3}' and timestamp='{4}'";
                Program.matrix.dbServer.SendSqlCmd(string.Format(sql, mesgs[0], mesgs[1], mesgs[2], this.deviceName, DbCmdServer.getTimeStampString(dt)));
                sql = "update tblTTSTravelTimeCheck5Min set DIV1_AVITIME={0},DIV2_AVITIME={1},DIV3_AVITIME={2}  where   devicename='{3}' and timestamp='{4}'";

                for (int i = 0; i < 3; i++)
                    avitime[i] = (avitime[i] == 0) ? -1 : avitime[i];

                Program.matrix.dbServer.SendSqlCmd(string.Format(sql, avitime[0], avitime[1], avitime[2], this.deviceName, DbCmdServer.getTimeStampString(dt)));
            }

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
                cmdMain.CommandText = "select DISPLAY_PART ,g_code_id,message1,message2,msg1forecolor,msg2forecolor,msg1backcolor,msg2backcolor,uppertravelTime,lowerTravelTime,offset,enable from tblRGSTravelTime where devicename='" + this.deviceName + "' and enable='Y' order by display_part";
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
                    cmd.CommandText = "select start_lineid, direction,  display_part,start_mileage,end_mileage,isXml from tblRgsTravelTimeDetail where devicename='" + this.deviceName + "'  and Display_part=" + displaypart;

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

                    mainary.Add(new TravelDisplaySettingData(displaypart, iconid, mesg1, mesg2, fcolor1, fcolor2, null, null, ddata, upperTravelTime, lowerTraveltime, offset, enable));

                }
                rdMain.Close();
                travelDisplaySettingData = new TravelDisplaySettingData[mainary.Count];
                for (int i = 0; i < travelDisplaySettingData.Length; i++)
                    travelDisplaySettingData[i] = (TravelDisplaySettingData)mainary.ToArray()[i];


            }
            catch (Exception ex)
            {
                RemoteInterface.ConsoleServer.WriteLine(this.deviceName + ex.Message + ex.StackTrace);
            }
            finally
            {
                cn.Close();
            }









        }


        
#if DEBUG
      public override void output()
      {

          OutputQueueData data = this.getOutputdata();
      }
#else
       public override void output()
       {
           try
           {

               OutputQueueData data = this.getOutputdata();
               RemoteInterface.MFCC.I_MFCC_TTS ttsobj = (RemoteInterface.MFCC.I_MFCC_TTS)this.getRemoteObj();
               if (!ttsobj.getConnectionStatus(this.deviceName))
                   return;

               if (data == null || data.data == null)
               {
                   ttsobj.setDisplayOff(this.deviceName);
                   return;
               }

             //  OutputQueueData qdata = this.getOutputdata();
               TTSOutputData ttsdata = (TTSOutputData)data.data;
               for (int i = 0; i < ttsdata.boardid.Length; i++)
               {

                   if (ttsdata.color[i] != 255)
                       ttsobj.SendDisplay(this.deviceName, ttsdata.boardid[i], ttsdata.traveltime[i], ttsdata.color[i]);
                   else
                       ttsobj.setDisplayOff(this.deviceName, ttsdata.boardid[i]);

               }

           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message);
           }


          // throw new Exception("The method or operation is not implemented.");
       }
#endif

        public override OutputQueueData getOutputdata()
        {
            System.Collections.ArrayList ary = new System.Collections.ArrayList();
            byte[] boardid = new byte[]{1,2,3}; 
            string[] traveltime = new string[] { " ", " ", " " };
            byte[] colors = new byte[] { 255, 255, 255 };
            OutputQueueData ret;
           
            if (outputQueue.Count == 0)
                return null;
            else
            {

                System.Collections.IEnumerator ie = outputQueue.GetEnumerator();
                while (ie.MoveNext())
                {
                    OutputQueueData quedata = (OutputQueueData)((System.Collections.DictionaryEntry)ie.Current).Value;


                    ary.Add(quedata);
                   
                }
            }

            ary.Sort();
            if (ary.Count == 0)
                return null;
            else if (ary.Count == 1)
            {
             RemoteInterface.HC.TTSOutputData data   = (TTSOutputData)((OutputQueueData)ary[ary.Count - 1]).data;

                for (int i = 0; i < data.boardid.Length; i++)
                {
                    boardid[data.boardid[i] - 1] = data.boardid[i];
                    traveltime[data.boardid[i] - 1] = data.traveltime[i];
                    colors[data.boardid[i] - 1] = data.color[i];
                }

            }
            else
            {
              

                for (int qinx = ary.Count - 1; qinx >= 0; qinx--)
                {
                    RemoteInterface.HC.TTSOutputData data = (TTSOutputData)((OutputQueueData)ary[qinx]).data;
                    for (int i = 0; i < data.boardid.Length; i++)
                    {

                        if (colors[data.boardid[i] - 1] == 255 && data.color[i]!=255)
                        {
                            boardid[data.boardid[i] - 1] = data.boardid[i];
                            traveltime[data.boardid[i] - 1] = data.traveltime[i];
                            colors[data.boardid[i] - 1] = data.color[i];
                        }
                    }

                }
                //for (int i = 0; i < data.boardid.Length; i++)
                //{
                //    boardid[data.boardid[i] - 1] = data.boardid[i];
                //    traveltime[data.boardid[i] - 1] = data.traveltime[i];
                //    colors[data.boardid[i] - 1] = data.color[i];
                //}
                //data = (TTSOutputData)((OutputQueueData)ary[ary.Count - 1]).data;
                //for (int i = 0; i < data.boardid.Length; i++)
                //{
                //    boardid[data.boardid[i] - 1] = data.boardid[i];
                //    traveltime[data.boardid[i] - 1] = data.traveltime[i];
                //    colors[data.boardid[i] - 1] = data.color[i];
                //}
            }

            ret = new OutputQueueData(this.deviceName,((OutputQueueData)ary[ary.Count - 1]).mode, ((OutputQueueData)ary[ary.Count - 1]).ruleid, ((OutputQueueData)ary[ary.Count - 1]).priority
                , new TTSOutputData(boardid, traveltime, colors));
            return ret;
           // object[] data = ary.ToArray();
            //if (this.deviceName == "CMS-T78-W-41.6")
            //    Console.WriteLine("stop here");

          //  return (OutputQueueData)data[data.Length - 1];
        }

       
   }
}
