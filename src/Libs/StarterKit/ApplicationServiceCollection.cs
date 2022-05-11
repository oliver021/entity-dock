using System;
using EntityDock.Lib.StarterKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationServiceCollection
    {
        /// <summary>
        /// Add new database from preset of providers.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="dev"></param>
        /// <returns></returns>
        public static IServiceCollection AddLPDatabase<TContext>(this IServiceCollection services,
                                                       IConfiguration configuration,
                                                       bool dev = false)
        where TContext : DbContext
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var pooling = configuration["Database:ContextPoolingEnable"] == "true";
            
            // determine if the pooling context should be use
            if(pooling)
                services.AddDbContextPool<TContext>(GetConfigurationDbProvider(configuration, dev));
            else
                services.AddDbContext<TContext>(GetConfigurationDbProvider(configuration, dev));
            
            if (dev)
            {
                // for errors in development
                services.AddDatabaseDeveloperPageExceptionFilter();
            }

            return services;

            // configure the db provider with several preset providers avalibles.
            // through configuration you can set provider and other parameters.
            static Action<DbContextOptionsBuilder> GetConfigurationDbProvider(IConfiguration configuration, bool dev)
            {
                return options =>
                {
                    string mainConnection = configuration.GetConnectionString(configuration["Database:ConnectionUsage"] ?? "DefaultConnection");

                    // base on configured provider
                    switch ((DbProvider)Enum.Parse(typeof(DbProvider), configuration["Database:Provider"]))
                    {
                        case DbProvider.SqlServer:
                            options.UseSqlServer(mainConnection);
                            break;

                        case DbProvider.Postgres:
                            options.UseNpgsql(mainConnection);
                            break;

                        case DbProvider.Mysql:
                            options.UseMySql(mainConnection, ServerVersion.AutoDetect(mainConnection));
                            break;

                        case DbProvider.Sqlite:
                            options.UseSqlite(mainConnection);
                            break;
                        case DbProvider.Memory:
                            options.UseInMemoryDatabase("local");
                            break;

                        default:
                            throw new NotSupportedException();
                    }

                    // if is development mode then use debbuging for ef
                    if (dev)
                    {
                        options.EnableDetailedErrors();
                        options.EnableSensitiveDataLogging();
                    }
                };
            }
        }
    }
}
