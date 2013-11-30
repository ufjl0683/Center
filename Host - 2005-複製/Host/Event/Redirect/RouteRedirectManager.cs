using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using RemoteInterface;

namespace Host.Event.Redirect
{
  public   class RouteRedirectManager
    {
      internal static  RGSRedirectLevelTable redir_level_table = new RGSRedirectLevelTable();

      System.Collections.Generic.Dictionary<string, RedirectSettingGroup> RedirectSettingGroups = new Dictionary<string, RedirectSettingGroup>();

      System.Collections.Hashtable hsEvent = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
    
      public RouteRedirectManager()
      {

         
          loadRecirectLevelTable();
          loadAllRoutSetting();

      }
      public void loadRecirectLevelTable()
      {
          redir_level_table.loadTable();

      
      }


      
      public void loadAllRoutSetting()
      {
          OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
          OdbcCommand cmd = new OdbcCommand("select devicename,display_part,start_lineid,direction,start_mileage,end_mileage,isxml from tblcomparetraveltimedetail");
          cmd.Connection = cn;
          OdbcDataReader rd = null;

          try
          {
              RedirectSettingGroups.Clear();

              cn.Open();
              rd = cmd.ExecuteReader();
              while (rd.Read())
              {
                  string devname,direction,lineid;

                  int display_part, start_mileage, end_mileage;
                  bool isxml = false;


                  devname = rd[0].ToString();
                  display_part = System.Convert.ToInt32(rd[1]);
                  lineid = rd[2].ToString();
                  direction = rd[3].ToString();
                  start_mileage = System.Convert.ToInt32(rd[4]);
                  end_mileage = System.Convert.ToInt32(rd[5]);
                  isxml = (rd[6].ToString().Trim() == "Y") ? true : false;
                  RedirectSettingGroup grp=  this.AddRoutingSetting(devname, display_part, lineid, direction, start_mileage, end_mileage, isxml);

                  if (grp != null)
                  {
                      grp.OnEvent += new EventHandler(grp_OnEvent);
                     // grp.OnEventStart += new EventHandler(grp_OnEventStart);
                  }

              }
              rd.Close();
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

      //void grp_OnEventStart(object sender, EventArgs e)
      //{
      //    RedirectSettingGroup group = sender as RedirectSettingGroup;

      //    try
      //    {
      //        RouteRedirectRange evt = new RouteRedirectRange(group);
      //        hsEvent.Add(group.devName,evt);

      //        evt.OnAbortToManager += new EventHandler(evt_OnAbortToManager);
      //      //  Program.matrix.event_mgr.AddEvent(range);
      //        Program.matrix.event_mgr.AddEvent(evt);

      //    }
      //    catch (Exception ex)
      //    {

      //        ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
      //        Util.SysLog("evterr.txt", ex.Message + "," + ex.StackTrace);
      //    }
          
          //throw new Exception("The method or operation is not implemented.");
      //}

      void evt_OnAbortToManager(object sender, EventArgs e)
      {
        //  throw new Exception("The method or operation is not implemented.");
          
          hsEvent.Remove((sender as RouteRedirectRange).devName);
      }



      void grp_OnEvent(object sender, EventArgs e)
      {

          RedirectSettingGroup group = sender as RedirectSettingGroup;

          try
          {

              if (!this.hsEvent.Contains(group.devName))
              {
                  if (group.Degree == 0 || group.Degree == -1)
                      return;
                  RouteRedirectRange evt = new RouteRedirectRange(group);
                  hsEvent.Add(group.devName, evt);
                  Program.matrix.event_mgr.AddEvent(evt);

              }
              else  //已存在事件
              {
                  Event evt = hsEvent[group.devName] as Event;
                  if (group.Degree == 0 || group.Degree==-1)
                  {

                      evt.invokeStop();
                      
                      hsEvent.Remove(group.devName);

                  }
                  else
                  {
                      evt.invokeDegreeChange();
                    
                  }
              }
          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
              Util.SysLog("evterr.txt", ex.Message + "," + ex.StackTrace);
          }

          //else

         
          //throw new Exception("The method or operation is not implemented.");
      }

      public void  FetchTravelTime()
      {
          foreach (RedirectSettingGroup setting in RedirectSettingGroups.Values)
          {
              try
              {
                  setting.FetchTravelTime();
              }
              catch
              { ;}
          }
      }

      public RedirectSettingGroup AddRoutingSetting(string devName, int displayPart, string lineid, string dir, int start_mileage, int end_mileage, bool isXml)
      {
          if (!RedirectSettingGroups.ContainsKey(devName))
          {
              RedirectSettingGroup group = new RedirectSettingGroup(devName);
              group.AddRountDetail(new RedirSetting(displayPart, lineid, dir, start_mileage, end_mileage, isXml));
              RedirectSettingGroups.Add(devName, group);
              return group;
          }
          else
          {

              RedirectSettingGroups[devName].AddRountDetail(new RedirSetting(displayPart, lineid, dir, start_mileage, end_mileage, isXml));
              return null;
          }
      }

      //public void AddRoutingSetting(RedirectSettingGroup grp,string devName, int displayPart, string lineid, string dir, int start_mileage, int end_mileage, bool isXml)
      //{
      //    if (!RedirectSettingGroups.ContainsKey(grp.devName))
      //    {
      //       // RedirectSettingGroup group = new RedirectSettingGroup(devName);
      //        grp.AddRountDetail(grp);
      //        RedirectSettingGroups.Add(grp.devName, grp);

      //    }
      //    else
      //    {

      //        RedirectSettingGroups[devName].AddRountDetail(new RedirSetting(displayPart, lineid, dir, start_mileage, end_mileage, isXml));
      //    }
      //}


      public override string ToString()
      {
          //return base.ToString();
          string ret = "";
          foreach (RedirectSettingGroup setting in RedirectSettingGroups.Values)
              ret += setting.ToString();
          return ret;
      }
      
    }



  public   class RedirectSettingGroup
    {
         public  string devName;
         System.Collections.Generic.List<RedirSetting> MainRoutes =   new List<RedirSetting>();
         System.Collections.Generic.List<RedirSetting> OptionRoutes = new List<RedirSetting>();
         System.Collections.Generic.Queue<int> queFiveMinMain=new Queue<int>();
         System.Collections.Generic.Queue<int> queFiveMinOpt=new Queue<int>();
     
      private  int _degree = 0;
     
      public event EventHandler OnEvent;
      public event EventHandler OnEventStart;

       public   RedirectSettingGroup(string devName):base()
         {
             this.devName = devName;

          

         }


   

         public  int Degree
          {
              set
              {
                  if (value != _degree )
                  {
                      //if ((_degree == 0 || _degree==-1) && value >0 )
                      //{
                      //     _degree = value;
                      //     if (this.OnEventStart != null)
                      //         this.OnEventStart(this, null);
                      //     return;
                      //}
                      

                      // _degree = value;
                      _degree = value;
                      if (this.OnEvent != null)
                          this.OnEvent(this, null);
                  }

                  
              }
              get
              {
                  return _degree;
              }
          }
         public void AddRountDetail(RedirSetting route)
         {
             if (route.DisplayPart == 1)
                 MainRoutes.Add(route);
             else
                 OptionRoutes.Add(route);
         }

      internal   void FetchTravelTime()
       {

         
           try
           {
               if (this.queFiveMinMain.Count < 5)

                   this.queFiveMinMain.Enqueue(getMainTravelTime());

               else
               {
                   this.queFiveMinMain.Dequeue();
                   this.queFiveMinMain.Enqueue(getMainTravelTime());
               }


               if (this.queFiveMinOpt.Count < 5)
               {
                   this.queFiveMinOpt.Enqueue(getOptTravelTime());

               }
               else
               {
                   this.queFiveMinOpt.Dequeue();
                   this.queFiveMinOpt.Enqueue(getOptTravelTime());
               }

           }
           catch
           {
               ;
           }

           Degree = this.getLevel();

           string sql = "insert into TBLCOMPARETRAVELTIMEDATA (devicename,timestamp,display_part,traveltime,level) values('{0}','{1}',{2},{3},{4})";

          System.DateTime dt=System.DateTime.Now;
          dt=dt.AddSeconds(-dt.Second);
           Program.matrix.dbServer.SendSqlCmd(  string.Format(sql, this.devName,DbCmdServer.getTimeStampString(dt),1,this.MainAVGTravelTime,this.getLevel()));
           Program.matrix.dbServer.SendSqlCmd(string.Format(sql, this.devName,DbCmdServer.getTimeStampString(dt), 2, this.OptAVGTravelTime,this.getLevel()));


       }

       int getMainTravelTime()
       {

           int mainRounteSec = 0;
           foreach (RedirSetting setting in MainRoutes)
           {
               int secs = 0;
               if ((secs = setting.getTravelTime()) != -1)
                   mainRounteSec += secs;
               else
                   return -1;
           }

           return mainRounteSec;
         
       }

       int getOptTravelTime()
       {

           int optionRouteSec = 0;
           foreach (RedirSetting setting in this.OptionRoutes)
           {
               int secs = 0;
               if ((secs = setting.getTravelTime()) != -1)
                   optionRouteSec += secs;
               else
                   return -1;
           }

           return optionRouteSec;

       }


       public int MainAVGTravelTime
       {

           get{
             int[] mainSecs = queFiveMinMain.ToArray();
             int msec = 0;
             int validcnt = 0;
             for (int i = 0; i < mainSecs.Length; i++)
             {
                 if (mainSecs[i] > 0)
                 {
                     msec += mainSecs[i];
                     validcnt++;
                 }
             }

             if (validcnt == 0)
                 return -1;

          return      msec = msec / validcnt;

           }

       }

      public int OptAVGTravelTime
      {
          get
          {
              int[] optSecs = queFiveMinOpt.ToArray();
              int osec = 0;

              int validcnt = 0;
              for (int i = 0; i < optSecs.Length; i++)
              {
                  if (optSecs[i] > 0)
                  {
                      osec += optSecs[i];
                      validcnt++;
                  }
              }

              if (validcnt == 0)
                  return -1;

             return    osec = osec / validcnt;

          }
      }

       public   int getLevel()
       {

         //  int[] mainSecs = queFiveMinMain.ToArray();
         //  int[] optSecs = queFiveMinOpt.ToArray();
           int msec = 0, osec = 0;

           //int validcnt = 0;
           //for (int i = 0; i < mainSecs.Length; i++)
           //{
           //    if (mainSecs[i] > 0)
           //    {
           //        msec += mainSecs[i];
           //        validcnt++;
           //    }
           //}

           //if (validcnt == 0)
           //    return 0;

           //msec = msec / validcnt;

           //validcnt = 0;

           //for (int i = 0; i < optSecs.Length; i++)
           //{
           //    if (optSecs[i] > 0)
           //    {
           //        osec +=optSecs[i];
           //        validcnt++;
           //    }
           //}

           //if (validcnt == 0)
           //    return 0;
           //osec = osec / validcnt;

           msec = this.MainAVGTravelTime;
           osec = this.OptAVGTravelTime;

           if (msec == -1 || msec == 0 || osec == -1 || osec == 0)
               return 0;

             return    RouteRedirectManager.redir_level_table.calcDegree(msec, osec);

           

       }

       public override string ToString()
       {
           string ret;
           ret = devName + ", Degree:"+ this.getLevel()+"\r\n";
           ret += "\tmain:" +this.MainAVGTravelTime+ " sec\r\n";
           ret += "\topt:" +this.OptAVGTravelTime + " sec\r\n";
           //foreach (RedirSetting setting in MainRoutes)
           //    ret += setting.ToString() + "\r\n";
           //ret += "\r\n";

           //foreach (RedirSetting setting in OptionRoutes)
           //    ret += setting.ToString() + "\r\n";
           return ret;
           
           //return base.ToString();
       }

     
   }


  public class RedirSetting
   {
      public  int DisplayPart, StartMileage,EndMileage;
      public  string Lineid, Dir;
      public bool IsXml = false;
      public  RedirSetting(int displayPart,string lineid,string dir,int start_mileage,int end_mileage,bool isXml)
       {
           this.DisplayPart = displayPart;
           this.Lineid = lineid;
           this.Dir = dir;
           this.StartMileage = start_mileage;
           this.EndMileage = end_mileage;
           this.IsXml = isXml;
       }

       public override string ToString()
       {
           //return base.ToString();
           return ((DisplayPart == 1) ? "Main:" : "Option") + Lineid + "," + Dir + "," + StartMileage + "~" + EndMileage + "," + getTravelTime() + "secs";
          
       }

       internal int getTravelTime()
       {

            int traveltime=0,ulimit=0,llimit=0;
            try
            {
                if (!IsXml)
                    return Program.matrix.line_mgr[Lineid].getTravelTime(this.Dir, this.StartMileage, this.EndMileage);
                else
                {
                    Program.matrix.timcc_section_mgr.getTravelDataByRange(Lineid, Dir, StartMileage, EndMileage, ref traveltime, ref ulimit, ref llimit);
                    return traveltime;
                }
            }
            catch
            {
                return -1;
            }

       }
   }


   class RGSRedirectLevelTable
    {
        class LevelRecord
        {
            public double lowerfactor, upperfactor;
           public   int degree;
            public LevelRecord(double lowerfactor, double upperfactor, int degree)
            {
                this.degree = degree;
                this.lowerfactor = lowerfactor;
                this.upperfactor = upperfactor;
            }
        }

        System.Collections.Generic.List<LevelRecord> lstRecord = new List<LevelRecord>();

        public RGSRedirectLevelTable()
        {
            lock (this)
            {
                loadTable();
            }
        }

        public void loadTable()
        {
            lstRecord.Clear();

            OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
            OdbcCommand cmd = new OdbcCommand("select lowerfactor,upperfactor,degree from tblCompareDegree order by degree");
            cmd.Connection = cn;
            OdbcDataReader rd;
            try
            {
                cn.Open();
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {

                    double lowerfactor, upperfactor;
                    int degree;

                    lowerfactor = System.Convert.ToDouble(rd[0]);
                    upperfactor = System.Convert.ToDouble(rd[1]);
                    degree = System.Convert.ToInt32(rd[2]);
                    lstRecord.Add(new LevelRecord(lowerfactor, upperfactor, degree));

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


        internal int calcDegree(int mainsec,int optsec)
        {
            if (optsec > mainsec )
                return 0;

            int diff = mainsec - optsec;

            foreach (LevelRecord rec in this.lstRecord)
            {

                if (diff >= rec.lowerfactor * mainsec && diff < rec.upperfactor * mainsec)
                    return rec.degree;
            }


            return 0;
        }

    }



}
