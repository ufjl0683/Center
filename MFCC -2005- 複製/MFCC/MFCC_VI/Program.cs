using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_VI
{
    class Program
    {
        public static MFCC_VI mfcc_vi;
        static void Main(string[] args)
        {
            mfcc_vi = new MFCC_VI("MFCC_VI", "VI", (int)RemoteInterface.RemotingPortEnum.MFCC_VI, (int)RemoteInterface.NotifyServerPortEnum.MFCC_VI,
             (int)RemoteInterface.ConsolePortEnum.MFCC_VI, "MFCC_VI", typeof(RemoteObj));


            ConsoleServer.WriteLine("MFCC_VI Start success!");
        }
    }
}
