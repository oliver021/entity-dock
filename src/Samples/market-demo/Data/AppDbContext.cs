using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MarketDemo.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<MarketAsset> Assets { get; set; }

        public AppDbContext([NotNull] DbContextOptions options) : base(options)
        {
          
        }
    }
}
