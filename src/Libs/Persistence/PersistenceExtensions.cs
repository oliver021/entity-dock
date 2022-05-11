using EntityDock;
using EntityDock.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistenceLayer(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // register repository
            services.AddScoped(typeof(IRepository<,>), typeof(RepositoryFactory<,>));

            // register service
            services.AddScoped(typeof(DataService<,,,>));

            // data services alias 
            services.AddScoped(typeof(DataService<,,>));
            services.AddScoped(typeof(DataService<,>));

            return services;
        }
    }
}
