using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Diquis.Application.Common.Wrapper;
using Diquis.Domain.Entities.Common;
using Diquis.Infrastructure.Auth.JWT.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Diquis.Infrastructure.Auth.JWT
{
    /// <summary>
    /// Provides functionality for generating and refreshing JWT tokens for authenticated users.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly JWTSettings _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenService"/> class.
        /// </summary>
        /// <param name="JWTSettings">The JWT settings options.</param>
        /// <param name="userManager">The user manager for <see cref="ApplicationUser"/>.</param>
        /// <param name="signInManager">The sign-in manager for <see cref="ApplicationUser"/>.</param>
        public TokenService(
            IOptions<JWTSettings> JWTSettings,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            _jwtSettings = JWTSettings.Value;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token and refresh token if successful.
        /// </summary>
        /// <param name="request">The token request containing user credentials.</param>
        /// <returns>
        /// A <see cref="Response{TokenResponse}"/> containing the JWT token, refresh token, and expiry information,
        /// or a failure response if authentication fails.
        /// </returns>
        public async Task<Response<TokenResponse>> GetTokenAsync(TokenRequest request)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return Response<TokenResponse>.Fail("Invalid user");

            if (user.IsActive == false)
                return Response<TokenResponse>.Fail("User account deactivated");

            SignInResult result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Response<TokenResponse>.Fail("Unauthorized");

            if (!user.EmailConfirmed)
                return Response<TokenResponse>.Fail("Unauthorized");

            // create refresh token
            string refreshToken = GenerateRefreshToken();
            DateTime refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiryTime;
            _ = await _userManager.UpdateAsync(user);

            JwtSecurityToken jwtSecurityToken = await GenerateJWTToken(user);
            TokenResponse response = new()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = refreshTokenExpiryTime
            };
            return Response<TokenResponse>.Success(response);
        }

        /// <summary>
        /// Refreshes a JWT token using a valid refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token to validate and use for generating a new JWT token.</param>
        /// <returns>
        /// A <see cref="Response{TokenResponse}"/> containing the new JWT token and refresh token information,
        /// or a failure response if the refresh token is invalid or expired.
        /// </returns>
        public async Task<Response<TokenResponse>> RefreshTokenAsync(string refreshToken)
        {
            ApplicationUser user = _userManager.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);
            if (user == null)
            {
                return Response<TokenResponse>.Fail("Invalid token");
            }

            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return Response<TokenResponse>.Fail("Refresh token expired");
            }

            // create token response
            JwtSecurityToken jwtSecurityToken = await GenerateJWTToken(user);
            TokenResponse response = new()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
            };
            return Response<TokenResponse>.Success(response);
        }

        /// <summary>
        /// Generates a JWT token for the specified user, including claims for roles, user ID, and tenant ID.
        /// </summary>
        /// <param name="user">The user for whom to generate the JWT token.</param>
        /// <returns>A <see cref="JwtSecurityToken"/> containing the user's claims.</returns>
        private async Task<JwtSecurityToken> GenerateJWTToken(ApplicationUser user)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);

            List<Claim> roleClaims = new();
            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            IEnumerable<Claim> claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                new Claim("tenant",user.TenantId)
            }
            .Union(roleClaims);

            SymmetricSecurityKey symmetricSecurityKey = new(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            SigningCredentials signingCredentials = new(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            // create JWT token with claims (roles, userid, tenantid) that expires after time set in appsettings.json
            JwtSecurityToken jwtSecurityToken = new(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AuthTokenDurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        /// <summary>
        /// Generates a secure random refresh token encoded as a URL-safe Base64 string.
        /// </summary>
        /// <returns>A new refresh token string.</returns>
        private string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using RandomNumberGenerator generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);
            string refreshToken = Convert.ToBase64String(randomNumber)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
            return refreshToken;
        }
    }
}


