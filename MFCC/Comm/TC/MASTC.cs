using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using RemoteInterface.HWStatus;
using RemoteInterface;

namespace Comm.TC
{
    public class MASTC : OutputTCBase
    {
        DataSet[] currentDispalyDataset=new DataSet[3];
        SendPackage []currentDisplayPackage=new SendPackage[3];
        private int[] curr_g_code_id=new int[3], curr_hor_space=new int [3];
        private string[] curr_mesg = new string[] { "", "", "", "", "", "" };
        private int[] curr_speedlimits =new int[]{ -1, -1, -1 };
        public MASTC(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus, byte comm_state)
          : base(protocol, devicename, ip, port, deviceid, hw_status,opmode,opstatus,comm_state)
        {



            this.OnTCReceiveText += new OnTCReportHandler(MASTC_OnTCReceiveText);
            
        }

        void MASTC_OnTCReceiveText(object tc, TextPackage txt)
        {
            if (txt.Text[0] == 0x5f && txt.Text[1] == 0x26)
                MAS_LedTest_Report(txt.Text, this.DeviceName);
            //throw new Exception("The method or operation is not implemented.");
        }
        void MAS_LedTest_Report(byte[] txt, string devname)//5F26
        {
            int ErrorSum = 0;
            string SqlLedTest = "";
            string SqlLedTestDetail = "";

            if (txt[7] != 0)
                ErrorSum = txt[7] * 256 + txt[8];
            else
                ErrorSum = txt[8];

            DateTime t = DateTime.Now;

            if (ErrorSum != 0)
            {
                SqlLedTest = "INSERT INTO db2inst1.TBLLEDTest_MAS(DEVICENAME,TIMESTAMP,LANE_ID,BADSETNO) VALUES('" + devname + "','" + RemoteInterface.DbCmdServer.getTimeStampString(t) + "'," + txt[2] + "," + ErrorSum + ") ";
                this.InvokeDBDemand(SqlLedTest);
                for (int i = 9; i < txt.Length; i = i + 2)
                {
                    SqlLedTestDetail = "INSERT INTO db2inst1.tblLEDTestDetail_MAS(DEVICENAME,TIMESTAMP,LANE_ID,X,Y) VALUES('" + devname + "','" + RemoteInterface.DbCmdServer.getTimeStampString(t) + "'," + txt[2] + "," + txt[i] + "," + txt[i + 1] + ")";
                    this.InvokeDBDemand(SqlLedTestDetail);
                }
            }
        }


       
      public override void DownLoadConfig()
      {
          throw new Exception("The method or operation is not implemented.");


      }


      public override void TC_SetDisplayOff()
      {
          lock (this.currDispLockObj)
          {
              for (byte i = 1; i <= 3; i++)
              {
                  TC_SetDisplayOff(i);
                  //currentDispalyDataset[i - 1] = null;
                  //curr_speedlimits[i - 1] = -1;
                  //curr_mesg[i - 1] = "";
                  //curr_g_code_id[i - 1] = 0;
              }
              
          }
       
      }

        public void TC_SetDisplayOff(byte laneid)
        {
            SendPackage pkg;
            byte[] data = new byte[] { 0x5f, 0x23, laneid };
            pkg = new SendPackage(CmdType.CmdSet, CmdClass.A, 0xffff, data);
            this.Send(pkg);
            //lock (currDispLockObj)
            //{
            if (currentDispalyDataset[laneid - 1] != null)
                InvokeOutPutChangeEvent(this, "laneid:" + laneid + " 熄滅");
                currentDispalyDataset[laneid - 1] = null;
                currentDisplayPackage[laneid - 1] = pkg;
                curr_speedlimits[laneid - 1] = -1;
                curr_mesg[laneid - 1] = "";
                curr_g_code_id[laneid - 1] = 0;
            //}
        }

        public void TC_SendDisplay(int laneid, int g_code_id, int hor_space, string mesgs, byte[] colors, byte[] vspaces)
     
