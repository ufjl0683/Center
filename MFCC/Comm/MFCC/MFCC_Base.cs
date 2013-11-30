using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.HC;
using RemoteInterface.MFCC;
using System.Data;

namespace Comm.MFCC
{
  public abstract class MFCC_Base
    {
        public RemoteInterface.HC.I_HC_Comm r_host_comm;
        protected System.Collections.ArrayList     tcAry=System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList());
        protected Comm.Protocol protocol;
        protected Comm.MFCC.TC_Manager manager;
         string devType;
        int remotePort;
        int notifyPort;
        int consolePort;
        string regRemoteName;
        protected string mfccid;
        Type regRemoteType;
        protected RemoteInterface.EventNotifyServer notifier;
        protected DbCmdServer dbServer = new DbCmdServer();
        public bool IsLoadTcCompleted = false;
      private ExactIntervalTimer tmrSunSet;
      System.Timers.Timer tmr1min = new System.Timers.Timer(1000 * 60);

      //protected DbCmdServer dbServer;
         public MFCC_Base(string mfccid,string devType,int remotePort,int notifyPort,int consolePort,string regRemoteName,Type regRemoteType)
        {
            Comm.DB2.Db2.MFCC_TYPE = mfccid;
            this.devType = devType;
            this.remotePort = remotePort;
            this.notifyPort = notifyPort;
            this.consolePort = consolePort;
            this.regRemoteName = regRemoteName;
            this.regRemoteType = regRemoteType;
            this.mfccid = mfccid;

            this.tmr1min.Elapsed += new System.Timers.ElapsedEventHandler(tmr1min_Elapsed);
            tmr1min.Start();
       //      dbServer = new DbCmdServer();
            init_RemoteInterface();

            try
            {
                if(mfccid!="MFCC_PBX")
                load_protocol();
            }
            catch
            {
                
                ConsoleServer.WriteLine("Loading Protocol error");
                System.Environment.Exit(-1);
            }
            notifier = new EventNotifyServer(notifyPort);
          
            ConsoleServer.WriteLine("loading Tc ...");
            loadTC_AndBuildManaer();
            IsLoadTcCompleted = true;
            this.AfterDeviceAllStart();
            ConsoleServer.WriteLine("load Tc Completed!");
          

           
            check_and_connect_remote_obj(r_host_comm);

            new System.Threading.Thread(HW_StatusDBTask).Start();

          //  new System.Threading.Thread(updateStstask).Start();

            if (!(this is MFCC.MFCC_DataColloetBase))
            {
                tmrSunSet = new ExactIntervalTimer(0,0, 0);
                tmrSunSet.OnElapsed += new OnConnectEventHandler(tmrSunSet_OnElapsed);
            }

        }

       volatile bool isInTmr = false;
      void tmr1min_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
      {
          //throw new Exception("The method or operation is not implemented.");
          if (isInTmr)
              return;



          try
          {
              isInTmr = true;
              if (r_host_comm == null) return;
              ((RemoteClassBase)r_host_comm).HelloWorld();
          }
          catch
          {
              r_host_comm = null;
              new System.Threading.Thread(RemoteObjectConnectTask).Start();

          }
          finally
          {
              isInTmr = false;
          }

         
      }

      void tmrSunSet_OnElapsed(object sender)
      {

          System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(DbCmdServer.getDbConnectStr());


          System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();

       //   DateTime uptime=null, settime=null;
          try
          {
              cmd.Connection = cn;
              cmd.CommandText = string.Format("select sunuptime,sunsettime from tblSysSunUp where sundate='{0:00}{1:00}'", System.DateTime.Now.Month, System.DateTime.Now.Day);
              cn.Open();
              System.Data.Odbc.OdbcDataReader rd = cmd.ExecuteReader();
              if (rd.Read())
              {
                     
                  DateTime uptime = System.Convert.ToDateTime(rd[0]);
                  DateTime settime = System.Convert.ToDateTime(rd[1]);

                  foreach (OutputTCBase tc in this.getTcManager().GetDevEnum())
                  {
                      if (tc.IsConnected)
                      {
                          try
                          {
                              tc.setSunRiseSunSet(uptime.Hour, uptime.Minute, settime.Hour, settime.Minute);
                          }
                          catch (Exception ex1)
                          {
                              ConsoleServer.WriteLine(ex1.Message + "," + ex1.StackTrace);
                          }
                      }
                  }



              }
          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
          }

          finally
          {
              cn.Close();
          }

        


          //throw new Exception("The method or operation is not implemented.");
      }

       
   
