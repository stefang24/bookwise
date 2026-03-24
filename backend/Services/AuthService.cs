using backend.DTOs;
using backend.Helpers;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResultResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            User? user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return ResultResponse<AuthResponse>.Fail("Invalid email or password.");

            if (!user.IsActive)
                return ResultResponse<AuthResponse>.Fail("Your account is deactivated. Contact administrator.");

            string token = JwtHelper.GenerateToken(user);

            AuthResponse authResponse = new()
            {
                Token = token,
                Email = user.Email,
                Username = user.Username,
                Role = user.Role.ToString()
            };

            return ResultResponse<AuthResponse>.Ok(authResponse);
        }

        public async Task<ResultResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            if (await _userRepository.EmailExistsAsync(request.Email))
                return ResultResponse<AuthResponse>.Fail("Email is already taken.");

            if (await _userRepository.UsernameExistsAsync(request.Username))
                return ResultResponse<AuthResponse>.Fail("Username is already taken.");

            UserRole role = request.Role == "Provider" ? UserRole.Provider : UserRole.User;
            string firstName = request.FirstName;
            string lastName = request.LastName;
            string? companyName = null;

            if (role == UserRole.Provider)
            {
                if (string.IsNullOrWhiteSpace(request.CompanyName))
                    return ResultResponse<AuthResponse>.Fail("Company name is required for providers.");

                companyName = request.CompanyName.Trim();
                firstName = companyName;
                lastName = string.Empty;
            }

            User user = new()
            {
                Email = request.Email,
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = firstName,
                LastName = lastName,
                Role = role,
                CompanyName = companyName,
                City = role == UserRole.Provider ? request.City?.Trim() : null,
                ProfileImagePath = ConfigProvider.DefaultProfileImagePath
            };

            await _userRepository.CreateAsync(user);

            string token = JwtHelper.GenerateToken(user);

            AuthResponse authResponse = new()
            {
                Token = token,
                Email = user.Email,
                Username = user.Username,
                Role = user.Role.ToString()
            };

            return ResultResponse<AuthResponse>.Ok(authResponse);
        }
    }
}