        {
  
            string mesg;
            
                    mesg = mesgs.Replace("\r", "\r\n");
           


            int ver_no = 1;
            byte[] big5bytes;
          

            this.checkConntected();

           




            //else  // old version cmd
            //{
                if (g_code_id == 0)
                {

                    System.Data.DataSet ds = this.m_protocol.GetSendDataSet("set_display_text");
                    // ds.Tables["tblMain"].Rows[0]["data_type"] = 0; //text

                    big5bytes = RemoteInterface.Util.StringToBig5Bytes(mesg);

                    ds.Tables["tblMain"].Rows[0]["msgcnt"] = ds.Tables["tblMain"].Rows[0]["msg_length"] = big5bytes.Length;
                    for (int i = 0; i < big5bytes.Length; i++)
                        if (big5bytes[i] == 0x0d)
                            ver_no++;

                    // ds.Tables["tblMain"].Rows[0]["ver_no"] = ver_no;
                    ds.Tables[0].Rows[0]["lane_id"] = laneid;
                    ds.Tables["tblMain"].Rows[0]["ver_no"] = vspaces.Length;
                    ds.Tables["tblMain"].Rows[0]["hor_space"] = hor_space;
                    for (int i = 0; i < vspaces.Length; i++)
                        ds.Tables["tblver_no"].Rows.Add(vspaces[i]);

                    for (int i = 0; i < big5bytes.Length; i++)
                        ds.Tables["tblmsgcnt"].Rows.Add(big5bytes[i]);



                    string mesg1 = mesg.Replace("\r", "").Replace("\n", "");
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

                    ds.AcceptChanges();
                    lock (currDispLockObj)
                    {
                        currentDispalyDataset[laneid-1] = ds;


                        currentDisplayPackage [laneid-1]= this.m_protocol.GetSendPackage(ds, 0xffff);
                        this.m_device.Send(currentDisplayPackage[laneid - 1]);
                        //    ConsoleServer.WriteLine("CMS:"+currentDisplayPackage.ToString());
                    }
                }
                else  // g_code_id
                {
                    System.Data.DataSet ds = this.m_protocol.GetSendDataSet("set_display_graph");
                    ds.Tables[0].Rows[0]["g_code_id"] = g_code_id;
                    ds.Tables[0].Rows[0]["lane_id"] = laneid;
                    ds.AcceptChanges();

                    lock (currDispLockObj)
                    {

                        currentDispalyDataset[laneid-1] = ds;



                        currentDisplayPackage[laneid-1] = this.m_protocol.GetSendPackage(ds, 0xffff);
                        this.m_device.Send(currentDisplayPackage[laneid-1]);
                    }
                }
                // ds.Dispose();


         //   }

            if ( curr_g_code_id[laneid-1] != g_code_id || curr_mesg[laneid-1] != mesg)
            {
               
                curr_g_code_id[laneid-1] = g_code_id;
                curr_hor_space[laneid-1] = hor_space;
                curr_mesg[laneid-1] = mesg;
                curr_speedlimits[laneid - 1] = -1;
                this.InvokeOutPutChangeEvent(this, this.GetCurrentDisplayDecs());
            }



        }

        public void TC_SendDisplay(int laneid, int speed)
        {

            DataSet ds = this.m_protocol.GetSendDataSet("set_mas_speed_limit_display");
            ds.Tables[0].Rows[0]["lane_id"] = laneid;
            ds.Tables[0].Rows[0]["speed_limit"] = speed;
            ds.AcceptChanges();

            if (speed != curr_speedlimits[laneid - 1])
            {

                SendPackage pkg = m_protocol.GetSendPackage(ds, 0xffff);
                this.Send(pkg);
                lock (currDispLockObj)
                {
                    currentDispalyDataset[laneid - 1] = ds;
                    currentDisplayPackage[laneid - 1] = pkg;
                    curr_speedlimits[laneid - 1] = speed;
                    curr_mesg[laneid - 1] = "";
                    curr_g_code_id[laneid - 1] = 0;
                    this.InvokeOutPutChangeEvent(this, this.GetCurrentDisplayDecs());
                }
            }
           // speedlimits[laneid - 1] = speed;
        }

