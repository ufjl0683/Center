using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_ETTU
{
    class Program
    {
        public static MFCC_ETTU mfcc_ettu;
        static void Main(string[] args)
        {
            mfcc_ettu = new MFCC_ETTU("MFCC_ETTU", "ETTU", (int)RemoteInterface.RemotingPortEnum.MFCC_ETTU, (int)RemoteInterface.NotifyServerPortEnum.MFCC_ETTU,
         (int)RemoteInterface.ConsolePortEnum.MFCC_ETTU, "MFCC_ETTU", typeof(RemoteObj));


            ConsoleServer.WriteLine("MFCC_ETTU Start success!");
        }
    }
}
