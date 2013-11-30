using System;
using System.Collections.Generic;
using System.Text;

namespace Host.CCTV
{

    
    public class LockWindows
    {

        public static string unlockurl =    "http://10.23.15.21/lockvideo_e_rels.php?mid={0}";
         public static string lockurlbase = "http://10.23.15.21/lockvideo_e.php?sip=0.0.0.0&sport=80&video_show=1&mid={0}&chlist={1},&eidlist={2},&edesclist={3},&presetlist={4},&presettl=0";
        int wid;  //lock windows id 2~9
        bool isLock = false;
        int cctvid = 0;
        string desc1, desc2;
        int preset = 0;
        public Event.Event evt ;
        public LockWindows(int wid )
        {
            this.wid = wid;
        }

        public void setLock(int cctvid, string desc1, string desc2, int preset)
        {
            this.cctvid = cctvid;
            this.desc1 = desc1;
            this.desc2 = desc2;
            byte[] codebig5 =/* RemoteInterface.Util.StringToUTF8Bytes(desc2);*/         RemoteInterface.Util.StringToBig5Bytes(desc2);
            
            desc2 = System.Web.HttpUtility.UrlEncode(codebig5);
            codebig5 = /* RemoteInterface.Util.StringToUTF8Bytes(desc1);   */         RemoteInterface.Util.StringToBig5Bytes(desc1);
            desc1 = System.Web.HttpUtility.UrlEncode(codebig5);
            string uristr = string.Format(LockWindows.lockurlbase, this.wid, this.cctvid, desc1, desc2, preset);
            //  string uristr = string.Format(LockWindows.lockurlbase, 3, this.cctvid, desc1, desc2, preset);
            //只鎖定 第3號視窗

            System.Net.WebRequest web = System.Net.HttpWebRequest.Create(new Uri(uristr
                 , UriKind.Absolute)
             );

            System.IO.Stream stream = web.GetResponse().GetResponseStream();
            System.IO.StreamReader rd = new System.IO.StreamReader(stream);
            string res = rd.ReadToEnd();
            isLock = true;
        }
        public void  setLock(Event.Event  evt,int cctvid,string desc1,string desc2,int preset)
        {
            
            try
            {
                unlock();
                this.cctvid=cctvid;
                this.desc1=desc1;
                this.desc2=desc2;
                this.preset=preset;
                this.evt = evt;
                byte[] codebig5 =/* RemoteInterface.Util.StringToUTF8Bytes(desc2);*/         RemoteInterface.Util.StringToBig5Bytes(desc2);

                desc2 =      System.Web.HttpUtility.UrlEncode(codebig5);
                codebig5 = /* RemoteInterface.Util.StringToUTF8Bytes(desc1);   */         RemoteInterface.Util.StringToBig5Bytes(desc1);
                desc1 = System.Web.HttpUtility.UrlEncode(codebig5);
                 string uristr=string.Format(LockWindows.lockurlbase,this.wid,this.cctvid,desc1,desc2,preset);
               //  string uristr = string.Format(LockWindows.lockurlbase, 3, this.cctvid, desc1, desc2, preset);
                //只鎖定 第3號視窗
            
                System.Net.WebRequest web = System.Net.HttpWebRequest.Create(new Uri(uristr
                     ,UriKind.Absolute)
                 );
              
                System.IO.Stream stream = web.GetResponse().GetResponseStream();
                System.IO.StreamReader rd = new System.IO.StreamReader(stream);
                string res = rd.ReadToEnd();
                isLock = true;
            }
            catch (Exception ex)
            {
                isLock = false;
            }

        }


        public void unlock()
        {
                System.Net.WebRequest web = System.Net.HttpWebRequest.Create(string.Format(LockWindows.unlockurl,this.wid));
                System.IO.Stream stream = web.GetResponse().GetResponseStream();
                System.IO.StreamReader rd = new System.IO.StreamReader(stream);
                string res = rd.ReadToEnd();

                this.cctvid = 0;
                this.desc1 = "";
                this.desc2 = "";
                this.preset = 0;
                isLock = false ;
                this.evt = null;

        }

        

    }
}
