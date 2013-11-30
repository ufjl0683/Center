using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HWStatus;



namespace Comm.TC
{
  public    class RMSTC:TCBase
    {

      private byte curr_mode=2, curr_planno=0;

      
      public RMSTC(Protocol protocol , string devicename,string ip, int port, int deviceid):base(protocol,devicename,ip,port,deviceid)
          {

              new System.Threading.Thread(checkModeAndPlannoTask).Start() ;
          }


          public void SetModeAndPlanno(byte mode,byte planno)
          {

              curr_mode = mode;
              curr_planno = planno;
              TC_SetModeAndPlanNo(mode, planno);
          }

          public override I_HW_Status_Desc getStatusDesc()
          {
             // throw new Exception("The method or operation is not implemented.")
              return new RMS_HW_StatusDesc(this.m_hwstaus);
          }

           private void checkModeAndPlannoTask()
            {
                SendPackage pkg;
               byte mode,planno;
                while (true)
                {

                    try
                    {
                        if (this.IsConnected)
                        {
                          pkg=  TC_GetCurrModeAndPlanNo();

                          if (pkg != null && pkg.result== CmdResult.ACK)
                          {
                             mode= pkg.ReturnTextPackage.Text[8];
                             planno = pkg.ReturnTextPackage.Text[9];
                             if (mode != curr_mode || mode==0 && curr_planno!=planno )
                             {
                                 Console.WriteLine(this.ToString()+", mode or  planno err!");
                                 TC_SetModeAndPlanNo(curr_mode, curr_planno);
                             }
                              
                             
                          }

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(this.ToString() + "," + ex.Message);
                    }

                    System.Threading.Thread.Sleep(30 * 1000);
                    this.getHwStaus();
                }


            }

          private void TC_SetModeAndPlanNo(byte mode, byte planno)
          {
              checkConntected();
              SendPackage pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0x82, mode, planno });
              m_device.Send(pkg);

          }

          private SendPackage TC_GetCurrModeAndPlanNo()
          {
              checkConntected();
              SendPackage pkg=new SendPackage(CmdType.CmdQuery,CmdClass.A,m_deviceid,new byte[]{0x04,0x82});
               m_device.Send(pkg);
              return  pkg;
          }

          //public  void m_device_OnReport(object sender, TextPackage txtObj)
          //{
          //    throw new Exception("The method or operation is not implemented.");
          //}
      }
}
