using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;


namespace MFCC_FS
{
    class Program
    {
        public static MFCC_FS mfcc_fs;
        static void Main(string[] args)
        {
            try
            {
                mfcc_fs = new MFCC_FS("MFCC_FS", "FS", (int)RemoteInterface.RemotingPortEnum.MFCC_FS
                    , (int)RemoteInterface.NotifyServerPortEnum.MFCC_FS, (int)RemoteInterface.ConsolePortEnum.MFCC_FS, "MFCC_FS", typeof(RemoteObj));

                ConsoleServer.WriteLine("mfcc fs started!");
                //  testtask();
            }
            catch
            {
                System.Environment.Exit(-1);
            }
        }
    }
}
