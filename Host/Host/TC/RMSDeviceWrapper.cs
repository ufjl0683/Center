using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using RemoteInterface.HC;

namespace Host.TC
{
  public   class RMSDeviceWrapper:OutPutDeviceBase
    {
      public RMSDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction)
          : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus, direction)
      {
          // 2013/3/5 remove
          //try
          //{
          //    if(this.getRemoteObj()!=null && this.getRemoteObj().getConnectionStatus(this.deviceName))
          //           this.getRemoteObj().SetModeAndPlanno(this.deviceName, 0, 30);
          //}
          //catch
          //{ ;}
      }


      
#if DEBUG
      public override void output()
      {
         
      }
#else
       public override void output()
       {
           //throw new Exception("The method or operation is not implemented.");

           OutputQueueData outdata=this.getOutputdata();
           if (outdata == null)
           {
               if (this.IsConnected)
                   ((RemoteInterface.MFCC.I_MFCC_RMS)this.getRemoteObj()).SetDisplayOff(this.deviceName);

               return;
           }
           if (outdata == null || outdata.data == null)
           {
#if DEBUG
#else
               this.getRemoteObj().SetDisplayOff(this.deviceName);
#endif
               return;
           }
           RMSOutputData data=(RMSOutputData)outdata.data;
           if (this.getRemoteObj() != null && this.getRemoteObj().getConnectionStatus(deviceName))
           {
               try
               {
                   outdata.status = 1;
                     this.InvokeOutputDataStatusChange(outdata);
                   this.getRemoteObj().SetModeAndPlanno(this.deviceName, (byte)data.mode, (byte)data.planno);
               }
               catch (Exception ex)
               {
                   outdata.IsSuccess = false;
                    this.InvokeOutputDataStatusChange(outdata);
               }
           }
           else
           {
               if (outdata != null)
               {
                   outdata.IsSuccess = false;
                   outdata.status = 2;
                
                    this.InvokeOutputDataStatusChange(outdata);
               }
               return;
           }

           if ( ((RMSOutputData)outdata.data).IsChangeDownStreamCapacity)
           {
               try
               {
                   RMSOutputData rmdata = outdata.data as RMSOutputData;
                   System.Data.DataSet ds = this.getRemoteObj().getSendDSByFuncName("set_tra_resp_ctl_param");
                   System.Data.DataRow r = ds.Tables[0].Rows[0];
                   r["main_occupy_threshold"] = rmdata.main_occupy_threshold;
                   r["max_rms_rate"] = rmdata.max_rms_rate;
                   r["min_rms_rate"] = rmdata.min_rms_rate;
                   r["ramp_threshold"] = rmdata.ramp_threshold;
                   r["ramp_termination_count_threshold"] = rmdata.ramp_termination_count_threshold;
                   r["cap_down"] = rmdata.newCapDown;
                   ds.AcceptChanges();

                   this.getRemoteObj().sendTC(this.deviceName, ds);
               }
               catch
               {
                   ;
               }
               

           }


       }
#endif
       public override OutputQueueData getOutputdata()
       {

           object[] data = GetPriorityQueueData(new RMS_Comparer());
           if (data.Length == 0)
               return null;

           return data[data.Length - 1] as OutputQueueData;
           
           //int[] priorityTable = new int[] { 2, 3, 1, 4, 0 };
           //int currPriority=-1;
           //OutputQueueData ret = null;
           //object[] data = GetPriorityQueueData();
           //if (data.Length == 0)
           //    return null;
           //if ((data[data.Length - 1] as OutputQueueData).mode != OutputModeEnum.ResponsePlanMode)
           //    return data[data.Length - 1] as OutputQueueData;
           //else
           //{
           //    for (int i = 0; i < data.Length; i++)
           //    {
           //        OutputQueueData qdata = data[i] as OutputQueueData;
           //        if (qdata.data == null || qdata.mode!= OutputModeEnum.ResponsePlanMode)
           //            continue;
           //        RMSOutputData rmsout = qdata.data as RMSOutputData;
           //        if (priorityTable[rmsout.mode] > currPriority)
           //        {
           //            currPriority = priorityTable[rmsout.mode];
           //            ret = qdata;
           //        }
                       
           //    }
           //}
           //return ret;

       }

      public override void removeOutput(int ruleId)
       {


           OutputQueueData data = this.GetQueueData(ruleId);
           if (data == null)
               return;
           RMSOutputData rmdata = data.data as RMSOutputData;
           base.removeOutput(ruleId);

           if(rmdata.IsChangeDownStreamCapacity)
           {
               try
               {
                   System.Data.DataSet ds = this.getRemoteObj().getSendDSByFuncName("set_tra_resp_ctl_param");
                   System.Data.DataRow r = ds.Tables[0].Rows[0];
                   r["main_occupy_threshold"] = rmdata.main_occupy_threshold;
                   r["max_rms_rate"] = rmdata.max_rms_rate;
                   r["min_rms_rate"] = rmdata.min_rms_rate;
                   r["ramp_threshold"] = rmdata.ramp_threshold;
                   r["ramp_termination_count_threshold"] = rmdata.ramp_termination_count_threshold;
                   r["cap_down"] = rmdata.oldCapDown;
                   ds.AcceptChanges();
                   this.getRemoteObj().sendTC(this.deviceName, ds);
               }
               catch
               {
                   ;
               }

           }
       }
       public new   I_MFCC_RMS getRemoteObj()
       {
             return (I_MFCC_RMS)base.getRemoteObj();

       }

      public void SetModeAndPlanno(string devname,OutputModeEnum mode,int ruleid,int priority, byte rmsmode, byte planno)
      {
           this.SetOutput(new OutputQueueData(this.deviceName,mode,ruleid,priority,new RMSOutputData(rmsmode,planno)));
          // output();
      }

   }


   
}
