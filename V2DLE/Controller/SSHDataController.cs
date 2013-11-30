using System;
using System.Collections.Generic;
using System.Text;
using Comm.DataStore;
using Comm.SensorConfig;
using Microsoft.JScript.Vsa;

namespace Comm.Controller
{
   public abstract class SSHDataController:ControllerBase
    {
       VsaEngine JS_eng = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
       public abstract void OnSSHDDataController_ReceiveText(TextPackage text);
       public SSHDataController(ControllerConfigBase config, PropertyBag propertybag)
           : base(config, propertybag)
       {
        
       }


       public sealed override void OnReceiveText(TextPackage text)
       {
           if (text.Cmd == 0x28)
               DoGetPeriodData(text);
           else // pass to children
               OnSSHDDataController_ReceiveText(text);
           
           //throw new NotImplementedException();
       }
       protected override void PeroidDataReportTask(object obj)
       {

           if (PropertyBag == null || PropertyBag.TransmitMode == 0)
               return;
           try
           {
               if (PropertyBag.TransmitCycle == 0)
                   return;
               DateTime dt = DateTime.Now;




               // this.v2dle.Send(new Comm.SendPackage(Comm.CmdType.CmdSet, Comm.CmdClass.A, this.ControllerID,
               //new byte[] { 0x0B,
               //      PropertyBag.HWtatus.hwstatus1,PropertyBag.HWtatus.hwstatus2,PropertyBag.HWtatus.hwstatus3,
               //      PropertyBag.HWtatus.hwstatus4,1/* comstate*/,PropertyBag.OPStatus,PropertyBag.OPMode}));
 
            //   if (v2dle != null)
                   DoPeriodDataReport();
                 
             
                
               SetTransmitCycleTmr();
               //int sec = (int)dt.Subtract(new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0)).TotalSeconds;
               //sec = PropertyBag.TransmitCycle*60 - (sec % (PropertyBag.TransmitCycle*60));
               //int sec = GetNextTransmitSeconds();
               //tmrPeriodDataReport.Change(sec * 1000, (PropertyBag.TransmitCycle*60) * 1000);

           }
           catch (Exception ex)
           {
               Console.WriteLine(ex.Message+","+ex.StackTrace);
           }
       }


       int GetDegree(int sensorid, double[] values10min,double[]values1hr)
       {


           int degree = 0;
           SensorConfigBase snrconfig =  config.sensors[sensorid];
           if (snrconfig.execution_mode == 0)
               return degree;
             
           for (int vinx = 0; vinx < snrconfig.sensor_values.Length; vinx++)
           {
               double sigma, initmean;
               //string formula;
               //double mean_threshold;
               sigma = snrconfig.sensor_values[vinx].SIGMA;
               initmean = snrconfig.sensor_values[vinx].INITMEAN;
             //  formula = snrconfig.sensor_values[vinx].ConvertFormula;
             //  mean_threshold = snrconfig.sensor_values[vinx].MeanThreshold;
               //for (int i = 0; i < snrconfig.sensor_values.Length; i++)
               //{
                   double value10min, value1hr;
                   value10min = values10min[vinx+sensorid * snrconfig.sensor_values.Length  ];
                   value1hr = values1hr[vinx + sensorid * snrconfig.sensor_values.Length  ];
                   if (value1hr > initmean + 2 * sigma || value1hr < initmean - 2 * sigma ||
                       (value10min > initmean + 2 * sigma && value1hr > initmean + 1.5 * sigma) || (value10min < initmean - 2 * sigma && value1hr < initmean - 1.5 * sigma))
                       degree = degree<3?3:degree;
                   else if (value1hr > initmean + 1.5 * sigma || value1hr < initmean - 1.5 * sigma ||
                       (value10min > initmean + 1.5 * sigma && value1hr > initmean + 1 * sigma) || (value10min < initmean - 1.5 * sigma && value1hr < initmean - 1 * sigma))
                       degree = degree<2 ? 2:degree;
                   else if(value1hr > initmean + 1  * sigma || value1hr < initmean - 1  * sigma ||
                       (value10min > initmean + 1  * sigma && value1hr > initmean + 0.5 * sigma) || (value10min < initmean - 1  * sigma && value1hr < initmean - 0.5 * sigma))
                       degree=degree<1?1:degree;



               //}

               //for (int rinx = snrconfig.sensor_values[vinx].rules.Length - 1; rinx >= 0; rinx--)
               //{


               ////    //SensorValueRuleConfigBase rule = snrconfig.sensor_values[vinx].rules[rinx];
               ////    //if (values[vinx] <= rule.lower_limit || values[vinx] >= rule.upper_limit)
               ////    //{
               ////    //    if (rule.level > degree)
               ////    //        degree = rule.level;
               ////    //    break;
               ////    //}

                  
                       

               //}

           }

           return degree;
       }

