using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HWStatus;
using RemoteInterface;
using System.Data;

namespace Comm.TC
{
    public class FSTC : OutputTCBase
    {
        byte curr_type=0;
        public FSTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus, byte comm_state)
          : base(protocol, devicename, ip, port, deviceid, hw_status,opmode,opstatus, comm_state)
        {

          

          
            
        }
      public override void DownLoadConfig()
      {
        //  throw new Exception("The method or operation is not implemented.");
      }


      private void TC_SetDispalyOnOff(bool onoff)
      {
          byte[] data = new byte[] { 0xd0,(byte)((onoff)?1:0)};
          SendPackage pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, data);
          this.Send(pkg);
      }
      public override void TC_SetDisplayOff()
      {
          if(curr_type!=0)
              this.InvokeOutPutChangeEvent(this,this.GetDisplayDecs(0) );
          this.TC_SetDispalyOnOff(false);
          curr_type = 0;
         // throw new Exception("The method or operation is not implemented.");
      }

        public void TC_SetDisplay(byte type)
        {
            lock (this.currDispLockObj)
            {
                byte[] data = new byte[] { 0xd2, type };

                SendPackage pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, data);
                this.Send(pkg);
                if (curr_type != type)
                {
                    curr_type = type;
                    this.InvokeOutPutChangeEvent(this, GetDisplayDecs(type));

                }

                

                this.TC_SetDispalyOnOff(true);
            }
        }

        public byte TC_GetDisplayType()
        {
            byte[] data = new byte[] { 0x04,0xd2};
            SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, 0xffff, data);
            this.Send(pkg);
            if (pkg.result == CmdResult.ACK)
            {
                return pkg.ReturnTextPackage.Text[8];
            }
            else
                throw new Exception(pkg.result.ToString());

        }
      public override string GetCurrentDisplayDecs()
      {
          if (curr_type == 0)
              return "熄滅";
          else
          {
              switch (curr_type)
              {
                  case 1:
                      return "VI";
                  case 2:
                      return "WD";
                  case 3:
                      return "RD";
                  default:
                      throw new Exception("type 不符");
              }

         }
      }


        public  string GetDisplayDecs(byte type)
        {
            if (type == 0)
                return "熄滅";
            else
            {
                switch (type)
                {
                    case 1:
                        return "VI";
                    case 2:
                        return "WD";
                    case 3:
                        return "RD";
                    default:
                        throw new Exception("type 不符");
                }

            }
        }

      protected override void CheckDisplayTask()
      {
         //throw new Exception("The method or operation is not implemented.");
          if (curr_type==0 || !IsConnected) return;

          try
          {
              byte type;
              if ((type=this.TC_GetDisplayType()) != curr_type)
              {
                  this.TC_SetDisplay(curr_type);
                  this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), GetDisplayDecs(type));
              }

          }
          catch(Exception ex)
          {
              ConsoleServer.WriteLine(this.DeviceName + "," + ex.Message+ex.StackTrace);
          }

          
      }

      public override RemoteInterface.I_HW_Status_Desc getStatusDesc()
      {
          return new FS_HW_StatusDesc(this.m_deviceName, m_hwstaus);
      }
  }
}
