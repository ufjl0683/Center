using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HWStatus;

namespace Comm.TC
{
  public   class RDTC : TCBase
    {
      

      private int curr_amount=-1, curr_acc_amount=-1, curr_degree=-1;
      public DateTime last_receive_time;

      public delegate void RDFiveMinDataHandler(Comm.TC.RDTC tcc,DateTime dt, int amount, int acc_amount, int degree,int datatype);
      public delegate void RDDegreeChangeHandler(Comm.TC.RDTC tcc,int data_type,DateTime dt,  int pluviometric, int degree);
       public event RDFiveMinDataHandler On_RD_FivceMinData;
      public event RDDegreeChangeHandler On_DegreeChange;
      public RDTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus, byte comm_state)
           : base(protocol, devicename, ip, port, deviceid, hw_status,opmode,opstatus, comm_state)
          {

             m_hw_cycle = 0; //on change
            m_trans_cycle = 5; //1:1min,5:5min
            m_trans_mode = 1; //0:polling,1:active



              this.OnTCReport += new OnTCReportHandler(RDTC_OnTCReport);
              this.OnConnectStatusChanged += new ConnectStatusChangeHandler(RDTC_OnConnectStatusChanged);
          }

      
       


        public override void OneMinTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) // 1 min 週期任務
        {
            base.OneMinTimer_Elapsed(sender, e);
            if (!this.IsTcpConnected) return;

            try
            {
                TC_SendCycleSettingData();
            }
            catch { ;}

            //   Util.GC();


        }
        

        void RDTC_OnConnectStatusChanged(object tc)
        {
           // throw new Exception("The method or operation is not implemented.");
            try
            {
                if (((TCBase)tc).IsConnected)
                {
                    this.TC_SendCycleSettingData();
                    ConsoleServer.WriteLine(((TCBase)tc).DeviceName + "  set cycle settting!");
                    System.DateTime dt = System.DateTime.Now.AddMinutes(-5);
                    byte[] data = new byte[] { 0x4d, (byte)dt.Day, (byte)dt.Hour, (byte)(dt.Minute / 5 * 5) };
                    Comm.SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.B, 0xffff, data);
                    this.Send(pkg);

                    this.curr_amount = (pkg.ReturnTextPackage.Text[8] == 255) ? -1 : pkg.ReturnTextPackage.Text[8];
                    this.curr_acc_amount = (pkg.ReturnTextPackage.Text[9] * 256 + pkg.ReturnTextPackage.Text[10] == 65535) ? -1 : pkg.ReturnTextPackage.Text[9] * 256 + pkg.ReturnTextPackage.Text[10];
                    this.curr_degree = (pkg.ReturnTextPackage.Text[11]==255)?-1:pkg.ReturnTextPackage.Text[11];
                    last_receive_time = dt;
                    
                }  
               // System.Data.DataSet ds = this.m_protocol.GetETTU_ReturnDsByTextPackage(pkg.ReturnTextPackage);
            }
            catch(Exception ex) { 
             ConsoleServer.WriteLine(ex.Message+ex.StackTrace)   ;}
        }

        void RDTC_OnTCReport(object tc, TextPackage txt)
        {
           // throw new Exception("The method or operation is not implemented.");
            try{
                if (txt.Text[0] == 0x48   )   // cycle data  主動回報
                {

                    if (txt.Text.Length != 13)
                    {
                        ConsoleServer.WriteLine(this.DeviceName + "," + txt.ToString() + ",長度不符");
                        return;
                    }



                    System.DateTime dt = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, txt.Text[6], txt.Text[7], txt.Text[8], 0);

                    curr_amount = (txt.Text[9] == 255) ? -1 : txt.Text[9];
                    curr_acc_amount = (txt.Text[10] * 256 + txt.Text[11] == 65535) ? -1 : txt.Text[10] * 256 + txt.Text[11];
                    curr_degree = (txt.Text[12] == 255) ? -1 : txt.Text[12];
                    last_receive_time = dt;

                    if (On_RD_FivceMinData != null)
                        this.On_RD_FivceMinData(this, dt, curr_amount, curr_acc_amount, curr_degree,(txt.Text[1]==0)?1:0);
                }
                else  if(txt.Text[0]==0x4a)
                {
                    System.DateTime dt = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, txt.Text[6], txt.Text[7], txt.Text[8], 0);
                    if(this.On_DegreeChange!=null)
                        On_DegreeChange(this, txt.Text[1],dt, txt.Text[9], txt.Text[10]);

                    

                }

               } 

               
             catch(Exception ex)
                 {
                    ConsoleServer.WriteLine(this.DeviceName+","+ex.Message);
                 }
           //  this.dbServer.SendSqlCmd(string.Format(sql,  txt.Text[9], txt.Text[10] * 256 + txt.Text[11], txt.Text[12], tc.DeviceName, Comm.DB2.Db2.getTimeStampString(dt)));
          
         
        }

          public void getCurrentRDData(ref DateTime dt, ref int amount, ref int acc_amount, ref int degree)
          {
              dt = last_receive_time;
              amount = curr_amount;
              acc_amount = curr_acc_amount;
              degree = curr_degree;
          }

        public override void DownLoadConfig()
        {
           // throw new Exception("The method or operation is not implemented.");
        }

        
        public override RemoteInterface.I_HW_Status_Desc getStatusDesc()
        {
            return new RD_HW_StatusDesc(this.DeviceName, this.m_hwstaus);
            //throw new Exception("The method or operation is not implemented.");
        }

    }
}
