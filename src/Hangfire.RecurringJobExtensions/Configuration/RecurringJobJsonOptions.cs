using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hangfire.RecurringJobExtensions.Configuration
{
	/// <summary>
	/// <see cref="RecurringJob"/> json configuration settings.
	/// </summary>
	public class RecurringJobJsonOptions
	{
		/// <summary>
		/// The job name represents for <see cref="RecurringJobInfo.RecurringJobId"/>
		/// </summary>
		[JsonProperty("job-name")]
#if !NET45
		[JsonRequired]
#endif
		public string JobName { get; set; }
		/// <summary>
		/// The job type while impl the interface <see cref="IRecurringJob"/>.
		/// </summary>
		[JsonProperty("job-type")]
#if !NET45
		[JsonRequired]
#endif
		public Type JobType { get; set; }

		/// <summary>
		/// Cron expressions
		/// </summary>
		[JsonProperty("cron-expression")]
#if !NET45
		[JsonRequired]
#endif
		public string Cron { get; set; }

		/// <summary>  
		/// The value of <see cref="TimeZoneInfo"/> can be created by <seealso cref="TimeZoneInfo.FindSystemTimeZoneById(string)"/>
		/// </summary>
		[JsonProperty("timezone")]
		[JsonConverter(typeof(TimeZoneInfoConverter))]
		public TimeZoneInfo TimeZone { get; set; }
		/// <summary>
		/// Whether the property <see cref="TimeZone"/> can be serialized or not.
		/// </summary>
		/// <returns>true if value not null, otherwise false.</returns>
		public bool ShouldSerializeTimeZone() => TimeZone != null;
		/// <summary>
		/// Hangfire queue name
		/// </summary>
		[JsonProperty("queue")]
		public string Queue { get; set; }
		/// <summary>
		/// Whether the property <see cref="Queue"/> can be serialized or not.
		/// </summary>
		/// <returns>true if value not null or empty, otherwise false.</returns>
		public bool ShouldSerializeQueue() => !string.IsNullOrEmpty(Queue);
		/// <summary>
		/// 
		/// </summary>
		[JsonProperty("job-data")]
		public IDictionary<string, object> ExtendedData { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeExtendedData() => ExtendedData != null && ExtendedData.Count > 0;

		/// <summary>
		/// 
		/// </summary>
		[JsonProperty("enable")]
		public bool? Enable { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeEnable() => Enable.HasValue;
	}
}
