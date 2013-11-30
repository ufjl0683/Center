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

       public static bool IsTunnelGearing(string tunnelid,int placeid,ref string[] cmsname,ref string[] cmsmessage,ref string[] lcsname)
        {
            bool isGearing = false;
            System.Collections.Generic.List<string> cmsnames = new List<string>();
            System.Collections.Generic.List<string> cmsmessages = new List<string>();
            System.Collections.Generic.List<string> lcsnames = new List<string>();

            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(DbCmdServer.getDbConnectStr());
           // string dir=(placeid%2==0)?"E":"W";
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand(string.Format("select Gearing,CMS_DEVICENAME,CMS_MESSAGE,LCS_DEVICENAME from VWRSP_TUNNELGEARING where tuncode='{0}'",tunnelid));
            cmd.Connection = cn;
            System.Data.Odbc.OdbcDataReader rd;
            try
            {
                cn.Open();
                rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    isGearing = rd[0].ToString().Trim() == "Y" ? true : false;
                    cmsnames.Add(rd[1].ToString());
                    cmsmessages.Add(rd[2].ToString());
                    lcsnames.Add(rd[3].ToString());
                }
                cmsname = cmsnames.ToArray();
                cmsmessage = cmsmessages.ToArray();
                lcsname = lcsnames.ToArray();

                rd.Close();
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                return false;
            }
            finally
            {
                cn.Close();
             
            }

            return isGearing;
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


        public static int getProcessTimeByAlarmClass(int alarmclass)
        {
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select avgtime from TBLSYSALARMCONFIG where alarmclass="+alarmclass);
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

        public static Host.Event.EventMode getEventMode(int alarmClass ,out bool isLock, out string desctiption)
        {
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("SELECT login_mode,isLock,description From tblSysAlarmConfig where alarmClass="+alarmClass);
            cmd.Connection = cn;
            
            try
            {
                cn.Open();
                System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    isLock=System.Convert.ToInt32(rd[1])==1? true:false;
                    desctiption = rd[2].ToString();
                    return ( Host.Event.EventMode)System.Convert.ToInt32(rd[0]);
                }
                else
                {
                    throw new Exception("Can not found alarm class " + alarmClass);
                }
               // return ( Host.Event.EventMode)System.Convert.ToInt32(cmd.ExecuteScalar());
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

        public static Host.Event.EventMode getEventModeBySectionID(string sectid,int alarmClass, ref bool isLock, ref string description )
        {
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("SELECT login_mode,IsLock,Description From vwSectionAlarmLoginMode where alarmClass=" + alarmClass+" and sectionid='"+sectid+"'");
            cmd.Connection = cn;
            try
            {
                cn.Open();
                System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    isLock=System.Convert.ToInt32(rd[1])==1? true:false;
                    description=rd[2].ToString();
                    return (Host.Event.EventMode)System.Convert.ToInt32(rd[0]);

                }
                else
                {
                    throw new Exception(" event not found at  alarmclass:" + alarmClass + "," + "sectionid:" + sectid + ",isLook");
                }

               
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
