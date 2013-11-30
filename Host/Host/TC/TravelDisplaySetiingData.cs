using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Host.TC
{
   public  class TravelDisplaySettingData
    {
     public  int iconid;
       public string message1, message2;
       public byte[] fcolorbyte1, fcolorbyte2, bcolorbyte1, bcolorbyte2;
       public TravelDisplayDetailData[] detailData;
       public  int displaypart;
       public int upperTravelTimeLimit, lowerTravelTimeLimit, offset;  // 旅許時間限制條件
       public bool enable;
       public int traveltime=-1, lower=-1, upper=-1;
       
       public TravelDisplaySettingData(int displaypart,int iconid, string message1, string message2, byte[] fcolorbyte1, byte[] fcolorbyte2, byte[] bcolorbyte1, byte[] bcolorbyte2, TravelDisplayDetailData[] detailData,int upperTravelTimeLimit,int lowerTravelTimeLimit,int offset,bool enable)
       {
            this.message1=message1;
            this.message2=message2;
            this.iconid=iconid;
            this.fcolorbyte1 = fcolorbyte1;
            this.fcolorbyte2 = fcolorbyte2;
            this.bcolorbyte1 = bcolorbyte1;
            this.bcolorbyte2 = bcolorbyte2;
            this.detailData = detailData;
            this.displaypart = displaypart;
            this.upperTravelTimeLimit = upperTravelTimeLimit;
            this.lowerTravelTimeLimit = lowerTravelTimeLimit;
            this.offset = offset;
            this.enable = enable;
       }


       public Color[] getForeColors1()
       {
           Color[] ret = new Color[fcolorbyte1.Length / 3];
           for (int i = 0; i < fcolorbyte1.Length / 3; i++)
               ret[i] = Color.FromArgb(fcolorbyte1[i * 3], fcolorbyte1[i * 3 + 1], fcolorbyte1[i * 3 + 2]);
           return ret;

       }

       public Color[] getForeColors2()
       {
           Color[] ret = new Color[fcolorbyte2.Length / 3];
           for (int i = 0; i < fcolorbyte2.Length / 3; i++)
               ret[i] = Color.FromArgb(fcolorbyte2[i * 3], fcolorbyte2[i * 3 + 1], fcolorbyte2[i * 3 + 2]);
           return ret;

       }

      




   }




   public class TravelDisplayDetailData
    {
        public int displaypart,  startMile, endMile;
        public string lineid, dir;
        public bool isXML = false; 

        public TravelDisplayDetailData(int displaypart,string  lineid,string dir,int startMile,int endMile)
        {
            this.displaypart = displaypart;
            this.lineid=lineid;
            this.dir=dir;
            this.startMile=startMile;
            this.endMile=endMile;
        }

       public TravelDisplayDetailData(int displaypart, string lineid, string dir, int startMile, int endMile,bool isXML):this(displaypart,lineid,dir,startMile,endMile)
       {
           this.isXML = isXML;
       }




    }
}
