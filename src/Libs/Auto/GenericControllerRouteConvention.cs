﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Reflection;

namespace EntityDock.Lib.Auto
{

    /// <summary>
    /// Route controllers by generic type uses an attribute: <see cref="SetRouteAttibute"/>
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
}
