using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace Host.Event.Jam
{
   public  class RampVDData
    {

       public string deviceName, lineid, direction, divisionId, divisionName, InOut, divisionType;
       public int mile_m,laneid;
       public TC.VDDeviceWrapper vd;
       public event EventHandler OnEvent;
       System.Timers.Timer tmr1min = new System.Timers.Timer(1000 * 60);

       public  int laneJamLevel=0;
        public RampVDData(string deviceName,string lineid,string dir,string divisionId,string divisionName,string InOut,TC.VDDeviceWrapper vd,int mile_m,int laneid)
        {
            this.deviceName = deviceName;
            this.lineid = lineid;
            this.direction = dir;
            this.divisionId = divisionId;
            this.divisionName = divisionName;
            this.InOut = InOut;
        
            this.vd = vd;
            this.mile_m = mile_m;
            this.laneid = laneid;
            this.vd.OnEvent += new EventHandler(RampVDData_OnEvent);
            if (laneid != -1)
            {
                this.tmr1min.Elapsed += new System.Timers.ElapsedEventHandler(tmr1min_Elapsed);
                this.tmr1min.Start();
            }
        }

       void tmr1min_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
       {
           //throw new Exception("The method or operation is not implemented.");
           try
           {
               if (vd.latest5minAvgVdData.lanedata.Length <= laneid)
               {
                   ConsoleServer.WriteLine(this.divisionName + ",壅塞偵測器車道" + laneid + "指定錯誤!");
                   return;
               }

               int occ, spd;

               spd = vd.latest5minAvgVdData.lanedata[0].lanedata[laneid].speed;
               occ = vd.latest5minAvgVdData.lanedata[0].lanedata[laneid].speed;
               int level = Program.matrix.vd_jam_eval.getLevel(vd.location, spd, occ);
               if (laneJamLevel != level)
               {
                   this.laneJamLevel = level;

                   if (this.OnEvent != null)

                       this.OnEvent(this, null);
               }

           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
           }



       }

       void RampVDData_OnEvent(object sender, EventArgs e)
       {
           if (this.OnEvent != null && this.laneid==-1)
             
               this.OnEvent(this, null);
       }

       public string Key
       {
           get
           {
               return lineid + "_" + direction + "_" + divisionId+"_"+this.vd.deviceName;
           }
       }

    }
}
