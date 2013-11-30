using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using System.Data.Odbc;



namespace Host.Schedule
{
   public  class ScheduleManager
    {

       
       
       public static void  LoadAllSchedule()
       {
           OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
           OdbcCommand cmd = new OdbcCommand("select schid from tblSchConfig  where enable='Y'  ");
           OdbcDataReader rd;
           cmd.Connection = cn;

           try
           {
               cn.Open();
               rd = cmd.ExecuteReader();
               while (rd.Read())
               {
                  AddSchedule(System.Convert.ToInt32(rd[0]));
               }
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message);
           }
           finally
           {
               cn.Close();
           }


           ConsoleServer.WriteLine("Schedule Load Completed!");
       }

       public static  string getAllScheduleStatus()
       {
           string ret="";
           for(int i=0;i<Scheduler.Count();i++)
              ret+= Scheduler.GetScheduleAt(i).ToString()+"\r\n";
          return ret;
       }


       public static void RemoveSchedule(int schid)
       {
           Scheduler.RemoveSchedule(schid.ToString());
       }
       public static void AddSchedule(int sschid)
       {
           OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
           OdbcCommand cmd = new OdbcCommand("select schid,schname,schtype,isduration,duration,week1,week2,week3,week4,week5,week6,week7,starttime,IsManualMode from tblSchConfig  where schid="+sschid);
           OdbcDataReader rd;
           cmd.Connection = cn;

           try
           {
               Scheduler.RemoveSchedule(sschid.ToString());
               cn.Open();
               rd = cmd.ExecuteReader();
               while (rd.Read())
               {
                   int schid = System.Convert.ToInt32(rd[Global.getFiledInx(rd, "schid")]);
                   string schname = rd[Global.getFiledInx(rd, "schname")].ToString();
                   int schtype = System.Convert.ToInt32(rd[Global.getFiledInx(rd, "schtype")]);
                   int isDuration = System.Convert.ToInt32(rd[Global.getFiledInx(rd, "isduration")]);
                   System.DateTime dt = System.Convert.ToDateTime(rd[Global.getFiledInx(rd, "starttime")]);
                   int duration = System.Convert.ToInt32(rd[Global.getFiledInx(rd, "duration")]);
                   int[] weekdays = new int[7];
                   for (int i = 1; i <= 7; i++)
                       weekdays[i - 1] = System.Convert.ToInt32(rd[Global.getFiledInx(rd, "week" + i)]);
                   Schedule schedule = CreateSchedule(schid, schname, schtype, isDuration, duration, dt, weekdays);
                   bool isManualMode = rd[Global.getFiledInx(rd, "IsManualMode" )].ToString().Trim()=="Y"? true:false;

                   if (isManualMode)
                       schedule.outputMode = RemoteInterface.HC.OutputModeEnum.ManualMode;
                   else
                       schedule.outputMode = RemoteInterface.HC.OutputModeEnum.ScheduleMode;


                   Scheduler.AddSchedule(schedule);
                   ConsoleServer.WriteLine(schedule.ToString());
               }
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message);
           }
           finally
           {
               cn.Close();
           }

       }

       public   static   Schedule CreateSchedule(int schid,string schname,int schtype,int isDuration,int durationMin,DateTime dt,int[] weekdays)
        {
            System.DateTime starttime=DateTime.MinValue;
#if DEBUG
          //  dt = System.DateTime.Now.AddMinutes(1);
 

#endif
            Schedule schedule=null;


            if (schtype == 0)  //repeat
            {
                for (int i = 0; i <= 7; i++)
                {
                    DateTime invokeTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, dt.Hour, dt.Minute, dt.Second).AddDays(i);
                    if (weekdays[(int)invokeTime.DayOfWeek] == 1 && invokeTime > DateTime.Now)
                    {
                        starttime = invokeTime;
                        break;
                    }


                }
            }
            else  //one time
                if (DateTime.Now < dt)
                    starttime = dt;
                else
                    throw new Exception(schid + " 過期");
            


             if (starttime == DateTime.MinValue)
                 throw new Exception("Schedule Err:can not find invoke time!");

             OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
             OdbcCommand cmd = new OdbcCommand("select schid,subschid,devicename,command from tblschdetail where schid=" + schid);
             cmd.Connection = cn;
             System.Collections.ArrayList ary = new System.Collections.ArrayList();
             try
             {

                
                 cn.Open();
                 OdbcDataReader rd = cmd.ExecuteReader();
              
               //  int inx = 0;
                 while (rd.Read())
                 {
                    // int schid = System.Convert.ToInt32(rd[Global.getFiledInx(rd, "schid")]);
                     int subschid = System.Convert.ToInt32(rd[Global.getFiledInx(rd, "subschid")]);
                     string devName = rd[Global.getFiledInx(rd, "devicename")].ToString();
                     string command ="";
                     if (!rd.IsDBNull(Global.getFiledInx(rd, "command")))
                         command = rd[Global.getFiledInx(rd, "command")].ToString();
                     else
                         command = "";
                     if(command=="")
                      ary.Add(  new ScheduleJob(schid,devName, subschid, null));
                     else
                      ary.Add(new ScheduleJob(schid,devName,subschid,RemoteInterface.Utils.Util.StringToObj(command)));


                 }

                  ScheduleJob[] schjobs= new ScheduleJob[ary.Count];
                  for (int i = 0; i < ary.Count; i++)
                      schjobs[i] = (ScheduleJob)ary[i];





                 switch (schtype)
                 {
                     case 0: //repeat
                         schedule = new DailySchedule(schid.ToString(), starttime, (isDuration == 0) ? 0 : durationMin, schjobs, true);
                         break;
                     case 1: //one time
                         schedule = new OneTimeSchedule(schid.ToString(), starttime, (isDuration == 0) ? 0 : durationMin, schjobs, true);
                         break;
                 }

                 for(int i=0;i<7;i++)
                     schedule.SetWeekDay((DayOfWeek)i,(weekdays[i]==1)?true:false);

                
                 return schedule;

             }
             catch (Exception ex)
             {
                 throw new Exception(ex.Message + "," + ex.StackTrace);
             }
             finally
             {
                 cn.Close();
             }


        }

    }
}
