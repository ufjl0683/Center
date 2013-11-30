using System;
using System.Collections.Generic;
using System.Text;
using Host.TC;
using RemoteInterface.MFCC;
using System.Threading;
using RemoteInterface;

namespace Host.AVI
{
    public class AVISection
    {
        public int/* EffectInterval = 80 ,*/upperinterval = 10, lowerinterval = 10;
        public string secid;
        public AVIDeviceWrapper startTC, endTC;
        private int travelSecs = -1;
        private double variance = -1;
        private int valid_cnt = 0;
        

        System.Collections.ArrayList dataContainer = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList());
       // System.Threading.Timer tmr;
        public AVISection(string secid, AVIDeviceWrapper startTC, AVIDeviceWrapper endTC,/* int EffectInterval,*/int upperInterval,int lowerInterval,int validcnt)
        {
            this.secid = secid;
            this.startTC = startTC;
            this.endTC = endTC;
            this.upperinterval = upperInterval;
            this.lowerinterval = lowerInterval;
            this.valid_cnt = validcnt;
          //  this.EffectInterval = EffectInterval;
           

           // tmr = new System.Threading.Timer(new System.Threading.TimerCallback(TmrElapse));
            System.DateTime dt = System.DateTime.Now;
            dt = dt.AddSeconds(-dt.Second);
            dt = dt.AddMinutes(1);
            new Thread(calcTravelTimeTask).Start();

          //  tmr.Change((int)((TimeSpan)(dt - DateTime.Now)).TotalMilliseconds, Timeout.Infinite);
           

        }


