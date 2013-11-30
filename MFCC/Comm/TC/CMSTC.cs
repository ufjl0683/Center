using System;
using System.Collections.Generic;
using System.Text;
using Comm;
using RemoteInterface.HWStatus;
using System.Drawing;
using RemoteInterface;
namespace Comm.TC
{
   
  public   class CMSTC:OutputTCBase
    {

      private SendPackage currentDisplayPackage;
      private System.Data.DataSet currentDispalyDataset;
      private int curr_g_code_id, curr_hor_space,curr_icon_id;
      private string curr_mesg;
      private bool isIcon=false;
      private int CrMode = 1;
      public CMSTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus, byte comm_state)
          : base(protocol, devicename, ip, port, deviceid, hw_status, opmode, opstatus,comm_state)
        {

       

           this.OnTCReport += new OnTCReportHandler(CMSTC_OnTCReport);
           this.OnTCReceiveText += new OnTCReportHandler(CMSTC_OnTCReceiveText);
         //  this.m_device.OnReceiveText += new OnTextPackageEventHandler(m_device_OnReceiveText);
            System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select show_icon,CRmode from tblCmsConfig where DeviceName='" + this.DeviceName + "'");
            cmd.Connection = cn;
            
            try
            {
               
                cn.Open();
                System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    isIcon = (System.Convert.ToInt32(rd[0])==0)?false:true;
                    CrMode = System.Convert.ToInt32(rd[1]);
                }
                rd.Close();
                
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

      void CMSTC_OnTCReceiveText(object tc, TextPackage txt)
      {
          if (txt.Cmd == 0x56)
          {
              CMS_LedTest_Report(txt.Text, this.DeviceName);
          }
          //throw new Exception("The method or operation is not implemented.");
      }

    

      void CMSTC_OnTCReport(object tc, TextPackage txt)
      {

#if DEBUG
        

#endif
         
          try
          {
              if (txt.Cmd == 0x5a || txt.Cmd == 0x5b)
              {
                  if (this.currentDispalyDataset == null)
                      return;

                  if (IsModuleErr(txt))
                  {
                      ConsoleServer.WriteLine(DeviceName + "," + "顯示模組錯誤!");
                      return;
                  }
                  System.Data.DataSet ds = this.m_protocol.GetSendDsByTextPackage(txt, CmdType.CmdReport);

                  //Console.WriteLine(GetDisplayDesc(ds));
                  //Console.WriteLine(GetDisplayDesc(currentDispalyDataset));

                  if (ds != null && !IsEqualToCurrentDisplay(ds))
                  {
                      ConsoleServer.WriteLine(this.DeviceName + "," + Util.ToHexString((byte)txt.Cmd) + ",顯示資料比對錯誤!");
                      //if (txt.Cmd == 0x5b)
                      this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), GetDisplayDesc(ds));
                  }
                  else
                  {
                      this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), GetCurrentDisplayDecs());

                  }
              }
             
          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine("in CmsTCOnReport:ex.Message"+ "," + ex.StackTrace);
          }

      }


      void CMS_LedTest_Report(byte[] txt, string devname)
      {

          int ErrorSum = 0;
          string SqlLedTest = "";
          string SqlLedTestDetail = "";

          if (txt[5] != 0)
              ErrorSum = txt[5] * 256 + txt[6];
          else
              ErrorSum = txt[6];

          DateTime t = DateTime.Now;

          if (ErrorSum != 0)
          {
              SqlLedTest = "INSERT INTO db2inst1.TBLLEDTest_CMS(DEVICENAME,TIMESTAMP,BADSETNO) VALUES('" + devname + "','" + RemoteInterface.DbCmdServer.getTimeStampString(t) + "'," + ErrorSum + ") ";
              for (int i = 7; i < txt.Length; i = i + 2)
              {
                  SqlLedTestDetail = "INSERT INTO db2inst1.tblLEDTestDetail_CMS(DEVICENAME,TIMESTAMP,X,Y) VALUES('" + devname + "','" + RemoteInterface.DbCmdServer.getTimeStampString(t) + "'," + txt[i] + "," + txt[i + 1] + ")";
                  this.InvokeDBDemand(SqlLedTestDetail);
              }
              this.InvokeDBDemand(SqlLedTest);
              
          }
        
      }

 


      //void tmrCheckDisplay_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
      //{
      //    // throw new Exception("The method or operation is not implemented.");

      //    if(this is OutputTCBase)
      //       CheckDisplayTask();



      //}
      public override void DownLoadConfig()
      {
         // throw new Exception("The method or operation is not implemented.");
      }
      protected override void CheckDisplayTask()
      {

#if DEBUG
          if (this.DeviceName != "CMS-T78-E-28.5")
              return;

#endif

           if (currentDispalyDataset == null || !IsConnected) return;
           //if (!this.IsConnected)
           //    return;
          try
          {

            System.Data.DataSet ds=this.TC_GetDisplay();
            bool isEqual = true;

            if (ds == null) return;

            isEqual = IsEqualToCurrentDisplay(ds);

            if (!isEqual)
            {
                ConsoleServer.WriteLine(this.DeviceName + " 顯示資料比對錯誤!");
                this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), GetDisplayDesc(ds));

                this.m_device.Send(this.m_protocol.GetSendPackage(currentDispalyDataset, 0xffff));
            }
            else
            {
                this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(),GetCurrentDisplayDecs());

            }

               
              
            }
          catch (Exception ex)
          {
              if (ex.Message == "DisplayModuleError")
                  ConsoleServer.WriteLine(this.DeviceName + "," + "顯示模組錯誤");
              else
                  ConsoleServer.WriteLine("in check display task" + this.DeviceName + ex.Message+ex.StackTrace);
          }
      }


      private bool IsEqualToCurrentDisplay(System.Data.DataSet ds)
      {
      
          bool isEqual = true;
           lock (currDispLockObj)
            {
                for (int i = ds.Tables[0].Columns.IndexOf("data_type"); i < ds.Tables[0].Columns.Count; i++)
                {

                    if (currentDispalyDataset.Tables[0].Rows[0]["func_name"].ToString() == "set_display_text" && (ds.Tables[0].Columns[i].ColumnName == "g_code_id" || ds.Tables[0].Columns[i].ColumnName.StartsWith("g_desc")))
                        continue;
                    if (currentDispalyDataset.Tables[0].Rows[0]["func_name"].ToString() == "set_display_graph" && ds.Tables[0].Columns[i].ColumnName != "g_code_id")
                        continue;

                    //if (this.DeviceName == "CMS-T78-E-28.5")
                    //{
                    //    Console.WriteLine(currentDispalyDataset.Tables[0].Rows[0][ds.Tables[0].Columns[i].ColumnName]);
                    //    Console.WriteLine(ds.Tables[0].Rows[0][i]);
                    //}

                   if(currentDispalyDataset.Tables[0].Rows[0][ds.Tables[0].Columns[i].ColumnName] !=System.DBNull.Value && ds.Tables[0].Rows[0][i] != System.DBNull.Value)
                      
                    isEqual = isEqual && (currentDispalyDataset.Tables[0].Rows[0][ds.Tables[0].Columns[i].ColumnName].Equals(ds.Tables[0].Rows[0][i]));

              
                  
                }

                if (isEqual)
                {
                    for (int j = 0; j < ds.Tables["tblmsgcnt"].Rows.Count; j++)
                        if (isEqual && System.Convert.ToInt32(ds.Tables["tblmsgcnt"].Rows[j]["message"]) != 0)
                            isEqual = isEqual && ds.Tables["tblmsgcnt"].Rows[j]["message"].ToString() == currentDispalyDataset.Tables["tblmsgcnt"].Rows[j]["message"].ToString();
                }
               
            }
                 
            return isEqual;

      }

    

      public override RemoteInterface.I_HW_Status_Desc getStatusDesc()
        {
            //throw new Exception("The method or operation is not implemented.");
            return new CMS_HW_StatusDesc(this.m_deviceName,m_hwstaus);
        }

      public override string GetCurrentDisplayDecs()
      {
          //throw new Exception("The method or operation is not implemented.");
          if (currentDispalyDataset == null)
              return "熄滅";
          if (curr_g_code_id != 0)
              return "g_code_id:" + curr_g_code_id;
          else
              return "iconid:" + curr_icon_id + " mesg:" + curr_mesg.Trim(new char[] { '\r' });
           

      }
      public string GetDisplayDesc(System.Data.DataSet ds)
      {
          if (ds.Tables[0].Rows[0]["Data_Type"].ToString() == "0" || ds.Tables[0].Rows[0]["Data_Type"].ToString() == "4" || ds.Tables[0].Rows[0]["Data_Type"].ToString() == "10")  //text
          {
              byte[] code = new byte[ds.Tables["tblmsgcnt"].Rows.Count];
              for (int i = 0; i < code.Length; i++)
                  code[i] = System.Convert.ToByte(ds.Tables["tblmsgcnt"].Rows[i]["message"]);

              if (code.Length == 0)
                  return "熄滅";

              return "g_code_id:0 mesg:" + Util.Big5BytesToString(code);
          }
          else
          {  // g_code_id
              return "g_code_id:" + ds.Tables[0].Rows[0]["g_code_id"].ToString() + "mesg:";
          }

      }


      public void TC_SendDisplay(int dataType, int icon_id, int g_code_id, int hor_space, string mesgs, byte[] colors, byte[] vspaces)
      {
          string mesg;
          switch (this.CrMode)
          {
              case 0:
                  mesg = mesgs.Replace("\r", "");
                  break;
              case 2:
                  mesg = mesgs.Replace("\r", "\r\n");
                  break;
              default:
                  mesg = mesgs;
                  break;

          }


          int ver_no = 1;
          byte[] big5bytes;
          //if (mesg.IndexOf('\r') == -1)  //append cr 保證 msgleng 不為零
          //    mesg += "\r";


          this.checkConntected();

          System.Data.DataSet ds = this.m_protocol.GetSendDataSet("set_display_control");

          ds.Tables["tblMain"].Rows[0]["data_type"] = dataType;
          
          

          ds.Tables["tblMain"].Rows[0]["icon_code_id"] = icon_id;
          ds.Tables["tblMain"].Rows[0]["g_code_id"] = g_code_id;
          big5bytes = RemoteInterface.Util.StringToBig5Bytes(mesg);

          ds.Tables["tblMain"].Rows[0]["msgcnt"] = ds.Tables["tblMain"].Rows[0]["msg_length"] = big5bytes.Length;
          for (int i = 0; i < big5bytes.Length; i++)
              if (big5bytes[i] == 0x0d)
                  ver_no++;

          //   ds.Tables["tblMain"].Rows[0]["ver_no"] = ver_no;
          ds.Tables["tblMain"].Rows[0]["ver_no"] = vspaces.Length;
          ds.Tables["tblMain"].Rows[0]["hor_space"] = hor_space;
          //for (int i = 0; i < ver_no; i++)
          //    ds.Tables["tblver_no"].Rows.Add(0);
          for (int i = 0; i < vspaces.Length; i++)
              ds.Tables["tblver_no"].Rows.Add(vspaces[i]);

          for (int i = 0; i < big5bytes.Length; i++)
              ds.Tables["tblmsgcnt"].Rows.Add(big5bytes[i]);



          //byte color_code = 0;


          //    if(colors[i]== Color.Black)
          //        color_code = 0;

          //    else if(colors[i]== Color.Green)
          //        color_code = 1;

          //    else if(colors[i]== Color.Red)
          //        color_code = 2;

          //    else if(colors[i]== Color.Red)
          //        color_code = 3;
          string mesg1;

          mesg1 = mesg.Replace("\r", "").Replace("\n", "");




          for (int i = 0; i < mesg1.Length; i++)
          {
              //if ((int)mesg1[i] <= 128)
              ds.Tables["tblcolorcnt"].Rows.Add(colors[i]);
              //else  //chinese char
              //{
              //    ds.Tables["tblcolorcnt"].Rows.Add(colors[i]);
              //    ds.Tables["tblcolorcnt"].Rows.Add(colors[i]);
              //}
          }
          ds.Tables["tblMain"].Rows[0]["colorcnt"] = ds.Tables["tblcolorcnt"].Rows.Count;

          currentDisplayPackage = this.m_protocol.GetSendPackage(ds, 0xffff);

          lock (currDispLockObj)
          {

              currentDispalyDataset = ds;
              this.m_device.Send(currentDisplayPackage);

          }
          if (currentDisplayPackage.result != CmdResult.ACK)
              throw new Exception(currentDisplayPackage.ToString() + " " + currentDisplayPackage.result.ToString());

          // ds.Dispose();

          if (curr_icon_id != icon_id || curr_g_code_id != g_code_id || curr_mesg != mesg)
          {
              curr_icon_id = icon_id;
              curr_g_code_id = g_code_id;
              curr_hor_space = hor_space;
              curr_mesg = mesg;
              this.InvokeOutPutChangeEvent(this, this.GetCurrentDisplayDecs());
          }

      }

      public  void TC_SendDisplay(int icon_id, int g_code_id,int hor_space, string mesgs, byte[]colors ,byte[] vspaces) 
            //color length= mesg length execept '\r'
        {
          //  checkConntected();



            


            string mesg;
            switch (this.CrMode)
            {
                case 0:
                    mesg = mesgs.Replace("\r", "");
                    break;
                case 2:
                    mesg = mesgs.Replace("\r", "\r\n");
                    break;
                default:
                    mesg = mesgs;
                    break;

            }


            int ver_no = 1;
            byte[]big5bytes;
            //if (mesg.IndexOf('\r') == -1)  //append cr 保證 msgleng 不為零
            //    mesg += "\r";

           
            this.checkConntected();

           // if (currentDispalyDataset != null)

           //     if (curr_icon_id == icon_id || curr_g_code_id == g_code_id || curr_mesg == mesg)
             //       return;
             

            if ( this.isIcon ) //全彩cms 3.0
            {
                
                System.Data.DataSet ds = this.m_protocol.GetSendDataSet("set_display_control");
                if(icon_id!=0)
                ds.Tables["tblMain"].Rows[0]["data_type"] = 1;
                else
                ds.Tables["tblMain"].Rows[0]["data_type"] = 4;

                ds.Tables["tblMain"].Rows[0]["icon_code_id"] = icon_id;
                ds.Tables["tblMain"].Rows[0]["g_code_id"] = g_code_id;
                big5bytes = RemoteInterface.Util.StringToBig5Bytes(mesg);

                ds.Tables["tblMain"].Rows[0]["msgcnt"] = ds.Tables["tblMain"].Rows[0]["msg_length"] = big5bytes.Length;
                for(int i=0;i<big5bytes.Length;i++)
                         if(big5bytes[i]==0x0d)
                             ver_no++;
               
             //   ds.Tables["tblMain"].Rows[0]["ver_no"] = ver_no;
                ds.Tables["tblMain"].Rows[0]["ver_no"] = vspaces.Length;
                ds.Tables["tblMain"].Rows[0]["hor_space"] = hor_space;
                //for (int i = 0; i < ver_no; i++)
                //    ds.Tables["tblver_no"].Rows.Add(0);
                for (int i = 0; i < vspaces.Length; i++)
                    ds.Tables["tblver_no"].Rows.Add(vspaces[i]);

                for (int i = 0; i < big5bytes.Length; i++)
                    ds.Tables["tblmsgcnt"].Rows.Add(big5bytes[i]);
                  
              
                
                     //byte color_code = 0;
                  
                     
                     //    if(colors[i]== Color.Black)
                     //        color_code = 0;
                            
                     //    else if(colors[i]== Color.Green)
                     //        color_code = 1;
                           
                     //    else if(colors[i]== Color.Red)
                     //        color_code = 2;
                          
                     //    else if(colors[i]== Color.Red)
                     //        color_code = 3;
                 string mesg1;
             
                    mesg1 = mesg.Replace("\r", "").Replace("\n","");
               

                    
                    
                     for (int i = 0; i < mesg1.Length; i++)
                     {
                         //if ((int)mesg1[i] <= 128)
                             ds.Tables["tblcolorcnt"].Rows.Add(colors[i]);
                         //else  //chinese char
                         //{
                         //    ds.Tables["tblcolorcnt"].Rows.Add(colors[i]);
                         //    ds.Tables["tblcolorcnt"].Rows.Add(colors[i]);
                         //}
                     }
                     ds.Tables["tblMain"].Rows[0]["colorcnt"] = ds.Tables["tblcolorcnt"].Rows.Count;

                     currentDisplayPackage = this.m_protocol.GetSendPackage(ds, 0xffff);
                   
                     lock (currDispLockObj)
                     {
                         
                      currentDispalyDataset = ds;
                      this.m_device.Send(currentDisplayPackage);

                     }
                     if (currentDisplayPackage.result != CmdResult.ACK)
                         throw new Exception(  currentDisplayPackage.ToString()+" "+ currentDisplayPackage.result.ToString());
                     
                // ds.Dispose();
                
                     
             }


              

            
            else  // old version cmd
            {
                if ( g_code_id==0)
                {

                    System.Data.DataSet ds = this.m_protocol.GetSendDataSet("set_display_text");
                    // ds.Tables["tblMain"].Rows[0]["data_type"] = 0; //text

                    big5bytes = RemoteInterface.Util.StringToBig5Bytes(mesg);

                    ds.Tables["tblMain"].Rows[0]["msgcnt"] = ds.Tables["tblMain"].Rows[0]["msg_length"] = big5bytes.Length;
                    for (int i = 0; i < big5bytes.Length; i++)
                        if (big5bytes[i] == 0x0d)
                            ver_no++;

                   // ds.Tables["tblMain"].Rows[0]["ver_no"] = ver_no;

                    ds.Tables["tblMain"].Rows[0]["ver_no"] = vspaces.Length;
                    ds.Tables["tblMain"].Rows[0]["hor_space"] = hor_space;
                    for (int i = 0; i < vspaces.Length; i++)
                        ds.Tables["tblver_no"].Rows.Add(vspaces[i]);

                    for (int i = 0; i < big5bytes.Length; i++)
                        ds.Tables["tblmsgcnt"].Rows.Add(big5bytes[i]);



                    string mesg1 = mesg.Replace("\r", "").Replace("\n","");
                    for (int i = 0; i < mesg1.Length; i++)
                    {
                        // if ((int)mesg1[i] <= 128)
                        ds.Tables["tblcolorcnt"].Rows.Add(colors[i]);
                        //else  //chinese char
                        //{
                        //    ds.Tables["tblcolorcnt"].Rows.Add(colors[i]);
                        //    ds.Tables["tblcolorcnt"].Rows.Add(colors[i]);
                        //}
                    }
                    ds.Tables["tblMain"].Rows[0]["colorcnt"] = ds.Tables["tblcolorcnt"].Rows.Count;

                    lock (currDispLockObj)
                    {
                        currentDispalyDataset = ds;
                           

                        currentDisplayPackage = this.m_protocol.GetSendPackage(ds, 0xffff);
                        this.m_device.Send(currentDisplayPackage);
                    //    ConsoleServer.WriteLine("CMS:"+currentDisplayPackage.ToString());
                    }
                }
                else  // g_code_id
                {
                    System.Data.DataSet ds = this.m_protocol.GetSendDataSet("set_display_graph");
                    ds.Tables[0].Rows[0]["g_code_id"]=g_code_id;
                    //ds.Tables[0].Rows[0]["data_type"] = 1;
                    ds.AcceptChanges();

                    lock (currDispLockObj)
                    {
                        
                        currentDispalyDataset = ds;
                               
                      

                        currentDisplayPackage = this.m_protocol.GetSendPackage(ds, 0xffff);
                        this.m_device.Send(currentDisplayPackage);
                    }
                }
               // ds.Dispose();

                
            }

            if (curr_icon_id != icon_id || curr_g_code_id != g_code_id || curr_mesg != mesg)
            {
                curr_icon_id = icon_id;
                curr_g_code_id = g_code_id;
                curr_hor_space = hor_space;
                curr_mesg = mesg;
                this.InvokeOutPutChangeEvent(this, this.GetCurrentDisplayDecs());
            }
            


        }

      public  System.Data.DataSet TC_GetDisplay()
        {


            System.Data.DataSet ds = null; ;
            // if(currentDispalyDataset.Tables[0].Rows[0]["func_name"].ToString()=="set_display_control")
          if(this.isIcon)
                ds = this.m_protocol.GetSendDataSet("get_display_control");
            else 
              ds= this.m_protocol.GetSendDataSet("get_CMS_display");
           // else if(currentDispalyDataset.Tables[0].Rows["func_name"].ToString()==

            
                SendPackage pkg = this.m_protocol.GetSendPackage(ds, 0xffff);
                this.Send(pkg);
                if (pkg.result != CmdResult.ACK)
                    throw new Exception(this.DeviceName+","+ pkg.result.ToString());


                     if(IsModuleErr(pkg.ReturnTextPackage))
                         throw new Exception("DisplayModuleError");
             


             //   ds.Dispose();
                //if(this.DeviceName=="CMS-N6-E-16.3")
                //{
                //    Console.WriteLine("Send:" + pkg.ToString());
                //    Console.WriteLine("return:" + pkg.ReturnTextPackage.ToString());

                //    }
                ds = m_protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
                return ds;
            
           
        }

      private bool IsModuleErr(TextPackage txt)
      {
          int msglength = 0, msginx = 0, msglenginx = 0;

          if (txt.Text[0] == 0x04 && txt.Text[7] == 0x57 && txt.Text[8] == 0)

              msglenginx = 9;

          else if (txt.Text[0] == 0x04 && txt.Text[7] == 0x5f && txt.Text[8] == 0x57 && (txt.Text[9] == 1 || txt.Text[9] == 4))
              msglenginx = 12;
          else if (txt.Text[0] == 0x5b && txt.Text[8] == 0)
              msglenginx = 11;
          else if (txt.Text[0] == 0x5a && txt.Text[8] == 0)
              msglenginx = 9;

          if (msglenginx != 0)
          {
              msglength = txt.Text[msglenginx];
              msginx = msglenginx + 1 + txt.Text[msglenginx + 1] + 2 + 1;
              for (int i = msginx; i < msginx + msglength; i++)
                  if (txt.Text[i] == 0)
                         return true; //throw new Exception("DisplayModuleError");
          }

          return false;
      }


      

      public override void TC_SetDisplayOff()
      {
          checkConntected();
          SendPackage pk = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0x53 });
          this.m_device.Send(pk);
          if (pk.result != CmdResult.ACK)
              Console.WriteLine(m_deviceName + ":set display off" + pk.result);

             lock (currDispLockObj)
              {
                  if (currentDispalyDataset != null)
                  
                      this.InvokeOutPutChangeEvent(this, "熄滅");
                  
                  curr_icon_id = 0;
                curr_g_code_id = 0;
                curr_hor_space = 0;
                curr_mesg = "";

                  this.currentDispalyDataset = null;
              }
          
      }

      public void TC_SetIconPic(int icon_id, string desc, Bitmap pic)
      {
          if (this.m_protocol.version != "3.0")
              throw new Exception("device do not support this command!");

          checkConntected();
          Comm.RGS30_Extend.SetIconPic(this.m_device, 0xffff, (byte)icon_id, desc, pic);
      }

      public Bitmap TC_GetIconPic(int icon_id, ref string desc)
      {
          if (this.m_protocol.version != "3.0")
              throw new Exception("device do not support this command!");
          checkConntected();
          return Comm.RGS30_Extend.GetIconPic(this.m_device, 0xffff, (byte)icon_id, ref desc);
      }

        public override void OneMinTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            base.OneMinTimer_Elapsed(sender, e);

            

        }
    }
}
