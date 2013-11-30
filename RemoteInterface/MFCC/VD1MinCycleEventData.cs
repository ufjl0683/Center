using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{

     [Serializable]
     public  class VD1MinCycleEventData
    {

         public string devName;
         public DateTime datatime;
         public int speed, vol, occupancy, length,interval;
         public  VD1MinCycleEventData[]lanedata;
         public  System.Data.DataSet orgds;
         public bool IsValid = false;
         public VD1MinCycleEventData(string devName,DateTime dt,int speed,int vol ,int occupancy,int length,int interval,VD1MinCycleEventData[]lanedata,System.Data.DataSet orgds,bool isValid)
         {
             this.devName = devName;
             this.speed = speed;
             this.vol = vol;
             this.occupancy = occupancy;
             this.datatime = dt;
             this.orgds=orgds;
             this.lanedata = lanedata;
             this.interval = interval;
             this.length = length;
             this.IsValid = isValid;
         }

         public override string ToString()
         {
           // if(this.IsValid)
                return devName+((devName.Length>=8)?"\t":"\t\t")+"spd:"+speed+"\tvol="+vol+"\tocpy:"+occupancy+"\t"+datatime.ToString("yy/MM/dd hh:mm:ss")+"\t"+((this.IsValid)?"V":"I");
            //else
         //  return devName +((devName.Length>=8)?"\t":"\t\t") + "spd:" + -1 + "\tvol=" + -1 + "\tocpy:" + -1+"\t"+datatime.ToString("yy/MM/dd hh:mm:ss");

         }
         


    }
}
