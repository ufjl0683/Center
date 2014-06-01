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
        private int totalcnt = 0;
        public string startDevSource;
        public string endDevSource;
       public bool IsETAGSection;
       // private string StartDevName;
       // string ext_linid="";
       // string ext_dir="";
       // int ext_mile_m=0;
         public    System.Collections.ArrayList dataContainer = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList());
       // System.Threading.Timer tmr;
        public AVISection(string secid, AVIDeviceWrapper startTC, AVIDeviceWrapper endTC,/* int EffectInterval,*/int upperInterval,int lowerInterval,int validcnt,string startDevSource,string endDevSource,bool IsEtagSection/*,string ext_linid,string ext_dir,int ext_mile_m*/)
        {
            this.secid = secid;
            this.startTC = startTC;
            this.endTC = endTC;
            this.upperinterval = upperInterval;
            this.lowerinterval = lowerInterval;
            this.valid_cnt = validcnt;
            this.startDevSource = startDevSource;
            this.endDevSource = endDevSource;
            this.IsETAGSection = IsEtagSection;
            //this.ext_linid = ext_linid;
            //this.ext_dir = ext_dir;
            //this.ext_mile_m = ext_mile_m;
            //this.StartDevName = StartDevName;
          //  this.EffectInterval = EffectInterval;
           

           // tmr = new System.Threading.Timer(new System.Threading.TimerCallback(TmrElapse));
            System.DateTime dt = System.DateTime.Now;
            dt = dt.AddSeconds(-dt.Second);
            dt = dt.AddMinutes(1);
            new Thread(calcTravelTimeTask).Start();

            //if (this.endTC.deviceName != "AVI-N1-N-241.7")
            //    return;

            if (this.startDevSource.Trim() != "C"  || this.endDevSource!="C")  //非中區自有的 AVI 設備
                new Thread(ExtAviFetchTask).Start();

//#if DEBUG 
//            lock (this)
//                System.Threading.Monitor.Wait(this);

//#endif
          //  tmr.Change((int)((TimeSpan)(dt - DateTime.Now)).TotalMilliseconds, Timeout.Infinite);
           

        }

        void ExtAviFetchTask()
        {
            System.DateTime lastExchangeTime = DateTime.MinValue;
           

            while (true)
            {

                try
                {
                    
                    Avi1MinXmlPackage extAviPkg;
                    if(this.startDevSource!="C")
                     extAviPkg = AVI_XML(this.startTC.deviceName, this.startDevSource);
                    else
                     extAviPkg = AVI_XML(this.endTC.deviceName, this.endDevSource);
#if DEBUG
                    if (secid == "N3_S_259.114_280.905")
                        Console.WriteLine("check point!");
#endif 

                    if (extAviPkg.datetime != lastExchangeTime)
                    {
                        lastExchangeTime = extAviPkg.datetime;
                        foreach (RemoteInterface.MFCC.AVIPlateData pdata in extAviPkg.Items)
                        {


                            this.AddAviData(pdata);
                        }
                    }

                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                }

//#if DEBUG
//                lock (this)
//                    System.Threading.Monitor.Pulse(this);
//#endif
                System.Threading.Thread.Sleep(30 * 1000);
            
            }
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

                    int weigthedSpd = -1;
                    if(weightedTravelTime!=-1)
                      weigthedSpd=  (int)(System.Math.Abs(this.StartMileage - this.EndMileage) / weightedTravelTime  *3.6);
                   
                    int vdtraveltime=-1,etctraveltime=-1,histraveltime=-1;
                    
                    vdtraveltime=Program.matrix.line_mgr[this.LineId].getVD_TravelTime(this.Direction, this.StartMileage, this.EndMileage);
                    etctraveltime = Program.matrix.line_mgr[this.LineId].getETC_TravelTime(this.Direction, this.StartMileage, this.EndMileage);
                    histraveltime = Program.matrix.line_mgr[this.LineId].getHIS_TravelTime(this.Direction, this.StartMileage, this.EndMileage);
                   // histraveltime = Program.matrix.line_mgr[this.LineId].get(this.Direction, this.StartMileage, this.EndMileage);
                    string sql = string.Format("update tblAVISectionData1min set traveltime={0},variance={1} ,speed={2}, release_traveltime={5}, release_speed={6},vd_traveltime={7},etc_traveltime={8},htd_traveltime={9},total_cnt={10},onreason={11} where AVISectionID='{3}' and timestamp='{4}' ",
                        travelSecs, variance, (travelSecs <= 0) ? -1 : (int)(Math.Abs(this.startTC.mile_m - this.endTC.mile_m) / travelSecs * 3.6)
                        
                     /*this.Speed */, this.secid, RemoteInterface.DbCmdServer.getTimeStampString(dt), weightedTravelTime, weigthedSpd, vdtraveltime, etctraveltime, histraveltime, this.totalcnt, (totalcnt < valid_cnt) ? 1 : 0);
#if !DEBUG
                    Program.matrix.dbServer.SendSqlCmd(sql);
#else
                    if (this.secid == "N1_N_190.6_173.27")
                        Console.WriteLine("check");
#endif

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

                if (this.totalcnt >= valid_cnt)
                    return travelSecs;
                else
                    return -1;
            }
        }

        public int Speed
        {
            get
            {
                if (TravelTime == -1)
                    return -1;
                if (this.totalcnt < valid_cnt)
                    return -1;

                return (int)(Math.Abs(this.startTC.mile_m - this.endTC.mile_m) / TravelTime * 3.6);
            }
        }

        public double Variance
        {

            get
            {

                if (this.totalcnt >= valid_cnt)
                    return variance;
                else
                    return -1;
            }
        }
        public void AddAviData(AVIPlateData data)
        {
            //if (this.endTC.deviceName != "AVI-N1-N-241.7")
            //    return;

           string sql = "insert into TBLAVIDATA1MINACROSS (DeviceName,TimeStamp,Vehicle_Plate) values('{0}','{1}','{2}')";
           string sql1 = "insert into tblEtagCompareLog (AviSectionId,UP_TIMESTAMP,DOWN_TIMESTAMP,TAGID) values('{0}','{1}','{2}','{3}')";

            if (this.startTC.deviceName == data.DevName)
            {
#if DEBUG
                //if (secid == "N1_S_248.2_275.155")
                //    Console.WriteLine("check point!");
#endif
                this.startTC.AddPlate(data);
#if !DEBUG
                if(startDevSource!="C")
                                Program.matrix.dbServer.SendSqlCmd(
                    string.Format(sql, data.DevName, DbCmdServer.getTimeStampString(data.dt), data.plate));
#endif           
            }
            else if (this.endTC.deviceName == data.DevName)
            {
 #if !DEBUG
                if(endDevSource!="C")
                   
                Program.matrix.dbServer.SendSqlCmd(
                     string.Format(sql, data.DevName, DbCmdServer.getTimeStampString(data.dt), data.plate));
#endif
                this.endTC.AddPlate(data);
                if (this.startTC.Match(data.plate))
                {
#if DEBUG
                    //if (secid == "N3_S_259.114_280.905")
                    //    Console.WriteLine("check point!");
#endif
                    if (this.startTC.IsETag())
                    {

                        Program.matrix.dbServer.SendSqlCmd(
                        string.Format(sql1, this.secid,
                        DbCmdServer.getTimeStampString(this.startTC.GetPlateData(data.plate).dt), DbCmdServer.getTimeStampString(data.dt), data.plate));
                    }

                    TimeSpan ts = data.dt - this.startTC.GetPlateData(data.plate).dt;

                    int speed = 0;
                    //if(this.startDevSource.Trim() != "C")
                    //   speed = (int)(System.Math.Abs(this.endTC.mile_m - this.startTC.mile_m) / ts.TotalSeconds * 3600 / 1000);
                    //else
                    speed = (int)(System.Math.Abs(this.endTC.mile_m - this.startTC.mile_m) / ts.TotalSeconds * 3600 / 1000);

                    //       speed = (int)(System.Math.Abs(this.endTC.mile_m - this.ext_mile_m) / ts.TotalSeconds * 3600 / 1000);
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
            //if (this.endTC.deviceName != "AVI-N1-N-241.7")
            //    return;
            

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
                    totalcnt = 0;
                    return;
                }


              //  int effectCnt = dataContainer.Count * EffectInterval / 100;
                int upperCnt = dataContainer.Count * upperinterval / 100;
                int lowerCnt = dataContainer.Count * lowerinterval / 100;


                dataContainer.Sort();
                int totalSecs = 0;
                int cnt = 0;

                //calc average secs;
                for (int i = lowerCnt; i < dataContainer.Count - upperCnt; i++)
                {
                    totalSecs += ((TravelData)dataContainer[i]).travelSecs;
                    cnt++;
                }
                //for (int i = (dataContainer.Count - effectCnt) / 2; i < (dataContainer.Count - effectCnt) / 2 + effectCnt; i++)
                //{
                //    totalSecs += ((TravelData)dataContainer[i]).travelSecs;
                //    cnt++;
                //}
                if (cnt == 0 )
                {
                    travelSecs = -1;
                    variance = -1;
                    totalcnt = 0;
                    return;
                }
                if (cnt == 1)
                {
                    travelSecs = totalSecs / cnt;
                    variance = 0;
                    totalcnt = 1;
                    return;
                }
               

                double totVar = 0;
                int avgSecs = totalSecs / cnt;
                cnt = 0;

                for (int i = lowerCnt; i < dataContainer.Count - upperCnt; i++)
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
                this.totalcnt = cnt;
                ////if (this.endDevSource != "C")
                ////    Console.WriteLine();
                //**************** 


            }

        }


         Avi1MinXmlPackage AVI_XML(string extAvidevName, string centerCode)
        {
            Avi1MinXmlPackage ret = null;
            string Nuri = "http://10.1.1.50/1min_avi_data.xml";
            string Suri = "http://10.41.0.100/xml/1min_avi_data.xml";
            string xmlAviDevname = "";

            System.Xml.XmlTextReader rd;
            if (centerCode == "N")
                rd = new System.Xml.XmlTextReader(Nuri);
            else if (centerCode == "S")
                rd = new System.Xml.XmlTextReader(Suri);
            else
                throw new Exception("Not a ext AVI Exchange area code!");
           
             
          //  System.DateTime xmlfiledate;
            while (rd.Read())
            {
                try
                {

                    switch (rd.NodeType)
                    {
                        case System.Xml.XmlNodeType.Element:
                            // Console.WriteLine(rd.Name);

                            if (rd.Name == "file_attribute")
                            {
                                //  Console.WriteLine(rd.GetAttribute("time"));
                                ret = new Avi1MinXmlPackage();
                                ret.datetime = System.Convert.ToDateTime(rd.GetAttribute("time"));
                            }
                            else
                                if (rd.Name == "avi_data")
                                {

                                    xmlAviDevname = rd.GetAttribute("eqId");
                                    //if (rd.GetAttribute("eqId") != extAvidevName)
                                    //{
                                    //    rd.ReadEndElement();
                                    //    break;
                                    //}
                                    //   Console.WriteLine("\t" + rd.GetAttribute("eqId"));
                                }
                                else if (rd.Name == "plateData" && xmlAviDevname == extAvidevName)
                                {
                                    // Console.WriteLine("\t\t" + rd.GetAttribute("plateNo") + "\t" + rd.GetAttribute("catchTime"));
                                    int catchSec = System.Convert.ToInt32(rd.GetAttribute("catchTime"));
                                    ret.Items.Add(new RemoteInterface.MFCC.AVIPlateData(extAvidevName, ret.datetime.AddSeconds(catchSec), rd.GetAttribute("plateNo")));
                                }
                            break;

                    }
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                }
            }
             if(rd!=null)
                    rd.Close();
            return ret;

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

      public  class Avi1MinXmlPackage
        {
            public DateTime datetime;
            public System.Collections.Generic.List<RemoteInterface.MFCC.AVIPlateData> Items;
            public Avi1MinXmlPackage()
            {
                Items = new List<RemoteInterface.MFCC.AVIPlateData>();
            }
           
        }

       
        

}
