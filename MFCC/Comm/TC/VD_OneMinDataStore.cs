using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
namespace Comm.TC
{
   public  class VD_OneMinDataStore
    {
        System.Collections.Queue queData = System.Collections.Queue.Synchronized(new System.Collections.Queue(10));
        System.DateTime minDateTime;
        System.DateTime maxDateTime;
       public string vdName;
       private VDTC vdtc;
        public VD_OneMinDataStore(VDTC vdtc)
        {
            this.vdName = vdtc.DeviceName;
            this.vdtc = vdtc;
        }
        public void inData(VD_MinAvgData data)
        {
            lock (queData)
            {
                try
                {
                   
                 //   ConsoleServer.WriteLine(data.ToString()+" enque!");
            
                    if (queData.Count == 0)
                    {
                        queData.Enqueue(data);
                        maxDateTime = minDateTime =data.dateTime;
                    }
                    else if (data.dateTime > maxDateTime )
                    {
                        queData.Enqueue(data);
                        if (System.Math.Abs(((TimeSpan)(data.dateTime - System.DateTime.Now)).TotalMinutes) < 6)
                            maxDateTime = data.dateTime;
                        else
                            maxDateTime = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day,
                                System.DateTime.Now.Hour, System.DateTime.Now.Minute, 0);
                        if (queData.Count > 5)
                        {

                            while (queData.Count > 5)
                                queData.Dequeue();
                           
                          
                        }
                        data = (VD_MinAvgData)queData.Peek();
                       // day = System.Convert.ToInt32(ds.Tables["tblmain"].Rows[0]["day"]);
                       // hour = System.Convert.ToInt32(ds.Tables["tblmain"].Rows[0]["hour"]);
                       // min = System.Convert.ToInt32(ds.Tables["tblmain"].Rows[0]["minute"]);
                        minDateTime = data.dateTime;
                        
                    }
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.StackTrace);
                }

            }
        }

