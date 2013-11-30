using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using RemoteInterface;

namespace Host.ETC
{
  
   public   class ETC_Manager
   {
       string[] baseurls = new string[] { "http://10.21.50.196" };
       System.Collections.Hashtable sections = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
       System.Timers.Timer tmr5Min = new System.Timers.Timer( 60 * 1000);
       RemoteInterface.ExactIntervalTimer tmrEveryDay = new ExactIntervalTimer(12, 0, 10);
       public ETC_Manager()
       {

           loadETCIPs();
           LoadEtcSection();
           tmr5Min_Elapsed(null, null);
           tmr5Min.Elapsed += new System.Timers.ElapsedEventHandler(tmr5Min_Elapsed);
           tmr5Min.Start();

           tmrEveryDay.OnElapsed += new OnConnectEventHandler(tmrEveryDay_OnElapsed);

          // tmrEveryDay_OnElapsed(null);
           //   Util.Log(Util.CPath(AppDomain.CurrentDomain.BaseDirectory+  "sys.log"),DateTime.Now+","+secid+",load historydata!\r\n");
       }



       public void loadETCIPs()
       {

           OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
           OdbcCommand cmd = new OdbcCommand("select etc1,etc2,etc3 from tbletcparameter");
           cmd.Connection = cn;
           OdbcDataReader rd;
           lock (this)
           {
               try
               {
                   cn.Open();
                   rd = cmd.ExecuteReader();
                   rd.Read();
                   string s = "http://" + rd[0].ToString() + "," + "http://" + rd[1].ToString() + "," + "http://" + rd[2].ToString();
                   baseurls = s.Split(new char[] { ',' });
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
       }
       void tmrEveryDay_OnElapsed(object sender)
       {

           try
           {
               RepairF3EtcTask();

               Util.SysLog("sys.log", DateTime.Now + ",Etc  Manager,Repair F3 Data Finished!");
              
           }
           catch (Exception ex)
           {
               Util.Log(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "sys.log"), DateTime.Now + ",Etc Manager,Repair F3 Data job! " + ex.StackTrace + "\r\n");
           }


           try
           {
               RepairF9EtcDataTask();
              
               Util.SysLog("sys.log", DateTime.Now + ",Etc Manager,Repair Data Finished!");
              // Util.Log(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "sys.log"), DateTime.Now + ",Etc Manager,Repair Data Finished!\r\n");
           }
           catch (Exception ex)
           {
               Util.Log(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + "sys.log"), DateTime.Now + ",Etc Manager,Repair Data job! "+ex.StackTrace+"\r\n");
           }
          
         
       }

       int lastupdateHour=-1;
       void tmr5Min_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
       {
           for(int i=0;i<baseurls.Length;i++)
           {
               try
               {
                   LoadEtcTrafficData(baseurls[i]);
                   return;
                
               }
               catch
               {
                   ;
               }

              
              
            }

            if (lastupdateHour != DateTime.Now.Hour)
            {
                lastupdateHour = DateTime.Now.Hour;
                for (int i = 0; i < baseurls.Length; i++)
                {
                    try
                    {
                        LoadEtcVolData(baseurls[i]);
                        return;

                    }
                    catch
                    {
                        ;
                    }

                }


            }



          

           //throw new Exception("The method or operation is not implemented.");
       }


    
       void LoadEtcSection()
       {

           //
           sections.Clear();
           OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
           OdbcCommand cmd = new OdbcCommand("select START_CODE,END_CODE,START_MILEAGE,END_MILEAGE,LINEID,DIRECTION,flowid from VWETC_SECTION");
           OdbcDataReader rd;
           cmd.Connection = cn;
           try
           {
               cn.Open();
               rd = cmd.ExecuteReader();
               while (rd.Read())
               {
                   string startdiv = rd[0].ToString();
                   string enddiv = rd[1].ToString();
                   int startMileage = System.Convert.ToInt32(rd[2]);
                   int endMileleage = System.Convert.ToInt32(rd[3]);
                   string lineid = rd[4].ToString();
                   string direction = rd[5].ToString();
                   int flowid = System.Convert.ToInt32(rd[6]);
                   ETC.EtcSection sec = new EtcSection(lineid, direction, startdiv, enddiv, startMileage, endMileleage,flowid);
                   sections.Add(sec.Key,sec);
                  // sections.Add(new AVISection(secid, startdev, enddev, sampleInterval));
               }

           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
           }
           finally
           { cn.Close(); }
       }

