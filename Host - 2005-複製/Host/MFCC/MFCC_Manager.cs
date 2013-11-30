using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using RemoteInterface;

namespace Host.MFCC
{
   public  class MFCC_Manager
    {
        
       System.Collections.Hashtable mfccs=System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
       
     
       public MFCC_Manager()
       {
           OdbcConnection cn = new OdbcConnection(Global.Db2ConnectionString);
           OdbcCommand cmd = new OdbcCommand("select hostid,hostip,mfccid,mfcctype,remoteport from vwhostmfcc");
         
           OdbcDataReader rd;
           try
           {
               string hostid, hostip, mfccid, mfcctype;
               int remoteport;
               cn.Open();
               cmd.Connection = cn;
               rd = cmd.ExecuteReader();
               while (rd.Read())
               {
                   hostid = rd[0] as  string;
                   hostip = rd[1] as  string;
                   mfccid = rd[2] as string;
                   mfcctype = rd[3] as string;
                   remoteport = System.Convert.ToInt32(rd[4]);
                   mfccs.Add(mfccid, new MFCC.MFCC_Object(hostid, hostip, remoteport,mfccid, mfcctype));
                   ConsoleServer.WriteLine("load mfcc:" + mfccid);
               }
              
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
               System.Environment.Exit(-1);
           }
           finally
           {
               cn.Close();
               cn.Dispose();
           }

       }

       public MFCC_Object this[string mfccid]
       {
           get
           {
               if (mfccs.Contains(mfccid))
                   return (MFCC_Object)mfccs[mfccid];
               else
                   return null;//throw new Exception(mfccid + " not found!");
                
           }
       }

    }
}
