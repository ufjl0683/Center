using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using RemoteInterface;

namespace Host.AVI
{
  public  class AVIManager
    {

      
       public   System.Collections.ArrayList sections = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList());
    //   public static int ETagLifeTime;


        static AVIManager()
       {

           //select VARIABLEVALUE from TBLSYSPARAMETER where VARIABLENAME = 'ETAGCOMPARTTIME'
           FetchETagLifeTime();
         


       }


       public static void FetchETagLifeTime()
      {

            OdbcConnection cn = new OdbcConnection(DbCmdServer.getDbConnectStr());
            OdbcCommand cmd = new OdbcCommand("select VARIABLEVALUE from TBLSYSPARAMETER where VARIABLENAME = 'ETAGCOMPARTTIME'");
            cmd.Connection = cn;
           cn.Open();
           TC.AVIDeviceWrapper.ETAGLiveTime  =System.Convert.ToInt32( cmd.ExecuteScalar());
           cn.Close();

      }
      public AVIManager()
      {

          loadSectionData();
          loadAVIData();

          foreach (AVI.AVISection sec in sections)
          {
//#if DEBUG
//              lock (sec)
//              {
//                  System.Threading.Monitor.Pulse(sec);
//              }
//#endif
              sec.calcTravelTime();
          }

      }


          public AVI.AVISection this[string secid]
          {
              get
              {

                  foreach (AVISection sec in sections)
                  {
                      if (sec.secid == secid)
                          return sec;
                  }


                  throw new Exception(secid + "not found");
              }


          }

       public int  GetTrafficSpeed(string lineid, string direction, int mile_m)
        {
            foreach (AVI.AVISection sec in sections)
            {
                if (sec.Direction != direction || sec.LineId!=lineid)
                    continue;
                 int begMile=sec.startTC.mile_m;
                 int endmile=sec.endTC.mile_m;
                if(endmile<begMile)
                {
                    int t=begMile;
                    begMile=endmile;
                    endmile=t;
                }

                if (mile_m >= begMile && mile_m < endmile)
                {


                    return  sec.Speed; ;
                }

            }

           return -1;
          
        }

      private void loadSectionData()
      {
          OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);

          OdbcCommand cmd = new OdbcCommand("select AVISectionID,Start_Devicename,End_DeviceName,upperinterval,lowerinterval,VALID_CNT_1MIN,Start_Devicename_Source,ext_lineid,ext_direction,ext_mile_m,END_DEVICENAME_SOURCE,ISETAGSECTION from tblAVISection  order by  ISETAGSECTION desc  ");


