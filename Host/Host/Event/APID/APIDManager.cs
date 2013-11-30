using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.APID
{
    public class APIDManager
    {

        System.Collections.Hashtable lines;
        System.Collections.Hashtable hsAIDEvents = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
        RemoteInterface.ExactIntervalTimer tmr1min;
        Neural neural;
        public APIDManager()
        {

            neural = new Neural();
            lines = Program.matrix.jammgr.getLines();
            tmr1min = new RemoteInterface.ExactIntervalTimer(40);
            tmr1min.OnElapsed += new RemoteInterface.OnConnectEventHandler(tmr1min_OnElapsed);

            foreach (System.Collections.ArrayList ary in lines.Values)
            {
                for (int i = 0; i < ary.Count - 1; i++)
                {
                    try
                    {
                        Host.TC.VDDeviceWrapper vd = ary[i] as Host.TC.VDDeviceWrapper;
                        //if (vd.aidobject != null)
                        //    vd.aidobject.LoadParameters();
                    }
                    catch (Exception ex)
                    {
                        RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                    }
                }
            }
        }

        public int GetEventCnt()
        {
            return this.hsAIDEvents.Count;
        }
        void tmr1min_OnElapsed(object sender)
        {
            //throw new NotImplementedException();

            foreach (System.Collections.ArrayList ary in lines.Values)
            {
                for (int i = 0; i < ary.Count-1; i++)
                {
                    try
                    {
                        Host.TC.VDDeviceWrapper vd = ary[i] as Host.TC.VDDeviceWrapper;
                        RemoteInterface.MFCC.VD1MinCycleEventData data=null;
                        bool isValid = true;
                        try
                        {
                            if (vd.IsConnected)
                                data = vd.latest5minAvgVdData.lanedata[vd.latest5minAvgVdData.lanedata.Length - 1];
                            else
                                isValid = false;
                        }
                        catch
                        {
                            isValid = false;
                        }

                        int spd =(isValid)? data.speed:-1;
                        int occ =(isValid)? data.occupancy:-1;
                        int vol = (isValid) ? data.vol : -1;
                        //if (vd.NextDevice == null)
                        //    continue;
                        Host.TC.VDDeviceWrapper nextvd = vd.NextDevice as Host.TC.VDDeviceWrapper;
                        RemoteInterface.MFCC.VD1MinCycleEventData nextdata =null;

                        isValid = true;
                        try
                        {
                            if (nextvd.IsConnected)
                                nextdata = nextvd.latest5minAvgVdData.lanedata[nextvd.latest5minAvgVdData.lanedata.Length - 1];
                            else
                                isValid = false;
                        }
                        catch
                        {
                            isValid = false;
                        }
                        int nextocc =(isValid)? nextdata.occupancy:-1;
                        int nextspd = (isValid) ? nextdata.speed : -1;
                        int nextvol = (isValid) ? nextdata.vol : -1;
                      
                        int result = vd.setAIDData(occ, nextocc, spd,nextspd,vol,nextvol);

                        //=======================neural=============================

                        if (isValid && (vd.aidobject.TYPE == AID.AIDType.NeuralNetwork || vd.aidobject.TYPE == AID.AIDType.Both))
                        {
                            int neuralState=0;
                            int[] vector = new int[9];
                            double[] dvector = new double[9];
                            vd.aidobject.GetData(ref vector[0], ref vector[1], ref vector[2], ref vector[3], ref vector[4], ref vector[5], ref vector[6], ref vector[7], ref vector[8]);
                            for (int j = 0; j < vector.Length; j++)
                                dvector[j] = vector[j];
                          neuralState=  neural.GetApidEvent(dvector);
                          if (neuralState != vd.Neural_Result)
                          {

                              if (neuralState == 1)
                              {
                                  if (!hsAIDEvents.ContainsKey(vd.deviceName))
                                  {
                                      Host.Event.APID.APIDRangeData evt = new APIDRangeData(vd, AIDTYPE.NEURAL);
                                      evt.OnAbortToManager += new EventHandler(evt_OnAbortToManager);
                                      hsAIDEvents.Add(vd.deviceName, evt);
                                      Program.matrix.event_mgr.AddEvent(evt);
                                      //  vd.aidobject.BeignRecord(evt.EventId, DateTime.Now);

                                  }
                              }
                              else  // =0
                              {
                                  if (hsAIDEvents.ContainsKey(vd.deviceName))
                                  {
                                      Event evt = hsAIDEvents[vd.deviceName] as Host.Event.Event;
                                      evt.invokeStop();
                                      hsAIDEvents.Remove(vd.deviceName);
                                    //  vd.aidobject.EndRecord();

                                  }
                              }

                              vd.Neural_Result = neuralState;
                          }
                         
                        }


                        //=======================neural=============================


                        //狀態 0 = 無事件 1 = 事件臨界  2 = 事件發生 3 = 事件持續


                        if (isValid && (vd.aidobject.TYPE == AID.AIDType.APID|| vd.aidobject.TYPE == AID.AIDType.Both))
                        {
                            if (vd.AID_Result != result)
                            {
                                vd.AID_Result = result;
                                if (vd.AID_Result == 0)
                                {
                                    if (hsAIDEvents.ContainsKey(vd.deviceName))
                                    {
                                        Event evt = hsAIDEvents[vd.deviceName] as Host.Event.Event;
                                        evt.invokeStop();
                                        hsAIDEvents.Remove(vd.deviceName);
                                        vd.aidobject.EndRecord();

                                    }
                                }
                                else if (vd.AID_Result == 2)
                                {
                                    if (!hsAIDEvents.ContainsKey(vd.deviceName))
                                    {
                                        Host.Event.APID.APIDRangeData evt = new APIDRangeData(vd,AIDTYPE.AID);
                                        evt.OnAbortToManager += new EventHandler(evt_OnAbortToManager);
                                        hsAIDEvents.Add(vd.deviceName, evt);
                                        Program.matrix.event_mgr.AddEvent(evt);
                                        vd.aidobject.BeignRecord(evt.EventId, DateTime.Now);

                                    }
                                }
                                //  if (vd.AID_Result != 0)
                                RemoteInterface.Util.SysLog("aid.log", DateTime.Now.ToString() + "," + vd.deviceName + "result:" + vd.AID_Result);
                            }
                        }
                          
                    }
                    catch (Exception ex)
                    {
                        RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                        RemoteInterface.Util.SysLog("aid.log",ex.Message+","+ex.StackTrace);
                    }
                }
            }
            RemoteInterface.Util.SysLog("aid.log", "aid job finish!");
        }

        void evt_OnAbortToManager(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Host.Event.APID.APIDRangeData evt = sender as Host.Event.APID.APIDRangeData;
            this.hsAIDEvents.Remove(evt.Key);
        }

    }
}
