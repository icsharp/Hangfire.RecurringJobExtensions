using System;
using System.Collections.Generic;

namespace Hangfire.RecurringJobExtensions
{
	public interface IRecurringJobInfoStorage : IDisposable
	{
		IEnumerable<RecurringJobInfo> FindAll();

		RecurringJobInfo FindByJobId(string jobId);

		RecurringJobInfo FindByRecurringJobId(string recurringJobId);

		void SetJobData(RecurringJobInfo recurringJobInfo);
	}
}
