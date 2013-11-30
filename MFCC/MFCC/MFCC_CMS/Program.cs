using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_CMS
{
    class Program
    {

        public static MFCC_CMS mfcc_cms;
        static void Main(string[] args)
        {
            int NotifyPort = -1, RemotingPort = -1, ConsolePort = -1;
            string mfccid = "MFCC_CMS";
            if (args.Length == 0 || args[0] == "MFCC_CMS")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_CMS;
                RemotingPort = (int)RemotingPortEnum.MFCC_CMS;
                ConsolePort = (int)ConsolePortEnum.MFCC_CMS;
                mfccid = "MFCC_CMS";

            }
            else if (args[0] == "MFCC_CMS2")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_CMS2;
                RemotingPort = (int)RemotingPortEnum.MFCC_CMS2;
                ConsolePort = (int)ConsolePortEnum.MFCC_CMS2;
                mfccid = "MFCC_CMS2";
            }
            else if (args[0] == "MFCC_CMS3")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_CMS3;
                RemotingPort = (int)RemotingPortEnum.MFCC_CMS3;
                ConsolePort = (int)ConsolePortEnum.MFCC_CMS3;
                mfccid = "MFCC_CMS3";
            }
            else if (args[0] == "MFCC_CMS4")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_CMS4;
                RemotingPort = (int)RemotingPortEnum.MFCC_CMS4;
                ConsolePort = (int)ConsolePortEnum.MFCC_CMS4;
                mfccid = "MFCC_CMS4";
            }
            else if (args[0] == "MFCC_CMS5")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_CMS5;
                RemotingPort = (int)RemotingPortEnum.MFCC_CMS5;
                ConsolePort = (int)ConsolePortEnum.MFCC_CMS5;
                mfccid = "MFCC_CMS5";
            }
            
            mfcc_cms = new MFCC_CMS(mfccid,"CMS", RemotingPort,
              NotifyPort, ConsolePort, "MFCC_CMS", typeof(RemoteObj));
            ConsoleServer.WriteLine("mfcc stated");
          //  testtask();
        }

        static void testtask()
        {
          //  mfcc_cms.getTcManager()["CMS
        }
    }
}
