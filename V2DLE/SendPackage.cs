using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Comm
{

    public enum CmdType
    {
        CmdSet,
        CmdQuery,
        CmdReport,
        CmdUnkonwn
    }
    public enum CmdClass
    {
        A,
        B,
        C,
        D
    }

    public enum CmdResult
    {
        ACK, LRC_ERR, Frame_ERR, Cmd_Invalid, Param_OverRange, TimeOut, Cmd_Fail, Unknown, TC_DLE_30_ADDR_ERR, TC_DEL_30_LEN_ERR, Parity_ERR,REACK, GOOD_C, GOOD_R,
        RESEND1_C,RESEND1_R,RESEND2_C,RESEND2_R,RESEND3_C,RESEND3_R

    }


    [Serializable]
  public   class SendPackage
    {
      public CmdClass cls = CmdClass.A;
      public CmdType type = CmdType.CmdUnkonwn;
      public byte[] text=null;
     
      public int returnCmd=-1; //-1 : set cmd
      public int returnSubCmd = 0xff;
     public  int sendCnt = 0;
      public  int address = -1;
      public int Seq = 0;
     // public DateTime BornDateTime = new DateTime();
      public CmdResult result= CmdResult.Unknown;
     // public TextPackage[] ReturnETTUTextPackage;  // for ettu
      public System.Collections.ArrayList ETTU_ReturnList = new System.Collections.ArrayList();
      public TextPackage ReturnTextPackage;
      public byte ETTU_EndCode=0xc7;
      
      volatile public bool IsDiscard = false;
      public SendPackage(CmdType type, CmdClass cls, int address,byte[] text)
      {
          this.type = type;
          this.cls = cls;
          this.text = text;
          this.address = address;
        //  this.BornDateTime=System.DateTime.Now;
          if (type == CmdType.CmdQuery)
              returnCmd = text[0];

          if ((text[0] & 0x0f) == 0x0f)
                 returnSubCmd = text[1];

      }


      public TextPackage[] ReturnETTUTextPackage
      {
          get
          {
              TextPackage[] txt = new TextPackage[ETTU_ReturnList.Count];
              for (int i = 0; i < ETTU_ReturnList.Count; i++)
                  txt[i] = (TextPackage)ETTU_ReturnList[i];
              return txt;
          }
      }

      public int ETTU_Cmd
      {
          get
          {
              return text[0];
          }
      }

      public   int  ETTU_SubCmd
      {
          get
          {
              //if (text[0] == 0x20 && text[1] == 0x20)  //connect request
              //    return 0x21;
              //else if (text[0] == 0x21 && text[1] == 0x22)
              //    return 0x23;
              //else
              return text[1];
          }
      }


      public int ETTU_RetCmd
      {
          get
          {
              if (text[0] == 0x20 && text[1] == 0x20)  //connect request
                  return 0x21;
              else if (text[0] == 0x21 && text[1] == 0x22)  // disconnect request
                  return 0x21;
              else if (text[0] == 0xfe && text[1] == 0xfe)
                  return 0xff;
              else
                  return text[0] + 2;
          }

      }

      

      public int ETTU_RetSubCmd
      {
          get
          {
              if (text[0] == 0x20 && text[1] == 0x20)  //connect request
                  return 0x21;
              else if (text[0] == 0x21 && text[1] == 0x22)  // disconnect request
                  return 0x23;
              else if (text[0] == 0xfe && text[1] == 0xfe)
                  return 0xff;
              else
              return text[1];
          }
      }
       public int Cmd
      {
         get
          {
              if (text == null)
                  return 0xff;
              else
                  return text[0] ;
          }
      }


      public int SubCmd
      {
          get
          {
              if ((text[0] & 0x0f) == 0x0f)
                  return text[1];
              else
                  return 0xff;//text[0];
          }
      }

      public override string ToString()
      {
          //return base.ToString();
          System.Text.StringBuilder sb = new StringBuilder(100);
          sb.Append(string.Format("cmd:{0:X2} type={1} address={2:X4} SendCnt={3} Seq=0x{4:X2} result={5}\r\n",Cmd,type,address,sendCnt,Seq,this.result));
          sb.Append("["+V2DLE.ToHexString(text)+"]");
          return sb.ToString();
      }
    }
}
