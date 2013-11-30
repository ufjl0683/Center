using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;


namespace MFCC_RD
{
 public   class MFCC_RD:Comm.MFCC.MFCC_DataColloetBase
    {

     int minvalue = 0, maxvalue = 250;

         public  MFCC_RD(string mfccid,string devType,int remotePort,int notifyPort,int consolePort,string regRemoteName,Type regRemoteType)
            :base( mfccid,devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
        {


            loadValidCheckRule();

             
            new System.Threading.Thread(DataRepairTask).Start();
            
          
        }

     public override void BindEvent(object tc)
     {
         //throw new Exception("The method or operation is not implemented.");

         //((Comm.TC.RDTC)tc).OnTCReport += new Comm.OnTCReportHandler(MFCC_RD_OnTCReport);
         ((Comm.TC.RDTC)tc).On_RD_FivceMinData += new Comm.TC.RDTC.RDFiveMinDataHandler(MFCC_RD_On_RD_FivceMinData);

         ((Comm.TC.RDTC)tc).On_DegreeChange += new Comm.TC.RDTC.RDDegreeChangeHandler(MFCC_RD_On_RD_DegreeChange);
        
     }

     void MFCC_RD_On_RD_DegreeChange(Comm.TC.RDTC tcc, int datatype,DateTime dt, int pluviometric, int degree)
     {

         try
         {
             dbServer.SendSqlCmd(string.Format("insert into tblEventRD (devicename,TimeStamp,DataType,Current_Volume,Degree) values('{0}','{1}',{2},{3},{4})",tcc.DeviceName,
                 DbCmdServer.getTimeStampString(DateTime.Now),datatype,pluviometric,degree));
             if (r_host_comm != null)
                 this.r_host_comm.setRDEventData(tcc.DeviceName, dt, pluviometric, degree);

         }
         catch (Exception ex)
         {
             ConsoleServer.WriteLine("Host err:" + ex.Message);
         }

         //throw new Exception("The method or operation is not implemented.");
     }

     public void loadValidCheckRule()
     {
         ConsoleServer.WriteLine("Loaad Range Table");
         System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
         System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select minvalue,maxvalue from tblRDvalidRange");
         try
         {
             cn.Open();
             cmd.Connection = cn;
             System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
             if (rd.Read())
             {
                 minvalue = System.Convert.ToInt32(rd[0]);
                 maxvalue = System.Convert.ToInt32(rd[1]);
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

     bool IsValid(int amount)
     {
         return (amount >= minvalue && amount < maxvalue);
     }

     

     void MFCC_RD_On_RD_FivceMinData(Comm.TC.RDTC tc ,DateTime dt, int amount, int acc_amount, int degree,int datatype) //data type 0:simulate 1:real
     {
         //throw new Exception("The method or operation is not implemented.");
         string sql = "update tblRdData5Min set DataValidity='{5}',DataType={6},current_pluviometric={0},acc_pluviometric={1},rd_degree={2} where devicename='{3}' and timestamp='{4}' ";
         this.dbServer.SendSqlCmd(string.Format(sql, amount, acc_amount, degree, tc.DeviceName, Comm.DB2.Db2.getTimeStampString(dt), IsValid(amount) ? "V" : "I", datatype));

        
         bool isValid = amount == -1;

         try
         {
             if (r_host_comm != null)
             this.r_host_comm.setRDFiveMinData(tc.DeviceName, dt, amount, acc_amount, degree);
         }
         catch (Exception ex)
         {
             ConsoleServer.WriteLine("Host err:"+ex.Message);
         }
         try
         {
             this.notifier.NotifyAll(new NotifyEventObject(EventEnumType.RD_5min_data_Event,tc.DeviceName,new RemoteInterface.MFCC.RD_5Min_Data(tc.DeviceName,dt,amount,acc_amount,degree)));
         }
         catch (Exception ex)
         {
             ConsoleServer.WriteLine("Notifier:" + ex.Message);
         }
     }

     //void MFCC_RD_OnTCReport(object tcc, Comm.TextPackage txt)
     //{
     //    //throw new Exception("The method or operation is not implemented.");
     //    Comm.TC.RDTC tc = (Comm.TC.RDTC)tcc;
     //       string sql = "update tblRdData5Min set DataValidity='V',DataType=1,current_pluviometric={0},acc_pluviometric={1},rd_degree={2} where devicename='{3}' and timestamp='{4}' ";
     //    if (txt.Text[0] == 0x48)   // cycle data
     //    {

     //        if (txt.Text.Length != 13)
     //        {
     //            ConsoleServer.WriteLine(tc.DeviceName + "," + txt.ToString() + ",長度不符");
     //            return;
     //        }
     //        else
     //            ConsoleServer.WriteLine(tc.DeviceName + "," + txt.ToString());



     //        System.DateTime dt = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, txt.Text[6], txt.Text[7], txt.Text[8], 0);
          
     //        this.dbServer.SendSqlCmd(string.Format(sql,  txt.Text[9], txt.Text[10] * 256 + txt.Text[11], txt.Text[12], tc.DeviceName, Comm.DB2.Db2.getTimeStampString(dt)));


     //    }
     //}

     void DataRepairTask()
     {

         
         System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
         System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();
        
         cmd.Connection = cn;
         string devName = "";
         DateTime dt = new DateTime();
         while (true)
         {
             Comm.TC.RDTC tc;

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
                 System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                 while (rd.Read())
                 {
                     try
                     {
                         devName = rd[0] as string;
                         dt = System.Convert.ToDateTime(rd[1]);

                         if (Program.mfcc_rd.manager.IsContains(devName))
                             tc = (Comm.TC.RDTC)Program.mfcc_rd.manager[devName];
                         else
                             continue;

                         if (!tc.IsConnected)
                         {
                             dbServer.SendSqlCmd(string.Format("update tbldeviceconfig  set comm_state=3 where devicename='{0}' ", devName));
                             continue;
                         }
                             System.Data.DataSet ds = this.protocol.GetSendDataSet("get_a_temp_data");

                             ds.Tables[0].Rows[0]["day"]=dt.Day;
                             ds.Tables[0].Rows[0]["hour"] = dt.Hour;
                             ds.Tables[0].Rows[0]["minute"] = dt.Minute;
                         Comm.SendPackage pkg=this.protocol.GetSendPackage(ds, 0xffff);

                             tc.Send(pkg);
                             if (pkg.ReturnTextPackage != null && pkg.ReturnTextPackage.Text.Length != 12)
                                 throw new Exception("回補資料長度錯誤");
                             else if (pkg.ReturnTextPackage == null && pkg.result == Comm.CmdResult.ACK)
                              throw new Exception(tc.DeviceName + "," + dt + ",資料回補失敗!");
                             
                         ds=protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);

                         int amount=System.Convert.ToInt32( ds.Tables[0].Rows[0]["current_pluviometric"]);
                         string sql =string.Format( "update tblRdData5Min set DataValidity='{5}',DataType=1,current_pluviometric={0},acc_pluviometric={1},rd_degree={2} where devicename='{3}' and timestamp='{4}' " ,
                             ds.Tables[0].Rows[0]["current_pluviometric"], ds.Tables[0].Rows[0]["acc_pluviometic"], ds.Tables[0].Rows[0]["rd_degree"], tc.DeviceName, Comm.DB2.Db2.getTimeStampString(dt),IsValid(amount)?"V":"I");
                       
                        
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
