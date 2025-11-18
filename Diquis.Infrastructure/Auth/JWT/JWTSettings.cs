namespace Diquis.Infrastructure.Auth.JWT
{
    /// <summary>
    /// Represents the settings required for JWT authentication.
    /// </summary>
    public class JWTSettings
    {
        /// <summary>
        /// Gets or sets the secret key used to sign the JWT.
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets the issuer of the JWT.
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// Gets or sets the audience for the JWT.
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// Gets or sets the duration (in minutes) for which the auth token is valid.
        /// </summary>
        public double AuthTokenDurationInMinutes { get; set; }
        /// <summary>
        /// Gets or sets the duration (in days) for which the refresh token is valid.
        /// </summary>
        public double RefreshTokenDurationInDays { get; set; }
    }
}
