using System;
using System.Collections.Generic;
using System.Text;
using Comm.MFCC;
using RemoteInterface;

namespace MFCC_AVI
{
   public  class MFCC_AVI:MFCC_DataColloetBase
    {
      
       volatile bool isInRobj = false;
    
       public MFCC_AVI(string mfccid, string devType, int remotePort, int notifyPort, int consolePort, string regRemoteName, Type regRemoteType)
           : base(mfccid, devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
       {

         
       }

       public override void BindEvent(object tc)
       {
           
           ((Comm.TCBase)tc).OnTCReport += new Comm.OnTCReportHandler(MFCC_AVI_OnTCReport);
       }


       public override void AfterDeviceAllStart()
       {
           base.AfterDeviceAllStart();
         
       }
        
       void MFCC_AVI_OnTCReport(object tc, Comm.TextPackage txt)
       {
          
           if (txt.Text[0] == 0x4f && txt.Text[1] == 0x04) //avi 主動回報
           {
               int day, hour, min, sec;
               day = txt.Text[6];
               hour = txt.Text[7];
               min=txt.Text[8];
               sec = txt.Text[9];
              // System.DateTime dt = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, day, hour, min, sec);
              System.DateTime dt = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);
             //  System.DateTime dt = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month,day, hour, min, sec);

               byte[] plate = new byte[6];
               System.Array.Copy(txt.Text, 10, plate,0 , 6);
               string plateStr =System.Text.UnicodeEncoding.Unicode.GetString( System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.Unicode, plate)).Trim();
               ConsoleServer.WriteLine(((Comm.TCBase)tc).DeviceName + "," + dt + "," + plateStr);
               string sql = string.Format("insert  into tblAviData1Min (devicename,timestamp,vehicle_plate) values('{0}','{1}','{2}')",
                   ((Comm.TCBase)tc).DeviceName, Comm.DB2.Db2.getTimeStampString(dt), plateStr);
               this.dbServer.SendSqlCmd(sql);
               try
               {
                   //if (!isInRobj)
                   //{
                     if(this.r_host_comm!=null)
                       this.r_host_comm.AddAviData(new RemoteInterface.MFCC.AVIPlateData(((Comm.TCBase)tc).DeviceName, dt, plateStr));
                   //}
               }
               catch (Exception ex)
               {
                   ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
               }
             //  finally
              
         
             }   
             else  if (txt.Text[0] == 0x4f && txt.Text[1] == 0x00)
             {
                 try
                 {
                     ConsoleServer.WriteLine(((Comm.TCBase)tc).DeviceName + ",總流量接收!");
                     System.Data.DataSet ds = this.protocol.GetSendDsByTextPackage(txt,Comm.CmdType.CmdReport);
                     int day, hour, min,amount;
                     System.Data.DataRow r = ds.Tables[0].Rows[0];
                     day = System.Convert.ToInt32(r["day"]);
                     hour = System.Convert.ToInt32(r["hour"]);
                     min = System.Convert.ToInt32(r["minute"]);
                     amount = System.Convert.ToInt32(r["amount"]);
                     DateTime dt=new DateTime(DateTime.Now.Year,DateTime.Now.Month,day,hour,min,0);
                     string sql = string.Format("insert into tblAviRecognition (devicename,timestamp,volume) values('{0}','{1}',{2})",
                         ((Comm.TCBase)tc).DeviceName, Comm.DB2.Db2.getTimeStampString(dt), amount);
                     this.dbServer.SendSqlCmd(sql);
                 }
                 catch (Exception ex)
                 {
                     ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                 }

             }
            
       }

    }
}
