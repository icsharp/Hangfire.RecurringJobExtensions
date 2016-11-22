using System;
using System.Collections.Generic;
using System.IO;
using Hangfire.Logging;
using System.Threading;

namespace Hangfire.RecurringJobExtensions.Configuration
{
	/// <summary>
	/// Represents a base class for file based <see cref="IConfigurationProvider"/>.
	/// </summary>
	public abstract class FileConfigurationProvider : IConfigurationProvider, IDisposable
	{
		private const int NumberOfRetries = 3;
		private const int DelayOnRetry = 1000;

		private static readonly ILog _logger = LogProvider.For<FileConfigurationProvider>();
		private readonly FileSystemWatcher _fileWatcher;
		private readonly object _fileWatcherLock = new object();
		private readonly IRecurringJobBuilder _builder;

		/// <summary>
		/// Initializes a new <see cref="IConfigurationProvider"/>
		/// </summary>
		/// <param name="builder">The builder for <see cref="IRecurringJobBuilder"/>.</param>
		/// <param name="configFile">The source settings file.</param>
		/// <param name="reloadOnChange">Whether the <see cref="RecurringJob"/> should be reloaded if the file changes.</param>
		public FileConfigurationProvider(IRecurringJobBuilder builder, string configFile, bool reloadOnChange = true)
		{
			_builder = builder;

			ConfigFile = configFile;

			if (!File.Exists(configFile))
				throw new FileNotFoundException($"The json file {configFile} does not exist.");

			_fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(configFile), Path.GetFileName(configFile));

			_fileWatcher.EnableRaisingEvents = reloadOnChange;
			_fileWatcher.Changed += OnChanged;
			_fileWatcher.Error += OnError;
		}

		private void OnError(object sender, ErrorEventArgs e)
		{
			_logger.InfoException($"File {ConfigFile} occurred errors.", e.GetException());
		}

		private void OnChanged(object sender, FileSystemEventArgs e)
		{
			lock (_fileWatcherLock)
			{
				_logger.Info($"File {e.Name} changed, try to reload configuration again...");

				_builder.Build(() => Load());
			}
		}

		/// <summary>
		/// <see cref="RecurringJob"/> configuraion file
		/// </summary>
		public virtual string ConfigFile { get; set; }

		/// <summary>
		/// Loads the data for this provider.
		/// </summary>
		/// <returns></returns>
		public abstract IEnumerable<RecurringJobInfo> Load();

		/// <summary>
		/// Reads from config file.
		/// </summary>
		/// <returns></returns>
		protected virtual string ReadFromFile()
		{
			if (!File.Exists(ConfigFile))
				throw new FileNotFoundException($"The json file {ConfigFile} does not exist.");

			var jsonContent = string.Empty;

			for (int i = 0; i < NumberOfRetries; ++i)
			{
				try
				{
					// Do stuff with file  
					using (var file = new FileStream(ConfigFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
					using (StreamReader reader = new StreamReader(file))
						jsonContent = reader.ReadToEnd();

					break; // When done we can break loop
				}
				catch (IOException e)
				{
					_logger.DebugException($"read file {ConfigFile} error.", e);
					// You may check error code to filter some exceptions, not every error
					// can be recovered.
					if (i == NumberOfRetries) // Last one, (re)throw exception and exit
						throw;

					Thread.Sleep(DelayOnRetry);
				}
			}
			return jsonContent;
		}

		/// <summary>
		/// Disposes the file watcher
		/// </summary>
		public virtual void Dispose()
		{
			_fileWatcher.Dispose();
		}
	}
}
