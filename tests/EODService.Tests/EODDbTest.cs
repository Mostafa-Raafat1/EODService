using System;
using System.Linq;
using System.Threading.Tasks;
using EODService.Persistance;
using EODService.DTOs.EOD;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EODService.Tests
{
    public class EODDbTest
    {
        private readonly ITestOutputHelper _output;
        private const string ConnectionString = ""Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.120.143.51)(PORT=1521))(CONNECT_DATA=(SERVER=dedicated)(SERVICE_NAME=cibcorclhq)));User Id=intern;Password=intern;"";

        public EODDbTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task TestQueryOracleDatabase()
        {
            using var dbContext = AppDbContextFactory.Create(ConnectionString);
            var daily = await dbContext.EodDaily.AsNoTracking().ToListAsync();

            _output.WriteLine($""=== EOD_DAILY ({daily.Count} rows) ==="");
            foreach (var d in daily)
            {
                _output.WriteLine($""[Daily] Id={d.Id,-5} Name={d.Name,-15} Date={d.Date:yyyy-MM-dd} Open={d.Open,8:F2} High={d.High,8:F2} Low={d.Low,8:F2} Close={d.Close,8:F2} Vol={d.Volume}"");
            }

            var history = await dbContext.EodHistory.AsNoTracking().Where(h => h.Date >= DateTime.Today.AddDays(-7)).ToListAsync();
            _output.WriteLine($""=== EOD_HISTORY Last 7 Days ({history.Count} rows) ==="");
            foreach (var h in history)
            {
                _output.WriteLine($""[History] Id={h.Id,-5} Name={h.Name,-15} Date={h.Date:yyyy-MM-dd} Close={h.Close,8:F2} Vol={h.Volume}"");
            }
        }
    }
}
