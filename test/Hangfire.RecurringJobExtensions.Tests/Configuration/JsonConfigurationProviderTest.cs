using System;
using System.IO;
using System.Linq;
using Hangfire.RecurringJobExtensions.Configuration;
using Newtonsoft.Json;
using Xunit;

namespace Hangfire.RecurringJobExtensions.Tests.Configuration
{
	public class JsonConfigurationProviderTest
	{
		[Fact]
		public void Ctor_ThrowsAnException_WhenFileIsNullOrNotExists()
		{
			Assert.Throws<ArgumentNullException>("fileName", () => new JsonConfigurationProvider(null));
			Assert.Throws<FileNotFoundException>(() => new JsonConfigurationProvider($"{Guid.NewGuid()}.json"));
		}

		[Fact]
		public void Load_ThrowsAnException_WhenFileContentIsInvliadJsonData()
		{
			var provider = new JsonConfigurationProvider("project.json");

			Assert.Throws<JsonSerializationException>(() => provider.Load().ToList());
		}

		[Fact]
		public void Load_WithNoJobData()
		{
			var provider = new JsonConfigurationProvider("job1.json");

			Assert.Equal(true, provider.ReloadOnChange);

			var data = provider.Load().ToList();

			Assert.True(data.Count > 0);
		}
		[Fact]
		public void Load_WithJobData()
		{
			var provider = new JsonConfigurationProvider("job2.json");

			Assert.Equal(true, provider.ReloadOnChange);

			var data = provider.Load().ToList();

			Assert.True(data.Count > 0);

			var recurringJobInfo = data.FirstOrDefault(x => x.RecurringJobId == "My Job2");

			Assert.True(recurringJobInfo.JobData != null && recurringJobInfo.JobData.Count > 0);
		}

	}
}
