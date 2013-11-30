using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HC;
using RemoteInterface.MFCC;
using Comm;


namespace MFCC_VD
{
    class Program
    {

        public static MFCC_VD mfcc_vd;
        static void Main(string[] args)
        {
            int NotifyPort=-1, RemotingPort=-1, ConsolePort=-1;
            string mfccid="MFCC_VD1";
            if (args.Length == 0 || args[0]=="MFCC_VD1")
            {
                NotifyPort=(int)NotifyServerPortEnum.MFCC_VD1;
                RemotingPort=(int)RemotingPortEnum.MFCC_VD1;
                ConsolePort = (int)ConsolePortEnum.MFCC_VD1;
                mfccid = "MFCC_VD1";
                
            }
            else if(args[0]=="MFCC_VD2")
            {
                 NotifyPort=(int)NotifyServerPortEnum.MFCC_VD2;
                RemotingPort=(int)RemotingPortEnum.MFCC_VD2;
                ConsolePort = (int)ConsolePortEnum.MFCC_VD2;
                mfccid = "MFCC_VD2";
            }
            else if (args[0] == "MFCC_VD3")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_VD3;
                RemotingPort = (int)RemotingPortEnum.MFCC_VD3;
                ConsolePort = (int)ConsolePortEnum.MFCC_VD3;
                mfccid = "MFCC_VD3";
            }
            else if (args[0] == "MFCC_VD4")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_VD4;
                RemotingPort = (int)RemotingPortEnum.MFCC_VD4;
                ConsolePort = (int)ConsolePortEnum.MFCC_VD4;
                mfccid = "MFCC_VD4";
            }
            else if (args[0] == "MFCC_VD5")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_VD5;
                RemotingPort = (int)RemotingPortEnum.MFCC_VD5;
                ConsolePort = (int)ConsolePortEnum.MFCC_VD5;
                mfccid = "MFCC_VD5";
            }
            else if (args[0] == "MFCC_VD6")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_VD6;
                RemotingPort = (int)RemotingPortEnum.MFCC_VD6;
                ConsolePort = (int)ConsolePortEnum.MFCC_VD6;
                mfccid = "MFCC_VD6";
            }
            else if (args[0] == "MFCC_VD7")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_VD7;
                RemotingPort = (int)RemotingPortEnum.MFCC_VD7;
                ConsolePort = (int)ConsolePortEnum.MFCC_VD7;
                mfccid = "MFCC_VD7";
            }
            else if (args[0] == "MFCC_VD8")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_VD8;
                RemotingPort = (int)RemotingPortEnum.MFCC_VD8;
                ConsolePort = (int)ConsolePortEnum.MFCC_VD8;
                mfccid = "MFCC_VD8";
            }
            else if (args[0] == "MFCC_VD9")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_VD9;
                RemotingPort = (int)RemotingPortEnum.MFCC_VD9;
                ConsolePort = (int)ConsolePortEnum.MFCC_VD9;
                mfccid = "MFCC_VD9";
            }
            else if (args[0] == "MFCC_VD10")
            {
                NotifyPort = (int)NotifyServerPortEnum.MFCC_VD10;
                RemotingPort = (int)RemotingPortEnum.MFCC_VD10;
                ConsolePort = (int)ConsolePortEnum.MFCC_VD10;
                mfccid = "MFCC_VD10";
            }
            
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
            mfcc_vd = new MFCC_VD(mfccid,"VD", RemotingPort, NotifyPort, ConsolePort, "MFCC_VD", typeof(RemoteObj));


            ConsoleServer.WriteLine("MFCC_VD Start success!");
            //Console.ReadKey();
            //mfcc_vd.getTcManager()["VD-N6-E-11.1"].getCurrentTcCommStatusStr(); ;
            //Console.ReadKey();

        }


       
    }
}
