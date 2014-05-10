using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace Host
{
    public   class OneMinTask
    {

        public OneMinTask()
        {
            System.DateTime dt = DateTime.Now.AddSeconds(-DateTime.Now.Second).AddMinutes(1).AddSeconds(30); ;

            //int m_sec = (int)((TimeSpan)(dt - DateTime.Now)).TotalMilliseconds;
            //if (m_sec <= 0)
            //    m_sec = 1000;
            //System.Threading.Thread.Sleep(m_sec);
#if !DEBUG
            new System.Threading.Thread(OneMinTask_Elapsed).Start();
#endif
        }

        void OneMinTask_Elapsed()
        {

           System.DateTime dt = System.DateTime.Now,dtbeg;

            while (true)
            {


                try
                {
                   dtbeg = dt = System.DateTime.Now;

                    try
                    {



                        TotblTrafficDataLogUnit(dt.AddSeconds(-dt.Second));

                    }
                    catch(Exception ex) {
                        ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                        
                        ;}

                    try
                    {


                        TotblTrafficDataLogSection(dt.AddSeconds(-dt.Second));

                    }
                    catch (Exception ex)
                    {
                        ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);

                     
                    }

                  //  ConsoleServer.WriteLine("1 min section and unit traffic log time cost:" + ((TimeSpan)(System.DateTime.Now - dtbeg)).TotalSeconds + "s");

                    try
                    {
                        Program.matrix.route_mgr.FetchTravelTime();
                        //ConsoleServer.WriteLine("**********************************************");
                        //ConsoleServer.WriteLine(Program.matrix.route_mgr.ToString());
                        //ConsoleServer.WriteLine("**********************************************");
                    }
                    catch (Exception ex)
                    {
                        ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                    }

                    try
                    {
                        Program.matrix.route_mgr74.FetchTravelTime();
                        //ConsoleServer.WriteLine("**********************************************");
                        //ConsoleServer.WriteLine(Program.matrix.route_mgr.ToString());
                        //ConsoleServer.WriteLine("**********************************************");
                    }
                    catch (Exception ex)
                    {
                        ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                    }

                    // System.DateTime dt = System.DateTime.Now;
                    //  dt = System.DateTime.Now;
                    dt = DateTime.Now.AddSeconds(-DateTime.Now.Second).AddMinutes(1).AddSeconds(30);

                    int m_sec = (int)((TimeSpan)(dt - DateTime.Now)).TotalMilliseconds;
                    if (m_sec <= 0)
                        m_sec = 1000;
                    System.Threading.Thread.Sleep(m_sec);
                }
                catch(Exception ex1)
                {
                    ConsoleServer.WriteLine(ex1.Message + "," + ex1.StackTrace);
                    ;
                }

            }



        }

        void TotblTrafficDataLogUnit(System.DateTime dt)
        {
            string sqlStr = "";

            foreach (Line line in Program.matrix.line_mgr.getLineEnum())
            {

                foreach (UnitRoad road in line.getAllUnitRoadEnum())
                {
                 try{

                    string[] vdlist = null;
                    int speed = 0, occupancy = 0, jamLevel = 0, travelTime = 0, volume = 0, lowerTravelTime = 0, UpperTravelTime = 0;

                    road.getAllVDTrafficData(ref vdlist, ref volume, ref speed, ref occupancy, ref jamLevel/*, ref travelTime*/,-1,-1);
                    travelTime = road.getTravelTime();
                    lowerTravelTime = road.getLowerTravelTimeLimit();
                    UpperTravelTime = road.getUpperTravelTimeLimit();

                    sqlStr = "insert into tblTrafficDataLogUnit (unitid,timestamp,car_volume,car_speed,AVERAGE_OCCUPANCY,level,travelTime,VDList,uppertraveltime,lowertraveltime) values('{0}', TIMESTAMP('{1}'),{2},{3},{4},{5},{6},'{7}',{8},{9}) ";
                    
#if !DEBUG
                        Program.matrix.dbServer.SendSqlCmd(string.Format(sqlStr, road.unitid, RemoteInterface.DbCmdServer.getTimeStampString(dt), volume, speed, occupancy, jamLevel, travelTime, string.Join(",", vdlist), UpperTravelTime, lowerTravelTime));
#endif                    
                        }
                    catch (Exception ex)
                    {
                        ConsoleServer.WriteLine(road.unitid + "," + ex.Message + ex.StackTrace);
                    }

                }




            }




        }



        void TotblTrafficDataLogSection(System.DateTime dt)
        {
            int seccnt = 0;
            int totalsec = 0,weight0cnt=0;
            foreach (Line line in Program.matrix.line_mgr.getLineEnum())
            {
                totalsec += line.getTotalSecCnt();
                foreach (Section section in line.getAllSectionEnum())
                {
                    //   string[] vdlist = null;
                    int speed = 0, occupancy = 0, jamLevel = 0, travelTime = 0, volume = 0, lowerTravelTime = 0, upperTravelTime = 0,vdspeed=0;

                    string sqlstr;
                    sqlstr = "insert into tblTrafficDataLogSection (sectionid,timestamp,car_volume,car_speed,AVERAGE_OCCUPANCY,level,travelTime,uppertravelTime,lowertravelTime,vd_travel_time,vd_travel_percent,avi_travel_time,avi_travel_percent,etc_travel_time,etc_travel_percent,htd_travel_time,htd_travel_percent) values('{0}', TIMESTAMP('{1}'),{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}) ";


                    try
                    {
                        section.getAllTrafficData(ref volume, ref speed, ref occupancy, ref jamLevel, ref travelTime, ref lowerTravelTime, ref upperTravelTime);
                        int vdtime,avitime,etctime,histime,weightid,vdpercent=100,avipercent=0,etcpercent=0,hispercent=0;//etctime,histime;
                        try
                        {
                            vdtime = section.getVD_TravelTime();
                        }
                        catch { vdtime = -1; }

                        try
                        {
                            avitime = section.getAVI_TravelTime();
                        }
                        catch { avitime = -1; }

                        try
                        {
                            etctime = section.getETC_TravelTime();
                        }
                        catch { etctime = -1; }

                        try
                        {
                            histime = section.getHIS_TravelTime();
                        }
                        catch { histime = -1; }

                        weightid = 0;
                        if (vdtime >=0)
                            weightid += 1;
                        if (avitime >= 0)
                            weightid += 2;

                         if (etctime >= 0)
                            weightid += 4;

                        if (histime >= 0)
                            weightid += 8;

                        if (weightid == 0)
                        {
                            weight0cnt++;
                            
                           // continue;
                        }
                       
                        section.getTravelTimeWeight(weightid, ref vdpercent, ref avipercent, ref etcpercent, ref hispercent);

                        travelTime = (vdtime * vdpercent + avitime * avipercent+etctime*etcpercent+histime*hispercent) / 100;
                        // 路段速度以VD時間計算


                        // 空間速度
                        //if (travelTime != -1 && travelTime!=0)



                        if (vdtime > 0)
                        {
                            vdspeed = System.Convert.ToInt32((double)System.Math.Abs(section.endMileage - section.startMileage) / vdtime * 3.6);
                            speed = section.getVDSpaceSpeed();
                        }
                        // speed = System.Convert.ToInt32((double)System.Math.Abs(section.endMileage - section.startMileage) / vdtime * 3.6);
                        else
                        {
                            vdspeed = -1;
                            speed = -1;
                        }

                       // ConsoleServer.WriteLine(section.ToString()+" spacespd:"+speed +" vdspd="+vdspeed);

#if !DEBUG
                        Program.matrix.dbServer.SendSqlCmd(string.Format(sqlstr, section.sectionId, RemoteInterface.DbCmdServer.getTimeStampString(dt), volume, speed, occupancy, jamLevel, travelTime, upperTravelTime, lowerTravelTime, travelTime, vdpercent, avitime, avipercent, etctime, etcpercent, histime, hispercent));
                        seccnt++;
#endif
                    }
                    catch (Exception ex)
                    {

                       Util.SysLog("sys.log", ex.Message + "," + ex.StackTrace);
                        ConsoleServer.WriteLine(section.sectionId + "," + ex.Message + ex.StackTrace);
                    }


                }
            }

         //  RemoteInterface.Util.SysLog("seccnt.log", System.DateTime.Now.ToString() + "," + seccnt+"/"+totalsec +",weight0 cnt="+weight0cnt);
        }

    }
}
