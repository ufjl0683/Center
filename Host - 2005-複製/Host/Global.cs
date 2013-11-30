using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using RemoteInterface;
namespace Host
{
    public class Global
    {

        public static string Db2ConnectionString = "dsn=TCS;uid=db2inst1;pwd=db2inst1";

        public static int getFiledInx(System.Data.Odbc.OdbcDataReader rd, string fieldName)
        {
            if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
                return rd.GetOrdinal(fieldName);

            for (int i = 0; i < rd.FieldCount; i++)
                if (fieldName.ToUpper() ==
             System.Text.Encoding.Unicode.GetString(System.Text.Encoding.BigEndianUnicode.GetBytes(rd.GetName(i))).ToUpper())
                    return i;


            throw new Exception(fieldName + " not found");



        }
        public static Color[] getColorByPattern(string str, string pattern, string replace, Color[] colors)
        {
            Color[] retColor;
            string resultStr;
            int replacePosition = str.IndexOf(pattern);
            if (replacePosition < 0)
                return colors;

            resultStr = str.Replace(pattern, replace);
            
            retColor = new Color[resultStr.Length];

            for (int i = 0; i < resultStr.Length; i++)
                if (i >= replacePosition && i < replacePosition + replace.Length)
                    retColor[i] = colors[replacePosition];
                else if (i >= replacePosition + replace.Length)
                    retColor[i] = colors[i - replace.Length + 1];
                else
                    retColor[i] = colors[i];


            return retColor;
            
        }


        public static byte[] getColorBytesByPattern(string str, string pattern, string replace, byte[] colors)
        {
            byte[] retColor;
            string resultStr;
            int replacePosition = str.IndexOf(pattern);
            if (replacePosition < 0)
                return colors;

            resultStr = str.Replace(pattern, replace);

            retColor = new byte[resultStr.Length];

            for (int i = 0; i < resultStr.Length; i++)
                if (i >= replacePosition && i < replacePosition + replace.Length)
                    retColor[i] = colors[replacePosition];
                else if (i >= replacePosition + replace.Length)
                    retColor[i] = colors[i - replace.Length + 1];
                else
                    retColor[i] = colors[i];


            return retColor;

        }

        public static int getAlarmId()
        {
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("SELECT NEXTVAL FOR DB2INST1.SEQ_ALARM FROM SYSIBM.SYSDUMMY1 ");
            cmd.Connection = cn;
            try
            {
                cn.Open();
               return System.Convert.ToInt32( cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message+","+ex.StackTrace);
                throw ex;
            }
            finally
            {
                try
                {
                    cn.Close();
                }
                catch { ;}
            }

        }

        public static int getEventId()
        {
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("SELECT NEXTVAL FOR DB2INST1.SEQ_EVENT FROM SYSIBM.SYSDUMMY1");
            cmd.Connection = cn;
            try
            {
                cn.Open();
                return System.Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                throw ex;
            }
            finally
            {
                try
                {
                    cn.Close();
                }
                catch { ;}
            }

        }

        public static Host.Event.EventMode getEventMode(int alarmClass)
        {
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("SELECT login_mode From tblSysAlarmConfig where alarmClass="+alarmClass);
            cmd.Connection = cn;
            try
            {
                cn.Open();
                return ( Host.Event.EventMode)System.Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                throw ex;
            }
            finally
            {
                try
                {
                    cn.Close();
                }
                catch { ;}
            }

        }

    }
}