      void updateStstask()
      {

          System.Threading.Thread.Sleep(1000 * 120);
          foreach (TCBase tc in manager.GetDevEnum())
          {
              dbServer.SendSqlCmd(string.Format("update tbldeviceconfig set comm_state={0},update_time='{2}' where devicename='{1}'",tc.IsConnected?1:3,tc.DeviceName,DbCmdServer.getTimeStampString(DateTime.Now)));
          }

      }


        
           System.Collections.Queue hw_status_db_queue = System.Collections.Queue.Synchronized(new System.Collections.Queue());

      public int getDbServerCurrentQueueCnt()
       {
           return dbServer.getCurrentQueueCnt();
       }


      public void ExecuteSql(string sql)
      {
          this.dbServer.SendSqlCmd(sql);
      }
      protected void downLoadParamToTc(string devName)
      {
          this.getTcManager()[devName].DownLoadConfig();
      }

      
       private void HW_StatusDBTask()
          {
            //  System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
            //  System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();
           //   cmd.Connection = cn;
              
              while(true)
              {
                  if (hw_status_db_queue.Count == 0)
                  {
                      lock (hw_status_db_queue)
                          System.Threading.Monitor.Wait(hw_status_db_queue);
                  }
                  
                      try
                      {
                        //  cn.Open();
                          string statusStr = "";
                          while (hw_status_db_queue.Count != 0)
                          {
                              byte[] hw_status;
                               Comm.MFCC.TC_Status tc_status=  (Comm.MFCC.TC_Status)hw_status_db_queue.Dequeue();
                               I_HW_Status_Desc desc = tc_status.desc;
                               hw_status =desc.getHW_status();      //desc.getHW_status();
                                  System.Collections.IEnumerator ie = desc.getEnum().GetEnumerator();
                                  statusStr = "";
                           
                             //  分開獨立寫入  位元變化
                                  System.DateTime dt = DateTime.Now;
                                  while (ie.MoveNext())
                                  {
                                      int inx = (int)ie.Current;
                                      statusStr += desc.getChiDesc(inx) + ":" + desc.getStatus(inx) + ",";
                                      dbServer.SendSqlCmd(string.Format("insert into tblDeviceStateLog (devicename,timestamp,type,bit,result) values('{0}','{1}','{2}',{3},{4})",
                                          desc.getDeviceName(), DB2.Db2.getTimeStampString(dt), 'H', inx, desc.getStatus(inx) ? 1 : 0));

                                  }
                                  
                                  statusStr = statusStr.TrimEnd(new char[] { ',' });
                               
                                 dbServer.SendSqlCmd( string.Format("update tblDeviceCOnfig set hw_status_1={0},hw_status_2={1},hw_status_3={2},hw_status_4={3},update_time='{5}' where deviceName='{4}'", hw_status[0], hw_status[1], hw_status[2], hw_status[3], desc.getDeviceName(),DbCmdServer.getTimeStampString(dt)));
                               
                               ConsoleServer.WriteLine(desc.getDeviceName()+statusStr);


                               //---------------------------要求下載參數--------------------------
                               if (desc.getStatus(4))
                                   downLoadParamToTc(desc.getDeviceName());



                              }
                            
                      }

                     
                      catch (Exception ex)
                      {
                          ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                      }
                     
                 


              }
          }


      //private void Hw_StatusToDb(RemoteInterface.I_HW_Status_Desc desc)
      //{
          
      //}

