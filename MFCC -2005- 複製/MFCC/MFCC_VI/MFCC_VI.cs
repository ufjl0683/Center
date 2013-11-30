using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_VI
{
    public class MFCC_VI : Comm.MFCC.MFCC_DataColloetBase
    {

        int minvalue=0, maxvalue=9999;
        
         public  MFCC_VI(string mfccid,string devType,int remotePort,int notifyPort,int consolePort,string regRemoteName,Type regRemoteType)
            :base( mfccid,devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
        {

            loadValidCheckRule();
          
            new System.Threading.Thread(DataRepairTask).Start();

          
        }


        public void loadValidCheckRule()
        {
             ConsoleServer.WriteLine("Load Range Table");
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select minvalue,maxvalue from tblVIvalidRange");
            try
            {
                cn.Open();
                cmd.Connection = cn;
                System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                   minvalue=System.Convert.ToInt32(rd[0]);
                   maxvalue=System.Convert.ToInt32(rd[1]);
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

        bool IsValid(int distance)
        {
            return (distance >= minvalue && distance < maxvalue);
        }
         public override void BindEvent(object tc)
         {
             //throw new Exception("The method or operation is not implemented.");

             //((Comm.TC.RDTC)tc).OnTCReport += new Comm.OnTCReportHandler(MFCC_RD_OnTCReport);
             ((Comm.TC.VITC)tc).On_VI_FivceMinData += new Comm.TC.VITC.VIFiveMinDataHandler(MFCC_VI_On_VI_FivceMinData);
             ((Comm.TC.VITC)tc).On_DegreeChange += new Comm.TC.VITC.VIDegreeChangeHandler(MFCC_VI_On_DegreeChange);
             
         }

        void MFCC_VI_On_DegreeChange(Comm.TC.VITC tcc,int datatype, DateTime dt, int distance, int degree) 
        {
            //throw new Exception("The method or operation is not implemented.");
            try
            {
                dbServer.SendSqlCmd(string.Format("insert into tblEventVI (DeviceName,TimeStamp,DataType,Distance,Degree) values('{0}','{1}',{2},{3},{4})",tcc.DeviceName,
                    DbCmdServer.getTimeStampString(DateTime.Now),datatype,distance,degree));
                if (r_host_comm != null)
                this.r_host_comm.setVIEventData(tcc.DeviceName, dt, distance, degree);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine("Host err:" + ex.Message);
            }
        }


        void MFCC_VI_On_VI_FivceMinData(Comm.TC.VITC tc, DateTime dt, int distance, int degree,int datatype) //datatype 1 real 0 simul
        {
            //throw new Exception("The method or operation is not implemented.");
            string sql = "update tblViData5Min set DataValidity='{4}',DataType={5},vi_distance={0},vi_degree={1} where devicename='{2}' and timestamp='{3}' ";
            this.dbServer.SendSqlCmd(string.Format(sql, distance, degree, tc.DeviceName, Comm.DB2.Db2.getTimeStampString(dt), IsValid(distance) ? "V" : "I", datatype));


            bool isValid = distance == -1;

            try
            {
                RemoteInterface.ConsoleServer.WriteLine(dt+","+tc.DeviceName+","+"Distance:"+distance+","+"degree:"+degree);
                if (r_host_comm != null)
               this.r_host_comm.setVIFiveMinData(tc.DeviceName, dt, distance, degree);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine("Host err:" + ex.Message);
            }
            try
            {
                this.notifier.NotifyAll(new NotifyEventObject(EventEnumType.VI_5min_data_Event, tc.DeviceName, new RemoteInterface.MFCC.VI_5Min_Data(tc.DeviceName, dt, distance, degree)));
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
                Comm.TC.VITC tc;

                if (!IsLoadTcCompleted)
                {
                    System.Threading.Thread.Sleep(1000);
                    continue;
                }

                try
                {
                   
                    cn.Open();
                    string sqlGetRepair = "select *  from (select t1.DEVICENAME, TIMESTAMP ,trycnt,datavalidity,comm_state from TBLRDDATA5MIN  t1 inner join tblDeviceConfig t2 on t1.devicename=t2.devicename where mfccid='{0}' and  TIMESTAMP between '{1}' and '{2}' and trycnt <1 and datavalidity='N' and comm_state<>3  and enable='Y' fetch first 300  row only) order by trycnt,timestamp desc ";

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

                            if (Program.mfcc_vi.manager.IsContains(devName))
                                tc = (Comm.TC.VITC)Program.mfcc_vi.manager[devName];
                            else
                                continue;

                            if (!tc.IsConnected)
                            {
                                dbServer.SendSqlCmd(string.Format("update tbldeviceconfig  set comm_state=3 where devicename='{0}' ", devName));
                                continue;
                            }



                            System.Data.DataSet ds = this.protocol.GetSendDataSet("get_a_temp_data");

                            ds.Tables[0].Rows[0]["day"] = dt.Day;
                            ds.Tables[0].Rows[0]["hour"] = dt.Hour;
                            ds.Tables[0].Rows[0]["minute"] = dt.Minute;
                            Comm.SendPackage pkg = this.protocol.GetSendPackage(ds, 0xffff);

                            tc.Send(pkg);

                            if (pkg.ReturnTextPackage == null  && pkg.result== Comm.CmdResult.ACK)
                            {
                               throw new Exception(tc.DeviceName + "," + dt + ", 資料回補失敗!");
                               
                            }
                            
                            if (pkg.ReturnTextPackage.Text.Length != 11)
                                throw new Exception("回補資料長度錯誤");

                            ds = protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);

                            string sql = string.Format("update tblVIData5Min set DataValidity='{4}',DataType=1,vi_distance={0},vi_degree={1} where devicename='{2}' and timestamp='{3}' ",
                                ds.Tables[0].Rows[0]["vi_distance"], ds.Tables[0].Rows[0]["vi_degree"], tc.DeviceName, Comm.DB2.Db2.getTimeStampString(dt), IsValid(System.Convert.ToInt32(ds.Tables[0].Rows[0]["vi_distance"]))?"V":"I" );


                            // Comm.TC.VD_MinAvgData data = tc.getOneMinAvgData(tc.Tc_GetVDData(dt), dt.Year, dt.Month);
                            dbServer.SendSqlCmd(sql);

                            ConsoleServer.WriteLine("==>repair:" + devName + "," + dt.ToString());

                        }
                        catch (Exception ex)
                        {
                            ConsoleServer.WriteLine(devName + "," + ex.Message + ex.StackTrace);
                            try
                            {

                                dbServer.SendSqlCmd(string.Format("update TBLRDDATA5MIN set trycnt=trycnt+1 where devicename='{0}' and  timestamp='{1}'", devName, Comm.DB2.Db2.getTimeStampString(dt)));

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
