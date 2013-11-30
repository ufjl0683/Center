using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;


namespace MFCC_BS
{
    class Program
    {
        public static MFCC_BS mfcc_bs;
        static void Main(string[] args)
        {
           
                mfcc_bs = new MFCC_BS("MFCC_BS", "BS", (int)RemoteInterface.RemotingPortEnum.MFCC_BS, (int)RemoteInterface.NotifyServerPortEnum.MFCC_BS,
                 (int)RemoteInterface.ConsolePortEnum.MFCC_BS, "MFCC_BS", typeof(RemoteObj));


                ConsoleServer.WriteLine("MFCC_BS Start success!");
            



        }
    }
}