        private  void load_protocol()
        {
            string protocol_source = "";
            if (r_host_comm != null)
            {
                try
                {
                    protocol_source = r_host_comm.getScriptSource(devType);
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
                    ConsoleServer.WriteLine("read local protocol.txt");
                    protocol_source = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "protocol.txt").ReadToEnd();

                }

                try
                {
                    protocol = new Protocol();
                    protocol.Parse(protocol_source,true);
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "protocol.txt");
                    sw.Write(protocol_source);
                    sw.Close();
                    return;
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
                }


            }

            else  //r_host_comm fail
            {
                // read local protocol.txt
                protocol_source = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "protocol.txt").ReadToEnd();
            }

            protocol = new Protocol();
            protocol.Parse(protocol_source,true);
        }

        public void check_and_connect_remote_obj(object robj)
        {
            if (robj == null)
                new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Connect_Remote_Host_Comm_Task)).Start(robj);
        }

      public void setDeviceCommMoniter(string devName, bool bStartEnd)
      {


          TCBase tc = (TCBase)this.getTcManager()[devName];
          if (bStartEnd && !tc.m_device.IsOnAckRegisted())
          {
              tc.m_device.OnAck += new OnAckEventHandler(m_device_OnAck);
              tc.m_device.OnBeforeAck += new OnSendingAckNakHandler(m_device_OnBeforeAck);
              tc.m_device.OnReceiveText += new OnTextPackageEventHandler(m_device_OnReceiveText);
              tc.m_device.OnSendingPackage += new OnSendPackgaeHandler(m_device_OnSendingPackage);
              tc.m_device.OnNak += new OnNakEventHandler(m_device_OnNak);
          }

          if (!bStartEnd && tc.m_device.IsOnAckRegisted())
          {
              tc.m_device.OnAck -= new OnAckEventHandler(m_device_OnAck);
              tc.m_device.OnBeforeAck -= new OnSendingAckNakHandler(m_device_OnBeforeAck);
              tc.m_device.OnReceiveText -= new OnTextPackageEventHandler(m_device_OnReceiveText);
              tc.m_device.OnSendingPackage -= new OnSendPackgaeHandler(m_device_OnSendingPackage);
              tc.m_device.OnNak -= new OnNakEventHandler(m_device_OnNak);
          }
         
      }

      void m_device_OnNak(object sender, NakPackage AckObj)
      {
          //throw new Exception("The method or operation is not implemented.");
          try
          {
              this.notifier.NotifyAll(new NotifyEventObject(EventEnumType.MFCC_Comm_Moniter_Event, ((Comm.I_DLE)sender).getDeviceName(),
                 "\t" + DateTime.Now.ToLongTimeString()+","+ AckObj.ToString() + "<=="));
          }
          catch { ;}
      }

      void m_device_OnSendingPackage(object sender, SendPackage pkg)
      {
          try
          {
              this.notifier.NotifyAll(new NotifyEventObject(EventEnumType.MFCC_Comm_Moniter_Event, ((Comm.I_DLE)sender).getDeviceName(),
                   "==>" + DateTime.Now.ToLongTimeString() + "," + pkg));
          }
          catch { ;}
      }

      //public void RemoveCommMoniterRegist(TCBase tc)
      //{
      //    try
      //    {
      //        tc.m_device.OnAck -= new OnAckEventHandler(m_device_OnAck);
      //        tc.m_device.OnBeforeAck -= new OnSendingAckNakHandler(m_device_OnBeforeAck);
      //        tc.m_device.OnSendingPackage -= new OnSendPackgaeHandler(m_device_OnSendingPackage);
      //        tc.m_device.OnReceiveText -= new OnTextPackageEventHandler(m_device_OnReceiveText);
      //        tc.m_device.OnNak -= new OnNakEventHandler(m_device_OnNak);
      //    }
      //    catch { ;}
      //}

     

      void m_device_OnReceiveText(object sender, TextPackage txtObj)
      {
          try
          {
              this.notifier.NotifyAll(new NotifyEventObject(EventEnumType.MFCC_Comm_Moniter_Event, ((Comm.I_DLE)sender).getDeviceName(),
                   "\t" + DateTime.Now.ToLongTimeString() + "," + txtObj.ToString() + "<=="));
          }
          catch (Exception ex){
              ConsoleServer.WriteLine(ex.Message+","+ex.StackTrace);
              ;}
          //throw new Exception("The method or operation is not implemented.");
      }

      void m_device_OnBeforeAck(object sender, ref byte[] data)
      {
          try
          {
              this.notifier.NotifyAll(new NotifyEventObject(EventEnumType.MFCC_Comm_Moniter_Event, ((Comm.I_DLE)sender).getDeviceName(),
                   "Ack==>" + DateTime.Now.ToLongTimeString() + "," + Util.ToHexString(data)));
          }
          catch { ;}
         // throw new Exception("The method or operation is not implemented.");
      }

      void m_device_OnAck(object sender, AckPackage AckObj)
      {
          try
          {
              this.notifier.NotifyAll(new NotifyEventObject(EventEnumType.MFCC_Comm_Moniter_Event, ((Comm.I_DLE)sender).getDeviceName(),
                   "\t" + DateTime.Now.ToLongTimeString() + "," + AckObj.ToString() + "<=="));
          }
          catch { ;}
        //  throw new Exception("The method or operation is not implemented.");
      }

       void Connect_Remote_Host_Comm_Task(object robj)
        {
            while (true)
            {
                try
                {

                    if (r_host_comm == null)
                    {
                        r_host_comm = (I_HC_Comm)RemoteBuilder.GetRemoteObj(typeof(I_HC_Comm), RemoteBuilder.getRemoteUri(RemoteBuilder.getHostIP(), (int)RemotingPortEnum.HOST, "Comm"));
                        if (r_host_comm != null)
                        {
                            ConsoleServer.WriteLine("r_host_comm" + " connected!");
                            return;
                        }
                    }
                  
                }
                catch
                {
                    ConsoleServer.WriteLine("reconnect host remoteObject failed..reconnecting!");
                  
                }



                    System.Threading.Thread.Sleep(10000);

            }

           
        }


       public   virtual void init_RemoteInterface()
        {
            try
            {
                r_host_comm = (I_HC_Comm)RemoteBuilder.GetRemoteObj(typeof(I_HC_Comm), RemoteBuilder.getRemoteUri(RemoteBuilder.getHostIP(), (int)RemotingPortEnum.HOST, "Comm"));

                if (r_host_comm == null)
                    new System.Threading.Thread(RemoteObjectConnectTask).Start();
            }
            catch(Exception ex) {
                ConsoleServer.WriteLine(ex.Message+ex.StackTrace);}


                try
                {
                    RemoteInterface.ServerFactory.SetChannelPort(remotePort);
                    ServerFactory.RegisterRemoteObject(regRemoteType, regRemoteName);
                    ConsoleServer.WriteLine("RemotePort Listen at " + remotePort);
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                }
            ConsoleServer.Start(consolePort);



        }

      volatile bool IsInRemoteObjectConnectTask = false;
      void RemoteObjectConnectTask()
      {
          if (IsInRemoteObjectConnectTask)
              return;
          IsInRemoteObjectConnectTask = true;
          while (true)
          {
              try
              {
                  r_host_comm = (I_HC_Comm)RemoteBuilder.GetRemoteObj(typeof(I_HC_Comm), RemoteBuilder.getRemoteUri(RemoteBuilder.getHostIP(), (int)RemotingPortEnum.HOST, "Comm"));

                  if (r_host_comm != null)
                  {
                      ConsoleServer.WriteLine( "hostrobj connected");
                      break;
                  }
                  System.Threading.Thread.Sleep(5000);
                  ConsoleServer.WriteLine(mfccid + "host robj reconnecting!");
              }
              catch
              {
                  ;
              }

          }

         IsInRemoteObjectConnectTask=false;
      }

      public virtual void loadTC_AndBuildManaer()
      {
          try
          {
              manager = new TC_Manager();
              System.Data.Odbc.OdbcDataReader rd;
              System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);


              rd = Comm.DB2.Db2.getDeviceConfigReader(cn, this.mfccid);

              // Comm.TCBase tc=null;
              while (rd.Read())
              {

                  try
                  {
                      this.AddDevice(rd[0].ToString());
                  }
                  catch (Exception ex)
                  {
                      ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
                  }
                  /*
                  byte[] hw_status = new byte[4];
                  byte opmode, opstatus;
                  for (int i = 0; i < 4; i++)
                      hw_status[i] = System.Convert.ToByte(rd[3 + i]);
                  opmode=System.Convert.ToByte(rd[7]);
                  opstatus=System.Convert.ToByte(rd[8]);
                 
                  if (this.devType == "VD")
                  {
                     tc = new Comm.TC.VDTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status,opmode,opstatus);
                  }

                  else if (this.devType == "RGS")
                  {
                      tc = new Comm.TC.RGSTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status,opmode, opstatus);

                  }
                  else if (this.devType == "RMS")
                  {
                      tc = new Comm.TC.RMSTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status,opmode, opstatus);

                  }
                  else if (this.devType == "CMS")
                  {
                      tc = new Comm.TC.CMSTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus);
                  }
                  else if (this.devType == "WIS")
                  {
                      tc = new Comm.TC.WISTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus);

                  }
                  else if (this.devType == "LCS")
                  {
                      tc = new Comm.TC.LCSTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus);

                  }
                  else if (this.devType == "CSLS")
                  {
                      tc = new Comm.TC.CSLSTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus);
                  }
                  ConsoleServer.WriteLine(string.Format("load tc:{0} ip:{1} port:{2}", rd[0], rd[1], rd[2]));
                  tcAry.Add(tc);
                  tc.OnHwStatusChanged += new HWStatusChangeHandler(tc_OnHwStatusChanged);
                  tc.OnConnectStatusChanged += new ConnectStatusChangeHandler(tc_OnConnectStatusChanged);
                  tc.OnOpModeChanged += new OnOPModeChangeHandler(tc_OnOpModeChanged);
                  tc.OnOpStatusChanged += new OnOPStatusChangeHandler(tc_OnOpStatusChanged);
                 // if(tc is OutputTCBase)

                  if (tc is OutputTCBase)
                  {
                      ((OutputTCBase)tc).OnOutputChanged += new Comm.OnOutputChangedHandler(MFCC_Base_OnOutputChanged);
                      ((OutputTCBase)tc).OnOutputDataCompareWrongEvent += new OnOutputDataCompareWrongEventHandler(MFCC_Base_OnOutputDataCompareWrongEvent);
                  }
                   
                  this.BindEvent(tc);
                   * */
                 
              }
              rd.Close();
              cn.Close();


            

          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(ex.Message+ex.StackTrace);
          }
            
      }

      public void  AddDevice(string devName)
      {

          if (this.manager.IsContains(devName))
              throw new Exception(devName + " alreay in MFCC name list!");

          System.Data.Odbc.OdbcDataReader rd=null;
          System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Comm.DB2.Db2.db2ConnectionStr);
          //rd = Comm.DB2.Db2.getDeviceConfigReader(cn, this.mfccid);
          System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand("select devicename,ip,port ,hw_status_1,hw_status_2,hw_status_3,hw_status_4,op_mode,op_status,comm_state,IsF311z,comparison_cycle,enable from tbldeviceconfig where mfccid='" + mfccid + "'" + " and  devicename='" + devName + "'" , cn);
          Comm.TCBase tc = null;
         // this.dbServer.SendSqlCmd("update tblDeviceConfig set comm_state=3 where devicename='" + devName + "'");
          try
          {
              
              cmd.Connection = cn;
              cn.Open();
              rd = cmd.ExecuteReader();

              if (!rd.Read())
                  throw new Exception("can't find " + devName + "in tblDeviceConfig");

           //   ConsoleServer.WriteLine(string.Format("load tc:{0} ip:{1} port:{2}", rd[0], rd[1], rd[2]));

          byte[] hw_status = new byte[4];
          byte opmode, opstatus;
          byte comm_state;
          bool enable;
          for (int i = 0; i < 4; i++)
              hw_status[i] = System.Convert.ToByte(rd[3 + i]);
          opmode = System.Convert.ToByte(rd[7]);
          opstatus = System.Convert.ToByte(rd[8]);
          comm_state = System.Convert.ToByte(rd[9]);
          enable = (rd[12].ToString() == "Y");
        
          if (this.devType == "VD")
          {
              tc = new Comm.TC.VDTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus,comm_state);
          }

          else if (this.devType == "RGS")
          {
              tc = new Comm.TC.RGSTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus,comm_state);
          }
          else if (this.devType == "RMS")
          {
              tc = new Comm.TC.RMSTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus,comm_state);

          }
          else if (this.devType == "CMS")
          {
              tc = new Comm.TC.CMSTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus,comm_state);

          }
          else if (this.devType == "WIS")
          {
              tc = new Comm.TC.WISTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus,comm_state);

          }
          else if (this.devType == "LCS")
          {
              tc = new Comm.TC.LCSTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus,comm_state);
          }
          else if (this.devType == "CSLS")
          {
              tc = new Comm.TC.CSLSTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus,comm_state);
          }
          else if (this.devType == "AVI")
          {
              tc = new Comm.TC.AVITC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus,comm_state);
          }
          else if (this.devType == "RD")
          {
              tc = new Comm.TC.RDTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus,comm_state);
          }
          else if (this.devType == "VI")
          {
              tc = new Comm.TC.VITC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus,comm_state);
          }
          else if (this.devType == "WD")
          {
              tc = new Comm.TC.WDTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus,comm_state);
          }
          else if (this.devType == "TTS")
          {
              tc = new Comm.TC.TTSTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus,comm_state);
          }
          else if (this.devType == "FS")
          {
              tc = new Comm.TC.FSTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus,comm_state);
          }
          else if (this.devType == "MAS")
          {
              tc = new Comm.TC.MASTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus,comm_state);
          }
          else if (this.devType == "IID")
          {
              tc = new Comm.TC.IIDTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus, comm_state);
          }
          else if (this.devType == "ETTU")
          {
              tc = new Comm.TC.ETTUTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus, comm_state);
          }
          else if (this.devType == "LS")
          {
              tc = new Comm.TC.LSTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus, comm_state);
          }
          else if (this.devType == "TEM")
          {
              tc = new Comm.TC.TEMTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus, comm_state);
          }
          else if (this.devType == "SCM")
          {
              tc = new Comm.TC.SCMTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus, comm_state);
          }
          else if (this.devType == "CMSRST")
          {
              tc = new Comm.TC.CMSRSTTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus, comm_state);
          }
          else if (this.devType == "BS")
          {
              tc = new Comm.TC.BSTC(protocol, rd[0].ToString().Trim(), rd[1].ToString(), (int)rd[2], 0xffff, hw_status, opmode, opstatus, comm_state);
          }
         



          ConsoleServer.WriteLine(string.Format("load tc:{0} ip:{1} port:{2}", rd[0], rd[1], rd[2]));
          if(tc is OutputTCBase)
             ((OutputTCBase)tc).setDisplayCompareCycle( System.Convert.ToInt32(rd[11]));
           
          tcAry.Add(tc);
          tc.OnHwStatusChanged += new HWStatusChangeHandler(tc_OnHwStatusChanged);
          tc.OnConnectStatusChanged += new ConnectStatusChangeHandler(tc_OnConnectStatusChanged);
          tc.OnOpModeChanged += new OnOPModeChangeHandler(tc_OnOpModeChanged);
          tc.OnOpStatusChanged += new OnOPStatusChangeHandler(tc_OnOpStatusChanged);
          tc.OnTCReport += new OnTCReportHandler(tc_OnTCReport);
          tc.OnDbDemand += new OnDbDemandHandler(tc_OnDbDemand);
          tc.IsF311z = (System.Convert.ToString(rd[10]).ToUpper() == "Y") ? true : false;
          tc.IsEnable = enable;
          if (tc is OutputTCBase)
          {
              ((OutputTCBase)tc).OnOutputChanged += new Comm.OnOutputChangedHandler(MFCC_Base_OnOutputChanged);
              ((OutputTCBase)tc).OnOutputDataCompareWrongEvent += new OnOutputDataCompareWrongEventHandler(MFCC_Base_OnOutputDataCompareWrongEvent);
          }
          this.BindEvent(tc);

          manager.AddTC(tc);

      }
      catch (Exception ex)
      {
          ConsoleServer.WriteLine("InAddDevice:" + ex.Message);
          throw new Exception("InAddDevice:" + ex.Message);
      }
      finally
      {
          rd.Close();
          cn.Close();
      }

      }

      void tc_OnDbDemand(object tc, string sql)
      {

          if (!((TCBase)tc).IsEnable)
              return;
          this.dbServer.SendSqlCmd(sql);
         // throw new Exception("The method or operation is not implemented.");
      }

      void tc_OnTCReport(object tc, TextPackage txt)
      {
        
          //throw new Exception("The method or operation is not implemented.");
          if (txt.Text[0] == 0x07)
          {
              
              try
              {
                //  if (r_host_comm == null) return;
                 this.notifier.NotifyAll(new NotifyEventObject( EventEnumType.TC_Manual_Ask_Event,((TCBase)tc).DeviceName,txt.Text[1]));
              }
              catch (Exception ex)
              {
                  ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
              }
          }
      }
      public void Remove(string devname)
      {

          if (!this.manager.IsContains(devname))
              throw new Exception(devname + " not found in mfcc list!");

          manager.Remove(devname);

      }
      void MFCC_Base_OnOutputDataCompareWrongEvent(OutputTCBase tc, string hostStr,string tcStr)
      {
          if (!((TCBase)tc).IsEnable)
              return;
          string sqlStr = "insert into tblDeviceComparisonLog (devicename,timestamp,display,device_display) values('{0}','{1}','{2}','{3}')";
         
          dbServer.SendSqlCmd(string.Format(sqlStr, tc.DeviceName, DB2.Db2.getTimeStampString(System.DateTime.Now), hostStr, tcStr));
        
      }

      void MFCC_Base_OnOutputChanged(OutputTCBase tc, string newOutputStr)
      {
          if (!((TCBase)tc).IsEnable)
              return;

          dbServer.SendSqlCmd("update tblDeviceConfig set display='"+ newOutputStr+"' ,device_display='"+newOutputStr+"',update_time='"+DbCmdServer.getTimeStampString(DateTime.Now)+"' where devicename='"+tc.DeviceName+"'");
       //   dbServer.SendSqlCmd(string.Format("insert into tblDeviceStateLog (devicename,timestamp,type,display) values('{0}','{1}','D','{2}')", tc.DeviceName, DB2.Db2.getTimeStampString(System.DateTime.Now),newOutputStr));
          ConsoleServer.WriteLine(tc.DeviceName + " change to " + newOutputStr);
      }

      void tc_OnOpStatusChanged(object tc, byte opstatus)
      {
          if (!((TCBase)tc).IsEnable)
              return;
          //dbServer.SendSqlCmd(string.Format("insert into tblDeviceStateLog (devicename,timestamp,type,result) values('{0}','{1}','S',{2})",
          //                    ((TCBase)tc).DeviceName, DB2.Db2.getTimeStampString(System.DateTime.Now), opstatus));


          dbServer.SendSqlCmd(string.Format("update tblDeviceCOnfig set op_status={0},update_time='{2}' where deviceName='{1}'", opstatus, ((TCBase)tc).DeviceName,DbCmdServer.getTimeStampString(DateTime.Now)));
          try
          {
              if(r_host_comm!=null)
              r_host_comm.setDeviceStatus(((TCBase)tc).DeviceName, ((TCBase)tc).getHwStaus(), ((TCBase)tc).m_opstatus, ((TCBase)tc).m_opmode, ((TCBase)tc).IsConnected);
          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine("host comm setDeviceStatus:" + ex.Message);
          }
      
      }

      void tc_OnOpModeChanged(object tc, byte opmode)
      {
          if (!((TCBase)tc).IsEnable)
              return;

          //dbServer.SendSqlCmd(string.Format("insert into tblDeviceStateLog (devicename,timestamp,type,result) values('{0}','{1}','M',{2})",
          //                      ((TCBase)tc).DeviceName , DB2.Db2.getTimeStampString(System.DateTime.Now),opmode ));


          dbServer.SendSqlCmd(string.Format("update tblDeviceCOnfig set op_mode={0},update_time='{2}' where deviceName='{1}'", opmode, ((TCBase)tc).DeviceName,DbCmdServer.getTimeStampString(DateTime.Now)));
          try
          {
              if (r_host_comm != null)
              r_host_comm.setDeviceStatus(((TCBase)tc).DeviceName, ((TCBase)tc).getHwStaus(), ((TCBase)tc).m_opstatus, ((TCBase)tc).m_opmode, ((TCBase)tc).IsConnected);
          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine("host comm setDeviceStatus:" + ex.Message);
          }
          
      }

    

      void tc_OnHwStatusChanged(object tcc, byte[] diff,byte opmode,byte opstatus)
      {
          if (!((TCBase)tcc).IsEnable)
              return;
          Comm.TCBase tc = (Comm.TCBase)tcc;
          I_HW_Status_Desc desc = null; ;

         
          if(this.devType=="VD")
              desc = new RemoteInterface.HWStatus.VD_HW_StatusDesc(tc.DeviceName,tc.getHwStaus(), diff);
          else if(this.devType=="RGS")
              desc = new RemoteInterface.HWStatus.RGS_HW_StatusDesc(tc.DeviceName,tc.getHwStaus(), diff); 
          else if(this.devType=="CMS")
              desc = new RemoteInterface.HWStatus.CMS_HW_StatusDesc(tc.DeviceName,tc.getHwStaus(), diff); 
          else if (this.devType == "RMS")
              desc = new RemoteInterface.HWStatus.RMS_HW_StatusDesc(tc.DeviceName,tc.getHwStaus(), diff);
          else if (this.devType == "WIS")
              desc = new RemoteInterface.HWStatus.WIS_HW_StatusDesc(tc.DeviceName, tc.getHwStaus(), diff);
          else if (this.devType == "LCS")
              desc = new RemoteInterface.HWStatus.LCS_HW_StatusDesc(tc.DeviceName, tc.getHwStaus(), diff);
          else if (this.devType == "CSLS")
              desc = new RemoteInterface.HWStatus.CSLS_HW_StatusDesc(tc.DeviceName, tc.getHwStaus(), diff);
          else if (this.devType == "AVI")
              desc = new RemoteInterface.HWStatus.AVI_HW_StatusDesc(tc.DeviceName, tc.getHwStaus(), diff);
          else if (this.devType == "RD")
              desc = new RemoteInterface.HWStatus.RD_HW_StatusDesc(tc.DeviceName, tc.getHwStaus(), diff);
          else if (this.devType == "VI")
              desc = new RemoteInterface.HWStatus.VI_HW_StatusDesc(tc.DeviceName, tc.getHwStaus(), diff);
          else if (this.devType == "WD")
              desc = new RemoteInterface.HWStatus.WD_HW_StatusDesc(tc.DeviceName, tc.getHwStaus(), diff);
          else if (this.devType == "TTS")
              desc = new RemoteInterface.HWStatus.TTS_HW_StatusDesc(tc.DeviceName, tc.getHwStaus(), diff);
          else if (this.devType == "FS")
              desc = new RemoteInterface.HWStatus.FS_HW_StatusDesc(tc.DeviceName, tc.getHwStaus(), diff);
          else if (this.devType == "MAS")
              desc = new RemoteInterface.HWStatus.MAS_HW_StatusDesc(tc.DeviceName, tc.getHwStaus(), diff);
          else if (this.devType == "LS")
              desc = new RemoteInterface.HWStatus.LS_HW_StatusDesc(tc.DeviceName, tc.getHwStaus(), diff);
          else if (this.devType == "SCM")
              desc = new RemoteInterface.HWStatus.SCM_HW_StatusDesc(tc.DeviceName, tc.getHwStaus(), diff);




         if(notifier!=null)
            notifier.NotifyAll(new NotifyEventObject(EventEnumType.HW_Status_Event, tc.DeviceName, desc));

            this.hw_status_db_queue.Enqueue(new Comm.MFCC.TC_Status(tc.getHwStaus(), desc,tc.m_opmode,tc.m_opstatus,(tc is OutputTCBase)? ((OutputTCBase)tc).GetCurrentDisplayDecs():""));
            lock (hw_status_db_queue)
            {
                System.Threading.Monitor.Pulse(hw_status_db_queue);
            }

           
           
          // throw new Exception("The method or operation is not implemented.");
      }
      void tc_OnConnectStatusChanged(object tcc)
      {
          if (!((TCBase)tcc).IsEnable)
              return;
          Comm.TCBase tc = (Comm.TCBase)tcc;
          try
          {
              notifier.NotifyAll(new NotifyEventObject(EventEnumType.Connection_Event, tc.DeviceName, tc.IsConnected));
          }
          catch { ;}
          try
          {
              if (r_host_comm != null)
              {
                //  lock(r_host_comm)
                  r_host_comm.setDeviceStatus(tc.DeviceName, tc.getHwStaus(), tc.m_opstatus, tc.m_opmode, tc.IsConnected);
              }
          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine("host comm setDeviceStatus:" + ex.Message);
          }

          //if (!tc.IsAllowHWCheck)
          //    return;

          //lock(this)
          //{
         // System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(DB2.Db2.db2ConnectionStr);
        //  System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand();
        //  cmd.Connection = cn;
          try
          {
              
            //  cn.Open();
            dbServer.SendSqlCmd( string.Format("insert into tblDeviceStateLog (devicename,timestamp,type,result) values('{0}','{1}','C',{2})", tc.DeviceName, DB2.Db2.getTimeStampString(System.DateTime.Now), tc.IsConnected ? 1 : 3));
            //  cmd.ExecuteNonQuery();

          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
          }

          try
          {

              //  cn.Open();

             // updateStstask();
              if (tc.IsConnected)
                  dbServer.SendSqlCmd("update tblDeviceConfig set comm_state=1,update_time='"+DB2.Db2.getTimeStampString(System.DateTime.Now)+"' where devicename='" + tc.DeviceName + "'");
              else
                  dbServer.SendSqlCmd("update tblDeviceConfig set comm_state=3,update_time='" + DB2.Db2.getTimeStampString(System.DateTime.Now) + "'   where devicename='" + tc.DeviceName + "'");
           //   cmd.ExecuteNonQuery();

          }
          catch (Exception ex)
          {
              ConsoleServer.WriteLine(ex.Message + ex.StackTrace);
          }

          //try
          //{
          //    if (r_host_comm != null)
          //    {
          //        lock(r_host_comm)
          //           r_host_comm.setDeviceStatus(tc.DeviceName, tc.getHwStaus(), tc.m_opstatus, tc.m_opmode, tc.IsConnected);
          //    }
          //}
          //catch (Exception ex)
          //{
          //    ConsoleServer.WriteLine("host comm setDeviceStatus:" + ex.Message);
          //}
          //finally
          //{
          //    try
          //    {
          //  //      cn.Close();
          //    }
          //    catch { ;}
          //}
          //}
      }

      public abstract void BindEvent(object tc);
      public MFCC.TC_Manager getTcManager()
      {

          return this.manager;
      }
     

      public DataSet getSendDsByFuncName(string funcname)
      {
          return protocol.GetSendDataSet(funcname);
      }


      public virtual DataSet SendTC(string tcname, DataSet ds)
      {

         TCBase tc=  getTcManager()[tcname];

         if (!tc.IsConnected) throw new Exception("tc 未連線!");
         SendPackage pkg= protocol.GetSendPackage(ds, tc.DeviceID);
        // ConsoleServer.WriteLine("Sending Pkg "+ds.Tables[0].Rows[0][0]+":" +pkg.ToString());
         tc.Send(pkg);

         if (pkg.result == CmdResult.ACK)
         {
             if (pkg.type == CmdType.CmdSet)
                 return null;
             else
             {
                 DataSet retds=protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
                 retds.AcceptChanges();
               //  ConsoleServer.WriteLine("Pkg Receive :"+pkg.ReturnTextPackage.ToString());
                 return retds;
             }
         }
         else
         {
             throw new Exception(tcname+pkg.result.ToString());
         }
      }
      public DataSet SendTC(string ip,int port, DataSet ds)
      {

          TCBase tc = getTcManager()[ip,port];
          if (!tc.IsConnected) throw new Exception("tc 未連線!");
          SendPackage pkg = protocol.GetSendPackage(ds, tc.DeviceID);
          tc.Send(pkg);

          if (pkg.result == CmdResult.ACK)
          {
              if (pkg.type == CmdType.CmdSet)
                  return null;
              else
                  return protocol.GetReturnDsByTextPackage(pkg.ReturnTextPackage);
          }
          else
          {
              throw new Exception(pkg.result.ToString());
          }
      }

      public virtual void AfterDeviceAllStart()
      {
      }

    
    }
}
