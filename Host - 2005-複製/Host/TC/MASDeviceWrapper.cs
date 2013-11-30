using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;

namespace Host.TC
{
   public  class MASDeviceWrapper : OutPutDeviceBase
    {
       public MASDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction)
           : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus, direction)
        {
        }
       
#if DEBUG
      public override void output()
      {
      }
#else
        public override void output()
        {
            //throw new Exception("The method or operation is not implemented.");
            try
            {
                if (!this.IsConnected)
                    return;

                OutputQueueData qdata = this.getOutputdata();
                RemoteInterface.MFCC.I_MFCC_MAS masobj = (RemoteInterface.MFCC.I_MFCC_MAS)this.getRemoteObj();
               

                if (qdata == null || qdata.data == null)
                {
                    masobj.SetDisplayOff(this.deviceName);
                    return;
                };

                MASOutputData data = (MASOutputData)qdata.data;

                for (int i = 0; i < data.laneids.Length; i++)
                {
                    if (data.displays[i] is CMSOutputData)
                        masobj.SendDisplay(this.deviceName, data.laneids[i], ((CMSOutputData)data.displays[i]).g_code_id, ((CMSOutputData)data.displays[i]).hor_space, ((CMSOutputData)data.displays[i]).mesg, ((CMSOutputData)data.displays[i]).colors);
                    else
                    {
                        if (System.Convert.ToInt32(data.displays[i]) >= 30)
                            masobj.SendDisplay(this.deviceName, data.laneids[i], System.Convert.ToInt32(data.displays[i]));

                        else
                            masobj.SetDisplayOff(this.deviceName, data.laneids[i]);
                    }
                        
                        //send  speed limit here
                }


            }
            catch (Exception ex)
            {
                RemoteInterface.ConsoleServer.WriteLine(ex.StackTrace+","+ex.Message);
            }
        }
#endif


       public override OutputQueueData getOutputdata()
       {

           
           System.Collections.ArrayList ary = new System.Collections.ArrayList();
           byte[] laneids = new byte[] { 1, 2, 3 };
           object[] displays = new object[3];
       //    byte  [] speedllimits = new byte[] { -1,-1,-1,-1,-1,-1};
           OutputQueueData ret;

           if (outputQueue.Count == 0)
               return null;
           else
           {

               System.Collections.IEnumerator ie = outputQueue.GetEnumerator();
               while (ie.MoveNext())
               {
                   OutputQueueData quedata = (OutputQueueData)((System.Collections.DictionaryEntry)ie.Current).Value;


                   ary.Add(quedata);

               }
           }

           ary.Sort();
           if (ary.Count == 0)
               return null;
           else if (ary.Count == 1)
           {
               RemoteInterface.HC.MASOutputData data = (MASOutputData)((OutputQueueData)ary[ary.Count - 1]).data;

               for (int i = 0; i < data.laneids.Length; i++)
               {
                   laneids[data.laneids[i] - 1] = data.laneids[i];
                   displays[data.laneids[i] - 1] = data.displays[i];
                 //  colors[data.boardid[i] - 1] = data.color[i];
               }

           }
           else
           {
               RemoteInterface.HC.MASOutputData data = (MASOutputData)((OutputQueueData)ary[ary.Count - 2]).data;

               for (int i = 0; i < data.laneids.Length; i++)
               {
                   laneids[data.laneids[i] - 1] = data.laneids[i];
                   displays[data.laneids[i] - 1] = data.laneids[i];
                   //colors[data.boardid[i] - 1] = data.color[i];
               }
               data = (MASOutputData)((OutputQueueData)ary[ary.Count - 1]).data;
               for (int i = 0; i < data.laneids.Length; i++)
               {
                   laneids[data.laneids[i] - 1] = data.laneids[i];
                   displays[data.laneids[i] - 1] = data.displays[i];
                  // colors[data.boardid[i] - 1] = data.color[i];
               }
           }

           ret = new OutputQueueData(this.deviceName,((OutputQueueData)ary[ary.Count - 1]).mode, ((OutputQueueData)ary[ary.Count - 1]).ruleid, ((OutputQueueData)ary[ary.Count - 1]).priority
               , new MASOutputData(laneids, displays));
           return ret;
       }

     
    }
}
