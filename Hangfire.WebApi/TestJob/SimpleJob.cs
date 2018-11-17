using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Hangfire.RecurringJobExtensions;
using Hangfire.RecurringJobExtensions.Manager;
using Hangfire.Server;
using Newtonsoft.Json;

namespace Hangfire.WebApi.TestJob
{
    public class SimpleJob : IBackgroundJob
    {
        [RecurringJob("*/3 * * * *")]//At every 3th minute.
        public Task Execute(PerformContext context)
        {
            var ctx = context as PerformContext;
            var data = ctx.GetJobData();

            Debug.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} {JsonConvert.SerializeObject(data)} Running ...");
            return Task.CompletedTask;
        }
    }

    public class SimpleJobArgs1 : IJobData
    {
        public string Name { get; set; }
        public SimpleJobArgsData1 Data { get; set; }
        public string OrderNo { get; set; }
    }

    public class SimpleJobArgsData1
    {
        public string OrderNo { get; set; }
        public string MerNo { get; set; }
    }
}