using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using System.Data.Odbc;
using System.Collections;

namespace Host
{
  public   class RGS_PolygonSectionMapping
    {

      System.Collections.Hashtable hs_g_code_id = new System.Collections.Hashtable();
      public RGS_PolygonSectionMapping()
      {
            loadPolyGonSessionMapping();
      }

      public IEnumerable getSectionVDEnum(int g_code_id, int sectionid)
      {
          lock (this)
          {
              if (!hs_g_code_id.Contains(g_code_id))
                  throw new Exception("can not find g_code_id in PolyGoonSection Mapping table!");
              SESSION_G_CODE g_code = (SESSION_G_CODE)hs_g_code_id[g_code_id];

              return g_code.getSection(sectionid).getVDEnum();
          }

      }


      public int getJamLevel(int g_code_id,int secid)
     {
         lock (this)
         {
             // int ret = 255; 
             int validcnt, jamlevel;

             jamlevel = 0;
             validcnt = 0;
             foreach (string vdname in getSectionVDEnum(g_code_id, secid))
             {


                 try
                 {
                     TC.VDDeviceWrapper vd = (TC.VDDeviceWrapper)Program.matrix.getDeviceWrapper(vdname);
                     if (vd.IsValid)
                     {
                         //  speed += vd.getCurrent5MinAvgData().speed;
                         // occupancy += vd.getCurrent5MinAvgData().occupancy;
                         jamlevel += vd.jamLevel;
                         validcnt++;
                     }

                 }
                 catch (Exception ex)
                 {
                     ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                 }

             }

             if (validcnt == 0)
                 return 255;      //未知
             else
                 return System.Convert.ToInt32((double)jamlevel / (double)validcnt);
         }

     }

      public  void loadPolyGonSessionMapping()
      {
          lock (this)
          {
              ConsoleServer.WriteLine("loading Polygon Session Mapping table!");
              OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
              OdbcCommand cmd = new OdbcCommand("select g_code_id,devicename,section_id from tblrgspolygonsection");
              OdbcDataReader rd = null; ;
              cmd.Connection = cn;
              try
              {
                  hs_g_code_id.Clear();
                  cn.Open();
                  rd = cmd.ExecuteReader();
                  while (rd.Read())
                  {
                      int g_code_id;
                      string vdName;
                      int sectionid;
                      SESSION_G_CODE g_code;
                      g_code_id = System.Convert.ToInt32(rd[0]);
                      vdName = rd[1].ToString();
                      sectionid = System.Convert.ToInt32(rd[2]);

                      if (hs_g_code_id.Contains(g_code_id))
                          g_code =(SESSION_G_CODE) hs_g_code_id[g_code_id];
                      else
                      {
                          g_code = new SESSION_G_CODE(g_code_id);
                          hs_g_code_id.Add(g_code_id,g_code);
                      }
                     // g_code.addSection(new NetworkSection(sectionid));
                      g_code.addSectionVD(sectionid, vdName);


                  }
              }
              catch (Exception ex)
              {
                  ConsoleServer.WriteLine("Loading polygon session mapping fail!!");
                  ConsoleServer.WriteLine(ex.Message);
              }
              finally
              {
                  try
                  {
                      cn.Close();
                  }
                  catch { ;}
              }
          }

     }

    }

    class SESSION_G_CODE
    {
        System.Collections.Hashtable hs_section = new System.Collections.Hashtable();

        int g_code_id;
     internal    SESSION_G_CODE(int g_code_id)
        {
            this.g_code_id = g_code_id;
        }

     //internal void addSection(NetworkSection section)
     //   {
     //       if(hs_section.Contains(section))
     //           return;
     //       hs_section.Add(section.sessionId,  section);
     //   }
      internal void addSectionVD(int sectionid,string vdName)
      {
          NetworkSection sec;
          if (hs_section.Contains(sectionid))
              sec = (NetworkSection)hs_section[sectionid];
          else
          {
              sec = new NetworkSection(sectionid);
              hs_section.Add(sectionid, sec);
          }

          sec.Add(vdName);

      }

        internal NetworkSection getSection(int sectionid)
        {
            if (!hs_section.Contains(sectionid))
                throw new Exception("can not found sectionid in PolyGoonSection Mapping table!");
            return (NetworkSection)hs_section[sectionid];

        }

    }

     class NetworkSection
     {
         System.Collections.ArrayList vdList=new System.Collections.ArrayList();
         internal int sessionId;

         

      internal   NetworkSection(int sectionId)
         {
             this.sessionId=sectionId;
         }

      internal void Add(string vdName)
         {
             vdList.Add(vdName);
         }

      internal System.Collections.IEnumerable getVDEnum()
         {

             foreach (string s in vdList) 
              yield return s;

         }

     }



 }
