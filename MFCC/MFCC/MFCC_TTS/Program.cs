using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_TTS
{
    class Program
    {
        public static MFCC_TTS mfcc_tts;
        static void Main(string[] args)
        {
            try
            {
                mfcc_tts = new MFCC_TTS("MFCC_TTS", "TTS", (int)RemoteInterface.RemotingPortEnum.MFCC_TTS
                    , (int)RemoteInterface.NotifyServerPortEnum.MFCC_TTS, (int)RemoteInterface.ConsolePortEnum.MFCC_TTS, "MFCC_TTS", typeof(RemoteObj));

                ConsoleServer.WriteLine("mfcc tts started!");
                //  testtask();
            }
            catch
            {
                System.Environment.Exit(-1);
            }
        }
    }
}
