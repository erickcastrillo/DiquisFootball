using Diquis.Application.Common.Marker;

namespace Diquis.Application.Common.PdfExport
{
    /// <summary>
    /// Defines the contract for a PDF export service.
    /// </summary>
    public interface IPdfExportService : IScopedService
    {
        /// <summary>
        /// Exports the given data to a PDF format.
        /// </summary>
        /// <typeparam name="T">The type of the data to export.</typeparam>
        /// <param name="data">The data object to be exported.</param>
        /// <returns>A task that represents the asynchronous export operation. The task result contains a byte array representing the generated PDF file.</returns>
        Task<byte[]> Export<T>(T data);
    }
}
