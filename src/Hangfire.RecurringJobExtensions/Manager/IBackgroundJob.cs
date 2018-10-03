using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Server;

namespace Hangfire.RecurringJobExtensions.Manager
{
    /// <summary>   Interface for background job. </summary>
    public interface IBackgroundJob
    {
        /// <summary>   Executes the given context. </summary>
        /// <param name="context">  The context. </param>
        /// <returns>   An asynchronous result. </returns>
        Task Execute(PerformContext context);
    }

    /// <summary>   Interface for background job. </summary>
    /// <typeparam name="TJobData"> Type of the job data. </typeparam>
    public interface IBackgroundJob<in TJobData> where TJobData : IJobData
    {
        /// <summary>   Executes. </summary>
        /// <param name="context">  The context. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>
        /// <returns>   An asynchronous result. </returns>
        Task Execute(PerformContext context, params TJobData[] args);
    }
}