using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using RemoteInterface;


namespace Host.TIMCC
{
   public  class TIMCC_SectionManager
    {
     System.Collections.Hashtable   /* <String, SectionTrafficData[]> */lines =System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
       string url;
       DateTime lastReceiveTime = DateTime.MinValue;
       System.Timers.Timer tmr1min = new System.Timers.Timer(1000 * 60*3);
       public TIMCC_SectionManager(String url)
           
    {
        this.url = url;
        //try
        //{
        //    LoadData();
        //}
        //catch (Exception ex)
        //{
        //    ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
        //}

        tmr1min.Elapsed += new System.Timers.ElapsedEventHandler(tmr1min_Elapsed);
        tmr1min.Start();
        LoadData();
	}

           bool  intmr1min = false;
       void tmr1min_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
       {
           try
           {
               if (intmr1min)
                   return;
               intmr1min = true;
               lastReceiveTime = DateTime.Now;
               LoadData();

           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
           }
           finally
           {
               intmr1min = false;
           }
           //throw new Exception("The method or operation is not implemented.");
       }

    public  void LoadData()
    {
      

            SectionTrafficData[] datas;
            try
            {
                datas = TIMCC.TIMCC_Factory.getSectionData(url);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                return;
            }

            Hashtable temp = new Hashtable();
            foreach (SectionTrafficData sec in datas)
            {
                //String key=sec.lineid+"-"+sec.direction;
                if (!temp.ContainsKey(sec.getKey()))
                {
                    temp.Add(sec.getKey(), new ArrayList());
                }
                //else
                    ((ArrayList)temp[sec.getKey()]).Add(sec);

            }
            lock (this.lines)
            {
                this.lines.Clear();
            foreach (String key in temp.Keys)
            {
                SectionTrafficData[] ds = new SectionTrafficData[((ArrayList)temp[key]).Count];

                for (int i = 0; i < ds.Length; i++)
                    ds[i] = (SectionTrafficData)((ArrayList)temp[key])[i];


                System.Array.Sort(ds);
                lines.Add(key, ds);

            }
        }
	   
    }
	

       public void getTravelDataByRange(String lineid, String dir, int begmile, int endmile,ref int  traveltime, ref int upperlimit,ref int lowerlimit )
       {
          

               traveltime = lowerlimit = upperlimit = 0;
               if (!lines.ContainsKey(lineid + "-" + dir))
                   throw new OutOfLineException();

               if (begmile > endmile)
               {
                   int t = begmile;
                   begmile = endmile;
                   endmile = t;

               }
               SectionTrafficData[] secs;
               lock (this.lines)
               {
                 secs= (SectionTrafficData[])lines[lineid.Trim() + "-" + dir.Trim()];
               }
               bool bfound = false;
               int beginx = 0;

               for (int i = 0; i < secs.Length; i++)
               {
                   int beg_mile = 0, end_mile = 0;
                   if (secs[i].fromMile < secs[i].endMile)
                   {
                       beg_mile = secs[i].fromMile;
                       end_mile = secs[i].endMile;
                   }
                   else
                   {
                       beg_mile = secs[i].endMile;
                       end_mile = secs[i].fromMile;
                   }

                   if (begmile >= beg_mile && begmile < end_mile && endmile > end_mile)
                   {
                       bfound = true;
                       beginx = i;
                       traveltime += secs[i].travelTime * (end_mile - begmile) / (end_mile - beg_mile);
                       lowerlimit += secs[i].lowerLimit * (end_mile - begmile) / (end_mile - beg_mile);
                       upperlimit += secs[i].upperLimit * (end_mile - begmile) / (end_mile - beg_mile);
                       break;
                   }
                   else if (begmile >= beg_mile && begmile < end_mile && endmile <= end_mile)
                   {
                       bfound = true;
                       beginx = i;
                       traveltime += secs[i].travelTime * (endmile - begmile) / (end_mile - beg_mile);
                       lowerlimit += secs[i].lowerLimit * (endmile - begmile) / (end_mile - beg_mile);
                       upperlimit += secs[i].upperLimit * (endmile - begmile) / (end_mile - beg_mile);
                       if (((TimeSpan)(DateTime.Now - SectionTrafficData.timeStamp)).TotalMinutes > 10)
                           traveltime = -1;
                       return;
                   }
               }

               if (!bfound)
               {
                   traveltime = lowerlimit = upperlimit = -1;
                   throw new Exception("Not found at Timcc Xml file!");
               }


               for (int i = beginx + 1; i < secs.Length; i++)
               {
                   int beg_mile = 0, end_mile = 0;
                   if (secs[i].fromMile < secs[i].endMile)
                   {
                       beg_mile = secs[i].fromMile;
                       end_mile = secs[i].endMile;
                   }
                   else
                   {
                       beg_mile = secs[i].endMile;
                       end_mile = secs[i].fromMile;
                   }

                   if (endmile > end_mile)
                   {
                       traveltime += secs[i].travelTime;
                       lowerlimit += secs[i].lowerLimit;
                       upperlimit += secs[i].upperLimit;
                   }

                   if (endmile <= end_mile && endmile >= beg_mile)
                   {
                       traveltime += secs[i].travelTime * (endmile - beg_mile) / (end_mile - beg_mile);
                       lowerlimit += secs[i].lowerLimit * (endmile - beg_mile) / (end_mile - beg_mile);
                       upperlimit += secs[i].upperLimit * (endmile - beg_mile) / (end_mile - beg_mile);
                   }

               }
               if (((TimeSpan)(DateTime.Now - SectionTrafficData.timeStamp)).TotalMinutes > 10)
                   traveltime = -1;
          
       }


