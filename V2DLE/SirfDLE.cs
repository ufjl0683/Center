using System;
using System.Collections.Generic;
using System.Text;

namespace Comm
{
  public  class SirfDLE:I_DLE
    {
        const byte STX = 0xA0;
        const byte SOH = 0xA2;
        const byte ETX = 0xB0;
        const byte EOH = 0xB3;
        public static byte DLE_ERR_PARITY_BIT_NO = 0;
        public static byte DLE_ERR_FRAME = 0x01;
        public static byte DLE_ERR_LCR = 0x02;
        public static byte DLE_ERR_TIMEOUT = 0x03;
        public static byte DLE_ERR_CMD_ERR = 0x4;
        public static byte DLE_ERR_CMD_PARAM_OVERRANGE = 0x5;
        public static byte DLE_ERR_CMD_FAIL = 0x6;

        private string m_devName;
        System.IO.Stream stream;
        System.Threading.Thread SendTaskThread;
        System.Threading.Thread ReceiveTaskThread;
        System.Collections.Queue ReceiveQueue = System.Collections.Queue.Synchronized(new System.Collections.Queue(100));
        bool m_enbable=true;
        bool IsClosed = false;
        public SirfDLE(string devName, System.IO.Stream stream)
        {
            this.m_devName = devName;
            this.stream = stream;

            ReceiveTaskThread = new System.Threading.Thread(ReceiveTask);
          //  SendTaskThread.Start();
            ReceiveTaskThread.Start();
        }


        void ReceiveTask()
        {
            int d=0,d1=0;
            bool dle_flag = false;
            int readcnt=0;
            TextPackage TxtPkg = null;
            for (int i = 0; i < 50; i++)
            {
                try
                {
                  d=  stream.ReadByte();
                  if (d == -1)
                      throw new Exception("Comm error!");
                }
                catch (Exception ex)
                {
                    if (OnCommError != null)
                    {

                        OnCommError(this, ex);
                    }
                    return;
                }
            }
            while (true && !IsClosed)
            {

               //d = stream.ReadByte();
               // if (d == 0xb0)
               // {
               //     d1 = stream.ReadByte();
               //     if (d1 == 0xb3)
               //         Console.WriteLine("0xB0 0xB3");
               //     else
               //         Console.Write("0xB0 "+ d1.ToString("X2"));
               // }
               // else
               // {

               //     Console.Write(d.ToString("X2") + " ");
               // }
               //     continue;
              

                try
                {
                  

                       // while ((d = stream.ReadByte()) != STX) ;
                    readcnt = 0;
                    do
                    {
                        if (readcnt > 0)
                            Console.Write(this.m_devName+",frame error!");

                        d = stream.ReadByte();
                        
                   //     Console.WriteLine("{0:X2}", d);
                        readcnt++;
                        if (d == -1)
                            throw new Exception("Comm error!");
                      //  Console.Write(Comm.V2DLE.ToHexString((byte)d));
                    }
                    while (d != STX);

                    d = stream.ReadByte();
                    if (d != SOH)
                        continue;
                    if (d == -1)
                        throw new Exception("Comm error!");
                    dle_flag = false;
                   // switch ((d = stream.ReadByte()))
                   // {
                    //    case SOH:

                            TxtPkg = ReadText();
                           // Console.WriteLine(stream.ReadByte().ToString("X2"));
                           // Console.WriteLine(stream.ReadByte().ToString("X2"));
                           d=  stream.ReadByte();
                           d= stream.ReadByte();
                            if (TxtPkg.HasErrors)
                            {
                                Console.WriteLine(TxtPkg.eErrorDescription);
                                Console.WriteLine(Comm.V2DLE.ToHexString(TxtPkg.Text));
                          
                            }
                            else
                            {
                                //orig ack place
                                  
                             if(OnReport != null)
                                            OnReport(this, TxtPkg);
                                        
                                  
                                if (this.OnReceiveText != null)
                                            OnReceiveText(this, TxtPkg);
                              
                            }
                          //  break;
                      


                    //}
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

        private TextPackage ReadText()
        {
            int len,devid;
            TextPackage txt = new TextPackage();
           // devid = stream.ReadByte();
            len =  stream.ReadByte()*256+stream.ReadByte();
             txt.Text = new byte[len];
            int rlen = 0;
            txt.Address = 0; ; // devid;
            do
            {
                rlen += stream.Read(txt.Text, rlen, len - rlen);

            } while (rlen != len);
            int cks = 0;
            foreach (byte d in txt.Text)
                cks += d;

            cks &=0x7fff;
            int tempcks = stream.ReadByte() * 256 + stream.ReadByte();

            if (cks != tempcks/*stream.ReadByte()*256+stream.ReadByte()*/)
            {
                txt.SetErrBit(SirfDLE.DLE_ERR_LCR, true);
                txt.eErrorDescription += getDeviceName() + "LRC Error!\r\n";

            }



            return txt;

         //   throw new NotImplementedException();
        }




        public void Close()
        {
            //throw new NotImplementedException();
            try
            {
               // this.SendTaskThread.Abort();
                this.IsClosed = true;
                this.ReceiveTaskThread.Abort();
            }
            catch { }
           
        }

        public System.IO.Stream GetStream()
        {
            //throw new NotImplementedException();
            return this.stream;
        }

        public event OnAckEventHandler OnAck;

        public event OnSendingAckNakHandler OnBeforeAck;

        public event OnSendingAckNakHandler OnBeforeNak;

        public event OnCommErrHandler OnCommError;

        public event OnNakEventHandler OnNak;

        public event OnTextPackageEventHandler OnReceiveText;

        public event OnTextPackageEventHandler OnReport;

        public event OnSendPackgaeHandler OnSendingPackage;

        public event OnTextPackageEventHandler OnTextCmdCheck;

        public event OnSendingAckNakHandler OnTextSending;

        public void Send(SendPackage pkg)
        {
           throw new NotImplementedException();
        }

        public void Send(SendPackage[] pkg)
        {
            throw new NotImplementedException();
        }

        public bool IsOnAckRegisted()
        {

            return this.OnAck == null ? false : true;
           // throw new NotImplementedException();
        }

        public string getQueueState()
        {
            throw new NotImplementedException();
        }

        public int getTotalQueueCnt()
        {
          //  throw new NotImplementedException();
            return ReceiveQueue.Count;
        }

        public void setEnable(bool enable)
        {
          //  throw new NotImplementedException();

            m_enbable = enable;
        }

        public bool getEnable()
        {
           // throw new NotImplementedException();

            return this.m_enbable;
        }

        public string getDeviceName()
        {
          //  throw new NotImplementedException();

            return this.m_devName;
        }

        public void setDeviceName(string devName)
        {
           // throw new NotImplementedException();
            m_devName = devName;
        }
    }
}
