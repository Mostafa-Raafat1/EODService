using EODService.DTOs.EOD;
using EODService.DTOs.Stock;
using EODService.Persistance.Repo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
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
        public DbSet<EodStock> EodStocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //For Daily Table
            modelBuilder.Entity<EodDataDaily>(entity =>
            {
                entity.ToTable("EodDaily");

                // Only one latest record per symbol
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("STOCK_ID");

                entity.Property(e => e.Name)
                    .HasColumnName("STOCK_NAME");

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
                    e.Id,
                    e.Date
                });

                entity.Property(e => e.Id)
                    .HasColumnName("STOCK_ID");

                entity.Property(e => e.Name)
                    .HasColumnName("STOCK_NAME");

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
                entity.ToTable("EOD_STOCKS");


                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("STOCK_ID")
                    .HasPrecision(10);

                entity.Property(e => e.SC_Comp_Id)
                    .HasColumnName("SC_COMP_ID")
                    .HasPrecision(4);

                entity.Property(e => e.StockName)
                    .IsRequired()
                    .HasColumnName("STOCK_NAME")
                    .HasMaxLength(200);

                entity.Property(e => e.YahooFinanceID)
                    .HasColumnName("Y_FINANCE")
                    .HasMaxLength(100);

                entity.Property(e => e.TwelveDataID)
                    .HasColumnName("T_DATA")
                    .HasMaxLength(100);

                entity.Property(e => e.YahooFinanceExists)
                    .IsRequired()
                    .HasColumnName("YF_FLAG")
                    .HasConversion(
                        v => v ? "Y" : "N",
                        v => v == "Y");

                entity.Property(e => e.TwelveDataExists)
                    .IsRequired()
                    .HasColumnName("TD_FLAG")
                    .HasConversion(
                        v => v ? "Y" : "N",
                        v => v == "Y");

                entity.Property(e => e.StockExchange)
                    .HasColumnName("SC_EXCHANGE")
                    .HasMaxLength(50);
            });

            // For EodStock Table (EOD_STOCK)
            modelBuilder.Entity<EodStock>(entity =>
            {
                entity.ToTable("EOD_STOCK");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.StockName)
                    .HasColumnName("STOCK_NAME")
                    .HasMaxLength(250)
                    .IsRequired();

                entity.Property(e => e.InitialId)
                    .HasColumnName("SC_STOCK_ID")
                    .HasMaxLength(100);

                entity.Property(e => e.Exchange)
                    .HasColumnName("EXCHANGE")
                    .HasMaxLength(100);

                entity.Property(e => e.TdTradable)
                    .HasColumnName("TD_TRADABLE")
                    .HasConversion(
                        v => v ? "Y" : "N",
                        v => v == "Y");

                entity.Property(e => e.YfTradable)
                    .HasColumnName("YF_TRADABLE")
                    .HasConversion(
                        v => v ? "Y" : "N",
                        v => v == "Y");

                entity.Property(e => e.TdSymbol)
                    .HasColumnName("TD_SYMBOL")
                    .HasMaxLength(100);

                entity.Property(e => e.YfSymbol)
                    .HasColumnName("YF_SYMBOL")
                    .HasMaxLength(100);
            });
        }



    }
}
