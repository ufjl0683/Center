using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;

namespace Host.CCTV
{

  
     public class CCTV_Manager
    {
         public LockWindows[] lockwindows;

         int lockinx = 0;
         public CCTV_Manager()
         {
             lockwindows = new LockWindows[4] { new LockWindows(2), new LockWindows(3), new LockWindows(6), new LockWindows(7) };
             //for (int i = 2; i <= 9; i++)
             //    lockwindows[i-2] = new LockWindows(i);

             

         }

         public void setLock(int cctvid, string desc, string desc2, int preset)
         {
             lockinx = 1;  //定住在第2個視窗
             lockwindows[lockinx].setLock(cctvid, desc, desc2, preset);
             lockinx = (lockinx + 1) % lockwindows.Length;
         }

         public void setETTULock(string ettuid)
         {
             int cctvid = -1;
             int preset = -1;
             string desc1 = ettuid , desc2 = " ";
             this.getCCTVDataByDeviceName(ettuid, ref cctvid, ref preset);
           //  desc1 = evt.EventId.ToString() + "_" + evt.description;

             //  byte[] desc2code= System.Text.Encoding.Unicode.GetBytes(evt.ToString());
             // desc2code=System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, desc2code, 0, desc2code.Length);
           //  desc2 = System.Web.HttpUtility.HtmlEncode(desc2);
             if (cctvid == -1 || preset == -1)
                 return;


             //foreach (LockWindows lckwnd in lockwindows)
             //{
             //    if (lckwnd.evt == null)
             //        continue;
             //    if (lckwnd.evt.OrgEventId == evt.OrgEventId)
             //    {
             //        lckwnd.setLock(evt, cctvid, desc1, desc2, preset);
             //        return;
             //    }
             //}



             //  lockinx = 1;  //定住在第2個視窗
             lockwindows[lockinx].setLock( cctvid, desc1, desc2, preset);
             lockinx = (lockinx + 1) % lockwindows.Length;

         }

         public void setLock(Event.Event evt)
         {
             int cctvid = -1;
             int preset = -1;
             string desc1="", desc2="";
             this.getCCTVDataByDeviceName(evt.getDeviceName(),ref cctvid,ref preset);
             desc1 = evt.EventId.ToString();// +"_" + evt.description;

           //  byte[] desc2code= System.Text.Encoding.Unicode.GetBytes(evt.ToString());
            // desc2code=System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, desc2code, 0, desc2code.Length);
             desc2 = " ";// System.Web.HttpUtility.HtmlEncode(evt.ToEventString());
             if (cctvid == -1 || preset == -1)
                 return;
             foreach (LockWindows lckwnd in lockwindows)
             {
                 if (lckwnd.evt == null)
                     continue;
                 if (lckwnd.evt.OrgEventId == evt.OrgEventId)
                 {
                     lckwnd.setLock(evt, cctvid, desc1, desc2, preset);
                     return;
                 }
             }



           //  lockinx = 1;  //定住在第2個視窗
             lockwindows[lockinx].setLock(evt, cctvid, desc1, desc2, preset);
             lockinx = (lockinx + 1) % lockwindows.Length;
             }
        //public  void setLock(int cctvid, string desc1, string desc2, int preset)
        // {  



        // }

         //void setLock(string devName,int cctvid, string desc1, string desc2)
         //{


         //   // lockwindows[lockinx++].setLock(cctvid, desc1, desc2, preset);
         //}


         public void unlock(Event.Event evt)
         {
             foreach (LockWindows lckwnd in lockwindows)
             {
                 if (lckwnd.evt == null)
                     continue;
                 if (lckwnd.evt == evt)
                     lckwnd.unlock();
             }

         }
         public void getCCTVDataByDeviceName(string devName, ref int cctvid, ref int preset)
         {
             OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
             OdbcCommand cmd=new OdbcCommand("select CAMERA_ID,PRESET_ID  from  tblCCTVPreset where devicename='"+devName+"'");
             cmd.Connection = cn;
             OdbcDataReader rd;
             try
             {
                 cn.Open();
                 rd = cmd.ExecuteReader();
                 if (rd.Read())
                 {

                     cctvid = System.Convert.ToInt32(rd[0]);
                     preset = System.Convert.ToInt32(rd[1]);
                 }
                 else
                 {
                     cctvid = -1;
                     preset = -1;
                 }
             }
             catch (Exception ex)
             {
                 RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
             }
             finally
             {
                 cn.Close();
             }
         }

         public  LockWindows this[int i]
         {

             get
             {
                 return lockwindows[i];
             }
          
         }


    }
}
