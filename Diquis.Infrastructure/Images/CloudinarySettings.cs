namespace Diquis.Infrastructure.Images
{
    /// <summary>
    /// Represents the configuration settings required to connect to a Cloudinary account.
    /// </summary>
    public class CloudinarySettings
    {
        /// <summary>
        /// Gets or sets the Cloudinary cloud name.
        /// </summary>
        public string CloudName { get; set; }

        /// <summary>
        /// Gets or sets the Cloudinary API key.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the Cloudinary API secret.
        /// </summary>
        public string ApiSecret { get; set; }
    }
}
