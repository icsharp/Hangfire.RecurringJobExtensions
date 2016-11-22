using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hangfire.RecurringJobExtensions
{
	/// <summary>
	/// Build <see cref="RecurringJob"/> automatically, <see cref="IRecurringJobBuilder"/> interface.
	/// </summary>
	public class RecurringJobBuilder : IRecurringJobBuilder
	{
		private IRecurringJobRegistry _registry;

		/// <summary>
		/// Initializes a new instance of the <see cref="RecurringJobBuilder"/>	with <see cref="IRecurringJobRegistry"/>.
		/// </summary>
		/// <param name="registry"><see cref="IRecurringJobRegistry"/> interface.</param>
		public RecurringJobBuilder(IRecurringJobRegistry registry)
		{
			_registry = registry;
		}

		/// <summary>
		/// Create <see cref="RecurringJob"/> with the provider for specified interface or class.
		/// </summary>
		/// <param name="typesProvider">Specified interface or class</param>
		public void Build(Func<IEnumerable<Type>> typesProvider)
		{
			if (typesProvider == null) throw new ArgumentNullException(nameof(typesProvider));

			foreach (var type in typesProvider())
			{
#if NET45
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
#else
				foreach (var method in type.GetTypeInfo().DeclaredMethods)
#endif
				{
					if (!method.IsDefined(typeof(RecurringJobAttribute), false)) continue;

					var attribute = method.GetCustomAttribute<RecurringJobAttribute>(false);

					if (attribute == null || !attribute.Enabled) continue;

					if (string.IsNullOrWhiteSpace(attribute.RecurringJobId))
						_registry.Register(method, attribute.Cron, attribute.TimeZone, attribute.Queue);
					else
						_registry.Register(attribute.RecurringJobId, method, attribute.Cron, attribute.TimeZone, attribute.Queue);
				}
			}
		}
		/// <summary>
		/// Create <see cref="RecurringJob"/> with the provider for specified list <see cref="RecurringJobInfo"/>.
		/// </summary>
		/// <param name="recurringJobInfoProvider">The provider to get <see cref="RecurringJobInfo"/> list.</param>
		public void Build(Func<IEnumerable<RecurringJobInfo>> recurringJobInfoProvider)
		{
			if (recurringJobInfoProvider == null) throw new ArgumentNullException(nameof(recurringJobInfoProvider));

			foreach (RecurringJobInfo recurringJobInfo in recurringJobInfoProvider())
				_registry.Register(recurringJobInfo);
		}
	}
}
