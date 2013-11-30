using System;
using System.Collections.Generic;
using System.Text;
using Host.TC;
using Host;
using System.Collections;
using RemoteInterface;
using RemoteInterface.HC;
namespace Host
{
   public class OutputDevicFetchManager
    {
       System.Collections.Hashtable lines = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
       

       public OutputDevicFetchManager(TC.DevcieManager devMgr)
       {

           loadMainLineOutDevice(devMgr);
       }


       public FetchDeviceData[] Fetch(string[] deviceTypes, string lineId, string direction,int mileage, int segCnt, int sysSegCnt, bool IsBranch)
       {
           Hashtable hsDevName = new Hashtable();
           System.Collections.ArrayList retlist = new ArrayList();
           if(!lines.Contains( lineId+"-"+direction))
               throw new Exception(lineId+"-"+direction+",not Found");

           
           if(sysSegCnt>0)
           {
             int seg_cnt=0, sys_seg_cnt=0;
               I_Positionable p = FindFirst_Position(lineId, direction, mileage);
               do
               {
                   if (p.getDevType() == "C")
                   {
                       sys_seg_cnt++;
                       seg_cnt++;
                   }
                   if (p.getDevType() == "I")
                       seg_cnt++;

                   p = p.getPrevDev();
               } while (p != null && sys_seg_cnt < sysSegCnt);

               if (seg_cnt > segCnt)
                   segCnt = seg_cnt;

           }


           foreach(string type in  deviceTypes)
           {
               ArrayList tmplist;
              tmplist= Search(type,FindFirst_Position(lineId,direction,mileage),segCnt,0,IsBranch);
              foreach (FetchDeviceData data in tmplist)
              {
                  if (!hsDevName.Contains(data.DevName))
                  {
                      hsDevName.Add(data.DevName, data);
                      retlist.Add(data);
                  }
              }
           }

           FetchDeviceData[] ret = new FetchDeviceData[retlist.Count];
         //  retlist.Sort();
           for (int i = 0; i < retlist.Count; i++)
               ret[i] = retlist[i] as FetchDeviceData;

           return ret;
              
       }
       public FetchDeviceData[] Fetch(string[] deviceTypes, string lineId, int startMileage, int endMileage)
       {

           if (startMileage == endMileage)
               throw new Exception("起訖範圍不能相等!");
            System.Collections.ArrayList []lineDevices=new ArrayList[2];
            if (System.Convert.ToInt32(lineId.Substring(lineId.Length - 1, 1)) % 2 == 0)
            {
                lineDevices[0] = lines[lineId + "-" + "E"] as System.Collections.ArrayList;
                lineDevices[1] = lines[lineId + "-" + "W"] as System.Collections.ArrayList;

            }
            else
            {
                lineDevices[0] = lines[lineId + "-" + "N"] as System.Collections.ArrayList;
                lineDevices[1] = lines[lineId + "-" + "S"] as System.Collections.ArrayList;

            }
            int temp;

            if (startMileage > endMileage)
            {
                temp = startMileage;
                startMileage = endMileage;
                endMileage = temp;
            }

           System.Collections.ArrayList retAry=new ArrayList();
           foreach (string devType in deviceTypes)
           {
               for (int i = 0; i < lineDevices.Length; i++)
               {
                   foreach (object obj in lineDevices[i])
                   {
                       if (obj is DeviceBaseWrapper)
                       {
                           DeviceBaseWrapper dev = obj as DeviceBaseWrapper;
                           if (dev.mile_m >= startMileage && dev.mile_m < endMileage && dev.deviceType == devType)
                               retAry.Add(dev);

                       }
                   }

               }
           }

            FetchDeviceData[] ret = new FetchDeviceData[retAry.Count];

            for (int i = 0; i < retAry.Count; i++)
            {
                DeviceBaseWrapper wraper=retAry[i] as DeviceBaseWrapper;
              
                ret[i] = new FetchDeviceData(wraper.deviceName, 0,wraper.lineid,wraper.direction,wraper.mile_m,
                    Program.matrix.line_mgr[lineId].getSectionByMile(wraper.direction,wraper.mile_m).maxSpeed,
                    Program.matrix.line_mgr[lineId].getSectionByMile(wraper.direction,wraper.mile_m).minSpeed);
            }

            return ret;

       }

