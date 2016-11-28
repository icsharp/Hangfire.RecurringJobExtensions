using System;
using System.Diagnostics;
using Hangfire.Server;

namespace Hangfire.RecurringJobExtensions.Tests
{
	public class MyJob2 : IRecurringJob
	{
		public void Execute(PerformContext context)
		{
			Debug.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} MyJob2 Running ...");
		}
	}
}