        public void GetCurrentDisplay(int laneid, ref int g_code_id, ref string mesg, ref int speed)
        {
            if (currentDispalyDataset == null)
            {
                g_code_id =0;
                mesg = "";
            }
            else
            {
                g_code_id = curr_g_code_id[laneid-1];
                mesg = curr_mesg[laneid-1];
                speed = curr_speedlimits[laneid - 1];
            }
        }

        public System.Data.DataSet TC_GetDisplay(byte laneid)
        {


            System.Data.DataSet ds = null; ;


            ds = this.m_protocol.GetSendDataSet("get_mas_status");


            ds.Tables[0].Rows[0]["lane_id"] = laneid
                ;
            SendPackage pkg = this.m_protocol.GetSendPackage(ds, 0xffff);
            this.Send(pkg);
            if (pkg.result != CmdResult.ACK)
                throw new Exception(this.DeviceName + "," + pkg.result.ToString());
            ds = m_protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
            return ds;
           // return ds;

            /*
            if (curr_speedlimits[laneid - 1] == -1)
            {
                ds = this.m_protocol.GetSendDataSet("get_mas_status");


                ds.Tables[0].Rows[0]["lane_id"] = laneid
                    ;
                SendPackage pkg = this.m_protocol.GetSendPackage(ds, 0xffff);
                this.Send(pkg);
                if (pkg.result != CmdResult.ACK)
                    throw new Exception(this.DeviceName + "," + pkg.result.ToString());


                //if (IsModuleErr(pkg.ReturnTextPackage))
                //    throw new Exception("DisplayModuleError");



                ds = m_protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
                return ds;
            }
            else
            {
                ds = this.m_protocol.GetSendDataSet("get_MAS_all_display");


                ds.Tables[0].Rows[0]["lane_id"] = laneid
                    ;
                SendPackage pkg = this.m_protocol.GetSendPackage(ds, 0xffff);
                this.Send(pkg);
                if (pkg.result != CmdResult.ACK)
                    throw new Exception(this.DeviceName + "," + pkg.result.ToString());


                //if (IsModuleErr(pkg.ReturnTextPackage))
                //    throw new Exception("DisplayModuleError");



                ds = m_protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
                return ds;
            }
             * */


        }

        public override string GetCurrentDisplayDecs()
        {
          string ret="";
          for (int i = 0; i < 3; i++)
          {
              ret += GetCurrentDisplayDecs(i + 1);


          }


          return ret;
      }
        public  string GetCurrentDisplayDecs(int laneid)
        {
            string ret = "";
            //for (int i = laneid; i <=laneid 6; i++)
            //{
            int i = laneid - 1;
                ret += "laneid:" + (i + 1);
                if (currentDispalyDataset[i] == null)
                    ret += "熄滅";
                else if (curr_speedlimits[i] != -1)
                    ret +=  " speedlimit" + curr_speedlimits[i];
                else if (curr_g_code_id[i] != 0)
                    ret += "g_code_id:" + curr_g_code_id[i];
                else
                    ret += " mesg:" + curr_mesg[i].Trim(new char[] { '\r' });

                ret += " ";


          //  }


            return ret;
        }

        protected override void CheckDisplayTask()
      {
         // throw new Exception("The method or operation is not implemented.");
          if ( !IsConnected) return;
          //if (!this.IsConnected)
          //    return;
          for (byte laneid = 1; laneid <= 3; laneid++)
          {
              if (currentDispalyDataset[laneid - 1] == null)
                  continue;
              try
              {
             

                  System.Data.DataSet ds = this.TC_GetDisplay(laneid);
                  if (  System.Convert.ToInt32(ds.Tables[0].Rows[0]["data_type"]) != 2)
                  {
                      bool isEqual = true;
                      if (ds == null) return;

                      isEqual = IsEqualToCurrentDisplay(laneid, ds);

                      if (!isEqual)
                      {
                          ConsoleServer.WriteLine(this.DeviceName + " 顯示資料比對錯誤!");
                          this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), GetDisplayDesc(ds));

                          this.m_device.Send(this.m_protocol.GetSendPackage(currentDispalyDataset[laneid - 1], 0xffff));
                      }
                      else
                      {
                          this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), GetCurrentDisplayDecs());
                      }
                  }
                  else
                  {
                      int spdlmt = System.Convert.ToInt32(ds.Tables[0].Rows[0]["pre_speed"]);
                      if (spdlmt != curr_speedlimits[laneid - 1])
                      {
                          ConsoleServer.WriteLine(this.DeviceName + " 顯示資料比對錯誤!");
                          this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), GetDisplayDesc(ds));

