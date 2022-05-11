using System;
using System.Collections.Generic;
using System.Text;

namespace EntityDock.Lib.Base
{
    /// <summary>
    /// Set a class as attribute
    /// </summary>
    public class EntityAttribute : Attribute
    {
        /// <summary>
        /// Put basic parameter the an entity
        /// </summary>
        /// <param name="name"></param>
        /// <param name="usage"></param>
        public EntityAttribute(string name, EntityUsage usage = EntityUsage.FullyUsage)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            Name = name;
            Usage = usage;
        }

        public string Name { get; }
        public EntityUsage Usage { get; }
    }

    public enum EntityUsage
    {
        Readonly,
        Record,
        FullyUsage
    }
}
