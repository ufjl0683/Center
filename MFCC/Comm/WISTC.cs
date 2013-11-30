 using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HWStatus;
using RemoteInterface;


namespace Comm.TC
{
  public   class WISTC:OutputTCBase
    {
    //  public event OnOutputChangedHandler OnOutputChanged;

       private SendPackage currentDisplayPackage;
      private System.Data.DataSet currentDispalyDataset;

      private int curr_g_code_id, curr_hor_space;
      private string curr_mesg;
      public WISTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus, byte comm_state)
          : base(protocol, devicename, ip, port, deviceid, hw_status,opmode,opstatus,comm_state)
        {

          //  this.OnConnectStatusChanged += new ConnectStatusChangeHandler(WISTC_OnConnectStatusChanged);

            this.OnTCReport += new OnTCReportHandler(WISTC_OnTCReport);
            this.OnTCReceiveText += new OnTCReportHandler(WISTC_OnTCReceiveText);
            
        }

      void WISTC_OnTCReceiveText(object tc, TextPackage txt)
      {

          if (txt.Text[0] == 0xdf && txt.Text[1]==0xd6)
              WIS_LedTest_Report(txt.Text, this.DeviceName);

          //throw new Exception("The method or operation is not implemented.");
      }

      void WIS_LedTest_Report(byte[] txt, string devname)//DF D6 
      {

          int ErrorSum = 0;

          string SqlLedTest = "";

          string SqlLedTestDetail = "";



          if (txt[6] != 0)

              ErrorSum = txt[6] * 256 + txt[7];

          else

              ErrorSum = txt[7];



          DateTime t = DateTime.Now;



          if (ErrorSum != 0)
          {

              SqlLedTest = "INSERT INTO db2inst1.TBLLEDTest_WIS(DEVICENAME,TIMESTAMP,BADSETNO) VALUES('" + devname + "','" + RemoteInterface.DbCmdServer.getTimeStampString(t) + "'," + ErrorSum + ") ";



              this.InvokeDBDemand(SqlLedTest);

              for (int i = 8; i < txt.Length; i = i + 2)
              {

                  SqlLedTestDetail = "INSERT INTO db2inst1.tblLEDTestDetail_WIS(DEVICENAME,TIMESTAMP,X,Y) VALUES('" + devname + "','" + RemoteInterface.DbCmdServer.getTimeStampString(t) + "'," + txt[i] + "," + txt[i + 1] + ")";



                  this.InvokeDBDemand(SqlLedTestDetail);

              }

          }

      }


      void WISTC_OnTCReport(object tc, TextPackage txt)
      {

#if DEBUG
          ////WIS-T78-E-26.9
          //if ((tc as TCBase).DeviceName != "WIS-T78-E-26.9")
          //    return;
#endif
          if (txt.Text[0] == 0xdf && txt.Text[1] == 0xda)
          {

               
              System.Data.DataSet ds = this.m_protocol.GetSendDsByTextPackage(txt,CmdType.CmdReport);
              this.InvokeOutPutWrongEvent(GetDisplayDesc(currentDispalyDataset),GetDisplayDesc(ds));
              
          }

          //throw new Exception("The method or operation is not implemented.");
      }
      public override void DownLoadConfig()
      {
        //  throw new Exception("The method or operation is not implemented.");
      }
      
