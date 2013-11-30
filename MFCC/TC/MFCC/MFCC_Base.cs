using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HC;
using RemoteInterface.MFCC;
using System.Data;

namespace Comm.MFCC
{
  public abstract class MFCC_Base
    {
         RemoteInterface.HC.I_HC_Comm r_host_comm;
         protected System.Collections.ArrayList     tcAry=new System.Collections.ArrayList();
        protected Comm.Protocol protocol;
         string devType;
        int remotePort;
        int notifyPort;
        int consolePort;
        string regRemoteName;
        Type regRemoteType;
         protected RemoteInterface.EventNotifyServer notifier;

         public MFCC_Base(string devType,int remotePort,int notifyPort,int consolePort,string regRemoteName,Type regRemoteType)
        {
            this.devType = devType;
            this.remotePort = remotePort;
            this.notifyPort = notifyPort;
            this.consolePort = consolePort;
            this.regRemoteName = regRemoteName;
            this.regRemoteType = regRemoteType;


            init_RemoteInterface();
         
            load_protocol();
            ConsoleServer.WriteLine("loading Tc ...");
            loadTC_AndBuildManaer();
            ConsoleServer.WriteLine("load Tc Completed!");
            notifier = new EventNotifyServer(notifyPort);

           
            check_and_connect_remote_obj(r_host_comm);
        }



        private  void load_protocol()
        {
            string protocol_source = "";
            if (r_host_comm != null)
            {
                try
                {
                    protocol_source = r_host_comm.getScriptSource(devType);
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message);
                    ConsoleServer.WriteLine("read local protocol.txt");
                    protocol_source = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "protocol.txt").ReadToEnd();

                }

                try
                {
                    protocol = new Protocol();
                    protocol.Parse(protocol_source,false);
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "protocol.txt");
                    sw.Write(protocol_source);
                    sw.Close();
                    return;
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message);
                }


            }

            else  //r_host_comm fail
            {
                // read local protocol.txt
                protocol_source = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "protocol.txt").ReadToEnd();
            }

            protocol = new Protocol();
            protocol.Parse(protocol_source,false);
        }

        public void check_and_connect_remote_obj(object robj)
        {
            if (robj == null)
                new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Connect_Remote_Host_Comm_Task)).Start(robj);
        }


       void Connect_Remote_Host_Comm_Task(object robj)
        {
            while (true)
            {
                try
                {

                    if (r_host_comm == null)
                    {
                        r_host_comm = (I_HC_Comm)RemoteBuilder.GetRemoteObj(typeof(I_HC_Comm), RemoteBuilder.getRemoteUri(RemoteBuilder.getHostIP(), (int)RemotingPortEnum.HOST, "Comm"));
                        if (r_host_comm != null)
                        {
                            ConsoleServer.WriteLine("r_host_comm" + " connected!");
                            return;
                        }
                    }
                  
                }
                catch
                {
                    ConsoleServer.WriteLine("reconnect host remoteObject failed..reconnecting!");
                  
                }



                    System.Threading.Thread.Sleep(10000);

            }

           
        }
       public   virtual void init_RemoteInterface()
        {
            try
            {
                r_host_comm = (I_HC_Comm)RemoteBuilder.GetRemoteObj(typeof(I_HC_Comm), RemoteBuilder.getRemoteUri(RemoteBuilder.getHostIP(), (int)RemotingPortEnum.HOST, "Comm"));

            }
            catch(Exception ex) {
                ConsoleServer.WriteLine(ex.Message);}


            RemoteInterface.ServerFactory.SetChannelPort(remotePort);

            ServerFactory.RegisterRemoteObject(regRemoteType, regRemoteName);
            ConsoleServer.Start(consolePort);



        }

      public abstract void loadTC_AndBuildManaer();
      public abstract MFCC.TC_Manager getTcManager();
      

      public DataSet getSendDsByFuncName(string funcname)
      {
          return protocol.GetSendDataSet(funcname);
      }


      public DataSet SendTC(string tcname, DataSet ds)
      {

         TCBase tc=  getTcManager()[tcname];

         if (!tc.IsConnected) throw new Exception("tc 未連線!");
         SendPackage pkg= protocol.GetSendPackage(ds, tc.DeviceID);
         tc.Send(pkg);

         if (pkg.result == CmdResult.ACK)
         {
             if (pkg.type == CmdType.CmdSet)
                 return null;
             else
                 return protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
         }
         else
         {
             throw new Exception(pkg.result.ToString());
         }
      }
      public DataSet SendTC(string ip,int port, DataSet ds)
      {

          TCBase tc = getTcManager()[ip,port];
          if (!tc.IsConnected) throw new Exception("tc 未連線!");
          SendPackage pkg = protocol.GetSendPackage(ds, tc.DeviceID);
          tc.Send(pkg);

          if (pkg.result == CmdResult.ACK)
          {
              if (pkg.type == CmdType.CmdSet)
                  return null;
              else
                  return protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
          }
          else
          {
              throw new Exception(pkg.result.ToString());
          }
      }

     
   
    }
}
