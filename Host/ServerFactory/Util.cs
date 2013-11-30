using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.InteropServices;
using System.IO;
namespace RemoteInterface
{
 public    class Util
    {


        static object filelockobj=new object();
         public static void Log(string pathfile, string message)
         {
             try
             {
                 lock (filelockobj)
                 {
                     System.IO.File.AppendAllText(pathfile, message);
                 }
             }
             catch (Exception ex)
             {
                 ConsoleServer.WriteLine(ex.Message+","+ex.StackTrace);
             }
         }


         public static void SysLog(string filename, string message)
         {

             Util.Log(Util.CPath(AppDomain.CurrentDomain.BaseDirectory + filename),DateTime.Now+","+ message+"\r\n");
         }

        public static string CPath(string WinPath)
        {


            if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
                return WinPath;
            else
            {
                //  Console.WriteLine("Unix");
                return WinPath.Replace(@"\", @"/");
            }
        }

        public static string ToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(100);

            for (int i = 0; i < data.Length; i++)
                sb.Append(string.Format("{0:X2} ", data[i]));

            return sb.ToString();
        }
        public static string ToHexString(byte data)
        {
            return string.Format("{0:X2}", data);
        }

        public static string ToColorString(System.Drawing.Color[] colors)
        {
            string ret = "";
            for (int i = 0; i < colors.Length; i++)
            {
                ret += colors[i].R.ToString() + "," + colors[i].G.ToString() + "," + colors[i].B.ToString() + ",";
            }
            return ret.TrimEnd(new char[] { ',' });

        }

        public static System.Drawing.Color[] ToColors(string colorstr)
        {
            string[] colorStr = colorstr.Split(new char[] { ',' });
            System.Drawing.Color[] colors = new System.Drawing.Color[colorStr.Length / 3];
            for (int i = 0; i < colors.Length; i++)

                colors[i] = System.Drawing.Color.FromArgb(System.Convert.ToInt32(colorStr[i * 3]), System.Convert.ToInt32(colorStr[i * 3 + 1]), System.Convert.ToInt32(colorStr[i * 3] + 2));


            return colors;
        }

        public static System.Data.DataTable getPureDataTable(System.Data.DataTable srctbl)
        {
            System.Data.DataTable retTable = new DataTable(srctbl.TableName);

            foreach (System.Data.DataColumn c in srctbl.Columns)
            {
                System.Data.DataColumn col = retTable.Columns.Add(c.ColumnName);
                col.DataType = c.DataType;

            }

            foreach (System.Data.DataRow r in srctbl.Rows)
            {
                System.Data.DataRow row = retTable.NewRow();
                for (int i = 0; i < r.ItemArray.Length; i++)
                    row[i] = r[i];
                row.RowError = r.RowError;

                retTable.Rows.Add(row);
            }
            retTable.AcceptChanges();
            return retTable;

        }
        public static DataSet getPureDataSet(DataSet ds)
        {
            DataSet retDs = new DataSet();
            foreach (System.Data.DataTable tbl in ds.Tables)
            {
                System.Data.DataTable table = retDs.Tables.Add(tbl.TableName);
                foreach (System.Data.DataColumn c in tbl.Columns)
                {
                    System.Data.DataColumn col = table.Columns.Add(c.ColumnName);
                    col.DataType = c.DataType;

                }

                foreach (System.Data.DataRow r in tbl.Rows)
                {
                    System.Data.DataRow row = table.NewRow();
                    for (int i = 0; i < r.ItemArray.Length; i++)
                        row[i] = r[i];
                    row.RowError = r.RowError;
                    table.Rows.Add(row);
                }
            }
            retDs.AcceptChanges();
            return retDs;

        }

