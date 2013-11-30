using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace Comm.TC
{
   public  class IIDTC : TCBase
    {
       public  IIDTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus, byte comm_state)
           : base(protocol, devicename, ip, port, deviceid, hw_status,opmode,opstatus,comm_state)
        {
        }

        public override void DownLoadConfig()
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public override RemoteInterface.I_HW_Status_Desc getStatusDesc()
        {
           // throw new Exception("The method or operation is not implemented.");
            return new RemoteInterface.HWStatus.WD_HW_StatusDesc(this.DeviceName,new byte[]{0,0,0,0});
        }
        public override void OneMinTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //base.OneMinTimer_Elapsed(sender, e);
            try
            {
                if (this.IsTcpConnected)
                {


                    //if(this is Comm.TC.AVITC)
                    //         ConsoleServer.WriteLine("週期詢問狀態");
                    ////  ConsoleServer.WriteLine(this.DeviceName + "secdiff:" + this.TC_SetDateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
                    //try
                    //{
                    //    if(mincnt==0)
                    //    this.TC_GetHW_Status();
                    //    else
                    //    this.TC_SendCycleSettingData();
                    //}
                    //catch { ;}
                    SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.B, this.m_deviceid, new byte[] { 0x01 });
                    try
                    {
                        this.m_device.Send(pkg);
                    }
                    catch
                    {
                        ;
                    }


                    if (System.DateTime.Now - this.LastReceiveTime > new TimeSpan(0, 3, 0))
                    {
                        this.LastReceiveTime = System.DateTime.Now;
                        this.IsConnected = false;
                        // this.IsTcpConnected = true;
                        ConsoleServer.WriteLine(this.DeviceName + "communication keep silent over 3 min!!");

                        //  this.start_connect();

                    }
                }
                else if (!this.IsTcpConnected && !InConnect_Task)
                {
                    this.start_connect();
                }

                //   Util.GC();
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                ;
            }
        }

        public override void OneHourTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //base.OneHourTimer_Elapsed(sender, e);
        }

        public override void SetTransmitCycle(int cycle)
        {
            //base.SetTransmitCycle(cycle);
            
        }
        public override void TC_SendCycleSettingData()
        {
            //base.TC_SendCycleSettingData();
        }

       public override int TC_SetDateTime(int year, int mon, int day, int hour, int min, int sec)
       {
           return 0;
           //return base.TC_SetDateTime(year, mon, day, hour, min, sec);
       }


    }
}
