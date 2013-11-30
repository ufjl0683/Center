using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
namespace MFCC_IID
{
    public class MFCC_IID : Comm.MFCC.MFCC_DataColloetBase
    {
     
      public MFCC_IID(string mfccid, string devType, int remotePort, int notifyPort, int consolePort, string regRemoteName, Type regRemoteType)
          : base(mfccid, devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
      {
        
      }

        public override void BindEvent(object tc)
        {
          //  throw new Exception("The method or operation is not implemented.");
            ((Comm.TCBase)tc).OnTCReport += new Comm.OnTCReportHandler(MFCC_IID_OnTCReport);
        }

        void MFCC_IID_OnTCReport(object tc, Comm.TextPackage txt)
        {
            //throw new Exception("The method or operation is not implemented.");
            ConsoleServer.WriteLine(txt.ToString());
            if (txt.Text[0] == 0x0b || txt.Text[0]==0x01)
                return;
            try
            {
                string result="";
                string sql = "insert into tblIIDStateLog (devicename,timestamp,lane_id,cam_id,event_id,action_type) values('{0}','{1}',{2},{3},{4},{5})";
                string updatesql = "update tblIIDCamConfig set event_id={0},action_type={1} where lane_id={2} and cam_id={3} ";

                System.Data.DataSet ds = this.protocol.GetSendDsByTextPackage(txt,Comm.CmdType.CmdReport);
                int year,mon,day,hr,min,sec;

                System.Data.DataRow r=ds.Tables[0].Rows[0];
                year=System.Convert.ToInt32(r["year"]);
                mon=System.Convert.ToInt32(r["month"]);
                day=System.Convert.ToInt32(r["day"]);
                hr=System.Convert.ToInt32(r["hour"]);
                min=System.Convert.ToInt32(r["minute"]);
                sec=System.Convert.ToInt32(r["second"]);

                DateTime dt=new DateTime(year,mon,day,hr,min,sec);
                
                for(int i =0;i<ds.Tables["tblevent_lane_count"].Rows.Count;i++)
                {
                    int camid,laneid,iidevtid,action;
                    camid = System.Convert.ToInt32(r["cam_id"]);
                    laneid = System.Convert.ToInt32(ds.Tables["tblevent_lane_count"].Rows[i]["lane_id"]);
                    iidevtid=System.Convert.ToInt32(r["event_id"]);
                    action=System.Convert.ToInt32(r["action_type"]);
                    string dbcmd=string.Format(sql, (tc as Comm.TCBase).DeviceName,Comm.DB2.Db2.getTimeStampString(dt),laneid,
                      camid ,iidevtid,action);
                  
                    dbServer.SendSqlCmd(dbcmd);
                    dbServer.SendSqlCmd(string.Format(updatesql,iidevtid,action,laneid,camid));
                    try
                    {
                        r_host_comm.setIIDEvent(camid, laneid, iidevtid, action);
                        Util.SysLog(this.mfccid + ".log", "IID:camid:" + camid + ",laneid:" + laneid + ",evtid:" + iidevtid + ",action:" + action);
                    }
                    catch (Exception ex1)
                    {
                      
                        ConsoleServer.WriteLine(ex1.StackTrace);
                    }
                    


                }
                for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                {
                   result+=ds.Tables[0].Columns[i].ColumnName+":"+ds.Tables[0].Rows[0][i]+",";
                }
                ConsoleServer.WriteLine(result.TrimEnd(new char[]{','}));
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(txt.ToString());
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }

        }
    }
}