       void RepairF3EtcTask()
       {
           System.Data.Odbc.OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
           System.Data.Odbc.OdbcCommand cmd = new OdbcCommand();
           cmd.Connection = cn;
           string sql = "select distinct timestamp from tbletcdata5min where timestamp between '{0}' and '{1}' and datavalidity_vol='N'";

           DateTime begtime, endtime = DateTime.Now.AddHours(-2);
           //   endtime = System.DateTime.Now.AddHours(-1);
           begtime = System.DateTime.Now.AddDays(-3);
           try
           {
               cn.Open();
               cmd.CommandText = string.Format(sql, RemoteInterface.DbCmdServer.getTimeStampString(begtime), RemoteInterface.DbCmdServer.getTimeStampString(endtime));
               System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
               while (rd.Read())
               {
                   if (System.Convert.ToDateTime(rd[0]).Minute != 0 && System.Convert.ToDateTime(rd[0]).Second != 0)
                       continue;
                   
                   try
                   {
                       RepairF3EtcData(System.Convert.ToDateTime(rd[0]));
                   }
                   catch (Exception ex)
                   {
                       ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                   }
               }
               rd.Close();
           }

           finally
           {
               cn.Close();

           }

       }
       void RepairF9EtcDataTask()
       {
           System.Data.Odbc.OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
           System.Data.Odbc.OdbcCommand cmd = new OdbcCommand();
           cmd.Connection = cn;
           string sql = "select  distinct timestamp from tbletcdata5min where  timestamp between '{0}' and '{1}' and datavalidity='N'";
         
           DateTime begtime, endtime= DateTime.Now.AddMinutes(-40);
        //   endtime = System.DateTime.Now.AddHours(-1);
           begtime = System.DateTime.Now.AddDays(-3);
           try
           {
               cn.Open();
               cmd.CommandText = string.Format(sql, RemoteInterface.DbCmdServer.getTimeStampString(begtime),RemoteInterface.DbCmdServer.getTimeStampString(endtime));
               System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
               while (rd.Read())
               {
                   try
                   {
                       RepairF9EtcData(System.Convert.ToDateTime(rd[0]));
                   }
                   catch (Exception ex)
                   {
                       ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                   }
               }
               rd.Close();
           }
          
           finally
           {
               cn.Close();
             
           }
       }


       void RepairF9EtcData(DateTime dt)
       {

           string filename = "{0:0000}{1:00}{2:00}/F09/TDCSF09{3:0000}{4:00}{5:00}{6:00}{7:00}{8:00}.txt";
         //  System.DateTime dt = System.DateTime.Now;
        //   dt = dt.AddMinutes(-35);
         //  dt = dt.AddSeconds(-dt.Second);
           DateTime fdt=dt.AddMinutes(-24);
           string etcfile = string.Format(filename, dt.Year, dt.Month, dt.Day,fdt.Year,fdt.Month,fdt.Day, fdt.Hour,fdt.Minute,fdt.Second);
          // RemoteInterface.Util.SysLog("f9.log", string.Format(filename, dt.Year, dt.Month, dt.Day, fdt.Year, fdt.Month, fdt.Day, fdt.Hour, fdt.Minute, fdt.Second));

           System.Net.WebRequest req = System.Net.WebRequest.Create(baseurls[0] + "/" +etcfile);
           System.IO.StreamReader rd;
           try
           {
               rd = new System.IO.StreamReader(req.GetResponse().GetResponseStream());
           }
           catch (Exception ex)
           {
               Program.matrix.dbServer.SendSqlCmd("update tbletcdata5min set datavalidity='R' where timestamp='" + DbCmdServer.getTimeStampString(dt) + "'");

               return;
           }

           while (true)
           {
               try
               {

                   string temp = null;

                   try
                   {
                       temp = rd.ReadLine();
                   }
                   catch (Exception ex)
                   {
                       RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                       return;
                   }
                   if (temp == null)
                   {
                       Program.matrix.dbServer.SendSqlCmd("update tbletcdata5min set datavalidity='R' where timestamp='" + DbCmdServer.getTimeStampString(dt) + "'");
                       break;
                   }
                   string[] data = temp.Split(new char[] { ',' });

                   DateTime timestamp = System.Convert.ToDateTime(data[0]);
                   string key = data[3] + "-" + data[1] + "-" + data[2];
                   int travelTime = System.Convert.ToInt32(data[5]);
                   int type = System.Convert.ToInt32(data[4]);
                   if (!this.sections.Contains(key))
                       continue;
                   EtcSection sec = sections[key] as EtcSection;
                 
                   string carfield = "";
                   switch (type)
                   {
                       case 31: //小客車
                           carfield = "minibus_TravelTime";

                           break;
                       case 32:  // 小貨
                           carfield = "minitruck_TravelTime";
                           break;
                       case 41:  // 大客
                           carfield = "Town_bus_TravelTime";
                           break;
                       case 42:  // 大貨
                           carfield = "truck_TravelTime";
                           break;
                       case 5:   // 連結
                           carfield = "connect_car_TravelTime";
                           break;

                   }

                   int speed = -1;
                   if (type == 31)
                   {
                       speed = System.Math.Abs(sec.endMile - sec.startMile) / travelTime * 60 / 1000;
                   }
                   //if (type == 31)
                   //{
                   //    sec.setTravelTime(dt, travelTime);
                   //    ConsoleServer.WriteLine(temp + ",spd=" + sec.Speed);
                   //}

                   //tblETCData5Min
                   string sql;
                   if (carfield != "")
                   {
                       if (type == 31)
                       {
                           sql = "update tbletcdata5min set traveltime={0},speed={3},dataValidity='R', {4}={5} where timestamp='{1}' and flowid={2} ";
                           sql = string.Format(sql, travelTime, RemoteInterface.DbCmdServer.getTimeStampString(timestamp), sec.flowid,speed, carfield, travelTime);
                       }
                       else
                       {
                           sql = "update tbletcdata5min set dataValidity='R', {2}={3} where timestamp='{0}' and flowid={1} ";
                           sql = string.Format(sql, RemoteInterface.DbCmdServer.getTimeStampString(timestamp), sec.flowid, carfield, travelTime);

                       }




                       Program.matrix.dbServer.SendSqlCmd(sql);
                   }
               }
               catch (Exception ex)
               {
                   RemoteInterface.ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
               }

           }

           rd.Close();
       }

