using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace EntityDock.Lib.Auto
{
    /// <summary>
    /// Extensions to contains candidates types
    /// </summary>
    public class AutoExtension : IDbContextOptionsExtension
    {
        /// <summary>
        ///  Auto extensions for only model
        /// </summary>
        /// <param name="types"></param>
        public AutoExtension(Type[] types)
        {
            if (types is null)
            {
                throw new ArgumentNullException(nameof(types));
            }
            Types = types;
        }

        /// <summary>
        /// Auto extensions for model and configuration
        /// </summary>
        /// <param name="types"></param>
        /// <param name="configure"></param>
        public AutoExtension(Type[] types, Action<DbContextOptionsBuilder> configure) : this(types)
        {
            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            Configure = configure;
        }

        /// <summary>
        /// Info for this extensions
        /// </summary>
        public DbContextOptionsExtensionInfo Info => new AutoExtensionInfo(this);

        /// <summary>
        /// types reference
        /// </summary>
        public Type[] Types {  get; }

        /// <summary>
        /// Configure for delegate passed
        /// </summary>
        public Action<DbContextOptionsBuilder> Configure { get; } = null;

        /// <summary>
        /// This property indicate if the extension contains a configuration delegate
        /// </summary>
        public bool HasConfiguration => Configure != null;

        /// <summary>
        /// Apply configuration if is passed
        /// </summary>
        /// <param name="services"></param>
        public void ApplyConfiguration(DbContextOptionsBuilder builder)
        {
            Configure.Invoke(builder);
        }

        /// <summary>
        /// Nothing to do
        /// </summary>
        /// <param name="services"></param>
        public void ApplyServices(IServiceCollection services)
        {
        }

        /// <summary>
        /// Nothing to do
        /// </summary>
        /// <param name="options"></param>
        public void Validate(IDbContextOptions options)
        {
        }

        /// <summary>
        /// Extension info implementation
        /// </summary>
        private sealed class AutoExtensionInfo : DbContextOptionsExtensionInfo
        {
            private readonly AutoExtension extension;

            /// <summary>
            /// Required entry extensions
            /// </summary>
            /// <param name="extension"></param>
            public AutoExtensionInfo([NotNull] AutoExtension extension) : base(extension)
            {
                if (extension is null)
                {
                    throw new ArgumentNullException(nameof(extension));
                }

                this.extension = extension;
            }

            /// <summary>
            /// Is not a databse provider 
            /// </summary>
            public override bool IsDatabaseProvider => false;

            /// <summary>
            /// Fetch a log text
            /// </summary>
            public override string LogFragment => GetLogFragment();

            /// <summary>
            /// Concatenate all entity model names
            /// </summary>
            /// <returns></returns>
            private string GetLogFragment()
            {
                return string.Join(',', extension.Types.Select(x => x.Name));
            }

            /// <summary>
            /// Not provide hash
            /// </summary>
            /// <returns></returns>
            public override long GetServiceProviderHashCode()
            {
                return 0;
            }

            /// <summary>
            /// Not populate debug info
            /// </summary>
            /// <param name="debugInfo"></param>
            public override void PopulateDebugInfo([NotNull] IDictionary<string, string> debugInfo)
            {
                // nothing do
            }
        }
    }
}