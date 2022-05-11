using EntityDock.Lib.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EntityDock.Lib.Auto
{
    public static class Extensions
    {
        /// <summary>
        /// Add auto controllers features in services collections
        /// </summary>
        /// <param name="services"></param>
        public static void AddDataControllers(this IMvcCoreBuilder services, Type[] types)
        {
            // feature to add controller frome ntity types
            services.PartManager.FeatureProviders.Add(new ControllerMakerFeatureProvider(types));

            // convention to use in routes
            services.AddMvcOptions(x => x.Conventions.Add(new GenericAppRouteConvention()));
        }

        /// <summary>
        /// Add auto controllers features in services collections
        /// </summary>
        /// <param name="services"></param>
        public static void AddFilterTriggers(this IMvcCoreBuilder services, Type[] types)
        {
            services.AddMvcOptions(opt => opt.Filters.Add<EntityEventFilter>());
        }

        /// <summary>
        /// Return true if is an entity class
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEntity(this Type type)
        {
            return type.IsDefined(typeof(EntityAttribute), false);
        }
    }
}
