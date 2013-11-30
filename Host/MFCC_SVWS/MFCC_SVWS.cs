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
            protocol.Parse(System.IO.File.ReadAllText(RemoteInterface.Util.CPath(AppDomain.CurrentDomain.BaseDirectory+  "protocol.txt")),false);
            ConsoleServer.Start((int)RemoteInterface.ConsolePortEnum.MFCC_SVWS);
            new SVWSTC(protocol, "192.168.22.130", 6013);
            new SVWSTC(protocol, "10.21.50.87", 1001);

            //SVWS_Status status = new SVWS_Status();
            //status.place = 21;
            //status.group = 1;
            //status.kind = 14;
            //status.dir = 1;
            //status.run_status = 0;
            //status.hw_status = 1;
            //status.type = "D";
            //stausmgr.AddSVWS_Status(status);
        }

        
   


    }
}
