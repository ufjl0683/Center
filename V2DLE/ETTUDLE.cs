using System;
using System.Collections.Generic;
using System.Text;

namespace Comm
{
  public  class ETTUDLE: Comm.I_DLE
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
        public static byte DLE_ERR_FRAME = 0x02;
        public static byte DLE_ERR_LCR = 0x01;
        public static byte DLE_ERR_ADDR_ERR = 0x04;
        public static byte DLE_ERRR_LEN_ERR = 0x08;
       
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
       
      // CCDU
      byte[] CONNECT_AGREE;
      byte[] DISCONNECT_AGREE;
      byte[] RESET_REQUEST;
      byte[] RESET_AGREE;
      byte[] REACK  ;
      byte[] GOOD_C ;
      byte[] GOOD_R;
      byte[] RESEND1_C ;
      byte[] RESEND1_R;
      byte[] RESEND2_C ;
      byte[] RESEND2_R ;
      byte[] RESEND3_C ;
      byte[] RESEND3_R;

      int state = 0;
      bool IsOffline = true;

      private  bool Enabled=true;

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
      public ETTUDLE(System.IO.Stream stream)
        {
            this.stream=stream;

            REACK = this.PackData(0xffff,new byte[] { 0x24, 0x00 });
            GOOD_C = this.PackData(0xffff,new byte[] { 0x24, 0x01 });
            GOOD_R = this.PackData(0xffff,new byte[] { 0x24, 0x02 });
            RESEND1_C =this.PackData(0xffff, new byte[] { 0x24, 0x03 });
            RESEND1_R = this.PackData(0xffff,new byte[] { 0x24, 0x04 });
            RESEND2_C = this.PackData(0xffff,new byte[] { 0x24, 0x05 });
            RESEND2_R = this.PackData(0xffff,new byte[] { 0x24, 0x06 });
            RESEND3_C = this.PackData(0xffff,new byte[] { 0x24, 0x07 });
            RESEND3_R = this.PackData(0xffff,new byte[] { 0x24, 0x08 });
            CONNECT_AGREE=this.PackData(0xff,new byte[]{0x21,0x21});
            DISCONNECT_AGREE = this.PackData(0xffff, new byte[] { 0x21, 0x23 });
            RESET_REQUEST = this.PackData(0xff, new byte[] { 0xFE, 0xFE });
            RESET_AGREE = this.PackData(0xffff, new byte[] { 0xFF, 0xFF });

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

        public void PrintQueueState()
        {
            
            Console.WriteLine("SendQueueA=" + SendQueueA.Count);
            Console.WriteLine("SendQueueB=" + SendQueueB.Count);
            Console.WriteLine("SendQueueC=" + SendQueueC.Count);
            Console.WriteLine("SendQueueD=" + SendQueueD.Count);
            Console.WriteLine("ReceiveQueue=" + ReceiveQueue.Count);
            Console.WriteLine("TimeOutQueue" + TimeOutQueue.Count);
        }
      public int getTotalQueueCnt()
      {
          return SendQueueA.Count + SendQueueB.Count + SendQueueC.Count + SendQueueD.Count + TimeOutQueue.Count;
      }
      public void Send(SendPackage[] pkgs)
      {
          throw new NotImplementedException();
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

      

        object SendTaskNotifyObj=new object();
      int sendState = 0;
      bool IsSendOffline = true;

      private int SendState4()
      {
          byte[] data;
          try
          {

              lock (stream)
              {
                  data = this.PackData(0xffff, currentSendingPackage.text);
                  stream.Write(data, 0, data.Length);
                  if (this.OnTextSending != null)
                      this.OnTextSending(this, ref data);
                  if (System.Threading.Monitor.Wait(stream, T0ms))
                  {

                      if ((CmdResult)currentRespObj == CmdResult.GOOD_C)
                      {

                          lock (currentSendingPackage)
                          {
                              System.Threading.Monitor.Pulse(currentSendingPackage);
                              return 1;
                          }


                      }
                      else if ((CmdResult)currentRespObj == CmdResult.RESEND3_C)
                      {
                          return -1;
                      }
                      else
                      {
                          return -1;
                      }



                  }
                  else
                  {
                      currentSendingPackage.result = CmdResult.TimeOut;
                      System.Threading.Monitor.Pulse(stream);
                      return 1;
                  }

              }


          }
          catch (Exception ex)
          {
              if (this.OnCommError != null)
                  OnCommError(this, ex);
              return -1;
              //  Console.WriteLine(ex.Message);
          }
      }
      private int SendState2()
      {
          byte[] data;
          try
          {

              lock (stream)
              {
                  data = this.PackData(0xffff, currentSendingPackage.text);
                  stream.Write(data, 0, data.Length);
                  if (this.OnTextSending != null)
                      this.OnTextSending(this, ref data);
                  if (System.Threading.Monitor.Wait(stream, T0ms))
                  {

                      if ((CmdResult)currentRespObj == CmdResult.GOOD_C)
                      {
                        
                              lock (currentSendingPackage)
                              {
                                  System.Threading.Monitor.Pulse(currentSendingPackage);
                                  return 1;
                              }
                          

                      }
                      else if ((CmdResult)currentRespObj == CmdResult.RESEND2_C)
                      {
                          return 4;
                      }
                      else
                      {
                          return -1;
                      }



                  }
                  else
                  {
                      currentSendingPackage.result = CmdResult.TimeOut;
                      System.Threading.Monitor.Pulse(stream);
                      return 1;
                  }
              }

         
          }
          catch (Exception ex)
          {
              if (this.OnCommError != null)
                  OnCommError(this, ex);
              return -1;
              //  Console.WriteLine(ex.Message);
          }
      }
      private int SendState1()
      {

          byte[] data;
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
                  else if (SendQueueD.Count != 0)
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

                  lock (stream)
                  {
                      data = this.PackData(0xffff, currentSendingPackage.text);
                      stream.Write(data, 0, data.Length);
                      if (this.OnTextSending != null)
                          this.OnTextSending(this, ref data);
                      if (System.Threading.Monitor.Wait(stream, T0ms))
                      {

                          if ((CmdResult)currentRespObj == CmdResult.GOOD_C)
                          {
                              
                                  lock (currentSendingPackage)
                                  {
                                      System.Threading.Monitor.Pulse(currentSendingPackage);
                                      continue;
                                  }
                            

                          }
                          else if ((CmdResult)currentRespObj == CmdResult.RESEND1_C)
                          {
                              return 2;
                          }
                          else
                          {
                              return -1;
                          }



                      }
                  }

                  #region old Sending Proc
                  //if (currentSendingPackage.sendCnt==0)
                  //        data = PackData(currentSendingPackage.address, currentSendingPackage.text);
                  //        else
                  //        data = PackData(currentSendingPackage.address, currentSendingPackage.text,(byte)currentSendingPackage.Seq);

                  //        currentSendingPackage.Seq = data[2];

                  //        if (currentSendingPackage.sendCnt >= 4)   //discard package
                  //        {
                  //            //if (this.OnCommError != null)
                  //            //    OnCommError(this,new Exception("Time Out Error!"));

                  //            lock (currentSendingPackage)
                  //            {
                  //                System.Threading.Monitor.Pulse(currentSendingPackage);
                  //            }
                  //            continue;
                  //        }
                  //        currentSendingPackage.sendCnt++;
                  //        if (OnSendingPackage != null)
                  //            OnSendingPackage(this, currentSendingPackage);
                  //        lock (stream)
                  //        {

                  //            if (OnTextSending != null)
                  //                OnTextSending(this,ref data);

                  //          stream.Write(data, 0, data.Length);
                  //          stream.Flush();


                  //            if (System.Threading.Monitor.Wait(stream, T0ms))
                  //            {
                  //                //No Time Out




                  //                if (currentSendingPackage.result == CmdResult.ACK)
                  //                {
                  //                    lock (currentSendingPackage)
                  //                    {
                  //                        System.Threading.Monitor.Pulse(currentSendingPackage);
                  //                    }
                  //                }
                  //                else
                  //                    TimeOutQueue.Enqueue(currentSendingPackage);//nak ,time out


                  //            }
                  //            else
                  //            {
                  //                //Time out
                  //                if( currentSendingPackage.result==CmdResult.Unknown)
                  //                currentSendingPackage.result = CmdResult.TimeOut;

                  //                TimeOutQueue.Enqueue(currentSendingPackage);
                  //            }
                  //        }
                  #endregion
              }
              catch (Exception ex)
              {
                  if (this.OnCommError != null)
                      OnCommError(this, ex);
                  //  Console.WriteLine(ex.Message);
              }
          }
      }


