using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using RemoteInterface;

namespace Host.TC
{
  //  public delegate void OnNew5MinAvgDataInHandler(VD1MinCycleEventData data);
    public delegate void CSLSControlEventHandler(object sender,int level);

    public struct TrafficCSLSCOntrolRule
    {
      public  int ls;
      public  int rs;
      public int lo;
      public int ro;
      public  int level;

    }

     public  class VDDeviceWrapper:DeviceBaseWrapper
    {

       public  VD1MinCycleEventData latest5minAvgVdData;
       public  int jamLevel=0;
       public int cslsControlLevel = 0;
       static System.Collections.ArrayList cslsRules = new System.Collections.ArrayList();
    //   public int _aid_result=0;
       public AID.AIDobject aidobject;
     
       public event System.EventHandler OnEvent;
       public event CSLSControlEventHandler OnCSLSControlEvent;
        
     //  public event OnNew5MinAvgDataInHandler On5MinAvdData;

        static VDDeviceWrapper()
       {
         LoadTrafficCSLSRule();
        
       }

        public static int GetCSLSControlLevel(int speed,int vol)
        {
            if (speed == -1 || vol == -1)
                return 0;
            lock (cslsRules)
            {
                for (int i = 0; i < cslsRules.Count; i++)
                {
                    TrafficCSLSCOntrolRule rule = (TrafficCSLSCOntrolRule)cslsRules[i];
                    if (speed >= rule.ls && speed < rule.rs   &&  vol>=rule.lo &&  vol <rule.ro  )
                        return rule.level;

                }

                return 0;
            }
        }
        public static void LoadTrafficCSLSRule()
        {
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(DbCmdServer.getDbConnectStr());
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand()
            {
                CommandText = "select ls,rs,level,lo,ro from tblvddegree where location='I' order by level",
                Connection = cn
            };
            lock (cslsRules)
            {
                cslsRules.Clear();


                try
                {
                    cn.Open();
                    System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        int ls, rs, level,lo,ro;
                        ls = System.Convert.ToInt32(rd["ls"]);
                        rs = System.Convert.ToInt32(rd["rs"]);
                        lo = System.Convert.ToInt32(rd["lo"]);
                        ro = System.Convert.ToInt32(rd["ro"]);
                        level = System.Convert.ToInt32(rd["level"]);
                        cslsRules.Add(new TrafficCSLSCOntrolRule()
                        {
                            level = level,
                            rs = rs,
                            ls = ls,
                            lo=lo,
                            ro=ro
                        });


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

            } //lock
        }



       public VDDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus,string direction,int start_mileage,int end_mileage)
           : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus,direction)
       {
           try
           {
               this.aidobject = new AID.AIDobject(this.deviceName);
           }
           catch(Exception ex)
               {
                   ConsoleServer.WriteLine(ex.Message);
               }
       
           latest5minAvgVdData = new VD1MinCycleEventData(this.deviceName, System.DateTime.Now, -1, -1, -1, -1, -1, null,null ,false);
           jamLevel = -1;
           this.start_mileage = start_mileage;
           this.end_mileage = end_mileage;
       //    new System.Threading.Thread(SetVD5MinAvgDataTask1).Start();
       }

       public int Neural_Result { get; set; }
       public int AID_Result
       {
           get;
           set;
       }
         public int setAIDData(int occ,int docc,int spd,int dspd,int vol,int dvol)
         {
             if (this.aidobject == null)
                 return -1;

             return aidobject.SetData(occ, docc, spd,dspd,vol,dvol);
             

         }


         public void NotifyAidObjSettingChange()
         {
             this.aidobject.SetObjectParameters();
         }
       public int AvgSpeed
       {
           get
           {

               int totalspd = 0;
               try
               {
                   //if (!this.IsValid)
                   //    return -1;
                   if (this.getCurrent5MinAvgData().lanedata != null && this.location != "R")  //分鐘資料
                   {
                   //    bool IsModify = false;
                       int invalidcnt=0;
                       for (int i = 0; i < this.getCurrent5MinAvgData().lanedata.Length; i++)
                       {
                         //  if (getCurrent5MinAvgData().lanedata[i].speed != 0 && getCurrent5MinAvgData().lanedata[i].speed != -1)
                           // 原 VD 無資料時 不再使用自由車速 改填-1
                           if(getCurrent5MinAvgData().lanedata[i].speed==-1)
                           {
                               invalidcnt++;
                               continue;
                           }
                           if (getCurrent5MinAvgData().lanedata[i].speed != 0)
                               totalspd += getCurrent5MinAvgData().lanedata[i].speed;
                           else 
                           {
                               totalspd += Program.matrix.line_mgr[this.lineid].getSectionByMile(this.direction, this.mile_m).maxSpeed;
                        //       IsModify = true;
                           }

                       }

                       //if (IsModify)
                       //    ConsoleServer.WriteLine(this.deviceName + "補自由車速," + totalspd / this.getCurrent5MinAvgData().lanedata.Length);

                       if (invalidcnt == this.getCurrent5MinAvgData().lanedata.Length)
                           return -1;
                       return totalspd / (this.getCurrent5MinAvgData().lanedata.Length-invalidcnt);
                   }
               }
               catch(Exception ex) {
                   ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                   ;}


                   return this.getCurrent5MinAvgData().speed;
           }
       }
       public int AvgVol
       {
           get
           {
               return this.getCurrent5MinAvgData().vol;
           }
       }
       public int AvgOcc
       {
           get
           {
               return this.getCurrent5MinAvgData().occupancy;
           }
       }


