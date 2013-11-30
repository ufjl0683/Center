using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;



namespace MFCC_CSLS
{
    class Program
    {
        public static MFCC_CSLS mfcc_csls;
        static void Main(string[] args)
        {

            int NotifyPort = -1, RemotingPort = -1, ConsolePort = -1;
            string mfccid = "MFCC_CSLS";
            if (args.Length == 0 || args[0] == "MFCC_CSLS")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_CSLS;
                RemotingPort = (int)RemotingPortEnum.MFCC_CSLS;
                ConsolePort = (int)ConsolePortEnum.MFCC_CSLS;
                mfccid = "MFCC_CSLS";

            }
            else if (args[0] == "MFCC_CSLS2")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_CSLS2;
                RemotingPort = (int)RemotingPortEnum.MFCC_CSLS2;
                ConsolePort = (int)ConsolePortEnum.MFCC_CSLS2;
                mfccid = "MFCC_CSLS2";
            }


            mfcc_csls = new MFCC_CSLS(mfccid, "CSLS", RemotingPort, NotifyPort, ConsolePort, "MFCC_CSLS", typeof(RemoteObj));
            ConsoleServer.WriteLine("mfcc stated");
        }
    }
}
