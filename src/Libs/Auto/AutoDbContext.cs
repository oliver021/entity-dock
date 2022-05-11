using EntityDock.Lib.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityDock.Lib.Auto
{
    /// <summary>
    /// Auto context take entities from an assembly that contains
    /// several class/schema as entity.
    /// </summary>
    public class AutoDbContext : DbContext
    {
        private readonly Assembly _assembly;
        private Type[] _types;

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public AutoDbContext()
        {
        }

        /// <summary>
        /// Main construct
        /// </summary>
        /// <param name="options"></param>
        public AutoDbContext([NotNull] DbContextOptions<AutoDbContext> options) : base(options)
        {
            /// check if passed options is <see cref="AutoDbContextOptions"/>
            if (options is AutoDbContextOptions autoOptions)
            {
                _types = autoOptions.Models;
            }
        }

        /// <summary>
        /// Require options and assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="options"></param>
        public AutoDbContext(Assembly assembly, [NotNull] DbContextOptions<AutoDbContext> options) : base(options)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            _assembly = assembly;
        }

        /// <summary>
        /// Require options and assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="options"></param>
        public AutoDbContext(Type[] types, [NotNull] DbContextOptions<AutoDbContext> options) : base(options)
        {
            if (types is null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            _types = types;
        }

        /// <summary>
        /// This override can check if the candidates model was passed by extensions
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // if for construct not entry the models then use an extensions
            var auto = optionsBuilder.Options.FindExtension<AutoExtension>();

            // check if has extensions for auto feature
            if (auto != null)
            {
                _types = auto.Types;

                // check if additional configuration is avaliable
                if (auto.HasConfiguration)
                {
                    auto.ApplyConfiguration(optionsBuilder);
                }
            }
        }

        /// <summary>
        /// Build assembly
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // assume all entities in the setted assembly
            if (_types is null && _assembly != null)
            {
                _types = _assembly.GetExportedTypes()
                .Where(t => t.IsEntity())
                .ToArray();
            }
            else if (_types is null && _assembly == null)
            {
                // crash if not has types
                throw new InvalidOperationException();
            }

            /// create model from types
            foreach (var t in _types)
            {
                // register as entity
                var ent = builder.Entity(t);

                // set simple comment
                ent.HasComment("build from auto context..");

                /*foreach (var item in t.GetCustomAttributes<SchemaAttibute>(true))
                {
                }*/
            }
        }
    }
}