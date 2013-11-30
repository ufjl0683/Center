using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HC;
using RemoteInterface.MFCC;
using Comm;

namespace MFCC_RGS
{
    class Program
    {
        public static MFCC_RGS mfcc_rgs;
        static void Main(string[] args)
        {
         mfcc_rgs=   new MFCC_RGS("MFCC_RGS","RGS",(int)RemoteInterface.RemotingPortEnum.MFCC_RGS,
             (int)RemoteInterface.NotifyServerPortEnum.MFCC_RGS,(int)RemoteInterface.ConsolePortEnum.MFCC_RGS,"MFCC_RGS",typeof(RemoteObj)  );


        }
    }
}
