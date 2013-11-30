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
    /// Service 的摘要描述
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
                    ret+=string.Format("{0}: {1}", property.Name, property.Value);
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
    }
}
