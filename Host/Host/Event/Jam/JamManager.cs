using System;
using System.Collections.Generic;
using System.Text;
using Host.TC;
using System.Collections;
using RemoteInterface;
using System.Data.Odbc;
namespace Host.Event.Jam
{
   public  class JamManager
    {
       System.Threading.Timer oneMinTmr;
       DateTime lastJamCheckTime = new DateTime();
       int CheckState = 0;
       //每條路線的車輛偵測器集合
       System.Collections.Hashtable lines = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());

       //每條路線的壅塞集合 JamRamge
       System.Collections.Hashtable lineJamRanges = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
       System.Collections.Hashtable lineDegree2and1JamRanges = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());

       System.Collections.ArrayList rampVDDataList = System.Collections.ArrayList.Synchronized(new ArrayList());
       System.Collections.Hashtable rampVDJamRanges = System.Collections.Hashtable.Synchronized( new System.Collections.Hashtable());

       System.Collections.Hashtable htVDDevices = System.Collections.Hashtable.Synchronized(new Hashtable());

       public JamManager(DevcieManager devMgr)
       {
           loadMainLineVD( devMgr);
           loadRampVD( devMgr);
       }

       public int GetEventCnt()
       {
           return this.lineDegree2and1JamRanges.Count + lineJamRanges.Count + rampVDJamRanges.Count;
       }
       public IEnumerator getAllRamData()
       {
           foreach(RampVDData vddata in rampVDDataList)
               yield return vddata;
       }

       public Hashtable getLines()
       {
           return lines;
       }
       private void loadRampVD(DevcieManager devMg)
       {
           OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
#if !DEBUG
           OdbcCommand cmd =new OdbcCommand( "select degree_vd,location_r,direction,lineid,divisionid,divisionname,mileage,degree_lane,g_code_id,RGSDeviceName from vwrampDegree where location_r='O'  or location_r='I' ");
#else
           OdbcCommand cmd = new OdbcCommand("select degree_vd,location_r,direction,lineid,divisionid,divisionname,mileage,degree_lane,g_code_id,RGSDeviceName from vwrampDegree where location_r='O' ");

#endif

           try
           {
               cn.Open();
               cmd.Connection = cn;
               OdbcDataReader rd = cmd.ExecuteReader();
               while (rd.Read())
               {
                   try
                   {
                       string devname, location_r, lineid, divisionid, divisionname, divisiontype, direction,rgs_devicename;
                       int mile_m,laneid,g_code_id;
                       devname = rd[0].ToString();
                       location_r = rd[1].ToString();
                       direction = rd[2].ToString();
                       lineid = rd[3].ToString();
                       divisionid = rd[4].ToString();
                       divisionname = rd[5].ToString();
                     //  divisiontype = rd[6].ToString();
                       mile_m = System.Convert.ToInt32(rd[6]);
                       laneid = System.Convert.ToInt32(rd[7]);

                       if (!rd.IsDBNull(8))
                           g_code_id = System.Convert.ToInt32(rd[8]);  // for MetroNetwork
                       else
                           g_code_id = -1;


                       if (!rd.IsDBNull(9))
                           rgs_devicename = rd[9].ToString();
                       else
                           rgs_devicename = null;

                       RampVDData rvddata = new RampVDData(devname, lineid, direction, divisionid, divisionname, location_r,devMg[devname] as TC.VDDeviceWrapper,mile_m,laneid,g_code_id,rgs_devicename);
                      
                       this.rampVDDataList.Add(rvddata);

                       rvddata.OnEvent += new EventHandler(rvddata_OnEvent);
                      
                   
                   }
                   catch (Exception ex1)
                   {
                       ConsoleServer.WriteLine(ex1.Message + "," + ex1.StackTrace);
                   }

               }
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
           }
           finally
           {
               cn.Close();
           }

       }

       void rvddata_OnEvent(object sender, EventArgs e)
       {
          // throw new Exception("The method or operation is not implemented.");
           try
           {
               RampVDData vddata = sender as RampVDData;
               int jamLevel = 0;
               if (vddata.laneid == -1)
                   jamLevel = vddata.vd.jamLevel;
               else
                   jamLevel = vddata.laneJamLevel;

               if (!this.rampVDJamRanges.Contains(vddata.Key))
               {
                  
                   if (jamLevel == 0 || jamLevel == -1)
                       return;
                   RampJamRange range = new RampJamRange(vddata);

                   this.rampVDJamRanges.Add(vddata.Key, range);
                   Program.matrix.event_mgr.AddEvent(range);
               }
               else
               {
                   RampJamRange range = this.rampVDJamRanges[vddata.Key] as RampJamRange;
                   if (jamLevel == -1 || jamLevel == 0)
                   {


                       this.rampVDJamRanges.Remove(vddata.Key);
                       range.invokeStop();

                   }
                   //else
                   //{
                   //    range.invokeDegreeChange();
                   //}
               }
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
           }
       }

     

       private void loadMainLineVD (DevcieManager devMgr)
       {
           foreach (DeviceBaseWrapper dev in devMgr.getDataDeviceEnum())
           {
               if (dev is VDDeviceWrapper)
               {
                   if (!lines.Contains(dev.lineid + "-" + dev.direction))
                   {
                       lines.Add(dev.lineid + "-" + dev.direction, ArrayList.Synchronized(new System.Collections.ArrayList()));
                       lineJamRanges.Add(dev.lineid + "-" + dev.direction, ArrayList.Synchronized(new System.Collections.ArrayList()));
                       lineDegree2and1JamRanges.Add(dev.lineid + "-" + dev.direction, ArrayList.Synchronized(new System.Collections.ArrayList()));
                   }

                   if (dev.location == "F" || dev.location == "H" || dev.location == "T")
                   {
                       htVDDevices.Add(dev.deviceName, dev);
                        ((ArrayList)lines[dev.lineid + "-" + dev.direction]).Add(dev);
                   }

               }
           }

           IDictionaryEnumerator ie = lines.GetEnumerator();  //取得所有的路線
           while (ie.MoveNext())
           {
               ((ArrayList)ie.Value).Sort();   //排序所有的車輛偵測器

               ArrayList vdlist = (ArrayList)ie.Value;
               if (vdlist.Count == 1)
                   (vdlist[0] as DeviceBaseWrapper).AryInx = 0;

               // 填上前後車輛偵測器鏈結
               if (vdlist.Count > 1)
                   for (int i = 0; i < vdlist.Count; i++)
                   {
                       ((DeviceBaseWrapper)vdlist[i]).AryInx = i;

                       if (i == 0)
                           ((DeviceBaseWrapper)vdlist[i]).NextDevice = vdlist[i + 1] as DeviceBaseWrapper;
                       else if (i == vdlist.Count - 1)
                           ((DeviceBaseWrapper)vdlist[i]).PreDevice = vdlist[i - 1] as DeviceBaseWrapper;
                       else
                       {
                           ((DeviceBaseWrapper)vdlist[i]).PreDevice = vdlist[i - 1] as DeviceBaseWrapper;
                           ((DeviceBaseWrapper)vdlist[i]).NextDevice = vdlist[i + 1] as DeviceBaseWrapper;
                       }


                   }

           }

           // oneMinTmr.Elapsed += new System.Timers.ElapsedEventHandler(oneMinTmr_Elapsed);
           // oneMinTmr.Start();
           oneMinTmr = new System.Threading.Timer(new System.Threading.TimerCallback(oneMinTmr_Elapsed));
           oneMinTmr.Change(0, 60 * 1000);

           ConsoleServer.WriteLine("壅塞管理啟動完成!");
       }

      volatile bool isIn1min = false;


       void oneMinTmr_Elapsed(object  stat)
       {
           //throw new Exception("The method or operation is not implemented.");
           if (isIn1min)
           {
               ConsoleServer.WriteLine("Jam check task is busy!");

               return;
           }

           try
           {
               isIn1min = true;
               ConsoleServer.WriteLine("======In Check Jam Range Task ======");
             
#if DEBUG
          //     Program.matrix.getinitVd5minData();
#endif
               CheckState = 1;
               JamCheckTask();
               
               lastJamCheckTime = DateTime.Now;
#if DEBUG
              PrintJamStatus();
#endif
           }
           catch (Exception ex)
           {
               Util.SysLog("sys.log","Jam Check Task:" + ex.Message + "," + ex.StackTrace);
               ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
           }
           finally
           {
               isIn1min = false;
               CheckState = 0;
           }

           isIn1min = false;
           CheckState = 0;
       }

       void JamCheckTask()
       {
           IDictionaryEnumerator ie = lines.GetEnumerator();  //取得所有的路線
           while (ie.MoveNext())
           {
               try
               {
                   CheckState = 3;
                   CheckLineJam(ie.Key.ToString());
                   CheckState = 4;
                   CheckDegree1And2LineJam(ie.Key.ToString());
               }
               catch (Exception ex)
               {
                   Util.SysLog("sys.log", "In Jam Check" + ex.Message + "," + ex.StackTrace);
                   ConsoleServer.WriteLine(ex.Message+","+ex.StackTrace);
               }

           }

       }

       void JamCheckDegree1and2Task()
       {
           IDictionaryEnumerator ie = lines.GetEnumerator();  //取得所有的路線
           while (ie.MoveNext())
           {
               try
               {
                   CheckLineJam(ie.Key.ToString());

               }
               catch (Exception ex)
               {
                   Util.SysLog("sys.log", "In Jam Check" + ex.Message + "," + ex.StackTrace);
                   ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
               }

           }

       }
       

       void PrintJamStatus()
       {
           ConsoleServer.WriteLine(this.ToString());       
       }

       public override string ToString()
       {
           
           string ret = lastJamCheckTime+"\r\n";
           ret += "IsIn1Min:" + isIn1min + "\r\n";
           ret += "CheckState:" + CheckState + "\r\n";
           foreach (string linekey in lines.Keys)
           {
               ret += "==============================" + linekey + " jam range ===============\r\n";
               
               foreach (VDDeviceWrapper vd in (lines[linekey] as ArrayList))
               {
                   // string[] vdnamepart =vd.deviceName.Split(new char[]{'-'});
                   if (vd.jamLevel == 3)
                       ret += string.Format("{0}:{1}({2},{3},{4})\t", vd.mile_m, vd.jamLevel, vd.AvgVol, vd.AvgSpeed, vd.AvgOcc);
                   else
                       ret += vd.mile_m + ":" + vd.jamLevel + "\t";
               }
               ret += "\r\n";

               foreach (JamRange jr in lineJamRanges[linekey] as ArrayList)
               {
                   ret += "****" + jr.ToString() + "****\r\n";
               }
               ret += "\r\n";
           }


           return ret;
       }

       void CheckDegree1And2LineJam(string key)
       {
           ArrayList tmp = new ArrayList();
           ArrayList vdlist = (ArrayList)lines[key];
           ArrayList rangelist = (ArrayList)this.lineDegree2and1JamRanges[key];
           ArrayList rmlst = new ArrayList();

           //check 已存在的1,2級事件是否 degree change
           foreach (JamRange12 r in rangelist)
           {

               if (r.CheckDegre1and2eChange())
               {
                   if (r.getDegree() == 1 || r.getDegree() == 2)
                   {
                       r.invokeDegreeChange();
                   }
                   else if (r.getDegree() == 0 || r.getDegree() == 3 || r.getDegree()==-1)
                   {
                       r.invokeStop();
                      // rangelist.Remove(r);
                       rmlst.Add(r);
                   }
               }
               //if (r.Shrink())
               //    r.invokeRangeChange();
               //else if (r.DelMark)
               //    rmlst.Add(r);

               //event here

           }
           foreach( JamRange12 r in rmlst)
                 rangelist.Remove(r);

           rmlst.Clear();
           // check new JamEvent
           foreach (VDDeviceWrapper vd in vdlist)
           {

               if(vd.jamLevel==2 || vd.jamLevel==1)
               {
                   bool found = false;
                   foreach (JamRange12 r in rangelist)
                   {
                       if (r.DeviceName == vd.deviceName)
                       {
                           found = true;
                           break;
                       }
                   }
                   // 加入新事件
                   if (!found)
                   { 
                       JamRange12 range=new JamRange12(vd);
                       rangelist.Add(range);
                       Program.matrix.event_mgr.AddEvent(range);
                   }
               }
           }



       }



       void CheckLineJam(string key)
       {
            ArrayList tmp=new ArrayList();
            ArrayList  vdlist= (ArrayList) lines[key];
            ArrayList rangelist = (ArrayList)lineJamRanges[key];
            ArrayList rmlst = new ArrayList();

        // 壅塞範圍改變檢查

            //try
            //{
            //    if (System.IO.File.Exists(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "jam.log")))
            //        System.IO.File.Delete(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "jam.log"));
            //}
            //catch (Exception ex)
            //{
            //    ConsoleServer.WriteLine(ex.Message);
            //}
            foreach (JamRange r in rangelist)
            {

                try
                {
                    if (r.Shrink())
                        r.invokeRangeChange();
                    else if (r.DelMark)
                        rmlst.Add(r);
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                }

                //event here

            }

            CheckState = 5;

            foreach (JamRange r in rmlst)
            {
                try
                {
                    rangelist.Remove(r);
                }
                catch (Exception ex)
                {
                    Util.SysLog("sys.log", "In Jam Check" + ex.Message + "," + ex.StackTrace);
                    ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                }
                try
                {
                    r.invokeStop();
                }
                catch(Exception ex)
                {
                    Util.SysLog("sys.log", "In Jam Check" + ex.Message + "," + ex.StackTrace);
                   ConsoleServer.WriteLine(ex.Message+","+ex.StackTrace) ;
                }
            }


            rmlst.Clear();
//  檢查所有壅塞程度為 3 的車輛偵測器
            CheckState = 6;
            for (int i = 0; i < vdlist.Count; i++)
            {
                VDDeviceWrapper vd = (VDDeviceWrapper)vdlist[i];
                if (vd.jamLevel == 3)
                {
 
                    Util.SysLog("jam.log", vd.deviceName+" try to join\r\n");
 
                    bool hasContain=false;
                    for (int j = 0; j < rangelist.Count; j++)
                    {
                        try
                        {
                            JamRange range = (JamRange)rangelist[j];

                            if (!range.DelMark && range.IsInRange(i))
                            {
                                hasContain = true;


                                Util.SysLog("jam.log", vd.deviceName + " contain in " + range.ToString() + "\r\n");

                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Util.SysLog("jam.log", ex.Message + "," + ex.StackTrace);
                            ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                        }
                    }

                    if (!hasContain)
                    {



 
                        Util.Log("jam.log", vd.deviceName + " new to range \r\n");
 
                        tmp.Add(new JamRange(vd));

                    }

                }
            }

            CheckState = 7;
           // check 合併  tmp range
            rmlst.Clear();
            Util.SysLog("jam.log", "tmp count:"+tmp.Count+ "\r\n");
           // for (int i = 0; i < tmp.Count - 1; i++)
           // {
           //     JamRange rangei = tmp[i] as JamRange;
           //     if (rangei.DelMark)
           //         continue;
           //     for (int j = i + 1; j < tmp.Count  ; j++)
           //     {
           //         try
           //         {
           //             JamRange rangej = tmp[j] as JamRange;
           //             if (rangej.DelMark)
           //                 continue;
           //             if (rangei.Merge(rangej))
           //             {
           //                 rmlst.Add(rangej);
           //             }
           //         }
           //         catch (Exception ex)
           //         {
           //             Util.SysLog("jam.log", ex.Message + "," + ex.StackTrace);
           //         }

           //     }
           // }
           //foreach(JamRange r in rmlst)
           //{
           // tmp.Remove(r);
           //}
           //rmlst.Clear();

           CheckState = 71;

            foreach (JamRange tmprange in tmp.ToArray())  // 20130425 to array
            {
              //  bool isMerge = false;
                foreach(JamRange range in  rangelist.ToArray())  //20130425 to array
                {
                    try
                    {
                        if (range.Merge(tmprange)) //2013-3-22 加上 delmark 條件
                        {


                            Util.SysLog("jam.log", range.ToString() + "Merge" + tmprange.ToString() + "\r\n");

                            rmlst.Add(tmprange);
                            range.invokeRangeChange();


                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Util.SysLog("jam.log", ex.Message + "," + ex.StackTrace);
                        ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                    }
                }
            }

            CheckState = 8;
            foreach (JamRange r in rmlst)
            {
                tmp.Remove(r);
                //r.invokeAbort();
            }


          

           
            rmlst.Clear();
            CheckState = 9;
           // 現有的壅塞範圍合併
            for (int i = 0; i < rangelist.Count ; i++)
            {
                JamRange rangei = rangelist[i] as JamRange;
                if (rangei.DelMark)
                    continue;
                for (int j = 0; j < rangelist.Count; j++)
                {

                    JamRange rangej = rangelist[j] as JamRange;
                    if (i == j  || rangei.EventId==rangej.EventId)
                        continue;

                    if (rangej.DelMark)
                        continue;
                    try
                    {
                        if (rangei.IsContain(rangej))
                        {
                            rangej.DelMark = true;
                            continue;
                        }
                        else if (rangej.IsContain(rangei))
                        {
                            rangei.DelMark = true;
                            continue;
                        }
                            
                        

                        if (rangei.Merge(rangej))
                        {
                            //Event here
                            rangei.invokeRangeChange();

                            //rangei.DelMark = true;
                            //JamRange newrange = new JamRange(rangei.getDevList()[0] as VDDeviceWrapper);
                            //newrange.setDevList(rangei.getDevList());
                            //rangelist.Add(newrange);


                            //Program.matrix.event_mgr.AddEvent(newrange);

                        }
                    }
                    catch (Exception ex)
                    {
                        Util.SysLog("jam.log", ex.Message + "," + ex.StackTrace);
                        ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                    }

                }
            }
            CheckState = 10;
             foreach (JamRange r in rangelist )
                if (r.DelMark)
                {
                    rmlst.Add(r);
                   
                }

             CheckState = 11;
            foreach (JamRange r in rmlst)
            {
                //Event here
                try
                {
                    r.invokeAbort();

                    rangelist.Remove(r);
                }
                catch (Exception ex)
                {
                    Util.SysLog("jam.log", ex.Message + "," + ex.StackTrace);
                }
                
            }
           //加入新的壅塞範圍
            foreach (JamRange r in tmp)
            {
                foreach (JamRange range in rangelist)
                {
                    if (range.IsInRange(r.StartIndex))
                        r.DelMark = true;
                }
                if (!r.DelMark)
                {
                    rangelist.Add(r);

                    //add event mgr  here

                    Program.matrix.event_mgr.AddEvent(r);
                }
              
            }


            CheckState = 12;
           //壅塞範圍分割

            foreach (JamRange r in rangelist.ToArray())
            {
                 JamRange[] rs;
                 if ((rs = r.Split()) != null)
                {
                    try
                    {
                        CheckState = 121;
                        r.invokeAbort();
                        CheckState = 122;
                        rangelist.Remove(r);
                        CheckState = 123;
                        rangelist.Add(rs[0]);
                        CheckState = 124;
                        rangelist.Add(rs[1]);
                        CheckState = 125;

                        Program.matrix.event_mgr.AddEvent(rs[0]);
                        CheckState = 126;
                        Program.matrix.event_mgr.AddEvent(rs[1]);
                        CheckState = 127;
                    }
                    catch (Exception ex)
                    {
                        Util.SysLog("jam.log", ex.Message + "," + ex.StackTrace);
                        ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace );
                    }
                     //add event mgr  here

                }

            }
            CheckState = 13;

       }


       //public void DoVD_InteropDataTask()
       //{
       //}

       public void DoVD_InteropData(string devName, System.DateTime dt)
       {

           VDDeviceWrapper dev = htVDDevices[devName] as VDDeviceWrapper;
           VDDeviceWrapper predev = dev.PreDevice as VDDeviceWrapper;
           VDDeviceWrapper nextdev = dev.NextDevice  as VDDeviceWrapper;
           RemoteInterface.MFCC.VD1MinCycleEventData preData=null, nextData=null;
           if (predev != null && predev.IsConnected &&
               Program.matrix.line_mgr[predev.lineid].getSectionByMile(predev.direction, predev.mile_m).sectionId == Program.matrix.line_mgr[dev.lineid].getSectionByMile(dev.direction, dev.mile_m).sectionId)

               try
               {
                   preData = predev.getRemoteObj().getVDLatest1MinData(predev.deviceName);
               }
               catch { ;}
                

             if (nextdev != null && nextdev.IsConnected &&
               Program.matrix.line_mgr[nextdev.lineid].getSectionByMile(nextdev.direction, nextdev.mile_m).sectionId == Program.matrix.line_mgr[dev.lineid].getSectionByMile(dev.direction, dev.mile_m).sectionId)
                 try
                 {
                     nextData = nextdev.getRemoteObj().getVDLatest1MinData(nextdev.deviceName);
                 }
                 catch { ;}

             int vol=0, speed=0, occ=0;

             RemoteInterface.MFCC.VD1MinCycleEventData[] data = new RemoteInterface.MFCC.VD1MinCycleEventData[2] { preData, nextData };

            int cnt=0; 
           for (int i = 0; i < data.Length; i++)
             {

                 if (data[i] == null ||  !data[i].IsValid) continue;
                 if (System.DateTime.Now.Subtract(data[i].datatime).TotalMinutes > 10)
                     continue;
                 vol += data[i].vol;
                 speed += data[i].speed;
                 occ += data[i].occupancy;
                 
                 cnt++;
               
             }

             if (cnt == 0)
             {
                 Program.matrix.dbServer.SendSqlCmd("update tblVDData1min set datavalidity='I' where devicename='"+devName+"' and timestamp='"+DbCmdServer.getTimeStampString(dt)+"'");

                 return;
             }

             vol /= cnt;
             speed /= cnt;
             occ /= cnt;
             
           string sql="update tblVDData1min set Car_Volume={0},Car_Speed={1},Average_Occupancy={2},datavalidity='V',Utility=2 where devicename='{3}' and timestamp='{4}' ";
           Program.matrix.dbServer.SendSqlCmd(string.Format(sql, vol, speed, occ,devName,DbCmdServer.getTimeStampString(dt)));


        //   Util.SysLog("interop.log", predev.deviceName + "," + devName + "," + nextdev.deviceName + ",vol=" + vol + ",spd=" + speed + ",occ=" + occ);
         
       }


   }

   //class VDInterOpParam
   //{
   //    public string DevName;
   //    public DateTime dt;
   //    public VDInterOpParam(string DevName, System.DateTime dt)
   //    {
   //        this.DevName = DevName;
   //        this.dt = dt;
   //    }
   //}
}
