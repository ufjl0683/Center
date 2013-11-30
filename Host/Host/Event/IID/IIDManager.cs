using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace Host.Event.IID
{
   public  class IIDManager
    {
       System.Collections.Hashtable hsCams = new System.Collections.Hashtable();
       System.Collections.Hashtable hsRange = new System.Collections.Hashtable();

       public IIDManager()
       {
          
           load_IID_Cam_Data();
       }
       public int GetEventCnt()
       {
           return this.hsCams.Count+hsRange.Count;
       }
       public void load_IID_Cam_Data()
       {

           hsCams.Clear();
           System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
           System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();
           System.Data.Odbc.OdbcDataReader rd;
           cmd.Connection = cn;
           cmd.CommandText = "select devicename,cam_id,direction,mileage,lane_id from tblIIDCamConfig";
           try
           {
               cn.Open();
               rd = cmd.ExecuteReader();
               while (rd.Read())
               {
                   string devname, direction;
                   int camid,laneid, mileage;
                   devname = rd[0].ToString();
                   camid = System.Convert.ToInt32(rd[1]);
                   direction = rd[2].ToString();
                   mileage = System.Convert.ToInt32(rd[3]);
                   laneid = System.Convert.ToInt32(rd[4]);

                   IID_CAM_Data camdata=new IID_CAM_Data("N6", direction, devname, camid, laneid, mileage);
                   camdata.OnEvent += new EventHandler(camdata_OnEvent);
                   hsCams.Add(camdata.Key,camdata);

               }
               rd.Close();
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
           }
           finally
           {
               cn.Close();
           }

           

          // string sql="select camName,
       }

       void camdata_OnEvent(object sender, EventArgs e)
       {
           //throw new Exception("The method or operation is not implemented.");

           try
           {
               IID_CAM_Data camdata = sender as IID_CAM_Data;

               if (!hsRange.Contains(camdata.Key))
               {
                   if (camdata.action != 0)
                   {
                       Range range = new IIDRange(camdata);
                       hsRange.Add(camdata.Key, range);
                       Program.matrix.event_mgr.AddEvent(range);
                   }
               }
               else
               {
                   if (camdata.action == 0)
                   {
                       Range range = hsRange[camdata.Key] as Range;
                       range.invokeStop();
                       hsRange.Remove(camdata.Key);
                   }


               }




           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
           }
           
       }


       public void setCamEvent(int camid,int laneid,int iideventid,int action)
       {
           IID_CAM_Data camdata = hsCams[camid + "-" + laneid] as IID_CAM_Data;
           camdata.setEvent(iideventid,action);
       }

    }
}
