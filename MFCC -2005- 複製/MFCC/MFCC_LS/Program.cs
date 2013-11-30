using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_LS
{
    class Program
    {
        public static MFCC_LS mfcc_ls;
        static void Main(string[] args)
        {
            mfcc_ls = new MFCC_LS("MFCC_LS", "LS", (int)RemoteInterface.RemotingPortEnum.MFCC_LS, (int)RemoteInterface.NotifyServerPortEnum.MFCC_LS,
             (int)RemoteInterface.ConsolePortEnum.MFCC_LS, "MFCC_LS", typeof(RemoteObj));


            ConsoleServer.WriteLine("MFCC_LS Start success!");
        }
    }
}