        public static DataTable getPureDataTable(System.Data.DataRow[] rows)
        {
            if (rows.Length == 0)
                throw new Exception("rows 長度為零");
            System.Data.DataTable retTable = new DataTable(rows[0].Table.TableName);

            foreach (System.Data.DataColumn c in rows[0].Table.Columns)
            {
                System.Data.DataColumn col = retTable.Columns.Add(c.ColumnName);
                col.DataType = c.DataType;

            }

            foreach (System.Data.DataRow r in rows)
            {
                System.Data.DataRow row = retTable.NewRow();
                for (int i = 0; i < r.ItemArray.Length; i++)
                    row[i] = r[i];
                row.RowError = r.RowError;
                retTable.Rows.Add(row);
            }
            retTable.AcceptChanges();
            return retTable;
        }

        [DllImport("Kernel32.dll")]
        private extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("Kernel32.dll")]
        private extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

     [DllImport("Kernel32.dll")]
     private extern static uint SetLocalTime(ref SYSTEMTIME lpSystemTime);


        private struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        //private void GetTime()
        //{
        //    // Call the native GetSystemTime method
        //    // with the defined structure.
        //    SYSTEMTIME stime = new SYSTEMTIME();
        //    GetSystemTime(ref stime);

        //    // Show the current time.           
        //    MessageBox.Show("Current Time: " +
        //        stime.wHour.ToString() + ":"
        //        + stime.wMinute.ToString());
        //}


        public static void SetSysTime(ushort year, ushort mon, ushort day, ushort hour, ushort min, ushort sec)
        {
            if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
                SetWinsTime(year, mon, day, hour, min, sec);
            else
                SetUnixTime(year, mon, day, hour, min, sec);

        }
     public static void SetSysTime(System.DateTime dt)
     {
         if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
             SetWinsTime((ushort)dt.Year, (ushort)dt.Month, (ushort)dt.Day,(ushort) dt.Hour, (ushort)dt.Minute,(ushort) dt.Second);
         else
             SetUnixTime((ushort)dt.Year, (ushort)dt.Month, (ushort)dt.Day, (ushort)dt.Hour, (ushort)dt.Minute, (ushort)dt.Second);

     }
        private static void SetWinsTime(ushort year, ushort mon, ushort day, ushort hour, ushort min, ushort sec)
        {
            // Call the native GetSystemTime method
            // with the defined structure.
          
                SYSTEMTIME systime = new SYSTEMTIME();
                //  GetSystemTime(ref systime);
                systime.wYear = year;
                systime.wMonth = mon;
                systime.wDay = day;
                systime.wHour = hour;
                systime.wMinute = min;
                systime.wSecond = sec;
                // Set the system clock ahead one hour.

                SetLocalTime(ref systime);
           
            
        }
        private static  void SetUnixTime(ushort year, ushort mon, ushort day, ushort hour, ushort min, ushort sec)
        {
           // throw new Exception("SetUnixTime not implemented!");
          //  System.DateTime d = System.DateTime.Now;
            System.Diagnostics.Process.Start(string.Format("sudo date {0:00}{1:00}{2:00}{3:00}{4:00}.{5:00}",mon,day,hour,min,year%100,sec));
        }

        public static string getObjectHexString(object obj)
        {
        

           
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
         //  System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            bf.Serialize(ms, obj);

            byte[] data = new byte[ms.Position];
            System.Array.Copy(ms.GetBuffer(), data, data.Length);
            string hexstr = Comm.V2DLE.ToHexString(data).Replace(" ","");
            return hexstr.Trim();

           

        
        }

        public static Object getObjectByHexString(string objHexStr)
        {
            System.IO.MemoryStream ms;
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            ms = new System.IO.MemoryStream(Util.ToBytes(objHexStr));

            return bf.Deserialize(ms);
        }
        public static byte[] ToBytes(string hexstr)
        {
           // string[] dats = hexstr.Trim().Split(new char[] { ' ' });
            byte[] data = new byte[hexstr.Length/2];
            for (int i = 0; i < data.Length; i++)
                data[i] = System.Convert.ToByte(hexstr[i*2].ToString()+hexstr[i*2+1].ToString(), 16);
            return data;
        }

