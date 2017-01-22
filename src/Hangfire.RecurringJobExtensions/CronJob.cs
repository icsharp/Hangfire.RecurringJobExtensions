using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hangfire.RecurringJobExtensions.Configuration;

namespace Hangfire.RecurringJobExtensions
{
	/// <summary>
	/// 
	/// </summary>
	public class CronJob
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="types"></param>
		public static void AddOrUpdate(params Type[] types)
		{
			AddOrUpdate(() => types);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="typesProvider"></param>
		public static void AddOrUpdate(Func<IEnumerable<Type>> typesProvider)
		{
			if (typesProvider == null) throw new ArgumentNullException(nameof(typesProvider));

			IRecurringJobBuilder builder = new RecurringJobBuilder();

			builder.Build(typesProvider);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jsonFiles"></param>
		/// <param name="reloadOnChange"></param>
		public static void AddOrUpdate(string[] jsonFiles, bool reloadOnChange = true)
		{
			if (jsonFiles == null) throw new ArgumentNullException(nameof(jsonFiles));

			foreach (var jsonFile in jsonFiles)
				AddOrUpdate(jsonFile, reloadOnChange);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="jsonFile"></param>
		/// <param name="reloadOnChange"></param>
		public static void AddOrUpdate(string jsonFile, bool reloadOnChange = true)
		{
			if (jsonFile == null) throw new ArgumentNullException(nameof(jsonFile));

			var configFile = File.Exists(jsonFile) ? jsonFile :
				Path.Combine(
#if NET45
				AppDomain.CurrentDomain.BaseDirectory,
#else
				AppContext.BaseDirectory,
#endif
				jsonFile);


			if (!File.Exists(configFile)) throw new FileNotFoundException($"The json file {configFile} does not exist.");

			IConfigurationProvider provider = new JsonConfigurationProvider(configFile, reloadOnChange);

			AddOrUpdate(provider);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="provider"></param>
		public static void AddOrUpdate(IConfigurationProvider provider)
		{
			if (provider == null) throw new ArgumentNullException(nameof(provider));

			IRecurringJobBuilder builder = new RecurringJobBuilder();

			AddOrUpdate(provider.Load());
		}

		public static void AddOrUpdate(IEnumerable<RecurringJobInfo> recurringJobInfos)
		{
			if (recurringJobInfos == null) throw new ArgumentNullException(nameof(recurringJobInfos));

			IRecurringJobBuilder builder = new RecurringJobBuilder();

			builder.Build(() => recurringJobInfos);
		}

		public static void AddOrUpdate(params RecurringJobInfo[] recurringJobInfos)
		{
			if (recurringJobInfos == null) throw new ArgumentNullException(nameof(recurringJobInfos));

			AddOrUpdate(recurringJobInfos.AsEnumerable());
		}
	}
}
