using System;
using System.Collections.Generic;
using System.Text;


namespace Host.TIMCC
{
    public class SectionTrafficData : IComparable<SectionTrafficData> {
	public static System.DateTime timeStamp;
	public String lineid,direction,fromLocation,endLocation;
	public int fromMile,endMile,travelTime,upperLimit,lowerLimit;
	public SectionTrafficData(String lineid,String direction,int fromMile,int endMile,String fromLocation,String endLocation,int travelTime,int upperLimit,int lowerLimit)
	{
	  this.lineid=lineid;
	  this.direction=direction;
	  this.fromMile=fromMile;
	  this.endMile=endMile;
	  this.fromLocation=fromLocation;
	  this.endLocation=endLocation;
	  this.travelTime=(travelTime==65535)?-1:travelTime;
      this.upperLimit = (upperLimit == 65535) ? -1 : upperLimit;
      this.lowerLimit = (lowerLimit == 65535) ? -1 : lowerLimit;
	  
	}
	
	public String getKey()
	{
		return this.lineid+"-"+this.direction;
	}
	
	public   String toString()
	{
		String ret="";
		ret="line:"+lineid+" " + "dir:"+direction+" fromMile:"+fromMile+" endMile:"+endMile+" travelTime:"+travelTime+" upper:"+upperLimit+" lower:"+lowerLimit + " from:"+fromLocation+" end:"+endLocation;
		return ret;
	}
	
	

	
	public int CompareTo(SectionTrafficData o) {
       // if (o.direction.Equals("S") || o.direction.Equals("E"))
            return this.fromMile - o.fromMile;
        //else
        //    return o.fromMile-this.fromMile;
	}
	
	
    
	
}
}