       void ReportGetPeriodDataEmpty(int day, int h, int m, int seq)
       {
           //DateTime dt = DateTime.Now;
           //dt = GetYMDHM(dt);

           //  StoreData<double>[] datas = this.dataStore.GetStoreData(dt, PropertyBag.TransmitCycle);

           System.Data.DataSet ds = protocol.GetReturnDataSet("get_cycle_data");
           System.Data.DataRow r = ds.Tables[0].Rows[0];
           r["response_type"] = 0;
           r["hw_status_1"] = this.PropertyBag.HWtatus[0]; 
           r["hw_status_2"] = this.PropertyBag.HWtatus[1];
           r["hw_status_3"] = this.PropertyBag.HWtatus[2];
           r["hw_status_4"] = this.PropertyBag.HWtatus[3];
           r["day"] = day;
           r["hour"] = h;
           r["minute"] = m;
           r["sensor_cnt"] = 0;
           r["value_cnt"] = 0;

           ds.AcceptChanges();
           SendPackage pkg = protocol.GetReturnPackage(ds, this.ControllerID);
           pkg.type = CmdType.CmdSet;
           pkg.Seq = seq;
           if (v2dle != null)
               this.SendDirect(pkg);
       }
       void ReportPeriodDataEmpty()
       {
           DateTime dt = DateTime.Now;
           dt = GetYMDHM(dt);

           //  StoreData<double>[] datas = this.dataStore.GetStoreData(dt, PropertyBag.TransmitCycle);

           System.Data.DataSet ds = protocol.GetSendDataSet("report_cycle_data");
           System.Data.DataRow r = ds.Tables[0].Rows[0];
           r["response_type"] = 0;
           r["hw_status_1"] = this.PropertyBag.HWtatus[0];
           r["hw_status_2"] = this.PropertyBag.HWtatus[1];
           r["hw_status_3"] = this.PropertyBag.HWtatus[2];
           r["hw_status_4"] = this.PropertyBag.HWtatus[3];
           r["day"] = dt.Day;
           r["hour"] = dt.Hour;
           r["minute"] = dt.Minute;
           r["sensor_cnt"] = 0;
           r["value_cnt"] = 0;
           ds.AcceptChanges();
           SendPackage pkg = protocol.GetSendPackage(ds, this.ControllerID);
           pkg.type = CmdType.CmdSet;
           if (v2dle != null)
               v2dle.Send(pkg);
       }

