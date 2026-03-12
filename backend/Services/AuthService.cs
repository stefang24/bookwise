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

            User user = new()
            {
                Email = request.Email,
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = role,
                ProfileImagePath = ProfileService.DefaultImagePath
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
