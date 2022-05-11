using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Reflection;

namespace EntityDock.Lib.Auto
{
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
                    // put at the modeling
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(customNameAttribute.Route)),
                    });
                }
            }
        }
    }
}
