using System.Collections.Generic;
using System.Linq;
using Diquis.Infrastructure.ExcelExport;
using Xunit;

namespace Diquis.Infrastructure.Tests.ExcelExport
{
    public class ExcelExportServiceTests
    {
        private class TestData
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        [Fact]
        public void ExportToExcel_ReturnsNonEmptyByteArray()
        {
            ExcelExportService service = new ExcelExportService();
            List<TestData> data =
            [
                new TestData { Id = 1, Name = "A" },
                new TestData { Id = 2, Name = "B" }
            ];
            byte[] result = service.ExportToExcel(data);
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }
    }
}