       void GetMeanValues(DateTime dt, int min, out double[] retValues,out int[] validcnt )
       {

          // double[ ]retValues;
             StoreData<double>[] datas = this.dataStore.GetStoreData(dt, min);
            if(datas.Length==0)
            {
                retValues = new double[0];
                validcnt = new int[0];
                return;
            }

            int valueLength = datas[0].GetValue().Length;  //total sensor value length
            int sensorValueLength = valueLength / config.sensors.Length;
             retValues=new double[valueLength];
             validcnt = new int[valueLength];
              for (int sensor_id = 0; sensor_id < devices.Length; sensor_id++)
              {
                  

               foreach (StoreData<double> d in datas)
               {
                  // double[] retValues = new double[valueLength];
                   double[] values = d.GetValue();
                   for (int i = 0; i < sensorValueLength; i++)
                   {
                       if (values[sensor_id *sensorValueLength  + i] != double.NegativeInfinity)
                       {
                          
                           retValues[sensor_id * sensorValueLength + i] += (values[sensor_id * sensorValueLength + i] - config.sensors[sensor_id].sensor_values[i].offset) * config.sensors[sensor_id].sensor_values[i].coefficient;
                           validcnt[sensor_id * sensorValueLength + i]++;
                       }
                   }


               }


               for (int i = 0; i < sensorValueLength; i++)
               {
                   if (validcnt[sensor_id * sensorValueLength + i] != 0)
                       retValues[sensor_id * sensorValueLength + i] = retValues[sensor_id * sensorValueLength + i] / validcnt[sensor_id * sensorValueLength + i];
               }
                


              }

       }
       protected void DoPeriodDataReport()
       {
           if (devices.Length == 0)
           {
               // return invalid data here
               ReportPeriodDataEmpty();
               return;
           }
           DateTime dt = DateTime.Now;
           dt = GetYMDHM(dt);

//StoreData<double>[] datas = this.dataStore.GetStoreData(dt, PropertyBag.TransmitCycle); mark 1/2/2013
             double[]ret,ret1hr; int[]valid,valid1hr;
           GetMeanValues(dt,PropertyBag.TransmitCycle,out ret,out valid);
           GetMeanValues(dt,60,out ret1hr,out valid1hr);

           if (ret.Length == 0)  // if (datas.Length == 0)  // mark 1/2/20123
           {
               // return invalid data here
               ReportPeriodDataEmpty();
               return;
           }

           System.Data.DataSet ds = protocol.GetSendDataSet("report_cycle_data");
           System.Data.DataRow r = ds.Tables[0].Rows[0];
           r["response_type"] = 0;
           r["hw_status_1"] = this.PropertyBag.HWtatus[0];
           r["hw_status_2"] = this.PropertyBag.HWtatus[1];
           r["hw_status_3"] = this.PropertyBag.HWtatus[2];
           r["hw_status_4"] = this.PropertyBag.HWtatus[3];
           r["day"] = dt.Day;
           r["hour"] = dt.Hour;
           r["minute"] = dt.Minute;

           r["sensor_cnt"] = config.sensors.Length;
           int valueLength = ret.Length;  // datas[0].GetValue().Length; mark 1/2
         
           int sensorValueLength = valueLength / config.sensors.Length;
           
           r["value_cnt"] = sensorValueLength;

           for (int sensor_id = 0; sensor_id < devices.Length; sensor_id++)
           {
               string str = config.sensors[sensor_id].id + "," + config.sensors[sensor_id].device_name +"  execution_mode:" +config.sensors[sensor_id].execution_mode + "\r\n";
             
               System.Data.DataRow rr = ds.Tables[1].NewRow();
               rr["id"] = sensor_id;

             //  double[] retValues = new double[valueLength]; mark 1/2/2013
              
             //  int[] validcnt = new int[valueLength];

               // 10 min data
               //foreach (StoreData<double> d in datas)  mark 1/2/2013
               //{
                   //double[] values =     //d.GetValue();
                   //for (int i = 0; i < sensorValueLength; i++)
                   //{
                   //    if (values[sensor_id *sensorValueLength  + i] != double.NegativeInfinity)
                   //    {
                   //        retValues[sensor_id * sensorValueLength + i] += (values[sensor_id * sensorValueLength + i] - config.sensors[sensor_id].sensor_values[i].offset) * config.sensors[sensor_id].sensor_values[i].coefficient;
                   //        validcnt[sensor_id * sensorValueLength + i]++;
                   //    }
                   //}


               //}

              


               //for (int i = 0; i < sensorValueLength; i++) mark 1/2/2013
               //{
               //    if (validcnt[sensor_id * sensorValueLength + i] != 0)
               //        retValues[sensor_id * sensorValueLength + i] = retValues[sensor_id * sensorValueLength + i] / validcnt[sensor_id * sensorValueLength + i];
               //}


               int degree = GetDegree(sensor_id, ret,ret1hr);

               for (int i = 0; i < 3; i++)
                   rr["value" + i] = 0;

               bool IsValid = true;
               for (int i = 0; i < sensorValueLength; i++)
               {
                   rr["value" + i] = V2DLE.DoubleTouLong(ret[sensor_id * sensorValueLength + i]);
                   if (valid[sensor_id * sensorValueLength + i] == 0)
                       IsValid = false;
                   str += "\t" + ret [sensor_id * sensorValueLength + i];

               }
               rr["degree"] = degree;
               str += "\t degree:" + degree+"\t valid:"+IsValid;
               Console.WriteLine(str);
               if (IsValid)
                   rr["is_valid"] = 1;
               else
               {
                   rr["is_valid"] = 0;
                  // rr["degree"]=0;
               }

               ds.Tables[1].Rows.Add(rr);
             

           }

         
           Console.WriteLine();

           ds.AcceptChanges();
           SendPackage pkg = protocol.GetSendPackage(ds, this.ControllerID);



           pkg.type = CmdType.CmdSet;
           if (v2dle != null)
               v2dle.Send(pkg);



       }

