using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
namespace MFCC_RMS
{
    class MFCC_RMS:Comm.MFCC.MFCC_Base
    {
        RMS_Manager RMS_manager;
        public MFCC_RMS(string mfccid,string devType, int remotePort, int notifyPort, int consolePort, string regRemoteName, Type regRemoteTyp)
            : base(mfccid,devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteTyp)
        {

        
        }

       
        //public override void loadTC_AndBuildManaer()
        //{

        //    this.tcAry.Add(new Comm.TC.RMSTC(protocol, "rms2", "192.168.0.2", 1002, 0xffff, new byte[] { 0, 0, 0, 0 }));
        //    RMS_manager=new RMS_Manager(tcAry);
        //   // throw new Exception("The method or operation is not implemented.");
        //    //this.tcAry.Add(new 
        //}

        public override void BindEvent(object tc)
        {
            ((Comm.TCBase)tc).OnTCReport += new Comm.OnTCReportHandler(MFCC_RMS_OnTCReport);
            //throw new Exception("The method or operation is not implemented.");
        }

        

        void MFCC_RMS_OnTCReport(object tc, Comm.TextPackage txt)
        {
            //throw new Exception("The method or operation is not implemented.");

            Comm.TC.RMSTC dev = (Comm.TC.RMSTC)tc;
            if (txt.Text[0] == 0x86 || txt.Text[0] == 0xa7 || txt.Text[0] == 0xae || txt.Text[0] == 0xaf && txt.Text[1] == 0x01)
            {
               // ConsoleServer.WriteLine("In Rms Report:0x" + Util.ToHexString(txt.Text[0]));
                System.Data.DataSet ds = this.protocol.GetSendDsByTextPackage(txt, Comm.CmdType.CmdReport);
                ds.AcceptChanges();
                this.notifier.NotifyAll(new NotifyEventObject(EventEnumType.MFCC_Report_Event,dev.DeviceName,ds));

                string sql="";
                switch (txt.Text[0])
                {
                    case 0x86:  //黃閃轉態紀錄
                      //  ConsoleServer.WriteLine("0x86 " + txt.Text[1] +"curr_lamp:"+dev.curr_lamep);
                        if (dev.curr_lamep != txt.Text[1] && dev.curr_lamep == 0x05)
                        {
                            sql = "insert into tblRmsActiveReport (devicename,timestamp,protocol,display) values('{0}','{1}','{2}','{3}')";
                            try
                            {
                                ConsoleServer.WriteLine(dev.DeviceName + ", 黃閃轉態!");
                                ConsoleServer.WriteLine(string.Format(sql, dev.DeviceName, DbCmdServer.getTimeStampString(DateTime.Now), "86", "flash Yellow to other state"));
                                this.dbServer.SendSqlCmd(string.Format(sql, dev.DeviceName, DbCmdServer.getTimeStampString(DateTime.Now), "86", "flash Yellow to other state"));
                                
                            }
                            catch (Exception ex)
                            {
                                ConsoleServer.WriteLine(ex.Message+","+ex.StackTrace);
                            }
                        }
                        dev.curr_lamep = txt.Text[1];
                        break;

                    case 0xa7:

                        sql = "insert into tblRmsActiveReport (devicename,timestamp,protocol,display) values('{0}','{1}','{2}','{3}')";

                        string  display = string.Format("plan_no:{0},warnsetno:{1},", ds.Tables[0].Rows[0]["plan_no"], ds.Tables[0].Rows[0]["warnsetno"]);
                        ConsoleServer.WriteLine("*****A7****" + display);
                        for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                        {
                            for (int j = 0; j < ds.Tables[1].Columns.Count; j++)
                            {
                                display += ds.Tables[1].Columns[j].ColumnName + i + ":" + ds.Tables[1].Rows[i][j] + ",";
                            }
                        }

                        ConsoleServer.WriteLine("*****A7****"+display);
                        this.dbServer.SendSqlCmd(string.Format(sql, dev.DeviceName, DbCmdServer.getTimeStampString(DateTime.Now), "A7", display.Trim(new char[] { ',' })));


                        break;
                    case 0xae:
                         
           
                    //bos_id(1:8-15)state(1:0"0_close",1"1_open",2"2_flash")msg_id(1:0-255)
                    int bos_id, state, msg_id;
                    bos_id = txt.Text[1];
                    state = txt.Text[2];
                    msg_id = txt.Text[3];
                    byte[] msgbyte = new byte[32];
                    System.Array.Copy(txt.Text, 4, msgbyte, 0, 32);
                    for (int i = 0; i < msgbyte.Length; i++)
                        if (msgbyte[i] == 0x00)
                            msgbyte[i] = 0x20;
                    string message = Util.Big5BytesToString(msgbyte);
                     display = string.Format("bos_id:{0}, state:{1}, msg_id:{2}, messgae:{3}", bos_id, state, msg_id, message.Trim());
                     sql = "insert into tblRmsActiveReport (devicename,timestamp,protocol,display) values('{0}','{1}','{2}','{3}')";
                    this.dbServer.SendSqlCmd(string.Format(sql, dev.DeviceName, DbCmdServer.getTimeStampString(DateTime.Now), "AE", display));

                    break;
           
                   
                }
            }
            else if (txt.Text[0] == 0xa6)  // 現場操作燈號改變
            {
                string sql = "insert into tblRmsActiveReport (devicename,timestamp,protocol,display) values('{0}','{1}','{2}','{3}')";
                dbServer.SendSqlCmd(string.Format(sql, dev.DeviceName, DbCmdServer.getTimeStampString(DateTime.Now), "A6", txt.Text[5]));
              //  ConsoleServer.WriteLine(dev.DeviceName + "," + (txt.Text[5] == 0 ? "儀控中止" : "儀控啟動"));
            }
            else if (txt.Text[0] == 0xaa)   //儀控啟動結束
            {


                string sql = "insert into tblRmsActiveReport (devicename,timestamp,protocol,display) values('{0}','{1}','{2}','{3}')";
                this.dbServer.SendSqlCmd(string.Format(sql, dev.DeviceName, DbCmdServer.getTimeStampString(DateTime.Now), "AA", txt.Text[5]));
                dev.isDisplayOff=(txt.Text[5] == 0)?true:false;
                ConsoleServer.WriteLine(dev.DeviceName + "," + (txt.Text[5] == 0 ? "儀控中止" : "儀控啟動"));
            }
            else if (txt.Text[0] == 0xAF && txt.Text[1] == 0x02)
            {
                int year = txt.Text[6] * 256 + txt.Text[7];
                int month = txt.Text[8];
                int day = txt.Text[9];
                int hour = txt.Text[10];
                int min = txt.Text[11];
                uint lvd_flow = (uint)(txt.Text[12] * Math.Pow(256, 3) + txt.Text[13] * Math.Pow(256, 2) + txt.Text[14] * Math.Pow(256, 1) + txt.Text[15]);
                int l_no = txt.Text[16];
                int l_mode = txt.Text[17];
                System.DateTime dt = new DateTime(year, month, day, hour, min, 0);

                string display = string.Format("vd_flow:{0},l_no:{1},l_mode:{2}", lvd_flow, l_no, l_mode);

                string sql = "insert into tblRmsActiveReport (devicename,timestamp,protocol,display) values('{0}','{1}','{2}','{3}')";
                this.dbServer.SendSqlCmd(string.Format(sql, dev.DeviceName, DbCmdServer.getTimeStampString(dt), "AF02", display));

            }
            else if (txt.Text[0] == 0xAF && txt.Text[1] == 0x01)
            {
                // main_occ(1:0-100)main_flow(1:0-255)vdq_occ(1:0-100)vdq_flow(1:0-255)lvd_occ(1:0-255)lvd_flow(1:0-255)
                int year = txt.Text[6] * 256 + txt.Text[7];
                int month = txt.Text[8];
                int day = txt.Text[9];
                int hour = txt.Text[10];
                int min = txt.Text[11];
                int sec = txt.Text[12];
                uint main_occ = txt.Text[13];
                int main_flow = txt.Text[14];
                int vdq_occ = txt.Text[15];
                int vdq_flow = txt.Text[16];
                int lvd_occ = txt.Text[17];
                int lvd_flow = txt.Text[18];
                System.DateTime dt = new DateTime(year, month, day, hour, min,sec);

                string display = string.Format("main_occ:{0},main_flow:{1},vdq_occ:{2},vdq_flow:{3},lvd_occ:{4},lvd_flow:{5}", main_occ,main_flow,vdq_occ,vdq_flow,lvd_occ,main_flow);

                string sql = "insert into tblRmsActiveReport (devicename,timestamp,protocol,display) values('{0}','{1}','{2}','{3}')";
                this.dbServer.SendSqlCmd(string.Format(sql, dev.DeviceName, DbCmdServer.getTimeStampString(dt), "AF01", display));

            }
            else if (txt.Text[0] == 0xAF && txt.Text[1] == 0x03)
            {
              //  lvd_flow(0:0-255)state(1:0-1)
                int year = txt.Text[6] * 256 + txt.Text[7];
                int month = txt.Text[8];
                int day = txt.Text[9];
                int hour = txt.Text[10];
                int min = txt.Text[11];
                int lvd_flow = txt.Text[12];
                int state=txt.Text[13];
              
               
                System.DateTime dt = new DateTime(year, month, day, hour, min,0);

                string display = string.Format("lvd_flow:{0},state:{1}",lvd_flow,state);

                string sql = "insert into tblRmsActiveReport (devicename,timestamp,protocol,display) values('{0}','{1}','{2}','{3}')";
                this.dbServer.SendSqlCmd(string.Format(sql, dev.DeviceName, DbCmdServer.getTimeStampString(dt), "AF03", display));

            }
          

            

        }

        //public override Comm.MFCC.TC_Manager getTcManager()
        //{
        //    return RMS_manager;
        //   // throw new Exception("The method or operation is not implemented.");
        //}
    }
}
