using System;
using System.Collections.Generic;
using Hangfire.Common;
using Hangfire.Storage;

namespace Hangfire.RecurringJobExtensions
{
	public class RecurringJobInfoStorage : IRecurringJobInfoStorage
	{
		private static readonly TimeSpan LockTimeout = TimeSpan.FromMinutes(1);

		private readonly IStorageConnection _connection;

		public RecurringJobInfoStorage() : this(JobStorage.Current.GetConnection()) { }

		public RecurringJobInfoStorage(IStorageConnection connection)
		{
			if (connection == null) throw new ArgumentNullException(nameof(connection));

			_connection = connection;
		}

		public IEnumerable<RecurringJobInfo> FindAll()
		{
			var recurringJobIds = _connection.GetAllItemsFromSet("recurring-jobs");

			foreach (var recurringJobId in recurringJobIds)
			{
				var recurringJob = _connection.GetAllEntriesFromHash($"recurring-job:{recurringJobId}");

				if (recurringJob == null) continue;

				yield return InternalFind(recurringJobId, recurringJob);
			}
		}
		public RecurringJobInfo FindByJobId(string jobId)
		{
			if (string.IsNullOrEmpty(jobId)) throw new ArgumentNullException(nameof(jobId));

			var paramValue = _connection.GetJobParameter(jobId, "RecurringJobId");

			var recurringJobId = JobHelper.FromJson<string>(paramValue);

			return FindByRecurringJobId(recurringJobId);
		}

		public RecurringJobInfo FindByRecurringJobId(string recurringJobId)
		{
			if (string.IsNullOrEmpty(recurringJobId)) throw new ArgumentNullException(nameof(recurringJobId));

			var recurringJob = _connection.GetAllEntriesFromHash($"recurring-job:{recurringJobId}");

			if (recurringJob == null) return null;

			return InternalFind(recurringJobId, recurringJob);
		}

		private RecurringJobInfo InternalFind(string recurringJobId, Dictionary<string, string> recurringJob)
		{
			if (string.IsNullOrEmpty(recurringJobId)) throw new ArgumentNullException(nameof(recurringJobId));
			if (recurringJob == null) throw new ArgumentNullException(nameof(recurringJob));

			var serializedJob = JobHelper.FromJson<InvocationData>(recurringJob["Job"]);
			var job = serializedJob.Deserialize();

			return new RecurringJobInfo
			{
				RecurringJobId = recurringJobId,
				Cron = recurringJob["Cron"],
				TimeZone = recurringJob.ContainsKey("TimeZoneId")
					? TimeZoneInfo.FindSystemTimeZoneById(recurringJob["TimeZoneId"])
					: TimeZoneInfo.Utc,
				Queue = recurringJob["Queue"],
				Method = job.Method,
				Enable = recurringJob.ContainsKey(nameof(RecurringJobInfo.Enable))
					? JobHelper.FromJson<bool>(recurringJob[nameof(RecurringJobInfo.Enable)])
					: true,
				JobData = recurringJob.ContainsKey(nameof(RecurringJobInfo.JobData))
					? JobHelper.FromJson<Dictionary<string, object>>(recurringJob[nameof(RecurringJobInfo.JobData)])
					: null
			};
		}

		public void SetJobData(RecurringJobInfo recurringJobInfo)
		{
			if (recurringJobInfo == null) throw new ArgumentNullException(nameof(recurringJobInfo));

			if (recurringJobInfo.JobData == null || recurringJobInfo.JobData.Count == 0) return;

			using (_connection.AcquireDistributedLock($"recurringjobextensions-jobdata:{recurringJobInfo.RecurringJobId}", LockTimeout))
			{
				var changedFields = new Dictionary<string, string>
				{
					[nameof(RecurringJobInfo.Enable)] = JobHelper.ToJson(recurringJobInfo.Enable),
					[nameof(RecurringJobInfo.JobData)] = JobHelper.ToJson(recurringJobInfo.JobData)
				};

				_connection.SetRangeInHash($"recurring-job:{recurringJobInfo.RecurringJobId}", changedFields);
			}
		}

		public void Dispose()
		{
			_connection?.Dispose();
		}
	}
}