       void DoGetPeriodData(Comm.TextPackage Text)
       {
           int day, min, hour;

           day = Text.Text[1];
           hour = Text.Text[2];
           min = Text.Text[3];


           DateTime dt;
           if (day <= DateTime.Now.Day)
               dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day, hour, min, 0);
           else
           {
               if(DateTime.Now.Month==1)
                     dt = new DateTime(DateTime.Now.Year, 12, day, hour, min, 0);
               else 
               dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, day, hour, min, 0);
           }

           //if (day > DateTime.Now.Day)
           //    dt = dt.AddMonths(-1);

           dt = GetYMDHM(dt);

         //  StoreData<double>[] datas = this.dataStore.GetStoreData(dt, PropertyBag.TransmitCycle);
           double[] ret, ret1hr; int[] valid, valid1hr;
           GetMeanValues(dt, PropertyBag.TransmitCycle, out ret, out valid);
           GetMeanValues(dt, 60, out ret1hr, out valid1hr);
           
            if(ret.Length==0)// if (datas.Length == 0)
           {
               // return invalid data here
               ReportGetPeriodDataEmpty(day, hour, min, Text.Seq);
               return;
           }

           System.Data.DataSet ds = protocol.GetReturnDataSet("get_cycle_data");
           System.Data.DataRow r = ds.Tables[0].Rows[0];
           r["response_type"] = 0;
           r["hw_status_1"] = this.PropertyBag.HWtatus[0];
           r["hw_status_2"] = this.PropertyBag.HWtatus[1];
           r["hw_status_3"] = this.PropertyBag.HWtatus[2];
           r["hw_status_4"] = this.PropertyBag.HWtatus[3];
           r["day"] = dt.Day;
           r["hour"] = dt.Hour;
           r["minute"] = dt.Minute;
           r["sensor_cnt"] = config.sensors.Length;


           int valueLength = ret.Length; //datas[0].GetValue().Length;
           int sensorValueLength = valueLength / config.sensors.Length;

           r["value_cnt"] = sensorValueLength;

