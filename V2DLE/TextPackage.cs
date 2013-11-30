using System;
using System.Collections.Generic;
using System.Text;

namespace Comm
{
  public   class TextPackage
    {

    
  //  public   byte[] text = null;
    public  int Seq=0, Address=0, LRC=0;
    public   byte[] Text=null;
    public byte CCU_EndCode;
    public  byte[] Err = new byte[2] { 0, 0 };
    public  string eErrorDescription="";
    public bool IsQueryCmd=false;
      public TextPackage()
      {

      }

      

      public bool IsLastCDU()
      {
          return this.CCU_EndCode == 0xc7;

      }
      public bool HasErrors
      {
          get
          {
              return Err[0] + Err[1] != 0;
          }
      }

      public byte ETTU_Cmd
      {
          get
          {
              return Text[0];
          }

      }

      public byte ETTU_SubCmd
      {
          get
          {
              return Text[1];
          }
      }


      public byte ETTU_EndCode
      {
          get
          {
              return Text[Text.Length - 1];
          }
      }

      public byte[] ETTU_Text
      {
          get
          {
              byte[] data = new byte[Text.Length-3];
              data[0] = Text[0];
              data[1] = Text[1];
              for (int i = 2; i < Text.Length - 3; i++)
                  data[i] = Text[i];
              return data;
          }
      }

      public void SetErrBit(int inx, bool b)
      {
          if (!(inx >= 0 && inx <= 15))
              return;
          // ((((data[3] * 256 + data[4]) >> inx) & 1) == 1);
          if (inx >= 0 && inx < 8)
          {
              if (b)
                  Err[1] |= (byte)(1 << inx);
              else
                  Err[1] &= (byte)~(1 << inx);

          }
          else
          {
              inx %= 8;
              if (b)
                  Err[0] |= (byte)(1 << inx);
              else
                  Err[0] &= (byte)~(1 << inx);

          }
      }
      public bool GetErrBit(int inx)
      {
          if (!(inx >= 0 && inx <= 15))
              return false;
          return ((((Err[0] * 256 + Err[1]) >> inx) & 1) == 1);
      }

      public int  Cmd
      {
          get
          {
              if (Text == null)
                  return -1;

              else
                  return Text[0];  
              
          }
      }

      public int SubCmd
      {
          get
          {
              if ((Cmd & 0x0f) == 0x0f)
                  return Text[1];
              else
                  return 0xff; //0xff
          }
      }


      public override string ToString()
      {
          StringBuilder retsStr = new StringBuilder();

          retsStr.Append( string.Format("address:0x{0:X2} Seq:{1:X2} cmd:0x{2:X2} \r\n",Address,Seq,Cmd));
          retsStr.Append("\t"+V2DLE.ToHexString(Text));

          return retsStr.ToString();
      }

      
    }
}