                          this.m_device.Send(this.m_protocol.GetSendPackage(currentDispalyDataset[laneid - 1], 0xffff));
                      }
                      else
                      {
                          this.InvokeOutPutWrongEvent(GetCurrentDisplayDecs(), GetCurrentDisplayDecs());
                      }

                  }
              }
              catch (Exception ex)
              {
                  if (ex.Message == "DisplayModuleError")
                      ConsoleServer.WriteLine(this.DeviceName + "," + "顯示模組錯誤");
                  else
                      ConsoleServer.WriteLine("in check display task" + this.DeviceName + ex.Message + ex.StackTrace);
              }

        }
      }

        private bool IsEqualToCurrentDisplay(byte laneid,System.Data.DataSet ds)
        {
            //  if (currentDispalyDataset == null || !IsConnected) return;
            bool isEqual = true;
            lock (currDispLockObj)
            {
                for (int i = ds.Tables[0].Columns.IndexOf("data_type"); i < ds.Tables[0].Columns.Count; i++)
                {
                    if (ds.Tables[0].Columns[i].ColumnName == "pre_speed" || ds.Tables[0].Columns[i].ColumnName == "goal_speed")
                        continue;
                    if (currentDispalyDataset[laneid-1].Tables[0].Rows[0]["data_type"].ToString() == "0" && (ds.Tables[0].Columns[i].ColumnName == "g_code_id" || ds.Tables[0].Columns[i].ColumnName.StartsWith("g_desc")))
                        continue;
                    if (currentDispalyDataset[laneid-1].Tables[0].Rows[0]["data_type"].ToString() == "1" && ds.Tables[0].Columns[i].ColumnName != "g_code_id")
                        continue;
                    //if (currentDispalyDataset.Tables[0].Rows[0]["func_name"].ToString() == "set_display_control" && ds.Tables[0].Columns[i].ColumnName == "g_code_id")
                    //    continue;

                    if (currentDispalyDataset[laneid-1].Tables[0].Rows[0][ds.Tables[0].Columns[i].ColumnName] != System.DBNull.Value && ds.Tables[0].Rows[0][i] != System.DBNull.Value)

                        isEqual = isEqual && (currentDispalyDataset[laneid-1].Tables[0].Rows[0][ds.Tables[0].Columns[i].ColumnName].Equals(ds.Tables[0].Rows[0][i]));

                    if (isEqual)
                    {
                        for (int j = 0; j < ds.Tables["tblmsgcnt"].Rows.Count; j++)
                            if (isEqual && System.Convert.ToInt32(ds.Tables["tblmsgcnt"].Rows[j]["message"]) != 0)
                                isEqual = isEqual && ds.Tables["tblmsgcnt"].Rows[j]["message"].ToString() == currentDispalyDataset[laneid-1].Tables["tblmsgcnt"].Rows[j]["message"].ToString();
                    }

                }


            }
            // this.OnOutputDataCompareWrongEvent(this, currentDispalyDataset);

            return isEqual;

        }

        public string GetDisplayDesc(System.Data.DataSet ds)
        {
            if (System.Convert.ToInt32(  ds.Tables[0].Rows[0]["data_type"]) !=2)  //not speed limit mode
            {
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
            else

                return "speed Limit:" + ds.Tables[0].Rows[0]["pre_speed"].ToString();

        }


      public override RemoteInterface.I_HW_Status_Desc getStatusDesc()
      {
          return new MAS_HW_StatusDesc(this.m_deviceName, m_hwstaus);
        //  throw new Exception("The method or operation is not implemented.");
      }
  }
}
