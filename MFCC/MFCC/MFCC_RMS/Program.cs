using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_RMS
{
    class Program
    {
       public  static MFCC_RMS mfcc_rms;
        static void Main(string[] args)
        {

            int NotifyPort = -1, RemotingPort = -1, ConsolePort = -1;
            string mfccid = "MFCC_RMS";
            if (args.Length == 0 || args[0] == "MFCC_RMS")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_RMS;
                RemotingPort = (int)RemotingPortEnum.MFCC_RMS;
                ConsolePort = (int)ConsolePortEnum.MFCC_RMS;
                mfccid = "MFCC_RMS";

            }
            else if (args[0] == "MFCC_RMS2")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_RMS2;
                RemotingPort = (int)RemotingPortEnum.MFCC_RMS2;
                ConsolePort = (int)ConsolePortEnum.MFCC_RMS2;
                mfccid = "MFCC_RMS2";
            }
            else
                Environment.Exit(-1);
           
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
            mfcc_rms = new MFCC_RMS(mfccid, "RMS", RemotingPort, NotifyPort, ConsolePort, "MFCC_RMS", typeof(RemoteObj));


            ConsoleServer.WriteLine("MFCC_RMS Start success!"+RemotingPort);
            //try
            //{
            //    mfcc_rms = new MFCC_RMS("MFCC_RMS","RMS", (int)RemoteInterface.RemotingPortEnum.MFCC_RMS
            //        , (int)RemoteInterface.NotifyServerPortEnum.MFCC_RMS, (int)RemoteInterface.ConsolePortEnum.MFCC_RMS, "MFCC_RMS", typeof(RemoteObj));

            //    ConsoleServer.WriteLine("mfcc rms started!");
            //  //  testtask();
            //}
            //catch
            //{
            //    System.Environment.Exit(-1);
            //}
        }

     static    void testtask()
        {

       
      
         RemoteInterface.MFCC.I_MFCC_RMS robj=(RemoteInterface.MFCC.I_MFCC_RMS)  RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.MFCC.I_MFCC_RMS),
            RemoteBuilder.getRemoteUri("192.168.22.89",(int)RemoteInterface.RemotingPortEnum.MFCC_RMS,"MFCC_RMS" ));
             //   robj.SetModeAndPlanno("RMS235", 0, 2);
              System.Data.DataSet ds=  robj.getSendDSByFuncName("get_plan");
              Console.WriteLine(ds.Tables[0].Rows[0]["func_name"].ToString());
                byte planno=0, mode=0;
                robj.GetModeAndPlanno("RMS235", ref mode,ref  planno);
        }
    }
}
