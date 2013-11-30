using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;

namespace SLProcessController.Web
{
    /// <summary>
    /// Service 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class Service : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public HostInfo[] getHostInfos()
        {
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection("dsn=TCS;uid=db2inst1;pwd=db2inst1");
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select HostID,HostIP from tblHostConfig  where  hosttype='1'");
            System.Data.Odbc.OdbcDataReader rd;
            try
            {
                cmd.Connection=cn;
                cn.Open();
                rd = cmd.ExecuteReader();
                System.Collections.Generic.List<HostInfo> lst = new List<HostInfo>();
                while (rd.Read())
                {
                    HostInfo hi = new HostInfo();
                    hi.HostName = rd["HostID"].ToString();
                    hi.IP = rd["HostIP"].ToString();
                    lst.Add(hi);

                }
                return lst.ToArray();
            }
           
            finally
            {
                cn.Close();
            }
        }

        [WebMethod]
        public void ChangeProcessState(string HostIp, string ProcessName, bool bPlay)
        {
            RemoteInterface.I_ProcessManager robj = RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.I_ProcessManager),
                RemoteInterface.RemoteBuilder.getRemoteUri(HostIp, (int)RemoteInterface.RemotingPortEnum.ProcessManager, RemoteInterface.RemotingPortEnum.ProcessManager.ToString())) as RemoteInterface.I_ProcessManager;

           robj.SetProcessRunningState(ProcessName, (bPlay) ? 1 : 0);
           try
           {
               LogMfccStart(ProcessName, bPlay);
           }
           catch { ;}


        }

        [WebMethod]
        public  void LogMfccStart(string ProcessName, bool bPlay)
        {
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection("dsn=TCS;uid=db2inst1;pwd=db2inst1");
         
             string sql=   "insert into  tblMFCCStateLog (TIMESTAMP,MFCCId,CONN_State) values('{0}','{1}',{2})" ;
             System.Data.Odbc.OdbcCommand cmd=new System.Data.Odbc.OdbcCommand(string.Format(sql, getTimeStampString(DateTime.Now), ProcessName, bPlay ? 1 : 3));
             try
             {
                 cn.Open();
                 cmd.Connection = cn;
                 cmd.ExecuteNonQuery();
             }
             finally
             {
                 cn.Close();
             }
 



        }

        public  string getTimeStampString(System.DateTime dt)
        {
            return string.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}.000000", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second); ;
        }

         [WebMethod]
        public int getDbQueueCnt( string mfcc_type,string ip ,int port,bool isHost)
        {

            if (!isHost)
            {
                RemoteInterface.MFCC.I_MFCC_Base robj = (RemoteInterface.MFCC.I_MFCC_Base)RemoteInterface.RemoteBuilder.GetRemoteObj(
                    typeof(RemoteInterface.MFCC.I_MFCC_Base), RemoteInterface.RemoteBuilder.getRemoteUri(ip, port, mfcc_type));
                if (robj == null)
                    return 0;
                else
                    return robj.getCurrentDBQueueCnt();
            }
            else
            {
                RemoteInterface.HC.I_HC_Comm robj = (RemoteInterface.HC.I_HC_Comm)RemoteInterface.RemoteBuilder.GetRemoteObj(
                   typeof(RemoteInterface.HC.I_HC_Comm), RemoteInterface.RemoteBuilder.getRemoteUri(ip, port, "Comm"));
                if (robj == null)
                    return 0;
                else
                    return robj.getCurrentDBQueueCnt();
            }
             

        }


      


        [WebMethod]

        public   ProcessInfo[] GetProcessInfo(string ip)
        {
            //RemoteInterface.I_ProcessManager robj;
            //if (Application["robj"] == null)
            //{
            RemoteInterface.I_ProcessManager robj = RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.I_ProcessManager),
                      RemoteInterface.RemoteBuilder.getRemoteUri(ip, (int)RemoteInterface.RemotingPortEnum.ProcessManager, RemoteInterface.RemotingPortEnum.ProcessManager.ToString())) as RemoteInterface.I_ProcessManager;
              //   Application["robj"] = robj;
            //}
            //else
            //{
            //    robj = Application["robj"] as RemoteInterface.I_ProcessManager;
            //}
            try
            {
                System.Data.DataTable tbl = robj.getProcessStatus();
                System.Collections.Generic.List<ProcessInfo> lst = new List<ProcessInfo>();
                foreach (System.Data.DataRow r in tbl.Rows)
                {
                    lst.Add(new ProcessInfo()
                    {
                        PID = System.Convert.ToInt32(r["pid"]),
                        ConsolePort = System.Convert.ToInt32(r["console_port"]),
                        CPU_Time = System.Convert.ToDouble(r["cpu_time"]),
                        IsAlive = System.Convert.ToBoolean(r["isAlive"]),
                        Mermory = System.Convert.ToInt64(r["memory"]),
                        ExecutiongString = r["execute_string"].ToString(),
                        ProcessName = r["process_name"].ToString(),
                        State = System.Convert.ToInt32(r["state"]),
                         MFCC_TYPE= r["mfcc_type"].ToString(),
                         HostIP=ip
                        


                    });
                }
                return lst.ToArray();
            }
            catch
            {
                return null;
            }
        }


        [WebMethod]
        public void SendSMS(string phoneNo, string body)
        {


            string encodebody = "";
            string urlbase = "http://202.39.48.216/kotsmsapi-1.php?username=fortech&password=123456&dstaddr={0}&smbody={1}&dlvtime=0";
            byte[] codebig5 = StringToBig5Bytes(body);


            encodebody = System.Web.HttpUtility.UrlEncode(codebig5);
            string uristr = string.Format(urlbase, phoneNo, encodebody);


            System.Net.WebRequest web = System.Net.HttpWebRequest.Create(new Uri(uristr
                 , UriKind.Absolute)
             );

            System.IO.Stream stream = web.GetResponse().GetResponseStream();
            System.IO.StreamReader rd = new System.IO.StreamReader(stream);
            string res = rd.ReadToEnd();
        }

        public byte[] StringToBig5Bytes(string text)
        {


            //byte[] b = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode.GetBytes(text));
            //return b;

            char[] chars = text.ToCharArray();
            byte[] b;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            for (int i = 0; i < text.Length; i++)
            {

                if (chars[i] >= 0xe000 && chars[i] <= 0xE4BE)
                {
                    ms.WriteByte((byte)((chars[i] + 0x1A40) / 256));
                    ms.WriteByte((byte)((chars[i] + 0x1A40) % 256));
                }
                else
                {
                    b = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode.GetBytes(chars, i, 1));

                    for (int j = 0; j < b.Length; j++)
                        ms.WriteByte(b[j]);
                }
                //if (b.Length == 2 && b[1] * 256 + b[0] >= 0xE000 && b[1] * 256 + b[0] <= 0xE05f)
                //{
                //    ms.WriteByte((byte)(b[0] + 0x1A));
                //    ms.WriteByte(b[1]);

                //}
                //else
                //{

                //}
            }

            return ms.ToArray();

        }

    }
}
