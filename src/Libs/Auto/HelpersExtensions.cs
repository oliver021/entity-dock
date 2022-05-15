using EntityDock.Lib.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityDock.Lib.Auto
{
    public static class HelpersExtensions
    {
        /// <summary>
        /// Return true if is an entity class
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEntity(this Type type)
        {
            return type.IsDefined(typeof(EntityAttribute), false);
        }

        public static Type FindKeyType(Type model)
        {
            var baseClass = model.BaseType;

            // verify that base class is AggregateRoot
            if (baseClass.FullName.StartsWith("EntityDock.Entities.Base.AggregateRoot"))
            {
                return baseClass.GenericTypeArguments[0];
            }
            else
            {
                return null;
            }
        }
    }
}
