using System;
using System.Collections.Generic;
using Hangfire.Common;
using Hangfire.Server;

namespace Hangfire.RecurringJobExtensions
{
	/// <summary>
	/// Extensions for <see cref="PerformContext"/>.
	/// </summary>
	public static class PerformContextExtensions
	{
		/// <summary>
		/// Gets job data from <see cref="PerformContext"/> if json configuration exists token 'job-data'.
		/// </summary>
		/// <param name="context">The <see cref="PerformContext"/>.</param>
		/// <param name="name">The dictionary key from the property <see cref="RecurringJobInfo.ExtendedData"/></param>
		/// <returns>The value from the property <see cref="RecurringJobInfo.ExtendedData"/></returns>
		public static object GetJobData(this PerformContext context, string name)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

			var jobDataKey = $"recurringjob-info-{context.BackgroundJob.Job.ToString()}";

			if (!context.Items.ContainsKey(jobDataKey)) return null;

			var jobData = context.Items[jobDataKey] as IDictionary<string, object>;

			if (jobData == null || jobData.Count == 0) return null;

			if (!jobData.ContainsKey(name)) return null;

			return jobData[name];
		}
		/// <summary>
		/// Gets job data from <see cref="PerformContext"/> if json configuration exists token 'job-data'.
		/// </summary>
		/// <typeparam name="T">The specified type to json value.</typeparam>
		/// <param name="context">The <see cref="PerformContext"/>.</param>
		/// <param name="name">The dictionary key from the property <see cref="RecurringJobInfo.ExtendedData"/></param>
		/// <returns>The value from the property <see cref="RecurringJobInfo.ExtendedData"/></returns>
		public static T GetJobData<T>(this PerformContext context, string name)
		{
			var o = GetJobData(context, name);

			var json = JobHelper.ToJson(o);

			return JobHelper.FromJson<T>(json);
		}
		/// <summary>
		/// Persists job data to <see cref="PerformContext"/>.
		/// </summary>
		/// <param name="context">The <see cref="PerformContext"/>.</param>
		/// <param name="name">The dictionary key from the property <see cref="RecurringJobInfo.ExtendedData"/></param>
		/// <param name="value">The persisting value.</param>
		public static void SetJobData(this PerformContext context, string name, object value)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

			var jobDataKey = $"recurringjob-info-{context.BackgroundJob.Job.ToString()}";

			if (!context.Items.ContainsKey(jobDataKey))
				throw new KeyNotFoundException($"The job data key: {jobDataKey} is not found.");

			var jobData = context.Items[jobDataKey] as IDictionary<string, object>;

			if (jobData == null) jobData = new Dictionary<string, object>();

			jobData[name] = value;
		}
	}
}
