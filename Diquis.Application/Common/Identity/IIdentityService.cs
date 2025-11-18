using Diquis.Application.Common.Identity.DTOs;
using Diquis.Application.Common.Images;
using Diquis.Application.Common.Marker;
using Diquis.Application.Common.Wrapper;

namespace Diquis.Application.Common.Identity
{
    public interface IIdentityService : IScopedService
    {
        Task<Response<IEnumerable<UserDto>>> GetUsersAsync(); // get user list (full list for client-side pagination)
        Task<Response<UserDto>> GetUserDetailsAsync(Guid id); // get user details
        Task<Response<UserDto>> RegisterUserAsync(RegisterUserRequest request); // register new user 
        Task<Response<UserDto>> UpdateUserAsync(UpdateUserRequest request, Guid id); // update user
        Task<Response<Guid>> DeleteUserAsync(Guid id); // delete user
        Task<Response<UserDto>> GetProfileDetailsAsync(); // get profile
        Task<Response<UserDto>> UpdateProfileAsync(UpdateProfileRequest request); // update profile
        Task<Response<string>> ChangeProfileImageAsync(ImageUploadRequest request); // change profile image
        Task<Response<string>> ChangePasswordAsync(ChangePasswordRequest request); // change password
        Task<Response<string>> ForgotPasswordAsync(ForgotPasswordRequest request, string origin, string route); // forgot password
        Task<Response<string>> ResetPasswordAsync(ResetPasswordRequest request); // reset password
    }
}
