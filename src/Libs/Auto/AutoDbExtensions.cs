using EntityDock.Lib.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using EntityDock.Lib.Auto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AutoDbExtensions
    {
        /// <summary>
        /// Add auto controllers features in services collections
        /// </summary>
        /// <param name="services"></param>
        /// <param name="types"></param>
        public static void AddDataControllers(this IMvcCoreBuilder services, Type[] types, AutoApiOption options = null)
        {
            // feature to add controller frome ntity types
            services.PartManager.FeatureProviders.Add(new ControllerMakerFeatureProvider(types, options));

            // convention to use in routes
            services.AddMvcOptions(x => x.Conventions.Add(new GenericControllerFeatureConvention()));
        }

        /// <summary>
        /// Add auto controllers features in services collections
        /// </summary>
        /// <param name="services"></param>
        /// <param name="types"></param>
        public static void AddDataControllers(this IMvcBuilder services, Type[] types, AutoApiOption options = null)
        {
            // feature to add controller frome ntity types
            services.PartManager.FeatureProviders.Add(new ControllerMakerFeatureProvider(types, options));

            // convention to use in routes
            services.AddMvcOptions(x => x.Conventions.Add(new GenericControllerFeatureConvention()));
        }

        /// <summary>
        /// Add auto controllers features in services collections
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbContext"></param>
        public static void AddDataControllers(this IMvcCoreBuilder services,
                                              Type dbContext,
                                              bool deepScan = false,
                                              AutoApiOption options = null)
        {
            if (!dbContext.IsAssignableFrom(typeof(DbContext)))
            {
                throw new Exception();
            }

            var types = dbContext.GetProperties()
                .Where(x => x.PropertyType.Name.Equals(typeof(DbSet<>).Name))
                .Select(x => x.PropertyType.GenericTypeArguments[0])
                .ToArray();

            // feature to add controller frome ntity types
            services.PartManager.FeatureProviders.Add(new ControllerMakerFeatureProvider(types, options));

            // convention to use in routes
            services.AddMvcOptions(x => x.Conventions.Add(new GenericControllerFeatureConvention()));
        }

        /// <summary>
        /// Add auto controllers features in services collections
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbContext"></param>
        public static void AddDataControllers(this IMvcBuilder services,
                                              Type dbContext,
                                              bool deepScan = false,
                                              AutoApiOption options = null)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (dbContext is null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            var types = dbContext.GetProperties()
                .Where(x => x.PropertyType.Name.Equals(typeof(DbSet<>).Name))
                .Select(x => x.PropertyType.GenericTypeArguments[0])
                .ToArray();

            // feature to add controller frome ntity types
            services.PartManager.FeatureProviders.Add(new ControllerMakerFeatureProvider(types, options));

            // convention to use in routes
            services.AddMvcOptions(x => x.Conventions.Add(new GenericControllerFeatureConvention()));
        }

        /// <summary>
        /// Add auto controllers features in services collections using a passed type argument.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="deepScan"></param>
        public static void AddDataControllers<TContext>(this IMvcCoreBuilder services, bool deepScan = false)
        {
            services.AddDataControllers(typeof(TContext), deepScan);
        }

        /// <summary>
        /// Add auto controllers features in services collections using a passed type argument.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="deepScan"></param>
        public static void AddDataControllers<TContext>(this IMvcBuilder services, bool deepScan = false)
        {
            services.AddDataControllers(typeof(TContext), deepScan);
        }

        /// <summary>
        /// Add auto controllers features in services collections
        /// </summary>
        /// <param name="services"></param>
        public static void AddFilterTriggers(this IMvcCoreBuilder services, Type[] types)
        {
            services.AddMvcOptions(opt => opt.Filters.Add<EntityEventFilter>());
        }
    }
}