      private int SendState0()
      {
          byte[] data;
          IsSendOffline = true;
          currentSendingPackage = new SendPackage(CmdType.CmdQuery, CmdClass.A, 0xffff, new byte[] { 0x20, 0x20 });
          while (true)
          {
              lock (stream)
              {
                  data = PackData(0xffff, currentSendingPackage.text);
                  stream.Write(data, 0, data.Length);
                  if (this.OnTextSending != null)
                      this.OnTextSending(this, ref data);
                  if (System.Threading.Monitor.Wait(stream, T0ms))
                  {
                      return 1;
                  }


              }
          }
      }

        void SendTask()
        {

            //byte[]data=null;

            while (true)
            {
                Console.WriteLine("SendState:" + sendState);
              switch(sendState)
              {
                  case 0:
                      sendState = SendState0();
                      
                      break;
                  case 1:
                      sendState = SendState1();
                     
                        break; // state=1
                  
                  case 2:
                      sendState = SendState2();
                      break;
                  case 3:
                      break;
                  case 4:
                      sendState = SendState4();
                      break;
                  case 5:
                      break;
                  case -1:
                      ResetProcedure();
                      break;
                 } //switch
             } //while

       




            


        }
      private int ResetProcedure()
      {
         // throw new System.NotImplementedException();

          lock (stream)
          {
              int trycnt = 0;
              while (true)
              {
                  stream.Write(RESET_REQUEST, 0, RESET_REQUEST.Length);
                  if (System.Threading.Monitor.Wait(stream, T0ms))
                      return 0;
                  trycnt++;
                  if (trycnt == 3)
                  {
                      
                      return 0;
                  }
              }

          }
         // return 0;

      }
      private int  state0()
      {
          TextPackage txt;
          IsOffline= true;
          while (true)
          {
              txt = ReadText();
              if (txt.HasErrors) continue;

              if (txt.Text[0] == 0x20 && txt.Text[1] == 0x20) //連線設定
              {

                  this.stream.Write(CONNECT_AGREE, 0, CONNECT_AGREE.Length);
                  IsOffline = false;
                  return 1; //state1
              }
              
          }
         
      }
      private int  state1()
      {
          TextPackage txt;
          int cn_cnt = 0;
          int err_cnt = 0;
          while (true)
          {
              if (IsOffline)
              {

                  txt = ReadText();
                  if (txt.HasErrors)
                  {
                      err_cnt++;
                      if (err_cnt == 3)
                          return -1;  // reset procedure
                      continue;
                  }
                  else
                      err_cnt = 0;


                  if (txt.Text[0] == 0x20 && txt.Text[1] == 0x20) //連線設定
                  {

                     
                         WriteStream(CONNECT_AGREE);
                     
                      cn_cnt++;
                      if (cn_cnt == 3)
                          return -1;  //reset
                  }
                  else
                  {
                      cn_cnt = 0;
                      if (txt.Text[0] == 0x21 || txt.Text[0] == 0x22 || txt.Text[0] == 0x23 || txt.Text[0] == 0xfe || txt.Text[0] == 0xff || txt.Text[0] == 0x24)
                          return -1;
                      else  //正確  dcu
                      {
                         
                              WriteStream(GOOD_C);
                          
                          return 2;
                      }


                  }
              }
              else  // isoffline=false;
              {
                  txt = ReadText();
                  if (IsCC(txt))
                      return -1;
                  if (txt.HasErrors)
                  {
                     
                         WriteStream(RESEND1_C);
                          return 3;
                      
                  }
                   
                  if (IsSC(txt) || IsDC(txt))
                  {
                     
                       WriteStream(GOOD_C);
                     //
                       if (IsDC(txt))
                           ProcessDC(txt);
                       if (IsSC(txt))
                           ProcessSC(txt);
                      return 2;
                  }
                  else
                      return -1;// reset

              }


          }
      }

