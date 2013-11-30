using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_MAS
{
    class Program
    {
        public static MFCC_MAS mfcc_mas;
        static void Main(string[] args)
        {
            mfcc_mas = new MFCC_MAS("MFCC_MAS", "MAS", (int)RemoteInterface.RemotingPortEnum.MFCC_MAS
                   , (int)RemoteInterface.NotifyServerPortEnum.MFCC_MAS, (int)RemoteInterface.ConsolePortEnum.MFCC_MAS, "MFCC_MAS", typeof(RemoteObj));

            ConsoleServer.WriteLine("mfcc mas started!");
        }
    }
}
