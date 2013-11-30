using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
namespace RemoteInterface
{
    public class ServerFactory
    {

        static public int TcpServerChannelPort = 9090;
        static TcpServerChannel RemoteServerTcp = null;
        public static void RegisterRemoteObject(Type ServerType, string uriname) //default port 9090
        {
            // TcpServerChannel tcp;

            try
            {

                if (RemoteServerTcp == null)
                {
                    RemoteServerTcp = new TcpServerChannel(uriname, TcpServerChannelPort);
                    

                    System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(RemoteServerTcp, false);
                }
                System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownServiceType(ServerType, uriname, WellKnownObjectMode.SingleCall);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            //  return RemoteServerTcp;
        }

        public static void SetChannelPort(int port)
        {
            TcpServerChannelPort = port;
        }

      

     


    }
}