      private void WriteStream( byte[]data)
      {
          lock (stream)
          {
              stream.Write(data,0,data.Length);
              stream.Flush();
          }
      }
      private int state2()
      {
          TextPackage txt;
          while (true)
          {
              txt = ReadText();
              if(IsCC(txt))
              {
                  ProcessCC(txt);
                  continue;
              }
              if (!txt.HasErrors && (IsSC(txt) || IsDC(txt)))
              {
                  WriteStream(GOOD_C);
                  if (IsDC(txt))
                      ProcessDC(txt);
                  if (IsSC(txt))
                      ProcessSC(txt);
              }

              if (IsCC(txt))
              {
                  if (txt.Text[0] == 0x24 && txt.Text[1] == 0x00)
                  {
                      WriteStream(GOOD_R);
                      return 4;
                  }
                  else

                      return -1;
              }



              if (txt.HasErrors)
              {
                 
                      WriteStream(RESEND1_C);
                      return 5;
             }



          }
      }
      private int state3()
      {
          TextPackage txt;
          while (true)
          {
              txt = ReadText();
              if (!txt.HasErrors && (IsDC(txt) || IsSC(txt)))
              {
                  WriteStream(GOOD_C);
                  if(IsDC(txt))
                      ProcessDC(txt);
                  if(IsSC(txt))
                      ProcessSC(txt);
                  return 2;
              }
              if(!txt.HasErrors && IsCC(txt))
              {
                  if (txt.Text[0] == 0x24 && txt.Text[1] == 0x00)
                  {
                      WriteStream(GOOD_R);
                      return 6;
                  }
                  else
                      return -1;

              }

              if (txt.HasErrors)
              {
                  WriteStream(RESEND2_C);
                  return 7;
              }
          }
      }

