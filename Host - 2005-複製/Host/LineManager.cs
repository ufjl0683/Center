using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace Host
{
  public   class LineManager
    {
      System.Collections.Hashtable hsLines = new System.Collections.Hashtable();
      public LineManager()
      {
          loadLine();

      }


      public Line this[string lineid]
      {
          get
          {
              if(!hsLines.Contains(lineid))
                  throw new Exception(lineid+"未載入!");
              lock(this)

              return (Line)hsLines[lineid];
        
          }
        
      }


      public System.Collections.IEnumerable getLineEnum()
      {
        System.Collections.IDictionaryEnumerator ie=  hsLines.GetEnumerator();

        while (ie.MoveNext())
            yield return ie.Value;
      }

      void loadLine()
      {
          lock (this)
          {
              System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Global.Db2ConnectionString);
              try
              {
                  hsLines.Clear();
#if !DEBUG
                  System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select lineid,linename,direction,startmileage,endmileage from tblGroupLine where enable='Y' ");
#else
                   System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select lineid,linename,direction,startmileage,endmileage from tblGroupLine where enable='Y' and lineid='N6' ");
                 // System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select lineid,linename,direction,startmileage,endmileage from tblGroupLine where enable='Y' ");
  
#endif
                  cmd.Connection = cn;
                  cn.Open();
                  System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();

                  while (rd.Read())
                  {
                      string linename, lineid, direction;
                      int startmileage, endmileage;

                      lineid = System.Convert.ToString(rd[0]);
                     // if (lineid != "N99") continue;
                      linename = System.Convert.ToString(rd[1]);
                      direction = System.Convert.ToString(rd[2]);
                      startmileage = System.Convert.ToInt32(rd[3]);
                      endmileage = System.Convert.ToInt32(rd[4]);
                      hsLines.Add(lineid, new Line(lineid, linename, direction, startmileage, endmileage));

                  }
              }
              catch (Exception ex)
              {
                  ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
              }
              finally
              {
                  cn.Close();
              }

          }
         

      }

    }
}
