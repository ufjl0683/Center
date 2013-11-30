using System;
using System.Collections.Generic;
using System.Text;
using Comm;
using RemoteInterface;
using Comm.MFCC;



namespace MFCC_RGS
{
    class MFCC_RGS:Comm.MFCC.MFCC_Base
    {
       //  RGS_Manager rgs_manager;
        public MFCC_RGS(string mfccid,string devType, int remotePort, int notifyPort, int consolePort, string regRemoteName, Type regRemoteType)
            : base(mfccid,devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
        {
           // new System.Threading.Thread(RGS_Test_Task).Start();   
        }



      

        public void RGS_Test_Task()
        {
            Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.manager["RGS-N3-N-198.182"];
            // Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.manager["RGS232"];
            tc.m_device.OnAck += new OnAckEventHandler(m_device_OnAck);
            tc.m_device.OnSendingPackage += new OnSendPackgaeHandler(m_device_OnSendingPackage);
            tc.m_device.OnReceiveText += new OnTextPackageEventHandler(m_device_OnReceiveText);
            while (true)
            {

                System.Drawing.Color[][] color;
                color = new System.Drawing.Color[1][];
                color[0] = new System.Drawing.Color[4] { System.Drawing.Color.Red, System.Drawing.Color.Red, System.Drawing.Color.Red, System.Drawing.Color.Red };
                try
                {
                    if (tc.IsConnected)
                        tc.SetTravelDisplay(new byte[0], new string[] { "系統測試" }, color);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message+","+ex.StackTrace);
                }
            }

        }

        void m_device_OnReceiveText(object sender, TextPackage txtObj)
        {
            // Console.WriteLine(txtObj.ToString());
            //throw new Exception("The method or operation is not implemented.");
        }

        void m_device_OnSendingPackage(object sender, SendPackage pkg)
        {
            Console.WriteLine("===>" + pkg.ToString());
            //throw new Exception("The method or operation is not implemented.");
        }

        void m_device_OnAck(object sender, AckPackage AckObj)
        {
            //throw new Exception("The method or operation is not implemented.");
            Console.WriteLine(AckObj);
        }

        public override void BindEvent(object tc)
        {
           // throw new Exception("The method or operation is not implemented.");
        }
        public override void loadTC_AndBuildManaer()
        {

            base.loadTC_AndBuildManaer();
            //  //  throw new Exception("The method or operation is not implemented.");

            //    try
            //    {
            //        System.Data.Odbc.OdbcDataReader rd;
            //        System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);


            //        rd = Comm.DB2.Db2.getDeviceConfigReader(cn, this.mfccid);


            //        while (rd.Read())
            //        {
            //            byte[] hw_status = new byte[4];
            //            for (int i = 0; i < 4; i++)
            //                hw_status[i] = System.Convert.ToByte(rd[3 + i]);

            //            Comm.TC.RGSTC tc = new Comm.TC.RGSTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status);
            //            ConsoleServer.WriteLine(string.Format("load tc:{0} ip:{1} port:{2}", rd[0], rd[1], rd[2]));
            //            tcAry.Add(tc);
            //            //tc.OnRealTimeData += new Comm.TC.OnRealTimeEventHandler(tc_OnRealTimeData);
            //            //tc.OnTriggerEvent += new Comm.TC.OnTriggerEventHandler(tc_OnTriggerEvent);
            //            tc.OnHwStatusChanged += new HWStatusChangeHandler(tc_OnHwStatusChanged);
            //            tc.OnConnectStatusChanged += new ConnectStatusChangeHandler(tc_OnConnectStatusChanged);
            //           // tc.On1MinTrafficData += new Comm.TC.On1MinEventDataHandler(tc_On1MinTrafficData);
            //        }
            //        rd.Close();
            //        cn.Close();


            //        rgs_manager = new RGS_Manager(tcAry);

            //    }
            //    catch (Exception ex)
            //    {
            //        ConsoleServer.WriteLine(ex.Message);
            //    }

            //this.tcAry.Add(new Comm.TC.RGSTC(protocol, "RGS-N1-S-203.79", "192.168.26.55", 4660, 0xffff, new byte[] { 0, 0, 0, 0 }));
            //this.manager = new TC_Manager(tcAry);
        }

        //public override Comm.MFCC.TC_Manager getTcManager()
        //{
        //   // throw new Exception("The method or operation is not implemented.");
        //    return rgs_manager;
        //}
    }
}
