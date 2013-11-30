using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HWStatus;
using RemoteInterface;
namespace Comm.TC
{
      public class TTSTC:OutputTCBase
    {
     //       private SendPackage currentDisplayPackage;
    //  private System.Data.DataSet currentDispalyDataset;
         
          private byte[][] currDisplayBytes = new byte[][] {new byte[] { 0x20, 0x20, 0x20,0x00 },new byte[] { 0x20, 0x20, 0x20,0x00 },new byte[] { 0x20, 0x20, 0x20 ,0x00} };
        //  private byte[] currdisplayColors = new byte[] { 0x0, 0x0, 0x0 };
    //  private string curr_mesg;
          public TTSTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus, byte comm_state)
          : base(protocol, devicename, ip, port, deviceid, hw_status,opmode,opstatus,  comm_state)
        {

          //  this.OnConnectStatusChanged += new ConnectStatusChangeHandler(WISTC_OnConnectStatusChanged);



            this.OnTCReport += new OnTCReportHandler(TTSTC_OnTCReport);
            this.OnTCReceiveText += new OnTCReportHandler(TTSTC_OnTCReceiveText);
          
            
        }

          void TTSTC_OnTCReceiveText(object tc, TextPackage txt)
          {
              //throw new Exception("The method or operation is not implemented.");
              if (txt.Text[0] == 0x5f && txt.Text[1] == 0x16)
                  TTS_LedTest_Report(txt.Text, this.DeviceName);

          }


          private void TTS_LedTest_Report(byte[] txt, string devname)//5F16     //5F 16 01 40 00 02 00 0F 00 00
          {
              int ErrorSum = 0;
              string SqlLedTest = "";
              string SqlLedTestDetail = "";

              if (txt[8] != 0)
                  ErrorSum = txt[8] * 256 + txt[9];
              else
                  ErrorSum = txt[9];

              DateTime t = DateTime.Now;

              if (ErrorSum != 0)
              {
                  SqlLedTest = "INSERT INTO db2inst1.TBLLEDTest_TTS(DEVICENAME,TIMESTAMP,Display_position,Light_status,Bad_set_no) VALUES('" + devname + "','" + RemoteInterface.DbCmdServer.getTimeStampString(t) + "'," + txt[2] + "," + txt[7] + "," + ErrorSum + ") ";
                  this.InvokeDBDemand(SqlLedTest);
                  for (int i = 10; i < txt.Length; i = i + 2)
                  {
                      SqlLedTestDetail = "INSERT INTO db2inst1.tblLEDTestDetail_TTS(DEVICENAME,TIMESTAMP,Display_position,Led_x,Led_y) VALUES('" + devname + "','" + RemoteInterface.DbCmdServer.getTimeStampString(t) + "'," + txt[2] + "," + txt[i] + "," + txt[i + 1] + ")";
                      this.InvokeDBDemand(SqlLedTestDetail);
                  }
              }
          }

