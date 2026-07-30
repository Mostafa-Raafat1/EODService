using System;
using System.Collections.Generic;
using System.Text;

namespace EODService.Services
{
    public interface IExport
    {
        public void Export(List<DTOs.EOD.EodData> eodDataList, string filePath);
    }
}
