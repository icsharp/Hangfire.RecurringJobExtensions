using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Hangfire.Server;

namespace Hangfire.RecurringJobExtensions
{
	/// <summary>
	/// Server filter
	/// </summary>
	public class ExtendedDataJobFilter : IServerFilter
	{
		/// <summary>
		/// 
		/// </summary>
		public ConcurrentDictionary<string, RecurringJobInfo> RecurringJobInfos { get; } = new ConcurrentDictionary<string, RecurringJobInfo>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="recurringJobInfos"></param>
		public ExtendedDataJobFilter(IList<RecurringJobInfo> recurringJobInfos)
		{
			if (recurringJobInfos == null) throw new ArgumentNullException(nameof(recurringJobInfos));

			//initialize data if exists extendeddata.
			foreach (var jobInfo in recurringJobInfos.Where(x => x.Enable && x.ExtendedData != null && x.ExtendedData.Count > 0))
				RecurringJobInfos.TryAdd(jobInfo.RecurringJobId, jobInfo);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filterContext"></param>
		public void OnPerforming(PerformingContext filterContext)
		{
			if (RecurringJobInfos == null
				|| RecurringJobInfos.Count == 0
				|| !RecurringJobInfos.ContainsKey(filterContext.BackgroundJob.Id)) return;

			var jobInfo = RecurringJobInfos[filterContext.BackgroundJob.Id];

			if (jobInfo == null
				|| jobInfo.ExtendedData == null
				|| jobInfo.ExtendedData.Count == 0) return;

			var jobDataKey = $"recurringjob-info-{jobInfo.Method.GetRecurringJobId()}";

			filterContext.Items[jobDataKey] = jobInfo.ExtendedData;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filterContext"></param>
		public void OnPerformed(PerformedContext filterContext)
		{
			//do nothing?
		}

	}
}
