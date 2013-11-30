using System;
using System.Collections.Generic;
using System.Text;

namespace MFCC_ETTU
{
   public    class ETStatus
    {
       byte[] f;
       byte[] x;
       string m_devicename;
      // string m_telcode;
       public ETStatus(string deviceName, byte[] f, byte[] x)
       {
           this.f = f;
           this.x = x;
           m_devicename = deviceName;
          // this.m_telcode = telcode;

       }

       public string TelCode
       {
           get
           {
               return m_devicename.Substring(3);
           }
       }
       public string DeviceName
       {
           get
           {
               return m_devicename;
           }
       }

       public void SetEtStatus(DateTime dt, byte[] x)
       {

           bool isChange = false;

           byte[] xor_x = new byte[x.Length];


           for (int i = 0; i < x.Length; i++)
           {
               xor_x[i] = (byte)(this.x[i] ^ x[i]);
               if (xor_x[i] != 0)
                   isChange = true;
           }

           if (isChange)
           {
               string sql = "update tblETConfig set x1={0},x2={1} where devicename='{2}'";
               Program.mfcc_ettu.ExecuteSql(string.Format(sql, m_devicename, x[0], x[1], m_devicename));
           }
           string timeStamp = Comm.DB2.Db2.getTimeStampString(dt);
         //  System.Collections.BitArray xorBitmap = new System.Collections.BitArray(xor_x);
         //  System.Collections.BitArray xBitmap = new System.Collections.BitArray(x);
          
           for (int i = 0; i < xor_x.Length; i++)
           {

               string sql = "insert into tblEtStatelog (devicename,timestamp,type,failno,result) values('{0}','{1}','{2}',{3},{4})";
               if (xor_x[i]!=0)
               {
                   Program.mfcc_ettu.ExecuteSql(string.Format(sql, this.m_devicename, timeStamp, "S", i, x[i]));

               }

           }


       }
       public void SetEttuFail(DateTime dt, byte[] f)
       {
           bool isChange = false;

           byte[] xor_f = new byte[f.Length];
        
           try
           {
               for (int i = 0; i < f.Length; i++)
               {
                   xor_f[i] = (byte)(this.f[i] ^ f[i]);
                   if (xor_f[i] != 0)
                       isChange = true;
               }



               if (isChange)
               {
                   string sql = "update tblETConfig set f1={0},f2={1},f3={2} where devicename='{3}'";
                   Program.mfcc_ettu.ExecuteSql(string.Format(sql,  f[0], f[1], f[2], m_devicename));
               }
               string timeStamp = Comm.DB2.Db2.getTimeStampString(dt);
               System.Collections.BitArray xorBitmap = new System.Collections.BitArray(xor_f);
               System.Collections.BitArray fBitmap = new System.Collections.BitArray(f);
               for (int i = 0; i < xor_f.Length * 8; i++)
               {

                   string sql = "insert into tblEtStatelog (devicename,timestamp,type,failno,result) values('{0}','{1}','{2}',{3},{4})";
                   if (xorBitmap.Get(i))
                   {
                       Program.mfcc_ettu.ExecuteSql(string.Format(sql, this.m_devicename, timeStamp, "F", i, fBitmap.Get(i) ? 1 : 0));


                   }

               }

             
           }
           catch (Exception ex)
           {
               RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
           }
           finally
           {
               this.f = f;
               this.x = f;
           }

          
       }







   }
}
