using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Comm.TC
{
   public  class VD_SpotSpeedData:I_DB_able
    {
       System.Data.DataRow row;
       string Devicename;
       int lane_id;
     public  VD_SpotSpeedData(string Devicename,int lane_id, DataRow row)
       {    this.row=row;
           this.Devicename = Devicename;
           this.lane_id = lane_id;
       }



       #region I_DB_able 成員

       public string getExecuteSql()
       {
           String HOSTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
           String GlobalSchema = "db2inst1";
           String InsertCmd = "insert into " + GlobalSchema + ".TBLVDDATASPOTSPEED" +
                             "(Devicename,HOSTTIME,TIMESTAMP,LANE_ID,CAR_VOLUME,CAR_SPEED,CAR_CLASS,CAR_LENGTH,CAR_INTERVAL) values";

           //  System.Data.DataSet ds = (System.Data.DataSet)objEvent.EventObj;
           ///   ds.AcceptChanges();

           // String Devicename  = objEvent.deviceName.ToString();//設備編號
           int CAR_VOLUME = 1;// ds.Tables[0].Rows.Count;//車流量            
           //foreach (DataRow row in ds.Tables[0].Rows)
           ////{
           //    for (int i = 0; i < Convert.ToInt32(row["car_volume"]); i++)
           //    {
           int LANE_ID = lane_id;// Convert.ToInt32(ds.Tables[0].Rows[i]["lane_id"]);//車輛所在之車道
                   string TIMESTAMP = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" +
                                      ((row["day"].ToString().Length == 2) ? row["day"].ToString() : "0" + row["day"].ToString()) + " " +
                                      ((row["hour"].ToString().Length == 2) ? row["hour"].ToString() : "0" + row["hour"].ToString()) + ":" +
                                      ((row["minute"].ToString().Length == 2) ? row["minute"].ToString() : "0" + row["minute"].ToString()) + ":" +
                                      ((row["second"].ToString().Length == 2) ? row["second"].ToString() : "0" + row["second"].ToString());
                   int CAR_SPEED = Convert.ToInt32(row["car_speed"]);//車速(公里/小時)
                   int CAR_LENGTH = Convert.ToInt32(row["car_length"]);
                   int CAR_INTERVAL = Convert.ToInt32(row["car_interval"]);
                   int CAR_CLASS = Convert.ToInt32(row["car_class"]);
                   InsertCmd += "('" + Devicename + "',TIMESTAMP('" + HOSTTIME + "'),TIMESTAMP('" + TIMESTAMP + "')," +
                                   LANE_ID.ToString() + "," + CAR_VOLUME.ToString() + "," + CAR_SPEED.ToString() + "," +
                                   CAR_CLASS + "," + CAR_LENGTH.ToString() + "," + CAR_INTERVAL.ToString() + ")";



               //}

           //}
           return InsertCmd;
       }
        

       #endregion
   }
}