      public override void  TC_SetDisplayOff()
      {
          checkConntected();
          SendPackage pk = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0xdf,0xd3 });
          this.m_device.Send(pk);
          if (pk.result != CmdResult.ACK)
              Console.WriteLine(m_deviceName + ":set display off" + pk.result);

          if (currentDispalyDataset != null )
              this.InvokeOutPutChangeEvent(this, "熄滅");

          this.currentDispalyDataset = null;
      }

      public override string GetCurrentDisplayDecs()
      {
          if (this.currentDispalyDataset == null)
              return "";

          return "g_code_id:" + curr_g_code_id + " mesg:" + curr_mesg;
          
          //throw new Exception("The method or operation is not implemented.");
      }

      public string GetDisplayDesc(System.Data.DataSet ds)
      {
          if (ds == null || ds.Tables[0].Rows[0]["msg_length"].ToString()=="0")
              return "熄滅";
          if (ds.Tables[0].Rows[0]["Data_Type"].ToString() == "0")  //text
          {
              byte[] code = new byte[ds.Tables["tblmsgcnt"].Rows.Count];
              for (int i = 0; i < code.Length; i++)
                  code[i] = System.Convert.ToByte(ds.Tables["tblmsgcnt"].Rows[i]["message"]);
              return "g_code_id:0 mesg:" + Util.Big5BytesToString(code);
          }
          else
          {  // g_code_id
              return "g_code_id:" + ds.Tables[0].Rows[0]["g_code_id"].ToString() + "mesg:";
          }

      }

      protected override void CheckDisplayTask()
      {
           if (currentDispalyDataset == null || !IsConnected) return;
          try
          {

            System.Data.DataSet ds=this.TC_GetDisplay();
            lock(this.currDispLockObj)
            {
            bool isEqual = true;
            for (int i = currentDispalyDataset.Tables[0].Columns.IndexOf("data_type"); i < currentDispalyDataset.Tables[0].Columns.Count; i++)
            {
                isEqual = isEqual && (currentDispalyDataset.Tables[0].Rows[0][i].Equals(ds.Tables[0].Rows[0][currentDispalyDataset.Tables[0].Columns[i].ColumnName]));
            }

            if (!isEqual)
            {
                ConsoleServer.WriteLine(this.DeviceName + " 顯示資料比對錯誤!");
                this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), GetDisplayDesc(ds));
                this.m_device.Send(this.m_protocol.GetSendPackage(currentDispalyDataset, 0xffff));
            }
            else
                this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), GetCurrentDisplayDecs());
            }
                   // this.OnOutputDataCompareWrongEvent(this, currentDispalyDataset);

          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine("in check display task"+ex.Message+ex.StackTrace);
          }
      }

      //void WISTC_OnConnectStatusChanged(object tc)
      //{
      //    //throw new Exception("The method or operation is not implemented.");
      //    try
      //    {
      //        if (this.IsConnected && currentDisplayPackage !=null)

      //            m_device.Send(currentDisplayPackage);
      //    }catch{;}
         

      //}

        public override RemoteInterface.I_HW_Status_Desc getStatusDesc()
        {
            //throw new Exception("The method or operation is not implemented.");
            return new WIS_HW_StatusDesc(this.DeviceName,m_hwstaus);
        }

        
        public  void TC_SendDisplay(int g_code_id,int hor_space, string mesg, byte[]colors) 
            //color length= mesg length execept '\r'
        {
            int ver_no = 1;
            byte[]big5bytes;
          //  if (mesg.IndexOf('\r') == -1)  //append cr 保證 msgleng 不為零
          //      mesg += "\r";

           
            this.checkConntected();

            lock (this.currDispLockObj)
            {

                if (g_code_id == 0)  //文字模式
                {
                    System.Data.DataSet ds = this.m_protocol.GetSendDataSet("set_display_text");
                    //  ds.Tables["tblMain"].Rows[0]["data_type"] = 0; //text

                    big5bytes = RemoteInterface.Util.StringToBig5Bytes(mesg);

                    ds.Tables["tblMain"].Rows[0]["msgcnt"] = ds.Tables["tblMain"].Rows[0]["msg_length"] = big5bytes.Length;
                    for (int i = 0; i < big5bytes.Length; i++)
                        if (big5bytes[i] == 0x0d)
                            ver_no++;

                    ds.Tables["tblMain"].Rows[0]["ver_no"] = ver_no;
                    ds.Tables["tblMain"].Rows[0]["hor_space"] = hor_space;
                    for (int i = 0; i < ver_no; i++)
                        ds.Tables["tblver_no"].Rows.Add(0);

                    for (int i = 0; i < big5bytes.Length; i++)
                        ds.Tables["tblmsgcnt"].Rows.Add(big5bytes[i]);



                    string mesg1 = mesg.Replace("\r", "");
                    for (int i = 0; i < mesg1.Length; i++)
                    {

                        ds.Tables["tblcolorcnt"].Rows.Add(colors[i]);

                    }
                    ds.Tables["tblMain"].Rows[0]["colorcnt"] = ds.Tables["tblcolorcnt"].Rows.Count;
                    ds.AcceptChanges();
                    currentDispalyDataset = ds;
                    currentDisplayPackage = this.m_protocol.GetSendPackage(ds, 0xffff);
                    this.m_device.Send(currentDisplayPackage);
                    // ds.Dispose();
                }
                else   //圖形模式
                {
                    System.Data.DataSet ds = this.m_protocol.GetSendDataSet("set_display_graph");
                    ds.Tables["tblMain"].Rows[0]["data_type"] = 1;
                    ds.Tables["tblMain"].Rows[0]["g_code_id"] = g_code_id;
                    currentDispalyDataset = ds;
                    ds.AcceptChanges();
                    currentDisplayPackage = this.m_protocol.GetSendPackage(ds, 0xffff);
                    this.m_device.Send(currentDisplayPackage);

                }

                if (curr_mesg != mesg || curr_g_code_id != g_code_id)
                {
                    curr_mesg = mesg;
                    curr_g_code_id = g_code_id;
                    curr_hor_space = hor_space;

                    this.InvokeOutPutChangeEvent(this, this.GetCurrentDisplayDecs());
                }
            }
 
            


        }

        public System.Data.DataSet TC_GetDisplay()
        {


            System.Data.DataSet ds = this.m_protocol.GetSendDataSet("get_wis_display");
            
                SendPackage pkg = this.m_protocol.GetSendPackage(ds, 0xffff);
                this.Send(pkg);
                if (pkg.result != CmdResult.ACK)
                    throw new Exception(pkg.result.ToString());

               // ds.Dispose();
                ds = m_protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
                return ds;
            
           
        }


      

        public override void OneMinTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            base.OneMinTimer_Elapsed(sender, e);

            //try
            //{
            //    this.TC_SendDisplay(0, 0, 0, "中A\r中", new byte[] { 0x30, 0x30, 0x30 });
            //}
            //catch (Exception ex)
            //{
            //    ConsoleServer.WriteLine(this+ex.Message);
            //}

            //if (!this.IsConnected)
            //    return;

            //if (currentDisplayPackage != null)
            //{
            //    try
            //    {
            //        this.m_device.Send(currentDisplayPackage);
            //    }
            //    catch { ;}
            //}

        }
    }
}
