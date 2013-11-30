using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_SVWS
{
 public    class MFCC_SVWS
    {
      public DbCmdServer dbServer = new DbCmdServer();
       
      // Comm.V2DLE dledev;
       Comm.Protocol protocol;
      public SVWS_StatusManager stausmgr = new SVWS_StatusManager();
       
      // System.Net.Sockets.TcpClient tcp;
        public MFCC_SVWS()
        {
            protocol = new Comm.Protocol();
            protocol.Parse(System.IO.File.ReadAllText("protocol.txt"),false);
            ConsoleServer.Start((int)RemoteInterface.ConsolePortEnum.MFCC_SVWS);
            new SVWSTC(protocol, "192.168.22.130", 6013);
            new SVWSTC(protocol, "10.21.16.18", 1001);
        }

        
   


    }
}
