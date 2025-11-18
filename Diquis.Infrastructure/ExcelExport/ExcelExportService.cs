using System.Reflection;
using Diquis.Application.Common.ExcelExport;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Diquis.Infrastructure.ExcelExport
{
    /// <summary>
    /// Provides functionality to export data collections to Excel files using EPPlus.
    /// </summary>
    public class ExcelExportService : IExcelExportService
    {
        /// <summary>
        /// Exports a collection of data to an Excel file.
        /// </summary>
        /// <typeparam name="T">The type of the data items.</typeparam>
        /// <param name="data">The data to export.</param>
        /// <param name="columnMapping">Optional mapping of property names to column headers.</param>
        /// <param name="sheetName">The name of the Excel worksheet. Defaults to "Sheet1".</param>
        /// <returns>A byte array representing the generated Excel file.</returns>
        public byte[] ExportToExcel<T>(IEnumerable<T> data, Dictionary<string, string>? columnMapping = null, string sheetName = "Sheet1")
        {
            ExcelPackage.License.SetNonCommercialOrganization("Diquis");
            using ExcelPackage package = new();
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Use either property names or columnMapping values for headers
            Dictionary<string, string> headers = columnMapping ?? properties.ToDictionary(p => p.Name, p => p.Name);

            int colIndex = 1;
            foreach (KeyValuePair<string, string> header in headers)
            {
                worksheet.Cells[1, colIndex].Value = header.Value;
                worksheet.Cells[1, colIndex].Style.Font.Bold = true;
                worksheet.Cells[1, colIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, colIndex].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                colIndex++;
            }

            int rowIndex = 2;
            foreach (T item in data)
            {
                colIndex = 1;
                foreach (KeyValuePair<string, string> header in headers)
                {
                    var value = GetNestedPropertyValue(item, header.Key);
                    worksheet.Cells[rowIndex, colIndex].Value = value;

                    // Format DateTime columns
                    if (value != null && IsDateTimeType(value.GetType()))
                    {
                        if (IsDateOnlyType(value.GetType()))
                        {
                            worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "mm/dd/yyyy";
                        }
                        else
                        {
                            worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss";
                        }
                    }
                    colIndex++;
                }
                rowIndex++;
            }

            // Apply column formatting for all DateTime columns
            colIndex = 1;
            foreach (KeyValuePair<string, string> header in headers)
            {
                var propertyType = GetNestedPropertyType(typeof(T), header.Key);
                if (propertyType != null && IsDateTimeType(propertyType))
                {
                    // Format the entire column for DateTime types
                    if (IsDateOnlyType(propertyType))
                    {
                        worksheet.Column(colIndex).Style.Numberformat.Format = "mm/dd/yyyy";
                    }
                    else
                    {
                        worksheet.Column(colIndex).Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss";
                    }
                }
                colIndex++;
            }

            // AutoFit columns first
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Set max width and wrap text for all columns
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                var column = worksheet.Column(col);

                // Set max width to 200 pixels (approximately 29 character width)
                if (column.Width > 40)
                {
                    column.Width = 40;
                }

                // Enable text wrapping for the entire column
                column.Style.WrapText = true;
            }

            return package.GetAsByteArray();
        }

        /// <summary>
        /// Retrieves the value of a (possibly nested) property from an object using a dot-separated property path.
        /// </summary>
        /// <param name="obj">The object to retrieve the property value from.</param>
        /// <param name="propertyPath">The dot-separated property path (e.g., "Address.City").</param>
        /// <returns>The value of the nested property, or null if not found.</returns>
        private static object? GetNestedPropertyValue(object obj, string propertyPath)
        {
            if (obj == null) return null;

            string[] propertyNames = propertyPath.Split('.');
            object currentObj = obj;

            foreach (string propertyName in propertyNames)
            {
                if (currentObj == null) return null;

                PropertyInfo property = currentObj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (property == null) return null;

                currentObj = property.GetValue(currentObj);
            }

            return currentObj;
        }

        /// <summary>
        /// Retrieves the type of a (possibly nested) property from a type using a dot-separated property path.
        /// </summary>
        /// <param name="type">The type to start from.</param>
        /// <param name="propertyPath">The dot-separated property path (e.g., "Address.City").</param>
        /// <returns>The type of the nested property, or null if not found.</returns>
        private static Type? GetNestedPropertyType(Type type, string propertyPath)
        {
            string[] propertyNames = propertyPath.Split('.');
            Type currentType = type;

            foreach (string propertyName in propertyNames)
            {
                PropertyInfo property = currentType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (property == null) return null;

                currentType = property.PropertyType;
            }

            return currentType;
        }

        /// <summary>
        /// Determines whether the specified type is a DateTime, DateTimeOffset, or DateOnly (including nullable types).
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is a DateTime, DateTimeOffset, or DateOnly; otherwise, false.</returns>
        private static bool IsDateTimeType(Type type)
        {
            // Handle nullable types as well
            Type nullableType = Nullable.GetUnderlyingType(type);
            return type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(DateOnly) ||
                   nullableType == typeof(DateTime) ||
                   nullableType == typeof(DateTimeOffset) ||
                   nullableType == typeof(DateOnly);
        }

        /// <summary>
        /// Determines whether the specified type is a DateOnly (including nullable types).
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is a DateOnly; otherwise, false.</returns>
        private static bool IsDateOnlyType(Type type)
        {
            Type nullableType = Nullable.GetUnderlyingType(type);
            return type == typeof(DateOnly) || nullableType == typeof(DateOnly);
        }
    }
}
