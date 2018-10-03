using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Annotations;
using Hangfire.Server;

namespace Hangfire.RecurringJobExtensions.Manager
{
    /// <summary>   Interface for scheduzle job manager. </summary>
    public interface IScheduleJobManager
    {
        /// <summary>   Executes the schedule asynchronous operation. </summary>
        /// <typeparam name="TJob">     Type of the job. </typeparam>
        /// <typeparam name="TJobData"> Type of the job data. </typeparam>
        /// <param name="args"> The arguments. </param>
        /// <returns>   An asynchronous result.. This will never be null. </returns>
        [NotNull]
        Task ExecuteScheduleAsync<TJob, TJobData>(TJobData args) where TJob : IBackgroundJob, new()
            where TJobData : IJobData;

        /// <summary>   Executes the background job asynchronous operation. </summary>
        /// <typeparam name="TJob">     Type of the job. </typeparam>
        /// <typeparam name="TJobData"> Type of the job data. </typeparam>
        /// <param name="context">  The context. </param>
        /// <param name="delay">    (Optional) The delay. </param>
        /// <param name="jobData">  A variable-length parameters list containing job data. </param>
        /// <returns>   An asynchronous result.. This will never be null. </returns>
        [NotNull]
        Task ExecuteBackgroundJobAsync<TJob, TJobData>(PerformContext context, TimeSpan? delay = null,
            params TJobData[] jobData)
            where TJob : IBackgroundJob<TJobData>, new()
            where TJobData : IJobData;
    }
}