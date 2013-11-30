using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using RemoteInterface;

namespace Host.TC
{
  //  public delegate void OnNew5MinAvgDataInHandler(VD1MinCycleEventData data);
     public  class VDDeviceWrapper:DeviceBaseWrapper
    {

       public  VD1MinCycleEventData latest5minAvgVdData;
       public  int jamLevel=0;


         public AID.AIDobject aidobject = new AID.AIDobject();
       public event System.EventHandler OnEvent;

       

     //  public event OnNew5MinAvgDataInHandler On5MinAvdData;
       public VDDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus,string direction,int start_mileage,int end_mileage)
           : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus,direction)
       {
           latest5minAvgVdData = new VD1MinCycleEventData(this.deviceName, System.DateTime.Now, -1, -1, -1, -1, -1, null,null ,false);
           jamLevel = -1;
           this.start_mileage = start_mileage;
           this.end_mileage = end_mileage;
       }


         public int setAIDData(int occ,int docc,int spd)
         {
             
             return aidobject.SetData(occ, docc, spd);
             

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
       public void Set5MinAvgData(VD1MinCycleEventData data)
       {
           this.latest5minAvgVdData = data;

           //int speed;

           if (data.IsValid)
           {
               int tmp;
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

                  

               }
               else
               {
                   if (jamLevel != 0)
                   {
                       jamLevel = 0;
                       if (this.OnEvent != null)
                           this.OnEvent(this, null);
                   }

                   
               }
           }
           else
           {
               jamLevel = -1;
           }
           //if(this.On5MinAvdData!=null)
           //    this.On5MinAvdData(data);
       }


       private  VD1MinCycleEventData getCurrent5MinAvgData()
       {
           if (System.DateTime.Now - this.latest5minAvgVdData.datatime > new TimeSpan(0, 5, 0))
           {
               this.jamLevel = -1;

               return new VD1MinCycleEventData(this.deviceName, this.latest5minAvgVdData.datatime, -1, -1, -1, -1, -1, null,null,false);
           }
           else
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
