using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_TEM
{
    class Program
    {
        public static MFCC_TEM mfcc_tem;
        static void Main(string[] args)
        {
            mfcc_tem = new MFCC_TEM("MFCC_TEM", "TEM", (int)RemoteInterface.RemotingPortEnum.MFCC_TEM, (int)RemoteInterface.NotifyServerPortEnum.MFCC_TEM,
         (int)RemoteInterface.ConsolePortEnum.MFCC_TEM, "MFCC_TEM", typeof(RemoteObj));


            ConsoleServer.WriteLine("MFCC_TEM Start success!");
        }
    }
}
