using System;
using System.Collections.Generic;
using System.Text;
using Comm;
using RemoteInterface;


namespace Host
{
    class ScriptsManager
    {

        Comm.Protocol[] protocols;
        System.Collections.Hashtable protocolhash = new System.Collections.Hashtable();
      internal ScriptsManager()
        {
           string scriptpath= Util.CPath(AppDomain.CurrentDomain.BaseDirectory+@"Scripts");
           string[] files =System.IO.Directory.GetFiles(scriptpath);
           protocols = new Protocol[files.Length];
           for(int i =0;i<protocols.Length;i++)
           {
               protocols[i] = new Protocol();
               try
               {
                   protocols[i].Parse(System.IO.File.ReadAllText(files[i]), true);
               }
               catch (Exception ex)
               {
                  ConsoleServer.WriteLine(files[i]+"數位簽章不符!") ; //write into error log
               }
               if (protocols[i].Enabled)
               {
                   protocolhash.Add(protocols[i].DeviceType, protocols[i]);
               }


           }


        }

        public Protocol  this  [string deviceType]
        {

            get
            {

                return(Protocol) protocolhash[deviceType];
            }
            
        }

    }
}
