using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HWStatus;
using RemoteInterface;

namespace Comm.TC
{
    public class SCMTC : OutputTCBase
    {

        public SCMTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus, byte comm_state)
            : base(protocol, devicename, ip, port, deviceid, hw_status, opmode, opstatus, comm_state)
       {
       }

       public override void TC_SetDisplayOff()
       {
          // throw new Exception("The method or operation is not implemented.");
       }

       public override string GetCurrentDisplayDecs()
       {
           return "";
          // throw new Exception("The method or operation is not implemented.");
       }

       protected override void CheckDisplayTask()
       {
          // throw new Exception("The method or operation is not implemented.");
       }

       public override void DownLoadConfig()
       {
           //throw new Exception("The method or operation is not implemented.");
          
       }

       public override I_HW_Status_Desc getStatusDesc()
       {
           return new SCM_HW_StatusDesc(this.DeviceName, m_hwstaus);
       }


        public override void ResetComm()
        {
            //base.ResetComm();
            try
            {
                byte[] data = new byte[] { 0x0f, 0x11 };
                SendPackage pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, data);
                this.Send(pkg);

            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }
        }

        public override byte[] TC_GetHW_Status()
        {
            byte[] data = new byte[] { 0x0f, 0x41 };
         
            SendPackage pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, data);
            this.Send(pkg);
            //if(pkg.ReturnTextPackage !=null)

            byte[] retdata = new byte[4];
            retdata[0] = pkg.ReturnTextPackage.Text[2];
            retdata[1] = pkg.ReturnTextPackage.Text[3];
            return retdata;
            //return base.TC_GetHW_Status();
        }

        public override void SetTransmitCycle(int cycle)
        {
            //base.SetTransmitCycle(cycle);
            ;

        }

        public override void TC_SendCycleSettingData()
        {
            //$base.TC_SendCycleSettingData();
           
            byte[] data=new  byte[]{0x0f,0x14,0};
            SendPackage pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, data);
            this.Send(pkg);
           
        }

        public override int TC_SetDateTime(int year, int mon, int day, int hour, int min, int sec)
        {
            //return base.TC_SetDateTime(year, mon, day, hour, min, sec);
          System.Data.DataSet ds=  this.m_protocol.GetSendDataSet("set_date_and_time");
          ds.Tables[0].Rows[0]["year"] = year-1911;
          ds.Tables[0].Rows[0]["month"] = mon;
          ds.Tables[0].Rows[0]["day"] = day;
          ds.Tables[0].Rows[0]["hour"] = hour;
          ds.Tables[0].Rows[0]["min"] = min;
          ds.Tables[0].Rows[0]["sec"] = sec;
          ds.Tables[0].Rows[0]["week"] = ((int)System.DateTime.Now.DayOfWeek) == 0 ? 7 : ((int)System.DateTime.Now.DayOfWeek);
          SendPackage pkg = m_protocol.GetSendPackage(ds, 0xffff);
          this.Send(pkg);
          return pkg.ReturnTextPackage.Text[2];
        }
   }
}
