using System;

namespace Host.Schedule
{
	// OneTimeSchedule is used to schedule an event to run only once
	// Used by specific tasks to check self status
    [Serializable]
	public class OneTimeSchedule : Schedule
	{
      //  public int durMin;
        public OneTimeSchedule( string schid, DateTime startTime, ScheduleJob[] jobs,bool IsPrimary)
            : this(schid, startTime, 0, jobs, IsPrimary)
		{
		}
        public OneTimeSchedule(string schid, DateTime startTime, int durMin, ScheduleJob[] jobs, bool IsPrimary)
            : base(schid, startTime, ScheduleType.ONETIME, jobs, IsPrimary)
        {
            //this.durMin = durMin;
            this.m_durationMin = durMin;
        }
		internal override void CalculateNextInvokeTime()
		{
			// it does not matter, since this is a one time schedule
			//m_nextTime = DateTime.MaxValue;
            if(this.IsPrimary)
                m_nextTime = DateTime.MaxValue;
            else
                m_nextTime = this.StartTime.AddMinutes(this.m_durationMin);
		}
	}

	// IntervalSchedule is used to schedule an event to be invoked at regular intervals
	// the interval is specified in seconds. Useful mainly in checking status of threads
	// and connections. Use an interval of 60 hours for an hourly schedule
    [Serializable]
	public class IntervalSchedule : Schedule
	{
		public IntervalSchedule( string schid, DateTime startTime, int secs,
                    TimeSpan fromTime, TimeSpan toTime, ScheduleJob[] jobs, bool IsPrimary) // time range for the day
            : base(schid, startTime, ScheduleType.INTERVAL, jobs, IsPrimary)
		{
			m_fromTime = fromTime;

			m_toTime = toTime;
			Interval = secs;
		}
		internal override void CalculateNextInvokeTime()
		{
			// add the interval of m_seconds
			m_nextTime = m_nextTime.AddSeconds(Interval);

			// if next invoke time is not within the time range, then set it to next start time
			if (! IsInvokeTimeInTimeRange())
			{
				if (m_nextTime.TimeOfDay < m_fromTime)
					m_nextTime.AddSeconds(m_fromTime.Seconds - m_nextTime.TimeOfDay.Seconds);
				else
					m_nextTime.AddSeconds((24 * 3600) - m_nextTime.TimeOfDay.Seconds + m_fromTime.Seconds);
			}

			// check to see if the next invoke time is on a working day
			while (! CanInvokeOnNextWeekDay())
				m_nextTime = m_nextTime.AddDays(1); // start checking on the next day
		}
	}

	// Daily schedule is used set off to the event every day
	// Mainly useful in maintanance, recovery, logging and report generation
	// Restictions can be imposed on the week days on which to run the schedule
    [Serializable]
	public class DailySchedule : Schedule
	{
        public DailySchedule(string schid, DateTime startTime, int durationMin, ScheduleJob[] jobs, bool IsPrimary)
            : base(schid, startTime, ScheduleType.DAILY, durationMin, jobs, IsPrimary)
		{
		}

		internal override void CalculateNextInvokeTime()
		{
			// add a day, and check for any weekday restrictions and keep adding a day
			m_nextTime = m_nextTime.AddDays(1);
			while (! CanInvokeOnNextWeekDay())
				m_nextTime = m_nextTime.AddDays(1);
		}
	}

	// Weekly schedules, useful generally in lazy maintanance jobs and
	// restarting services and others major jobs
    [Serializable]
	public class WeeklySchedule : Schedule
	{
        public WeeklySchedule(string schid, DateTime startTime, ScheduleJob[] jobs, bool IsPrimary)
            : base(schid, startTime, ScheduleType.WEEKLY, jobs, IsPrimary)
		{
		}
		// add a week (or 7 days) to the date
		internal override void CalculateNextInvokeTime()
		{
			m_nextTime = m_nextTime.AddDays(7);
		}
	}

	// Monthly schedule - used to kick off an event every month on the same day as scheduled
	// and also at the same hour and minute as given in start time
    [Serializable]
	public class MonthlySchedule : Schedule
	{
        public MonthlySchedule(string schid, DateTime startTime, ScheduleJob[] jobs,bool IsPrimary)
            : base(schid, startTime, ScheduleType.MONTHLY, jobs, IsPrimary)
		{
		}
		// add a month to the present time
		internal override void CalculateNextInvokeTime()
		{
			m_nextTime = m_nextTime.AddMonths(1);
		}
	}
}