      private int state4()
      {
          TextPackage txt;
          while (true)
          {
              txt = ReadText();
              if (!txt.HasErrors && (IsDC(txt) || IsSC(txt)))
              {
                  WriteStream(GOOD_C);
                  if (IsDC(txt))
                      ProcessDC(txt);
                  if (IsSC(txt))
                      ProcessSC(txt);
                  return 2;
              }
              if (!txt.HasErrors && IsCC(txt))
              {
                  if (txt.Text[0] == 0x24 && txt.Text[1] == 0x00)//reack
                  {
                     
                      return -1;
                  }
                  else
                      return -1;

              }

              if (txt.HasErrors)
              {
                  WriteStream(RESEND1_C);
                  return 5;
              }
          }
      }

      private int state5()
      {
          TextPackage txt;
          while (true)
          {
              txt = ReadText();
              if (!txt.HasErrors && (IsDC(txt) || IsSC(txt)))
              {
                  WriteStream(GOOD_C);
                  if (IsDC(txt))
                      ProcessDC(txt);
                  if (IsSC(txt))
                      ProcessSC(txt);
                  return 2;
              }
              if (!txt.HasErrors && IsCC(txt))
              {
                  if (txt.Text[0] == 0x24 && txt.Text[1] == 0x00)
                  {
                      WriteStream(RESEND1_R);
                      return 6;
                  }
                  else
                      return -1;

              }

              if (txt.HasErrors)
              {
                  WriteStream(RESEND2_C);
                  return 7;
              }
          }

      }

