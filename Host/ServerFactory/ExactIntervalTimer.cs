using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface
{

    
    public delegate void OnElaspedHandler (object sender);
    public   class ExactIntervalTimer
    {
        int atHour; int atMin; int atSec;
        System.Threading.Timer tmr;
        public event OnConnectEventHandler OnElapsed;

        System.DateTime nextInvokeTime;

       public    ExactIntervalTimer(int atHour, int atMin, int atSec)
          {
              tmr=new System.Threading.Timer(tmr_Elapse);
              this.atHour= atHour;
              this.atMin=atMin;
              this.atSec=atSec;

              System.DateTime now = System.DateTime.Now;
              if (atHour != -1 && atMin != -1 && atSec != -1)
              {


                  if (now < new System.DateTime(now.Year, now.Month, now.Day, atHour, atMin, atSec))

                      nextInvokeTime = new System.DateTime(now.Year, now.Month, now.Day, atHour, atMin, atSec);
                  else
                      nextInvokeTime = new System.DateTime(now.Year, now.Month, now.Day, atHour, atMin, atSec).AddHours(24);

                 
              }
              else if (atHour == -1 && atMin != -1 && atSec != -1)
                  {


                      if (now < new System.DateTime(now.Year, now.Month, now.Day, now.Hour, atMin, atSec))

                          nextInvokeTime = new System.DateTime(now.Year, now.Month, now.Day, now.Hour, atMin, atSec);
                      else
                          nextInvokeTime = new System.DateTime(now.Year, now.Month, now.Day, now.Hour, atMin, atSec).AddHours(1);

                    
                  }
                  else if (atHour == -1 && atMin == -1 && atSec != -1)
                  {


                      if (now < new System.DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, atSec))

                          nextInvokeTime = new System.DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, atSec);
                      else
                          nextInvokeTime = new System.DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, atSec).AddMinutes(1);

                   //   tmr.Change(((TimeSpan)(nextInvokeTime - System.DateTime.Now)).Milliseconds, 1000 * 60  );
                  }

                  try
                  {
                      tmr.Change((int)((TimeSpan)(nextInvokeTime - now)).TotalMilliseconds, System.Threading.Timeout.Infinite);
                  }
                  catch (Exception ex)
                  {
                      tmr.Change(1, System.Threading.Timeout.Infinite);
                  }
          
          
            
          }
       public  ExactIntervalTimer( int atMin, int atSec):this(-1,atMin,atSec)
        {
           
        }

      public   ExactIntervalTimer( int atSec):this(-1,-1,atSec)
        {
          

        }

        private void tmr_Elapse(object sender)
        {
            if (this.OnElapsed != null)
            {
                try
                {
                    this.OnElapsed(this);
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                }

            }

            if (atHour != -1 && atMin != -1 && atSec != -1)
                while (nextInvokeTime < DateTime.Now)
                 nextInvokeTime = nextInvokeTime.AddHours(24);
            else if (atHour == -1 && atMin != -1 && atSec != -1)
                while (nextInvokeTime < DateTime.Now)
                 nextInvokeTime = nextInvokeTime.AddHours(1);
            else if (atHour == -1 && atMin == -1 && atSec != -1)
                while(nextInvokeTime < DateTime.Now)
                    nextInvokeTime = nextInvokeTime.AddMinutes(1);
           

                //if (((TimeSpan)(nextInvokeTime - System.DateTime.Now)).TotalMilliseconds < 0)
                //    tmr.Change(1, System.Threading.Timeout.Infinite);
                //else

            try
            {
                tmr.Change((int)((TimeSpan)(nextInvokeTime - System.DateTime.Now)).TotalMilliseconds, System.Threading.Timeout.Infinite);
            }
            catch
            {
                tmr.Change(1, System.Threading.Timeout.Infinite);
            }
          
            
        }
      

    }
}
