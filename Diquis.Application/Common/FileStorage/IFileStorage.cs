using Diquis.Application.Common.Marker;

namespace Diquis.Application.Common.FileStorage
{
    // NOTE: if using file storage --> register as scoped service like this:
    // public interface IFileStorageService : IScopedService
    public interface IFileStorageService
    {
        Task UploadFileAsync(string tenantId, string fileName, Stream content, string contentType);
        Task<Stream> DownloadFileAsync(string tenantId, string fileName);
        Task<IEnumerable<string>> ListFilesAsync(string tenantId);
        Task DeleteFileAsync(string tenantId, string fileName);
        Task<Stream> DownloadAllAsZipAsync(string tenantId);
    }
}
