using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Hangfire.Console;
using Hangfire.RecurringJobExtensions;
namespace Hangfire.Samples
{
	public class Startup
	{
		private readonly IConfigurationRoot _config;

		public Startup(IHostingEnvironment env)
		{
			// Set up configuration providers.
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

			if (env.IsDevelopment())
			{
				// For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
				//builder.AddUserSecrets();
			}

			builder.AddEnvironmentVariables();

			_config = builder.Build();
		}
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddHangfire(x =>
			{
				x.UseSqlServerStorage(_config.GetConnectionString("Hangfire"));

				x.UseConsole();

				x.UseRecurringJob("recurringjob.json");

				x.UseRecurringJob(typeof(RecurringJobService));	

				x.UseDefaultActivator();
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHangfireServer(new BackgroundJobServerOptions
			{
				Queues = new[] { "default", "apis", "jobs" }
			});

			app.UseHangfireDashboard("");

		}
	}
}
