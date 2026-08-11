using EODService.DTOs.EOD;
using EODService.DTOs.Stock;
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
        public DbSet<Stock> Stock { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //For Daily Table
            modelBuilder.Entity<EodDataDaily>(entity =>
            {
                entity.ToTable("EodDaily");

                // Only one latest record per symbol
                entity.HasKey(e => e.TickerID);

                entity.Property(e => e.TickerID)
                    .HasColumnName("Ticker_ID");

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
                    e.TickerID,
                    e.Date
                });

                entity.Property(e => e.TickerID)
                    .HasColumnName("Ticker_ID");

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

            // For Stock Table
            modelBuilder.Entity<Stock>(entity =>
            {
                entity.ToTable("STOCK");


                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("SC_COMP_ID")
                    .HasPrecision(4);

                entity.Property(e => e.LongArName)
                    .HasColumnName("SCA_LONG_NAME")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.LongEngName)
                    .HasColumnName("SCE_LONG_NAME")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.ShortArName)
                    .HasColumnName("SCA_SHORT_NAME")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.ShortEngName)
                    .HasColumnName("SCE_SHORT_NAME")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.ArabicAddress)
                    .HasColumnName("SCA_ADDRESS")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.EnglishAddress)
                    .HasColumnName("SCE_ADDRESS")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.TickerID)
                    .HasColumnName("TICKER_ID")
                    .HasMaxLength(100);

                entity.Property(e => e.YahooFinanceID)
                    .HasColumnName("Y_FINANCE")
                    .HasMaxLength(100);

                entity.Property(e => e.TwelveDataID)
                    .HasColumnName("T_DATA")
                    .HasMaxLength(100);

                entity.Property(e => e.YahooFinanceExists)
                    .HasColumnName("YF_FLAG")
                    .HasConversion(
                        v => v ? "Y" : "N",
                        v => v == "Y");

                entity.Property(e => e.TwelveDataExists)
                    .HasColumnName("TD_FLAG")
                    .HasConversion(
                        v => v ? "Y" : "N",
                        v => v == "Y");
            });
        }



    }
}
