using System;
using System.Collections.Generic;
using System.Text;

namespace Comm
{
 public    class ETTUDLE1 : Comm.I_DLE
    {

      public    const   byte DLE = 0x10;
        public const byte SOH = 0x01;
        public const byte ACK = 0x06;
        public const byte NAK = 0x15;
        public const int T0ms = 5000;  //Controller 上傳等待TimeOut 時間
        public const int T1ms = 6000;  //中央電腦 要求資料上傳 等待TimeOut 時間
        public const int T2ms = 6000;//中央電腦 資料下載 等待TimeOut 時間
        public const int PackageSendTimeOut = 20000;
        public static byte DLE_ERR_PARITY_BIT_NO =0;
        public static byte DLE_ERR_FRAME = 0x01;
        public static byte DLE_ERR_LCR = 0x02;
        public static byte DLE_ERR_TIMEOUT = 0x03;
        public static byte DLE_ERR_CMD_ERR = 0x4;
        public static byte DLE_ERR_CMD_PARAM_OVERRANGE = 0x5;
        public static byte DLE_ERR_CMD_FAIL = 0x6;
        
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
        System.Collections.Queue SendQueueA = System.Collections.Queue.Synchronized(new System.Collections.Queue(100));
        System.Collections.Queue SendQueueB = System.Collections.Queue.Synchronized(new System.Collections.Queue(100));
        System.Collections.Queue SendQueueC = System.Collections.Queue.Synchronized(new System.Collections.Queue(100));
        System.Collections.Queue SendQueueD = System.Collections.Queue.Synchronized( new System.Collections.Queue(100));
        System.Collections.Queue ReceiveQueue = System.Collections.Queue.Synchronized(new System.Collections.Queue(100));
        System.Collections.Queue TimeOutQueue = System.Collections.Queue.Synchronized(new System.Collections.Queue(100));
        private string m_devName;
        System.IO.Stream stream;
        SendPackage currentSendingPackage = null;
        Object currentRespObj = null;

        byte[] AckData = new byte[6];
        byte[] NakData = new byte[8];
        System.Threading.Thread SendTaskThread ;
        System.Threading.Thread ReceiveTaskThread;
        
        public ETTUDLE1(string devName,System.IO.Stream stream)
        {
            this.m_devName = devName;
            this.stream=stream;
            SendTaskThread = new System.Threading.Thread(SendTask);
            ReceiveTaskThread = new System.Threading.Thread(ReceiveTask);
            SendTaskThread.Start();
            ReceiveTaskThread.Start();


        }
         public bool IsOnAckRegisted()
         {
             return this.OnAck != null;
         }

     public void setEnable(bool enable)
     {
         this.Enabled = enable;
     }

     