/*
       public VD_MinAvgData getFiveMinMovingAvgData()
       {
           VD_MinAvgData ret = new VD_MinAvgData(this.vdtc.DeviceName);



           lock (this.queData)
           {

               object[] objs = queData.ToArray();
               VD_MinAvgData[] OneMinDatas = new VD_MinAvgData[objs.Length];

               for (int i = 0; i < OneMinDatas.Length; i++)
                   OneMinDatas[i] = (VD_MinAvgData)objs[i];






               // cal five min total vol
               ret.year = this.maxDateTime.Year;
               ret.month = this.maxDateTime.Month;
               ret.day = this.maxDateTime.Day;
               ret.hour = this.maxDateTime.Hour;
               ret.min = this.maxDateTime.Minute;
               ret.vdName = this.vdtc.DeviceName;

               int invalidCnt = 0;
               for (int i = 0; i < OneMinDatas.Length; i++)
               {
                   if (!OneMinDatas[i].IsValid)
                   {
                       invalidCnt++;
                      // ret.vol += -1;
                   }
                   else
                       ret.vol += OneMinDatas[i].vol;
               }

               if (invalidCnt == OneMinDatas.Length)
               {
                   ret.vol = -1;
                   ret.speed = -1;
                   ret.occupancy = -1;
                   return ret;
               }

               if (ret.vol == 0)
               {
                   ret.speed = 0;
                   ret.occupancy = 0;
                   return ret;
               }
               //else
               //    ret.vol = ret.vol + invalidCnt + (ret.vol + invalidCnt) / (OneMinDatas.Length - invalidCnt) * invalidCnt;  //補足無效資料
               // cal 5 min  avg speed
               invalidCnt = 0;
              // int totalvol = 0;
               for (int i = 0; i < OneMinDatas.Length; i++)
               {
                   //  if (OneMinDatas[i].speed != -1 || OneMinDatas[i].vol != -1)
                   if (OneMinDatas[i].IsValid)
                   {
                       ret.speed += OneMinDatas[i].speed * OneMinDatas[i].vol;
                     //  totalvol += OneMinDatas[i].vol;
                   }




               }

                   ret.speed = ret.speed / ret.vol;

               //cal 5 min occupancy

               invalidCnt = 0;

               for (int i = 0; i < OneMinDatas.Length; i++)
               {
                   // if (OneMinDatas[i].occupancy == -1)

                   ret.occupancy += OneMinDatas[i].occupancy * OneMinDatas[i].vol;
               }


               ret.occupancy = ret.occupancy / ret.vol; 

               return ret;
           }

           //throw new NotImplementedException();
       }
 * */

      
       public VD_MinAvgData getFiveMinMovingAvgData()
       {
           VD_MinAvgData ret = new VD_MinAvgData(this.vdtc.DeviceName);

         
           
           lock (this.queData)
           {

               object[] objs= queData.ToArray();
               VD_MinAvgData[] OneMinDatas = new VD_MinAvgData[objs.Length];

               for (int i = 0; i < OneMinDatas.Length; i++)
                   OneMinDatas[i] = (VD_MinAvgData)objs[i];
                  
                   


              

               // cal five min total vol
               ret.year = this.maxDateTime.Year;
               ret.month = this.maxDateTime.Month;
               ret.day = this.maxDateTime.Day;
               ret.hour = this.maxDateTime.Hour;
               ret.min = this.maxDateTime.Minute;
               ret.vdName = this.vdtc.DeviceName;
                 
               int invalidCnt = 0;
               for (int i = 0; i < OneMinDatas.Length; i++)
               {
                   if (!OneMinDatas[i].IsValid)
                   {
                       invalidCnt++;
                       ret.vol += -1;
                   }
                   else
                       ret.vol += OneMinDatas[i].vol;
               }

               if (invalidCnt == OneMinDatas.Length)
               {
                   ret.vol = -1;
                   ret.speed = -1;
                   ret.occupancy = -1;
                   return ret; ;
               }
               else
               {
                   ret.vol = ret.vol + invalidCnt + (ret.vol + invalidCnt) / (OneMinDatas.Length - invalidCnt) * invalidCnt;  //補足無效資料
                   
               }
               // cal 5 min  avg speed
               invalidCnt = 0;
               int totalvol=0;
               //int freespd=0;

               
                for (int i = 0; i < OneMinDatas.Length; i++)
                {
                  //  if (OneMinDatas[i].speed != -1 || OneMinDatas[i].vol != -1)
                    if(OneMinDatas[i].IsValid)
                    {
                        ret.speed +=       OneMinDatas[i].speed * OneMinDatas[i].vol;
                        totalvol += OneMinDatas[i].vol;
                    }


                    else
                        invalidCnt++;


                }

                if (invalidCnt == OneMinDatas.Length)
                    ret.speed = -1;
                else if (totalvol == 0)
                    ret.speed = 0;
                else
                    ret.speed = ret.speed / totalvol;
                
               //cal 5 min occupancy
               
                invalidCnt = 0;
            
               for (int i = 0; i < OneMinDatas.Length; i++)
               {
                  // if (OneMinDatas[i].occupancy == -1)
                   if(!OneMinDatas[i].IsValid)
                       invalidCnt++;
                   else
                       ret.occupancy += OneMinDatas[i].occupancy * OneMinDatas[i].vol ;
               }

               if (invalidCnt == OneMinDatas.Length)
                   ret.occupancy = -1;
               else if (totalvol == 0)
                   ret.occupancy = 0;
               else
                   ret.occupancy = ret.occupancy / totalvol;

               //add for new  2010-3-5
               ret.orgVDLaneData = OneMinDatas;
               return ret;
           }

           //throw new NotImplementedException();
       }
       
      



       //public VD_MinAvgData getOneMinAvgData(System.Data.DataSet ds)
       //  {
       //    System.Data.DataRow rmain = ds.Tables["tblMain"].Rows[0];

       //    VD_MinAvgData[] laneData = new VD_MinAvgData[ds.Tables["tbllane_count"].Rows.Count];
       //    int  invalidcnt=0;
       //     VD_MinAvgData ret = new VD_MinAvgData(this.vdtc);

       //     ret.year = System.DateTime.Now.Year;
       //     ret.month = System.DateTime.Now.Month;
       //     ret.day = System.Convert.ToInt32(ds.Tables["tblmain"].Rows[0]["day"]);
       //     ret.hour = System.Convert.ToInt32(ds.Tables["tblmain"].Rows[0]["hour"]);
       //     ret.min = System.Convert.ToInt32(ds.Tables["tblmain"].Rows[0]["minute"]);

       //     for (int laneid = 0; laneid < ds.Tables["tbllane_count"].Rows.Count;laneid++ )
       //     {
       //         int small_car_volume = 0, big_car_volume = 0, connect_car_volume = 0;
       //         int small_car_speed = 0, big_car_speed = 0, connect_car_speed = 0,average_occupancy=0,average_car_interval=0;
       //         System.Data.DataRow laneRow = ds.Tables["tbllane_count"].Rows[laneid];

       //         laneData[laneid] = new VD_MinAvgData(this.vdtc);


       //         small_car_volume = System.Convert.ToInt32(laneRow["small_car_volume"]);

       //         big_car_volume = System.Convert.ToInt32(laneRow["big_car_volume"]);

       //         connect_car_volume = System.Convert.ToInt32(laneRow["connect_car_volume"]);
       //         big_car_speed=System.Convert.ToInt32(laneRow["big_car_speed"]);
       //         small_car_speed=System.Convert.ToInt32(laneRow["small_car_speed"]);
       //         connect_car_speed = System.Convert.ToInt32(laneRow["connect_car_speed"]);
       //         average_occupancy=System.Convert.ToInt32(laneRow["average_occupancy"]);
       //         average_car_interval=System.Convert.ToInt32(laneRow["average_car_interval"]);




       //            //  車道總流量計算

       //         if (small_car_volume == 0xff && big_car_volume == 0xff && connect_car_volume == 0xff) 
       //             laneData[laneid].vol = -1;
       //         else
       //         {
       //             if (small_car_volume != 0xff)
       //                 laneData[laneid].vol += small_car_volume;
       //             if (big_car_volume != 0xff)
       //                 laneData[laneid].vol += big_car_volume;
       //             if (connect_car_volume != 0xff)
       //                 laneData[laneid].vol += connect_car_volume;
       //         }

       //         if (laneData[laneid].vol == -1 || connect_car_speed == 0xff && small_car_speed == 0xff && big_car_speed==0xff)
       //             laneData[laneid].speed = -1;
       //         else
       //         {
       //             if (connect_car_volume != 0xff && connect_car_speed != 0xff)
       //                 laneData[laneid].speed += connect_car_volume * connect_car_speed;
       //             if(small_car_volume!=0xff && small_car_speed !=0xff)
       //                 laneData[laneid].speed += small_car_volume * small_car_speed;

       //             if (big_car_volume != 0xff && big_car_speed != 0xff)
       //                 laneData[laneid].speed += big_car_volume * big_car_speed;

       //         }

       //         if (laneData[laneid].speed == -1 || laneData[laneid].vol == -1)
       //             laneData[laneid].speed = -1;
       //         else if (laneData[laneid].vol == 0)

       //             laneData[laneid].speed = 0;
       //         else

       //             laneData[laneid].speed = laneData[laneid].speed / laneData[laneid].vol;

       //         if (average_occupancy == 0xff)
       //             laneData[laneid].occupancy = -1;
       //         else
       //             laneData[laneid].occupancy = average_occupancy;

       //         laneData[laneid].interval = average_car_interval;

       //     }// for
       //    // 記算不分車道總流量
       //    invalidcnt=0;
       //     for (int laneId = 0; laneId < laneData.Length; laneId++)
       //     {
       //         ret.vol += laneData[laneId].vol;
       //         if (laneData[laneId].vol == -1)
       //             invalidcnt++;
       //     }

       //     if (ret.vol == laneData.Length * -1)
       //         ret.vol = -1;
       //     else
       //         ret.vol = ret.vol + invalidcnt + (ret.vol + invalidcnt) / (laneData.Length - invalidcnt) * invalidcnt;

       //    //計算平均車速

       //     invalidcnt = 0;
       //     for (int laneId = 0; laneId < laneData.Length; laneId++)
       //     {
       //         if(laneData[laneId].vol!=-1 && laneData[laneId].speed!=-1)
       //             ret.speed += laneData[laneId].vol * laneData[laneId].speed;
       //         else
       //             invalidcnt++;
       //     }

       //     if (invalidcnt == laneData.Length)
       //         ret.speed = -1;
       //     else
       //         ret.speed = ret.speed / (laneData.Length - invalidcnt);


       //    //計算平均佔有率

       //       invalidcnt = 0;
       //       for (int laneId = 0; laneId < laneData.Length; laneId++)
       //       {

       //           if (laneData[laneId].occupancy == -1)
       //               invalidcnt++;
       //           else
       //               ret.occupancy += laneData[laneId].occupancy;

       //       }

       //       if (invalidcnt == laneData.Length)
       //           ret.occupancy = -1;
       //       else
       //           ret.occupancy = ret.occupancy / (laneData.Length - invalidcnt);



       //       return ret;
       //  }

   }
}
