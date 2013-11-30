using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_CMSRST
{
    class Program
    {
        public static MFCC_CMSRST mfcc_cmsrst;
        static void Main(string[] args)
        {
            int NotifyPort = -1, RemotingPort = -1, ConsolePort = -1;
            string mfccid = "MFCC_CMSRST";
            //if (args.Length == 0 || args[0] == "MFCC_CMS")
            //{
                NotifyPort = (int)NotifyServerPortEnum.MFCC_CMSRST;
                RemotingPort = (int)RemotingPortEnum.MFCC_CMSRST;
                ConsolePort = (int)ConsolePortEnum.MFCC_CMSRST;
                mfccid = "MFCC_CMSRST";

            //}
            mfcc_cmsrst = new MFCC_CMSRST(mfccid, "CMSRST", RemotingPort,
            NotifyPort, ConsolePort, "MFCC_CMSRST", typeof(RemoteObj));
          //  ConsoleServer.WriteLine(mfcc_cmsrst.getTcManager()["CMS-N1-S-158-N-2"].DeviceName);

           // System.Threading.Thread.Sleep(1000);
           //((Comm.TC.CMSRSTTC)mfcc_cmsrst.getTcManager()["CMS-N1-S-158-N-2"]).TC_SendDisplay(0, 0, 0, "系統測試", new byte[] { 0x30, 0x30, 0x30, 0x30 }, new byte[] { 0 });
           //System.Data.DataSet ds = ((Comm.TC.CMSRSTTC)mfcc_cmsrst.getTcManager()["CMS-N1-S-158-N-2"]).TC_GetDisplay(0);

           //System.Data.DataRow r = ds.Tables[0].Rows[0];
            ConsoleServer.WriteLine("mfcc_cmsrst stated");
            Console.ReadKey();
            (mfcc_cmsrst.getTcManager()["CMS-N1-S-158-N-2"] as Comm.OutputTCBase).setDisplayCompareCycle(2);
        }
    }
}
