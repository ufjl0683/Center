using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.MFCC;
namespace Host.MFCC
{
  public   class MFCC_Object
    {

      public string  hostid, hostip, mfccid, mfcctype;
      int remoteport;
      RemoteInterface.MFCC.I_MFCC_Base robj;

      System.Timers.Timer tmr1min = new System.Timers.Timer(1000 * 60);
      public MFCC_Object(string hostid,string hostip,int remoteport,string mfccid,string mfcctype)
      {
          this.hostid = hostid;
          this.hostip = hostip;
          this.mfccid = mfccid;
          this.mfcctype = mfcctype;
          this.remoteport = remoteport;

           ConnectRemoteObject();
          if (robj == null)
          {
              ConsoleServer.WriteLine(mfccid + "robj build fail! stating reconnect!");
              new System.Threading.Thread(RemoteObjectConnectTask).Start();
          }
          tmr1min.Elapsed += new System.Timers.ElapsedEventHandler(tmr1min_Elapsed);
          tmr1min.Start();
      }

      volatile bool isIntmr1min = false;

      void tmr1min_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
      {
          if (robj == null)
              return;
          if (isIntmr1min)
              return;
          try
          {

              isIntmr1min = true;

              ((RemoteClassBase)robj).HelloWorld();

          }
          catch {
              robj = null;
              new System.Threading.Thread(RemoteObjectConnectTask).Start();
              
              ;}
          finally
          {
              isIntmr1min = false;
          }
          //throw new Exception("The method or operation is not implemented.");
      }

    private  void ConnectRemoteObject()
      {
          if (mfcctype == "VD")
          {
              robj = (I_MFCC_VD)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_VD), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_VD"));
          }
          else if (mfcctype == "RGS")
          {

              robj = (I_MFCC_RGS)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_RGS), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_RGS"));
          }
          else if (mfcctype == "CMS")
          {

              robj = (I_MFCC_CMS)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_CMS), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_CMS"));
          }
          else if (mfcctype == "WIS")
          {

              robj = (I_MFCC_WIS)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_WIS), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_WIS"));
          }
          else if (mfcctype == "RMS")
          {

              robj = (I_MFCC_RMS)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_RMS), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_RMS"));
          }
          else if (mfcctype == "LCS")
          {

              robj = (I_MFCC_LCS)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_LCS), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_LCS"));
          }
          else if (mfcctype == "CSLS")
          {

              robj = (I_MFCC_CSLS)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_CSLS), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_CSLS"));
          }
          else if (mfcctype == "AVI")
          {

              robj = (I_MFCC_AVI)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_AVI), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_AVI"));
          }
          else if (mfcctype == "RD")
          {

              robj = (I_MFCC_RD)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_RD), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_RD"));
          }
          else if (mfcctype == "VI")
          {

              robj = (I_MFCC_VI)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_VI), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_VI"));
          }
          else if (mfcctype == "WD")
          {

              robj = (I_MFCC_WD)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_WD), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_WD"));
          }
          else if (mfcctype == "TTS")
          {

              robj = (I_MFCC_TTS)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_TTS), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_TTS"));
          }
          else if (mfcctype == "FS")
          {

              robj = (I_MFCC_FS)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_FS), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_FS"));
          }
          else if (mfcctype == "MAS")
          {

              robj = (I_MFCC_MAS)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_MAS), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_MAS"));
          }
          else if (mfcctype == "IID")
          {

              robj = (I_MFCC_IID)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_IID), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_IID"));
          }
          else if (mfcctype == "ETTU")
          {

              robj = (I_MFCC_ETTU)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_ETTU), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_ETTU"));
          }
          else if (mfcctype == "LS")
          {

              robj = (I_MFCC_LS)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_LS), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_LS"));
          }
          else if (mfcctype == "TEM")
          {

              robj = (I_MFCC_TEM)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_TEM), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_TEM"));
          }
          else if (mfcctype == "SCM")
          {

              robj = (I_MFCC_TEM)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_TEM), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_SCM"));
          }
          else if (mfcctype == "CMSRST")
          {

              robj = (I_MFCC_CMSRST)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_CMSRST), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_CMSRST"));
          }
          else if (mfcctype == "BS")
          {

              robj = (I_MFCC_BS)RemoteBuilder.GetRemoteObj(typeof(I_MFCC_BS), RemoteBuilder.getRemoteUri(hostip, remoteport, "MFCC_BS"));
          }
      }

     volatile bool IsInRemoteObjectConnectTask = false;
      void RemoteObjectConnectTask()
      {
          if (IsInRemoteObjectConnectTask)
              return;
          IsInRemoteObjectConnectTask = true;
          while (true)
          {
              try
              {
                  ConnectRemoteObject();
                  if (robj != null)
                  {
                      ConsoleServer.WriteLine(mfccid + "connected");
                      break;
                  }
                  System.Threading.Thread.Sleep(5000);
                  ConsoleServer.WriteLine(mfccid + "reconnecting!");
              }
              catch
              {
                  ;
              }

          }
          IsInRemoteObjectConnectTask = false;
      }


      public I_MFCC_Base getRemoteObject()
      {
          return robj;
      }

    }
}
