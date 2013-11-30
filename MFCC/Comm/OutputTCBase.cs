using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace Comm
{
    public delegate void OnOutputDataCompareWrongEventHandler(OutputTCBase tc, string hostoutStr,string tcOutStr);
    public delegate void OnOutputChangedHandler(OutputTCBase tc, string newOutputStr);
    abstract  public  class OutputTCBase:TCBase
   {
        public event OnOutputDataCompareWrongEventHandler OnOutputDataCompareWrongEvent;
        public event OnOutputChangedHandler OnOutputChanged;
        protected System.Timers.Timer tmrCheckOutput;
        protected bool isCompareError = false;
        
       
        public OutputTCBase(Protocol protocol, string devicename, string ip, int port, int deviceid, byte[] hw_status, byte opmode, byte opstatus,byte comm_state)
            :base(protocol, devicename, ip, port, deviceid, hw_status,opmode,opstatus, comm_state)
        {
            tmrCheckOutput = new System.Timers.Timer(outputCompareMin*1000 * 60);
            tmrCheckOutput.Elapsed += new System.Timers.ElapsedEventHandler(tmrCheckOutput_Elapsed);
            tmrCheckOutput.Start();
        }



        public void setSunRiseSunSet(int RiseHour, int RiseMin,int SetHour ,int SetMin)
        {
            byte[] data = new byte[] { 0x09, (byte)RiseHour, (byte)RiseMin, (byte)SetHour, (byte)SetMin };
            SendPackage pkg = new SendPackage(CmdType.CmdSet, CmdClass.B, 0xffff, data);

            this.Send(pkg);
        }


        public void setDisplayCompareCycle( int min)
        {
            tmrCheckOutput.Interval = min * 60 * 1000;

        }

        void tmrCheckOutput_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
            try
            {
              //  if(this.IsTcpConnected)
                             CheckDisplayTask();
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine("In check display task:" + ex.Message+ex.StackTrace);
            }
        }
        public abstract void TC_SetDisplayOff();

        public abstract string GetCurrentDisplayDecs();

        protected bool InvokeOutPutWrongEvent(string  hostOutStr,string tcOutStr)
        {
            if ( (hostOutStr != tcOutStr) != isCompareError)  // 有狀態改變 才寫入資料
            {

                if (this.OnOutputDataCompareWrongEvent != null)
                    this.OnOutputDataCompareWrongEvent(this, hostOutStr, tcOutStr);

              return  isCompareError = (hostOutStr != tcOutStr);

            }
            return false;
           
        }
        protected void InvokeOutPutChangeEvent(OutputTCBase tc,string newOutputStr)
        {
            if (this.OnOutputChanged != null  )
                this.OnOutputChanged(tc, newOutputStr);
        }
        public void ChangeDisplayCheckCycle(int min)
        {
            this.m_CheckOutputCycle = min;
            tmrCheckOutput.Interval = min * 60 * 1000;
            tmrCheckOutput.Stop();
            tmrCheckOutput.Start();
        }
        protected abstract void CheckDisplayTask();
       
     //   public abstract void TC_SetDisplayOff();

   }
}
