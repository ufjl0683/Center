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
       

        public override void output()
        {
            //throw new Exception("The method or operation is not implemented.");
            try
            {
               

                OutputQueueData qdata = this.getOutputdata();
                if (qdata == null)
                {
                    if (this.IsConnected)
                        ((RemoteInterface.MFCC.I_MFCC_MAS)this.getRemoteObj()).SetDisplayOff(this.deviceName);

                    return;
                }

                RemoteInterface.MFCC.I_MFCC_MAS masobj = (RemoteInterface.MFCC.I_MFCC_MAS)this.getRemoteObj();

                if (!this.IsConnected)
                {
                    if (qdata != null)
                    {
                        qdata.IsSuccess = false;
                        qdata.status = 2;
                        this.InvokeOutputDataStatusChange(qdata);
                    }

                    return;
                }
                if (qdata == null || qdata.data == null)
                {
#if DEBUG
#else
                    masobj.SetDisplayOff(this.deviceName);
#endif
                    return;
                };

                MASOutputData data = (MASOutputData)qdata.data;
#if DEBUG
#else
                for (int i = 0; i < data.laneids.Length; i++)
                {
                    try
                    {
                        qdata.status = 1;
                        if (data.displays[i] is CMSOutputData)
                            masobj.SendDisplay(this.deviceName, data.laneids[i], ((CMSOutputData)data.displays[i]).g_code_id, ((CMSOutputData)data.displays[i]).hor_space, ((CMSOutputData)data.displays[i]).mesg, ((CMSOutputData)data.displays[i]).colors);
                        else
                        {
                            if (System.Convert.ToInt32(data.displays[i]) >= 30)
                                masobj.SendDisplay(this.deviceName, data.laneids[i], System.Convert.ToInt32(data.displays[i]));

                            else
                                masobj.SetDisplayOff(this.deviceName, data.laneids[i]);
                        }
                        this.InvokeOutputDataStatusChange(qdata);
                    }
                    catch (Exception ex)
                    {
                        qdata.IsSuccess = false;
                        this.InvokeOutputDataStatusChange(qdata);
                    }
                        
                        //send  speed limit here
                }
#endif


            }
            catch (Exception ex)
            {
                RemoteInterface.ConsoleServer.WriteLine(ex.StackTrace+","+ex.Message);
            }
        }



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
