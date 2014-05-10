using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HWStatus;
using RemoteInterface;

namespace Comm.TC
{
     public  class ETAGTC : TCBase
    {

       // private byte m_trans_cycle = 1; //1:1min,5:5min
       
      //  private byte m_trans_mode = 1; //0:polling,1:active
      //  private byte m_event_mode = 0;//0:polling,1:active  20 sec 事件停止
        
     //   private byte m_hw_cycle = 0; //on change
   //      private byte m_devType = 17;  //avi
         public ETAGTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus,byte comm_state)
             : base(protocol, devicename, ip, port, deviceid, hw_status, opmode, opstatus,comm_state)
         {

             this.SetTransmitCycle(60);
             

           //  this.OnTCReport += new OnTCReportHandler(AVITC_OnTCReport);
            // this.OnConnectStatusChanged += new ConnectStatusChangeHandler(VDTC_OnConnectStatusChanged);
         }

         void AVITC_OnTCReport(object tc, TextPackage txt)
         {
             //throw new Exception("The method or operation is not implemented.");

            
         }


         public override void DownLoadConfig()
         {
            // throw new Exception("The method or operation is not implemented.");
         }

         public override RemoteInterface.I_HW_Status_Desc getStatusDesc()
         {
            return new AVI_HW_StatusDesc(this.DeviceName,this.m_hwstaus);
         }
     }
}
