using Diquis.Application.Common.Marker;

namespace Diquis.Infrastructure.Auth.JWT.DTOs
{
    /// <summary>
    /// Represents a response containing JWT tokens and their expiry information.
    /// </summary>
    public class TokenResponse : IDto
    {
        /// <summary>
        /// Gets or sets the JWT token.
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        public string RefreshToken { get; set; }
        /// <summary>
        /// Gets or sets the expiry time of the refresh token.
        /// </summary>
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
