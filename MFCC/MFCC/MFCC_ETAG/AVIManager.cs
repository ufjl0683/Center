using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using RemoteInterface;

namespace Host.AVI
{
  public  class AVIManager
    {
      System.Collections.ArrayList sections = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList());
      public AVIManager()
      {
          loadSectionData();
          loadAVIData();

          foreach (AVI.AVISection sec in sections)
          {
              sec.calcTravelTime();
          }
      }

      private void loadSectionData()
      {
          //OdbcConnection cn = new OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
          //OdbcCommand cmd = new OdbcCommand("select AVISectionID,Start_Devicename,End_DeviceName from tblAVISection");
          //OdbcDataReader rd;
          //cmd.Connection = cn;
          //try
          //{
              //cn.Open();
              //rd = cmd.ExecuteReader();
              //while (rd.Read())
              //{
              //    string secid = rd[0].ToString();
              //    string startDevName = rd[1].ToString();
              //    string endDevName = rd[2].ToString();

          string secid = "N3_N_244.8_261.4";
          Comm.TCBase tc = MFCC_ETAG.Program.mfcc_etag.getTcManager()["AVI-N3-N-261.4"];
          Host.TC.AVIDeviceWrapper startdev = new Host.TC.AVIDeviceWrapper("MFCCAVI", tc.DeviceName, "AVI", "", 0, "", "N3", 261400, new byte[0], 0, 0);
          tc = MFCC_ETAG.Program.mfcc_etag.getTcManager()["AVI-N3-N-244.8"];
          Host.TC.AVIDeviceWrapper enddev = new Host.TC.AVIDeviceWrapper("MFCCAVI", tc.DeviceName, "AVI", "", 0, "", "N3", 244800, new byte[0], 0, 0);

          sections.Add(new AVISection(secid, startdev, enddev));
          //    }

          //}
          //catch (Exception ex)
          //{
          //}
          //finally
          //{ cn.Close(); }
      }

      private void loadAVIData()
      {

          OdbcConnection cn = new OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
          OdbcCommand cmd = new OdbcCommand("select DeviceName,TimeStamp,Vehicle_plate from TBLAVIDATA1MIN  where timestamp between '" + Comm.DB2.Db2.getTimeStampString(System.DateTime.Now.AddMinutes(-120)) + "' and '" + Comm.DB2.Db2.getTimeStampString(System.DateTime.Now )+ "' order by timestamp");
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

              sec.AddAviData(data);
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
    }
}
