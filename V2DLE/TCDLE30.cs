using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Comm
{


  

    public class TCDLE30 : Comm.I_DLE
    {
        public    const   byte DLE = 0xaa;
        public const byte STX = 0xbb;
        public const byte ETX = 0xcc;
        public const byte ACK = 0xdd;
        public const byte NAK = 0xee;
        public const int T0ms = 5000;  //Controller 上傳等待TimeOut 時間
        public const int T1ms = 5000;  //中央電腦 要求資料上傳 等待TimeOut 時間
        public const int T2ms = 5000;//中央電腦 資料下載 等待TimeOut 時間
        public const int PackageSendTimeOut = 20000;
       // public static byte DLE_ERR_PARITY_BIT_NO =0;
        public static byte DLE_ERR_FRAME = 0x01;
        public static byte DLE_ERR_LCR = 0x00;
        public static byte DLE_ERR_ADDR_ERR = 0x02;
        public static byte DLE_ERRR_LEN_ERR = 0x03;
        //public static byte DLE_ERR_TIMEOUT = 0x03;
        //public static byte DLE_ERR_CMD_ERR = 0x4;
        //public static byte DLE_ERR_CMD_PARAM_OVERRANGE = 0x5;
        //public static byte DLE_ERR_CMD_FAIL = 0x6;
        
        public event OnCommErrHandler OnCommError;
        public event OnAckEventHandler OnAck;
        public event OnNakEventHandler OnNak;
        public event OnTextPackageEventHandler OnReceiveText;
        public event OnTextPackageEventHandler OnTextCmdCheck;
        public event OnSendingAckNakHandler OnBeforeAck;
        public event OnSendingAckNakHandler OnBeforeNak;
        public event OnTextPackageEventHandler OnReport;
        public event OnSendPackgaeHandler OnSendingPackage;
        public event OnSendingAckNakHandler OnTextSending;
       // public event OnSendingAckNakHandler OnSendingAck;
       

        
         private bool Enabled=true;

      //  public  ISCmdValidCheckTemplate  IsValidCCmdFuncPtr;  //chk cmd range valid function
        byte Seq = 0; //0~127
        System.Collections.Queue SendQueueA = new System.Collections.Queue(100);
        System.Collections.Queue SendQueueB = new System.Collections.Queue(100);
        System.Collections.Queue SendQueueC = new System.Collections.Queue(100);
        System.Collections.Queue SendQueueD = new System.Collections.Queue(100);
        System.Collections.Queue ReceiveQueue = new System.Collections.Queue(100);
        System.Collections.Queue TimeOutQueue = new System.Collections.Queue(100);

        System.IO.Stream stream;
        SendPackage currentSendingPackage = null;
        Object currentRespObj = null;

        byte[] AckData = new byte[8];
        byte[] NakData = new byte[9];
        System.Threading.Thread SendTaskThread ;
        System.Threading.Thread ReceiveTaskThread;

        private string m_devName;
        public TCDLE30(string devName, System.IO.Stream stream)
        {
            this.m_devName = devName;
            this.stream=stream;
            SendTaskThread = new System.Threading.Thread(SendTask);
            ReceiveTaskThread = new System.Threading.Thread(ReceiveTask);
            SendTaskThread.Start();
            ReceiveTaskThread.Start();


        }

        public string getDeviceName()
        {
            return this.m_devName;
        }
        public void setDeviceName(string devName)
        {
            this.m_devName = devName;
        }
        public void setEnable(bool enable)
        {
            this.Enabled = enable;
        }

        public bool getEnable()
        {
            return Enabled;
        }
        public bool IsOnAckRegisted()
        {
            return this.OnAck != null;
        }
        public void PrintQueueState()
        {
            
            Console.WriteLine("SendQueueA=" + SendQueueA.Count);
            Console.WriteLine("SendQueueB=" + SendQueueB.Count);
            Console.WriteLine("SendQueueC=" + SendQueueC.Count);
            Console.WriteLine("SendQueueD=" + SendQueueD.Count);
            Console.WriteLine("ReceiveQueue=" + ReceiveQueue.Count);
            Console.WriteLine("TimeOutQueue" + TimeOutQueue.Count);
        }
        public string getQueueState()
        {

            return "SendQueueA=" + SendQueueA.Count + "\r\n" + "SendQueueB=" + SendQueueB.Count + "\r\n" + "SendQueueC=" + SendQueueC.Count + "\r\n" + "SendQueueD=" + SendQueueD.Count +
                "\r\n" + "ReceiveQueue=" + ReceiveQueue.Count + "\r\n" + "TimeOutQueue=" + TimeOutQueue.Count;
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine("SendQueueC=" + SendQueueC.Count);
            //Console.WriteLine("SendQueueD=" + SendQueueD.Count);
            //Console.WriteLine("ReceiveQueue=" + ReceiveQueue.Count);
            //Console.WriteLine("TimeOutQueue" + TimeOutQueue.Count);
        }

        public int getTotalQueueCnt()
        {
            return SendQueueA.Count + SendQueueB.Count + SendQueueC.Count + SendQueueD.Count + TimeOutQueue.Count;
        }


        public int GeTimeOutQueueCount()
        {
            return TimeOutQueue.Count;
        }
        public void  Send(SendPackage pkg)
        {

            if(!Enabled)
                throw new Exception("CommError");
            switch (pkg.cls)
            {
                case CmdClass.A:
                    lock (pkg)
                    {
                   
                        lock (SendTaskNotifyObj)
                        {
                            SendQueueA.Enqueue(pkg);
                            System.Threading.Monitor.Pulse(SendTaskNotifyObj);
                        }
                        if (!System.Threading.Monitor.Wait(pkg, PackageSendTimeOut))
                        {
                            
                            pkg.result = CmdResult.TimeOut;
                            throw new Exception(string.Format("Queue Waiting Exceed {0} sec!", PackageSendTimeOut/1000));
                        }
                    }
                  
                    break;
                case CmdClass.B:
                    lock (pkg)
                    {
                       
                        lock (SendTaskNotifyObj)
                        {
                            SendQueueB.Enqueue(pkg);
                            System.Threading.Monitor.Pulse(SendTaskNotifyObj);
                        }
                        if (!System.Threading.Monitor.Wait(pkg, PackageSendTimeOut))
                        {
                            pkg.result = CmdResult.TimeOut;
                            throw new Exception(string.Format("Queue Waiting Exceed {0} sec!", PackageSendTimeOut / 1000));
                        }
                    }
                    break;
                case CmdClass.C:
                    lock (pkg)
                    {
                      
                        lock (SendTaskNotifyObj)
                        {
                            SendQueueC.Enqueue(pkg);
                            System.Threading.Monitor.Pulse(SendTaskNotifyObj);
                        }

                        if (!System.Threading.Monitor.Wait(pkg, PackageSendTimeOut))
                        {
                            pkg.result = CmdResult.TimeOut;
                            throw new Exception(string.Format("Queue Waiting Exceed {0} sec!", PackageSendTimeOut / 1000));
                        }
                    }
                    break;
            }
        }

        public void Send(SendPackage[] pkgs)
        {
            foreach (SendPackage pkg in pkgs)
            {
                try
                {

                    Send(pkg);
                }
                catch { ;}
            }


        }

        object SendTaskNotifyObj=new object();
        
        void SendTask()
        {

            byte[]data=null;
            while (true)
            {

                lock (SendTaskNotifyObj)
                {
                    if (SendQueueA.Count != 0)

                        currentSendingPackage = (SendPackage)SendQueueA.Dequeue();
                    else if (SendQueueB.Count != 0)
                        currentSendingPackage = (SendPackage)SendQueueB.Dequeue();
                    else if (SendQueueC.Count != 0)
                        currentSendingPackage = (SendPackage)SendQueueC.Dequeue();
                    else if(SendQueueD.Count!=0)
                        currentSendingPackage = (SendPackage)SendQueueD.Dequeue();
                    else if (TimeOutQueue.Count != 0)
                        currentSendingPackage = (SendPackage)TimeOutQueue.Dequeue();
                    else
                    {
                        //no Data To Send , and waiting

                        currentSendingPackage = null;
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        System.Threading.Monitor.Wait(SendTaskNotifyObj);


                        continue;
                    }

                }


                  
                        try
                        {  
                            if(currentSendingPackage.sendCnt==0)
                            data = PackData(currentSendingPackage.address, currentSendingPackage.text);
                            else
                            data = PackData(currentSendingPackage.address, currentSendingPackage.text,(byte)currentSendingPackage.Seq);

                            currentSendingPackage.Seq = data[2];
                           
                            if (currentSendingPackage.sendCnt >= 4)   //discard package
                            {
                                //if (this.OnCommError != null)
                                //    OnCommError(this,new Exception("Time Out Error!"));
                                
                                lock (currentSendingPackage)
                                {
                                    System.Threading.Monitor.Pulse(currentSendingPackage);
                                }
                                continue;
                            }
                            currentSendingPackage.sendCnt++;
                            if (OnSendingPackage != null)
                                OnSendingPackage(this, currentSendingPackage);
                            lock (stream)
                            {

                                if (OnTextSending != null)
                                    OnTextSending(this,ref data);
                              
                              stream.Write(data, 0, data.Length);
                              stream.Flush();
                                //for (int i = 0; i < data.Length; i++)
                                //{
                                  //  stream.WriteByte(data[i]);
                                    //stream.Flush();
                               // }


                                if (System.Threading.Monitor.Wait(stream, T0ms))
                                {
                                    //No Time Out




                                    if (currentSendingPackage.result == CmdResult.ACK)
                                    {
                                        lock (currentSendingPackage)
                                        {
                                            System.Threading.Monitor.Pulse(currentSendingPackage);
                                        }
                                    }
                                    else
                                        TimeOutQueue.Enqueue(currentSendingPackage);//nak ,time out


                                }
                                else
                                {
                                    //Time out
                                    if( currentSendingPackage.result==CmdResult.Unknown)
                                    currentSendingPackage.result = CmdResult.TimeOut;

                                    TimeOutQueue.Enqueue(currentSendingPackage);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (this.OnCommError != null)
                                OnCommError(this,ex);
                          //  Console.WriteLine(ex.Message);
                        }
                   
             }//while




            


        }

        void ReceiveTask()
        {
            int d;
            bool dle_flag = false;
             TCDLE30AckPackage AckPkg=null;
            TCDLE30NakPackage NakPkg = null;
            TextPackage TxtPkg = null;
            while (true)
            {

                try
                {
                    if(!dle_flag) 
                    while (  (d = stream.ReadByte()) != DLE ) ;
                //{
                //    if (d != -1)
                //        Console.WriteLine(d.ToString());
                //    ;
                //};
                    dle_flag = false;
                    switch ((d = stream.ReadByte()))
                    {
                        case STX:
                          
                            TxtPkg=  ReadText();
                            currentRespObj = TxtPkg;
                            
                            if (OnTextCmdCheck != null)
                                OnTextCmdCheck(this,TxtPkg);

                            if (TxtPkg.HasErrors)
                            {
                                
                                    //new byte[] { DLE, V2DLE.ACK, TxtPkg.Seq, TxtPkg.Address / 256, TxtPkg.Address % 256, 0 };  //send nak
                                    NakData[0] = DLE;
                                    NakData[1] = NAK;
                                    NakData[2] = (byte)TxtPkg.Seq;
                                    NakData[3] = (byte)(TxtPkg.Address / 256);
                                    NakData[4] = (byte)(TxtPkg.Address % 256);
                                    NakData[5] = 0x00;
                                    NakData[6] = 0x09;
                                    NakData[7] = TxtPkg.Err[0];
                                   // NakData[6] = TxtPkg.Err[1];
                                    NakData[8] = (byte)(NakData[0] ^ NakData[1] ^ NakData[2] ^ NakData[3] ^ NakData[4] ^ NakData[5] ^ NakData[6] ^ NakData[7]);
                                    if (OnBeforeNak != null)
                                        OnBeforeNak(this, ref NakData);                                
                                    lock (stream)
                                    {
                                        stream.Write(NakData, 0, NakData.Length);
                                    }
                                       
                               
                            }
                            else
                            {  
                                //
                                
                                AckData[0] = DLE;
                                AckData[1] = ACK;
                                AckData[2] = (byte)TxtPkg.Seq;
                                AckData[3] = (byte)(TxtPkg.Address / 256);
                                AckData[4] = (byte)(TxtPkg.Address % 256);
                                AckData[5] = 0x00;
                                AckData[6] = 0x08;
                                AckData[7] = (byte)(AckData[0] ^ AckData[1] ^ AckData[2] ^ AckData[3] ^ AckData[4] ^ AckData[5] ^ AckData[6]);
                                if (OnBeforeAck != null)
                                {
                                    OnBeforeAck(this, ref AckData);
                                    
                                }

                                lock (stream)
                                {
                                    if (AckData.Length == 8)
                                        stream.Write(AckData, 0, AckData.Length);
                                    else
                                        AckData = new byte[8];
                                   
                                    ; //send ack
                                }

                                if (currentSendingPackage != null && currentSendingPackage.returnCmd == TxtPkg.Cmd && (currentSendingPackage.returnSubCmd | 0x80) == TxtPkg.SubCmd)
                                {

                                    currentSendingPackage.result = CmdResult.ACK;
                                    TxtPkg.Text[1] &= (0x7f);
                                    currentSendingPackage.ReturnTextPackage = TxtPkg;

                                    lock (stream)
                                    {
                                        System.Threading.Monitor.Pulse(stream);
                                    }
                                   

                                }
                                else
                                {
                                    if (OnReport != null)
                                        OnReport(this,TxtPkg);
                                }
                                if (this.OnReceiveText != null)
                                    OnReceiveText(this,TxtPkg);
                            }
                            break;
                        case ACK:

                           AckPkg=  ReadAck();
                           currentRespObj = AckPkg;

                           if (currentSendingPackage !=null  &&  currentSendingPackage.Seq == AckPkg.Seq )
                           {  
                               currentSendingPackage.result= CmdResult.ACK;
                               if(currentSendingPackage.type==CmdType.CmdSet )
                               lock (stream)
                               {
                                   System.Threading.Monitor.Pulse(stream);
                               }
                           }

                           if (OnAck != null)
                               OnAck(this,AckPkg);

                            break;
                        case NAK:
                            NakPkg=  ReadNak();
                            if (currentSendingPackage!=null &&  currentSendingPackage.Seq == NakPkg.Seq)
                            {
                                currentSendingPackage.result = GetCmdResult(NakPkg);
                                lock (stream)
                                {
                                    System.Threading.Monitor.Pulse(stream);
                                }


                            }
                            if (OnNak != null)
                                OnNak(this,NakPkg);
                            break;
                        
                           
                        default:
                            if (d == DLE)
                            {
                                dle_flag = true;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                  //  Console.WriteLine(ex.StackTrace);
                    if (OnCommError != null)
                    {

                        OnCommError(this, ex);
                    }
                    break;
                }

                
            }
        }

        CmdResult GetCmdResult(TCDLE30NakPackage pkg)
        {
            if (pkg.GetErrBit(TCDLE30.DLE_ERR_ADDR_ERR))
                return CmdResult.TC_DLE_30_ADDR_ERR;
            else if (pkg.GetErrBit(TCDLE30.DLE_ERR_FRAME))
                return CmdResult.Frame_ERR;
            else if (pkg.GetErrBit(TCDLE30.DLE_ERR_LCR))
                return CmdResult.LRC_ERR;
            else if (pkg.GetErrBit(TCDLE30.DLE_ERRR_LEN_ERR))
                return CmdResult.TC_DEL_30_LEN_ERR;
           
            else
                return CmdResult.Unknown;
        }

        CmdResult GetCmdResult(TextPackage pkg)
        {
            if (pkg.GetErrBit(TCDLE30.DLE_ERR_ADDR_ERR))
                return CmdResult.TC_DLE_30_ADDR_ERR;
            else if (pkg.GetErrBit(TCDLE30.DLE_ERR_FRAME))
                return CmdResult.Frame_ERR;
            else if (pkg.GetErrBit(TCDLE30.DLE_ERR_LCR))
                return CmdResult.LRC_ERR;
            else if (pkg.GetErrBit(TCDLE30.DLE_ERRR_LEN_ERR))
                return CmdResult.TC_DEL_30_LEN_ERR;

            else
                return CmdResult.Unknown;
        }
        
        public System.IO.Stream GetStream()
        {
            return stream;
        }

        private TextPackage ReadText()
        {
            int Seq=0,Address=0,Len=0,LRC=0;//,HeadLRC=0;
            byte[] text=null;
            TextPackage textPackage = new TextPackage();
            Seq = stream.ReadByte();
            Address = stream.ReadByte() * 256;
            Address+= stream.ReadByte();
            Len = stream.ReadByte() * 256;
            Len += stream.ReadByte();
         //   HeadLRC = stream.ReadByte();
            Len -= 10;
            textPackage.Seq = Seq;
            textPackage.Address = Address;
         

            //if (HeadLRC != (((Address >> 8) & 0x00ff) ^ (Address & 0x00ff) ^ ((Len >> 8) & 0x00ff) ^ (Len & 0x00ff)))
            //{
            //  // textPackage.HasErrors = true;
            //    textPackage.SetErrBit(V2DLE.DLE_ERR_FRAME, true);
            //    textPackage.eErrorDescription += "Hearder LRC Error!\r\n";
            //    //Console.WriteLine("Hearder LRC Error!");
            //    return textPackage;
            //}
            //else
            //{
               
                text = new byte[Len];
                int rlen = 0;




                do
                {
                    rlen+=stream.Read(text,rlen, Len-rlen);
                    
                } while (rlen != Len);
                //for (int i = 0; i < Len; i++)
                //    text[i] = (byte)stream.ReadByte();
               
                
              
                stream.ReadByte();   //READ DLE
                stream.ReadByte();    // READ ETX

                LRC= STX ^ Seq ^ETX ^ (Len+10) /256 ^ (Len+10)%256 ;
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                bool isDLE=false;
                for (int i = 0; i < text.Length; i++)
                {
                    LRC ^= text[i];
                    if (text[i] != DLE)
                    {
                        ms.WriteByte(text[i]);
                        isDLE = false;
                    }
                    else  //double dle 處理
                    {
                        if (isDLE) //2nd dle
                            isDLE = false;
                        else  // 1st dle
                        {
                            isDLE = true;
                            ms.WriteByte(text[i]);
                        }
                    }

                }
                
         
                int tmp = stream.ReadByte();
                if (LRC !=tmp)// stream.ReadByte())
                {
                    textPackage.SetErrBit(TCDLE30.DLE_ERR_LCR, true);
                    textPackage.eErrorDescription += "LRC Error!\r\n";
                    Console.WriteLine("LRC Error!");
                    return textPackage;
                }
                else
                {
                   textPackage.Text=text;
                   textPackage.LRC = LRC;
                   return textPackage;
                    
                }
                
          //  }


        }

        private TCDLE30AckPackage ReadAck()
        {
            byte[] data = new byte[6];
            stream.Read(data, 0, 6);
           return new TCDLE30AckPackage(data);

        }

        private TCDLE30NakPackage ReadNak()
        {
            byte[] data = new byte[7];
            stream.Read(data, 0, 7);

            return new TCDLE30NakPackage(data);
        }

        public byte[] PackData(int address, byte[] text,byte seq)
        {
            byte[] data;
            ushort dlecnt = 0;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            ms.WriteByte(DLE);
            ms.WriteByte(STX);
            ms.WriteByte(seq);
            ms.WriteByte((byte)(address / 256));
            ms.WriteByte((byte)(address % 256));
            ms.WriteByte((byte)(text.Length / 256));
            ms.WriteByte((byte)(text.Length % 256));
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == DLE)
                {
                    ms.WriteByte(text[i]);
                    dlecnt++;
                }
                ms.WriteByte(text[i]);
            }
            ms.WriteByte(DLE);
            ms.WriteByte(ETX);
            ms.GetBuffer()[5] = (byte)((text.Length + dlecnt+10) / 256);
            ms.GetBuffer()[6] = (byte)((text.Length + dlecnt+10) % 256);
            byte LRC = 0;
            for (int i = 0; i < ms.Length; i++)
                LRC ^= ms.GetBuffer()[i];

            ms.WriteByte(LRC);
           data= ms.ToArray();
           
           return data;
            
           /* byte[] data = new byte[10 + text.Length];
            int inx = 0;
            data[0] = DLE;
            data[1] = STX;
            data[2] = seq;
            data[3] = (byte)(address / 256);
            data[4] = (byte)(address % 256);
            data[5] = (byte)(text.Length / 256);
            data[6] = (byte)(text.Length % 256);
         
            for ( inx = 0; inx < text.Length; inx++)
                data[7 + inx] = text[inx];
            data[7 + inx] = DLE;
            data[8 + inx] = ETX;


            byte LRC = 0;
            for (int i = 0; i < data.Length - 1; i++)
                LRC ^= data[i];

            data[data.Length - 1] = LRC;

            return data;*/
        }

        private  byte[] PackData(int address ,byte[] text)
        {

            byte[] data;
            ushort dlecnt = 0;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            ms.WriteByte(DLE);
            ms.WriteByte(STX);
            ms.WriteByte(this.GetSeq());
            ms.WriteByte((byte)(address / 256));
            ms.WriteByte( (byte)(address % 256));
            ms.WriteByte((byte)(text.Length / 256));
            ms.WriteByte((byte)(text.Length % 256));
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == DLE)
                {
                    ms.WriteByte(text[i]);
                    dlecnt++;
                }
                ms.WriteByte(text[i]);
            }
            ms.WriteByte(DLE);
            ms.WriteByte(ETX);

            ms.GetBuffer()[5] = (byte)((text.Length + dlecnt+10) / 256);
            ms.GetBuffer()[6] = (byte)((text.Length + dlecnt+10) % 256);
           
              byte LRC = 0;
              for (int i = 0; i < ms.Length; i++)
                  LRC ^= ms.GetBuffer()[i];
              ms.WriteByte(LRC);
              data= ms.ToArray();
             
              return data;
            /*
            byte[] data = new byte[10 + text.Length];
            int inx = 0;
           data[0] = DLE;
            data[1] = STX;
            data[2] = this.GetSeq();
            data[3] = (byte)(address / 256);
            data[4] = (byte)(address % 256);
            data[5] = (byte)(text.Length / 256);
            data[6] = (byte)(text.Length % 256);

            for (inx = 0; inx < text.Length; inx++)
                data[7 + inx] = text[inx];
            data[7 + inx] = DLE;
            data[8 + inx] = ETX;


            byte LRC = 0;
            for (int i = 0; i < data.Length - 1; i++)
                LRC ^= data[i];

            data[data.Length - 1] = LRC; 

            return data; */
          

        }

        private byte GetSeq()
        {

            Seq = (byte)((Seq + 1) % 128);
            return Seq;
        }

        public void Close()
        {
            try
            {
                this.SendTaskThread.Abort();
                this.ReceiveTaskThread.Abort();
            }
            catch { }
           
        }


        public static string ToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(100);
            
            for(int i=0;i<data.Length;i++)
                sb.Append(string.Format("{0:X2} ",data[i]));

            return sb.ToString();
        }
        public static  string ToHexString(byte data)
        {
            return string.Format("{0:X2}", data);
        }
        public static string ToHexString(int data)
        {
            return string.Format("{0:X4}", data);
        }

        public static byte[] getIP(string ipstr)
        {
            string[] ips = ipstr.Split(new char[] { '.' });
            byte[] ipByte = new byte[4];
            for (int i = 0; i < 4; i++)
                ipByte[i] = System.Convert.ToByte(ips[i]);

            return ipByte;

        }
         public static string CPath(string WinPath)
        {


            if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
                return WinPath;
            else
            {
              //  Console.WriteLine("Unix");
                return WinPath.Replace(@"\", @"/");
            }
        }

    }
}