       public bool IsValid
       {
           get
           {
               return this.getCurrent5MinAvgData().IsValid;
           }
       }

       object FiveMinLockObj = new object();
       public void Set5MinAvgData(VD1MinCycleEventData data)
       {

           Set5MinAvgDataTask(data);
           //latest5minAvgVdData = data;
           //lock (FiveMinLockObj)
           //{
           //    System.Threading.Monitor.Pulse(FiveMinLockObj);
           //}
       }

       //void SetVD5MinAvgDataTask1( )
       //{

       //    while (true)
       //    {

       //        lock (FiveMinLockObj)
       //        {
       //            System.Threading.Monitor.Wait(FiveMinLockObj);
       //        }

       //        Set5MinAvgDataTask(latest5minAvgVdData);
       //    }


       //}
         private  void Set5MinAvgDataTask(VD1MinCycleEventData data )
         {

            // VD1MinCycleEventData data;


             //while (true)
             //{
                 try
                 {
                     //lock (FiveMinLockObj)
                     //{
                     //    System.Threading.Monitor.Wait(FiveMinLockObj);
                     //}

                    this.latest5minAvgVdData=data;

                     //int speed;

                     #region body
                     if (data.IsValid)
                     {
                         int tmp, tmpcslcLevel;
                         if (data.vol != 0)
                         {    //jamLevel = Program.matrix.vd_jam_eval.getLevel(this.location, data.speed, data.occupancy);

                             tmp = Program.matrix.vd_jam_eval.getLevel(this.location, this.AvgSpeed, this.AvgOcc);
                             //  tmp = jamLevel;




                             if (tmp != jamLevel)
                             {
                                 jamLevel = tmp;
                                 if (this.OnEvent != null)
                                     this.OnEvent(this, null);
                             }
                             if (this.latest5minAvgVdData != null && this.latest5minAvgVdData.lanedata != null && this.latest5minAvgVdData.lanedata[0].lanedata != null && this.latest5minAvgVdData.lanedata[0].lanedata.Length != 0)
                             {
                                 tmpcslcLevel = GetCSLSControlLevel(this.AvgSpeed, this.AvgVol / this.latest5minAvgVdData.lanedata[0].lanedata.Length);
                                 if (tmpcslcLevel != this.cslsControlLevel)
                                 {
                                     this.cslsControlLevel = tmpcslcLevel;
                                     if (this.OnCSLSControlEvent != null)
                                         this.OnCSLSControlEvent(this, this.cslsControlLevel);
                                 }

                             }

                         }
                         else
                         {
                             if (jamLevel != 0)
                             {
                                 jamLevel = 0;
                                 if (this.OnEvent != null)
                                     this.OnEvent(this, null);
                             }

                             if (cslsControlLevel != 0)
                             {
                                 cslsControlLevel = 0;
                                 if (this.OnCSLSControlEvent != null)
                                     this.OnCSLSControlEvent(this, cslsControlLevel);
                             }


                         }
                     }
                     else
                     {
                         jamLevel = -1;
                     }
                     #endregion

                 }
                 catch
                 { ;}
             //}
            

         }

       private  VD1MinCycleEventData getCurrent5MinAvgData()
       {
#if! DEBUG
           if (System.DateTime.Now - this.latest5minAvgVdData.datatime > new TimeSpan(0, 5, 0))
           {
               this.jamLevel = -1;

               return new VD1MinCycleEventData(this.deviceName, this.latest5minAvgVdData.datatime, -1, -1, -1, -1, -1, null,null,false);
           }
           else
#endif
               return latest5minAvgVdData;

       }

       public new I_MFCC_VD getRemoteObj()
       {
           return (I_MFCC_VD)base.getRemoteObj();
       }


       public RemoteInterface.HC.VD5MinMovingData ToFWIS_Get5minMovingAvgData()
       {
           VD1MinCycleEventData data = getCurrent5MinAvgData();
         //  return new  RemoteInterface.HC.VD5MinMovingData(this.deviceName,data.vol,data.speed,data.occupancy,jamLevel);
           return new RemoteInterface.HC.VD5MinMovingData(this.deviceName, this.AvgVol,this.AvgSpeed, this.AvgOcc, jamLevel);

       }


       public override string ToString()
       {
           //return base.ToString();
           if (this.latest5minAvgVdData != null)
               return this.latest5minAvgVdData.ToString();
           else
               return "null";
       }

       //public void  getAllTrafficData(ref int speed,ref int occ,ref int jamlvl)
       //{
       //    if (System.DateTime.Now - latest5minAvgVdData.datatime > new TimeSpan(0, 5, 0) && latest5minAvgVdData.IsValid)
       //    {
       //        speed = latest5minAvgVdData.speed;
       //        occ = latest5minAvgVdData.occupancy;
       //        jamlvl = jamLevel;
       //    }
       //    else
       //    {
       //        speed = -1;
       //        occ = -1;
       //        jamlvl = -1;
       //    }
       //}



    }
}