      private int state6()
      {
          TextPackage txt;
          while (true)
          {
              txt = ReadText();
              if (!txt.HasErrors && (IsDC(txt) || IsSC(txt)))
              {
                  WriteStream(GOOD_C);
                  if (IsDC(txt))
                      ProcessDC(txt);
                  if (IsSC(txt))
                      ProcessSC(txt);
                  return 2;
              }
              if (!txt.HasErrors && IsCC(txt))
              {
                  if (txt.Text[0] == 0x24 && txt.Text[1] == 0x00) //reack
                  {
                      return -1;
                  }
                  else
                      return -1;

              }

              if (txt.HasErrors)
              {
                  WriteStream(RESEND2_C);
                  return 7;
              }
          }

      }

      private int state7()
      {
          TextPackage txt;
          while (true)
          {
              txt = ReadText();
              if (!txt.HasErrors && (IsDC(txt) || IsSC(txt)))
              {
                  WriteStream(GOOD_C);
                  if (IsDC(txt))
                      ProcessDC(txt);
                  if (IsSC(txt))
                      ProcessSC(txt);
                  return 2;
              }
              if (!txt.HasErrors && IsCC(txt))
              {
                  if (txt.Text[0] == 0x24 && txt.Text[1] == 0x00) //reack
                  {
                      WriteStream(RESEND2_R);
                      return 8;
                  }
                  else
                      return -1;

              }

              if (txt.HasErrors)
              {
                  WriteStream(RESEND3_C);
                  return 9;
              }
          }

      }

      private int state8()
      {
          TextPackage txt;
          while (true)
          {
              txt = ReadText();
              if (!txt.HasErrors && (IsDC(txt) || IsSC(txt)))
              {
                  WriteStream(GOOD_C);
                  if (IsDC(txt))
                      ProcessDC(txt);
                  if (IsSC(txt))
                      ProcessSC(txt);
                  return 2;
              }
              if (!txt.HasErrors && IsCC(txt))
              {
                  if (txt.Text[0] == 0x24 && txt.Text[1] == 0x00) //reack
                  {
                     // WriteStream(RESEND2_R);
                      return -1;
                  }
                  else
                      return -1;

              }

              if (txt.HasErrors)
              {
                  WriteStream(RESEND3_C);
                  return 9;
              }
          }

      }

      private int state9()
      {
          TextPackage txt;
          while (true)
          {
              txt = ReadText();

              if (!txt.HasErrors && IsCC(txt))
              {
                  if (txt.Text[0] == 0x24 && txt.Text[1] == 0x00) //reack
                  {
                      WriteStream(RESEND3_R);
                      return 10;
                  }

              }
              else
                  return -1; 

            
          }

      }

      private int state10()
      {
          ReadText();
          return -1;
      }

      private void ProcessCC(TextPackage txt)
      {

          switch (txt.Text[0] * 256 + txt.Text[1])
          {
              case 0x2400: //reack
                  this.currentRespObj = CmdResult.REACK;
                               

                      break;
              case 0x2401:  //GOOD_C
                  this.currentRespObj = CmdResult.GOOD_C;
                  if (currentSendingPackage.type == CmdType.CmdQuery)
                      return;
                  
                  break;
              case 0x2402: //GOOD_R
                  this.currentRespObj = CmdResult.GOOD_R;
                  break;
              case 0x2403: //resend1_c
                  this.currentRespObj = CmdResult.RESEND1_C;
                  break;
              case 0x2404: //resend1_r
                  this.currentRespObj = CmdResult.RESEND1_R;
                  break;
              case 0x2405: // resend2_c
                  this.currentRespObj = CmdResult.RESEND2_C;
                  break;
              case 0x2406: // resend2_r
                  this.currentRespObj = CmdResult.RESEND2_R;
                  break;
              case 0x2407:  //resend3_c
                  this.currentRespObj = CmdResult.RESEND3_C;
                  break;
              case 0x2408://resend3_r
                  this.currentRespObj = CmdResult.RESEND3_R;
                  break;

          }
          lock (stream)
          { 
              System.Threading.Monitor.Pulse(stream);
          }

      }
      private void  ProcessDC(TextPackage txt)
      {
          if (currentSendingPackage.type == CmdType.CmdQuery)
          {
              if (currentSendingPackage!=null &&  currentSendingPackage.ETTU_RetCmd == txt.Text[0] && currentSendingPackage.ETTU_RetSubCmd == txt.Text[1])
              {
                  currentSendingPackage.ETTU_ReturnList.Add(txt);
                  currentSendingPackage.result = CmdResult.ACK;
                  if (txt.CCU_EndCode == 0xc7)
                  {

                      lock (stream)
                      {   
                          System.Threading.Monitor.Pulse(stream);
                      }
                  }
              }
              else
              {
                  if (this.OnReport != null)
                      this.OnReport(this, txt);
              }
          }
                        
      }



