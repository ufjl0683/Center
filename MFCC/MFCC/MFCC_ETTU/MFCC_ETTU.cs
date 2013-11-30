using System;
using System.Collections.Generic;
using System.Text;
using Comm;
using System.Data;
using RemoteInterface;

namespace MFCC_ETTU
{
    public class MFCC_ETTU : Comm.MFCC.MFCC_DataColloetBase
    {
        System.Collections.Hashtable hsEtStatus=System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());

         public MFCC_ETTU(string mfccid, string devType, int remotePort, int notifyPort, int consolePort, string regRemoteName, Type regRemoteType)
          : base(mfccid, devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
          {

              loadETStatus();
          }


        void loadETStatus()
        {
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select deviceName,telcode,f1,f2,f3,x1,x2 from tblETConfig");
            cmd.Connection = cn;
            try
            {
                cn.Open();
                System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    string devicename;
                    string telcode;
                    byte[]f=new byte[3];
                    byte[] x = new byte[2];
                    devicename = rd[0].ToString();
                   // telcode = rd[1].ToString();
                    f[0] = System.Convert.ToByte(rd[2]);
                    f[1] = System.Convert.ToByte(rd[3]);
                    f[2] = System.Convert.ToByte(rd[4]);
                    x[0] = System.Convert.ToByte(rd[5]);
                    x[1] = System.Convert.ToByte(rd[6]);
                    try
                    {
                        hsEtStatus.Add(devicename, new ETStatus(devicename,  f, x));
                    }
                    catch (Exception ex1)
                    {
                        ConsoleServer.WriteLine(ex1.Message + "," + ex1.StackTrace);
                    }

                }

            }
            catch (Exception ex)
            {
                cn.Close();
            }

        }


        public override void BindEvent(object tc)
        {
            //  throw new Exception("The method or operation is not implemented.");
            ((Comm.TCBase)tc).OnTCReport += new OnTCReportHandler(MFCC_ETTU_OnTCReport);
        }

