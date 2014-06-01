using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using System.Data.Odbc;
using System.Collections;

namespace Host
{

    public delegate void OnRGS_Metro_Jam_Change_Handler(string rgs_name,int level,int g_code_id);


  public   class RGS_PolygonSectionMapping
    {
      public event OnRGS_Metro_Jam_Change_Handler OnMetroJamChangeEvent;
      System.Collections.Hashtable hs_g_code_id = new System.Collections.Hashtable();
      System.Collections.Generic.Dictionary<string,RGS_MainLineVD> hs_rgs_main_vd= new System.Collections.Generic.Dictionary<string,RGS_MainLineVD>();
      System.Threading.Thread thCheckLevel;
      public RGS_PolygonSectionMapping()
      {
          loadPolyGonSessionMapping();
          loadRGSMainVDData();
          thCheckLevel = new System.Threading.Thread(CheckLevelTask);
          thCheckLevel.Start();
      }

      void CheckLevelTask()
      {
          while (true)
          {
              try
              {
                 
                  foreach (RGS_MainLineVD rgsvdmaindata in hs_rgs_main_vd.Values)
                  {
                      rgsvdmaindata.ChechChange();
                  }
                  System.Threading.Thread.Sleep(1000*30);

              }
              catch(Exception ex)
              {
                  Console.WriteLine(ex.Message+","+ex.StackTrace); ;
              }


          }
      }
      void  loadRGSMainVDData()
      {

            ConsoleServer.WriteLine("loading Polygon Session Mapping table!");
              OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
              OdbcCommand cmd = new OdbcCommand("select rgsdevicename,devicename,g_code_id from vwrgspolygonsection where location<>'R'");
              OdbcDataReader rd = null; ;
              cmd.Connection = cn;
              try
              {
                   this.hs_rgs_main_vd.Clear();
                  cn.Open();
                  rd = cmd.ExecuteReader();
                  while (rd.Read())
                  {
                      string rgs_name,vdname;
                      int g_code_id=0;
                      rgs_name=rd[0].ToString();
                      vdname=rd[1].ToString();
                      g_code_id = System.Convert.ToInt32(rd[2]);
                      if (hs_rgs_main_vd.ContainsKey(rgs_name))
                      {
                          hs_rgs_main_vd[rgs_name].AddVD(vdname);
                      }
                      else
                      {
                          try
                          {
                              RGS_MainLineVD rgsvd = new RGS_MainLineVD(rgs_name, g_code_id);

                              rgsvd.AddVD(vdname);
                              rgsvd.On_RGS_Main_VD_Jam += new OnRGS_Metro_Jam_Change_Handler(rgsvd_On_RGS_Main_VD_Jam);
                              hs_rgs_main_vd.Add(rgs_name, rgsvd);
                          }
                          catch(Exception ex1)
                          {
                              ConsoleServer.WriteLine(ex1.Message+ex1.StackTrace) ;
                          }
                      }

                   //  hs_rgs_main_vd.Add(rgs_name,new RGS_MainLineVD(){ RGS_name=rgs_name,VD_name=vdname});

                  }
              }
          catch(Exception ex)
              {
                    ConsoleServer.WriteLine(ex.Message+","+ex.StackTrace);
              }
          finally
              {
                  cn.Close();
              }

      }

      void rgsvd_On_RGS_Main_VD_Jam(string rgs_name, int level,int g_code_id)
      {
          //throw new NotImplementedException();
          if (this.OnMetroJamChangeEvent != null)
              this.OnMetroJamChangeEvent(rgs_name, level,g_code_id);
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


  //   bool IsMetroEvent(


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

    public class RGS_MainLineVD{

       public  string RGS_name;
       public  System.Collections.Generic.List<Host.TC.VDDeviceWrapper> VdDevs=new List<TC.VDDeviceWrapper>();
       public event OnRGS_Metro_Jam_Change_Handler On_RGS_Main_VD_Jam;
       int _level=0;
       int g_code_id;
       public RGS_MainLineVD(string rgsname,int g_code_id)
       {
           RGS_name = rgsname;
           this.g_code_id=g_code_id;
       }
       public void AddVD(string VDName)
       {
           VdDevs.Add(Program.matrix.device_mgr[VDName] as TC.VDDeviceWrapper);

           
       }

       public void ChechChange()
       {
           int currLevel = getLevel();
           if (currLevel != _level)
           {
               this._level = currLevel;
               if (this.On_RGS_Main_VD_Jam != null)
                   this.On_RGS_Main_VD_Jam(this.RGS_name,currLevel,this.g_code_id);
           }
       }
       public int  getLevel()
       {
             
               if (this.VdDevs.Count == 0)
                   return 0;
               int validcnt = 0, jamcnt = 0;
               foreach (Host.TC.VDDeviceWrapper vd in VdDevs)
               {
                  if(vd.IsConnected) validcnt++;
                  if (vd.jamLevel > 0)
                      jamcnt++;


               }

               if (validcnt == 0)
                   return 0;

               return (jamcnt > validcnt / 2) ? 1 : 0;

      }


      

    }

 }
