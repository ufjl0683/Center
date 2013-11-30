using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace Host
{
  public  class Section
  {
     public  string lineid, sectionId, sectionName, direction;
      public int startMileage,  endMileage, maxSpeed, minSpeed ;
      public Line line;

      HisTravelTimeManager hisTravelTime;
     
      System.Collections.ArrayList aryNormalDayWeight = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList());
      System.Collections.ArrayList aryHoliDayWeight = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList());
    
      public Section(Line line,string lineid,string sectionId,string sectionName,string direction,int startMileage, int endMileage,int maxSpeed,int minSpeed  )
      {
          this.line = line;
          this.lineid = lineid;
          this.sectionId = sectionId;
          this.sectionName = sectionName;
          this.direction = direction;
          this.startMileage = startMileage;
          this.endMileage = endMileage;
          this.maxSpeed = maxSpeed;
          this.minSpeed = minSpeed;
         
          LoadSectionTravelTimeWeight();
          hisTravelTime = new HisTravelTimeManager(this.sectionId);
      }

    

      public void LoadSectionTravelTimeWeight()
      {
          System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
          string sql = "";
          System.Data.Odbc.OdbcCommand cmd =new System.Data.Odbc.OdbcCommand();
          System.Data.Odbc.OdbcDataReader rd;
          cmd.Connection=cn;
      

          
          try
          {
              cn.Open();
              lock (aryNormalDayWeight)
              {
                  aryNormalDayWeight.Clear();
                  sql = "select SectionId,Start_Timestamp,End_Timestamp,WeightedId,Vd_Percentage,Avi_Percentage,Etc_Percentage,History_Percentage,Holiday from tblSectionTravelTimeWeight  where sectionid='{0}' and holiday={1} order by Start_Timestamp,WeightedId  ";
                  cmd.CommandText = string.Format(sql, this.sectionId, 0); // normalday 
                  rd = cmd.ExecuteReader();
                  DateTime dt=new DateTime();
                  SectionTravelTimeWeightSegnment seg = null;
                  while (rd.Read())
                  {
                      DateTime beg, end;
                      int weightid,vd_percent, avi_percent, etc_percent, his_percent;
                     
                      beg = System.Convert.ToDateTime(rd[1]);
                      end = System.Convert.ToDateTime(rd[2]);
                      weightid = System.Convert.ToInt32(rd[3]);
                      vd_percent = System.Convert.ToInt32(rd[4]);
                      avi_percent = System.Convert.ToInt32(rd[5]);
                      etc_percent = System.Convert.ToInt32(rd[6]);
                      his_percent = System.Convert.ToInt32(rd[7]);
                      if (beg != dt)
                      {
                          dt = beg;
                          seg = new SectionTravelTimeWeightSegnment(beg, end);
                          aryNormalDayWeight.Add(seg);
                      }
                      if (weightid < 16)
                      seg.AddWeight(weightid, vd_percent, avi_percent, etc_percent, his_percent);

                  }
                  rd.Close();
                  aryNormalDayWeight.Sort();
              }
              lock (aryHoliDayWeight)
              {
                  aryHoliDayWeight.Clear();
                  sql = "select SectionId,Start_Timestamp,End_Timestamp,WeightedId,Vd_Percentage,Avi_Percentage,Etc_Percentage,History_Percentage,Holiday from tblSectionTravelTime  where sectionid='{0}' and holiday={1} order by Start_Timestamp,WeightedId  ";
                  cmd.CommandText = string.Format(sql, this.sectionId, 1); // holiday
                  rd = cmd.ExecuteReader();
                  DateTime dt = new DateTime();
                  SectionTravelTimeWeightSegnment seg = null;
                  while (rd.Read())
                  {
                      DateTime beg, end;
                      int weightid, vd_percent, avi_percent, etc_percent, his_percent;
                   
                      beg = System.Convert.ToDateTime(rd[1]);
                      end = System.Convert.ToDateTime(rd[2]);
                      weightid = System.Convert.ToInt32(rd[3]);
                      vd_percent = System.Convert.ToInt32(rd[4]);
                      avi_percent = System.Convert.ToInt32(rd[5]);
                      etc_percent = System.Convert.ToInt32(rd[6]);
                      his_percent = System.Convert.ToInt32(rd[7]);
                      if (beg != dt)
                      {
                          dt = beg;
                          seg = new SectionTravelTimeWeightSegnment(beg, end);
                         aryHoliDayWeight.Add(seg);
                      }
                      if(weightid<16)
                      seg.AddWeight(weightid, vd_percent, avi_percent, etc_percent, his_percent);

                  }
                  rd.Close();
                  aryHoliDayWeight.Sort();
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

      public void getTravelTimeWeight(int weightid, ref int vd_percent,ref int avi_percent,ref int etc_percent,ref int his_percent)
      {
          System.Collections.ArrayList ary;
          if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
              ary = aryHoliDayWeight;
          else
              ary = aryNormalDayWeight;
          lock (ary)
          {
              DateTime dt = new DateTime(1900, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
              foreach (SectionTravelTimeWeightSegnment seg in ary)
              {
                  if (dt >= seg.begtime && dt <= seg.endtime)
                  {
                      vd_percent = seg.weights[weightid, 0];
                      avi_percent = seg.weights[weightid, 1];
                      etc_percent = seg.weights[weightid, 2];
                      his_percent = seg.weights[weightid, 3];
                      return;
                  }
              }

              vd_percent = 100;
              avi_percent = 0;
              etc_percent = 0;
              his_percent = 0;
          }

      }
      public bool IsInSection(string dir, int mileage)
      {
          int startMile=0, endMile = 0;
          if (this.startMileage > this.endMileage)
          {
              endMile = startMileage;
              startMile = endMileage;
          }
          else
          {
              startMile = startMileage;
              endMile=endMileage;
          }


          return dir == this.direction && (mileage >= startMile && mileage < endMile || (mileage + 1000) >= startMile && mileage + 1000 < endMile);
      }


      public void getAllTrafficData(ref int volume,ref int speed, ref int occpuancy, ref int jamlevel, ref int traveltime,ref int lowerTravelTime,ref int upperTravelTime)
      {


          this.line.getAllTrafficData(this.direction, this.startMileage, this.endMileage, ref volume, ref  speed, ref  occpuancy, ref jamlevel, ref  traveltime, ref lowerTravelTime,ref upperTravelTime);
        // traveltime = getTravelTime();

      }




      public int getVDSpaceSpeed()
      {
          return this.line.getSpaceSpeed(this.direction, this.startMileage, this.endMileage);
      }
  

      public int getVD_TravelTime()
      {
          return this.line.getVD_TravelTime(this.direction, this.startMileage, this.endMileage);
      }

      public int getAVI_TravelTime()
      {
          return this.line.getAVI_TravelTime(this.direction, this.startMileage, this.endMileage);
      }

      public int getETC_TravelTime()
      {
          return this.line.getETC_TravelTime(this.direction, this.startMileage, this.endMileage);
      }

      public int getHIS_TravelTime()
      {
          return this.hisTravelTime.getTravelTime();
      }
      
      public int  getHIS_Speed()
      {
          return hisTravelTime.getSpeed();
      }
      /*
     public int  getVDTravelTime()
     {
         int startmile = startMileage;
         int endmile = endMileage;

        // Line line = (Line)line_mgr[lineid];
         int totalSec = 0;
         bool IsInvalid = false;
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
             UnitRoad unit = line[direction, i];
             unitsec = unit.getVD_TravelTime();
             if (unitsec >= 0)
             {
                 if (i == startmile / 1000)

                     unitsec = (int)(unitsec * ((i + 1) * 1000.0 - startmile) / 1000.0);

                 else if (i == endmile / 1000)

                     unitsec = (int)(unitsec * (endmile - i * 1000) / 1000.0);
             }
             else
             {
                 IsInvalid = true;
                 continue;
             }

             totalSec += unitsec;



         }

         return (IsInvalid) ? -1 : totalSec;
     }
       * */

  }


    class HisTravelTimeManager
    {

      //  int _speed = -1, _traveltime = -1;
        int _classid = -1;
        int _gid = -1;
        string secid;

        System.Collections.Hashtable hsTravelData = new System.Collections.Hashtable();
        RemoteInterface.ExactIntervalTimer tmrEveryDay;// = new ExactIntervalTimer(0, 0, 10);
        public HisTravelTimeManager(string secid)
        {
            this.secid = secid;

            tmrEveryDay = new ExactIntervalTimer(0, 0, 10);
            tmrEveryDay.OnElapsed += new OnConnectEventHandler(tmrEveryDay_OnElapsed);
            tmrEveryDay_OnElapsed(null);
          
        }

        void tmrEveryDay_OnElapsed(object sender)
        {


            getClassId();
            getGID();

            loadHisTravelTime();
        }


        void getGID()
        {
            System.DateTime dt = System.DateTime.Today;
            string sql = "select gid from tblavitimegroup where  month={0}";
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand(string.Format(sql,DateTime.Now.Month));

            try
            {
                cn.Open();
                cmd.Connection = cn;
                _gid = System.Convert.ToInt32(cmd.ExecuteScalar());
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


        public int getSpeed()
        {
            System.DateTime dt = DateTime.Now;

               dt=  dt.Add(- ( dt - new DateTime(1900,1,1,dt.Hour,dt.Minute,dt.Second)));   
               dt= dt.AddSeconds(-dt.Second).AddMinutes( - dt.Minute % 5);
               
               if (!hsTravelData.Contains(dt))
                   return -1;

             return (hsTravelData[dt] as TravelData).Speed;

        }


        public int getTravelTime()
        {
            System.DateTime dt = DateTime.Now;
            dt = dt.Add(-(dt - new DateTime(1900, 1, 1, dt.Hour, dt.Minute, dt.Second)));   
            dt = dt.AddSeconds(-dt.Second).AddMinutes(-dt.Minute % 5);
            
            
            if (!hsTravelData.Contains(dt))
                return -1;
            return (hsTravelData[dt] as TravelData).TravelTime;

        }

        //public int getTravelTime(DateTime dt)
        //{
        //    dt = dt.AddSeconds(-dt.Second).AddMinutes(-dt.Minute % 5);

        //    if(! hsTravelData.Contains(dt))
        //        return -1;
        //    return ((TravelData)hsTravelData[dt]).TravelTime;
        //}

        //public int getSpeed(DateTime dt)
        //{
        //    dt = dt.AddSeconds(-dt.Second).AddMinutes(-dt.Minute % 5);

        //    if (!hsTravelData.Contains(dt))
        //        return -1;
        //    return ((TravelData)hsTravelData[dt]).Speed;
        //}


        void loadHisTravelTime()
        {
            System.DateTime dt = System.DateTime.Today;
            string sql = "select timestamp,speed,travelTime from tblTTSDataHistory where sectionid='{0}' and gid={1} and classid='{2}' ";
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand(string.Format(sql, this.secid,this._gid,this._classid));

            try
            {
                cn.Open();
                cmd.Connection = cn;
                System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    DateTime time = Convert.ToDateTime(rd[0]);
                    int speed = Convert.ToInt32(rd[1]);
                    int traveltime = Convert.ToInt32(rd[2]);

                    hsTravelData.Add(time, new TravelData(time, speed, traveltime));

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

        void getClassId()
        {

            System.DateTime dt = System.DateTime.Today;
            string sql="select classid from tblAnnualDate where  datecode='{0}'";
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
             System.Data.Odbc.OdbcCommand cmd=new System.Data.Odbc.OdbcCommand(string.Format(sql,DbCmdServer.getTimeStampString(dt)));

             try
             {
                 cn.Open();
                 cmd.Connection = cn;
                 _classid = System.Convert.ToInt32(cmd.ExecuteScalar());
             }
             catch (Exception ex)
             {
                 ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
             }
             finally
             {
                 cn.Close();
             }



          //  System.Data.Odbc.OdbcDataReader 

        }

        //public int Speed
        //{
        //    get { return _speed; }
        //    set { this._speed = value; }

        //}

        //public int TravelTime
        //{
        //    get { return _traveltime; }
        //    set { this._traveltime = value; }

        //}


       
    }

    public class TravelData
    {
        DateTime _dt;
        int _speed, _traveltime;
        
        public TravelData(DateTime dt, int speed, int traveltime)
        {
            this._speed = speed;
            this._traveltime = traveltime;
            _dt = dt;
        }

        //DateTime dt
        //{
        //    get {return  _dt; }
        //}
     public   int Speed
        {
            get { return _speed; }
        }
      public  int TravelTime
        {
            get
            {
                return _traveltime;
            }
        }

        DateTime Key
        {
            get { return _dt; }
        }
        

    }

}