        void MFCC_ETTU_OnTCReport(object tc, TextPackage txt)
        {


            ConsoleServer.WriteLine(txt.ToString());
            try
            {
                if (txt.Text[0] == 0x29 && txt.Text[1] == 0x17 || txt.Text[0] == 0x25 && txt.Text[1] == 0x25 || txt.Text[0]==0x29 && txt.Text[1]==0x08 )
                {

                    DataSet ds = protocol.GetETTU_SendDsByReportTextPackage(txt);
                    System.Data.DataRow r = ds.Tables[0].Rows[0];
                    string devicename = "ET_";
                    for (int i = 1; i <= 9; i++)
                        if(txt.Text[0]==0x29 && txt.Text[1]==0x08)
                            devicename = devicename + System.Convert.ToChar(r["c" + i]).ToString();
                        else
                            devicename = devicename + System.Convert.ToChar(r["co" + i]).ToString();

                    string year = "";

                    for (int i = 1; i <= 4; i++)
                        year += System.Convert.ToChar(r["y" + i]).ToString();

                    string mon = "";

                    for (int i = 1; i <= 2; i++)
                        mon += System.Convert.ToChar(r["m" + i]).ToString();
                    string day = "";

                    for (int i = 1; i <= 2; i++)
                        day += System.Convert.ToChar(r["d" + i]).ToString();


                    string hour = "";

                    for (int i = 1; i <= 2; i++)
                        hour += System.Convert.ToChar(r["h" + i]).ToString();

                    string min = "";

                    for (int i = 1; i <= 2; i++)
                        min += System.Convert.ToChar(r["i" + i]).ToString();


                    string sec = "";

                    for (int i = 1; i <= 2; i++)
                        sec += System.Convert.ToChar(r["s" + i]).ToString();

                    DateTime dt = new DateTime(System.Convert.ToInt32(year), System.Convert.ToInt32(mon), System.Convert.ToInt32(day), System.Convert.ToInt32(hour), System.Convert.ToInt32(min), System.Convert.ToInt32(sec));

                    byte[] f = new byte[3];
                    for (int i = 1; i <= 3; i++)
                        f[i - 1] = System.Convert.ToByte(r["f" + i]);

                    if (!this.hsEtStatus.Contains(devicename))
                        ConsoleServer.WriteLine(devicename + "not exist in et config table");
                    else
                        ((ETStatus)hsEtStatus[devicename]).SetEttuFail(dt, f);

                    if (txt.Text[0] == 0x25 && txt.Text[1] == 0x25 ||   txt.Text[0]==0x29 && txt.Text[1]==0x08)
                    {
                        string sql="insert into tblETTestLog (DeviceName,TimeStamp,F1,F2,F3,Telcode) values('{0}','{1}',{2},{3},{4},'{5}')";

                        try
                        {
                            dbServer.SendSqlCmd(string.Format(sql, devicename, DbCmdServer.getTimeStampString(dt), f[0], f[1], f[2], devicename.Split(new char[] { '_' })[1]));
                            ds.AcceptChanges();
                            this.notifier.NotifyAll(new NotifyEventObject(EventEnumType.ETTU_other_report_Event, ((TCBase)tc).DeviceName, ds));
                        }
                        catch (Exception ex)
                        {
                            ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                        }

                    }


                }

                else if (txt.Text[0] == 0x42 && txt.Text[1] == 0x01) //話機拿起主動回報
                {
                    char[] chr_etnums = new char[10];

                    for (int i = 2; i < 2 + 10; i++)
                    {
                        chr_etnums[i - 2] = (char)txt.Text[i];

                    }

                    string sql="insert into tblETStateLog (devicename,timestamp,type,telcode) values('{0}','{1}','U','{2}')"; 
                    string etnum = new string(chr_etnums);
                    dbServer.SendSqlCmd(string.Format(sql,((TCBase)tc).DeviceName,DbCmdServer.getTimeStampString(System.DateTime.Now),etnum));
                    try
                    {
                        if(r_host_comm!=null)
                             r_host_comm.SetETTUCCTVLock("ET_" + etnum.Trim());
                    }
                    catch { ;}
                    this.notifier.NotifyAll(new NotifyEventObject(EventEnumType.ETTU_CC_Report_Event, ((TCBase)tc).DeviceName, etnum));
                    ConsoleServer.WriteLine(etnum + " 話機拿起!");
                }
                else if (txt.Text[0] == 0x28 && txt.Text[1] == 0x28   )
                {


                    string sql = "insert into tblETTestLog (TimeStamp,F1,F2,F3,Telcode) values('{0}',{1},{2,{3},'{4}')";

                    try
                    {
                        dbServer.SendSqlCmd(string.Format(sql, DbCmdServer.getTimeStampString(DateTime.Now), 0,0,0, "999999999"));
                    }
                    catch (Exception ex)
                    {
                        ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                    }
                }

                else
                {
                    DataSet ds = protocol.GetETTU_SendDsByReportTextPackage(txt);
                    if (ds != null)
                    {
                        ds.AcceptChanges();
                        this.notifier.NotifyAll(new NotifyEventObject(EventEnumType.ETTU_other_report_Event, ((TCBase)tc).DeviceName, ds));
                    }

                }


            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }
            //throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataSet SendTC(string tcname, System.Data.DataSet ds)
        {
            Comm.TC.ETTUTC tc = getTcManager()[tcname]  as Comm.TC.ETTUTC;

            if (!tc.IsConnected) throw new Exception("tc 未連線!");
            SendPackage[] pkgs = protocol.GetETTU_SendPackage(ds, tc.DeviceID);
         
            tc.Send(pkgs);

            //if (pkgs[pkgs.Length - 1].result != CmdResult.ACK && pkgs.Length != 0)
            //{
            //    MessageBox.Show(pkgs[pkgs.Length - 1].result.ToString());
            //    IsAck = false;
            //}

            if (pkgs[pkgs.Length - 1].result == CmdResult.ACK)
            {
                if (pkgs[pkgs.Length - 1].type == CmdType.CmdSet)
                    return null;
                else
                {
                    DataSet retds = protocol.GetETTU_ReturnDsByTextPackage(pkgs[pkgs.Length - 1].ReturnETTUTextPackage);
                    retds.AcceptChanges();
                    //  ConsoleServer.WriteLine("Pkg Receive :"+pkg.ReturnTextPackage.ToString());
                    return retds;
                }
            }
            else
            {
                throw new Exception(tcname + pkgs[pkgs.Length - 1].result.ToString());
            }
        }

    }
}