      private void  ProcessSC(TextPackage txt)
      {
          switch (txt.Text[0]*256+txt.Text[1])
          {
              case 0x2020:  //連線設定
                  
                     WriteStream(CONNECT_AGREE);
                  
                  break;
              case 0x2121:  //連線同意
                  ProcessDC(txt);
                  break;
             
              case 0x2122: //離線設定
                  
                     WriteStream(DISCONNECT_AGREE);
                
                  break;
              case 0x2123:  //離線同意

                  ProcessDC(txt);
                  break;
              case 0xfefe:  //重社要求 
                  WriteStream(RESET_AGREE);
                  break;
              case 0xffff: // 重設同意
                  ProcessDC(txt);
                  break;

          }
      }


      private bool IsCC(TextPackage txt)
      {
          if (txt.Text[0] == 0x24)
              return true;
          else
              return false;
      }
      private bool IsSC(TextPackage txt)
      {
          if (txt.Text[0] == 0x20 || txt.Text[0] == 0x21 || txt.Text[0] == 0x22 || txt.Text[0] == 0x23 || txt.Text[0] == 0xfe || txt.Text[0] == 0xff)
              return true;
          else
              return false;
      }
      private bool IsDC(TextPackage txt)
      {
          return !(IsCC(txt) || IsSC(txt));  
      }






