using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace Comm.TC
{
   public  class ETTUTC : TCBase
    {
        public ETTUTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus, byte comm_state)
            : base(protocol, devicename, ip, port, deviceid, hw_status, opmode, opstatus, comm_state)
        {

          
        }

        public override void DownLoadConfig()
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public override RemoteInterface.I_HW_Status_Desc getStatusDesc()
        {
            // throw new Exception("The method or operation is not implemented.");
            return new RemoteInterface.HWStatus.WD_HW_StatusDesc(this.DeviceName, new byte[] { 0, 0, 0, 0 });
        }

       public override byte[] TC_GetHW_Status()
       {
           return new byte[] { 0, 0, 0, 0 };
       }

       public override void ResetComm()
       {
           //base.ResetComm();
       }
        public override void OneMinTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //base.OneMinTimer_Elapsed(sender, e);
            try
            {

                if (DateTime.Now.Minute % 5 == 0)
                {
                    try
                    {
                        if(this.IsTcpConnected)
                             setAllEttuToReport();

                    }
                    catch (Exception ex1)
                    {
                        ConsoleServer.WriteLine(ex1.Message); 
                    }
                }


                try
                {
                    if(this.IsTcpConnected)
                            getSysClock();
                }
                catch(Exception ex1)
                { ConsoleServer.WriteLine(ex1.Message+","+ex1.StackTrace);}
                if (this.IsTcpConnected)
                {


                   

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
                ConsoleServer.WriteLine(this.DeviceName+","+ex.Message + ex.StackTrace);
                ;
            }
        }

       public override int TC_SetDateTime(int year, int mon, int day, int hour, int min, int sec)
       {
           //return base.TC_SetDateTime(year, mon, day, hour, min, sec);
           try
           {
               System.Data.DataSet ds;
               ds = m_protocol.GetSendDataSet("set_sys_clock");

               System.Data.DataRow r = ds.Tables[0].Rows[0];
               r["m1"] = string.Format("{0:00}", mon)[0];
               r["m2"] = string.Format("{0:00}", mon)[1];
               r["d1"] = string.Format("{0:00}", day)[0];
               r["d2"] = string.Format("{0:00}", day)[1];
               r["y1"] = string.Format("{0:0000}", year)[0];
               r["y2"] = string.Format("{0:0000}", year)[1];
               r["y3"] = string.Format("{0:0000}", year)[2];
               r["y4"] = string.Format("{0:0000}", year)[3];
               r["h1"] = string.Format("{0:00}", hour)[0];
               r["h2"] = string.Format("{0:00}", hour)[1];
               r["i1"] = string.Format("{0:00}", min)[0];
               r["i2"] = string.Format("{0:00}", min)[1];
               r["s1"] = string.Format("{0:00}", sec)[0];
               r["s2"] = string.Format("{0:00}", sec)[1];



               SendPackage[] pkgs = m_protocol.GetETTU_SendPackage(ds, 0xffff);

               Send(pkgs);
               if (pkgs[pkgs.Length - 1].result != CmdResult.ACK)
                   throw new Exception(pkgs[pkgs.Length - 1].result.ToString()+pkgs[pkgs.Length - 1].ToString());

               //    ConsoleServer.WriteLine("Set time ok!");

               return 0;
           }
           catch (Exception ex)
           {
               throw new Exception(this.DeviceName + "," + ex.Message );
           }
       }


       public void setAllEttuToReport()
       {
           System.Data.DataSet ds;
           ds = this.m_protocol.GetSendDataSet("get_all_phone_breakdown_status");
           SendPackage[] pkgs = m_protocol.GetETTU_SendPackage(ds, 0xffff);
           this.Send(pkgs);
       }


       void getSysClock()
       {
           System.Data.DataSet ds;
           ds = this.m_protocol.GetSendDataSet("get_sys_clock");
           SendPackage[] pkgs = m_protocol.GetETTU_SendPackage(ds, 0xffff);
           this.Send(pkgs);
       }

        public override void OneHourTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //base.OneHourTimer_Elapsed(sender, e);
            try
            {
                System.DateTime dt = System.DateTime.Now;
                if (this.IsTcpConnected)
                {
                   
                    ConsoleServer.WriteLine(this.DeviceName + "secdiff:" + this.TC_SetDateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
                }

               
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                ;
            }
           
        }

        public override void SetTransmitCycle(int cycle)
        {
            //base.SetTransmitCycle(cycle);

        }
        public override void TC_SendCycleSettingData()
        {
            //base.TC_SendCycleSettingData();
        }
        
        public void Send(SendPackage[] pkgs)
        {
            this.m_device.Send(pkgs);
        }
       

    }
}
