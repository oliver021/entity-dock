using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityDock.Lib.Auto
{
    /// <summary>
    /// Options for <see cref="AutoDbContext"/>
    /// </summary>
    public class AutoDbContextOptions : DbContextOptions<AutoDbContext>
    {
        public AutoDbContextOptions(Type[] models)
        {
            Models = models ?? throw new ArgumentNullException(nameof(models));
        }

        public Type[] Models { get; }
    }

    public class AutoDbContextOptionsBuilder : DbContextOptionsBuilder<AutoDbContext>
    {

    }
}
