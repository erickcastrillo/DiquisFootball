using Diquis.Application.Common.Marker;
using Diquis.Application.Common.Wrapper;
using Diquis.Infrastructure.Auth.JWT.DTOs;

namespace Diquis.Infrastructure.Auth.JWT
{
    /// <summary>
    /// Defines methods for generating and refreshing JWT tokens.
    /// </summary>
    public interface ITokenService : ITransientService
    {
        /// <summary>
        /// Generates a new JWT token based on the provided authentication request.
        /// </summary>
        /// <param name="request">The token request containing user credentials.</param>
        /// <returns>
        /// A <see cref="Response{TokenResponse}"/> containing the generated token and related information.
        /// </returns>
        Task<Response<TokenResponse>> GetTokenAsync(TokenRequest request);

        /// <summary>
        /// Refreshes an existing JWT token using the provided refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token to use for obtaining a new JWT token.</param>
        /// <returns>
        /// A <see cref="Response{TokenResponse}"/> containing the refreshed token and related information.
        /// </returns>
        Task<Response<TokenResponse>> RefreshTokenAsync(string refreshToken);
    }
}
