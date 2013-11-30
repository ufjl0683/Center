using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using RemoteInterface.MFCC;
//using Comm;

namespace RemoteInterface.MFCC
{
 public    interface I_MFCC_VD:I_MFCC_Base
    {

      

      void setRealTime(string tcname, int laneid,int cycle,int  durationMin); //現點數率  ,laneid=0 代表立即終止
      void setRealTime(string ip, int port, int laneid, int cycle, int durationMin);
      VD1MinCycleEventData  getVDLatest5MinAvgData(string devname);
      void loadValidCheckRule();
      void setTransmitCycle(string devName,int cycle);
     VD1MinCycleEventData getVDLatest1MinData(string devname);
     void enableVD20SecData(string devName,bool enabled);
     
    } 
}
