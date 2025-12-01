using Diquis.Application.Common.Identity;
using Diquis.Application.Common.Identity.DTOs;
using Diquis.Application.Common.Images;
using Diquis.Application.Common.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diquis.WebApi.Controllers
{
    /// <summary>
    /// Controller for managing user identity, authentication, and profile operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityController"/> class.
        /// </summary>
        /// <param name="identityService">The identity service to use for user operations.</param>
        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        // USER MANAGEMENT (admin-level permissions)
        #region [user management]
        /// <summary>
        /// Gets the list of all users. Requires admin or root role.
        /// </summary>
        /// <remarks>
        /// Retrieves all users in the system.<br/>
        /// <b>Sample request:</b> GET /api/Identity/users<br/>
        /// <b>Authorization:</b> Requires roles: root, admin.
        /// </remarks>
        /// <response code="200">Returns the list of users.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsersAsync()
        {
            Response<IEnumerable<UserDto>> result = await _identityService.GetUsersAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gets the details of a specific user by ID. Requires admin or root role.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <remarks>
        /// Retrieves details for a specific user.<br/>
        /// <b>Sample request:</b> GET /api/Identity/users/{id}<br/>
        /// <b>Authorization:</b> Requires roles: root, admin.
        /// </remarks>
        /// <response code="200">Returns the user details.</response>
        /// <response code="400">If the user ID is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin")]
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserDetailsAsync(Guid id)
        {
            Response<UserDto> result = await _identityService.GetUserDetailsAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Registers a new user. Requires admin or root role.
        /// </summary>
        /// <param name="request">The registration request data.</param>
        /// <remarks>
        /// Registers a new user in the system.<br/>
        /// <b>Sample request:</b> <br/>
        /// POST /api/Identity/users<br/>
        /// <pre>{
        ///   "email": "user@example.com",
        ///   "password": "P@ssw0rd!",
        ///   "firstName": "John",
        ///   "lastName": "Doe",
        ///   "roles": ["basic"]
        /// }</pre>
        /// <b>Authorization:</b> Requires roles: root, admin.
        /// </remarks>
        /// <response code="200">Returns the registered user details.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="409">If a user with the same email already exists.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin")]
        [HttpPost("users")]
        public async Task<IActionResult> RegisterUserAsync(RegisterUserRequest request)
        {
            Response<UserDto> result = await _identityService.RegisterUserAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing user. Requires admin or root role.
        /// </summary>
        /// <param name="request">The update request data.</param>
        /// <param name="id">The user ID.</param>
        /// <remarks>
        /// Updates the details of an existing user.<br/>
        /// <b>Sample request:</b> <br/>
        /// PUT /api/Identity/users/{id}<br/>
        /// <pre>{
        ///   "email": "user@example.com",
        ///   "firstName": "John",
        ///   "lastName": "Doe",
        ///   "roles": ["admin"]
        /// }</pre>
        /// <b>Authorization:</b> Requires roles: root, admin.
        /// </remarks>
        /// <response code="200">Returns the updated user details.</response>
        /// <response code="400">If the request data or user ID is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="409">If a user with the same email already exists.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin")]
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUserAsync(UpdateUserRequest request, Guid id)
        {
            Response<UserDto> result = await _identityService.UpdateUserAsync(request, id);
            return Ok(result);
        }

        /// <summary>
        /// Deletes (soft-deletes) a user by ID. Requires admin or root role.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <remarks>
        /// Soft-deletes a user from the system.<br/>
        /// <b>Sample request:</b> DELETE /api/Identity/users/{id}<br/>
        /// <b>Authorization:</b> Requires roles: root, admin.
        /// </remarks>
        /// <response code="200">Returns the ID of the deleted user.</response>
        /// <response code="400">If the user ID is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root,admin")]
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUserAsync(Guid id)
        {
            Response<Guid> userId = await _identityService.DeleteUserAsync(id);
            return Ok(userId);
        }

        #endregion [user management]

        // PROFILE (basic-level permissions)
        #region [profile]
        /// <summary>
        /// Gets the profile details of the current user.
        /// </summary>
        /// <remarks>
        /// Retrieves the profile details of the currently authenticated user.<br/>
        /// <b>Sample request:</b> GET /api/Identity/profile<br/>
        /// <b>Authorization:</b> Requires roles: root, admin, editor, basic.
        /// </remarks>
        /// <response code="200">Returns the profile details.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfileDetailsAsync()
        {
            Response<UserDto> result = await _identityService.GetProfileDetailsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Updates the profile of the current user.
        /// </summary>
        /// <param name="request">The profile update request data.</param>
        /// <remarks>
        /// Updates the profile of the currently authenticated user.<br/>
        /// <b>Sample request:</b> <br/>
        /// PUT /api/Identity/profile<br/>
        /// <pre>{
        ///   "firstName": "John",
        ///   "lastName": "Doe"
        /// }</pre>
        /// <b>Authorization:</b> Requires roles: root, admin, editor, basic.
        /// </remarks>
        /// <response code="200">Returns the updated profile details.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfileAsync(UpdateProfileRequest request)
        {
            Response<UserDto> result = await _identityService.UpdateProfileAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Updates the profile image of the current user.
        /// </summary>
        /// <param name="request">The image upload request.</param>
        /// <remarks>
        /// Updates the profile image for the currently authenticated user.<br/>
        /// <b>Sample request:</b> <br/>
        /// PUT /api/Identity/profile-image<br/>
        /// <pre>--form 'file=@profile.jpg'</pre>
        /// <b>Authorization:</b> Requires roles: root, admin, editor, basic.
        /// </remarks>
        /// <response code="200">Returns the result of the image update.</response>
        /// <response code="400">If the image upload is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpPut("profile-image")]
        public async Task<IActionResult> UpdateProfileImageAsync([FromForm] ImageUploadRequest request)
        {
            Response<string> result = await _identityService.ChangeProfileImageAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Changes the password of the current user.
        /// </summary>
        /// <param name="request">The change password request data.</param>
        /// <remarks>
        /// Changes the password for the currently authenticated user.<br/>
        /// <b>Sample request:</b> <br/>
        /// PUT /api/Identity/change-password<br/>
        /// <pre>{
        ///   "currentPassword": "OldP@ssw0rd!",
        ///   "newPassword": "NewP@ssw0rd!"
        /// }</pre>
        /// <b>Authorization:</b> Requires roles: root, admin, editor, basic.
        /// </remarks>
        /// <response code="200">Returns the result of the password change.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="403">If the user does not have permission.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = "root, admin, editor, basic")]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest request)
        {
            Response<string> result = await _identityService.ChangePasswordAsync(request);
            return Ok(result);
        }
        #endregion [profile]

        // FORGOT/RESET PASSWORD (anonymous permissions -- tenant ID from header or subdomain)
        #region [forgot/reset password]
        /// <summary>
        /// Initiates the forgot password process and sends a reset link to the user's email.
        /// </summary>
        /// <param name="request">The forgot password request data.</param>
        /// <remarks>
        /// Initiates the forgot password process and sends a reset link to the user's email.<br/>
        /// <b>Sample request:</b> <br/>
        /// POST /api/Identity/forgot-password<br/>
        /// <pre>{
        ///   "email": "user@example.com"
        /// }</pre>
        /// <b>Authorization:</b> Anonymous (tenant ID required in header or subdomain).
        /// </remarks>
        /// <response code="200">Returns the result of the forgot password operation.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            string origin = GenerateOrigin();
            string route = "ResetPassword";
            Response<string> result = await _identityService.ForgotPasswordAsync(request, origin, route);
            return Ok(result);
        }

        /// <summary>
        /// Resets the user's password using the provided token and new password.
        /// </summary>
        /// <param name="request">The reset password request data.</param>
        /// <remarks>
        /// Resets the user's password using the provided token and new password.<br/>
        /// <b>Sample request:</b> <br/>
        /// POST /api/Identity/reset-password<br/>
        /// <pre>{
        ///   "email": "user@example.com",
        ///   "token": "reset-token",
        ///   "newPassword": "NewP@ssw0rd!"
        /// }</pre>
        /// <b>Authorization:</b> Anonymous (tenant ID required in header or subdomain).
        /// </remarks>
        /// <response code="200">Returns the result of the password reset operation.</response>
        /// <response code="400">If the request data or token is invalid.</response>
        /// <response code="404">If the user is not found or token is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            Response<string> result = await _identityService.ResetPasswordAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Generates the origin URL for password reset links.
        /// </summary>
        /// <returns>The origin URL as a string.</returns>
        private string GenerateOrigin()
        {
            string baseUrl = $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
            string origin = string.IsNullOrEmpty(Request.Headers["origin"].ToString()) ? baseUrl : Request.Headers["origin"].ToString();
            return origin;
        }
        #endregion [forgot/reset password]
    }
}
