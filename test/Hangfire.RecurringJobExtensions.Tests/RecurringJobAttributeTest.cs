using System;
using Xunit;
namespace Hangfire.RecurringJobExtensions.Tests
{
	public class RecurringJobAttributeTest
	{
		[Fact]
		public void Ctor_ThrowsAnException_WhenCronIsNullOrEmpty()
		{
			Assert.Throws<ArgumentNullException>("cron", () => new RecurringJobAttribute(null));
			Assert.Throws<ArgumentNullException>("cron", () => new RecurringJobAttribute(""));
		}
		[Fact]
		public void Ctor_ThrowsAnException_WhenQueueIsNullOrEmpty()
		{
			Assert.Throws<ArgumentNullException>("queue", () => new RecurringJobAttribute("* * * * *", null));
			Assert.Throws<ArgumentNullException>("queue", () => new RecurringJobAttribute("* * * * *", ""));
		}
		[Fact]
		public void Ctor_ThrowsAnException_WhenTimeZoneIsNullOrEmpty()
		{
			Assert.Throws<ArgumentNullException>("timeZone", () => new RecurringJobAttribute("* * * * *", null, "default"));
			Assert.Throws<ArgumentNullException>("timeZone", () => new RecurringJobAttribute("* * * * *", "", "default"));
		}
		[Fact]
		public void Ctor_Normally()
		{
			var attr = new RecurringJobAttribute("* * * * *", "UTC", "default");

			Assert.Equal("* * * * *", attr.Cron);
			Assert.Equal("UTC", attr.TimeZone);
			Assert.Equal("default", attr.Queue);
			Assert.Equal(TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById(attr.TimeZone));
		}
	}
}
