using Diquis.Application.Common.Marker;
using Microsoft.AspNetCore.Http;

namespace Diquis.Application.Common.Images
{
    /// <summary>
    /// Request object for uploading an image.
    /// </summary>
    public class ImageUploadRequest : IDto
    {
        /// <summary>
        /// The image file to upload.
        /// </summary>
        public required IFormFile ImageFile { get; set; } = null!;

        /// <summary>
        /// Indicates whether to delete the current image.
        /// </summary>
        public bool DeleteCurrentImage { get; set; }
    }
}
