using System;
using System.Collections.Generic;
using System.Text;
//using Comm.TC;
using System.Runtime.Serialization;
using  RemoteInterface.MFCC;
namespace Comm.TC

{
   
    public class VD_MinAvgData:I_DB_able
    {

        public    int vol=0;
       // private int m_vol_cnt;
        public  int speed=0;
        public int  occupancy = 0;
        public int interval = 0;
        public int length = 0;
        public int year, month, day,hour,min,sec=0;
        public string vdName;
      //  private VDTC vdtc;
        public System.Data.DataSet orgds;
        public  int small_car_volume = 0;
        public int big_car_volume = 0;
        public  int connect_car_volume = 0;
        public  int big_car_speed = 0;
        public  int small_car_speed=0;
        public  int connect_car_speed = 0;
        public int small_car_length = 0;
        public int big_car_length = 0;
        public int connect_car_length = 0;
        public  VD_MinAvgData[] orgVDLaneData;
        public bool isReCalculate = false;
       
        public VD_MinAvgData(string devName)
        {
            this.vdName =devName;
           // this.vdtc = vdtc;
        }

        //public int vol
        //{
        //    get
        //    {


        //    }
        //}

        public DateTime dateTime
        {
            get
            {
                return new System.DateTime(year, month, day, hour, min,sec);
            }
        }

        public string devName
        {
            get
            {
                return vdName;
            }
            set
            {
                vdName = value;
            }
        }

        public RemoteInterface.MFCC.VD1MinCycleEventData ToVD1MinCycleEventData()// for client event use!
        {
            RemoteInterface.MFCC.VD1MinCycleEventData[] lanedata = null;

            try
            {
              
                if (this.orgds != null)
                    this.orgds.AcceptChanges();
                if (this.orgVDLaneData != null)
                {
                    lanedata = new RemoteInterface.MFCC.VD1MinCycleEventData[this.orgVDLaneData.Length];
                    for (int i = 0; i < this.orgVDLaneData.Length; i++)
                    {
                        VD1MinCycleEventData[] subLanedata=null;
                        if (orgVDLaneData[i].orgVDLaneData != null)
                        {
                            subLanedata = new VD1MinCycleEventData[orgVDLaneData[i].orgVDLaneData.Length];
                            for (int j = 0; j < orgVDLaneData[i].orgVDLaneData.Length; j++)
                                subLanedata[j] = orgVDLaneData[i].orgVDLaneData[j].ToVD1MinCycleEventData();
                        }
                        lanedata[i] = new RemoteInterface.MFCC.VD1MinCycleEventData(devName, new DateTime(this.year, this.month, this.day, this.hour, this.min, 0), orgVDLaneData[i].speed, orgVDLaneData[i].vol, orgVDLaneData[i].occupancy, orgVDLaneData[i].length, orgVDLaneData[i].interval,subLanedata, null, orgVDLaneData[i].IsValid);
                    }
                }
            }
            catch (Exception ex)
            {
                RemoteInterface.ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
            }
           
                return new RemoteInterface.MFCC.VD1MinCycleEventData(this.devName, new DateTime(this.year, this.month, this.day, this.hour, this.min, 0), speed, vol, occupancy, length, interval, lanedata, this.orgds, this.IsValid);
           
        }
        public bool IsValid
        {
            get

            {
                if (vol == 0 && speed == 0 && occupancy == 0)
                    return true;
                else if (vol == -1 || speed == -1 || occupancy == -1)
                    return false;
                else if (vol == 0 && (speed != 0 || occupancy != 0))
                    return false;

                return VDDataValidCheck.IsValid(speed, occupancy, vol);
            }
        }

        public override string ToString()
        {
            //return base.ToString();
            return this.vdName+","+this.dateTime+string.Format(",vol:{0},speed:{1},occpancy:{2}", vol, speed, occupancy);
        }

