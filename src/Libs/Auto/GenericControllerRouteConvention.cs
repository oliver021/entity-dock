using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Reflection;

namespace EntityDock.Lib.Auto
{

    /// <summary>
    /// Route controllers by generic type usage an attribute: <see cref="SetRouteAttibute"/>
    /// </summary>
    public class GenericControllerRouteConvention : IControllerModelConvention
    {
        /// <summary>
        /// Applied routing selector for generic controllers
        /// </summary>
        /// <param name="controller"></param>
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.IsGenericType)
            {
                var genericType = controller.ControllerType.GenericTypeArguments[0];
                var customNameAttribute = genericType.GetCustomAttribute<SetRouteAttibute>();

                if (customNameAttribute?.Route != null)
                {
                    // put
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(customNameAttribute.Route)),
                    });
                }
            }
        }
    }

    /// <summary>
    /// Genenric app route controller by application convention
    /// </summary>
    public class GenericAppRouteConvention : IApplicationModelConvention
    {
        /// <summary>
        /// Applied routing selector for generic controllers
        /// </summary>
        /// <param name="controller"></param>
        public void Apply(ApplicationModel app)
        {
            foreach (ControllerModel controller in app.Controllers)
            {
                // put a route attribute on controller
                PutRoute(controller);
            }
        }

        /// <summary>
        /// Proccess controller
        /// </summary>
        /// <param name="controller"></param>
        private static void PutRoute(ControllerModel controller)
        {
            if (controller.ControllerType.IsGenericType)
            {
                var genericType = controller.ControllerType.GenericTypeArguments[0];
                var customNameAttribute = genericType.GetCustomAttribute<SetRouteAttibute>();

                if (customNameAttribute?.Route != null)
                {
                    // put
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(customNameAttribute.Route)),
                    });
                }
            }
        }
    }

    /// <summary>
    /// Simple attribute for entity
    /// </summary>
    public sealed class SetRouteAttibute : Attribute
    {
        /// <summary>
        /// Simple mark an entity with routing specification.
        /// </summary>
        /// <param name="route"></param>
        public SetRouteAttibute(string route)
        {
            if (route is null)
            {
                throw new ArgumentNullException(nameof(route));
            }
            Route = route;
        }

        /// <summary>
        /// The target route to locate the entity.
        /// </summary>
        public string Route { get; }
    }
}
