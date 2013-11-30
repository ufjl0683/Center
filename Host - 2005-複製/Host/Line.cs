using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace Host
{
    public class Line
    {

        public string lineid,linename, direction;
        public int startmileage, endmileage;

        public System.Collections.Hashtable[] hsUnitRoads = new System.Collections.Hashtable[] { new System.Collections.Hashtable(), new System.Collections.Hashtable() };
        public System.Collections.Hashtable[] hsSections = new System.Collections.Hashtable[] { new System.Collections.Hashtable(), new System.Collections.Hashtable() };
        
        public Line(string lineid, string linename, string direction, int startmileage, int endmileage)
        {
            this.lineid = lineid;
            this.linename = linename;
            this.direction = direction;
            this.startmileage = startmileage;
            this.endmileage = endmileage;
            loadUnitRoad();
            loadSection();
        }

        public Section getSection(string dir, string sectionid)
        {
            return (Section)hsSections[direction.IndexOf(dir)][sectionid];
        }


        public Section getSectionByMile(string dir, int mile)
        {

            System.Collections.IDictionaryEnumerator ie = hsSections[direction.IndexOf(dir)].GetEnumerator();
            while (ie.MoveNext())
            {

                Section sec = (Section)ie.Value;
                if (sec.IsInSection(dir, mile))
                    return sec;
            }

           // return null;
            throw new Exception(this.lineid + "," + dir + "," + mile + " not found in sections collection!");

        }


        public System.Collections.IEnumerable getAllSectionEnum(string dir)
        {
            System.Collections.IDictionaryEnumerator ie = hsSections[direction.IndexOf(dir)].GetEnumerator();

            while (ie.MoveNext())
            {
                yield return ie.Value;
            }

        }

        public System.Collections.IEnumerable getAllUnitRoadEnum()
        {
            for (int i = 0; i < this.hsUnitRoads.Length; i++)
            {
                System.Collections.IDictionaryEnumerator ie = hsUnitRoads[i].GetEnumerator();

                while (ie.MoveNext())
                {
                    yield return ie.Value;
                }

            }
        }

        public System.Collections.IEnumerable getAllSectionEnum()
        {

            for (int i = 0; i < hsSections.Length; i++)
            {
                System.Collections.IDictionaryEnumerator ie = hsSections[i].GetEnumerator();

                while (ie.MoveNext())
                {
                    yield return ie.Value;
                }

            }

        }
        public void getAllTrafficData(string dir,int startMile,int endMile,ref int volume, ref int speed, ref int occpuancy, ref int jamlevel, ref int traveltime,ref int lowerTravelTime,ref int upperTravelTime)
        {

            int spd = 0, occ = 0, jmlvl = 0, ttime = 0, vol = 0;
            int begKm, endKm;
            int spdTotal = 0, occTotal = 0, jmlvlTotal = 0, voltotal = 0;
            int validCnt = 0;
            string[] refVDLst=null;
            if (startMile > endMile)
            {
                endKm = (startMile-1) / 1000;
                begKm = endMile / 1000;
            }
            else
            {
                begKm = startMile / 1000;
                endKm = (endMile-1) / 1000;
            }


            string location="";

            if(lineid.StartsWith("N"))
                location="F";
            else if(lineid.StartsWith("T"))
                location="H";
            else
                location="F";

            for (int i = begKm; i <= endKm; i++)
            {
                ((Host.UnitRoad)this.hsUnitRoads[this.direction.IndexOf(dir)][i]).getAllVDTrafficData(ref refVDLst,ref vol,ref spd, ref occ, ref jmlvl/*, ref ttime*/);
                //ttime 只計算完整的 unitroad 時間
               
                if (spd >= 0)
                {
                    spdTotal += spd;
                    occTotal += occ;
                    jmlvlTotal += jmlvl;
                    voltotal += vol;
                    
                   // ttimeTotal += 
                    validCnt++;
                }
                
            }

         

            if (validCnt == 0)
            {
              volume=  speed = occpuancy = jamlevel =  -1;

             // traveltime = this.getTravelTime(dir, startMile, endMile);   
              traveltime = -1;
         //    return;
            }
            else
            {
                speed = System.Convert.ToInt32(spdTotal / validCnt);
                occpuancy = System.Convert.ToInt32(occTotal / validCnt);
                //jamlevel = System.Convert.ToInt32(jmlvlTotal / validCnt);
                jamlevel = Program.matrix.vd_jam_eval.getLevel(location, speed, occpuancy);
                volume = System.Convert.ToInt32(voltotal / validCnt);
                traveltime = this.getTravelTime(dir, startMile, endMile);  // System.Convert.ToInt32(ttimeTotal / validCnt);
                
            }

            // 空間速度
            if (traveltime != -1)
            {
                speed = System.Convert.ToInt32(((double)System.Math.Abs(endMile - startMile) / traveltime * 3.6));

                jamlevel = Program.matrix.vd_jam_eval.getLevel(location, speed, occpuancy);
            }
            upperTravelTime = this.getUpperTravelTime(dir, startMile, endMile);
            lowerTravelTime = this.getLowerTravelTime(dir, startMile, endMile);




        }
      
        
        public int getSpaceSpeed(string dir, int startmile_m, int endmile_m)
        {

            
            int totalSec = 0;
         //   int totalvol = 0;
            int totalmeter = 0;
            bool IsInvalid = false;
            int tmp;
            if (startmile_m > endmile_m)  //swap  so startmile always < endmile
            {
                tmp = startmile_m;
                startmile_m = endmile_m;
                endmile_m = tmp;
            }

            endmile_m -= 1;
            for (int i = startmile_m / 1000; i <= endmile_m / 1000; i++)
            {
                int unitsec=0,unitvol=0, unitmeter=0;

                UnitRoad unit = this[dir, i];

                if (unit == null)
                    throw new Exception(this.lineid + "," + dir + i + "公里處無對應單位路段");

                unit.getVDSpaceData(ref unitvol, ref unitsec);
               // int tvol=0,tspeed=0,tocc=0,tjam=0;
              //  string [] vdlist=null;
             //   unit.getAllVDTrafficData(ref vdlist, ref tvol, ref tspeed, ref tocc, ref tjam);
             //   unitsec* unitvol;
                unitmeter=1000;
                if (unitsec >= 0)
                {
                    if (i == startmile_m / 1000)
                    {
                        unitsec = (int)(unitsec * ((i + 1) * 1000.0 - startmile_m) / 1000.0);
                        unitmeter=(int)(((i + 1) * 1000.0 - startmile_m));
                    }

                    else if (i == endmile_m / 1000)
                    {
                        unitsec = (int)(unitsec * (endmile_m - i * 1000) / 1000.0);
                        unitmeter=(int)((endmile_m - i * 1000));
                    }
                }
                else
                    IsInvalid = true;

                
                totalSec += unitsec*unitvol;
                totalmeter += unitmeter*unitvol;
               



            }



            if (totalSec == 0)
                return this.getSectionByMile(dir, startmile_m).maxSpeed;

            return (IsInvalid) ? -1 : System.Convert.ToInt32(totalmeter/ totalSec*3.6);






        }
        public int getTravelTime(string dir, int startmile_m,int endmile_m)
        {
            int totalSec = 0;
            bool IsInvalid = false;
            int tmp;
            if (startmile_m > endmile_m)  //swap  so startmile always < endmile
            {
                tmp = startmile_m;
                startmile_m = endmile_m;
                endmile_m = tmp;
            }

            endmile_m -= 1;
            for (int i = startmile_m / 1000; i <= endmile_m / 1000; i++)
            {
                int unitsec;

                UnitRoad unit = this[dir, i];

                if (unit == null)
                    throw new Exception(this.lineid + "," + dir + i + "公里處無對應單位路段");

                unitsec = unit.getTravelTime();
                if (unitsec >= 0)
                {
                    if (i == startmile_m / 1000)

                        unitsec = (int)(unitsec * ((i + 1) * 1000.0 - startmile_m) / 1000.0);

                    else if (i == endmile_m / 1000)

                        unitsec = (int)(unitsec * (endmile_m - i * 1000) / 1000.0);
                }
                else
                    IsInvalid = true;

                totalSec += unitsec;





            }

            return (IsInvalid) ? -1 : totalSec;
          
        }


        public int getLowerTravelTime(string dir, int startmile, int endmile)
        {
            //  Line line = (Line)line_mgr[lineid];
            int totalSec = 0;
            // bool IsInvalid = false;
            int tmp;
            if (startmile > endmile)  //swap  so startmile always < endmile
            {
                tmp = startmile;
                startmile = endmile;
                endmile = tmp;
            }
            endmile -= 1;
            for (int i = startmile / 1000; i <= endmile / 1000; i++)
            {
                int unitsec;
                UnitRoad unit = this[dir, i];
                unitsec = unit.getLowerTravelTimeLimit();
                //if (unitsec >= 0)
                //{
                if (i == startmile / 1000)

                    unitsec = (int)(unitsec * ((i + 1) * 1000.0 - startmile) / 1000.0);

                else if (i == endmile / 1000)

                    unitsec = (int)(unitsec * (endmile - i * 1000) / 1000.0);
                //}
                //else
                //    IsInvalid = true;

                totalSec += unitsec;



            }

            return totalSec;

        }
        public int getUpperTravelTime(string dir, int startmile, int endmile)
        {
            //  Line line = (Line)line_mgr[lineid];
            int totalSec = 0;
           // bool IsInvalid = false;
            int tmp;
            if (startmile > endmile)  //swap  so startmile always < endmile
            {
                tmp = startmile;
                startmile = endmile;
                endmile = tmp;
            }

            endmile -= 1;
            for (int i = startmile / 1000; i <= endmile / 1000; i++)
            {
                int unitsec;
                UnitRoad unit = this[dir, i];
                unitsec = unit.getUpperTravelTimeLimit();
                //if (unitsec >= 0)
                //{
                    if (i == startmile / 1000)

                        unitsec = (int)(unitsec * ((i + 1) * 1000.0 - startmile) / 1000.0);

                    else if (i == endmile / 1000)

                        unitsec = (int)(unitsec * (endmile - i * 1000) / 1000.0);
                //}
                //else
                //    IsInvalid = true;

                totalSec += unitsec;



            }

            return  totalSec;

        }


        public int getVD_TravelTime(string dir,int startmile_m,int endmile_m)
        {
          //  Line line = (Line)line_mgr[lineid];
            int totalSec = 0;
            bool IsInvalid = false;
            int tmp;
            if (startmile_m > endmile_m)  //swap  so startmile always < endmile
            {
                tmp = startmile_m;
                startmile_m = endmile_m;
                endmile_m = tmp;
            }

            endmile_m -= 1;
            for (int i = startmile_m / 1000; i <= endmile_m / 1000; i++)
            {
                int unitsec;
               
                    UnitRoad unit = this[dir, i];

                if(unit==null)
                    throw new Exception(this.lineid + "," + dir + i + "公里處無對應單位路段");
                    
                     unitsec = unit.getVD_TravelTime();
                if (unitsec >= 0)
                {
                    if (i == startmile_m / 1000)

                        unitsec = (int)(unitsec * ((i + 1) * 1000.0 - startmile_m) / 1000.0);

                    else if (i == endmile_m / 1000)

                        unitsec = (int)(unitsec * (endmile_m - i * 1000) / 1000.0);
                }
                else
                    IsInvalid = true;

                totalSec += unitsec;
           




            }

            return (IsInvalid) ? -1 : totalSec;

        }
        public int getAVI_TravelTime(string dir, int startmile_m, int endmile_m)
        {
            //  Line line = (Line)line_mgr[lineid];
            int totalSec = 0;
            bool IsInvalid = false;
            int tmp;
            if (startmile_m > endmile_m)  //swap  so startmile always < endmile
            {
                tmp = startmile_m;
                startmile_m = endmile_m;
                endmile_m = tmp;
            }

            endmile_m -= 1;
            for (int i = startmile_m / 1000; i <= endmile_m / 1000; i++)
            {
                int unitsec;

                UnitRoad unit = this[dir, i];

                if (unit == null)
                    throw new Exception(this.lineid + "," + dir + i + "公里處無對應單位路段");

                unitsec = unit.getAVI_TravelTime();
                if (unitsec >= 0)
                {
                    if (i == startmile_m / 1000)

                        unitsec = (int)(unitsec * ((i + 1) * 1000.0 - startmile_m) / 1000.0);

                    else if (i == endmile_m / 1000)

                        unitsec = (int)(unitsec * (endmile_m - i * 1000) / 1000.0);
                }
                else
                    IsInvalid = true;

                totalSec += unitsec;





            }

            return (IsInvalid) ? -1 : totalSec;

        }

        public int getETC_TravelTime(string dir, int startmile_m, int endmile_m)
        {
            //  Line line = (Line)line_mgr[lineid];
            int totalSec = 0;
            bool IsInvalid = false;
            int tmp;
            if (startmile_m > endmile_m)  //swap  so startmile always < endmile
            {
                tmp = startmile_m;
                startmile_m = endmile_m;
                endmile_m = tmp;
            }

            endmile_m -= 1;
            for (int i = startmile_m / 1000; i <= endmile_m / 1000; i++)
            {
                int unitsec;

                UnitRoad unit = this[dir, i];

                if (unit == null)
                    throw new Exception(this.lineid + "," + dir + i + "公里處無對應單位路段");

                unitsec = unit.getETC_TravelTime();
                if (unitsec >= 0)
                {
                    if (i == startmile_m / 1000)

                        unitsec = (int)(unitsec * ((i + 1) * 1000.0 - startmile_m) / 1000.0);

                    else if (i == endmile_m / 1000)

                        unitsec = (int)(unitsec * (endmile_m - i * 1000) / 1000.0);
                }
                else
                    IsInvalid = true;

                totalSec += unitsec;





            }

            return (IsInvalid) ? -1 : totalSec;

        }

        public int getHIS_TravelTime(string dir, int startmile_m, int endmile_m)
        {
            //  Line line = (Line)line_mgr[lineid];
            int totalSec = 0;
            bool IsInvalid = false;
            int tmp;
            if (startmile_m > endmile_m)  //swap  so startmile always < endmile
            {
                tmp = startmile_m;
                startmile_m = endmile_m;
                endmile_m = tmp;
            }

            endmile_m -= 1;
            for (int i = startmile_m / 1000; i <= endmile_m / 1000; i++)
            {
                int unitsec;

                UnitRoad unit = this[dir, i];

                if (unit == null)
                    throw new Exception(this.lineid + "," + dir + i + "公里處無對應單位路段");

                unitsec = unit.getHIS_TravelTime();
                if (unitsec >= 0)
                {
                    if (i == startmile_m / 1000)

                        unitsec = (int)(unitsec * ((i + 1) * 1000.0 - startmile_m) / 1000.0);

                    else if (i == endmile_m / 1000)

                        unitsec = (int)(unitsec * (endmile_m - i * 1000) / 1000.0);
                }
                else
                    IsInvalid = true;

                totalSec += unitsec;





            }

            return (IsInvalid) ? -1 : totalSec;
        }
        public void loadSection()
        {

              System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);

              try
              {
                  hsSections[0].Clear();
                  hsSections[1].Clear();
                  System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select lineid,sectionid,sectionName,direction,startmileage,endmileage,maxspeed,minspeed  from vwSection where lineid='" + this.lineid + "'");
                  cmd.Connection = cn;
                  cn.Open();
                  System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                  while (rd.Read())
                  {
                      string secname, secid, dir;
                      int startmile, endmile, maxspd, minspd;
                      dir = System.Convert.ToString(rd[3]);
                      secid = System.Convert.ToString(rd[1]);
                      secname = System.Convert.ToString(rd[2]);
                      startmile = System.Convert.ToInt32(rd[4]);
                      endmile = System.Convert.ToInt32(rd[5]);
                      maxspd = System.Convert.ToInt32(rd[6]);
                      minspd = System.Convert.ToInt32(rd[7]);
                      try
                      {
                          hsSections[this.direction.IndexOf(dir)].Add(secid, new Section(this, lineid, secid, secname, dir, startmile, endmile, maxspd, minspd));
                      }
                      catch (Exception ex)
                      {
                          ;
                      }
                  }

              }
              catch (Exception ex)
              {
                  ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
              }
              finally
              {
                  cn.Close();
              }


        }

        public UnitRoad this[string dir,int milkm]
        {

            get
            {
                return (UnitRoad)hsUnitRoads[direction.IndexOf(dir)][milkm];
            }
        }

        public void loadUnitRoad()
        {
            ConsoleServer.WriteLine("loading " + this.lineid);
            lock (this)
            {
                System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);

                try
                {
                    hsUnitRoads[0].Clear();
                    hsUnitRoads[1].Clear();
                    System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select direction,startmileage,endmileage,unitid  from tblGroupUnitRoad where lineid='" + this.lineid + "'");
                    cmd.Connection = cn;
                    cn.Open();
                    System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        string dir, unitid;
                        int startmil, endmil;
                        dir = System.Convert.ToString(rd[0]);
                        startmil = System.Convert.ToInt32(rd[1]);
                        endmil = System.Convert.ToInt32(rd[2]);
                        unitid = System.Convert.ToString(rd[3]);
                        hsUnitRoads[direction.IndexOf(dir)].Add(startmil, new UnitRoad(this,lineid,dir,unitid, startmil, endmil,(direction.IndexOf(dir)==0)?true:false));
                        
                    }
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                }
                finally
                {
                    cn.Close();
                }

            }
        }
    }
}
