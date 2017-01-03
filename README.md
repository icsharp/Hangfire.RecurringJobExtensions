# Hangfire.RecurringJobExtensions

[![Official Site](https://img.shields.io/badge/site-hangfire.io-blue.svg)](http://hangfire.io)
[![NuGet](https://buildstats.info/nuget/Hangfire.RecurringJobExtensions)](https://www.nuget.org/packages/Hangfire.RecurringJobExtensions/)
[![Build status](https://ci.appveyor.com/api/projects/status/i02yxvu0mvhyv5nk?svg=true)](https://ci.appveyor.com/project/icsharp/hangfire-recurringjobextensions)
[![License MIT](https://img.shields.io/badge/license-MIT-green.svg)](http://opensource.org/licenses/MIT)

This repo is the extension for [Hangfire](https://github.com/HangfireIO/Hangfire) to build `RecurringJob` automatically.
When app start, `RecurringJob` will be added/updated automatically.
There is two ways to build `RecurringJob`.

- `RecurringJobAttribute` attribute
- Json Configuration

## Using RecurringJobAttribute

We can use the attribute `RecurringJobAttribute` to assign the interface/instance/static method.


```csharp
public class RecurringJobService
{
    [RecurringJob("*/1 * * * *")]
    [Queue("jobs")]
    public void TestJob1(PerformContext context)
    {
        context.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} TestJob1 Running ...");
    }
    [RecurringJob("*/2 * * * *", RecurringJobId = "TestJob2")]
    [Queue("jobs")]
    public void TestJob2(PerformContext context)
    {
        context.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} TestJob2 Running ...");
    }
    [RecurringJob("*/2 * * * *", "China Standard Time", "jobs")]
    public void TestJob3(PerformContext context)
    {
        context.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} TestJob3 Running ...");
    }
    [RecurringJob("*/5 * * * *", "jobs")]
    public void InstanceTestJob(PerformContext context)
    {
        context.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} InstanceTestJob Running ...");
    }

    [RecurringJob("*/6 * * * *", "UTC", "jobs")]
    public static void StaticTestJob(PerformContext context)
    {
        context.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} StaticTestJob Running ...");
    }
}
```

## Json Configuration

It is similar to [quartz.net](http://www.quartz-scheduler.net/), We also define the unified interface `IRecurringJob`.
Recurring jobs must impl the specified interface like this.

```csharp
[AutomaticRetry(Attempts = 0)]
[DisableConcurrentExecution(90)]
public class LongRunningJob : IRecurringJob
{
    public void Execute(PerformContext context)
    {
        context.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} LongRunningJob Running ...");

        var runningTimes = context.GetJobData<int>("RunningTimes");

        context.WriteLine($"get job data parameter-> RunningTimes: {runningTimes}");

        var progressBar = context.WriteProgressBar();

        foreach (var i in Enumerable.Range(1, runningTimes).ToList().WithProgress(progressBar))
        {
            Thread.Sleep(1000);
        }
    }
}
```

Now we need to provider the json config file to assign the implemented recurring job, the json configuration samples as below.

```json
[{
    "job-name": "My Job1",
    "job-type": "Hangfire.Samples.MyJob1, Hangfire.Samples",
    "cron-expression": "*/1 * * * *",
    "timezone": "China Standard Time",
    "queue": "jobs"
},
{
    "job-name": "My Job2",
    "job-type": "Hangfire.Samples.MyJob2, Hangfire.Samples",
    "cron-expression": "*/5 * * * *",
    "job-data": {
        "IntVal": 1,
        "StringVal": "abcdef",
        "BooleanVal": true,
        "SimpleObject": {
            "Name": "Foo",
            "Age": 100
        }
    }
},
{
    "job-name": "Long Running Job",
    "job-type": "Hangfire.Samples.LongRunningJob, Hangfire.Samples",
    "cron-expression": "*/2 * * * *",
    "job-data": {
        "RunningTimes": 300
    }
}]
```

The json token description to the configuration is here.

JSON Token | Description
---|---
**job-name** | *[required]* The job name to `RecurringJob`.
**job-type** | *[required]* The job type while impl the interface `IRecurringJob`.
**cron-expression** | *[required]* Cron expressions.
timezone | *[optional]* Default value is `TimeZoneInfo.Utc`.
queue | *[optional]* The specified queue name , default value is `default`.
job-data | *[optional]* Similar to the [quartz.net](http://www.quartz-scheduler.net/) `JobDataMap`, it is can be deserialized to the type `Dictionary<string,object>`.
enable | *[optional]* Whether the `RecurringJob` can be added/updated, default value is true, if false `RecurringJob` will be deleted automatically.

*To the json token `job-data`, we can use extension method to get/set data with specified key from `PerformContext` when recurring job running.*

```csharp
var intVal = context.GetJobData<int>("IntVal");

context.SetJobData("IntVal", ++intVal);
```

## Building RecurringJob

Finally, we can use extension method `UseRecurringJob` to build `RecurringJob`. In .NET Core's Startup.cs.

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHangfire(x =>
    {
        x.UseSqlServerStorage(_config.GetConnectionString("Hangfire"));

        x.UseConsole();

        //using json config file to build RecurringJob automatically.
        x.UseRecurringJob("recurringjob.json");
        //using RecurringJobAttribute to build RecurringJob automatically.
        x.UseRecurringJob(typeof(RecurringJobService));

        x.UseDefaultActivator();
    });
}
```

*For the json configuration file, we can monitor the file change and reload `RecurringJob` dynamically by passing the parameter `reloadOnChange = true`.*