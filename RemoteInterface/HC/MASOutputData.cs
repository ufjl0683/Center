using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HC
{

   
     [Serializable]
   public  class MASOutputData
    {
       public byte[] laneids;
       public object [] displays;
     //  public int[] speedlimits;
       public MASOutputData(byte[] laneids, object [] dispays)  //display can be Cmsoutput data or int speedlimit
       {
           this.laneids = laneids;
           this.displays = dispays;
           if (displays == null)
               throw new Exception(" argument displays can not be null");
          // this.speedlimits=speedlimits;
       }



        

      // public MASOutputData(byte[]laneids,int [] limt
      
       
       
    }
}
