using Diquis.Application.Common.Marker;

namespace Diquis.Application.Common.FileStorage
{
    /// <summary>
    /// Defines the contract for a file storage service.
    /// </summary>
    // NOTE: if using file storage --> register as scoped service like this:
    // public interface IFileStorageService : IScopedService
    public interface IFileStorageService
    {
        /// <summary>
        /// Uploads a file to the storage for a specific tenant.
        /// </summary>
        /// <param name="tenantId">The identifier of the tenant.</param>
        /// <param name="fileName">The name of the file to upload.</param>
        /// <param name="content">The stream containing the file content.</param>
        /// <param name="contentType">The MIME type of the file.</param>
        /// <returns>A task that represents the asynchronous upload operation.</returns>
        Task UploadFileAsync(string tenantId, string fileName, Stream content, string contentType);

        /// <summary>
        /// Downloads a file from the storage for a specific tenant.
        /// </summary>
        /// <param name="tenantId">The identifier of the tenant.</param>
        /// <param name="fileName">The name of the file to download.</param>
        /// <returns>A task that represents the asynchronous download operation. The task result contains the file's content as a stream.</returns>
        Task<Stream> DownloadFileAsync(string tenantId, string fileName);

        /// <summary>
        /// Lists all files in the storage for a specific tenant.
        /// </summary>
        /// <param name="tenantId">The identifier of the tenant.</param>
        /// <returns>A task that represents the asynchronous list operation. The task result contains an enumerable of file names.</returns>
        Task<IEnumerable<string>> ListFilesAsync(string tenantId);

        /// <summary>
        /// Deletes a file from the storage for a specific tenant.
        /// </summary>
        /// <param name="tenantId">The identifier of the tenant.</param>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        Task DeleteFileAsync(string tenantId, string fileName);

        /// <summary>
        /// Downloads all files for a specific tenant as a single ZIP archive.
        /// </summary>
        /// <param name="tenantId">The identifier of the tenant.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the ZIP archive's content as a stream.</returns>
        Task<Stream> DownloadAllAsZipAsync(string tenantId);
    }
}
