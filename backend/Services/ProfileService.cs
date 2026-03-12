using backend.DTOs;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _environment;

        private static readonly HashSet<string> AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
        private const long MaxFileSize = 5 * 1024 * 1024;
        public const string DefaultImagePath = "/images/profiles/default.png";

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

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Bio = request.Bio;
            user.PhoneNumber = request.PhoneNumber;

            if (user.Role == UserRole.Provider)
            {
                user.CompanyName = request.CompanyName;
                user.CompanyDescription = request.CompanyDescription;
                user.Address = request.Address;
                user.Website = request.Website;
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

            if (file.Length > MaxFileSize)
                return ResultResponse<ProfileResponse>.Fail("File size must be less than 5MB.");

            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
                return ResultResponse<ProfileResponse>.Fail("Only image files are allowed (.jpg, .jpeg, .png, .gif, .webp).");

            if (!file.ContentType.StartsWith("image/"))
                return ResultResponse<ProfileResponse>.Fail("Only image files are allowed.");

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "profiles");
            Directory.CreateDirectory(uploadsFolder);

            if (!string.IsNullOrEmpty(user.ProfileImagePath) && user.ProfileImagePath != DefaultImagePath)
            {
                string oldFilePath = Path.Combine(_environment.WebRootPath, user.ProfileImagePath.TrimStart('/'));
                if (File.Exists(oldFilePath))
                    File.Delete(oldFilePath);
            }

            string fileName = $"{userId}_{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (FileStream stream = new(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.ProfileImagePath = $"/images/profiles/{fileName}";
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
                CompanyDescription = user.CompanyDescription,
                Address = user.Address,
                Website = user.Website,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
