using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_WIS
{
    class Program
    {

       public  static MFCC_WIS mfcc_wis;
        static void Main(string[] args)
        {
            mfcc_wis = new MFCC_WIS("MFCC_WIS","WIS", (int)RemoteInterface.RemotingPortEnum.MFCC_WIS,
            (int)RemoteInterface.NotifyServerPortEnum.MFCC_WIS, (int)RemoteInterface.ConsolePortEnum.MFCC_WIS, "MFCC_WIS", typeof(RemoteObj));
            ConsoleServer.WriteLine("mfcc started");
        }
    }
}
