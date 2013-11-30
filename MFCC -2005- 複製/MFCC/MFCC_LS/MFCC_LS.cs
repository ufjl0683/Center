using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using System.Data;

namespace MFCC_LS
{
    public class MFCC_LS : Comm.MFCC.MFCC_DataColloetBase
    {
      //  System.Collections.Hashtable hsRangeTable=System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
        int[,] RangeTable = new int[2, 2]  {{0,30000},{0,30000}};
        
         public  MFCC_LS(string mfccid,string devType,int remotePort,int notifyPort,int consolePort,string regRemoteName,Type regRemoteType)
            :base( mfccid,devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
        {


           // loadRangeTable();
            new System.Threading.Thread(DataRepairTask).Start();
            
          
        }

      

       public    void loadRangeTable()
        {
           //type=A  avg wnd spd   ,I immediate wns speed
            ConsoleServer.WriteLine("Loaad Range Table");
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select type,minvalue,maxvalue from tblLSvalidRange");
            try
            {
                cn.Open();
                cmd.Connection = cn;
                System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    if (rd[0].ToString().Trim() == "D")  //Day var
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

         bool IsValidData(int dayAmount,int MonAmount)
         {
             bool result = true;

             result = result && (dayAmount >= RangeTable[0, 0] && dayAmount < RangeTable[0, 1]);
             result = result && (MonAmount >= RangeTable[1, 0] && MonAmount <= RangeTable[1, 1]);
             return result;
         }

         public override void BindEvent(object tc)
         {
             //throw new Exception("The method or operation is not implemented.");

             //((Comm.TC.RDTC)tc).OnTCReport += new Comm.OnTCReportHandler(MFCC_RD_OnTCReport);
             ((Comm.TC.LSTC)tc).On_LS_CycleData += new Comm.TC.LSTC.LSCycleDataHandler(MFCC_LS_On_LS_TenMinData);

             ((Comm.TC.LSTC)tc).On_DegreeChange += new Comm.TC.LSTC.LSDegreeChangeHandler(MFCC_LS_On_DegreeChange);
         }

        void MFCC_LS_On_DegreeChange(Comm.TC.LSTC tc, DateTime dt, DataSet ds)
        {
            try
            {

                int day_var=System.Convert.ToInt32(ds.Tables[0].Rows[0]["day_var"]);
                int mon_var=System.Convert.ToInt32(ds.Tables[0].Rows[0]["mon_var"]);
                int degree=System.Convert.ToInt32(ds.Tables[0].Rows[0]["lsd_degree"]);
                int datatype=System.Convert.ToInt32(ds.Tables[0].Rows[0]["response_type"]);
                string sql = "insert into tblEventLS (DeviceName,TimeStamp,DataType,day_var,mon_var,Degree) values('{0}','{1}',{2},{3},{4},{5})";
                dbServer.SendSqlCmd(string.Format(sql, tc.DeviceName, DbCmdServer.getTimeStampString(DateTime.Now), datatype, day_var, mon_var, degree));
                this.r_host_comm.setLSEventData(tc.DeviceName, dt,mon_var,day_var,degree );
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine("Host err:" + ex.Message);
            }
        }


        void MFCC_LS_On_LS_TenMinData(Comm.TC.LSTC tc, DateTime dt, DataSet ds)
        {
        //   throw new Exception("The method or operation is not implemented.");

            try
            {
                int day_var, mon_var,datatype;

                //day=System.Convert.ToInt32(ds.Tables[0].Rows[0]["day"]);
                //hour = System.Convert.ToInt32(ds.Tables[0].Rows[0]["hour"]);
                //min = System.Convert.ToInt32(ds.Tables[0].Rows[0]["min"]);
                //System.DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day, hour, min, 0);

                day_var = System.Convert.ToInt32(ds.Tables[0].Rows[0]["day_var"]);
                mon_var = System.Convert.ToInt32(ds.Tables[0].Rows[0]["mon_var"]);
                datatype = System.Convert.ToInt32(ds.Tables[0].Rows[0]["response_type"])==0?1:0;
                string validChar = IsValidData(day_var, mon_var) ? "V" : "I";
                string sql = "update tblLsData1Hr set day_var={0},mon_var={1},DataValidity='{4}',datatype={5} where DeviceName='{2}' and timestamp='{3}'";
                Program.mfcc_ls.dbServer.SendSqlCmd(string.Format(sql,day_var,mon_var,tc.DeviceName,Comm.DB2.Db2.getTimeStampString(dt),validChar));

                for (int i = 0; i < System.Convert.ToInt32(ds.Tables[0].Rows[0]["ai_cnt"]); i++)
                {
                    sql = "insert into tblLsData1HrDetail (devicename,timestamp,ai_cnt,sensor_var,acc_var) values ('{0}','{1}',{2},{3},{4}) ";
                    Program.mfcc_ls.dbServer.SendSqlCmd(string.Format(sql,tc.DeviceName,Comm.DB2.Db2.getTimeStampString(dt),i
                        , System.Convert.ToInt32(ds.Tables[0].Rows[0]["ai_cnt"]), System.Convert.ToInt32(ds.Tables["tblai_cnt"].Rows[i]["sensor_var"]), System.Convert.ToInt32(ds.Tables["tblai_cnt"].Rows[i]["acc_var"])));
                        
                }

            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }


            //string sql = "update tblLSData10Min set DataValidity='{7}',DataType=1,average_wind_speed={0},average_wind_direction={1}  , max_wind_speed={2} ,max_wind_direction={3} ,am_degree={4} where devicename='{5}' and timestamp='{6}' ";
            //this.dbServer.SendSqlCmd(string.Format(sql, average_wind_speed,  average_wind_direction,max_wind_speed,max_wind_direction,am_degree, tc.DeviceName, Comm.DB2.Db2.getTimeStampString(dt),
            //    IsValidData(average_wind_speed, max_wind_speed)?"V":"I"));


            //bool isValid = average_wind_speed == -1;

            //try
            //{
            //    RemoteInterface.ConsoleServer.WriteLine(dt+","+tc.DeviceName+","+"AvgWndSpd:"+average_wind_speed+","+"AvgWndDir:"+average_wind_direction+",MaxWndSpd:"+max_wind_speed+",MaxWndDir:"+max_wind_direction+",Degree:"+am_degree);
            //    this.r_host_comm.setLSTenMinData(tc.DeviceName, dt, average_wind_speed, average_wind_direction, max_wind_speed, max_wind_direction, am_degree);
            //}
            //catch (Exception ex)
            //{
            //    ConsoleServer.WriteLine("Host err:" + ex.Message);
            //}
            //try
            //{
            //    this.notifier.NotifyAll(new NotifyEventObject(EventEnumType.LS_10min_data_Event, tc.DeviceName, new RemoteInterface.MFCC.LS_10Min_Data(tc.DeviceName, dt, average_wind_speed, average_wind_direction, max_wind_speed, max_wind_direction, am_degree)));
            //}
            //catch (Exception ex)
            //{
            //    ConsoleServer.WriteLine("Notifier:" + ex.Message);
            //}
        }

        void DataRepairTask()
        {


           // throw new Exception("datarepair task not implemented!");
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();

            cmd.Connection = cn;
            string devName = "";
            DateTime dt = new DateTime();
            while (true)
            {
                Comm.TC.LSTC tc;

                if (!IsLoadTcCompleted)
                {
                    System.Threading.Thread.Sleep(1000);
                    continue;
                }

                try
                {

                    cn.Open();
                    string sqlGetRepair = "select *  from (select t1.DEVICENAME, TIMESTAMP ,trycnt,datavalidity,comm_state from TBLLsDATA1Hr  t1 inner join tblDeviceConfig t2 on t1.devicename=t2.devicename where mfccid='{0}' and  TIMESTAMP between '{1}' and '{2}' and trycnt <1 and datavalidity='N' and comm_state<>3  and enable='Y' fetch first 300  row only) order by trycnt,timestamp desc ";

                    cmd.CommandText = string.Format(sqlGetRepair, mfccid, Comm.DB2.Db2.getTimeStampString(System.DateTime.Now.AddDays(-7)), Comm.DB2.Db2.getTimeStampString(System.DateTime.Now.AddMinutes(-20)));
                    ConsoleServer.WriteLine("Repair sql check!");
                    System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                    ConsoleServer.WriteLine("Repair task beg!");
                    while (rd.Read())
                    {
                        try
                        {
                            devName = rd[0] as string;
                            dt = System.Convert.ToDateTime(rd[1]);

                            if (Program.mfcc_ls.manager.IsContains(devName))
                                tc = (Comm.TC.LSTC)Program.mfcc_ls.manager[devName];
                            else
                                continue;

                            if (!tc.IsConnected)
                            {
                                dbServer.SendSqlCmd(string.Format("update tbldeviceconfig  set comm_state=3 where devicename='{0}' ", devName));
                                continue;
                            }



                            System.Data.DataSet ds = this.protocol.GetSendDataSet("get_a_temp_data");

                            ds.Tables[0].Rows[0]["month"] = dt.Month;
                            ds.Tables[0].Rows[0]["day"] = dt.Day;
                            ds.Tables[0].Rows[0]["hour"] = dt.Hour;
                            ds.Tables[0].Rows[0]["minute"] = dt.Minute;

                           // ds.Tables[0].Rows[0]["minute"] = dt.Minute;
                            Comm.SendPackage pkg = this.protocol.GetSendPackage(ds, 0xffff);

                            tc.Send(pkg);

                            if (pkg.ReturnTextPackage == null)
                                throw new Exception(tc.DeviceName + "," + dt + ",回補資料失敗!");

                          

                            ds = protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);

                            string sql = string.Format("update tblLSData1hr set DataValidity='V',day_var={0},mon_var={1} where devicename='{2}' and timestamp='{3}' ",
                                ds.Tables[0].Rows[0]["day_var"], ds.Tables[0].Rows[0]["mon_var"], tc.DeviceName, Comm.DB2.Db2.getTimeStampString(dt));


                            // Comm.TC.VD_MinAvgData data = tc.getOneMinAvgData(tc.Tc_GetVDData(dt), dt.Year, dt.Month);
                            dbServer.SendSqlCmd(sql);


                            for (int i = 0; i < System.Convert.ToInt32(ds.Tables[0].Rows[0]["ai_cnt"]); i++)
                            {
                                sql = "insert into tblLsData1HrDetail (devicename,timestamp,ai_cnt,sensor_var,acc_var) values ('{0}','{1}',{2},{3},{4}) ";
                                Program.mfcc_ls.dbServer.SendSqlCmd(string.Format(sql, tc.DeviceName, Comm.DB2.Db2.getTimeStampString(dt), i
                             , System.Convert.ToInt32(ds.Tables[0].Rows[0]["ai_cnt"]), System.Convert.ToInt32(ds.Tables["tblai_cnt"].Rows[i]["sensor_var"]), System.Convert.ToInt32(ds.Tables["tblai_cnt"].Rows[i]["acc_var"])));

                            }

                            ConsoleServer.WriteLine("==>repair:" + devName + "," + dt.ToString());

                        }
                        catch (Exception ex)
                        {
                            ConsoleServer.WriteLine(devName + "," + ex.Message + ex.StackTrace);
                            try
                            {

                                dbServer.SendSqlCmd(string.Format("update TBLLSDATA1hr set trycnt=trycnt+1 where devicename='{0}' and  timestamp='{1}'", devName, Comm.DB2.Db2.getTimeStampString(dt)));

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
