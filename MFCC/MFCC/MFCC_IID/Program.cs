using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_IID
{
    class Program
    {
        public static MFCC_IID mfcc_iid;
        static void Main(string[] args)
        {
            mfcc_iid = new MFCC_IID("MFCC_IID", "IID", (int)RemoteInterface.RemotingPortEnum.MFCC_IID, (int)RemoteInterface.NotifyServerPortEnum.MFCC_IID,
         (int)RemoteInterface.ConsolePortEnum.MFCC_IID, "MFCC_IID", typeof(RemoteObj));


            ConsoleServer.WriteLine("MFCC_IID Start success!");
        }
    }
}
