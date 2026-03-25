using backend.DTOs;
using backend.Helpers;
using backend.Models;
using backend.Repositories.Interfaces;
using backend.Services.Interfaces;

namespace backend.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _environment;

        public ProfileService(IUserRepository userRepository, IWebHostEnvironment environment)
        {
            _userRepository = userRepository;
            _environment = environment;
        }

        public async Task<ResultResponse<ProfileResponse>> GetProfileAsync(int userId)
        {
            User? user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return ResultResponse<ProfileResponse>.Fail("User not found.");

            ProfileResponse profile = MapToProfileResponse(user);
            return ResultResponse<ProfileResponse>.Ok(profile);
        }

        public async Task<ResultResponse<ProfileResponse>> UpdateProfileAsync(int userId, UpdateProfileRequest request)
        {
            User? user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return ResultResponse<ProfileResponse>.Fail("User not found.");

            user.Bio = request.Bio;
            user.PhoneNumber = request.PhoneNumber;

            if (user.Role == UserRole.Provider)
            {
                user.CompanyName = request.CompanyName;
                user.PrimaryCategory = request.PrimaryCategory;
                user.CompanyDescription = request.CompanyDescription;
                user.City = request.City;
                user.Address = request.Address;
                user.Website = request.Website;

                if (!string.IsNullOrWhiteSpace(request.CompanyName))
                {
                    user.FirstName = request.CompanyName.Trim();
                    user.LastName = string.Empty;
                }
            }
            else
            {
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
            }

            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                if (string.IsNullOrEmpty(request.CurrentPassword))
                    return ResultResponse<ProfileResponse>.Fail("Current password is required.");

                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                    return ResultResponse<ProfileResponse>.Fail("Current password is incorrect.");

                if (request.NewPassword.Length < 6)
                    return ResultResponse<ProfileResponse>.Fail("New password must be at least 6 characters.");

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            }

            await _userRepository.UpdateAsync(user);

            ProfileResponse profile = MapToProfileResponse(user);
            return ResultResponse<ProfileResponse>.Ok(profile);
        }

        public async Task<ResultResponse<ProfileResponse>> UploadProfileImageAsync(int userId, IFormFile file)
        {
            User? user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return ResultResponse<ProfileResponse>.Fail("User not found.");

            if (file.Length == 0)
                return ResultResponse<ProfileResponse>.Fail("File is empty.");

            if (file.Length > ConfigProvider.MaxImageFileSizeBytes)
                return ResultResponse<ProfileResponse>.Fail("File size must be less than 5MB.");

            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!ConfigProvider.AllowedImageExtensions.Contains(extension))
                return ResultResponse<ProfileResponse>.Fail($"Only image files are allowed ({ConfigProvider.AllowedImageExtensionsText}).");

            if (!file.ContentType.StartsWith("image/"))
                return ResultResponse<ProfileResponse>.Fail("Only image files are allowed.");

            string uploadsFolder = Path.Combine(_environment.WebRootPath, ConfigProvider.ProfileImagesFolderRelativePath);
            Directory.CreateDirectory(uploadsFolder);

            if (!string.IsNullOrEmpty(user.ProfileImagePath) && user.ProfileImagePath != ConfigProvider.DefaultProfileImagePath)
            {
                string oldFilePath = Path.Combine(_environment.WebRootPath, user.ProfileImagePath.TrimStart('/'));
                if (File.Exists(oldFilePath))
                    File.Delete(oldFilePath);
            }

            string fileName = $"{ConfigProvider.ProfileImageFilePrefix}_{userId}_{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (FileStream stream = new(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.ProfileImagePath = $"/{ConfigProvider.ProfileImagesFolderRelativePath.Replace(Path.DirectorySeparatorChar, '/')}/{fileName}";
            await _userRepository.UpdateAsync(user);

            ProfileResponse profile = MapToProfileResponse(user);
            return ResultResponse<ProfileResponse>.Ok(profile);
        }

        private static ProfileResponse MapToProfileResponse(User user)
        {
            return new ProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                ProfileImagePath = user.ProfileImagePath,
                Bio = user.Bio,
                PhoneNumber = user.PhoneNumber,
                CompanyName = user.CompanyName,
                PrimaryCategory = user.PrimaryCategory,
                CompanyDescription = user.CompanyDescription,
                City = user.City,
                Address = user.Address,
                Website = user.Website,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