          void TTSTC_OnTCReport(object tc, TextPackage txt)
          {
             // TTS-N1-S-248.2


#if DEBUG

              if ((tc as TCBase).DeviceName != "TTS-N1-N-216.7")
                  return;

#endif
              if (txt.Text[0] == 0x5f && txt.Text[1] == 0x1a)
              {

                  try
                  {
                      lock (this.currDispLockObj)
                      {
                          byte[][] data = new byte[3][];

                          for (int i = 0; i < 3; i++)
                          {
                              data[i] = new byte[4];
                              System.Array.Copy(currDisplayBytes[i], data[i], 4);

                          }

                          int position = txt.Text[2];
                          data[position - 1][0] = txt.Text[10];
                          data[position - 1][1] = txt.Text[11];
                          data[position - 1][2] = txt.Text[12];


                          this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), GetDisplayDesc(data));
                      }
                  }
                  catch
                  {
                      ;
                  }

                
                 


                   
              }

              //throw new Exception("The method or operation is not implemented.");
          }
      public override void DownLoadConfig()
      {
        //  throw new Exception("The method or operation is not implemented.");
      }


       public void TC_SetDisplayOff(byte boardid)
       {

           checkConntected();
           if (currDisplayBytes[boardid - 1][3] != 0x00)
           {
               SendPackage pk = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0x5f, 0x17, boardid, 0x20, 0x20, 0x20, 0x00 });
               this.m_device.Send(pk);
               currDisplayBytes[boardid - 1][0] = 0x20;
               currDisplayBytes[boardid - 1][1] = 0x20;
               currDisplayBytes[boardid - 1][2] = 0x20;
               currDisplayBytes[boardid - 1][3] = 0x00;
               this.InvokeOutPutChangeEvent(this, this.GetCurrentDisplayDecs());
           }

        
       }
      public override void  TC_SetDisplayOff()
      {
          checkConntected();

          SendPackage pk = null;

          if (currDisplayBytes[0][3] != 0x00)
          {
               pk = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0x5f, 0x17, 1, 0x20, 0x20, 0x20, 0x00 });
              this.m_device.Send(pk);
          }

          if (currDisplayBytes[1][3] != 0x00)
          {
              pk = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0x5f, 0x17, 2, 0x20, 0x20, 0x20, 0x00 });
              this.m_device.Send(pk);
          }

          if (currDisplayBytes[2][3] != 0x00)
          {
              pk = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0x5f, 0x17, 3, 0x20, 0x20, 0x20, 0x00 });
              this.m_device.Send(pk);
          }
          //if (pk.result != CmdResult.ACK)
          //    Console.WriteLine(m_deviceName + ":set display off" + pk.result);
          //if (pk.result != CmdResult.ACK)
          //    Console.WriteLine(m_deviceName + ":set display off" + pk.result);
          //if (pk.result != CmdResult.ACK)
          //    Console.WriteLine(m_deviceName + ":set display off" + pk.result);
        //  string str="";
          bool isDifference = false;
          for(int i=0;i<3;i++)
              for(int j=0;j<4;j++)
              {
                  if (j == 3)  //color
                  {
                      if (currDisplayBytes[i][3] != 0x00)
                      {
                        //  this.InvokeOutPutChangeEvent(this, "boardid " + (i + 1) + ":熄滅");
                          isDifference = true;

                      }
                      currDisplayBytes[i][j] = 0x00;
                  }
                  else
                      currDisplayBytes[i][j] = 0x20;
              }

          if (isDifference)
          {
              this.InvokeOutPutChangeEvent(this, "boardid 1:熄滅 boardid 2:熄滅 boardid 3:熄滅 ");
          }
        

      }

      public override string GetCurrentDisplayDecs()
      {
          string ret = "";
          //if (this.currentDispalyDataset == null)
          //    return "熄滅";
          for (int i = 0; i < 3; i++)
          {
              if(currDisplayBytes[i][3]==0)
                  ret += "boardid " + (i + 1) + ":熄滅";
              else
              {
              ret += "boardid " + (i + 1)+":";
              ret += System.Text.ASCIIEncoding.ASCII.GetString(currDisplayBytes[i], 0, 3);
              }
              ret += " ";
            //  ret+="\r\n";
          }
             
          return ret;
          
          //throw new Exception("The method or operation is not implemented.");
      }

          public string GetDisplayDesc(byte[][] data)
          {

              string ret = "";
              //if (this.currentDispalyDataset == null)
              //    return "熄滅";
              for (int i = 0; i < 3; i++)
              {
                  if (data[i][3] == 0)
                      ret += "boardid " + (i + 1) + ":熄滅";
                  else
                  {
                      ret += "boardid " + (i + 1) + ":";
                      ret += System.Text.ASCIIEncoding.ASCII.GetString(data[i], 0, 3);
                  }
                  ret += " ";
                  //  ret+="\r\n";
              }

              return ret;

          }


          public void GetCurrentDispaly(byte boardid, ref string mesg, ref byte color)
          {
             // mesg = System.Text.ASCIIEncoding.ASCII.GetString(currDisplayBytes[boardid - 1], 0, 3);
             // color = currDisplayBytes[boardid - 1][3];
              System.Data.DataSet ds ;
             
                  ds = this.TC_GetDisplay(boardid);
                  byte[] traveltimebytes = new byte[3];
                  traveltimebytes[0] = System.Convert.ToByte(ds.Tables[0].Rows[0]["travel_time1"]);
                  traveltimebytes[1] = System.Convert.ToByte(ds.Tables[0].Rows[0]["travel_time2"]);
                  traveltimebytes[2] = System.Convert.ToByte(ds.Tables[0].Rows[0]["travel_time3"]);

                  mesg = System.Text.ASCIIEncoding.ASCII.GetString(traveltimebytes, 0, 3);
                  color = System.Convert.ToByte(ds.Tables[0].Rows[0]["color"]);
          }

          public string GetDisplayDesc(System.Data.DataSet ds)
      {
          string ret="";
          byte[] travelData=new byte[3];
          ret = "boardid " + ds.Tables[0].Rows[0]["display_position"] + ":";
          travelData[0] = System.Convert.ToByte(ds.Tables[0].Rows[0]["travel_time1"]);
          travelData[1] = System.Convert.ToByte(ds.Tables[0].Rows[0]["travel_time2"]);
          travelData[2] = System.Convert.ToByte(ds.Tables[0].Rows[0]["travel_time3"]);
          ret += "travel_time:" + System.Text.ASCIIEncoding.ASCII.GetString(travelData);

          return ret;
      }

         

      protected override void CheckDisplayTask()
      {
           System.Data.DataSet ds;
           bool isEqual = true;
          // if (currentDispalyDataset == null || !IsConnected) return;
          try
          {

          //  System.Data.DataSet ds=this.TC_GetDisplay();
              if (!this.IsConnected)
                  return;
            lock(this.currDispLockObj)
            {

                string dev_display = "";
                byte[] time = new byte[3];
               // bool isWrong = false;
                for (int i = 0; i < 3; i++)
                {
                    ds = this.TC_GetDisplay(i+1);
                    for (int j = 0; j < 3; j++)
                    {
                        isEqual = isEqual && (currDisplayBytes[i][j] == System.Convert.ToByte(ds.Tables[0].Rows[0]["travel_time" + (j + 1)]));
                        time[j] = System.Convert.ToByte(ds.Tables[0].Rows[0]["travel_time" + (j + 1)]);
                    }
                    if (!isEqual)
                    {
                       
                        dev_display += "boardid" + (i + 1) + ":" + System.Text.ASCIIEncoding.ASCII.GetString(time, 0, 3);
                      
                     //   this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), GetDisplayDesc(ds));
                        SendPackage pk = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0x5f, 0x17, (byte)(i + 1), currDisplayBytes[i][0], currDisplayBytes[i][1], currDisplayBytes[i][2], currDisplayBytes[i][3] });
                        this.m_device.Send(pk);
                        
                    }
                }

                if (!isEqual)
                {
                    ConsoleServer.WriteLine(this.DeviceName + " 顯示資料比對錯誤!" + GetCurrentDisplayDecs() + "," + dev_display);
                    this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), dev_display);
                }
                else
                {
                    this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), GetCurrentDisplayDecs());
                }


               
            //bool isEqual = true;
            //for (int i = currentDispalyDataset.Tables[0].Columns.IndexOf("data_type"); i < currentDispalyDataset.Tables[0].Columns.Count; i++)
            //{
            //    isEqual = isEqual && (currentDispalyDataset.Tables[0].Rows[0][i].Equals(ds.Tables[0].Rows[0][currentDispalyDataset.Tables[0].Columns[i].ColumnName]));
            //}

                
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
            return new TTS_HW_StatusDesc(this.DeviceName,m_hwstaus);
        }

        
        public  void TC_SendDisplay(byte boardid,string traveltime,  byte color) 
            
        {
            lock (this.currDispLockObj)
            {
                byte[] travelbytes = System.Text.ASCIIEncoding.ASCII.GetBytes(traveltime);
                byte[] comdata = new byte[] { 0x5f, 0x17, boardid, travelbytes[0], travelbytes[1], travelbytes[2], color };
                bool isEqual = true;

                isEqual = isEqual && (currDisplayBytes[boardid - 1][0] == travelbytes[0]);
                isEqual = isEqual && (currDisplayBytes[boardid - 1][1] == travelbytes[1]);
                isEqual = isEqual && (currDisplayBytes[boardid - 1][2] == travelbytes[2]);

                isEqual=isEqual &&(color==currDisplayBytes[boardid -1][3]);
                if (!isEqual)
                {

                    try
                    {
                        SendPackage pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, comdata);

                        System.Array.Copy(comdata, 3, currDisplayBytes[boardid - 1], 0, 4);

                        this.Send(pkg);

                        currDisplayBytes[boardid - 1][0] = travelbytes[0];
                        currDisplayBytes[boardid - 1][1] = travelbytes[1];
                        currDisplayBytes[boardid - 1][2] = travelbytes[2];
                    }
                    catch (Exception ex)
                    {
                        ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                    }

                    this.InvokeOutPutChangeEvent(this, this.GetCurrentDisplayDecs());
                }
            }
          
            


        }

        public System.Data.DataSet TC_GetDisplay(int boardid)
        {


            System.Data.DataSet ds = this.m_protocol.GetSendDataSet("get_mntr");

            ds.Tables[0].Rows[0]["display_position"] = boardid;
            ds.AcceptChanges();
                SendPackage pkg = this.m_protocol.GetSendPackage(ds, 0xffff);
                this.Send(pkg);
                

               // ds.Dispose();
                ds = m_protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
                return ds;
            
           
        }


      

        public override void OneMinTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            base.OneMinTimer_Elapsed(sender, e);
            if (DateTime.Now.Minute % 5 != 0)
                return;
            DateTime dt = DateTime.Now.AddSeconds(-DateTime.Now.Second);
            string[] dev_display = new string[] { "NA", "NA", "NA" };
            byte[] time = new byte[3];
            for (int i = 0; i < 3; i++)
            {
              System.Data.DataSet  ds = this.TC_GetDisplay(i + 1);
              try
              {
                  for (int j = 0; j < 3; j++)
                  {
                      //  isEqual = isEqual && (currDisplayBytes[i][j] == System.Convert.ToByte(ds.Tables[0].Rows[0]["travel_time" + (j + 1)]));
                      time[j] = System.Convert.ToByte(ds.Tables[0].Rows[0]["travel_time" + (j + 1)]);
                  }
                  dev_display[i] = System.Text.ASCIIEncoding.ASCII.GetString(time, 0, 3);
               

              }
              catch (Exception ex)
              {
                  ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
              }


              string sql = "update tblTTStravelTimeCheck5Min set DIV1_DISPLAYTIME='{0}' ,DIV2_DISPLAYTIME='{1}', DIV3_DISPLAYTIME='{2}' where devicename='{3}' and timestamp='{4}'";

              this.InvokeDBDemand(string.Format(sql, dev_display[0], dev_display[1], dev_display[2], this.DeviceName, DbCmdServer.getTimeStampString(dt)));

  
 
                //if (!isEqual)
                //{

                //    dev_display += "boardid" + (i + 1) + ":" + System.Text.ASCIIEncoding.ASCII.GetString(time, 0, 3);

                //    //   this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), GetDisplayDesc(ds));
                //    SendPackage pk = new SendPackage(CmdType.CmdSet, CmdClass.A, m_deviceid, new byte[] { 0x5f, 0x17, (byte)(i + 1), currDisplayBytes[i][0], currDisplayBytes[i][1], currDisplayBytes[i][2], currDisplayBytes[i][3] });
                //    this.m_device.Send(pk);

                //}
            }

        }
    }
}
