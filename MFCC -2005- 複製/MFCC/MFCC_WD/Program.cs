using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_WD
{
    class Program
    {
        public static MFCC_WD mfcc_wd;
        static void Main(string[] args)
        {

            int NotifyPort = -1, RemotingPort = -1, ConsolePort = -1;
            string mfccid = "MFCC_CMS";
            if (args.Length == 0 || args[0] == "MFCC_WD")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_WD;
                RemotingPort = (int)RemotingPortEnum.MFCC_WD;
                ConsolePort = (int)ConsolePortEnum.MFCC_WD;
                mfccid = "MFCC_WD";

            }
            else if (args[0] == "MFCC_WD2")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_WD2;
                RemotingPort = (int)RemotingPortEnum.MFCC_WD2;
                ConsolePort = (int)ConsolePortEnum.MFCC_WD2;
                mfccid = "MFCC_WD2";
            }
            mfcc_wd = new MFCC_WD(mfccid, "WD", RemotingPort,
           NotifyPort, ConsolePort, "MFCC_WD", typeof(RemoteObj));
            ConsoleServer.WriteLine("mfccwd  stated");
            /*
            mfcc_wd = new MFCC_WD("MFCC_WD", "WD", (int)RemoteInterface.RemotingPortEnum.MFCC_WD, (int)RemoteInterface.NotifyServerPortEnum.MFCC_WD,
             (int)RemoteInterface.ConsolePortEnum.MFCC_WD, "MFCC_WD", typeof(RemoteObj));
             * */

            ConsoleServer.WriteLine("MFCC_WD Start success!");
            
        }
    }
}