       void RepairF3EtcData(System.DateTime dt)
       {


           string filename = "{0:0000}{1:00}{2:00}/F03/TDCSF03{3:0000}{4:00}{5:00}{6:00}{7:00}{8:00}.txt";

           // System.DateTime dt = System.DateTime.Now;
           //  dt = dt.AddMinutes(-35);
           // dt = dt.AddSeconds(-dt.Second);
         //  dt = dt.AddHours(-2);
           DateTime ft = dt.AddHours(-2);
         //  RemoteInterface.Util.SysLog("f3.log", string.Format(filename, dt.Year, dt.Month, dt.Day,ft.Year,ft.Month,ft.Day, ft.Hour, 0/* dt.Minute*/, 0/*dt.Second*/));
           System.Net.WebRequest req = System.Net.WebRequest.Create(baseurls[0] + "/" + string.Format(filename, dt.Year, dt.Month, dt.Day, ft.Year, ft.Month, ft.Day, ft.Hour, 0/* dt.Minute*/, 0/*dt.Second*/));
           System.IO.StreamReader rd = new System.IO.StreamReader(req.GetResponse().GetResponseStream());

           while (true)
           {
               try
               {

                   string temp = null;

                   try
                   {
                       temp = rd.ReadLine();
                   }
                   catch (Exception ex)
                   {
                       RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                       Program.matrix.dbServer.SendSqlCmd("update tbletcdata5min set datavalidity_vol='R' where timestamp='" + DbCmdServer.getTimeStampString(dt) + "'");

                       break;
                   }
                   if (temp == null)
                       break;
                   //{
                   //    Program.matrix.dbServer.SendSqlCmd("update tbletcdata5min set datavalidity='V' where timestamp='" + DbCmdServer.getTimeStampString(dt) + "'");
                   //    break;
                   //}
                   string[] data = temp.Split(new char[] { ',' });

                   DateTime timestamp = System.Convert.ToDateTime(data[0]);
                   string key = data[3] + "-" + data[1] + "-" + data[2];
                   int vol = System.Convert.ToInt32(data[5]);
                   int type = System.Convert.ToInt32(data[4]);
                   if (!this.sections.Contains(key))
                       continue;
                   EtcSection sec = sections[key] as EtcSection;

                   //  sec.setTravelTime(dt, type, travelTime);
                   //ConsoleServer.WriteLine(temp + ",spd=" + sec.Speed);
                   string carfield = "";
                   switch (type)
                   {
                       case 31: //小客車
                           carfield = "minibus_vol";

                           break;
                       case 32:  // 小貨
                           carfield = "minitruck_vol";
                           break;
                       case 41:  // 大客
                           carfield = "Town_bus_vol";
                           break;
                       case 42:  // 大貨
                           carfield = "truck_vol";
                           break;
                       case 5:   // 連結
                           carfield = "connect_car_vol";
                           break;

                   }

                   //tblETCData5Min


                   //if (type == 31)
                   //{
                   //    sec.setTravelTime(dt, travelTime);
                   //    ConsoleServer.WriteLine(temp + ",spd=" + sec.Speed);
                   //}

                   string sql;
                   if (carfield != "")
                   {
                       // if (type == 31)
                       //{
                       //    sql = "update tbletcdata5min set traveltime={0},speed={3},dataValidity='V', {4}={5} where timestamp='{1}' and flowid={2} ";
                       //    sql = string.Format(sql, travelTime, RemoteInterface.DbCmdServer.getTimeStampString(timestamp), sec.flowid, sec.Speed, carfield, travelTime);
                       //}
                       //else
                       //{
                       sql = "update tbletcdata5min set {2}={3},datavalidity_vol='R'  where timestamp='{0}' and flowid={1} ";
                       sql = string.Format(sql, RemoteInterface.DbCmdServer.getTimeStampString(timestamp), sec.flowid, carfield, vol);

                       //}




                       Program.matrix.dbServer.SendSqlCmd(sql);
                   }
               }
               catch (Exception ex)
               {
                   RemoteInterface.ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
               }

           }

           rd.Close();
       }

