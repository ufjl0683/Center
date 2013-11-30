using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace Host
{
  public   class UnitRoad
    {
      public int startmileKM, endmileKM;
      public string unitid,lineid,direction;
      string[][] vd_travel_mapping_table;
      public bool IsForward;

      public Line line;
       
      public UnitRoad(Line line,string lineid, string direction, string unitid,int startmileKM, int endmileKM,bool bIsForward)
      {
          this.line = line;
          this.startmileKM = startmileKM;
          this.endmileKM = endmileKM;
          this.unitid = unitid;
          this.direction = direction;
          this.lineid = lineid;
          this.IsForward = bIsForward;
          load_vd_travel_mapping_table();

      }


      public int getTravelTime()
      {
          int vdtime=-1, avitime=-1, etctime=-1,histime=-1,weightid=0;
          int vdpercent=-1,avipercent=-1,etcpercent=-1,hispercent=-1;

          try
          {
              vdtime = getVD_TravelTime(-1,-1);
          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(this.section + " VD 取得時間模組錯誤!," + ex.Message);
              vdtime = -1;
          }

          try
          {
              avitime = getAVI_TravelTime();
          }
          catch(Exception ex)
          {
              ConsoleServer.WriteLine(this.section + " AVI 取得時間模組錯誤!,"+ex.Message);
              avitime = -1;
          }
          //try
          //{
          //    avitime = getAVI_TravelTime();
          //}
          //catch (Exception ex)
          //{
          //    ConsoleServer.WriteLine(this.section + " AVI 取得時間模組錯誤!," + ex.Message);
          //    avitime = -1;
          //}
          try
          {
              etctime = getETC_TravelTime();
          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(this.section + " ETC 取得時間模組錯誤!," + ex.Message);
              etctime = -1;
          }

          try
          {
              histime = getHIS_TravelTime();
          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(this.section + " His 取得時間模組錯誤!," + ex.Message+","+ex.StackTrace);
              histime = -1;
          }

          bool bThresholdTrigger = false;
          if(histime>0 && (vdtime >0 ||  avitime>0))
          {
              if (this.section.unit == 0)  // threshold unit sec
              {
                  if (vdtime > 0)
                      bThresholdTrigger = System.Math.Abs(vdtime - histime) > section.threshold;
                  if (avitime > 0)
                      bThresholdTrigger = bThresholdTrigger || System.Math.Abs(avitime - histime) > section.threshold;
              }
              else  //threshold unit percentage
              {
                  if (vdtime > 0)
                      bThresholdTrigger = System.Math.Abs(vdtime - histime)/vdtime > section.threshold;
                  if (avitime > 0)
                      bThresholdTrigger = bThresholdTrigger || System.Math.Abs(avitime - histime)/avitime > section.threshold;
              }
          }
          if (bThresholdTrigger)
          {
              histime = -1;
              Util.SysLog("unitthresholdtrigger.log", this.unitid.ToString());
          }

          weightid += (vdtime >= 0) ? 1 : 0;
          weightid += (avitime >= 0) ? 2 : 0;
          weightid += (etctime >= 0) ? 4 : 0;
          weightid += (histime >= 0) ? 8 : 0;

          if (weightid == 0)
              return -1;
          this.section.getTravelTimeWeight(weightid, ref vdpercent, ref avipercent, ref etcpercent, ref hispercent);

          if (vdpercent + avipercent + etcpercent + hispercent == 100)
              return (vdtime * vdpercent + avitime * avipercent + etctime * etcpercent+histime*hispercent) / 100;
          else
              return vdtime;
          
      }

      public Section section
      {
          get
          {
              return this.line.getSectionByMile(this.direction, this.startmileKM * 1000);
          }
      }
     public  int getAVI_TravelTime()
      {

          int speed = Program.matrix.avimgr.GetTrafficSpeed(this.lineid, this.direction,(int)( (this.startmileKM + this.endmileKM) / 2.0 * 1000));
          if (speed == -1 ) return -1;

          return 3600 / speed;
      }

      public int getETC_TravelTime()
      {

          int speed = Program.matrix.etcmgr.GetTrafficSpeed(this.lineid, this.direction, (int)((this.startmileKM + this.endmileKM) / 2.0 * 1000));
          if (speed == -1 || speed > 120) return -1;

          return 3600 / speed;
      }

      public int getHIS_TravelTime()
      {
          //int speed = Program.matrix.etcmgr.GetTrafficSpeed(this.lineid, this.direction, (int)((this.startmileKM + this.endmileKM) / 2.0 * 1000));

          int speed= this.section.getHIS_Speed(); ;
          if (speed == -1 || speed > 120 ||  speed==0) return -1;

          return 3600 / speed;
      }

      


      public int getLowerTravelTimeLimit()
      {
          try
          {
              Section sec = this.line.getSectionByMile(this.direction, this.startmileKM * 1000);

              return System.Convert.ToInt32(3600.0 / sec.maxSpeed);
          }
          catch (Exception ex)
          {

              throw new Exception(this.unitid + "找不到對應的路段," + ex.Message);
          }
      }
      public int getUpperTravelTimeLimit()
      {
          try
          {

                 Section sec=   this.line.getSectionByMile(this.direction, this.startmileKM * 1000);
            
                  return System.Convert.ToInt32(3600.0 /sec.minSpeed * 2.5);
           


          }
          catch (Exception ex)
          {
             
              throw new Exception(this.unitid+"找不到對應的路段,"+ex.Message);
          }
      }



      public void getVDSpaceData(ref int vol, ref int traveltime,int begmile,int endmile)
      {
          try
          {
              int[] speeds = null; ;
              int[] vols = null;
              for (int i = 0; i < vd_travel_mapping_table.Length; i++)
              {
                  speeds = new int[vd_travel_mapping_table[i].Length];
                  vols = new int[vd_travel_mapping_table[i].Length];
                  int invalidcnt = 0;
                  for (int j = 0; j < vd_travel_mapping_table[i].Length; j++)
                  {
                      try
                      {
                          speeds[j] = getSpeedFromVDDev(vd_travel_mapping_table[i][j]);
                          string devname = vd_travel_mapping_table[i][j];
                          if (!InRange(begmile, endmile, ((Host.TC.VDDeviceWrapper)Program.matrix.getDevicemanager()[devname]).mile_m))
                              speeds[j] = -1;
                      }
                      catch (Exception ex)
                      {
                          RemoteInterface.ConsoleServer.WriteLine(vd_travel_mapping_table[i][j] + ex.Message + ",以無效資料取代!");
                          speeds[j] = -1;
                      }
                      try
                      {
                          vols[j] = getVolFromVDDEV(vd_travel_mapping_table[i][j]);
                      }
                      catch (Exception ex)
                      {
                          RemoteInterface.ConsoleServer.WriteLine(vd_travel_mapping_table[i][j] + ex.Message + ",以無效資料取代!");
                          vols[j] = -1;
                      }
                      if (speeds[j] == -1 || vols[j]==-1) invalidcnt++;

                      // vd_travel_mapping_table


                  }
                  if (invalidcnt != vd_travel_mapping_table[i].Length)
                      break;
              }


              int totalspeed = 0, cnt = 0,totalvol=0;
              if (speeds == null || vols==null)
              {
                  RemoteInterface.ConsoleServer.WriteLine(unitid + "查無對應車輛偵測器資料");
                  vol = -1;
                  traveltime = -1;
                  return ;
              }
             
              for (int i = 0; i < speeds.Length; i++)
              {
                  if (speeds[i] != -1)
                  {
                      totalspeed += speeds[i];
                      totalvol += vols[i];
                      cnt++;
                  }
              }


              if (cnt == 0)
              {

                  vol = -1;
                  traveltime = -1;
                  return;
              }

              if (totalspeed == 0)
              {
                 traveltime=  System.Convert.ToInt32(3600.0 / this.line.getSectionByMile(this.direction, this.startmileKM * 1000).maxSpeed);  //指定速限上線
                  vol=1;
                    return;
              }
                traveltime=System.Convert.ToInt32(3600.0 / (totalspeed / cnt));
                vol=System.Convert.ToInt32((double)totalvol / cnt);
                return;
          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
              throw ex;
          }
      }

      public int getVD_TravelTime(int begmile,int endmile)    //sec
      {

          
          try
          {
              int[] speed = null; ;
              for (int i = 0; i < vd_travel_mapping_table.Length; i++)
              {
                  speed = new int[vd_travel_mapping_table[i].Length];
                  int invalidcnt = 0;
                  for (int j = 0; j < vd_travel_mapping_table[i].Length; j++)
                  {
                      try
                      {
                          speed[j] = getSpeedFromVDDev(vd_travel_mapping_table[i][j]);
                          string devname=vd_travel_mapping_table[i][j];
                          if (!InRange(begmile, endmile, ((Host.TC.VDDeviceWrapper)Program.matrix.getDevicemanager()[devname]).mile_m))
                              speed[j] = -1;

                      }
                      catch (Exception ex)
                      {
                          RemoteInterface.ConsoleServer.WriteLine(vd_travel_mapping_table[i][j] + ex.Message + ",以無效資料取代!");
                          speed[j] = -1;
                      }
                      if (speed[j] == -1) invalidcnt++;

                      // vd_travel_mapping_table


                  }
                  if (invalidcnt != vd_travel_mapping_table[i].Length)
                      break;
              }


              int totalspeed = 0, cnt = 0;
              if (speed == null)
              {
                  RemoteInterface.ConsoleServer.WriteLine(unitid + "查無對應車輛偵測器資料");
                  return -1;
              }
              for (int i = 0; i < speed.Length; i++)
              {
                  if (speed[i] != -1)
                  {
                      totalspeed += speed[i];
                      cnt++;
                  }
              }

              
              if (cnt == 0)
                  return -1;

              if (totalspeed == 0 ) return System.Convert.ToInt32(3600.0 / this.line.getSectionByMile(this.direction, this.startmileKM * 1000).maxSpeed);  //指定速限上線
              return System.Convert.ToInt32(3600.0 / (totalspeed / cnt));
          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
              throw ex;
          }
         
      }


      private bool InRange(int begmile, int endmile, int mile_m)
      {

          if (endmile == -1 || begmile == -1)
              return true;
          if (begmile > endmile)
          {
              int t = begmile;
              begmile = endmile;
              endmile = t;
          }

          return mile_m >= begmile && mile_m <= endmile;
      }
      
      public void  getAllVDTrafficData(ref string [] refVdList, ref  int volume, ref int speed,ref int occpuancy,ref int jamlevel /*,ref int traveltime*/,int begmile,int endmile)
      {
          int[] speeds = null;
          int [] occs=null;
          int[] jamlevels = null;
          int[] volumes = null; ;
          string[] refVds = null;
         
          for (int i = 0; i < vd_travel_mapping_table.Length; i++)
          {
              speeds = new int[vd_travel_mapping_table[i].Length];
              occs = new int[vd_travel_mapping_table[i].Length];
              jamlevels = new int[vd_travel_mapping_table[i].Length];
              refVds = new string[vd_travel_mapping_table[i].Length];
              volumes = new int[vd_travel_mapping_table[i].Length];
              int invalidcnt = 0;
              for (int j = 0; j < vd_travel_mapping_table[i].Length; j++)
              {

                  try
                  {
                     
                     
                      Host.TC.VDDeviceWrapper vdtc = ((Host.TC.VDDeviceWrapper)Program.matrix.getDevicemanager()[vd_travel_mapping_table[i][j]]);;

                      // mark on 2011/1/19
                      //speeds[j] =vdtc.AvgSpeed;
                      //occs[j] = vdtc.AvgOcc;
                      //refVds[j] = vd_travel_mapping_table[i][j];
                      //volumes[j] = vdtc.AvgVol;
                        //jamlevels[j] = (vdtc.IsValid) ? vdtc.jamLevel : -1;
                      if (vdtc.IsValid  && InRange(begmile,endmile,vdtc.mile_m ))
                      {
                          speeds[j] = vdtc.AvgSpeed;
                          occs[j] = vdtc.AvgOcc;
                          refVds[j] = vd_travel_mapping_table[i][j];
                          volumes[j] = vdtc.AvgVol;
                          jamlevels[j] = vdtc.jamLevel;
                      }
                      else
                      {
                          speeds[j] = -1;
                          occs[j] = -1;
                          refVds[j] = vd_travel_mapping_table[i][j];
                          volumes[j] =-1;
                          jamlevels[j] = -1;
                      }
                      

                  }
                  catch 
                  {
                      speeds[j] =-1;
                      occs[j] = -1;
                      refVds[j] = vd_travel_mapping_table[i][j];
                      volumes[j] = -1;
                      jamlevels[j] = -1;
                  }
                  if (speeds[j] == -1) invalidcnt++;

                  // vd_travel_mapping_table


              }
              if (invalidcnt != vd_travel_mapping_table[i].Length)
                  break;
          }



          int totalspeed = 0, totalocc = 0, totaljamlevel = 0, totalvolume = 0, cnt = 0;
          if (speeds == null)
          {
              RemoteInterface.ConsoleServer.WriteLine(unitid + "查無對應車輛偵測器資料");
              speed=-1;
              jamlevel=-1;
              occpuancy=-1;
             // traveltime=-1;
              volume = -1;
            //  lowerTravelTime=this.getLowerTravelTimeLimit();
            //  upperTraveltime = this.getUpperTravelTimeLimit();
              refVdList = new string[0];
              return;
          }
          for (int i = 0; i < speeds.Length; i++)
          {
              if (speeds[i] != -1)
              {
                  totalspeed += speeds[i];
                  totalocc+=occs[i];
                  totaljamlevel+=jamlevels[i];
                  totalvolume += volumes[i];
                  cnt++;
              }
          }

          if (cnt == 0) 
          {
               speed=-1;
              jamlevel=-1;
              occpuancy=-1;
            //  traveltime=-1;
            //  traveltime = this.getTravelTime();
              volume = -1;
            //  lowerTravelTime = this.getLowerTravelTimeLimit();
            //  upperTraveltime = this.getUpperTravelTimeLimit();
              refVdList = new string[0];
              return;
          }
          if (totalspeed == 0) 
          {
              speed = this.line.getSectionByMile(this.direction, this.startmileKM * 1000).maxSpeed;  //指定速限上線
              occpuancy=0;
              jamlevel=0;
              volume = 0;
              //traveltime = System.Convert.ToInt32(3600.0 / speed);
             // lowerTravelTime = this.getLowerTravelTimeLimit();
             // upperTraveltime = this.getUpperTravelTimeLimit();
              refVdList = refVds;
              return;
          }

          speed=System.Convert.ToInt32((double)totalspeed / cnt);
          occpuancy=System.Convert.ToInt32((double)totalocc / cnt);
          jamlevel=System.Convert.ToInt32((double)totaljamlevel / cnt);
         // traveltime= System.Convert.ToInt32(3600.0 / (totalspeed / cnt));
          volume = System.Convert.ToInt32((double)totalvolume/ cnt);
        //  lowerTravelTime = this.getLowerTravelTimeLimit();
        //  upperTraveltime = this.getUpperTravelTimeLimit();
          refVdList = refVds;
      }



      private int getSpeedFromVDDev(string devname)
      {
          try
          {
              Host.TC.VDDeviceWrapper vddev = ((Host.TC.VDDeviceWrapper)Program.matrix.getDevicemanager()[devname]);//.getCurrent5MinAvgData().speed;
              if (vddev.IsValid)
                  return vddev.AvgSpeed;
              else
                  return -1;
          }
          catch (Exception ex)
          {
              RemoteInterface.ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
              return -1;
          }

      }

      private int getOccFromVDDEV(string devName)
      {
          try
          {
              Host.TC.VDDeviceWrapper vddev = ((Host.TC.VDDeviceWrapper)Program.matrix.getDevicemanager()[devName]);//.getCurrent5MinAvgData().speed;
              if (vddev.IsValid)
                  return vddev.AvgOcc;
              else
                  return -1;
          }
          catch (Exception ex)
          {
              RemoteInterface.ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
              return -1;
          }
      }
      private int getVolFromVDDEV(string devName)
      {

          try
          {
              Host.TC.VDDeviceWrapper vddev = ((Host.TC.VDDeviceWrapper)Program.matrix.getDevicemanager()[devName]);//.getCurrent5MinAvgData().speed;
              if (vddev.IsValid)
                  return vddev.AvgVol;
              else
                  return -1;
          }
          catch (Exception ex)
          {
              RemoteInterface.ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
              return -1;
          }


      }

     public  void load_vd_travel_mapping_table()
      {
          string[][] newTable;
         
              System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
              try
              {
                  System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand(string.Format("select count(*) from tblGroupUnitVD where unitID='{0}' ", unitid));
                  cmd.Connection = cn;
                  cn.Open();
                  newTable = new string[System.Convert.ToInt32(cmd.ExecuteScalar())][];
                  cmd.CommandText = string.Format("select devicename1,DeviceName2,DeviceName3,DeviceName4 from tblGroupUnitVD where  unitid='{0}' order by priority", unitid);
                  System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                  int inx = 0;
                  while (rd.Read())
                  {
                      System.Collections.ArrayList ary = new System.Collections.ArrayList();

                      string devName1, devName2, devName3, devName4;
                      devName1 = System.Convert.ToString(rd[0]);
                      if (devName1 != "")
                          ary.Add(devName1);
                      devName2 = System.Convert.ToString(rd[1]);
                      if (devName2 != "")
                          ary.Add(devName2);

                      devName3 = System.Convert.ToString(rd[2]);
                      if (devName3 != "")
                          ary.Add(devName3);
                      devName4 = System.Convert.ToString(rd[3]);
                      if (devName4 != "")
                          ary.Add(devName4);
                      newTable[inx] = new string[ary.Count];
                      for (int i = 0; i < ary.Count; i++)
                          newTable[inx][i] = ary[i].ToString();


                      inx++;
                  }
                  vd_travel_mapping_table = newTable;
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

    }
}