          OdbcDataReader rd;
          cmd.Connection = cn;
          try
          {
              cn.Open();
              rd = cmd.ExecuteReader();
              while (rd.Read())
              {
                  try
                  {
                  string secid = rd[0].ToString();
                  string startDevName = rd[1].ToString();
                  string endDevName = rd[2].ToString();
                   
                  int upperinterval = System.Convert.ToInt32(rd[3]);
                  int lowerinterval = System.Convert.ToInt32(rd[4]);
                  int validcnt = System.Convert.ToInt32(rd[5]);
                  string startAviSource = rd[6].ToString().Trim();
                  string endAviSource = rd[10].ToString().Trim();
                  string ext_lineid="", ext_dir="";
                  bool IsETagSection = System.Convert.ToInt32(rd[11]) == 1 ? true : false;
                  int ext_mile_m = 0;
                  //if (secid == "N3_S_259.114_280.905")
                  //    Console.WriteLine("tets");

                  Host.TC.AVIDeviceWrapper startdev=null;
                  Host.TC.AVIDeviceWrapper enddev = null;
                  if (startAviSource.Trim() != "C")  //非中區自有AVI
                  {
                         
                      ext_lineid = rd[7].ToString();
                      ext_dir = rd[8].ToString();
                      ext_mile_m = System.Convert.ToInt32(rd[9]);
                      startdev = new TC.AVIDeviceWrapper("", startDevName, "ExtAVI", "0.0.0.0", -1, "F", ext_lineid,
                          ext_mile_m, new byte[] { 0, 0, 0, 0 }, 0, 0, ext_dir);
                      enddev = Program.matrix.getDevicemanager()[endDevName] as Host.TC.AVIDeviceWrapper;
                  }
                  else if (endAviSource.Trim() != "C")
                  {
                      ext_lineid = rd[7].ToString();
                      ext_dir = rd[8].ToString();
                      ext_mile_m = System.Convert.ToInt32(rd[9]);
                      startdev = Program.matrix.getDevicemanager()[startDevName] as Host.TC.AVIDeviceWrapper;
                      enddev = new TC.AVIDeviceWrapper("", endDevName, "ExtAVI", "0.0.0.0", -1, "F", ext_lineid,
                          ext_mile_m, new byte[] { 0, 0, 0, 0 }, 0, 0, ext_dir);
                  }
                  else
                  {
                      startdev = Program.matrix.getDevicemanager()[startDevName] as Host.TC.AVIDeviceWrapper;

                      /*  Host.TC.AVIDeviceWrapper*/
                      enddev = Program.matrix.getDevicemanager()[endDevName] as Host.TC.AVIDeviceWrapper;
                  }


          //string secid = "N3_N_244.8_261.4";
          //Comm.TCBase tc = MFCC_AVI.Program.mfcc_avi.getTcManager()["AVI-N3-N-261.4"];
          //Host.TC.AVIDeviceWrapper startdev = new Host.TC.AVIDeviceWrapper("MFCCAVI", tc.DeviceName, "AVI", "", 0, "", "N3", 261400, new byte[0], 0, 0);
          //tc = MFCC_AVI.Program.mfcc_avi.getTcManager()["AVI-N3-N-244.8"];
          //Host.TC.AVIDeviceWrapper enddev = new Host.TC.AVIDeviceWrapper("MFCCAVI", tc.DeviceName, "AVI", "", 0, "", "N3", 244800, new byte[0], 0, 0);
#if DEBUG 
            //      if (secid == "N3_S_259.114_280.905")
#endif
                  sections.Add(new AVISection(secid, startdev, enddev, upperinterval, lowerinterval, validcnt, startAviSource,endAviSource,IsETagSection/*, ext_lineid, ext_dir, ext_mile_m*/));
                 
                  }
                  catch (Exception ex)
                  {
                      ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                  }
           }

          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
          }
          finally
          { cn.Close(); }
      }

      private void loadAVIData()
      {

          OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
#if DEBUG
          OdbcCommand cmd = new OdbcCommand("select DeviceName,TimeStamp,Vehicle_plate from TBLAVIDATA1MIN  where timestamp between '" + RemoteInterface.DbCmdServer.getTimeStampString(System.DateTime.Now.AddMinutes(-20)) + "' and '" + RemoteInterface.DbCmdServer.getTimeStampString(System.DateTime.Now) + "' order by timestamp");

#else
          OdbcCommand cmd = new OdbcCommand("select DeviceName,TimeStamp,Vehicle_plate from TBLAVIDATA1MIN  where timestamp between '" + RemoteInterface.DbCmdServer.getTimeStampString(System.DateTime.Now.AddMinutes(-40)) + "' and '" + RemoteInterface.DbCmdServer.getTimeStampString(System.DateTime.Now) + "' order by timestamp");
#endif 
          OdbcDataReader rd;
          cmd.Connection = cn;
          try
          {
              cn.Open();
              rd = cmd.ExecuteReader();
              while (rd.Read())
              {
                  string devName = rd[0].ToString();
                  DateTime dt = System.Convert.ToDateTime(rd[1]);
                  string plate = rd[2].ToString();
                  this.AddAviData(new RemoteInterface.MFCC.AVIPlateData(devName,dt,plate));
              }
          }
          catch (Exception ex)
          {
             ConsoleServer.WriteLine(ex.Message+","+ex.StackTrace);
          }
          finally
          {
              cn.Close();
          }

      }
      public void AddAviData(RemoteInterface.MFCC.AVIPlateData data)
      {
          foreach(AVI.AVISection sec in sections)
          {

              try
              {
                  sec.AddAviData(data);
              }
              catch (Exception ex)
              {
                  ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
              }

              //if (sec.startTC.devicename == data.DevName)
              //    sec.startTC.AddPlate(data);
              //else if (sec.endTC.devicename == data.DevName)
              //{
              //    sec.endTC.AddPlate(data);
              //    if (sec.startTC.Match(data.plate))
              //    {
                     

              //        TimeSpan ts = data.dt-sec.startTC.GetPlateData(data.plate).dt;
              //        int speed =(int)( System.Math.Abs(sec.endTC.mile_m - sec.startTC.mile_m) / ts.TotalSeconds*3600/1000);
              //        RemoteInterface.ConsoleServer.WriteLine("******"+data.plate + " match!speed="+speed+"km*************");
              //    }
              //}
          }
      }


      public override string ToString()
      {
          string ret = "";

          foreach (AVI.AVISection sec in sections)
          {
             
              ret += sec.secid + ",spd:" + sec.Speed + ",traveltime:" + sec.TravelTime + ",variance:" + sec.Variance + ",upperinterval:" + sec.upperinterval+",lowerinterval:"+sec.lowerinterval+"\r\n";
          }
          return ret;
          
          //return base.ToString();
      }
    }
    }

