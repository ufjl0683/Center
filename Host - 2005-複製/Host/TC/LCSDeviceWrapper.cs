using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
using RemoteInterface.MFCC;
using RemoteInterface.HC;

namespace Host.TC
{
    class LCSDeviceWrapper:OutPutDeviceBase
    {

        byte[] signalPriority = new byte[] { 0,5,10,7,7,6,9,8,8};
        byte[] currSignal = new byte[32];
        /*
           index    signal      priority 
            0       off         0
            1       green       5
            2       red         10
            3       right-top   7
            4       left-top    7
            5       green flash 6
            6       red flash   9
            7       r-t-flash   8
         *  8       l-t-flash   8
           
         */
        public LCSDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction)
            : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus, direction)
       {
       }

        public void SetDisplay(OutputModeEnum mode, int ruleid, int priority,LCSOutputData lcstata)
       {

           OutputQueueData data = new OutputQueueData(this.deviceName,mode, ruleid, priority, lcstata);
           this.SetOutput(data);
          // output();
            
       }



        public void SetDisplayOff(OutputModeEnum mode, int ruleid, int priority)
        {
            //OutputQueueData data = new OutputQueueData(mode,ruleid, priority, null);
            //this.EnOutputQueue(data);
            this.SetDisplay(mode, ruleid, priority, null);
            output();
        }

        public new I_MFCC_LCS getRemoteObj()
        {
            return (I_MFCC_LCS)base.getRemoteObj();

        }

        public int getTEMStatus()
        {
            for (int i = 0; i < 32; i++)
                if (currSignal[i] == 2)
                    return 1;


            return 0;
        }

        public override OutputQueueData getOutputdata()
        {
            System.Collections.ArrayList ary=new System.Collections.ArrayList();
            int maxMode = -1000;
            if (outputQueue.Count == 0)
                return null;
           
                System.Collections.IEnumerator ie = outputQueue.GetEnumerator();
                while (ie.MoveNext())
                {
                    OutputQueueData quedata = (OutputQueueData)((System.Collections.DictionaryEntry)ie.Current).Value;
                    if ((int)quedata.mode == maxMode)
                    {
                       
                        ary.Add(quedata);
                    }
                    else if ((int)quedata.mode > maxMode)
                    {
                        ary.Clear();
                        ary.Add(quedata);
                        maxMode = (int)quedata.mode;
                    }
                       
                }



                byte[] signals = new byte[32];

                for (int i = 0; i < 32; i++)
                    signals[i] = 255;

                foreach (OutputQueueData qdata in ary)
                {
                    LCSOutputData lcsdata=(LCSOutputData)qdata.data;
                     
                    foreach (System.Data.DataRow r in lcsdata.dataset.Tables[1].Rows) 
                    {
                        int signo = System.Convert.ToInt32(r["sign_no"]);
                        byte sigstatus= System.Convert.ToByte(r["sign_status"]);

                        if(signals[signo] == 255)
                            signals[signo] = sigstatus;
                        else  if (this.signalPriority[signals[signo]] < signalPriority[sigstatus])
                            signals[signo] = sigstatus;

                        
                    }


                }
                
              // 檢查 箭頭是否由其他狀態轉換成 X 並通知  TEM ，或 x 轉換成其他訊號

                try
                {

                    if (this.location.Trim() == "T"  && this.lineid=="N6" )
                    {
                        bool isAnySignalToX = false, isAnyXtoOther = false; ;
                        for (int i = 0; i < 32; i++)
                        {
                            if (currSignal[i] != signals[i] && signals[i] == 2)
                            {

                                //  通知 mfcctem here
                                isAnySignalToX = true;
                                //(Program.matrix.getRemoteObject("MFCC_TEM") as I_MFCC_TEM).setLCSStatus(this.deviceName, 1);
                                //;


                            }
                            else if (currSignal[i] != signals[i] && currSignal[i] == 2)
                                isAnyXtoOther = true;
                            
                            currSignal[i] = signals[i];
                        }

                        if(isAnySignalToX)
                            (Program.matrix.getRemoteObject("MFCC_TEM") as I_MFCC_TEM).setLCSStatus(this.deviceName, 1);

                        if (isAnyXtoOther)
                        {
                            int xcnt=0;
                            for (int i = 0; i < 32; i++)
                                if (currSignal[i] == 2)
                                    xcnt++;

                            if(xcnt==0)
                                (Program.matrix.getRemoteObject("MFCC_TEM") as I_MFCC_TEM).setLCSStatus(this.deviceName, 0);
                        }

                    }
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                }
               





                System.Data.DataSet ds = getRemoteObj().getSendDSByFuncName("set_ctl_sign");

                for (int i = 0; i < 32; i++)
                {
                    if(signals[i]!=255)
                        ds.Tables[1].Rows.Add(i, signals[i]);
                }


                ds.Tables[0].Rows[0]["sign_cnt"] = ds.Tables[1].Rows.Count;

                ds.AcceptChanges();
             OutputQueueData topqdata=(OutputQueueData)ary.ToArray()[0];
             return new OutputQueueData(this.deviceName,topqdata.mode, topqdata.ruleid, topqdata.priority, new LCSOutputData(ds));
        }


#if DEBUG
      public override void output()
      {
      }
#else

        public override void output()
        {
               LCSOutputData data=null;
            //throw new Exception("The method or operation is not implemented.");
            OutputQueueData outdata = this.getOutputdata();
            if(outdata!=null)
                data = (LCSOutputData)outdata.data;

            if (this.getRemoteObj() != null && this.getRemoteObj().getConnectionStatus(this.deviceName))
                if (outdata==null || data == null)  //null 代表沒有輸出資料
                
                    this.getRemoteObj().SetDisplay(this.deviceName, null);

                else 
                                 
                      this.getRemoteObj().SetDisplay(this.deviceName, data.dataset);
                

          
                
        }
#endif

    }


    
}
