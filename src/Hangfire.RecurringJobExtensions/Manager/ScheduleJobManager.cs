using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Server;

namespace Hangfire.RecurringJobExtensions.Manager
{
    /// <summary>   Manager for schedule jobs. </summary>
    public class ScheduleJobManager : IScheduleJobManager
    {
        /// <summary>   Executes the schedule asynchronous operation. </summary>
        /// <typeparam name="TJob">     Type of the job. </typeparam>
        /// <typeparam name="TJobData"> Type of the job data. </typeparam>
        /// <param name="args"> The arguments. </param>
        /// <returns>   An asynchronous result. </returns>
        public Task ExecuteScheduleAsync<TJob, TJobData>(TJobData args)
            where TJob : IBackgroundJob, new()
            where TJobData : IJobData
        {
            CronJob.AddOrUpdate<TJob>(j => j.Execute(null), args);

            return Task.FromResult(0);
        }

        /// <summary>   Executes the background job asynchronous operation. </summary>
        /// <typeparam name="TJob">     Type of the job. </typeparam>
        /// <typeparam name="TJobData"> Type of the job data. </typeparam>
        /// <param name="context">  The context. </param>
        /// <param name="delay">    (Optional) The delay. </param>
        /// <param name="jobData">  A variable-length parameters list containing job data. </param>
        /// <returns>   An asynchronous result. </returns>
        public Task ExecuteBackgroundJobAsync<TJob, TJobData>(PerformContext context, TimeSpan? delay = null, params TJobData[] jobData)
            where TJob : IBackgroundJob<TJobData>, new()
            where TJobData : IJobData
        {
            if (!delay.HasValue)
            {
                BackgroundJob.Enqueue<TJob>(job => job.Execute(context, jobData));
            }
            else
            {
                BackgroundJob.Schedule<TJob>(job => job.Execute(context, jobData), delay.Value);
            }
            return Task.FromResult(0);
        }
    }
}