using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Hangfire.RecurringJobExtensions.Configuration;
using Hangfire.Common;

namespace Hangfire.RecurringJobExtensions.Tests.Configuration
{
	public class RecurringJobJsonOptionsTest
	{
		[Fact]
		public void SerializeRecurringJobJsonOptionsNormally()
		{
			var list = new List<RecurringJobJsonOptions>
			{
			  new RecurringJobJsonOptions { JobName="My Job1", JobType=typeof(MyJob1), Cron="*/1 * * * *", Queue="jobs" },
			  new RecurringJobJsonOptions { JobName="My Job12", JobType=typeof(MyJob2), Cron="*/2 * * * *", Queue="jobs", Enable=false, TimeZone=TimeZoneInfo.Utc }
			};

			var json = JobHelper.ToJson(list);
			Assert.NotNull(json);

			var o = JobHelper.FromJson<List<RecurringJobJsonOptions>>(json);

			Assert.Equal(2, o.Count);
		}
		[Fact]
		public void SerializeRecurringJobJsonOptionsContainsExtendedDataProperty()
		{
			var list = new List<RecurringJobJsonOptions>
			{
			  new RecurringJobJsonOptions
			  {
				  JobName = "My Job1",
				  JobType = typeof(MyJob1),
				  Cron = "*/1 * * * *",
				  Queue = "jobs" ,
				  JobData = new Dictionary<string, object>
				  {
					["IntVal"] = 1,
					["StringVal"] = "abcdef",
					["DateVal"] = DateTime.Now,
					["SimpleObject"] = new { Name = "Foo",Age = 100 }
				  }
			  }
			};

			var json = JobHelper.ToJson(list);
			Assert.NotNull(json);

			var o = JobHelper.FromJson<List<RecurringJobJsonOptions>>(json);

			Assert.Equal(1, o.Count);
		}
		[Theory]
		[InlineData("job1.json")]
		[InlineData("job2.json")]
		public void DeserializeRecurringJobJsonOptionsFromJsonFile(string jsonFile)
		{
			var dir = Directory.GetCurrentDirectory();
			var jsonPath = Path.Combine(dir, jsonFile);
			Assert.True(File.Exists(jsonPath));

			var jsonData = File.ReadAllText(jsonPath);

			var o = JobHelper.FromJson<List<RecurringJobJsonOptions>>(jsonData);

			Assert.Equal(2, o.Count);
		}
	}
}
