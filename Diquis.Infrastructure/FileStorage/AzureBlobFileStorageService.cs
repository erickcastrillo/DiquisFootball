using System.IO.Compression;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Diquis.Application.Common.FileStorage;

namespace Diquis.Infrastructure.FileStorage
{
    /// <summary>
    /// Provides file storage operations using Azure Blob Storage for multi-tenant scenarios.
    /// </summary>
    public class AzureBlobFileStorageService : IFileStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerPrefix = "tenant-";

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobFileStorageService"/> class.
        /// </summary>
        /// <param name="blobServiceClient">The Azure Blob service client.</param>
        public AzureBlobFileStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        /// <summary>
        /// Gets the <see cref="BlobContainerClient"/> for the specified tenant.
        /// Creates the container if it does not exist.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns>The blob container client.</returns>
        private BlobContainerClient GetContainerClient(string tenantId)
        {
            string containerName = $"{_containerPrefix}{tenantId}".ToLower();
            BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(containerName);
            container.CreateIfNotExists(PublicAccessType.None);
            return container;
        }

        /// <summary>
        /// Uploads a file to Azure Blob Storage for the specified tenant.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="fileName">The name of the file to upload.</param>
        /// <param name="content">The file content stream.</param>
        /// <param name="contentType">The MIME type of the file.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UploadFileAsync(string tenantId, string fileName, Stream content, string contentType)
        {
            BlobContainerClient container = GetContainerClient(tenantId);
            BlobClient blobClient = container.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
            await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });
        }

        /// <summary>
        /// Downloads a file from Azure Blob Storage for the specified tenant.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="fileName">The name of the file to download.</param>
        /// <returns>A stream containing the file content.</returns>
        public async Task<Stream> DownloadFileAsync(string tenantId, string fileName)
        {
            BlobContainerClient container = GetContainerClient(tenantId);
            BlobClient blobClient = container.GetBlobClient(fileName);
            Azure.Response<BlobDownloadInfo> download = await blobClient.DownloadAsync();
            return download.Value.Content;
        }

        /// <summary>
        /// Lists all file names in the Azure Blob Storage container for the specified tenant.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns>A collection of file names.</returns>
        public async Task<IEnumerable<string>> ListFilesAsync(string tenantId)
        {
            BlobContainerClient container = GetContainerClient(tenantId);
            List<string> files = [];
            await foreach (BlobItem blobItem in container.GetBlobsAsync())
            {
                files.Add(blobItem.Name);
            }
            return files;
        }

        /// <summary>
        /// Deletes a file from Azure Blob Storage for the specified tenant.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteFileAsync(string tenantId, string fileName)
        {
            BlobContainerClient container = GetContainerClient(tenantId);
            BlobClient blobClient = container.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Downloads all files for the specified tenant as a ZIP archive.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns>A stream containing the ZIP archive of all files.</returns>
        public async Task<Stream> DownloadAllAsZipAsync(string tenantId)
        {
            BlobContainerClient container = GetContainerClient(tenantId);
            MemoryStream zipStream = new MemoryStream();
            using (ZipArchive archive = new(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                await foreach (BlobItem blobItem in container.GetBlobsAsync())
                {
                    BlobClient blobClient = container.GetBlobClient(blobItem.Name);
                    ZipArchiveEntry entry = archive.CreateEntry(blobItem.Name, CompressionLevel.Fastest);
                    using Stream blobStream = await blobClient.OpenReadAsync();
                    using Stream entryStream = entry.Open();
                    await blobStream.CopyToAsync(entryStream);
                }
            }
            zipStream.Position = 0;
            return zipStream;
        }
    }
}
