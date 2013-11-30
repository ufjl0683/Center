using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_SCM
{
    class Program
    {
        public static MFCC_SCM mfcc_scm;
        static void Main(string[] args)
        {
            try
            {
                mfcc_scm = new MFCC_SCM("MFCC_SCM", "SCM", (int)RemoteInterface.RemotingPortEnum.MFCC_SCM
                    , (int)RemoteInterface.NotifyServerPortEnum.MFCC_SCM, (int)RemoteInterface.ConsolePortEnum.MFCC_SCM, "MFCC_SCM", typeof(RemoteObj));

                ConsoleServer.WriteLine("mfcc scm started!");
                //  testtask();
            }
            catch
            {
                System.Environment.Exit(-1);
            }
        }
    }
}