       public FetchDeviceData[] Fetch(string[] deviceTypes, string lineId,string direction, int startMileage, int endMileage)
       {

           if (startMileage == endMileage)
               throw new Exception("起訖範圍不能相等!");
           System.Collections.ArrayList lineDevices;
           //if (System.Convert.ToInt32(lineId.Substring(lineId.Length - 1, 1)) % 2 == 0)
           //{
           //    lineDevices[0] = lines[lineId + "-" + "E"] as System.Collections.ArrayList;
           //    lineDevices[1] = lines[lineId + "-" + "W"] as System.Collections.ArrayList;

           //}
           //else
           //{
           //    lineDevices[0] = lines[lineId + "-" + "N"] as System.Collections.ArrayList;
           //    lineDevices[1] = lines[lineId + "-" + "S"] as System.Collections.ArrayList;

           //}
           lineDevices = lines[lineId + "-" + direction] as ArrayList;

           int temp;

           if (startMileage > endMileage)
           {
               temp = startMileage;
               startMileage = endMileage;
               endMileage = temp;
           }

           System.Collections.ArrayList retAry = new ArrayList();
           foreach (string devType in deviceTypes)
           {
               //for (int i = 0; i < lineDevices.Length; i++)
               //{
                   foreach (object obj in lineDevices)
                   {
                       if (obj is DeviceBaseWrapper)
                       {
                           DeviceBaseWrapper dev = obj as DeviceBaseWrapper;
                           if (dev.mile_m >= startMileage && dev.mile_m < endMileage && dev.deviceType == devType)
                               retAry.Add(dev);

                       }
                   }

               //}
           }

           retAry.Sort();

           FetchDeviceData[] ret = new FetchDeviceData[retAry.Count];

           for (int i = 0; i < retAry.Count; i++)
           {
               DeviceBaseWrapper wraper = retAry[i] as DeviceBaseWrapper;

               ret[i] = new FetchDeviceData(wraper.deviceName, 0, wraper.lineid, wraper.direction, wraper.mile_m ,
                   Program.matrix.line_mgr[lineId].getSectionByMile(wraper.direction,wraper.mile_m).maxSpeed,
                    Program.matrix.line_mgr[lineId].getSectionByMile(wraper.direction,wraper.mile_m).minSpeed);
           }

           return ret;

       }

       ArrayList Search(string devType, I_Positionable p, int totalSeg,int segid,bool IsBranch)
       {
           System.Collections.ArrayList list=new ArrayList();


           if ( p==null || segid>totalSeg )
               return list;

           ArrayList tmplst = null, branchlst1 = null, branchlst2 = null;
            if (p.getDevType() == "C" || p.getDevType() == "I")
            {
                tmplst = Search(devType, p.getPrevDev(), totalSeg, segid + 1,IsBranch);
                if (p.getDevType() == "C" && IsBranch)
                {
                    branchlst1 = Search(devType, ((InterSection)p).branch1, totalSeg, segid , false);
                    branchlst2 = Search(devType, ((InterSection)p).branch2, totalSeg, segid, false);
                }
            }
             else
            {
                tmplst = Search(devType, p.getPrevDev(), totalSeg, segid,IsBranch);

            }
            if (p.getDevType() == devType)
            {
                // will try catch here
                list.Add(new FetchDeviceData(p.getDevName(), segid, p.getLineID(), p.getDirection(), p.getMileage(),
                     Program.matrix.line_mgr[p.getLineID()].getSectionByMile(p.getDirection(), p.getMileage()).maxSpeed,
                     Program.matrix.line_mgr[p.getLineID()].getSectionByMile(p.getDirection(), p.getMileage()).minSpeed));
            }

           foreach (FetchDeviceData fdata in tmplst)
               list.Add(fdata);
            if(branchlst1!=null)
                foreach (FetchDeviceData fdata in branchlst1)
                    list.Add(fdata);

            if (branchlst2 != null)
                foreach (FetchDeviceData fdata in branchlst2)
                    list.Add(fdata);
           return list;

       }


       I_Positionable FindFirst_Position( string lineid, string direction, int mileage)
       {
           if (direction == "N" || direction == "W")
           {
               ArrayList list = lines[lineid + "-" + direction] as ArrayList;

               I_Positionable p = list[list.Count - 1] as I_Positionable;
               while (true)
               {
                   if (p == null) return null;
                   if (p.getMileage() > mileage)
                       return p;
                   else
                       p = p.getPrevDev();
               }


           }
           else  //S E
           {
               ArrayList list = lines[lineid + "-" + direction] as ArrayList;

               I_Positionable p = list[list.Count - 1] as I_Positionable;
               while (true)
               {
                   if (p == null) return null;
                   if (p.getMileage() < mileage)
                       return p;
                   else
                       p = p.getPrevDev();
               }

           }
       }
       
