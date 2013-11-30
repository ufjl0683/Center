using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using RemoteInterface;
using System.Collections;

namespace Host
{
  public   class VDJamEvalue
    {

      Hashtable hsLocation = new Hashtable();
      

      public VDJamEvalue()
      {

          LoatTable();
      }

      public void LoatTable()
      {

          lock (this)
          {
              OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
           
              
              try
              {
                  
                  OdbcCommand cmd = new OdbcCommand();
                  OdbcDataReader rd = null;
                  cmd.Connection = cn;
                  this.hsLocation.Clear();
                  cn.Open();
                  cmd.CommandText = "select location,level,ls,rs,logic,lo,ro from tblVDDegree order by location ,level";
                  rd = cmd.ExecuteReader();
                  while (rd.Read())
                  {
                      if (!hsLocation.Contains(rd[0].ToString()))
                          hsLocation.Add(rd[0].ToString(), new Rules(rd[0].ToString()));
                      //
                      ((Rules)hsLocation[rd[0].ToString()]).AddRule((int)rd[1], (int)rd[2], (int)rd[3], rd[4] as string, (int)rd[5], (int)rd[6]);
                      //  }
                  }
                  ConsoleServer.WriteLine("雍塞程度評估表載入完成!");

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

      public int getLevel(string location,int lval,int rval)
      {
          lock (this)
          {
              //try
              //{
              if(lval==-1 || rval==-1)
                    return -1;

                return ((Rules)hsLocation[location]).getLevel(lval, rval);
              //}
              //catch
              //{
                  //ConsoleServer.WriteLine("壅塞評估表 位置:" + location + "找不到!");
              //}
            
          }
      }


    }


  internal  class Rules
    {
        System.Collections.ArrayList rules=new ArrayList();
        public string location;
      // public string dependOn = "O";//O occupancy, S speed
  internal  Rules(string location)
        {
            this.location = location;
        }

        public void AddRule(int level,int lLow,int lHigh, string logic,int hLow , int hHigh)
        {
            rules.Add(new Rule(level, lLow, lHigh, logic, hLow, hHigh));
        }

       public int  getLevel(int lval,int rval)
       {
         
           foreach (Rule r in rules)
           {
               if (r.getLevel(lval, rval) >= 0)
                   return r.getLevel(lval, rval);
           }

           throw new Exception(string.Format("can not find levle in Jam table!,location:{0},lval:{1},rval{2}",location,lval,rval));
       }
    }


   internal class Rule
    {
        int level; int llow; int lhigh; string logic; int rlow; int rhigh;
       
         internal   Rule(int level,int llow,int lhigh, string logic, int  rlow,int rhigh)
        {

            this.llow = llow;
            this.lhigh = lhigh;
            this.rlow = rlow;
            this.rhigh = rhigh;
            this.level = level;
            this.logic=logic;
        }

       public int getLevel(int lval,int rval)
       {
           if(logic=="S")
               if( lval>=llow && lval < lhigh) // && rval>=rlow && rval <rhigh)
                   return   level;
               else
                   return -1;
           else
               if ( rval >= rlow && rval < rhigh)
                   return level;
               else
                   return -1;


       }
    }
}
