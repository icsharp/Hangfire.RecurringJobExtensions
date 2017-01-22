
namespace Hangfire.RecurringJobExtensions
{
	public interface IRecurringJobDataChanged
	{
		void NotifyFilter(string recurringJobId);
	}
}
