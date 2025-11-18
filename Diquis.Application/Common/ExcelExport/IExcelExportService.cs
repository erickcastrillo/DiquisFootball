using Diquis.Application.Common.Marker;

namespace Diquis.Application.Common.ExcelExport
{
    /// <summary>
    /// Service interface for exporting data to Excel format.
    /// </summary>
    public interface IExcelExportService : IScopedService
    {
        /// <summary>
        /// Exports a collection of data to an Excel file.
        /// </summary>
        /// <typeparam name="T">The type of the data items.</typeparam>
        /// <param name="data">The data to export.</param>
        /// <param name="columnMapping">Optional mapping of property names to column headers.</param>
        /// <param name="sheetName">The name of the Excel worksheet. Defaults to "Sheet1".</param>
        /// <returns>A byte array representing the generated Excel file.</returns>
        byte[] ExportToExcel<T>(IEnumerable<T> data, Dictionary<string, string>? columnMapping = null, string sheetName = "Sheet1");
    }
}
