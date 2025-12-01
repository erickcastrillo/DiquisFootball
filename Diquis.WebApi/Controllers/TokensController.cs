using System.Threading.Tasks;
using Diquis.Application.Common.Wrapper;
using Diquis.Infrastructure.Auth.JWT;
using Diquis.Infrastructure.Auth.JWT.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diquis.WebApi.Controllers
{
    /// <summary>
    /// API controller for handling JWT token operations such as login and refresh.
    /// </summary>
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokensController"/> class.
        /// </summary>
        /// <param name="tokenService">The token service used for generating and refreshing tokens.</param>
        public TokensController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token if successful.
        /// </summary>
        /// <param name="request">The token request containing user credentials.</param>
        /// <returns>A response containing the generated JWT token and related information.</returns>
        /// <remarks>
        /// Authenticates a user and returns a JWT token if the credentials are valid.<br/>
        /// <b>Sample request:</b> <br/>
        /// POST /api/Tokens<br/>
        /// <pre>{
        ///   "email": "user@example.com",
        ///   "password": "P@ssw0rd!",
        ///   "tenantId": "tenant-guid"
        /// }</pre>
        /// <b>Authorization:</b> Anonymous (tenant ID required in header or subdomain).
        /// </remarks>
        /// <response code="200">Returns the generated JWT token and related information.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="401">If the credentials are invalid.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequest request)
        {
            Response<TokenResponse> response = await _tokenService.GetTokenAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Refreshes a JWT token using the provided refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token to use for obtaining a new JWT token.</param>
        /// <returns>A response containing the refreshed JWT token and related information.</returns>
        /// <remarks>
        /// Refreshes a JWT token using the provided refresh token.<br/>
        /// <b>Sample request:</b> <br/>
        /// GET /api/Tokens/{refreshToken}<br/>
        /// <pre>{
        ///   "refreshToken": "your-refresh-token"
        /// }</pre>
        /// <b>Authorization:</b> Anonymous (tenant ID required in header or subdomain).
        /// </remarks>
        /// <response code="200">Returns the refreshed JWT token and related information.</response>
        /// <response code="400">If the refresh token is invalid.</response>
        /// <response code="401">If the refresh token is expired or invalid.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [AllowAnonymous]
        [HttpGet("{refreshToken}")]
        public async Task<IActionResult> RefreshTokenAsync(string refreshToken)
        {
            Response<TokenResponse> response = await _tokenService.RefreshTokenAsync(refreshToken);
            return Ok(response);
        }
    }
}