       void LoadEtcVolData(string baseurl)
       {

           string filename = "{0:0000}{1:00}{2:00}/F03/TDCSF03{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}.txt";

           System.DateTime dt = System.DateTime.Now;
         //  dt = dt.AddMinutes(-35);
          // dt = dt.AddSeconds(-dt.Second);
            dt= dt.AddHours(-2);
           System.Net.WebRequest req = System.Net.WebRequest.Create(baseurl + "/" + string.Format(filename, dt.Year, dt.Month, dt.Day, dt.Hour,0/* dt.Minute*/, 0/*dt.Second*/));
           System.IO.StreamReader rd = new System.IO.StreamReader(req.GetResponse().GetResponseStream());

           while (true)
           {
               try
               {

                   string temp = null;

                   try
                   {
                       temp = rd.ReadLine();
                   }
                   catch (Exception ex)
                   {
                       RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                       break;
                   }
                   if (temp == null)
                       break;
                   //{
                   //    Program.matrix.dbServer.SendSqlCmd("update tbletcdata5min set datavalidity='V' where timestamp='" + DbCmdServer.getTimeStampString(dt) + "'");
                   //    break;
                   //}
                   string[] data = temp.Split(new char[] { ',' });

                   DateTime timestamp = System.Convert.ToDateTime(data[0]);
                   string key = data[3] + "-" + data[1] + "-" + data[2];
                   int vol = System.Convert.ToInt32(data[5]);
                   int type = System.Convert.ToInt32(data[4]);
                   if (!this.sections.Contains(key))
                       continue;
                   EtcSection sec = sections[key] as EtcSection;

                   //  sec.setTravelTime(dt, type, travelTime);
                   //ConsoleServer.WriteLine(temp + ",spd=" + sec.Speed);
                   string carfield = "";
                   switch (type)
                   {
                       case 31: //小客車
                           carfield = "minibus_vol";

                           break;
                       case 32:  // 小貨
                           carfield = "minitruck_vol";
                           break;
                       case 41:  // 大客
                           carfield = "Town_bus_vol";
                           break;
                       case 42:  // 大貨
                           carfield = "truck_vol";
                           break;
                       case 5:   // 連結
                           carfield = "connect_car_vol";
                           break;

                   }

                   //tblETCData5Min


                   //if (type == 31)
                   //{
                   //    sec.setTravelTime(dt, travelTime);
                   //    ConsoleServer.WriteLine(temp + ",spd=" + sec.Speed);
                   //}

                   string sql;
                   if (carfield != "")
                   {
                      // if (type == 31)
                       //{
                       //    sql = "update tbletcdata5min set traveltime={0},speed={3},dataValidity='V', {4}={5} where timestamp='{1}' and flowid={2} ";
                       //    sql = string.Format(sql, travelTime, RemoteInterface.DbCmdServer.getTimeStampString(timestamp), sec.flowid, sec.Speed, carfield, travelTime);
                       //}
                       //else
                       //{
                           sql = "update tbletcdata5min set {2}={3} where timestamp='{0}' and flowid={1} ";
                           sql = string.Format(sql, RemoteInterface.DbCmdServer.getTimeStampString(timestamp), sec.flowid, carfield, vol);

                       //}




                       Program.matrix.dbServer.SendSqlCmd(sql);
                   }
               }
               catch (Exception ex)
               {
                   RemoteInterface.ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
               }

           }

           rd.Close();
       }

