using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using RemoteInterface;

namespace Comm.DB2
{
  public   class Db2
    {

       public static string MFCC_TYPE="";
    //  private static string m_db2ConnectionStr="dsn=TCS;uid=db2inst1;pwd=db2inst1";
      public static OdbcDataReader getReader(string sqlStr,OdbcConnection cn)
      {
           //  cn = new OdbcConnection(db2ConnectionStr);
          
              cn.Open();
              OdbcDataReader rd = new OdbcCommand(sqlStr, cn).ExecuteReader();
              return rd;
               


      }

      public static  string db2ConnectionStr
      {
          get{
//#if !DEBUG
//              if (MFCC_TYPE == "MFCC_VD2")
//                  return "dsn=TCS1;uid=db2inst1;pwd=db2inst1";
//              else
//#endif
              return DbCmdServer.getDbConnectStr();
          }
      }

      public static OdbcDataReader getDeviceConfigReader(OdbcConnection cn, string mfccid)
      {
          return getReader("select devicename,ip,port ,hw_status_1,hw_status_2,hw_status_3,hw_status_4,op_mode,op_status from tbldeviceconfig where mfccid='" + mfccid + "'  and enable='Y'", cn);
      }

      public static string getTimeStampString(System.DateTime dt)
      {
          return string.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}.000000", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute,dt.Second); ;
      }

      public static string getTimeStampString(int year,int month,int day,int hour,int min,int sec)
      {

          return string.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}.000000", year, month, day,hour, min, sec); ;

      }

      public static int getFiledInx(System.Data.Odbc.OdbcDataReader rd,string fieldName)
      {
           if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
               return rd.GetOrdinal(fieldName);

           for (int i = 0; i < rd.FieldCount; i++)
               if(fieldName.ToUpper()==
            System.Text.Encoding.Unicode.GetString(System.Text.Encoding.BigEndianUnicode.GetBytes(rd.GetName(i))).ToUpper())  
              return i;


        throw new Exception(fieldName + " not found");


           
      }
  }
}