     public bool getEnable()
     {
         return Enabled;
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

        public int GeTimeOutQueueCount()
        {
            return TimeOutQueue.Count;
        }

     public int getTotalQueueCnt()
     {
         return SendQueueA.Count + SendQueueB.Count + SendQueueC.Count + SendQueueD.Count + TimeOutQueue.Count;
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
                         
                        if ( !System.Threading.Monitor.Wait(pkg, PackageSendTimeOut))
                        {
                            
                            pkg.result = CmdResult.TimeOut;
                            pkg.IsDiscard = true;
                            throw new Exception(string.Format(this.m_devName + ",Queue Waiting Exceed {0} sec!" + pkg, PackageSendTimeOut / 1000 + "\r\n" )+ this.getQueueState());
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
                        if (pkg.type != CmdType.CmdSet && !System.Threading.Monitor.Wait(pkg, PackageSendTimeOut))
                        {
                            pkg.result = CmdResult.TimeOut;
                            pkg.IsDiscard = true;
                            throw new Exception(string.Format(this.m_devName + ",Queue Waiting Exceed {0} sec!" + pkg, PackageSendTimeOut / 1000) + "\r\n"+this.getQueueState());
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

                        if (pkg.type != CmdType.CmdSet && !System.Threading.Monitor.Wait(pkg, PackageSendTimeOut))
                        {
                            pkg.result = CmdResult.TimeOut;
                            pkg.IsDiscard = true;
                            throw new Exception(string.Format(this.m_devName + ",Queue Waiting Exceed {0} sec!" + pkg, PackageSendTimeOut / 1000) + "\r\n" + this.getQueueState());
                        }
                    }
                    break;
                case CmdClass.D:
                    lock (pkg)
                    {

                        lock (SendTaskNotifyObj)
                        {
                            SendQueueD.Enqueue(pkg);
                            System.Threading.Monitor.Pulse(SendTaskNotifyObj);
                        }

                        if (pkg.type != CmdType.CmdSet && !System.Threading.Monitor.Wait(pkg, PackageSendTimeOut))
                        {
                            pkg.result = CmdResult.TimeOut;
                            pkg.IsDiscard = true;
                            throw new Exception(string.Format(this.m_devName + ",Queue Waiting Exceed {0} sec!" + pkg, PackageSendTimeOut / 1000) + "\r\n" + this.getQueueState());
                        }
                    }
                    break;

            }
           
       //     Console.WriteLine("---------------finish pkg" + pkg);

        }

        public void Send(SendPackage[] pkgs)
       {
           for (int i = 0; i < pkgs.Length;i++ )
           {
               try
               {
                   if (i != pkgs.Length - 1)
                   {
                       pkgs[i].ETTU_EndCode = 0xf8;
                      
                       pkgs[i].type = CmdType.CmdSet;
                   }
                   else
                   {
                       
                       pkgs[i].ETTU_EndCode = 0xc7;
                       //pkgs[i].type = CmdType.CmdQuery;
                   }
                   Send(pkgs[i]);
               }
               catch { ;}
           }


     }

         private byte[] ETTU_PackData(int address, byte[] text,byte EndCode,int seq)
        {
            byte[] data = new byte[3 + text.Length];
            uint LCR = 0;
            System.Array.Copy(text, data, text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                //if (i > 1)
                //    data[i] += 0x30;
                 LCR += data[i];
            }

          //  LCR = (~LCR) + 1;
            data[text.Length] = (byte)((LCR >>4) & 0x0f);
            data[text.Length + 1] = (byte)(LCR & 0x0f);
            data[data.Length - 1] = EndCode;
            return  PackData(address, data,(byte)seq);
        }
     public byte[] ETTU_PackData(int address, byte[] text, byte EndCode)
     {
         byte[] data = new byte[3 + text.Length];
         uint LCR = 0;
         System.Array.Copy(text, data, text.Length);
         for (int i = 0; i < text.Length; i++)
         {
             //if (i > 1)
             //    data[i] += 0x30;
             LCR += data[i];
         }

        // LCR = (~LCR) + 1;
         data[text.Length] = (byte)((LCR >> 4) & 0x0f);
         data[text.Length + 1] = (byte)(LCR & 0x0f);
         data[data.Length - 1] = EndCode;
         return   PackData(address, data);
     }
        //check ettu
        //private  byte[] ETTU_PackData(int address ,byte[] text)  // End with C7,or F8
        //{


        //    return ETTU_PackData(address, text, 0xc7);
          

        //}

        object SendTaskNotifyObj=new object();
        
        void SendTask()
        {

            byte[]data=null;
            while (true)
            {

                lock (SendTaskNotifyObj)
                {
                   
                    
                    if (SendQueueA.Count != 0)
                        while (SendQueueA.Count != 0)
                        {
                            currentSendingPackage = (SendPackage)SendQueueA.Dequeue();
                            if (!currentSendingPackage.IsDiscard)
                                break;
                           

                        }
                        
                    else if (SendQueueB.Count != 0)

                        while (SendQueueB.Count != 0)
                        {
                            currentSendingPackage = (SendPackage)SendQueueB.Dequeue();
                            if (!currentSendingPackage.IsDiscard)
                                break;
                            else
                                currentSendingPackage = null;
                        }
                    else if (SendQueueC.Count != 0)
                        while (SendQueueC.Count != 0)
                        {
                            currentSendingPackage = (SendPackage)SendQueueC.Dequeue();
                            if (!currentSendingPackage.IsDiscard)
                                break;
                            else
                                currentSendingPackage = null;
                        }
                    else if(SendQueueD.Count!=0)
                        while (SendQueueD.Count != 0)
                        {
                            currentSendingPackage = (SendPackage)SendQueueD.Dequeue();
                            if (!currentSendingPackage.IsDiscard)
                                break;
                            else
                                currentSendingPackage = null;
                        }
                    else if (TimeOutQueue.Count != 0)
                        while (TimeOutQueue.Count != 0)
                        {
                            currentSendingPackage = (SendPackage)TimeOutQueue.Dequeue();
                            if (!currentSendingPackage.IsDiscard)
                                break;
                            else
                                currentSendingPackage = null;
                        }
                    else
                    {
                        //no Data To Send , and waiting

                        currentSendingPackage = null;
                        //GC.Collect();
                        //GC.WaitForPendingFinalizers();
                        System.Threading.Monitor.Wait(SendTaskNotifyObj);


                        continue;
                    }


                }


                  
                        try
                        {
                            if (currentSendingPackage == null) continue;
                            if(currentSendingPackage.sendCnt==0)
                            data = ETTU_PackData(currentSendingPackage.address, currentSendingPackage.text,currentSendingPackage.ETTU_EndCode);
                            else
                            data = ETTU_PackData(currentSendingPackage.address, currentSendingPackage.text, currentSendingPackage.ETTU_EndCode, (byte)currentSendingPackage.Seq);

                            currentSendingPackage.Seq = data[2];
                           
                            if (currentSendingPackage.sendCnt >= 3)   //discard package
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
//---------------------------------------------------------------------------------
                                //if (currentSendingPackage.type == CmdType.CmdSet)
                                //    Console.WriteLine("<============Sendig Seq :" + currentSendingPackage.Seq);
//--------------------------------------------------------------------

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
                                            //----------------------------------------------
                                          //  Console.WriteLine("receive:" + currentSendingPackage);
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
            AckPackage AckPkg=null;
            NakPackage NakPkg = null;
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
                        case SOH:
                          
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
                                    NakData[5] = TxtPkg.Err[0];
                                    NakData[6] = TxtPkg.Err[1];
                                    NakData[7] = (byte)(NakData[0] ^ NakData[1] ^ NakData[2] ^ NakData[3] ^ NakData[4] ^ NakData[5] ^ NakData[6]);
                                    if (OnBeforeNak != null)
                                        OnBeforeNak(this, ref NakData);                                
                                    lock (stream)
                                    {
                                        stream.Write(NakData, 0, NakData.Length);
                                    }
                                       
                               
                            }
                            else
                            {  
                                //orig ack place
                                   
                               

                                if (currentSendingPackage != null && currentSendingPackage.ETTU_RetCmd == TxtPkg.ETTU_Cmd && currentSendingPackage.ETTU_RetSubCmd== TxtPkg.ETTU_SubCmd)
                                {

                                    if (currentSendingPackage.type == CmdType.CmdSet)
                                    {
                                        currentSendingPackage.result = CmdResult.ACK;
                                        // Console.WriteLine(currentSendingPackage.Cmd + "receive ack!");
                                        lock (stream)
                                        {
                                            System.Threading.Monitor.Pulse(stream);
                                        }
                                    }
                                    else  //query
                                    {


                                        currentSendingPackage.ReturnTextPackage = TxtPkg;
                                        TxtPkg.Text[0] = (byte)currentSendingPackage.Cmd;
                                        currentSendingPackage.result = CmdResult.ACK;
                                        currentSendingPackage.ETTU_ReturnList.Add(TxtPkg);
                                        if (TxtPkg.Text[TxtPkg.Text.Length - 1] == 0xc7)
                                        {
                                            lock (stream)
                                            {
                                                System.Threading.Monitor.Pulse(stream);
                                            }
                                        }
                                    }
                                   

                                }
                                else
                                {
                                    //----------------------------------------------------------------------------------------------------

                                    bool bfind = false;
                                    for (int i = 0; i < TimeOutQueue.Count; i++)
                                    {
                                        SendPackage pkg = (SendPackage)TimeOutQueue.Dequeue();
                                        if (pkg.type == CmdType.CmdQuery &&  pkg.ETTU_RetCmd== TxtPkg.ETTU_Cmd && pkg.ETTU_RetSubCmd == TxtPkg.SubCmd)
                                        {
                                           
                                            pkg.ReturnTextPackage = TxtPkg;
                                            pkg.result = CmdResult.ACK;
                                            TxtPkg.Text[0] =(byte) currentSendingPackage.Cmd;
                                            //lock (stream)
                                            //{
                                            //    System.Threading.Monitor.Pulse(stream);
                                            //}
                                            currentSendingPackage.ETTU_ReturnList.Add(TxtPkg);
                                            if (TxtPkg.Text[TxtPkg.Text.Length - 1] == 0xc7)
                                            {
                                                lock (pkg)
                                                {
                                                    System.Threading.Monitor.Pulse(pkg);
                                                }
                                            }
                                            //lock (pkg)
                                            //{
                                            //    System.Threading.Monitor.Pulse(pkg);
                                            //}
                                            bfind = true;

                                        }
                                        else
                                            TimeOutQueue.Enqueue(pkg);  //put back time out queue
                                    }


                                    if (!bfind)
                                    {
                                        //----------------------------------------------------------------------------------------------------

                                        AckData[0] = DLE;
                                        AckData[1] = ACK;
                                        AckData[2] = (byte)TxtPkg.Seq;
                                        AckData[3] = (byte)(TxtPkg.Address / 256);
                                        AckData[4] = (byte)(TxtPkg.Address % 256);
                                        AckData[5] = (byte)(AckData[0] ^ AckData[1] ^ AckData[2] ^ AckData[3] ^ AckData[4]);
                                        if (OnBeforeAck != null)
                                        {
                                            OnBeforeAck(this, ref AckData);

                                        }

                                        lock (stream)
                                        {
                                            if (AckData.Length == 6)
                                                stream.Write(AckData, 0, AckData.Length);
                                            else
                                                AckData = new byte[6];

                                            ; //send ack
                                        }


                                        if (OnReport != null)
                                        {
                                            if(TxtPkg.ETTU_Cmd==0x25 && TxtPkg.ETTU_SubCmd==0x25)
                                                lock (stream)
                                                {
                                                    byte[] confirmCode=this.ETTU_PackData(0xffff, new byte[] { 0x26, 0x26 }, 0xc7,TxtPkg.Seq);
                                                    stream.Write(confirmCode, 0, confirmCode.Length);
                                                    if (this.OnTextSending != null)
                                                        this.OnTextSending(this, ref confirmCode);
                                                }

                                            OnReport(this, TxtPkg);
                                        }
                                    }
                                }
                              //  Console.WriteLine("<onReceiveText>" + TxtPkg.ToString());
                                if (this.OnReceiveText != null)
                                {
                                  
                                    OnReceiveText(this, TxtPkg);
                                }
                            }
                            break;
                        case ACK:

                           AckPkg=  ReadAck();
                           currentRespObj = AckPkg;

                           //lock (currentSendingPackage)
                           //{
                               //---------------------------------------------------------
                               ///  if (currentSendingPackage.type == CmdType.CmdSet)

                               //---------------------------------------------------------------
                               if (currentSendingPackage != null && currentSendingPackage.Seq == AckPkg.Seq)
                               {
                                   //Console.WriteLine("======================>Receive Seq :" + AckPkg.Seq);

                                   if (currentSendingPackage.type == CmdType.CmdSet)
                                   {
                                       currentSendingPackage.result = CmdResult.ACK;
                                      // Console.WriteLine(currentSendingPackage.Cmd + "receive ack!");
                                       lock (stream)
                                       {
                                           System.Threading.Monitor.Pulse(stream);
                                       }
                                   }

                               }
                               //----------------------------------------------------------------------------
                               else  //check pkg in TimeOutQueue
                               {
                                   for (int i = 0; i < TimeOutQueue.Count; i++)
                                   {
                                     
                                       SendPackage pkg = (SendPackage)TimeOutQueue.Dequeue();
                                      // Console.WriteLine("check wait queue:pkg.seq=" + pkg.Seq + ",ACK.seq=" + AckPkg.Seq);
                                       if (pkg.type == CmdType.CmdSet && AckPkg.Seq == pkg.Seq)
                                       {
                                           pkg.result = CmdResult.ACK;
                                           //lock (stream)
                                           //{
                                           //    System.Threading.Monitor.Pulse(stream);
                                           //}

                                           lock (pkg)
                                           {
                                               System.Threading.Monitor.Pulse(pkg);
                                           }

                                       }
                                       else
                                           TimeOutQueue.Enqueue(pkg);  //put back time out queue
                                   }

                               }
                           //}
                            //-------------------------------------------------------------------------------------------

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

        CmdResult GetCmdResult(NakPackage pkg)
        {
            if (pkg.GetErrBit(V2DLE.DLE_ERR_LCR))
                return CmdResult.LRC_ERR;
            else if (pkg.GetErrBit(V2DLE.DLE_ERR_FRAME))
                return CmdResult.Frame_ERR;
            else if (pkg.GetErrBit(V2DLE.DLE_ERR_CMD_ERR))
                return CmdResult.Cmd_Invalid;
            else if (pkg.GetErrBit(V2DLE.DLE_ERR_CMD_PARAM_OVERRANGE))
                return CmdResult.Param_OverRange;
            else if (pkg.GetErrBit(V2DLE.DLE_ERR_CMD_FAIL))
                return CmdResult.Cmd_Fail;
        
            else if (pkg.GetErrBit(V2DLE.DLE_ERR_PARITY_BIT_NO))
                return CmdResult.Parity_ERR;
          
            else
                return CmdResult.Unknown;
        }

        CmdResult GetCmdResult(TextPackage pkg)
        {
            if (pkg.GetErrBit(V2DLE.DLE_ERR_LCR))
                return CmdResult.LRC_ERR;
            else if (pkg.GetErrBit(V2DLE.DLE_ERR_FRAME))
                return CmdResult.Frame_ERR;
            else if (pkg.GetErrBit(V2DLE.DLE_ERR_CMD_ERR))
                return CmdResult.Cmd_Invalid;
            else if (pkg.GetErrBit(V2DLE.DLE_ERR_CMD_PARAM_OVERRANGE))
                return CmdResult.Param_OverRange;
            else if (pkg.GetErrBit(V2DLE.DLE_ERR_CMD_FAIL))
                return CmdResult.Cmd_Fail;
            else if (pkg.GetErrBit(V2DLE.DLE_ERR_PARITY_BIT_NO))
                return CmdResult.Parity_ERR;
            else
                return CmdResult.Unknown;
        }
        
        public System.IO.Stream GetStream()
        {
            return stream;
        }

        private TextPackage ReadText()
        {
            int Seq=0,Address=0,Len=0,LRC=0,HeadLRC=0;
            byte[] text=null;
            TextPackage textPackage = new TextPackage();
            Seq = stream.ReadByte();
            Address = stream.ReadByte() * 256;
            Address+= stream.ReadByte();
            Len = stream.ReadByte() * 256;
            Len += stream.ReadByte();
            HeadLRC = stream.ReadByte();
            textPackage.Seq = Seq;
            textPackage.Address = Address;
          //  textPackage.LRC = HeadLRC;

            if (HeadLRC != (((Address >> 8) & 0x00ff) ^ (Address & 0x00ff) ^ ((Len >> 8) & 0x00ff) ^ (Len & 0x00ff)))
            {
              // textPackage.HasErrors = true;
                textPackage.SetErrBit(V2DLE.DLE_ERR_FRAME, true);
                textPackage.eErrorDescription += "Hearder LRC Error!\r\n";
                //Console.WriteLine("Hearder LRC Error!");
                return textPackage;
            }
            else
            {
               
                text = new byte[Len];
                int rlen = 0;




                do
                {
                    rlen+=stream.Read(text,rlen, Len-rlen);
                    
                } while (rlen != Len);
                //for (int i = 0; i < Len; i++)
                //    text[i] = (byte)stream.ReadByte();
              
                
                //if (rlen != Len)
                //   Console.WriteLine("rLen={0}!=len={1}",rlen,Len);


                LRC=DLE ^ SOH ^ Seq ;
                for (int i = 0; i < text.Length; i++)
                    LRC ^= text[i];

                int tmp = stream.ReadByte();
                if (LRC !=tmp)// stream.ReadByte())
                {
                    textPackage.SetErrBit(V2DLE.DLE_ERR_LCR, true);
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
                
            }


        }

        private AckPackage ReadAck()
        {
            byte[] data = new byte[4];
            stream.Read(data, 0, 4);
           return new AckPackage(data);

        }

        private NakPackage ReadNak()
        {
            byte[] data = new byte[6];
            stream.Read(data, 0, 6);

            return new NakPackage(data);
        }

        private byte[] PackData(int address, byte[] text,byte seq)
        {

         //   byte []text=ETTU_PackData(0xffff,ettutext);
            byte[] data = new byte[9 + text.Length];

            data[0] = DLE;
            data[1] = SOH;
            data[2] = seq;
            data[3] = (byte)(address / 256);
            data[4] = (byte)(address % 256);
            data[5] = (byte)(text.Length / 256);
            data[6] = (byte)(text.Length % 256);
            data[7] = (byte)(data[3] ^ data[4] ^ data[5] ^ data[6]);
            for (int i = 0; i < text.Length; i++)
                data[8 + i] = text[i];

            byte LRC = 0;
            for (int i = 0; i < data.Length - 1; i++)
                LRC ^= data[i];

            data[data.Length - 1] = LRC;

            return data;
        }

        private  byte[] PackData(int address ,byte[] text)
        {

       //     byte[] text = ETTU_PackData(0xffff, ettutext);
            byte[] data=new byte[9+text.Length];

            data[0] = DLE;
            data[1] = SOH;
            data[2] =  GetSeq();
            data[3] = (byte)(address / 256);
            data[4] = (byte)(address % 256);
            data[5] = (byte)(text.Length / 256);
            data[6] = (byte)(text.Length % 256);
            data[7] =(byte)( data[3] ^ data[4] ^ data[5] ^ data[6]);
            for (int i = 0; i < text.Length; i++)
                data[8 + i] = text[i];

            byte LRC=0;
            for(int i=0;i<data.Length-1;i++)
                LRC ^= data[i];

            data[data.Length - 1] = LRC;

            return data;

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


     

        public string getDeviceName()
        {
            return this.m_devName;
        }

        public void setDeviceName(string devName)
        {
            this.m_devName = devName;
        }

     
    }



    }

