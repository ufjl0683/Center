using System;
using System.Collections.Generic;

using System.Text;

namespace Host.Event.APID
{
   public  class TrainData
    {

       public  VDData vd1;
       public  VDData vd2;
       public TrainData()
       {

            vd1 = new VDData();
            vd2 = new VDData();
       }

       public void getTrainData(ref int difvol,ref int difspd,ref int difocc  )
       {

           difvol = vd1.Volume - vd2.Volume;
           difspd = vd1.AvgSpd - vd2.AvgSpd;
           difocc = vd1.Occupancy - vd2.Occupancy;

       }

       public void AddLaneData(string vdname, LaneData ldata)
       {
           if (vdname == "VD01")
               vd1.AddLaneData(ldata);
           else
               vd2.AddLaneData(ldata);
       }

       public int Level
       {
           get
           {
               return vd1.Level;
           }
       }

       public override string ToString()
       {
           //return base.ToString();
           return vd1.ToString() + "\r\n" + vd2.ToString();
       }

    }


   public class VDData
   {
       System.Collections.Generic.List<LaneData> lanedatas = new List<LaneData>();
      


       public void AddLaneData(LaneData ldata)
       {
           lanedatas.Add(ldata);

       }
       public int Level
       {
           get
           {
               return lanedatas[0].level;
           }
       }
       public int AvgSpd
       {
           get
           {
               int totalspd = 0,totalvol=0;

               foreach (LaneData data in lanedatas)
               {
                   totalspd += data.smallspd * data.smallvol + data.bigspd * data.bigvol + data.connectspd * data.connectvol;
                   totalvol += data.smallvol + data.bigvol + data.connectvol;
               }

               try
               {
                   return totalspd / totalvol;
               }
               catch (Exception ex)
               {
                   throw new Exception(ex.Message);
               }
           }
       }



       public int Volume
       {
           
           get{
                   int totalvol = 0;
                   foreach (LaneData data in lanedatas)
                       totalvol +=  data.bigvol+data.smallvol+data.connectvol;

                   return totalvol;
              }

       }
       public int Occupancy
       {
           get
           {
               int totalocc = 0;
               foreach (LaneData data in lanedatas)
                   totalocc += data.occ;

               return totalocc / lanedatas.Count;
               

           }
       }


       public override string ToString()
       {
           //return base.ToString();

           return "Spd:" + AvgSpd + "," + "vol:" + Volume + "," + "occ:" + Occupancy+"lvl:"+Level;
       }


       
   }


    public class LaneData
    {
        public  int smallvol , bigvol, connectvol, smallspd, bigspd, connectspd, occ, level;
        public LaneData(int smallvol ,int bigvol,int connectvol,int smallspd,int bigspd,int connectspd,int occ,int level)
        {
          //  this.laneid = laneid;
            this.smallvol = smallvol;
            this.smallspd = smallspd;
            this.bigvol = bigvol;
            this.bigspd = bigspd;
            this.connectvol = connectvol;
            this.connectspd = connectspd;
            this.occ = occ;
            this.level = level;
        }
    }
}
