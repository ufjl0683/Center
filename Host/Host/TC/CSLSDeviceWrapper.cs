using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;

namespace Host.TC
{
   public  class CSLSDeviceWrapper : OutPutDeviceBase
    {

       public CSLSDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction)
           : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus, direction)
       {
       }
      

       public override void output()
       {
          // throw new Exception("The method or operation is not implemented.");
            CSLSOutputData data =null;
           OutputQueueData outdata = this.getOutputdata();
           if (outdata != null)
               data = (CSLSOutputData)outdata.data;
           else  // no queue data
           {
               if (this.IsConnected)
               {
#if !DEBUG
                   ((RemoteInterface.MFCC.I_MFCC_CSLS)this.getRemoteObj()).SetDisplayOff(this.deviceName);
#endif
               }

               return;
           }
           if (this.getRemoteObj() != null && this.IsConnected)
           {

               if (outdata == null || data == null)  //null 代表沒有輸出資料
               {
#if !DEBUG

                   ((RemoteInterface.MFCC.I_MFCC_CSLS)this.getRemoteObj()).SetDisplay(this.deviceName, null);
#endif

               }

               else
               {
                   outdata.status = 1;
                 
                   try
                   {
#if !DEBUG

                       ((RemoteInterface.MFCC.I_MFCC_CSLS)this.getRemoteObj()).SetDisplay(this.deviceName, data.dataset);
                       
#endif
                       this.InvokeOutputDataStatusChange(outdata);
                   }
                   catch(Exception ex)
                   {
                       outdata.IsSuccess = false;
                       this.InvokeOutputDataStatusChange(outdata);
                   }
               }

           }
           else  // 不連線
           {
               outdata.IsSuccess = false;
               outdata.status = 2;
           
               this.InvokeOutputDataStatusChange(outdata);
           }

       }


       //public override OutputQueueData getOutputdata()
       //{
       //    //return base.getOutputdata();
       //    System.Collections.ArrayList ary = new System.Collections.ArrayList();
       //    int maxMode = -1000;
       //    if (outputQueue.Count == 0)
       //        return null;

       //    System.Collections.IDictionaryEnumerator ie = outputQueue.GetEnumerator();
       //    while (ie.MoveNext())
       //    {
       //        OutputQueueData quedata = (OutputQueueData)ie.Value;
       //        if ((int)quedata.mode == maxMode)
       //        {

       //            ary.Add(quedata);
       //        }
       //        else if ((int)quedata.mode > maxMode)
       //        {
       //            ary.Clear();
       //            ary.Add(quedata);
       //            maxMode = (int)quedata.mode;
       //        }

       //    }

       //    int maxSpd = -100;
       //    OutputQueueData maxSpdQueData = null;
       //    foreach (OutputQueueData data in ary)
       //    {
       //        if (data.data == null && maxSpdQueData == null)
       //        {
       //            maxSpdQueData = data;
       //            continue;
       //        }
        
       //        System.Data.DataSet ds = ((RemoteInterface.HC.CSLSOutputData)data.data).dataset; 
       //        if (System.Convert.ToInt32(ds.Tables[0].Rows[0]["speed"]) > maxSpd)
       //        {
       //            maxSpdQueData = data;
       //            maxSpd = System.Convert.ToInt32(ds.Tables[0].Rows[0]["speed"]);
       //        }
       //    }

       //    return maxSpdQueData;

       //}
   }
}
