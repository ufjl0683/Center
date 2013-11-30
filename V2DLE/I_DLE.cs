using System;
namespace Comm
{
    public delegate bool ISCmdValidCheckTemplate(object sender, byte cmd);
    public delegate void OnCommErrHandler(object sender, Exception ex);
    public delegate void OnAckEventHandler(object sender, AckPackage AckObj);
    public delegate void OnNakEventHandler(object sender, NakPackage AckObj);
    public delegate void OnTextPackageEventHandler(object sender, TextPackage txtObj);
    public delegate void OnSendingAckNakHandler(object sender, ref byte[] data);
    public delegate void OnSendPackgaeHandler(object sender, SendPackage pkg);
   

   public interface I_DLE
    {
        void Close();
      //  int GeTimeOutQueueCount();
        System.IO.Stream GetStream();
        event OnAckEventHandler OnAck;
        event OnSendingAckNakHandler OnBeforeAck;
        event OnSendingAckNakHandler OnBeforeNak;
        event OnCommErrHandler OnCommError;
        event OnNakEventHandler OnNak;
        event OnTextPackageEventHandler OnReceiveText;
        event OnTextPackageEventHandler OnReport;
        event OnSendPackgaeHandler OnSendingPackage;
        event OnTextPackageEventHandler OnTextCmdCheck;
        event OnSendingAckNakHandler OnTextSending;
      //  byte[] PackData(int address, byte[] text);
      //  byte[] PackData(int address, byte[] text, byte seq);
      //  void PrintQueueState();
        void Send(SendPackage pkg);
        void Send(SendPackage[] pkg);
        bool IsOnAckRegisted();
       string getQueueState();
       int getTotalQueueCnt();
        void  setEnable(bool enable);
       bool getEnable();

       string getDeviceName();
       void setDeviceName(string devName);
       
    }
}
