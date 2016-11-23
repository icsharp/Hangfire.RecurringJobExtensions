using System;
using System.Collections.Generic;
using Hangfire.Server;

namespace Hangfire.RecurringJobExtensions
{
	/// <summary>
	/// 
	/// </summary>
	public static class PerformContextExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static object GetJobData(this PerformContext context, string name)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

			var jobDataKey = $"recurringjob-info-{context.BackgroundJob.Job.Method.GetRecurringJobId()}";

			if (!context.Items.ContainsKey(jobDataKey)) return null;

			var jobData = context.Items[jobDataKey] as IDictionary<string, object>;

			if (jobData == null || jobData.Count == 0) return null;

			if (!jobData.ContainsKey(name)) return null;

			return jobData[name];
		}
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="context"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static T GetJobData<T>(this PerformContext context, string name)
		{
			var o = GetJobData(context, name);

			return o == null ? default(T) : (T)o;
		}
	}
}
