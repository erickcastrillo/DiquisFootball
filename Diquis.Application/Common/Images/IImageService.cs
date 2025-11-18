using Diquis.Application.Common.Marker;
using Microsoft.AspNetCore.Http;

namespace Diquis.Application.Common.Images
{
    /// <summary>
    /// Service interface for image operations.
    /// </summary>
    public interface IImageService : IScopedService
    {
        /// <summary>
        /// Adds an image with the specified dimensions.
        /// </summary>
        /// <param name="file">The image file to add.</param>
        /// <param name="height">The desired height of the image.</param>
        /// <param name="width">The desired width of the image.</param>
        /// <returns>The URL of the added image.</returns>
        Task<string> AddImage(IFormFile file, int height, int width);

        /// <summary>
        /// Deletes an image by its URL.
        /// </summary>
        /// <param name="url">The URL of the image to delete.</param>
        /// <returns>The URL of the deleted image.</returns>
        Task<string> DeleteImage(string url);
    }
}
