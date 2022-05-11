using System;

namespace EntityDock.Lib.Auto
{
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
