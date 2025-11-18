using Diquis.Application.Common.Identity;
using Diquis.Application.Common.Identity.DTOs;
using Diquis.Application.Common.Images;
using Diquis.Application.Common.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diquis.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase // identity API controller
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService; // inject identity service
        }

        // USER MANAGEMENT (admin-level permissions)
        #region [user management]
        [Authorize(Roles = "root, admin")]
        [HttpGet("users")] // get user list
        public async Task<IActionResult> GetUsersAsync()
        {
            Response<IEnumerable<UserDto>> result = await _identityService.GetUsersAsync();
            return Ok(result);
        }

        [Authorize(Roles = "root, admin")]
        [HttpGet("users/{id}")] // get user details
        public async Task<IActionResult> GetUserDetailsAsync(Guid id)
        {
            Response<UserDto> result = await _identityService.GetUserDetailsAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "root, admin")]
        [HttpPost("users")] // register new user 
        public async Task<IActionResult> RegisterUserAsync(RegisterUserRequest request)
        {
            Response<UserDto> result = await _identityService.RegisterUserAsync(request);
            return Ok(result);
        }

        [Authorize(Roles = "root, admin")]
        [HttpPut("users/{id}")] // update user 
        public async Task<IActionResult> UpdateUserAsync(UpdateUserRequest request, Guid id)
        {
            Response<UserDto> result = await _identityService.UpdateUserAsync(request, id);
            return Ok(result);
        }

        [Authorize(Roles = "root,admin")]
        [HttpDelete("users/{id}")] // delete user (soft-delete)
        public async Task<IActionResult> DeleteUserAsync(Guid id)
        {
            Response<Guid> userId = await _identityService.DeleteUserAsync(id);
            return Ok(userId);
        }

        #endregion [user management]

        // PROFILE (basic-level permissions)
        #region [profile]
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpGet("profile")] // get profile 
        public async Task<IActionResult> GetProfileDetailsAsync()
        {
            Response<UserDto> result = await _identityService.GetProfileDetailsAsync();
            return Ok(result);
        }

        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpPut("profile")] // update profile 
        public async Task<IActionResult> UpdateProfileAsync(UpdateProfileRequest request)
        {
            Response<UserDto> result = await _identityService.UpdateProfileAsync(request);
            return Ok(result);
        }

        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpPut("profile-image")] // update profile image
        public async Task<IActionResult> UpdateProfileImageAsync([FromForm] ImageUploadRequest request)
        {
            Response<string> result = await _identityService.ChangeProfileImageAsync(request);
            return Ok(result);
        }


        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpPut("change-password")] // update password 
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest request)
        {
            Response<string> result = await _identityService.ChangePasswordAsync(request);
            return Ok(result);
        }
        #endregion [profile]

        // FORGOT/RESET PASSWORD (anonymous permissions -- tenant ID from header or subdomain)
        #region [forgot/reset password]
        [AllowAnonymous]
        [HttpPost("forgot-password")] // forgot password 
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            string origin = GenerateOrigin();
            string route = "ResetPassword";
            Response<string> result = await _identityService.ForgotPasswordAsync(request, origin, route); // origin and route used to construct reset link in email message
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("reset-password")] // reset password 
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            Response<string> result = await _identityService.ResetPasswordAsync(request);
            return Ok(result);
        }

        private string GenerateOrigin() // helper method to return origin URL
        {
            string baseUrl = $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
            string origin = string.IsNullOrEmpty(Request.Headers["origin"].ToString()) ? baseUrl : Request.Headers["origin"].ToString();
            return origin;
        }
        #endregion [forgot/reset password]
    }
}