using Azure.Storage.Blobs;
using Diquis.Infrastructure.FileStorage;
using Moq;

using Xunit;

namespace Diquis.Infrastructure.Tests.FileStorage
{
    public class AzureBlobFileStorageServiceTests
    {
        [Fact]
        public void Constructor_SetsBlobServiceClient()
        {
            Mock<BlobServiceClient> blobServiceClientMock = new Mock<BlobServiceClient>();
            AzureBlobFileStorageService service = new AzureBlobFileStorageService(blobServiceClientMock.Object);
            Assert.NotNull(service);
        }
    }
}