        public void LoadAVISectionEffective()
        {
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();
            cmd.Connection = cn;
            try
            {
                cmd.CommandText = "select SampleInterval,upperinterval,lowerinterval,VALID_CNT_1MIN from tblAVISection where avisectionid='" + this.secid + "'";
                cn.Open();
                System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                   // this.EffectInterval = System.Convert.ToInt32(rd[0]);
                    this.upperinterval = System.Convert.ToInt32(rd[1]);
                    this.lowerinterval = System.Convert.ToInt32(rd[2]);
                    this.valid_cnt = System.Convert.ToInt32(rd[3]);
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


        
        public string LineId
        {
            get
            {
                return startTC.lineid;
            }
        }
        public string Direction
        {
            get
            {
               return  startTC.direction;
            }
        }


        public int  StartMileage
        {
            get
            {
                return this.startTC.mile_m;
            }
        }

        public int EndMileage
        {
            get
            {
                return this.endTC.mile_m;
            }
        }

        void calcTravelTimeTask()
        {
            System.DateTime dt=new DateTime();
            while (true)
            {
                try
                {

                    // dt;
                    this.calcTravelTime();
                    RemoteInterface.ConsoleServer.WriteLine("***********" + this.ToString() + "*************");

                    dt = dt.AddSeconds(-dt.Second);

                    int weightedTravelTime = Program.matrix.line_mgr[this.LineId].getTravelTime(this.Direction, this.StartMileage, this.EndMileage);
                    int weigthedSpd = (int)(System.Math.Abs(this.StartMileage - this.EndMileage) / weightedTravelTime  *3.6);
                    int vdtraveltime=-1,etctraveltime=-1,histraveltime=-1;
                    
                    vdtraveltime=Program.matrix.line_mgr[this.LineId].getVD_TravelTime(this.Direction, this.StartMileage, this.EndMileage);
                    etctraveltime = Program.matrix.line_mgr[this.LineId].getETC_TravelTime(this.Direction, this.StartMileage, this.EndMileage);
                    histraveltime = Program.matrix.line_mgr[this.LineId].getHIS_TravelTime(this.Direction, this.StartMileage, this.EndMileage);
                   // histraveltime = Program.matrix.line_mgr[this.LineId].get(this.Direction, this.StartMileage, this.EndMileage);
                    string sql = string.Format("update tblAVISectionData1min set traveltime={0},variance={1} ,speed={2}, release_traveltime={5}, release_speed={6},vd_traveltime={7},etc_traveltime={8},htd_traveltime={9} where AVISectionID='{3}' and timestamp='{4}'",
                        this.TravelTime, this.Variance, this.Speed, this.secid, RemoteInterface.DbCmdServer.getTimeStampString(dt), weightedTravelTime, weigthedSpd,vdtraveltime,etctraveltime,histraveltime);

                    Program.matrix.dbServer.SendSqlCmd(sql);

                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message);

                }
                finally
                {
                      dt= System.DateTime.Now;
                    dt = System.DateTime.Now.AddSeconds(-dt.Second).AddMinutes(1);
                    //dt = dt.AddMinutes(1);
                    int m_sec = (int)((TimeSpan)(dt - DateTime.Now)).TotalMilliseconds;
                    if (m_sec <= 0)
                        m_sec = 1000;
                    System.Threading.Thread.Sleep(m_sec);
                }
            }

        }
        public int TravelTime
        {

            get
            {
                return travelSecs;
            }
        }

        public int Speed
        {
            get
            {
                if (TravelTime == -1)
                    return -1;

                return (int)(Math.Abs(this.startTC.mile_m - this.endTC.mile_m) / TravelTime * 3.6);
            }
        }

        public double Variance
        {

            get
            {

                return variance;
            }
        }
        public void AddAviData(AVIPlateData data)
        {


            if (this.startTC.deviceName == data.DevName)
                this.startTC.AddPlate(data);
            else if (this.endTC.deviceName == data.DevName)
            {
                this.endTC.AddPlate(data);
                if (this.startTC.Match(data.plate))
                {


                    TimeSpan ts = data.dt - this.startTC.GetPlateData(data.plate).dt;
                    int speed = (int)(System.Math.Abs(this.endTC.mile_m - this.startTC.mile_m) / ts.TotalSeconds * 3600 / 1000);
                    lock (dataContainer)
                    {
                        dataContainer.Add(new TravelData(data.dt, (int)ts.TotalSeconds));
                    }

               //     RemoteInterface.ConsoleServer.WriteLine("******" + secid + "," + data.plate + " match!speed=" + speed + "km*************");
                }
            }
        }
        public void calcTravelTime()
        {


            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            System.Collections.ArrayList ary = new System.Collections.ArrayList();



            lock (dataContainer)
            {


                TimeSpan fiveMinSpan=new TimeSpan(0, 5, 0);

                foreach (TravelData data in dataContainer)
                {

                    if (dt - data.dt > fiveMinSpan)
                    {
                        ary.Add(data);
                        continue;
                    }



                }



                foreach (TravelData data in ary)
                    dataContainer.Remove(data);


                if (dataContainer.Count == 0)
                {
                    travelSecs = -1;
                    variance = -1;
                    return;
                }


              //  int effectCnt = dataContainer.Count * EffectInterval / 100;
                int upperCnt = dataContainer.Count * upperinterval / 100;
                int lowerCnt = dataContainer.Count * lowerinterval / 100;


                dataContainer.Sort();
                int totalSecs = 0;
                int cnt = 0;

                //calc average secs;
                for (int i =lowerinterval; i < dataContainer.Count-upperinterval ; i++)
                {
                    totalSecs += ((TravelData)dataContainer[i]).travelSecs;
                    cnt++;
                }
                //for (int i = (dataContainer.Count - effectCnt) / 2; i < (dataContainer.Count - effectCnt) / 2 + effectCnt; i++)
                //{
                //    totalSecs += ((TravelData)dataContainer[i]).travelSecs;
                //    cnt++;
                //}
                if (cnt == 0 || cnt < valid_cnt)
                {
                    travelSecs = -1;
                    variance = -1;
                    return;
                }
                if (cnt == 1)
                {
                    travelSecs = totalSecs / cnt;
                    variance = 0;
                    return;
                }
               

                double totVar = 0;
                int avgSecs = totalSecs / cnt;
                cnt = 0;

                for (int i = lowerinterval; i < dataContainer.Count - upperinterval; i++)
                {
                    totVar += System.Math.Pow(avgSecs - ((TravelData)dataContainer[i]).travelSecs, 2);
                    cnt++;
                }
                //for (int i = (dataContainer.Count - effectCnt) / 2; i < (dataContainer.Count - effectCnt) / 2 + effectCnt; i++)
                //{
                //    totVar += System.Math.Pow(avgSecs - ((TravelData)dataContainer[i]).travelSecs, 2);
                //    cnt++;
                //}

                this.travelSecs = avgSecs;
                this.variance = totVar / cnt;


                //**************** 


            }

        }
        public override string ToString()
        {

           return this.secid + ",spd:" + this.Speed + ",traveltime:" + this.TravelTime + ",variance:" + this.Variance + ",upperinterval:" + this.upperinterval+",lowerinterval:"+lowerinterval ;

            //return base.ToString();
        }

    }



    class TravelData : IComparable
    {


        public DateTime dt;
        public int travelSecs;

        internal TravelData(DateTime dt, int travelSec)
        {
            this.dt = dt;
            this.travelSecs = travelSec;
        }


        //  #region IComparable 成員

        public int CompareTo(object obj)
        {
            return this.travelSecs - ((TravelData)obj).travelSecs;
        }

        // #endregion
    }

}
