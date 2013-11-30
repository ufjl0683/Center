using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_SVWS
{
   public    class SVWS_StatusManager
    {

       System.Collections.Hashtable hs_status = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
       public SVWS_StatusManager()
       {
         
       }


       public void AddSVWS_Status(SVWS_Status svws_status)
       {
           if (!hs_status.Contains(svws_status.Key))
           {
               hs_status.Add(svws_status.Key, svws_status);
               Program.mfcc_svws.dbServer.SendSqlCmd(svws_status.GetInsertStatement());
               Program.mfcc_svws.dbServer.SendSqlCmd(svws_status.GetInsertLogSql());
           }
           else
           {
               if (((SVWS_Status)hs_status[svws_status.Key]).Equals(svws_status))
                   return;

               hs_status.Remove(svws_status.Key);
               hs_status.Add(svws_status.Key, svws_status);
               Program.mfcc_svws.dbServer.SendSqlCmd(svws_status.GetUpdateStatement());
              
               
           }
       }
       //public bool IsMatch(string key)
       //{
       //    return hs_status.Contains(key);
       //}
       public void loadStatus()
       {
          
               System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(DbCmdServer.getDbConnectStr());
               System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();
               cmd.Connection = cn;
               cmd.CommandText= "select type,place,group,kind,dir,run_status,hw_status,density,level,no from tblsvwsconfig";
               try
               {
                   cn.Open();
                   System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();

                   while (rd.Read())
                   {
                       string type;
                       int place, group, kind, dir, run_status, hw_status, density, level, no;
                       type = rd[0].ToString();
                       place = System.Convert.ToInt32(rd[1]);
                       group = System.Convert.ToInt32(rd[2]);
                       kind = System.Convert.ToInt32(rd[3]);
                       dir = System.Convert.ToInt32(rd[4]);
                       run_status = System.Convert.ToInt32(rd[5]);
                       hw_status = System.Convert.ToInt32(rd[6]);
                       density = System.Convert.ToInt32(rd[7]);
                       level = System.Convert.ToInt32(rd[8]);
                       no = System.Convert.ToInt32(rd[9]);
                       SVWS_Status svws_status=new SVWS_Status();
                        if(type=="A")
                            svws_status.SetAnalogStatus(place,group,kind,dir,density,level);
                        else if(type=="D")
                             svws_status.SetDigitalStatus(place,group,kind,dir,run_status,hw_status);

                         this.hs_status.Add(svws_status.Key, svws_status);

                   }

                   rd.Close();
               }
               catch (Exception ex)
               {
                   ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
               }
               finally
               {
                   try { cn.Close(); }
                   catch { ;}
               }
          
       }

    }



   public class SVWS_Status
    {
     public   string type;
     public int place;
     public int group;
     public int kind;
     public int dir;
     public int run_status;
     public int hw_status;
     public int density;
     public int level;
     public int no;

        public  void SetDigitalStatus(int place, int group, int kind, int dir, int run_status, int hw_status)
        {
            this.type = "D";
            this.place = place;
            this.group = group;
            this.kind = kind;
            this.dir = dir;
            this.run_status = run_status;
            this.hw_status = hw_status;
            
        }

        public void SetAnalogStatus(int place, int group, int kind, int dir, int density, int level)
        {
            this.type = "A";
            this.place = place;
            this.group = group;
            this.kind = kind;
            this.dir = dir;

        }

        public string GetInsertStatement()
        {
            string sql="";
            if (type == "A")
            {
                sql = "insert into tblsvwsconfig (type,place,group,kind,dir,density,level) values('{0}',{1},{2},{3},{4},{5},{6})";
                return string.Format(sql, type, place, group, kind, dir, density, level);

            }
            else if (type == "D")
            {
                sql = "insert into tblsvwsconfig (type,place,group,kind,dir,run_status,hw_status) values('{0}',{1},{2},{3},{4},{5},{6})";
                return string.Format(sql, type, place, group, kind, dir, run_status, hw_status);

            }

            throw new Exception("GetInsertStatement: Ubknow Type");
          
        }

        public string GetUpdateStatement()
        {

             string sql="";
             if (type == "A")
             {
                 sql = "update tblsvwsconfig set density={0},level={1} where type='A' and place={2} and group={3} and kind={4} and dir={5}";
                 return string.Format(sql, density, level, place, group, kind, dir);
             }
             else if (type == "D")
             {
                 sql = "update tblsvwsconfig set run_status={0},hw_status={1} where type='D' and place={2} and group={3} and kind={4} and dir={5}";
                 return string.Format(sql, run_status, hw_status, place, group, kind, dir);
             }

             throw new Exception("in GetUpdateStatement:unknow type");
         
        }


           public string GetInsertLogSql()
           {
               string sql = "";
               if (type == "A")
               {
                   sql = "insert into tblsvwslog (type,place,group,kind,dir,density,level,timestamp) values('{0}',{1},{2},{3},{4},{5},{6},'{7}')";
                   return string.Format(sql, type, place, group, kind, dir, density, level, RemoteInterface.DbCmdServer.getTimeStampString(DateTime.Now));
               }
               else if (type == "D")
               {
                   sql = "insert into tblsvwslog (type,place,group,kind,dir,run_status,hw_status,timestamp) values('{0}',{1},{2},{3},{4},{5},{6},'{7}')";
                   return string.Format(sql, type, place, group, kind, dir, run_status, hw_status, DbCmdServer.getTimeStampString(DateTime.Now));
               }

               throw new Exception("in GetInsertLogSql:unknow type");

           }

        public SVWS_Status()
        {
        }

        public SVWS_Status(System.Data.DataSet ds)
        {
            System.Data.DataRow r = ds.Tables[0].Rows[0];
            if (r["func_name"].ToString() == "report_digital_signal_status")
            {
                type = "D";
                run_status = System.Convert.ToInt32(r["run_status"]);
                hw_status = System.Convert.ToInt32(r["hw_status"]);

            }
            else if (r["func_name"].ToString() == "report_analogy_signal_status")
            {
                type = "A";
                level = System.Convert.ToInt32(r["level"]);
                density = System.Convert.ToInt32(r["density"]);

            }
            else
                throw new Exception("Unknow Data type");

         
            place = System.Convert.ToInt32(r["place"]);
            group = System.Convert.ToInt32(r["group"]);
            kind = System.Convert.ToInt32(r["kind"]);
            dir = System.Convert.ToInt32(r["dir"]);

        }

        public string Key
        {

            get
            {
                return type + "-" + place.ToString() + "-" + group.ToString() + "-" + kind.ToString() + "-" + dir.ToString();
            }
        }


       public override bool Equals(object obj)
       {
           SVWS_Status to = obj as SVWS_Status;
           if (type == "A")
               return level == to.level;
           else if (type == "D")
               return run_status == to.run_status && hw_status == to.hw_status;
           else
               return true;

       }
    }
}
