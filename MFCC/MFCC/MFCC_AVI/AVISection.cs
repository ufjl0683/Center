using System;
using System.Collections.Generic;
using System.Text;
using Host.TC;
using RemoteInterface.MFCC;
using System.Threading;

namespace Host.AVI
{
   public  class AVISection
    {
       public int EffectInterval = 80;
       public string secid;
       public AVIDeviceWrapper startTC, endTC;
       private int travelSecs = -1;
       private double variance=-1;
       

       System.Collections.ArrayList dataContainer =System.Collections.ArrayList.Synchronized( new System.Collections.ArrayList());
       System.Threading.Timer tmr;
       public AVISection(string secid, AVIDeviceWrapper startTC, AVIDeviceWrapper endTC)
       {
           this.secid = secid;
           this.startTC = startTC;
           this.endTC = endTC;



           tmr = new System.Threading.Timer(new System.Threading.TimerCallback(TmrElapse));
           System.DateTime dt = System.DateTime.Now;
           dt = dt.AddSeconds(-dt.Second);
           dt = dt.AddMinutes(1);
           
           tmr.Change((int) ((TimeSpan)(dt-DateTime.Now)).TotalMilliseconds  , Timeout.Infinite);

       }

       void TmrElapse(object sender)
       {
           this.calcTravelTime();
           RemoteInterface.ConsoleServer.WriteLine(DateTime.Now + "," + secid + ",traver seconds:" + this.TravelTime + "secs,avg spd:" + this.Speed + ",variance:" + this.variance+ ",startTcDataCnt:"+this.startTC.CurrDataCnt+",EndTCDataCnt:"+this.endTC.CurrDataCnt);
           System.DateTime dt = System.DateTime.Now;
           dt = dt.AddSeconds(-dt.Second);
           string sql = string.Format("update tblAVISectionData1min set traveltime={0},variance={1}  where AVISectionID='{2}' and timestamp='{3}' ", this.TravelTime, this.Variance, this.secid, Comm.DB2.Db2.getTimeStampString(dt));
           MFCC_AVI.Program.mfcc_avi.ExecuteSql(sql);
           dt = dt.AddMinutes(1);
           tmr.Change((int)((TimeSpan)(dt - DateTime.Now)).TotalMilliseconds, Timeout.Infinite);

        
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

           
           if (this.startTC.devicename == data.DevName)
               this.startTC.AddPlate(data);
            else if (this.endTC.devicename == data.DevName)
           {
               this.endTC.AddPlate(data);
               if (this.startTC.Match(data.plate)  )
               {


                   TimeSpan ts = data.dt - this.startTC.GetPlateData(data.plate).dt;
                   int speed = (int)(System.Math.Abs(this.endTC.mile_m - this.startTC.mile_m) / ts.TotalSeconds * 3600 / 1000);
                   lock (dataContainer)
                   {
                       dataContainer.Add(new TravelData(data.dt, (int)ts.TotalSeconds));
                   }
                  
                   RemoteInterface.ConsoleServer.WriteLine("******"+ secid+"," + data.plate + " match!speed=" + speed + "km*************");
               }
           }
       }


       public void  calcTravelTime()
       {

        
           DateTime dt =new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,DateTime.Now.Hour,DateTime.Now.Minute,0);
          System.Collections.ArrayList ary = new System.Collections.ArrayList();
        


           lock (dataContainer)
           {
             



               foreach (TravelData data in dataContainer)
               {

                   if (dt - data.dt > new TimeSpan(0, 5, 0))
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


               int effectCnt=  dataContainer.Count * EffectInterval / 100;
               
               dataContainer.Sort();
               int totalSecs=0;
               int cnt = 0;

               //calc average secs;
               for (int i = (dataContainer.Count - effectCnt) / 2; i < (dataContainer.Count - effectCnt) / 2 + effectCnt; i++)
               {
                   totalSecs += ((TravelData)dataContainer[i]).travelSecs;
                   cnt++;
               }
               if (cnt == 0)
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

               double totVar=0;
               int avgSecs = totalSecs / cnt;
               cnt = 0;
               for (int i = (dataContainer.Count - effectCnt) / 2; i < (dataContainer.Count - effectCnt) / 2 + effectCnt; i++)
               {
                   totVar += System.Math.Pow(avgSecs - ((TravelData)dataContainer[i]).travelSecs, 2);
                   cnt++;
               }

               this.travelSecs = avgSecs;
               this.variance = totVar / cnt;
              

               //**************** 


           }

       }

    }

   

     class TravelData: IComparable
    {


      public   DateTime dt;
      public int travelSecs;

     internal    TravelData(DateTime dt, int travelSec)
        {
            this.dt = dt;
            this.travelSecs = travelSec;
        }


      //  #region IComparable 成員

        public int CompareTo(object obj)
        {
            return this.travelSecs-((TravelData)obj).travelSecs;
        }

       // #endregion
    }
    
}
