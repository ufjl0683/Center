using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_WD
{
    public class MFCC_WD : Comm.MFCC.MFCC_DataColloetBase
    {
      //  System.Collections.Hashtable hsRangeTable=System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
        int[,] RangeTable = new int[2, 2]  {{0,50},{0,50}};
        
         public  MFCC_WD(string mfccid,string devType,int remotePort,int notifyPort,int consolePort,string regRemoteName,Type regRemoteType)
            :base( mfccid,devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
        {


            loadRangeTable();
            new System.Threading.Thread(DataRepairTask).Start();
            
          
        }

      

       public    void loadRangeTable()
        {
           //type=A  avg wnd spd   ,I immediate wns speed
            ConsoleServer.WriteLine("Loaad Range Table");
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select type,minvalue,maxvalue from tblWDvalidRange");
            try
            {
                cn.Open();
                cmd.Connection = cn;
                System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    if (rd[0].ToString().Trim() == "A")
                    {
                        RangeTable[0, 0] = System.Convert.ToInt32(rd[1]);
                        RangeTable[0, 1] = System.Convert.ToInt32(rd[2]);
                    }
                    else
                    {

                        RangeTable[1, 0] = System.Convert.ToInt32(rd[1]);
                        RangeTable[1, 1] = System.Convert.ToInt32(rd[2]);
                    }
                }

            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }
            finally
            {
                cn.Close();
            }

        }

         bool IsValidData(int avgspd,int maxspd)
         {
             bool result = true;

             result = result && (avgspd >= RangeTable[0,0] && avgspd < RangeTable[0,1]);
             result = result && (maxspd >= RangeTable[1,0] && maxspd <= RangeTable[1,1]);
             return result;
         }

         public override void BindEvent(object tc)
         {
            
             ((Comm.TC.WDTC)tc).On_WD_CycleData += new Comm.TC.WDTC.WDCycleDataHandler(MFCC_WD_On_WD_TenMinData);
             ((Comm.TC.WDTC)tc).On_DegreeChange += new Comm.TC.WDTC.WDDegreeChangeHandler(MFCC_WD_On_DegreeChange);
             
         }

        void MFCC_WD_On_DegreeChange(Comm.TC.WDTC tcc,int datatype ,DateTime dt, int average_wind_speed, int average_wind_direction, int max_wind_speed, int max_wind_direction, int degree)
        {
            try
            {
                string sql = "insert into tblEventWD (devicename,timestamp,datatype,Average_Speed,Average_Direction,Max_Speed,Max_Direction,Degree) values('{0}','{1}',{2},{3},{4},{5},{6},{7})";

                dbServer.SendSqlCmd(string.Format(sql, tcc.DeviceName, DbCmdServer.getTimeStampString(DateTime.Now),
                    datatype, average_wind_speed, average_wind_direction, max_wind_speed, max_wind_direction, degree));
                if (r_host_comm != null)
                     this.r_host_comm.setWDEventData(tcc.DeviceName, dt,average_wind_speed,average_wind_direction,max_wind_speed,max_wind_direction, degree);

                ConsoleServer.WriteLine(tcc.DeviceName + "," + degree + "事件!");
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine("Host err:" + ex.Message);
            }
           
        }


        void MFCC_WD_On_WD_TenMinData(Comm.TC.WDTC tc, DateTime dt, int average_wind_speed, int average_wind_direction,int max_wind_speed,int max_wind_direction,int am_degree,int datatype) //datatype 1:real 0 simula
        {
           
            string sql = "update tblWDData10Min set DataValidity='{7}',DataType={8},average_wind_speed={0},average_wind_direction={1}  , max_wind_speed={2} ,max_wind_direction={3} ,am_degree={4} where devicename='{5}' and timestamp='{6}' ";
            this.dbServer.SendSqlCmd(string.Format(sql, average_wind_speed,  average_wind_direction,max_wind_speed,max_wind_direction,am_degree, tc.DeviceName, Comm.DB2.Db2.getTimeStampString(dt),
                IsValidData(average_wind_speed, max_wind_speed)?"V":"I",datatype));


            bool isValid = average_wind_speed == -1;

            try
            {
                RemoteInterface.ConsoleServer.WriteLine(dt+","+tc.DeviceName+","+"AvgWndSpd:"+average_wind_speed+","+"AvgWndDir:"+average_wind_direction+",MaxWndSpd:"+max_wind_speed+",MaxWndDir:"+max_wind_direction+",Degree:"+am_degree);
                this.r_host_comm.setWDTenMinData(tc.DeviceName, dt, average_wind_speed, average_wind_direction, max_wind_speed, max_wind_direction, am_degree);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine("Host err:" + ex.Message);
            }
            try
            {
                this.notifier.NotifyAll(new NotifyEventObject(EventEnumType.WD_10min_data_Event, tc.DeviceName, new RemoteInterface.MFCC.WD_10Min_Data(tc.DeviceName, dt, average_wind_speed, average_wind_direction, max_wind_speed, max_wind_direction, am_degree)));
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine("Notifier:" + ex.Message);
            }
        }

        void DataRepairTask()
        {


            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();

            cmd.Connection = cn;
            string devName = "";
            DateTime dt = new DateTime();
            while (true)
            {
                Comm.TC.WDTC tc;

                if (!IsLoadTcCompleted)
                {
                    System.Threading.Thread.Sleep(1000);
                    continue;
                }

                try
                {

                    cn.Open();
                    string sqlGetRepair = "select *  from (select t1.DEVICENAME, TIMESTAMP ,trycnt,datavalidity,comm_state from TBLWDDATA10MIN  t1 inner join tblDeviceConfig t2 on t1.devicename=t2.devicename where mfccid='{0}' and  TIMESTAMP between '{1}' and '{2}' and trycnt <1 and datavalidity='N' and comm_state=1  and enable='Y' fetch first 300  row only) order by trycnt,timestamp desc ";

                    cmd.CommandText = string.Format(sqlGetRepair, mfccid, Comm.DB2.Db2.getTimeStampString(System.DateTime.Now.AddDays(-7)), Comm.DB2.Db2.getTimeStampString(System.DateTime.Now.AddMinutes(-10)));
                    ConsoleServer.WriteLine("Repair sql check!");
                    System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                    ConsoleServer.WriteLine("Repair task beg!");
                    while (rd.Read())
                    {
                        try
                        {
                            devName = rd[0] as string;
                            dt = System.Convert.ToDateTime(rd[1]);

                            if (Program.mfcc_wd.manager.IsContains(devName))
                                tc = (Comm.TC.WDTC)Program.mfcc_wd.manager[devName];
                            else
                                continue;

                            if (!tc.IsConnected) continue;



                            System.Data.DataSet ds = this.protocol.GetSendDataSet("get_unread_data");

                            ds.Tables[0].Rows[0]["day"] = dt.Day;
                            ds.Tables[0].Rows[0]["hour"] = dt.Hour;
                            ds.Tables[0].Rows[0]["minute"] = dt.Minute;
                            Comm.SendPackage pkg = this.protocol.GetSendPackage(ds, 0xffff);

                            tc.Send(pkg);

                            if (pkg.ReturnTextPackage == null && pkg.result== Comm.CmdResult.ACK)
                                throw new Exception(tc.DeviceName + "," + dt + ",回補資料失敗!");

                            if (pkg.ReturnTextPackage.Text.Length != 13)
                                throw new Exception("回補資料長度錯誤");

                            ds = protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);

                            string sql = string.Format("update tblWDData10Min set DataValidity='V',DataType=1,average_wind_speed={0},average_wind_direction={1},max_wind_speed={2},max_wind_direction={3},am_degree={4} where devicename='{5}' and timestamp='{6}' ",
                                ds.Tables[0].Rows[0]["average_wind_speed"], ds.Tables[0].Rows[0]["average_wind_direction"], ds.Tables[0].Rows[0]["max_wind_speed"], ds.Tables[0].Rows[0]["max_wind_direction"], ds.Tables[0].Rows[0]["am_degree"], tc.DeviceName, Comm.DB2.Db2.getTimeStampString(dt));


                           
                            dbServer.SendSqlCmd(sql);

                            ConsoleServer.WriteLine("==>repair:" + devName + "," + dt.ToString());

                        }
                        catch (Exception ex)
                        {
                            ConsoleServer.WriteLine(devName + "," + ex.Message + ex.StackTrace);
                            try
                            {

                                dbServer.SendSqlCmd(string.Format("update TBLWDDATA10MIN set trycnt=trycnt+1 where devicename='{0}' and  timestamp='{1}'", devName, Comm.DB2.Db2.getTimeStampString(dt)));

                            }
                            catch (Exception ee)
                            {
                                ConsoleServer.WriteLine(ee.Message + ee.StackTrace);
                            }
                        }
                    }
                    rd.Close();

                    System.Threading.Thread.Sleep(1000 * 60);
                }
                catch (Exception x)
                {
                    ConsoleServer.WriteLine(x.Message + x.StackTrace);
                }
                finally
                {
                    cn.Close();
                }


            }  //while


        }
    }
}