     public static void GC()
     {
         System.GC.Collect();
         System.GC.WaitForPendingFinalizers();
         for (int i = 0; i < 1000; i++)
             System.Diagnostics.Debug.Print("collect!!") ;
     }


     public static byte[] StringToUTF8Bytes(string text)
     {
       return  System.Text.Encoding.Convert(System.Text.UnicodeEncoding.Unicode, System.Text.UTF8Encoding.UTF8, System.Text.UnicodeEncoding.Unicode.GetBytes(text));
     }
     public static byte[] StringToBig5Bytes(string text)
     {
       

          //byte[] b = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode.GetBytes(text));
          //return b;

          char[] chars = text.ToCharArray();
          byte[] b;
          System.IO.MemoryStream ms = new System.IO.MemoryStream();
          for (int i = 0; i < text.Length; i++)
          {

              if (chars[i] >= 0xe000 && chars[i] <= 0xE4BE)
              {
                  ms.WriteByte((byte)((chars[i] + 0x1A40) / 256));
                  ms.WriteByte((byte)((chars[i] + 0x1A40) % 256));
              }
              else
              {
                  b = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode.GetBytes(chars, i, 1));

                  for (int j = 0; j < b.Length; j++)
                      ms.WriteByte(b[j]);
              }
              //if (b.Length == 2 && b[1] * 256 + b[0] >= 0xE000 && b[1] * 256 + b[0] <= 0xE05f)
              //{
              //    ms.WriteByte((byte)(b[0] + 0x1A));
              //    ms.WriteByte(b[1]);

              //}
              //else
              //{
                 
              //}
          }

          return ms.ToArray();

     }

     public static string Big5BytesToString(byte[] code_big5)
     {

         System.IO.MemoryStream ms = new System.IO.MemoryStream();

         int inx = 0;
         int data=0;
         string s="";
         while (inx < code_big5.Length)
         {
             if (code_big5[inx] < 0x80)  //1 byte
             {
                 s += System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode, new byte[] { code_big5[inx++] }));
             }
             else    // 2 bytes
             {
                 data = code_big5[inx++] * 256;
                 data += code_big5[inx++];

                 if (data >= 0xfa40 && data <= 0XFEFE)
                 {
                     data -= 0x1a40;
                     s += System.Text.UnicodeEncoding.Unicode.GetString(new byte[] { (byte)(data % 256), (byte)(data / 256) });
                 }
                 else
                     s += System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode, new byte[] { (byte)(data / 256), (byte)(data % 256) }));
                 }

         }
         return s;

    //    return System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding("big5"), System.Text.Encoding.Unicode, code_big5));


     }
     public static System.Drawing.Bitmap ColorsToBitMap(System.Drawing.Color[][] pic)
     {
         System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(pic.GetUpperBound(0)+1, pic[0].GetUpperBound(0)+1);
         for (int w = 0; w < bmp.Width; w++)
             for (int h = 0; h < bmp.Height; h++)
             {
                 bmp.SetPixel(w, h, pic[w][h]);
             }

         return bmp;
     }

     public static System.Drawing.Color[][] BitMapToColors(System.Drawing.Bitmap pic)
     {
       //  System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(pic.Length.Length, pic[0].Length);
         System.Drawing.Color[][] ret;
         ret = new System.Drawing.Color[pic.Width][];

         for (int w = 0; w < pic.Width; w++)
         {
             ret[w]= new System.Drawing.Color[pic.Height];
             for (int h = 0; h < pic.Height; h++)
                 ret[w][h] = pic.GetPixel(w, h);

         }

         return ret;
     }


     public static System.Drawing.Color[,] BitMapToColorsArray(System.Drawing.Bitmap pic)
     {
         System.Drawing.Color[,] ret;

         ret = new System.Drawing.Color[pic.Height, pic.Width];
        

         for (int w = 0; w < pic.Width; w++)
         {
            // ret[w] = new System.Drawing.Color[pic.Height];
             for (int h = 0; h < pic.Height; h++)
                 ret[h,w] = pic.GetPixel(w, h);

         }

         return ret;
     }
 }
}
