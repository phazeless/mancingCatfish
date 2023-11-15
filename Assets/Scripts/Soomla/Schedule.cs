using System;
using System.Collections.Generic;

namespace Soomla
{
	public class Schedule
	{
		public Schedule(int activationLimit) : this(null, Schedule.Recurrence.NONE, activationLimit)
		{
		}

		public Schedule(DateTime startTime, DateTime endTime, Schedule.Recurrence recurrence, int activationLimit) : this(new List<Schedule.DateTimeRange>
		{
			new Schedule.DateTimeRange(startTime, endTime)
		}, recurrence, activationLimit)
		{
		}

		public Schedule(List<Schedule.DateTimeRange> timeRanges, Schedule.Recurrence recurrence, int activationLimit)
		{
			this.TimeRanges = timeRanges;
			this.RequiredRecurrence = recurrence;
			this.ActivationLimit = activationLimit;
		}

		public Schedule(JSONObject jsonSched)
		{
			if (jsonSched["schedRecurrence"])
			{
				this.RequiredRecurrence = (Schedule.Recurrence)jsonSched["schedRecurrence"].n;
			}
			else
			{
				this.RequiredRecurrence = Schedule.Recurrence.NONE;
			}
			this.ActivationLimit = (int)Math.Ceiling(jsonSched["schedApprovals"].n);
			this.TimeRanges = new List<Schedule.DateTimeRange>();
			if (jsonSched["schedTimeRanges"])
			{
				List<JSONObject> list = jsonSched["schedTimeRanges"].list;
				foreach (JSONObject jsonobject in list)
				{
					DateTime start = new DateTime(TimeSpan.FromMilliseconds((double)((long)jsonobject["schedTimeRangeStart"].n)).Ticks);
					DateTime end = new DateTime(TimeSpan.FromMilliseconds((double)((long)jsonobject["schedTimeRangeEnd"].n)).Ticks);
					this.TimeRanges.Add(new Schedule.DateTimeRange(start, end));
				}
			}
		}

		public static Schedule AnyTimeOnce()
		{
			return new Schedule(1);
		}

		public static Schedule AnyTimeLimited(int activationLimit)
		{
			return new Schedule(activationLimit);
		}

		public static Schedule AnyTimeUnLimited()
		{
			return new Schedule(0);
		}

		public JSONObject toJSONObject()
		{
			JSONObject jsonobject = new JSONObject(JSONObject.Type.OBJECT);
			jsonobject.AddField("className", SoomlaUtils.GetClassName(this));
			jsonobject.AddField("schedRecurrence", (int)this.RequiredRecurrence);
			jsonobject.AddField("schedApprovals", this.ActivationLimit);
			JSONObject jsonobject2 = new JSONObject(JSONObject.Type.ARRAY);
			if (this.TimeRanges != null)
			{
				foreach (Schedule.DateTimeRange dateTimeRange in this.TimeRanges)
				{
					long num = dateTimeRange.Start.Ticks / 10000L;
					long num2 = dateTimeRange.End.Ticks / 10000L;
					JSONObject jsonobject3 = new JSONObject(JSONObject.Type.OBJECT);
					jsonobject3.AddField("className", SoomlaUtils.GetClassName(dateTimeRange));
					jsonobject3.AddField("schedTimeRangeStart", (float)num);
					jsonobject3.AddField("schedTimeRangeEnd", (float)num2);
					jsonobject2.Add(jsonobject3);
				}
			}
			jsonobject.AddField("schedTimeRanges", jsonobject2);
			return jsonobject;
		}

		public bool Approve(int activationTimes)
		{
			DateTime now = DateTime.Now;
			if (this.ActivationLimit < 1 && (this.TimeRanges == null || this.TimeRanges.Count == 0))
			{
				SoomlaUtils.LogDebug(Schedule.TAG, "There's no activation limit and no TimeRanges. APPROVED!");
				return true;
			}
			if (this.ActivationLimit > 0 && activationTimes >= this.ActivationLimit)
			{
				SoomlaUtils.LogDebug(Schedule.TAG, "Activation limit exceeded.");
				return false;
			}
			if (this.TimeRanges == null || this.TimeRanges.Count == 0)
			{
				SoomlaUtils.LogDebug(Schedule.TAG, "We have an activation limit that was not reached. Also, we don't have any time ranges. APPROVED!");
				return true;
			}
			foreach (Schedule.DateTimeRange dateTimeRange in this.TimeRanges)
			{
				if (now >= dateTimeRange.Start && now <= dateTimeRange.End)
				{
					SoomlaUtils.LogDebug(Schedule.TAG, "We are just in one of the time spans, it can't get any better then that. APPROVED!");
					return true;
				}
			}
			if (this.RequiredRecurrence == Schedule.Recurrence.NONE)
			{
				return false;
			}
			foreach (Schedule.DateTimeRange dateTimeRange2 in this.TimeRanges)
			{
				if (now.Minute >= dateTimeRange2.Start.Minute && now.Minute <= dateTimeRange2.End.Minute)
				{
					SoomlaUtils.LogDebug(Schedule.TAG, "Now is in one of the time ranges' minutes span.");
					if (this.RequiredRecurrence == Schedule.Recurrence.EVERY_HOUR)
					{
						SoomlaUtils.LogDebug(Schedule.TAG, "It's a EVERY_HOUR recurrence. APPROVED!");
						return true;
					}
					if (now.Hour >= dateTimeRange2.Start.Hour && now.Hour <= dateTimeRange2.End.Hour)
					{
						SoomlaUtils.LogDebug(Schedule.TAG, "Now is in one of the time ranges' hours span.");
						if (this.RequiredRecurrence == Schedule.Recurrence.EVERY_DAY)
						{
							SoomlaUtils.LogDebug(Schedule.TAG, "It's a EVERY_DAY recurrence. APPROVED!");
							return true;
						}
						if (now.DayOfWeek >= dateTimeRange2.Start.DayOfWeek && now.DayOfWeek <= dateTimeRange2.End.DayOfWeek)
						{
							SoomlaUtils.LogDebug(Schedule.TAG, "Now is in one of the time ranges' day-of-week span.");
							if (this.RequiredRecurrence == Schedule.Recurrence.EVERY_WEEK)
							{
								SoomlaUtils.LogDebug(Schedule.TAG, "It's a EVERY_WEEK recurrence. APPROVED!");
								return true;
							}
							if (now.Day >= dateTimeRange2.Start.Day && now.Day <= dateTimeRange2.End.Day)
							{
								SoomlaUtils.LogDebug(Schedule.TAG, "Now is in one of the time ranges' days span.");
								if (this.RequiredRecurrence == Schedule.Recurrence.EVERY_MONTH)
								{
									SoomlaUtils.LogDebug(Schedule.TAG, "It's a EVERY_MONTH recurrence. APPROVED!");
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		private static string TAG = "SOOMLA Schedule";

		public Schedule.Recurrence RequiredRecurrence;

		public List<Schedule.DateTimeRange> TimeRanges;

		public int ActivationLimit;

		public enum Recurrence
		{
			EVERY_MONTH,
			EVERY_WEEK,
			EVERY_DAY,
			EVERY_HOUR,
			NONE
		}

		public class DateTimeRange
		{
			public DateTimeRange(DateTime start, DateTime end)
			{
				this.Start = start;
				this.End = end;
			}

			public DateTime Start;

			public DateTime End;
		}
	}
}
