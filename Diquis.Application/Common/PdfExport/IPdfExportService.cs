using Diquis.Application.Common.Marker;

namespace Diquis.Application.Common.PdfExport
{
    public interface IPdfExportService : IScopedService
    {
        Task<byte[]> Export<T>(T data);
    }
}
