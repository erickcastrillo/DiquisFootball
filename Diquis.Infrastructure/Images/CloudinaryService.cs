    using CloudinaryDotNet; // cloudinary sdk
    using CloudinaryDotNet.Actions; // cloudinary sdk
    using Diquis.Application.Common.Images;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Cloudinary is a 3rd party platform for digital asset management.
    /// Images (and files) are stored on the platform and retrieved via public CDN.
    /// This is an implementation of their service using the CloudinaryDotNet SDK.
    /// Create a free account and change the API keys in appsettings.json.
    /// </summary>
    namespace Diquis.Infrastructure.Images
    {
        /// <summary>
        /// Service for managing images using Cloudinary.
        /// </summary>
        public class CloudinaryService : IImageService
        {
            private readonly Cloudinary _cloudinary;

            /// <summary>
            /// Initializes a new instance of the <see cref="CloudinaryService"/> class.
            /// </summary>
            /// <param name="config">The Cloudinary configuration settings.</param>
            public CloudinaryService(IOptions<CloudinarySettings> config)
            {
                Account account = new(
                    config.Value.CloudName,
                    config.Value.ApiKey,
                    config.Value.ApiSecret
                );
                _cloudinary = new Cloudinary(account);
            }

            /// <summary>
            /// Uploads an image to Cloudinary with the specified dimensions.
            /// </summary>
            /// <param name="file">The image file to upload.</param>
            /// <param name="height">The desired height of the image.</param>
            /// <param name="width">The desired width of the image.</param>
            /// <returns>The secure URL of the uploaded image, or <c>null</c> if the file is empty.</returns>
            /// <exception cref="Exception">Thrown if the upload fails.</exception>
            public async Task<string> AddImage(IFormFile file, int height, int width)
            {
                if (file.Length > 0)
                {
                    await using Stream stream = file.OpenReadStream();
                    ImageUploadParams uploadParams = new()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Height(height).Width(width).Crop("fill").Gravity("auto")
                    };

                    ImageUploadResult uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        throw new Exception(uploadResult.Error.Message);
                    }

                    return uploadResult.SecureUrl.ToString();
                }

                return null;
            }

            /// <summary>
            /// Deletes an image from Cloudinary by its URL.
            /// </summary>
            /// <param name="url">The URL of the image to delete.</param>
            /// <returns>
            /// The result string "ok" if deletion was successful; otherwise, <c>null</c>.
            /// </returns>
            public async Task<string> DeleteImage(string url)
            {
                string urlSegment = new Uri(url).Segments.Last();
                string publicId = Path.GetFileNameWithoutExtension(urlSegment);

                DeletionParams deleteParams = new(publicId);
                DeletionResult result = await _cloudinary.DestroyAsync(deleteParams);
                return result.Result == "ok" ? result.Result : null;
            }
        }
    }
