using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Drawing;
using Comm;
using RemoteInterface;
using RemoteInterface.HWStatus;
using RemoteInterface.MFCC;




namespace Comm.TC
{
    /*
(a) 所有bits = 0：正常
(b) bit 0 = 1：設備故障
(c) bit 1 = 1：箱門開啟
(d) bit 2 = 1：手提測試機操作
(e) bit 3 = 1：控制面板操作、現場操作
(f) bit 4 = 1：基本參數為內定(要求下傳基本參數)
(g) bit 5 = 1：自行重新起動
(h) bit 6 = 1：燈號熄減
(i) bit 7 = 1：輸出/輸入單元故障
     * byte 2:
bit 0: 顯示設備故障
bit 1: 終端控制器與上層連線異常
bit 2: 終端控制器與下層連線異常
bit 3: LED故障模組
bit 4: 顯示板過電流/過電壓
bit 5: CGS 照明燈異常
4.1 - 38
bit 6: CGS 警示黃燈故障
bit 7: RTU 連動通訊異常
byte 3 : 保留
byte 4 : 保留

*/
  

    public class RGSTC : TCBase
    {

      
      
        RGS_GenericDisplay_Data curr_display_data;




        public RGSTC(Protocol protocol, string devicename, string ip, int port, int deviceid): base(protocol, devicename,ip, port, deviceid)
       {

           //m_protocol = protocol;
           //m_deviceName = devicename;
          // this.deviceid = deviceid;
           //System.Net.WebRequest req = System.Net.HttpWebRequest.Create(QYTask.Settings1.Default.TIMCC_uri);

           //System.IO.Stream stream = req.GetResponse().GetResponseStream();
           //System.IO.TextReader rd = new System.IO.StreamReader(stream);
           //System.IO.TextWriter wr = new System.IO.StreamWriter("tmp.xml");
           //wr.Write(rd.ReadToEnd());
           //wr.Flush();
           //wr.Close();
           //rd.Close();
           //stream.Close();

           new System.Threading.Thread(CheckGenericDisplayTask).Start();
        

       }


       

      

//#if  DEBUG
 //  public  void m_device_OnReport(object sender, TextPackage txtObj)
//#else
   

        public RGS_GenericDisplay_Data TC_GetCurrDisplayData()
        {
          return  RGS30_Extend.GetGenericDisplayData(m_device, m_deviceid);
        }

        private void CheckGenericDisplayTask()
        {
            while (true)
            {
                try
                {
                    if (this.curr_display_data != null)
                    {
                        RGS_GenericDisplay_Data data = TC_GetCurrDisplayData();
                        if (!data.Equals(curr_display_data))
                        {
                         //   TC_SetDisplayOff();
                            TC_SetGenericDisplay(curr_display_data);
                            Console.WriteLine(this.DeviceName + "Check Display Data is Different!");

                        }
                    }



                }
                catch (Exception ex)
                {
                    Console.WriteLine("CheckGenericDisplayTask", ex.Message);
                }
                finally
                {
                    System.Threading.Thread.Sleep(1000 * 30);
                }
            }
        }

       public void SetTravelDisplay(byte[] iconids, string[] messages, Color[][] forecolors)
       {
           byte mode=2 ;//cms mode
           byte iconcnt=0;
           byte msgcnt = 0;
          

        

           for (int i = 0; i < iconids.Length; i++)
               if (iconids[i] != 0) iconcnt++;
           for (int i = 0; i < messages.Length; i++)
           {
               if (messages[i] != "") msgcnt++;
              
           }

           
           
           RGS_Generic_ICON_Data[] icons = new RGS_Generic_ICON_Data[iconcnt];
           byte inx=0;
           for(int i=0;i<iconids.Length;i++)
           {
               if (iconids[i] != 0)
                   icons[inx++] = new RGS_Generic_ICON_Data(iconids[i], 0, (ushort)(128 * i));
           }

           RGS_Generic_Message_Data[] msgs=new RGS_Generic_Message_Data[msgcnt];
           inx=0;
           for (int i = 0; i < messages.Length; i++)
           {
               if(messages[i]!="")
               {
                   Color[]backcolor=new Color[messages[i].Length];
                   for(int j=0;j<messages[i].Length;j++)
                       backcolor[j]=Color.Black;
                   int offset =-128;
                   for (int j = 0; j < icons.Length; j++)
                   {
                       if (icons[j].y / 128 == i / 2)
                       {
                           offset = 0;
                           break;
                       }
                   }
                   msgs[inx++] = new RGS_Generic_Message_Data(messages[i], forecolors[i], backcolor,(ushort) (128+4+offset),(ushort)( 64 * i));

               }

           }


           

           RGS_GenericDisplay_Data data = new RGS_GenericDisplay_Data(mode,0,icons,msgs,new RGS_Generic_Section_Data[0]);
           curr_display_data = data;
           TC_SetGenericDisplay(data);
       }

       private void TC_SetGenericDisplay(RGS_GenericDisplay_Data data)
       {

           checkConntected();
        ///   TC_SetDisplayOff();
           RGS30_Extend.SetGenericDisplay(m_device, (int)m_deviceid, data);
           
       }

        private void TC_SetDisplayOff()
        {
            checkConntected();
            SendPackage pk = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0x53});
            this.m_device.Send(pk);
            if (pk.result != CmdResult.ACK)
                Console.WriteLine(m_deviceName + ":set display off" + pk.result);
        }

    
        //public I_HW_Status_Desc TC_GetHW_Status()
        //{
        //    checkConntected();
        //    SendPackage pkg = new SendPackage(CmdType.CmdQuery, CmdClass.A, this.m_deviceid, new byte[] { 0x01 });
        //    m_device.Send(pkg);
        //    if (pkg.result != CmdResult.ACK)
        //    {
        //        return null;
        //    }

        //    return new RGS_HW_StatusDesc(new byte[] { pkg.ReturnTextPackage.Text[1], pkg.ReturnTextPackage.Text[2], pkg.ReturnTextPackage.Text[3], pkg.ReturnTextPackage.Text[4] });

        //}

        public override I_HW_Status_Desc getStatusDesc()
        {
            return new RGS_HW_StatusDesc(m_hwstaus);
        }

        
     
     

      
       
    }
}