       void LoadEtcTrafficData(string baseurl)
       {
           //http://10.21.50.196/20100603/F09/TDCSF0920100603003000.txt
           string filename = "{0:0000}{1:00}{2:00}/F09/TDCSF09{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}.txt";
           
           System.DateTime dt =System.DateTime.Now;
           dt = dt.AddMinutes(-35);
           dt = dt.AddSeconds(-dt.Second);
           System.Net.WebRequest req= System.Net.WebRequest.Create(baseurl+"/"+string.Format(filename,dt.Year,dt.Month,dt.Day,dt.Hour,dt.Minute,dt.Second));
           System.IO.StreamReader rd = new System.IO.StreamReader(req.GetResponse().GetResponseStream());
        
           while(true)
           {
               try
               {
                  
                   string temp=null ;

                   try
                   {
                       temp = rd.ReadLine();
                   }
                   catch (Exception ex)
                   {
                       RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                       break;
                   }
                   if (temp == null)
                   {
                       Program.matrix.dbServer.SendSqlCmd("update tbletcdata5min set datavalidity='V' where timestamp='" + DbCmdServer.getTimeStampString(dt) + "'");
                       break;
                   }
                   string[] data = temp.Split(new char[] { ',' });

                   DateTime timestamp = System.Convert.ToDateTime(data[0]);
                   string key = data[3] + "-" + data[1] + "-" + data[2];
                   int travelTime = System.Convert.ToInt32(data[5]);
                   int type = System.Convert.ToInt32(data[4]);
                   if (!this.sections.Contains(key))
                       continue;
                   EtcSection sec = sections[key] as EtcSection;
                  
                      //  sec.setTravelTime(dt, type, travelTime);
                   //ConsoleServer.WriteLine(temp + ",spd=" + sec.Speed);
                   string carfield="";
                   switch(type)
                   {
                       case 31: //小客車
                           carfield="minibus_TravelTime";

                           break;
                       case 32:  // 小貨
                           carfield = "minitruck_TravelTime";  
                            break;
                       case 41:  // 大客
                           carfield = "Town_bus_TravelTime";
                           break;
                       case 42:  // 大貨
                           carfield = "truck_TravelTime";
                           break;
                       case 5:   // 連結
                           carfield = "connect_car_TravelTime";
                           break;
                     
                   }

                   //tblETCData5Min


                   if (type == 31)
                   {
                       sec.setTravelTime(dt, travelTime);
                       ConsoleServer.WriteLine(temp + ",spd=" + sec.Speed);
                   }

                    string sql;
                    if (carfield != "")
                    {
                        if (type == 31)
                        {
                            sql = "update tbletcdata5min set traveltime={0},speed={3},dataValidity='V', {4}={5} where timestamp='{1}' and flowid={2} ";
                            sql = string.Format(sql, travelTime, RemoteInterface.DbCmdServer.getTimeStampString(timestamp), sec.flowid, sec.Speed, carfield, travelTime);
                        }
                        else
                        {
                            sql = "update tbletcdata5min set dataValidity='V', {2}={3} where timestamp='{0}' and flowid={1} ";
                            sql = string.Format(sql, RemoteInterface.DbCmdServer.getTimeStampString(timestamp), sec.flowid, carfield, travelTime);

                        }




                            Program.matrix.dbServer.SendSqlCmd(sql);
                    }
               }
               catch (Exception ex)
               {
                   RemoteInterface.ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
               }
                
           }

           rd.Close();
          

       }
       public int GetTrafficSpeed(string lineid, string direction, int mile_m)
       {
           foreach (ETC.EtcSection sec in sections.Values)
           {
               if (sec.direction != direction || sec.lineid != lineid)
                   continue;
               int begMile = sec.startMile;
               int endmile = sec.endMile;
               if (endmile < begMile)
               {
                   int t = begMile;
                   begMile = endmile;
                   endmile = t;
               }

               if (mile_m >= begMile && mile_m < endmile)
               {


                   return sec.Speed; ;
               }

           }

           return -1;

       }
      
    }
}

    

