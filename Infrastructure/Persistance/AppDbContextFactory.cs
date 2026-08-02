using Microsoft.EntityFrameworkCore;
using Oracle.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistance
{
    public class AppDbContextFactory
    {
        public static AppDbContext Create(string ConnectionSettings)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseOracle(ConnectionSettings)
                .Options;

            return new AppDbContext(options);
        }
    }
}