        void ReceiveTask()
        { 
           

           
            bool dle_flag = false;
             AckPackage AckPkg=null;
            NakPackage NakPkg = null;
            TextPackage TxtPkg = null;
            while (true)
            {

                try
                {
                    Console.WriteLine("state:" + state);
                    switch (state)
                    {
                        case 0:
                           state=   state0();
                            break;
                        case 1:
                            state = state1();
                            break;
                        case 2:
                            state = state2();
                            break;
                        case 3:
                            state = state3();
                            break;
                        case 4:
                            state = state4();
                            break;
                        case 5:
                            state = state5();
                            break;
                        case 6:
                            state = state6();
                            break;
                        case 7:
                            state = state7();
                            break;
                        case 8:
                            state = state8();
                            break;
                        case 9:
                            state = state9();
                            break;
                        case 10:
                            state = state10();
                            break;
                        case -1:  //reset proc
                            break;
                    }

                    #region  tc_d2le

                    //if(!dle_flag) 
                    //while (  (d = stream.ReadByte()) != DLE ) ;
               
                    //dle_flag = false;
                    //switch ((d = stream.ReadByte()))
                    //{
                    //    case STX:
                          
                    //        TxtPkg=  ReadText();
                    //        currentRespObj = TxtPkg;
                            
                    //        if (OnTextCmdCheck != null)
                    //            OnTextCmdCheck(this,TxtPkg);

                    //        if (TxtPkg.HasErrors)
                    //        {
                                
                    //                NakData[0] = DLE;
                    //                NakData[1] = NAK;
                    //                NakData[2] = (byte)TxtPkg.Seq;
                    //                NakData[3] = (byte)(TxtPkg.Address / 256);
                    //                NakData[4] = (byte)(TxtPkg.Address % 256);
                    //                NakData[5] = 0x00;
                    //                NakData[6] = 0x09;
                    //                NakData[7] = TxtPkg.Err[0];
                    //                NakData[8] = (byte)(NakData[0] ^ NakData[1] ^ NakData[2] ^ NakData[3] ^ NakData[4] ^ NakData[5] ^ NakData[6] ^ NakData[7]^ETX);
                    //                if (OnBeforeNak != null)
                    //                    OnBeforeNak(this, ref NakData);                                
                    //                lock (stream)
                    //                {
                    //                    stream.Write(NakData, 0, NakData.Length);
                    //                }
                                       
                               
                    //        }
                    //        else
                    //        {  
                    //            //
                                
                    //            AckData[0] = DLE;
                    //            AckData[1] = ACK;
                    //            AckData[2] = (byte)TxtPkg.Seq;
                    //            AckData[3] = (byte)(TxtPkg.Address / 256);
                    //            AckData[4] = (byte)(TxtPkg.Address % 256);
                    //            AckData[5] = 0x00;
                    //            AckData[6] = 0x08;
                    //            AckData[7] = (byte)(AckData[0] ^ AckData[1] ^ AckData[2] ^ AckData[3] ^ AckData[4] ^ AckData[5] ^ AckData[6]^ETX);
                    //            if (OnBeforeAck != null)
                    //            {
                    //                OnBeforeAck(this, ref AckData);
                                    
                    //            }

                    //            lock (stream)
                    //            {
                    //                if (AckData.Length == 8)
                    //                    stream.Write(AckData, 0, AckData.Length);
                    //                else
                    //                    AckData = new byte[8];
                                   
                    //                ; //send ack
                    //            }

                    //            if (currentSendingPackage != null && currentSendingPackage.returnCmd == TxtPkg.Cmd && (currentSendingPackage.returnSubCmd | 0x80) == TxtPkg.SubCmd)
                    //            {
                    //                currentSendingPackage.result = CmdResult.ACK;
                    //                currentSendingPackage.ReturnTextPackage = TxtPkg;

                    //                lock (stream)
                    //                {
                    //                    System.Threading.Monitor.Pulse(stream);
                    //                }
                                   

                    //            }
                    //            else
                    //            {
                    //                if (OnReport != null)
                    //                    OnReport(this,TxtPkg);
                    //            }
                    //            if (this.OnReceiveText != null)
                    //                OnReceiveText(this,TxtPkg);
                    //        }
                    //        break;
                    //    case ACK:

                    //       AckPkg=  ReadAck();
                    //       currentRespObj = AckPkg;

                    //       if (currentSendingPackage !=null  &&  currentSendingPackage.Seq == AckPkg.Seq )
                    //       {  
                    //           currentSendingPackage.result= CmdResult.ACK;
                    //           if(currentSendingPackage.type==CmdType.CmdSet )
                    //           lock (stream)
                    //           {
                    //               System.Threading.Monitor.Pulse(stream);
                    //           }
                    //       }

                    //       if (OnAck != null)
                    //           OnAck(this,AckPkg);

                    //        break;
                    //    case NAK:
                    //        NakPkg=  ReadNak();
                    //        if (currentSendingPackage!=null &&  currentSendingPackage.Seq == NakPkg.Seq)
                    //        {
                    //            currentSendingPackage.result = GetCmdResult(NakPkg);
                    //            lock (stream)
                    //            {
                    //                System.Threading.Monitor.Pulse(stream);
                    //            }


                    //        }
                    //        if (OnNak != null)
                    //            OnNak(this,NakPkg);
                    //        break;
                        
                           
                    //    default:
                    //        if (d == DLE)
                    //        {
                    //            dle_flag = true;
                    //        }
                    //        break;
                    //}

                    #endregion
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

      //----------------------Read CDU
        private TextPackage ReadText()
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            byte data;
            TextPackage ret = new TextPackage();

            while (true)
            {
                data =(byte) stream.ReadByte();
                ms.WriteByte(data);
                if (data == 0xF8 || data == 0xC7)
                    break;
            }

            byte[] databuff = ms.ToArray();

            uint LCR = 0;
            byte lcr1, lcr2;
            for (int i = 0; i < databuff.Length - 3; i++)  //cal lcr
                LCR += databuff[i];
            LCR =(uint)( (~LCR) + 1);  //2's complement
            lcr1 =(byte)( LCR & 0x0f);
            lcr2 =(byte)( (LCR >> 4) & 0x0f);
            ret.Text = new byte[databuff.Length - 3];

            ret.CCU_EndCode = databuff[databuff.Length - 1];

            System.Array.Copy(databuff, ret.Text, ret.Text.Length);
            if (!(lcr1 == databuff[databuff.Length - 2] && lcr2 == databuff[databuff.Length - 3]))
            {
               
                ret.SetErrBit((int)V2DLE.DLE_ERR_LCR,true);
                ret.eErrorDescription="LCR Error!";
                ret.LRC =(byte)( (lcr2 << 4) | lcr1);

            }
            else
            {
                ret.LRC = (byte)LCR;
            }

            if(this.OnReceiveText!=null)
            this.OnReceiveText(this, ret);
            return ret;

        }
      //private TextPackage ReadText()
      //{
      //    int Seq = 0, Address = 0, Len = 0, LRC = 0;//,HeadLRC=0;
      //    byte[] text = null;
      //    TextPackage textPackage = new TextPackage();
      //    Seq = stream.ReadByte();
      //    Address = stream.ReadByte() * 256;
      //    Address += stream.ReadByte();
      //    Len = stream.ReadByte() * 256;
      //    Len += stream.ReadByte();
      //    //   HeadLRC = stream.ReadByte();
      //    textPackage.Seq = Seq;
      //    textPackage.Address = Address;



      //    text = new byte[Len];
      //    int rlen = 0;




      //    do
      //    {
      //        rlen += stream.Read(text, rlen, Len - rlen);

      //    } while (rlen != Len);
      //    //for (int i = 0; i < Len; i++)
      //    //    text[i] = (byte)stream.ReadByte();



      //    stream.ReadByte();   //READ DLE
      //    stream.ReadByte();    // READ ETX

      //    LRC = STX ^ Seq ^ ETX ^ Len / 256 ^ Len % 256;
      //    for (int i = 0; i < text.Length; i++)
      //        LRC ^= text[i];

      //    int tmp = stream.ReadByte();
      //    if (LRC != tmp)// stream.ReadByte())
      //    {
      //        textPackage.SetErrBit(TCDLE30.DLE_ERR_LCR, true);
      //        textPackage.eErrorDescription += "LRC Error!\r\n";
      //        Console.WriteLine("LRC Error!");
      //        return textPackage;
      //    }
      //    else
      //    {
      //        textPackage.Text = text;
      //        textPackage.LRC = LRC;
      //        return textPackage;

      //    }

      //    //  }


      //}

      //  private TCDLE30AckPackage ReadAck()
      //  {
      //      byte[] data = new byte[6];
      //      stream.Read(data, 0, 6);
      //     return new TCDLE30AckPackage(data);

      //  }

        private NakPackage ReadNak()
        {
            byte[] data = new byte[7];
            stream.Read(data, 0, 7);

            return new TCDLE30NakPackage(data);
        }

        public byte[] PackData(int address, byte[] text,byte EndCode)
        {
            byte[] data = new byte[3 + text.Length];
            uint LCR = 0;
            System.Array.Copy(text, data, text.Length);
            for (int i = 0; i < text.Length; i++)
                LCR += text[i];

            LCR = (~LCR) + 1;
            data[text.Length] = (byte)((LCR >>4) & 0x0f);
            data[text.Length + 1] = (byte)(LCR & 0x0f);
            data[data.Length - 1] = EndCode;
            return data;
        }
        //check ettu
        private  byte[] PackData(int address ,byte[] text)  // End with C7,or F8
        {


            return PackData(address, text, 0xc7);
          

        }

        //private byte GetSeq()
        //{

        //    Seq = (byte)((Seq + 1) % 128);
        //    return Seq;
        //}

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
