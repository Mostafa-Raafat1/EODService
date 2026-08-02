using Microsoft.EntityFrameworkCore;
using Oracle.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.Persistance
{
    public class AppDbContextFactory
    {
        public static AppDbContext Create(string ConnectionSettings)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseOracle(ConnectionSettings)
                .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Warning)
                .Options;

            return new AppDbContext(options);
        }
    }
}
