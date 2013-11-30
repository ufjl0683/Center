using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_RD
{
    class Program
    {
        public static MFCC_RD mfcc_rd;
        static void Main(string[] args)
        {
            mfcc_rd = new MFCC_RD("MFCC_RD", "RD", (int)RemoteInterface.RemotingPortEnum.MFCC_RD, (int)RemoteInterface.NotifyServerPortEnum.MFCC_RD,
               (int) RemoteInterface.ConsolePortEnum.MFCC_RD, "MFCC_RD", typeof(RemoteObj));


            ConsoleServer.WriteLine("MFCC_RD Start success!");
        }
    }
}
