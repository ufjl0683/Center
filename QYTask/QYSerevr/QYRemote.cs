using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using System.Data;


namespace QYTask
{

    
    class QYRemote:RemoteInterface.RemoteClassBase,RemoteInterface.IQYCommands
    {

        public QYRemote()
        {
           // QYTask.Program.OnNewSecData += new new_5min_sec_data_handler(OnNewSecData);
        }

       

        public override object InitializeLifetimeService()
        {
            return null;
        }
        
        public    DataSet get_current_5min_all_section_data()
        {
           // DataSet ds = new DataSet();
           //  ds.Merge( QYTask.Program.Curr5minSecDs.Clone());
            try
            {
                using (DataSet ds = Util.getPureDataSet(QYTask.Program.Curr5minSecDs))
                {
                    return ds;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Remote Method :get_current_5min_all_section_data,", ex.Message);
                throw new RemoteException(ex.Message);
            }

            
        }


        public DataSet get_current_travel_time()
        {
            try
            {
                using (DataSet ds = new DataSet())
                {

                    ds.Tables.Add(Util.getPureDataTable(Program.RGSConfDs.tblRGS_Config));
                    return ds;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Remote Method :get_current_travel_time,", ex.Message+","+ex.StackTrace);
                throw new RemoteException(ex.Message);
            }
        }


          public    DataSet get_all_rgs_display_status()
          {
              try
              {
                  using (DataSet ds = new DataSet())
                  {
                      ds.Tables.Add(Util.getPureDataTable(Program.RGSConfDs.tblRGSMain));
                      return ds;
                  }
              }
              catch (Exception ex)
              {
                  throw new RemoteException(ex.Message);
              }
           }
         public   DataSet get_rgs_conf_table()
        {
            try
            {
                using (DataSet ds = new DataSet())
                {
                    ds.Tables.Add(Util.getPureDataTable(Program.RGSConfDs.tblRGS_Config));

                    return ds;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Remote Method :get_rgs_conf_table,", ex.Message);
                throw new RemoteException(ex.Message);
            }
        }


        public DateTime get_current_travel_data_time_stamp()
        {
            try
            {
               
                return QYTask.Program.Curr5minSecDs.tblFileAttr[0].time;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new RemoteException(ex.Message);
            }
        }

        public DataSet get_travel_sections_detail_data( string ip,byte dispaly_part)
        {
            try
            {
                Ds.tblRGS_ConfigRow r = QYTask.Program.RGSConfDs.tblRGS_Config.FindByipdisplay_part(ip, dispaly_part);
                if (r == null)
                    throw new RemoteException("data not found!");


                System.Data.DataView vw = null;

                using (vw = Util.get_travel_sections_detail_data(r.from_milepost, r.end_milepost, r.freewayId, r.direction))
                {
                    //if (r.direction == "S")
                    //    vw = new DataView(QYTask.Program.Curr5minSecDs.tblSecTrafficData,
                    //   string.Format("from_milepost>={0} and  from_milepost < {1} and freewayId={2} and directionId='S'", r.from_milepost, r.end_milepost, r.freewayId)
                    //   , "", DataViewRowState.CurrentRows);
                    //else if (r.direction == "N")
                    //    vw = new DataView(QYTask.Program.Curr5minSecDs.tblSecTrafficData,
                    //    string.Format("from_milepost<={0} and  from_milepost > {1} and freewayId={2} and directionId='N'", r.from_milepost, r.end_milepost, r.freewayId)
                    //, "", DataViewRowState.CurrentRows);

                    using (DataSet ds = new DataSet())
                    {
                        ds.Merge(vw.ToTable());
                        return ds;
                    }
                }

               
            }
            catch (Exception ex)
            {
                Console.WriteLine("Remote Method :get_travel_sections_detail_data,", ex.Message);
                throw new RemoteException(ex.Message);
            }
        }

        #region IQYCommands 成員


        public DataSet get_tblIcons()
        {
          //  throw new Exception("The method or operation is not implemented.");
          
            try
            {
                using (DataSet ds = new DataSet())
                {
                    ds.Merge(Util.getPureDataTable(Program.RGSConfDs.tblIcons));
                    return ds;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Remote Method :get_tblIcons,", ex.Message);
                throw new RemoteException(ex.Message);
            }
        }


        public DataSet get_RGS_all_tables()
        {
            // DataSet ds = new DataSet();
            //ds.Merge(Program.RGSConfDs);
            try
            {
                using(DataSet ds=Util.getPureDataSet(Program.RGSConfDs))
                {
                    return ds;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Remote Method :getRGSAllTables,", ex.Message);
                throw new RemoteException(ex.Message);
            }
        }
      public   void setFreeInputModeAndMessage(string ip, byte display_part,byte iconid, string finput1, string finputcolor1, string finput2, string finputcolor2)
        {

            if (!QYTask.Program.rgs_manager[ip].IsConnected)
                throw new RemoteException("TC 未連線!");
            try
            {
                QYTask.Ds.tblRGS_ConfigRow r = Program.RGSConfDs.tblRGS_Config.FindByipdisplay_part(ip, display_part);
                if (r == null)
                    throw new RemoteException("找不到資料:ip=" + ip + " dispaly_part=" + display_part);

                r["mode"] = 1;
                r["ficon"] = iconid;
                r["finput1"] = finput1;
                r["finput2"] = finput2;
                r["finputcolor1"] = finputcolor1;
                r["finputcolor2"] = finputcolor2;
                Program.NotifyDisplayTask();
             //   System.IO.File.AppendAllText("op.log",System.DateTime.Now+string.Format(" ip:{0} ,display_part:{1} ,iconid:{2},finput1:{3},finput2:{4} \r\n",ip,display_part,iconid,finput1,finput2));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Remote Method :setFreeInputModeAndMessage,", ex.Message);
                throw new RemoteException(ex.Message);
            }

        }

        public  void setTravelMode(string ip, byte display_part)
        {

            if (!QYTask.Program.rgs_manager[ip].IsConnected)
                throw new RemoteException("TC 未連線!");
            try
            {
                QYTask.Ds.tblRGS_ConfigRow r = Program.RGSConfDs.tblRGS_Config.FindByipdisplay_part(ip, display_part);
                if (r == null)
                    throw new RemoteException("找不到資料:ip=" + ip + " dispaly_part=" + display_part);
                r["mode"] = 0;
                Program.NotifyDisplayTask();
              //  System.IO.File.AppendAllText("op.log", System.DateTime.Now + string.Format(" ip:{0} ,display_part:{1}  travel mode \r\n", ip, display_part));

            }
            catch (Exception ex)
            {
                Console.WriteLine("Remote Method :setTravelMode,", ex.Message);
                throw new RemoteException(ex.Message);
            }

        }

        public DataSet get_rms_config()
        {
            using (DataSet ds = Util.getPureDataSet(QYTask.Program.RMSConfDs))
            {
                return ds;
            }

        }

        public void set_rms_mode_planno(string ip, byte mode, byte planno)
        {

            if (!QYTask.Program.rms_manager[ip].IsConnected)
                throw new RemoteException("TC 未連線!");
            
            lock (QYTask.Program.RMSConfDs.tblRmsConfig)
            {


                try
                {
                   

                    QYTask.Ds.tblRmsConfigRow r = QYTask.Program.RMSConfDs.tblRmsConfig.FindByip(ip);
                    r.ctl_mode_setting = mode;
                    r.planno_setting = planno;

                    QYTask.Program.NotifyRmsModeTask();
                 //   System.IO.File.AppendAllText("op.log", System.DateTime.Now + string.Format("rms ip:{0} ,mode{1},planno {2} \r\n", ip, mode,planno));
                   
                }
                catch (Exception ex)
                {
                    throw new RemoteException(ex.Message);
                }
            }
           

        }

        public bool IsRGSConnected(string ip)
        {
            return Program.rgs_manager[ip].IsConnected;
        }
        

        #endregion



       
    }
}
