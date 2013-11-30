using System;
using System.Threading;
using System.Collections;
using Host.Schedule;
using RemoteInterface;

namespace Host.Schedule
{
	// enumeration of Scheduler events used by the delegate
	public enum SchedulerEventType { CREATED, DELETED, INVOKED };
   
	// delegate for Scheduler events
	public delegate void SchedulerEventDelegate(SchedulerEventType type, string schid);

	// This is the main class which will maintain the list of Schedules
	// and also manage them, like rescheduling, deleting schedules etc.
	public sealed class Scheduler
	{
		// Event raised when for any event inside the scheduler
		static public event SchedulerEventDelegate OnSchedulerEvent;
        static object lockObj = new object();

		// next event which needs to be kicked off,
		// this is set when a new Schedule is added or after invoking a Schedule
		static Host.Schedule.Schedule m_nextSchedule = null;
		static ArrayList m_schedulesList = new ArrayList(); // list of schedles
		static Timer m_timer = new Timer(new TimerCallback(DispatchEvents), // main timer
											null,
											Timeout.Infinite,
											Timeout.Infinite);

		// Get schedule at a particular index in the array list
        public static Host.Schedule.Schedule GetScheduleAt(int index)
		{
			if (index < 0 || index >= m_schedulesList.Count)
				return null;
            return (Host.Schedule.Schedule)m_schedulesList[index];
		}

		// Number of schedules in the list
		public static int Count()
		{
			return m_schedulesList.Count;
		}

		// Indexer to access a Schedule object by name
        public static Host.Schedule.Schedule GetSchedule(string schid)
		{
			for (int index=0; index < m_schedulesList.Count; index++)
                if (((Host.Schedule.Schedule)m_schedulesList[index]).schid == schid)
                    return (Host.Schedule.Schedule)m_schedulesList[index];
			return null;
		}

		// call back for the timer function
		static void DispatchEvents(object obj) // obj ignored
		{
            lock (lockObj)
            {
                try
                {

                    if (m_nextSchedule == null)
                        return;
                    if (System.Math.Abs(m_nextSchedule.NextInvokeTime.Subtract(DateTime.Now).TotalSeconds) > 10)
                    {
                        RemoteInterface.ConsoleServer.WriteLine(m_nextSchedule.schid + ", not yet!");
                        return;
                    }
                    Schedule temp = m_nextSchedule;   //®æ§ÓDoSchedule m_next ßÔ≈‹
                    if (temp.Type == ScheduleType.ONETIME)
                    {

                        if (temp.IsPrimary)
                        {

                            //   Schedule  temp = m_nextSchedule; 
                            try
                            {
                                temp.DoScheduleTask();
                                temp.ScheduleEndTask();
                                //m_nextSchedule.DoScheduleTask();
                                //m_nextSchedule.ScheduleEndTask();
                                RemoveSchedule(temp); // remove the schedule from the list
                            }
                            catch { ;}

                                m_schedulesList.Sort();
                          
                            SetNextEventTime();
                            return;
                        }
                        else

                            if (temp.NextInvokeTime == m_nextSchedule.StartTime.AddMinutes(temp.m_durationMin))
                            {
                                // Schedule temp = m_nextSchedule; 
                                if (temp.m_durationMin == 0)
                                    temp.DoScheduleTask();
                                temp.ScheduleEndTask();
                                RemoveSchedule(temp); // remove the schedule from the list
                                m_schedulesList.Sort();
                                SetNextEventTime();
                                return;
                            }
                    }
                    try
                    {
                        temp.CalculateNextInvokeTime();
                    }
                    catch { ;}
                    //  m_nextSchedule.TriggerEvents(); // make this happen on a thread to let this thread continue
                    //else
                    //{
                    //if (OnSchedulerEvent != null)
                    //    OnSchedulerEvent(SchedulerEventType.INVOKED, m_nextSchedule.schid);
                    //  Schedule tmp = m_nextSchedule;
                    // m_nextSchedule.DoScheduleTask();
                    temp.DoScheduleTask();
                    m_schedulesList.Sort();
                    SetNextEventTime();
                }
                catch (Exception ex)
                {
                    ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
                }
               }
          
            //}
		}

		// method to set the time when the timer should wake up to invoke the next schedule
		static void SetNextEventTime()
		{
            try
            {
                if (m_schedulesList.Count == 0)
                {
                    m_timer.Change(Timeout.Infinite, Timeout.Infinite); // this will put the timer to sleep
                    return;
                }
                m_nextSchedule = (Host.Schedule.Schedule)m_schedulesList[0];
                TimeSpan ts = m_nextSchedule.NextInvokeTime.Subtract(DateTime.Now);
                if (ts < TimeSpan.Zero)
                    ts = TimeSpan.Zero; // cannot be negative !
                m_timer.Change((int)ts.TotalMilliseconds, Timeout.Infinite); // invoke after the timespan
            }
            catch
            { ;}
            }

		// add a new schedule
        public static void AddSchedule(Host.Schedule.Schedule s)
		{

           
                if (GetSchedule(s.schid) != null)
                    throw new SchedulerException("Schedule with the same schid already exists");
                m_schedulesList.Add(s);
                m_schedulesList.Sort();
                // adjust the next event time if schedule is added at the top of the list
                if (m_schedulesList[0] == s)
                    SetNextEventTime();
                if (OnSchedulerEvent != null)
                    OnSchedulerEvent(SchedulerEventType.CREATED, s.schid);
           
		}

		// remove a schedule object from the list
        public static void RemoveSchedule(Host.Schedule.Schedule s)
		{
            
                m_schedulesList.Remove(s);
            //    SetNextEventTime();
                if (OnSchedulerEvent != null)
                    OnSchedulerEvent(SchedulerEventType.DELETED, s.schid);
            
		}

		// remove schedule by name
		public static void RemoveSchedule(string schid)
		{
			RemoveSchedule(GetSchedule(schid));
		}
	}
}
