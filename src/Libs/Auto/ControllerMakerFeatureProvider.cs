using System;
using System.Linq;
using System.Reflection;
using EntityDock.Lib.Base;
using System.Collections.Generic;
using EntityDock.Lib.Auto.Controllers;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;


namespace EntityDock.Lib.Auto
{
    /// <summary>
    /// Create controller by passed types as controller
    /// </summary>
    public class ControllerMakerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        /// <summary>
        /// Create feature from an assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static ControllerMakerFeatureProvider FromAssembly(Assembly assembly)
        {
            // filter and take an array of the candidates
            return new ControllerMakerFeatureProvider(types: assembly.GetExportedTypes()
               .Where(t => t.IsDefined(typeof(SetRouteAttibute)))
               .ToArray()
            );
        }

        /// <summary>
        /// Require entry types for mapping
        /// </summary>
        /// <param name="types"></param>
        public ControllerMakerFeatureProvider(Type[] types, AutoApiOption options = default)
        {
            if (options is null)
            {
                options = new AutoApiOption();
            }

            TargetTypes = types ?? throw new ArgumentNullException(nameof(types));
            Options = options;
        }

        /// <summary>
        /// All passed types
        /// </summary>
        public Type[] TargetTypes { get; }
        public AutoApiOption Options { get; }

        /// <summary>
        /// Populate all controller created from passed types
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="feature"></param>
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            // for passed routes
            foreach (UnitRoute route in GetRoutes())
            {
                // push new controller from route
                feature.Controllers.Add(item: GetCandidateController(route)
                    .MakeGenericType(route.Model, HelpersExtensions.FindKeyType(route.Model))
                    .GetTypeInfo()
                );
            }
        }

        /// <summary>
        /// Get candidate controller for a unit route
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Type GetCandidateController(UnitRoute item)
        => item.ModelType switch
        {
            ModelType.FullyFeatures when(Options.ApiUsageService) => typeof(FullyFeatureController<,>),
            ModelType.Record when(Options.ApiUsageService) => typeof(RecordController<,>),
            ModelType.FullyFeatures => typeof(RepoFullyFeatureController<,>),
            ModelType.Record => typeof(RepoRecordController<,>),
            _ => null
        };

        /// <summary>
        /// Take routes from passed types
        /// </summary>
        /// <returns></returns>
        private IEnumerable<UnitRoute> GetRoutes()
        {
            return TargetTypes.Select(target =>
            {
                var attr = target.GetCustomAttribute<EntityAttribute>();

                // if has attribute
                if (attr != null)
                {
                    // make an route from attributes specifications
                    return new UnitRoute
                    {
                        Model = target,
                        ModelType = attr.Usage switch
                        {
                            EntityUsage.Readonly => ModelType.Readonly,
                            EntityUsage.Record => ModelType.Record,
                            EntityUsage.FullyUsage => ModelType.FullyFeatures,
                            _ => throw new InvalidOperationException()
                        }
                    };
                }
                else
                {
                    // from static definition
                    return new UnitRoute
                    {
                        Model = target,
                        ModelType = ModelType.FullyFeatures
                    };
                }
            });
        }
    }
}