           for (int sensor_id = 0; sensor_id < devices.Length; sensor_id++)
           {
               string str = config.sensors[sensor_id].id + "," + config.sensors[sensor_id].device_name + "  execution_mode:" + config.sensors[sensor_id].execution_mode + "\r\n";

               System.Data.DataRow rr = ds.Tables[1].NewRow();
               rr["id"] = sensor_id;

               //double[] retValues = new double[valueLength];
               //int[] validcnt = new int[valueLength];
               //foreach (StoreData<double> d in datas)
               //{
               //    double[] values = d.GetValue();
               //    for (int i = 0; i < sensorValueLength; i++)
               //    {
               //        if (values[sensor_id * sensorValueLength + i] != double.NegativeInfinity)
               //        {
               //            retValues[sensor_id * sensorValueLength + i] += (values[sensor_id * sensorValueLength + i] - config.sensors[sensor_id].sensor_values[i].offset) * config.sensors[sensor_id].sensor_values[i].coefficient;
               //            validcnt[sensor_id * sensorValueLength + i]++;
               //        }
               //    }


               //}

               //for (int i = 0; i < sensorValueLength; i++)
               //{
               //    if (validcnt[sensor_id * sensorValueLength + i] != 0)
               //        retValues[sensor_id * sensorValueLength + i] = retValues[sensor_id * sensorValueLength + i] / validcnt[sensor_id * sensorValueLength + i];
               //}


               int degree = GetDegree(sensor_id, ret,ret1hr);

               for (int i = 0; i < 3; i++)
                   rr["value" + i] = 0;

               bool IsValid = true;
               for (int i = 0; i < sensorValueLength; i++)
               {
                   rr["value" + i] = V2DLE.DoubleTouLong(ret[sensor_id * sensorValueLength + i]);
                   if (valid[sensor_id * sensorValueLength + i] == 0)
                       IsValid = false;
                   str += "\t" + ret[sensor_id * sensorValueLength + i];

               }
               rr["degree"] = degree;
               str += "\t degree:" + degree + "\t valid:" + IsValid;
               Console.WriteLine(str);
               if (IsValid)
                   rr["is_valid"] = 1;
               else
                   rr["is_valid"] = 0;

               ds.Tables[1].Rows.Add(rr);


           }
           //for (int sensor_id = 0; sensor_id < devices.Length; sensor_id++)
           //{
           //    System.Data.DataRow rr = ds.Tables[1].NewRow();
           //    rr["id"] = sensor_id;

           //    double[] retValues = new double[valueLength];
           //    int[] validcnt = new int[valueLength];
           //    foreach (StoreData<double> d in datas)
           //    {
           //        double[] values = d.GetValue();
           //        for (int i = 0; i < retValues.Length; i++)
           //        {
           //            if (values[sensor_id * 2 + i] != double.NegativeInfinity)
           //            {
           //                retValues[i] += values[sensor_id * 2 + i];
           //                validcnt[i]++;
           //            }
           //        }


           //    }

           //    for (int i = 0; i < valueLength; i++)
           //    {
           //        if (validcnt[i] != 0)
           //            retValues[i] = retValues[i] / validcnt[i];
           //    }


           //    int degree = GetDegree(sensor_id, retValues);

           //    for (int i = 0; i < 3; i++)
           //        rr["value" + i] = 0;

           //    bool IsValid = true;
           //    for (int i = 0; i < valueLength; i++)
           //    {
           //        rr["value" + i] = V2DLE.DoubleTouLong(retValues[i]);
           //        if (validcnt[i] == 0)
           //            IsValid = false;
           //    }
           //    rr["degree"] = degree;


           //    if (IsValid)
           //        rr["is_valid"] = 1;
           //    else
           //        rr["is_valid"] = 0;

           //    ds.Tables[1].Rows.Add(rr);

           //}
           ds.AcceptChanges();
           SendPackage pkg = protocol.GetReturnPackage(ds, this.ControllerID);
           pkg.type = CmdType.CmdSet;
           pkg.Seq = Text.Seq;
           if (v2dle != null)
               this.SendDirect(pkg);

       }
    }
}
