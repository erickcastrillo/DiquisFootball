using AutoMapper;
using Diquis.Application.Common;
using Diquis.Application.Common.Identity;
using Diquis.Application.Common.Identity.DTOs;
using Diquis.Application.Common.Images;
using Diquis.Application.Common.Mailer;
using Diquis.Application.Common.Wrapper;
using Diquis.Application.Utility;
using Diquis.Domain.Entities.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Diquis.Infrastructure.Identity
{
    /// <summary>
    /// Provides identity-related services such as user management, profile management, password operations, and preferences.
    /// </summary>
    public class IdentityService : IIdentityService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentTenantUserService _currentTenantUserService;
        private readonly IMailService _mailService;
        private readonly IImageService _imageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityService"/> class.
        /// </summary>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="userManager">The ASP.NET Core Identity user manager.</param>
        /// <param name="currentTenantUserService">The current tenant and user context service.</param>
        /// <param name="mailService">The mail service for sending emails.</param>
        /// <param name="imageService">The image service for profile image operations.</param>
        public IdentityService(
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            ICurrentTenantUserService currentTenantUserService,
            IMailService mailService,
            IImageService imageService)
        {
            _userManager = userManager;
            _currentTenantUserService = currentTenantUserService;
            _mailService = mailService;
            _imageService = imageService;
            _mapper = mapper;
        }

        #region [-- USER MANAGEMENT --]

        /// <summary>
        /// Retrieves a list of users for client-side pagination.
        /// </summary>
        /// <returns>A response containing a list of <see cref="UserDto"/> objects.</returns>
        public async Task<Response<IEnumerable<UserDto>>> GetUsersAsync()
        {
            List<ApplicationUser> usersList = await _userManager.Users.OrderByDescending(x => x.CreatedOn).ToListAsync();
            foreach (ApplicationUser user in usersList)
            {
                Task<IList<string>> roles = _userManager.GetRolesAsync(user);
                string roleId = roles.Result.FirstOrDefault();
                user.RoleId = roleId;
            }

            List<UserDto> dtoList = _mapper.Map<List<UserDto>>(usersList);

            return Response<IEnumerable<UserDto>>.Success(dtoList);
        }

        /// <summary>
        /// Retrieves the details of a specific user by their identifier.
        /// </summary>
        /// <param name="id">The user's unique identifier.</param>
        /// <returns>A response containing the <see cref="UserDto"/> details.</returns>
        public async Task<Response<UserDto>> GetUserDetailsAsync(Guid id)
        {
            ApplicationUser user = await _userManager.Users.Where(x => x.Id == Convert.ToString(id)).FirstOrDefaultAsync();
            if (user == null)
            {
                return Response<UserDto>.Fail("User doesn't exist");
            }

            Task<IList<string>> roles = _userManager.GetRolesAsync(user);
            string roleId = roles.Result.FirstOrDefault();
            user.RoleId = roleId;

            UserDto responseDto = _mapper.Map(user, new UserDto());

            return Response<UserDto>.Success(responseDto);
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="request">The registration request details.</param>
        /// <returns>A response containing the registered <see cref="UserDto"/>.</returns>
        public async Task<Response<UserDto>> RegisterUserAsync(RegisterUserRequest request)
        {
            ApplicationUser userExist = await _userManager.FindByEmailAsync(request.Email);
            if (userExist != null)
            {
                return Response<UserDto>.Fail("User already exists");
            }

            ApplicationUser user = new()
            {
                UserName = request.Email + "." + _currentTenantUserService.TenantId + "." + NanoHelpers.GenerateHex(4),
                TenantId = _currentTenantUserService.TenantId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                EmailConfirmed = true,
                IsActive = true
            };

            IdentityResult result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                _ = await _userManager.AddToRoleAsync(user, request.RoleId);
                user.RoleId = request.RoleId;

                UserDto responseDto = _mapper.Map(user, new UserDto());

                return Response<UserDto>.Success(responseDto);
            }
            else
            {
                List<string> messages = new();
                foreach (IdentityError error in result.Errors)
                {
                    messages.Add(error.Description);
                }
                return Response<UserDto>.Fail(messages);
            }
        }

        /// <summary>
        /// Updates an existing user's information.
        /// </summary>
        /// <param name="request">The update request details.</param>
        /// <param name="id">The user's unique identifier.</param>
        /// <returns>A response containing the updated <see cref="UserDto"/>.</returns>
        public async Task<Response<UserDto>> UpdateUserAsync(UpdateUserRequest request, Guid id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return Response<UserDto>.Fail("Not found");
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);
            if (roles.FirstOrDefault() == "root")
            {
                return Response<UserDto>.Fail("Cannot edit root user");
            }

            if (id == Guid.Parse(_currentTenantUserService.UserId))
            {
                return Response<UserDto>.Fail("Cannot update current user");
            }

            if (request.Email != user.Email)
            {
                ApplicationUser userExist = await _userManager.FindByEmailAsync(request.Email);
                if (userExist != null)
                {
                    return Response<UserDto>.Fail("Email already in use");
                }
            }

            ApplicationUser updatedUser = _mapper.Map(request, user);
            IdentityResult result = await _userManager.UpdateAsync(updatedUser);

            if (result.Succeeded)
            {
                UserDto updatedUserDTO = _mapper.Map(updatedUser, new UserDto());

                _ = await _userManager.RemoveFromRolesAsync(user, roles.ToArray());
                _ = await _userManager.AddToRoleAsync(user, request.RoleId);

                return Response<UserDto>.Success(updatedUserDTO);
            }
            else
            {
                List<string> messages = new();
                foreach (IdentityError error in result.Errors)
                {
                    messages.Add(error.Description);
                }
                return Response<UserDto>.Fail(messages);
            }
        }

        /// <summary>
        /// Deletes a user from the system.
        /// </summary>
        /// <param name="id">The user's unique identifier.</param>
        /// <returns>A response containing the identifier of the deleted user.</returns>
        public async Task<Response<Guid>> DeleteUserAsync(Guid id)
        {
            try
            {
                ApplicationUser user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    return Response<Guid>.Fail("Not found");
                }

                IList<string> roles = await _userManager.GetRolesAsync(user);
                if (roles.FirstOrDefault() == "root")
                {
                    return Response<Guid>.Fail("Cannot delete root user");
                }
                if (id == Guid.Parse(_currentTenantUserService.UserId))
                {
                    return Response<Guid>.Fail("Cannot delete current user");
                }

                _ = await _userManager.DeleteAsync(user);

                return Response<Guid>.Success(id);
            }
            catch (Exception ex)
            {
                return Response<Guid>.Fail(ex.Message);
            }
        }
        #endregion [-- USER MANAGEMENT --]

        #region [-- PROFILE --]

        /// <summary>
        /// Retrieves the profile details of the current user.
        /// </summary>
        /// <returns>A response containing the <see cref="UserDto"/> for the current user.</returns>
        public async Task<Response<UserDto>> GetProfileDetailsAsync()
        {
            ApplicationUser currentUser = await _userManager.Users.Where(x => x.Id == _currentTenantUserService.UserId).FirstOrDefaultAsync();

            Task<IList<string>> roles = _userManager.GetRolesAsync(currentUser);
            string roleId = roles.Result.FirstOrDefault();
            currentUser.RoleId = roleId;

            UserDto dtoUser = _mapper.Map(currentUser, new UserDto());

            return Response<UserDto>.Success(dtoUser);
        }

        /// <summary>
        /// Updates the profile information of the current user.
        /// </summary>
        /// <param name="request">The profile update request details.</param>
        /// <returns>A response containing the updated <see cref="UserDto"/>.</returns>
        public async Task<Response<UserDto>> UpdateProfileAsync(UpdateProfileRequest request)
        {
            ApplicationUser userInDb = await _userManager.FindByIdAsync(_currentTenantUserService.UserId);
            if (userInDb == null)
            {
                return Response<UserDto>.Fail("User Not Found");
            }

            ApplicationUser userWithEmail = await _userManager.FindByEmailAsync(request.Email);

            if (userWithEmail == userInDb)
            {
                userWithEmail = null;
            }

            if (userWithEmail != null)
            {
                return Response<UserDto>.Fail("Email already in use");
            }

            ApplicationUser updatedAppUser = _mapper.Map(request, userInDb);

            _ = await _userManager.UpdateAsync(updatedAppUser);

            UserDto responseDto = _mapper.Map(updatedAppUser, new UserDto());
            responseDto.RoleId = _userManager.GetRolesAsync(updatedAppUser).Result.FirstOrDefault();

            return Response<UserDto>.Success(responseDto);
        }

        /// <summary>
        /// Changes the password for the current user.
        /// </summary>
        /// <param name="request">The change password request details.</param>
        /// <returns>A response indicating the result of the password change operation.</returns>
        public async Task<Response<string>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(_currentTenantUserService.UserId);
            if (user == null)
            {
                return Response<string>.Fail("Not found");
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);
            if (result.Succeeded)
            {
                return Response<string>.Success("Password updated");
            }
            else
            {
                List<string> errorList = new();
                foreach (IdentityError error in result.Errors)
                {
                    errorList.Add(error.Description);
                }
                return Response<string>.Fail(errorList);
            }
        }

        /// <summary>
        /// Changes the profile image for the current user.
        /// </summary>
        /// <param name="request">The image upload request details.</param>
        /// <returns>A response containing the new image URL or an error message.</returns>
        public async Task<Response<string>> ChangeProfileImageAsync(ImageUploadRequest request)
        {
            ApplicationUser userInDb = await _userManager.FindByIdAsync(_currentTenantUserService.UserId);
            if (userInDb == null)
            {
                return Response<string>.Fail("User Not Found");
            }

            string currentImage = userInDb.ImageUrl ?? "";

            if (request.ImageFile != null)
            {
                string imageResult = await _imageService.AddImage(request.ImageFile, 300, 300);
                userInDb.ImageUrl = imageResult;

                if (currentImage != "")
                {
                    _ = await _imageService.DeleteImage(currentImage);
                }
            }

            if (request.DeleteCurrentImage && currentImage != "")
            {
                _ = await _imageService.DeleteImage(currentImage);
                userInDb.ImageUrl = "";
            }

            _ = await _userManager.UpdateAsync(userInDb);
            return Response<string>.Success(userInDb.ImageUrl);
        }
        #endregion [-- PROFILE --]

        #region [-- FORGOT/RESET PASSWORD --]

        /// <summary>
        /// Initiates the forgot password process for a user by sending a reset token via email.
        /// </summary>
        /// <param name="request">The forgot password request details.</param>
        /// <param name="origin">The origin URL for the reset link.</param>
        /// <param name="route">The route for the reset endpoint.</param>
        /// <returns>A response indicating whether the reset email was sent.</returns>
        public async Task<Response<string>> ForgotPasswordAsync(ForgotPasswordRequest request, string origin, string route)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email.Normalize());
            if (user is null || user.IsActive == false)
            {
                return Response<string>.Fail("Not found");
            }

            string code = await _userManager.GeneratePasswordResetTokenAsync(user);

            Uri endpointUri = new(string.Concat($"{origin}/", route));

            MailRequest mailRequest = new()
            {
                To = request.Email,
                Subject = "Reset Password",
                Body = $"Your password reset token is '{code}'. You can reset your password using the {endpointUri} endpoint.",
            };

            await _mailService.SendAsync(mailRequest);

            return Response<string>.Success("Password reset has been sent to your email.");
        }

        /// <summary>
        /// Resets the password for a user using a provided token.
        /// </summary>
        /// <param name="request">The reset password request details.</param>
        /// <returns>A response indicating the result of the password reset operation.</returns>
        public async Task<Response<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || user.IsActive == false)
            {
                return Response<string>.Fail("Not found");
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (result.Succeeded)
            {
                return Response<string>.Success("Password reset");
            }
            else
            {
                return Response<string>.Fail("Password reset fail");
            }
        }
        #endregion [-- FORGOT/RESET PASSWORD --]
    }
}
