using System.Threading.Tasks;
using Diquis.Infrastructure.Auth.JWT;
using Diquis.Infrastructure.Auth.JWT.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diquis.WebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase // tokens API controller, on login returns JWT tokens to authenticated users
    {
        private readonly ITokenService _tokenService;

        public TokensController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost] // get token (login) -- must provide tenant ID in header or subdomain
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequest request)
        {
            var response = await _tokenService.GetTokenAsync(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("{refreshToken}")] // get token (login) -- must provide tenant ID in header or subdomain
        public async Task<IActionResult> RefreshTokenAsync(string refreshToken)
        {
            var response = await _tokenService.RefreshTokenAsync(refreshToken);
            return Ok(response);
        }
    }
}
