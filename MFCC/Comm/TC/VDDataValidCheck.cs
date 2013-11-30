using System;
using System.Collections.Generic;
using System.Text;

namespace Comm.TC
{




  public   class VDDataValidCheck
    {


      //  public static int conflict_speed = 40, conflict_occupancy = 40;
         static int[] Value1;
         static int[] Value2;
         static char[] type1;
         static char[] type2;
         
         static object lockobj=new object();


          static RangeData[] rangeDatas;

       // public static System.Collections.ArrayList validRules = new System.Collections.ArrayList();
           static VDDataValidCheck()
          {
              LoadRule();
              
          }
        
        public static void LoadRule()
        {
            lock (lockobj)
            {

                loadSpeedOccupancyRule();
                loadRangeRule();

            }

        }

     
      private static void loadRangeRule()
      {
          System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
          System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();
          System.Data.Odbc.OdbcDataReader rd;
          cmd.Connection = cn;
          int num = 0;
          try
          {
              cn.Open();
              cmd.CommandText = "select count(*) from tblVDValidRange";
              num = System.Convert.ToInt32(cmd.ExecuteScalar());
              rangeDatas = new RangeData[num];

              cmd.CommandText = "select  type,maxvalue,minvalue from tblVDValidRange";
              rd = cmd.ExecuteReader();
              int i = 0;
              while (rd.Read())
              {
                  try
                  {
                      rangeDatas[i++] = new RangeData(System.Convert.ToString(rd[0]).Trim(), System.Convert.ToInt32(rd[1]), System.Convert.ToChar(rd[2]));
                  }
                  catch (Exception ex1)
                  {
                      RemoteInterface.ConsoleServer.WriteLine(ex1.Message + ex1.StackTrace);
                  }
                  //Value1[i] = System.Convert.ToInt32(rd[1]);
                  //type2[i] = System.Convert.ToChar(rd[2]);
                  //Value2[i] = System.Convert.ToInt32(rd[3]);

              }


              rd.Close();

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
      private static void loadSpeedOccupancyRule()
      {
          System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
          System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();
          System.Data.Odbc.OdbcDataReader rd;
          cmd.Connection = cn;
          int num = 0;
          try
          {
              cn.Open();
              cmd.CommandText = "select count(*) from tblVDInvalidPrinciple";
              num = System.Convert.ToInt32(cmd.ExecuteScalar());

              Value1 = new int[num];
              Value2 = new int[num];
              type1 = new char[num];
              type2 = new char[num];

              cmd.CommandText = "select  type1,value1,type2,value2 from tblVDInvalidPrinciple";
              rd = cmd.ExecuteReader();
              int i = 0;
              while (rd.Read())
              {
                  type1[i] = System.Convert.ToChar(rd[0]);
                  Value1[i] = System.Convert.ToInt32(rd[1]);
                  type2[i] = System.Convert.ToChar(rd[2]);
                  Value2[i] = System.Convert.ToInt32(rd[3]);
                  i++;

              }


              rd.Close();

          }
          catch (Exception ex)
          {
              type1 = new char[1];
              type2 = new char[1];
              Value1 = new int[1];
              Value2 = new int[1];
              type1[0] = 'O';
              type2[0] = 'S';
              Value1[0] = 40;
              Value2[0] = 40;

              RemoteInterface.ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
          }
          finally
          {
              cn.Close();
          }
      }
      public static bool IsValid(int speed, int occupancy, int volume)
      {
          return SpeedOcupancyCheck(speed, occupancy, volume) && rangeCheck(speed, occupancy, volume);

      }


      public  static bool SpeedVolumnRangeCheck(int speed,int volume)
      {
          bool ret = true;
          //   int max = 0, min = 0;
          for (int i = 0; i < rangeDatas.Length; i++)
          {
              switch (rangeDatas[i].type[0])
              {
                  case 'O':
                    //  ret = ret && (occupancy >= rangeDatas[i].minval && occupancy <= rangeDatas[i].maxval);
                      break;
                  case 'V':  //volume
                      ret = ret && (volume >= rangeDatas[i].minval && volume <= rangeDatas[i].maxval);
                      break;
                  case 'S':
                      ret = ret && (speed >= rangeDatas[i].minval && speed <= rangeDatas[i].maxval);
                      break;
              }




          }
          return ret;


      }

      private static bool rangeCheck(int speed, int occupancy, int volume)
      {
          bool ret = true;
       //   int max = 0, min = 0;
          for (int i = 0; i < rangeDatas.Length; i++)
          {
              switch (rangeDatas[i].type[0])
              {
                  case 'O':
                      ret = ret && (occupancy >= rangeDatas[i].minval && occupancy <=rangeDatas[i].maxval);
                      break;
                  case 'V':  //volume
                      ret = ret && (volume >= rangeDatas[i].minval && volume <= rangeDatas[i].maxval);
                      break;
                  case 'S':
                      ret = ret && (speed >= rangeDatas[i].minval && speed <= rangeDatas[i].maxval);
                      break;
              }

            
            

          }
          return ret;


      }
      private static  bool SpeedOcupancyCheck(int speed, int occupancy, int volume)
      {
          int value1 = 0, value2 = 0;
          bool result = true;
          for (int i = 0; i < type1.Length; i++)
          {
              switch (type1[i])
              {
                  case 'O':
                      value1 = occupancy;
                      break;
                  case 'V':  //volume
                      value1 = volume;
                      break;
                  case 'S':
                      value1 = speed;
                      break;
              }

              switch (type2[i])
              {
                  case 'O':
                      value2 = occupancy;
                      break;
                  case 'V':  //volume
                      value2 = volume;
                      break;
                  case 'S':
                      value2 = speed;
                      break;
              }

              result = result && !(value1 > Value1[i] && value2 > Value2[i]);

          }
          return result;
      }

  }

    class  RangeData
    {

        public string type;
        public int maxval, minval;
    public   RangeData (string type,int maxval,int minval)
        {
            this.maxval = maxval;
            this.minval = minval;
            this.type = type;
        }
    }
    
}
