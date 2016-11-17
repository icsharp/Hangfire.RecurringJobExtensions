# Hangfire.RecurringJobExtensions

[![Official Site](https://img.shields.io/badge/site-hangfire.io-blue.svg)](http://hangfire.io)
[![NuGet](https://img.shields.io/nuget/v/Hangfire.RecurringJobExtensions.svg)](https://www.nuget.org/packages/Hangfire.RecurringJobExtensions/)
[![Build status](https://ci.appveyor.com/api/projects/status/i02yxvu0mvhyv5nk?svg=true)](https://ci.appveyor.com/project/icsharp/hangfire-recurringjobextensions)
[![License MIT](https://img.shields.io/badge/license-MIT-green.svg)](http://opensource.org/licenses/MIT)

This repo is the extension for [Hangfire](https://github.com/HangfireIO/Hangfire) to build `RecurringJob` automatically. We can use the attribute `RecurringJobAttribute` to assign the interface/instance/static method.
When app start, `RecurringJob` will be added/updated automatically.

```csharp
public class RecurringJobService
{
    [RecurringJob("*/1 * * * *")]
    [DisplayName("InstanceTestJob")]
    [Queue("jobs")]
    public void InstanceTestJob(PerformContext context)
    {
        context.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} InstanceTestJob Running ...");
    }

    [RecurringJob("*/5 * * * *")]
    [DisplayName("JobStaticTest")]
    [Queue("jobs")]
    public static void StaticTestJob(PerformContext context)
    {
        context.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} StaticTestJob Running ...");
    }
}
```

More details [here](https://github.com/icsharp/Hangfire.Topshelf).
