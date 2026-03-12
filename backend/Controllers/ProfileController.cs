using System.Security.Claims;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            ResultResponse<ProfileResponse> response = await _profileService.GetProfileAsync(id);
            return Ok(response);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ResultResponse<ProfileResponse> response = await _profileService.UpdateProfileAsync(userId, request);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ResultResponse<ProfileResponse> response = await _profileService.UploadProfileImageAsync(userId, file);
            return Ok(response);
        }
    }
}
