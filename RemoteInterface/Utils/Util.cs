using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace RemoteInterface.Utils
{
   public  class Util
    {
       static object filelockobj = new object();
       static HC.I_HC_Comm hostcomobj=null;
        private static void Log(string pathfile, string message)
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
                Console.WriteLine(ex.Message + "," + ex.StackTrace);
            }
        }


        public static void SysLog(string filename, string message)
        {

            Util.Log(CPath(AppDomain.CurrentDomain.BaseDirectory + filename), DateTime.Now + "," + message + "\r\n");
        }
        private static string CPath(string WinPath)
        {


            if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
                return WinPath;
            else
            {
                //  Console.WriteLine("Unix");
                return WinPath.Replace(@"\", @"/");
            }
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
            int data = 0;
            string s = "";
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

        public static string ObjToString(object obj)
        {



            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //  System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            bf.Serialize(ms, obj);

            byte[] data = new byte[ms.Position];
            System.Array.Copy(ms.GetBuffer(), data, data.Length);
            string hexstr = ToHexString(data).Replace(" ", "");
            return hexstr.Trim();




        }

       private static string ToHexString(byte[] data)
       {
           StringBuilder sb = new StringBuilder(100);

           for (int i = 0; i < data.Length; i++)
               sb.Append(string.Format("{0:X2} ", data[i]));

           return sb.ToString();
       }
       private static byte[] ToBytes(string hexstr)
       {
           // string[] dats = hexstr.Trim().Split(new char[] { ' ' });
           byte[] data = new byte[hexstr.Length / 2];
           for (int i = 0; i < data.Length; i++)
               data[i] = System.Convert.ToByte(hexstr[i * 2].ToString() + hexstr[i * 2 + 1].ToString(), 16);
           return data;
       }
        public static Object StringToObj(string objHexStr)
        {
            System.IO.MemoryStream ms;
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            ms = new System.IO.MemoryStream(ToBytes(objHexStr));

            return bf.Deserialize(ms);
        }

        public static void  setDateTimeFromHost()
        {
            if (hostcomobj == null)
                hostcomobj = (RemoteInterface.HC.I_HC_Comm)RemoteBuilder.GetRemoteObj(typeof(HC.I_HC_Comm),
                    RemoteBuilder.getRemoteUri(RemoteBuilder.getHostIP(),(int)RemotingPortEnum.HOST,"Comm"));

            if (hostcomobj != null)
            {
              DateTime dt=  hostcomobj.getDateTime();
              SetSysTime(dt);
            }
        }

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

        [DllImport("Kernel32.dll")]
        private extern static uint SetLocalTime(ref SYSTEMTIME lpSystemTime);

        private static void SetSysTime(System.DateTime dt)
        {
          //  if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
                SetWinsTime((ushort)dt.Year, (ushort)dt.Month, (ushort)dt.Day, (ushort)dt.Hour, (ushort)dt.Minute, (ushort)dt.Second);
          //  else
            //    SetUnixTime((ushort)dt.Year, (ushort)dt.Month, (ushort)dt.Day, (ushort)dt.Hour, (ushort)dt.Minute, (ushort)dt.Second);

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

    }
}
