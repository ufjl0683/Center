using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using Host.TC;
using RemoteInterface;

namespace Host
{
   public class FiveMinVDAVGDataManager
    {
       System.Collections.Generic.Queue<VD1MinCycleEventData> queue = new Queue<VD1MinCycleEventData>();
       object lockobj = new object();
       System.Threading.Thread th;
       public int state = -1;
       public string LastDeviceName = "";
      public  string LastErrorMsg = "";
       public DateTime LastVDDateTime;
       bool IsClose = false;
       public FiveMinVDAVGDataManager()
       {
           ConsoleServer.WriteLine("VD 5min Data Manager init!");
           th = new System.Threading.Thread(ComsumeTask);
           th.Start();

           new System.Threading.Thread(getinitVd5minData).Start();
           ConsoleServer.WriteLine("VD 5min Data Manager Start finished!");
       }

       public void Set5MinAvgData(VD1MinCycleEventData data)
       {


           lock (lockobj)
           {
               LastVDDateTime = data.datatime;
               queue.Enqueue(data);
               System.Threading.Monitor.PulseAll(lockobj);
           }
       }

        void getinitVd5minData()
       {
           System.Collections.IEnumerable ie = Program.matrix.device_mgr.getDataDeviceEnum();
           foreach (Host.TC.DeviceBaseWrapper device in ie)
           {
               try
               {
                   if (device is Host.TC.VDDeviceWrapper)
                   {

                       I_MFCC_VD robj = (I_MFCC_VD)((Host.TC.VDDeviceWrapper)device).getRemoteObj();//.getVDLatest5MinAvgData(device.deviceName);
                       if (robj.getConnectionStatus(device.deviceName))
                       {
                           //  ((Host.TC.VDDeviceWrapper)device).Set5MinAvgData(robj.getVDLatest5MinAvgData(device.deviceName));
                         Set5MinAvgData(robj.getVDLatest5MinAvgData(device.deviceName));
                       }
                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine(device.deviceName + "," + ex.Message + ex.StackTrace);
               }
           }
       }

       public void Close()
       {
           IsClose = true;
       }

       public int  VDFiveMinQueueCnt
       {
          get
          {
              return queue.Count;
            }

       }
       void ComsumeTask()
       {
           VDDeviceWrapper dev;
           VD1MinCycleEventData data;
           while (!IsClose)
           {
               try
               {

                   state = 0;
                       lock (lockobj)
                       {
                           if (queue.Count == 0)
                           {
                               state = 1;
                               System.Threading.Monitor.Wait(lockobj);
                           }
                           state = 2;
                           data = queue.Dequeue();
                       }

                     
                  // lock(lockobj)
                
                   LastDeviceName = data.devName;
                  
                   if (!Program.matrix.device_mgr.IsContainDevice(data.devName))
                       continue;
                   state = 4;
                   dev= Program.matrix.device_mgr[data.devName] as VDDeviceWrapper;
                   

                   //if (dev == null)
                   //    continue;
                   state = 5;
                   dev.Set5MinAvgData(data);
                   state = 6;

               }
               catch(Exception ex)
               {
                   LastErrorMsg = (ex.Message);
                   ConsoleServer.WriteLine(ex.Message);
                   }
           }

       }

       
    }
}
