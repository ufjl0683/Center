using System;
using System.Collections.Generic;
using System.Text;

namespace MFCC_AVI
{
    class Program
    {
        public static MFCC_AVI mfcc_avi;
       // public static  Host.AVI.AVIManager avimgr;

        static void Main(string[] args)
        {

            mfcc_avi=new MFCC_AVI("MFCC_AVI","AVI",(int)RemoteInterface.RemotingPortEnum.MFCC_AVI,(int)RemoteInterface.NotifyServerPortEnum.MFCC_AVI,
             (int)RemoteInterface.ConsolePortEnum.MFCC_AVI    ,"MFCC_AVI",typeof(RemoteObj));

          //  avimgr = new Host.AVI.AVIManager();
        }
    }
}
