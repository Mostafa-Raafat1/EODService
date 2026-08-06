using EODService.DTOs.EOD;
using EODService.Persistance.Repo;
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
        
        public DbSet<EodDataHistory> EodHistory { get; set; }
        public DbSet<EodDataDaily> EodDaily { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Daily Table


            // =====================
            // EOD DAILY
            // =====================
            modelBuilder.Entity<EodDataDaily>(entity =>
            {
                entity.ToTable("EodDaily");

                // Only one latest record per symbol
                entity.HasKey(e => e.Symbol);

                entity.Property(e => e.Symbol)
                    .HasColumnName("SYMBOL");

                entity.Property(e => e.Date)
                    .HasColumnName("DATE");

                entity.Property(e => e.Open)
                    .HasColumnName("OPEN")
                    .HasPrecision(18, 4);

                entity.Property(e => e.High)
                    .HasColumnName("HIGH")
                    .HasPrecision(18, 4);

                entity.Property(e => e.Low)
                    .HasColumnName("LOW")
                    .HasPrecision(18, 4);

                entity.Property(e => e.Close)
                    .HasColumnName("CLOSE")
                    .HasPrecision(18, 4);

                entity.Property(e => e.AdjustedClose)
                    .HasColumnName("ADJUSTEDCLOSE")
                    .HasPrecision(18, 4);

                entity.Property(e => e.Volume)
                    .HasColumnName("VOLUME");
            });


            // For History Table
            modelBuilder.Entity<EodDataHistory>(entity =>
            {
                entity.ToTable("EodHistory");

                entity.HasKey(e => new
                {
                    e.Symbol,
                    e.Date
                });

                entity.Property(e => e.Symbol)
                    .HasColumnName("SYMBOL");

                entity.Property(e => e.Date)
                    .HasColumnName("DATE");

                entity.Property(e => e.Open)
                    .HasColumnName("OPEN")
                    .HasPrecision(18, 4);

                entity.Property(e => e.High)
                    .HasColumnName("HIGH")
                    .HasPrecision(18, 4);

                entity.Property(e => e.Low)
                    .HasColumnName("LOW")
                    .HasPrecision(18, 4);

                entity.Property(e => e.Close)
                    .HasColumnName("CLOSE")
                    .HasPrecision(18, 4);

                entity.Property(e => e.AdjustedClose)
                    .HasColumnName("ADJUSTEDCLOSE")
                    .HasPrecision(18, 4);

                entity.Property(e => e.Volume)
                    .HasColumnName("VOLUME");
            });
        }


    
    }
}
