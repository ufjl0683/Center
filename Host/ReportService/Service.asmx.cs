using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Management;

namespace ReportService
{
    /// <summary>
    ///Service1 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class Service : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            string query = string.Format("SELECT * from Win32_Printer ");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection coll = searcher.Get();
            string ret = System.Environment.UserName; ;
            foreach (ManagementObject printer in coll)
            {
                foreach (PropertyData property in printer.Properties)
                {
                    ret += string.Format("{0}: {1}", property.Name, property.Value);
                }
            }



            return ret;
        }

        [WebMethod]
        public void printReport(int rptId)
        {
            ReportForm.ReportServer.PrintRoport(rptId);

            //this.HelloWorld
        }
        [WebMethod]
        public void SetMovingContructEvent(int id, string notifier, string  timeStamp, string lineID, string directionID, int startMileage, int endMileage, int blockTypeId, string blocklane, string description)
        {
            RemoteInterface.HC.I_HC_FWIS robj = (RemoteInterface.HC.I_HC_FWIS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_FWIS),
                RemoteInterface.RemoteBuilder.getRemoteUri(RemoteInterface.RemoteBuilder.getHostIP(), (int)RemoteInterface.RemotingPortEnum.HOST, "FWIS"));

            System.DateTime dt = System.Convert.ToDateTime(timeStamp);

            robj.SetMovingContructEvent(id, notifier, dt, lineID, directionID, startMileage, endMileage, blockTypeId, blocklane, description);
        
          
        }

          [WebMethod]
           public void CloseMovingConstructEvent(int id)
          {

                 RemoteInterface.HC.I_HC_FWIS robj = (RemoteInterface.HC.I_HC_FWIS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_FWIS),
                RemoteInterface.RemoteBuilder.getRemoteUri(RemoteInterface.RemoteBuilder.getHostIP(), (int)RemoteInterface.RemotingPortEnum.HOST, "FWIS"));

        

            robj.CloseMovingConstructEvent(id);
          }

        [WebMethod]
        public int SendSMS(string phoneNo, string body)
        {


            //string encodebody = "";
            //string urlbase = "http://202.39.48.216/kotsmsapi-1.php?username=fortech&password=123456&dstaddr={0}&smbody={1}&dlvtime=0";
            //byte[] codebig5 =stringToBig5Bytes(body);


            //encodebody = System.Web.HttpUtility.UrlEncode(codebig5);
            //string uristr = string.Format(urlbase, phoneNo, encodebody);


            //System.Net.WebRequest web = System.Net.HttpWebRequest.Create(new Uri(uristr
            //     , UriKind.Absolute)
            // );

            //System.IO.Stream stream = web.GetResponse().GetResponseStream();
            //System.IO.StreamReader rd = new System.IO.StreamReader(stream);
            //string res = rd.ReadToEnd();


            SNSCOMSERVER.SnsComObject sms = new SNSCOMSERVER.SnsComObject();
            int ret = sms.Login("203.66.172.133", 8001, "10371", "10371");
            if (ret != 0)
            {
                return 1;
            }

            string mesg =body;
            string[] messages = new string[mesg.Length % 70 == 0 ? mesg.Length / 70 : mesg.Length / 70 + 1];
            for (int i = 0; i < messages.Length; i++)
            {
                if (i == messages.Length - 1)
                    messages[i] = mesg.Substring(i * 70);
                else
                    messages[i] = mesg.Substring(i * 70, 70);
            }

            for (int i = 0; i < messages.Length; i++)
                ret = sms.SubmitBig5Message(phoneNo, messages[i]);




        //    ret = sms.SubmitBig5Message(phoneNo, body);

            if (ret != 0)
            {
                return 1;
                //throw new Exception("error!");
            }


            string smsid = sms.RespMessage;
            //do
            //{
            //    System.Threading.Thread.Sleep(1000);
            //    ret = sms.QryMessageStatus(phoneNo, smsid);
               
            //}
            //while (ret == 1 );
            //if (ret != 0)
            //    throw new Exception("error!");

            sms.Logout();
            return ret;


        }

        public  byte[] stringToBig5Bytes(string text)
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
