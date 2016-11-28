using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Hangfire.RecurringJobExtensions.Configuration;
namespace Hangfire.RecurringJobExtensions.Tests.Configuration
{
	public class RecurringJobJsonOptionsTest
	{
		[Fact]
		public void SearializeRecurringJobJsonOptionsTest()
		{
			var list = new List<RecurringJobJsonOptions>
			{
			  new RecurringJobJsonOptions { JobName="" },
			  new RecurringJobJsonOptions { },
			};
		}

	}
}
