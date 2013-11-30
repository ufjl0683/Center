using System;
using System.Collections.Generic;
using System.Text;

namespace Host.TIMCC
{
   public class TravelData{
	string rampname;
	int secs;
	String lineid,dir;
	public TravelData(String lineid,String dir,String rampname,int secs)
	{
		this.rampname=rampname;
		this.secs=secs;
		this.lineid=lineid;
		this.dir=dir;
	}
	
	public String toString()
	{
	  return "<data lineid=\""+lineid+"\" direction=\""+dir+"\" ramp=\""+rampname+"\" travel_time=\""+secs+"\"/>";
	}
}
}
