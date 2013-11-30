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
  

    public class RGSTC :OutputTCBase
    {


      //  public event OnOutputChangedHandler OnOutputChanged;
        RGS_GenericDisplay_Data curr_display_data;




        public RGSTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus, byte comm_state)
            : base(protocol, devicename, ip, port, deviceid, hw_status,opmode,opstatus,comm_state)
       {


           this.OnTCReport += new OnTCReportHandler(RGSTC_OnTCReport);

       }

        void RGSTC_OnTCReport(object tc, TextPackage txt)
        {
           

            //throw new Exception("The method or operation is not implemented.");
        }



     

      

//#if  DEBUG
 //  public  void m_device_OnReport(object sender, TextPackage txtObj)
//#else
   

        public RGS_GenericDisplay_Data TC_GetCurrDisplayData()
        {
          return  RGS30_Extend.GetGenericDisplayData(m_device, m_deviceid);
        }
        public override void DownLoadConfig()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void CheckDisplayTask()
        {
            //while (true)
            //{
                try
                {
                    if (!this.IsConnected || this.curr_display_data==null)
                        return;

                    //curr_display_data
                    lock (currDispLockObj)
                    {
                        

                            RGS_GenericDisplay_Data data = TC_GetCurrDisplayData();
                            if (!data.Equals(curr_display_data))
                            {
                                //   TC_SetDisplayOff();

                                //TC_SetGenericDisplay(curr_display_data);
                                this.InvokeOutPutWrongEvent(curr_display_data.ToString(),data.ToString());


                                ConsoleServer.WriteLine(this.DeviceName + "Check Display Data is Different!");
                                this.TC_SetGenericDisplay(curr_display_data);
                            }
                       
                    }



                }
                catch (Exception ex)
                {
                    Console.WriteLine("CheckGenericDisplayTask", this.DeviceName+ex.Message);
                }
                //finally
                //{
                //    System.Threading.Thread.Sleep(1000 * 30);
                //}
            //}
        }

        public void TC_SetIconPic(int icon_id, string desc, Bitmap pic)
        {
            checkConntected();
            Comm.RGS30_Extend.SetIconPic(this.m_device, 0xffff, (byte)icon_id,desc, pic);
        }

        public Bitmap TC_GetIconPic(int icon_id,ref string desc)
        {
            checkConntected();
          return  Comm.RGS30_Extend.GetIconPic(this.m_device,0xffff,(byte)icon_id,ref desc);
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
           lock (currDispLockObj)
           {

               curr_display_data = data;
               TC_SetGenericDisplay(data);
           }
       }

        public void TC_SetBackGroundPicture(int address,int mode,int g_code_id,string desc,System.Drawing.Bitmap pic)
        {
            checkConntected();
            RGS30_Extend.SetBackgroundPic(this.m_device, address, (byte)mode, (byte)g_code_id, desc, pic);
        }

        public Bitmap TC_GetBackgroundPic(int address,byte mode,byte g_code_id,ref string desc)
        {
            checkConntected();
           return RGS30_Extend.GetBackgroundPic(this.m_device, address, mode, g_code_id, ref desc);
        }

        public override string GetCurrentDisplayDecs()
        {
            if (curr_display_data == null)
                return "熄滅";
            else
                return curr_display_data.ToString();
        }
       public void TC_SetGenericDisplay(RGS_GenericDisplay_Data data)
       {

           checkConntected();
        ///   TC_SetDisplayOff();
        ///
          
              
               lock (currDispLockObj)
               {
               if (curr_display_data != null)
               {
                   if (!curr_display_data.Equals(data))
                   {
                       curr_display_data = data;

                       this.InvokeOutPutChangeEvent(this, data.ToString());
                       RGS30_Extend.SetGenericDisplay(m_device, (int)m_deviceid, data);
                   }


               }
               else
               {
                       curr_display_data = data;
                       RGS30_Extend.SetGenericDisplay(m_device, (int)m_deviceid, data);
                       this.InvokeOutPutChangeEvent(this, data.ToString());

                }

             
              
               
                

               }
           
       }

        public string  TC_GetCurrentDisplayDecs()
        {

            if (curr_display_data == null)
                return "";
            else
                return curr_display_data.ToString();
        }


        public void TC_SetPolygon(int address, byte g_code_id, RGS_PolygonData pdata)
        {
            checkConntected();
            RGS30_Extend.RGS_setPolygons(this.m_device, address, g_code_id, pdata);

        }

        public RGS_PolygonData TC_GetPolygon(int address, byte g_code_id)
        {
            checkConntected();
            return RGS30_Extend.RGS_getPolygons(this.m_device, address, g_code_id);

        }

        public override void TC_SetDisplayOff()
        {
            checkConntected();
            SendPackage pk = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0x53});

            lock (currDispLockObj)
            {
                if (curr_display_data != null)
                    this.InvokeOutPutChangeEvent(this, "熄滅");
                this.curr_display_data = null;
                this.m_device.Send(pk);
                if (pk.result != CmdResult.ACK)
                    Console.WriteLine(m_deviceName + ":set display off" + pk.result);
            }
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
            return new RGS_HW_StatusDesc(this.DeviceName,m_hwstaus);
        }

        
     
     

      
       
    }
}
