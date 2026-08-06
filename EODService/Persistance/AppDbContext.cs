using EODService.DTOs.EOD;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.Persistance
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<EodData> EodDaily { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite primary key for EodData
            modelBuilder.Entity<EodData>()
                .HasKey(e => new { e.Symbol, e.Date });

            // Configure decimal precision to resolve Oracle warnings
            modelBuilder.Entity<EodData>().Property(e => e.Open).HasPrecision(18, 4);
            modelBuilder.Entity<EodData>().Property(e => e.High).HasPrecision(18, 4);
            modelBuilder.Entity<EodData>().Property(e => e.Low).HasPrecision(18, 4);
            modelBuilder.Entity<EodData>().Property(e => e.Close).HasPrecision(18, 4);
            modelBuilder.Entity<EodData>().Property(e => e.AdjustedClose).HasPrecision(18, 4);
        }
    }
}
