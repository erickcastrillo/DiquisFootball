using Diquis.Application.Common.Marker;

namespace Diquis.Infrastructure.Auth.JWT.DTOs
{
    /// <summary>
    /// Represents a request for a JWT token.
    /// </summary>
    public class TokenRequest : IDto
    {
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        public string Password { get; set; }
    }
}
