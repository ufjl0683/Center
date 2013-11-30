using System;

namespace Host.Schedule
{
	/// <summary>
	/// Summary description for SchedulerException.
	/// </summary>
	public class SchedulerException : Exception
	{
		public SchedulerException(string msg) : base(msg)
		{
		}
	}
}