    public TravelData[] getTravelData(String lineid,String dir,int mile,int retcnt) 
    {
    	TravelData[] ret;
    	int travelTime=-1;
    	ArrayList  ary=new ArrayList();
    //	ret=new TravelData[retcnt];
    	if(!lines.ContainsKey(lineid+"-"+dir))
    		throw new OutOfLineException();
        int currSecInx=-1;
    	SectionTrafficData[] secs=(SectionTrafficData[])lines[lineid.Trim()+"-"+dir.Trim()];
    	
    	for(int i=0;i<secs.Length;i++)
    	{
            SectionTrafficData sec = (SectionTrafficData)secs[i];
    	
    	   if( mile >=sec.fromMile && mile <sec.endMile || mile <=sec.fromMile && mile >sec.endMile)
    	   {
    		   if(sec.travelTime==-1)
    			   throw new InvalidTrafficDataException();
    		  travelTime=(int)(((double)sec.endMile-mile)/(sec.endMile-secs[i].fromMile)*sec.travelTime);
    		  currSecInx=i;
    		  ary.Add(new TravelData(sec.lineid,secs[i].direction,sec.endLocation,travelTime));
    		  break;
    	   }
    	   
    	}
    	
    	if(currSecInx==-1)
    		throw new OutOfLineException();
    		
    	for(int i = 1;i<retcnt;i++)
    	{
    		if(i+currSecInx>=secs.Length)//超過最後依各交流道
    			break;
    		SectionTrafficData sec=secs[i+currSecInx];
    		if(sec.travelTime==-1)
    			break;
    		
    			travelTime+=   sec.travelTime;
    			ary.Add(new TravelData(sec.lineid,sec.direction,sec.endLocation,travelTime));
    	}
    	
    	ret=new TravelData[ary.Count];
    	for(int i=0;i<ret.Length;i++)
    		ret[i]=(TravelData)ary[i];
    	return ret;
    }
    
    public override String ToString()
    {
    	String ret="";
        ret += "LasteReceiveTime=" + lastReceiveTime.ToString() + "\r\n";
        ret += "IsInTmr=" + intmr1min + "\r\n";
    foreach(String key in this.lines.Keys)
    	foreach(SectionTrafficData sec in  (SectionTrafficData[]) lines[key])
    		ret+=sec.toString()+"\r\n";
    			
    
    return ret;
    }
    }
}