        public string get1minUpdateSql()
        {
            string sql = "update tblvddata1min ";
         
            sql += string.Format(" set DataValidity='{0}',", (IsValid) ? 'V' : 'I');
            if(orgds.Tables[0].Rows[0]["func_name"].ToString()!="get_unread_data")
            {
            int responseType = System.Convert.ToInt32(orgds.Tables[0].Rows[0]["Response_Type"]);
            switch (responseType)
            {
                case 0:
                      sql += "datatype=1,";
                    break;
                case 1:
                    goto case 0;
                case 2:
                    goto case 0;
                default:
                    sql += "datatype=0,";
                    break;
            }
           }
           else
            sql += "datatype=1,";
          //  sql += "datatype='R',";
       // int totalVol = 0;
            for (int i = 0; i < orgVDLaneData.Length; i++)
            {
                sql += "small_car_volume_Lane" + (i + 1) + "=" + orgVDLaneData[i].small_car_volume + ",";
                sql += "small_car_speed_Lane" + (i + 1) + "=" + orgVDLaneData[i].small_car_speed + ",";
                sql += "small_car_length_Lane" + (i + 1) + "=" + orgVDLaneData[i].small_car_length + ",";
                sql += "big_car_volume_Lane" + (i + 1) + "=" + orgVDLaneData[i].big_car_volume + ",";
                sql += "big_car_speed_Lane" + (i + 1) + "=" + orgVDLaneData[i].big_car_speed + ",";
                sql += "big_car_length_Lane" + (i + 1) + "=" + orgVDLaneData[i].big_car_length + ",";
                sql += "connect_car_volume_Lane" + (i + 1) + "=" + orgVDLaneData[i].connect_car_volume + ",";
                sql += "connect_car_speed_Lane" + (i + 1) + "=" + orgVDLaneData[i].connect_car_speed + ",";
                sql += "connect_car_length_Lane" + (i + 1) + "=" + orgVDLaneData[i].connect_car_length + ",";
                
                sql += "average_car_interval_Lane" + (i + 1) + "=" + orgVDLaneData[i].interval + ",";
                sql += "average_occupancy_Lane" + (i + 1) + "=" + orgVDLaneData[i].occupancy + ",";
/*
              
                this.small_car_volume += (orgVDLaneData[i].small_car_volume == -1) ? 0 : orgVDLaneData[i].small_car_volume;
                this.big_car_volume += (orgVDLaneData[i].big_car_volume == -1) ? 0 : orgVDLaneData[i].big_car_volume;
                this.connect_car_volume += (orgVDLaneData[i].connect_car_volume == -1) ? 0 : orgVDLaneData[i].connect_car_volume;
 * */
                 


            }
            sql += "car_volume=" + this.vol+",";
            sql += "small_car_volume=" + this.small_car_volume+",";
            sql += "big_car_volume=" + this.big_car_volume+",";
            sql += "connect_car_volume=" + this.connect_car_volume + ",";

            sql += "small_car_speed=" + this.small_car_speed + ",";
            sql += "big_car_speed=" + this.big_car_speed + ",";
            sql += "connect_car_speed=" + this.connect_car_speed + ",";
            sql += "car_speed=" + this.speed+ ",";

            sql += "small_car_length=" + this.small_car_length + ",";
            sql += "big_car_length=" + this.big_car_length + ",";
            sql += "connect_car_length=" + this.connect_car_length + ",";
            sql += "car_length=" + this.length + ",";

            sql += "Average_car_interval=" + this.interval + ",";
            if (orgds.Tables[0].Rows[0]["func_name"].ToString() == "get_unread_data")
            {
                sql += "trycnt=trycnt+1 ,";
            }
            sql += (isReCalculate) ? "utility=1," : "utility=-1,";
            sql += "Average_Occupancy=" + this.occupancy;

            

            sql += " where devicename='" + vdName + "' and timestamp='"+ DB2.Db2.getTimeStampString( year, month, day, hour, min,0)   +"'";


            return sql;
        }

        #region I_DB_able 成員

        public string getExecuteSql()
        {
            return get1minUpdateSql();
           // throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
