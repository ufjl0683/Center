using System;
using System.Collections.Generic;
using System.Text;

namespace MFCC_ETAG
{
    class Program
    {
        public static MFCC_ETAG mfcc_etag;
       // public static  Host.AVI.AVIManager avimgr;

        static void Main(string[] args)
        {

            mfcc_etag=new MFCC_ETAG("MFCC_ETAG","ETAG",(int)RemoteInterface.RemotingPortEnum.MFCC_ETAG,(int)RemoteInterface.NotifyServerPortEnum.MFCC_ETAG,
             (int)RemoteInterface.ConsolePortEnum.MFCC_ETAG    ,"MFCC_ETAG",typeof(RemoteObj));

          //  avimgr = new Host.AVI.AVIManager();
        }
    }
}