       private void loadMainLineOutDevice(TC.DevcieManager devMgr)
       {

           lines.Clear();

           foreach (DeviceBaseWrapper dev in devMgr.getOutputDeviceEnum())
           {
              
                   if (!lines.Contains(dev.lineid + "-" + dev.direction))
                   {
                       lines.Add(dev.lineid + "-" + dev.direction, ArrayList.Synchronized(new System.Collections.ArrayList()));
                      // lineJamRanges.Add(dev.lineid + "-" + dev.direction, ArrayList.Synchronized(new System.Collections.ArrayList()));
                   }

                   //if (dev.location == "F" || dev.location == "H" || dev.location == "T")
                   //{
                       ((ArrayList)lines[dev.lineid + "-" + dev.direction]).Add(dev);
                   //}

              
           }
           System.Collections.Hashtable hsInters = new Hashtable();
           System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
           System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("Select divisiontype,lineid1,direction1,mileage1,lineid2,direction2,mileage2 from vwcloverleaf");
           cmd.Connection = cn;
           try
           {
               cn.Open();
               System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
               while (rd.Read())
               {
                   try
                   {
                       string divisionType, lineid1, direction1, lineid2, direction2;
                       int mileage1, mileage2;
                       divisionType = rd[0].ToString();
                       lineid1 = rd[1].ToString();
                       direction1 = rd[2].ToString();
                       mileage1 = System.Convert.ToInt32(rd[3]);
                       if (divisionType == "C") //系統交流道
                       {
                           lineid2 = rd[4].ToString();
                           direction2 = rd[5].ToString();
                           mileage2 = System.Convert.ToInt32(rd[6]);
                       }
                       else  //匝道
                       {
                           lineid2 = lineid1;
                           direction2 = direction1;
                           mileage2 = mileage1;

                       }
                       InterSection intersec = new InterSection(divisionType, lineid1, direction1, mileage1, lineid2, direction2, mileage2);

                       if (!hsInters.Contains(intersec.getDevName()))
                       {
                           hsInters.Add(intersec.getDevName(), intersec);
                           try
                           {
                               ((ArrayList)lines[intersec.getLineID() + "-" + intersec.getDirection()]).Add(intersec);
                           }
                           catch (Exception ex2)
                           {
                               ConsoleServer.WriteLine(ex2.Message + "," + ex2.StackTrace);
                           }

                       }
                       else if (intersec.getDevType() == "C")
                       {
                           ((InterSection)hsInters[intersec.getDevName()]).BranchName2 = intersec.BranchName1;
                       }

                       //if (lines.Contains(lineid1 + "-" + direction1))
                       //{
                     

                       //}

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

           IDictionaryEnumerator iesec = hsInters.GetEnumerator();

           while (iesec.MoveNext())
           {
               try
               {
                   InterSection sec = (InterSection)iesec.Value;
                   if (sec.type == "C")
                   {
                       sec.branch1 = (InterSection)hsInters[sec.BranchName1];
                       sec.branch2 = (InterSection)hsInters[sec.BranchName2];
                   }
               }
               catch (Exception ex)
               {
                   ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
               }
           }

           IDictionaryEnumerator ie = lines.GetEnumerator();  //取得所有的路線
           while (ie.MoveNext())
           {
               ((ArrayList)ie.Value).Sort();   //排序所有的車輛偵測器

               ArrayList list = (ArrayList)ie.Value;
               //if (list.Count == 1)
               //    (vdlist[0] as DeviceBaseWrapper).AryInx = 0;

               // 填上前後車輛偵測器鏈結
              // if (list.Count > 1)
                   for (int i = 0; i < list.Count; i++)
                   {
                      // ((DeviceBaseWrapper)vdlist[i]).AryInx = i;

                       if (i == 0)
                           ((I_Positionable)list[i]).setNextDev( list[i + 1] as I_Positionable);
                       else if (i == list.Count - 1)
                           ((I_Positionable)list[i]).setPreDev( list[i - 1] as I_Positionable);
                       else
                       {
                           ((I_Positionable)list[i]).setPreDev( list[i - 1] as I_Positionable);
                           ((I_Positionable)list[i]).setNextDev( list[i + 1] as I_Positionable);
                       }


                   }

           }

           // oneMinTmr.Elapsed += new System.Timers.ElapsedEventHandler(oneMinTmr_Elapsed);
           // oneMinTmr.Start();
           //oneMinTmr = new System.Threading.Timer(new System.Threading.TimerCallback(oneMinTmr_Elapsed));
           //oneMinTmr.Change(0, 60 * 1000);

           ConsoleServer.WriteLine("設備收尋管理啟動完成!");
       }

    }
}
