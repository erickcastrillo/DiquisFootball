using Diquis.Application.Common.Identity.DTOs;
using Diquis.Application.Common.Images;
using Diquis.Application.Common.Marker;
using Diquis.Application.Common.Wrapper;

namespace Diquis.Application.Common.Identity
{
    /// <summary>
    /// Defines the contract for identity-related service operations.
    /// </summary>
    public interface IIdentityService : IScopedService
    {
        /// <summary>
        /// Retrieves a list of all users.
        /// </summary>
        /// <returns>A <see cref="Response{T}"/> containing an enumerable of <see cref="UserDto"/>.</returns>
        Task<Response<IEnumerable<UserDto>>> GetUsersAsync();

        /// <summary>
        /// Retrieves the details of a specific user.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>A <see cref="Response{T}"/> containing the <see cref="UserDto"/> for the specified user.</returns>
        Task<Response<UserDto>> GetUserDetailsAsync(Guid id);

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">The <see cref="RegisterUserRequest"/> containing the new user's details.</param>
        /// <returns>A <see cref="Response{T}"/> containing the <see cref="UserDto"/> of the registered user.</returns>
        Task<Response<UserDto>> RegisterUserAsync(RegisterUserRequest request);

        /// <summary>
        /// Updates an existing user's details.
        /// </summary>
        /// <param name="request">The <see cref="UpdateUserRequest"/> containing the updated user details.</param>
        /// <param name="id">The unique identifier of the user to update.</param>
        /// <returns>A <see cref="Response{T}"/> containing the <see cref="UserDto"/> of the updated user.</returns>
        Task<Response<UserDto>> UpdateUserAsync(UpdateUserRequest request, Guid id);

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>A <see cref="Response{T}"/> containing the unique identifier of the deleted user.</returns>
        Task<Response<Guid>> DeleteUserAsync(Guid id);

        /// <summary>
        /// Retrieves the profile details of the current authenticated user.
        /// </summary>
        /// <returns>A <see cref="Response{T}"/> containing the <see cref="UserDto"/> of the current user's profile.</returns>
        Task<Response<UserDto>> GetProfileDetailsAsync();

        /// <summary>
        /// Updates the profile details of the current authenticated user.
        /// </summary>
        /// <param name="request">The <see cref="UpdateProfileRequest"/> containing the updated profile details.</param>
        /// <returns>A <see cref="Response{T}"/> containing the <see cref="UserDto"/> of the updated profile.</returns>
        Task<Response<UserDto>> UpdateProfileAsync(UpdateProfileRequest request);

        /// <summary>
        /// Changes the profile image of the current authenticated user.
        /// </summary>
        /// <param name="request">The <see cref="ImageUploadRequest"/> containing the new image data.</param>
        /// <returns>A <see cref="Response{T}"/> containing the URL of the changed profile image.</returns>
        Task<Response<string>> ChangeProfileImageAsync(ImageUploadRequest request);

        /// <summary>
        /// Changes the password of the current authenticated user.
        /// </summary>
        /// <param name="request">The <see cref="ChangePasswordRequest"/> containing the old and new passwords.</param>
        /// <returns>A <see cref="Response{T}"/> indicating the success or failure of the password change.</returns>
        Task<Response<string>> ChangePasswordAsync(ChangePasswordRequest request);

        /// <summary>
        /// Initiates the forgot password process for a user.
        /// </summary>
        /// <param name="request">The <see cref="ForgotPasswordRequest"/> containing the user's email.</param>
        /// <param name="origin">The origin URL for generating the reset link.</param>
        /// <param name="route">The route for the password reset page.</param>
        /// <returns>A <see cref="Response{T}"/> indicating the success or failure of the request.</returns>
        Task<Response<string>> ForgotPasswordAsync(ForgotPasswordRequest request, string origin, string route);

        /// <summary>
        /// Resets a user's password using a reset token.
        /// </summary>
        /// <param name="request">The <see cref="ResetPasswordRequest"/> containing the email, new password, and reset token.</param>
        /// <returns>A <see cref="Response{T}"/> indicating the success or failure of the password reset.</returns>
        Task<Response<string>> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
