using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Hangfire.RecurringJobExtensions.Manager;
using Hangfire.Server;

namespace Hangfire.WebApi.TestJob
{
    public class SimpleJob2 : IBackgroundJob<SimpleJobArgs2>
    {
        public Task Execute(PerformContext context, params SimpleJobArgs2[] args)
        {
            var ctx = context as PerformContext;

            Debug.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} {args[0].Name}'s SimpleJobArgs2 (ExecuteBackgroundJobAsync) is Running ...");
            return Task.CompletedTask;
        }
    }

    public class SimpleJobArgs2 : IJobData
    {
        public string Name { get; set; }
        public SimpleJobArgsData2 Data { get; set; }
        public string OrderNo { get; set; }
    }

    public class SimpleJobArgsData2
    {
        public string OrderNo { get; set; }
        public string MerNo { get; set; }
    }
}