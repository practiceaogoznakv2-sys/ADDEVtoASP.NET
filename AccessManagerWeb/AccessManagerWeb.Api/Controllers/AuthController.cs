using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AccessManagerWeb.Core.Interfaces;

namespace AccessManagerWeb.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IActiveDirectoryService _adService;
        private readonly IConfiguration _configuration;

        public AuthController(IActiveDirectoryService adService, IConfiguration configuration)
        {
            _adService = adService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var isValid = await _adService.ValidateUserCredentialsAsync(request.Username, request.Password);
            if (!isValid)
                return Unauthorized();

            var userProfile = await _adService.GetUserProfileAsync(request.Username);
            if (userProfile == null)
                return NotFound();

            var userGroups = await _adService.GetUserGroupsAsync(request.Username);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userProfile.Username),
                new Claim(ClaimTypes.Email, userProfile.Email),
                new Claim("DisplayName", userProfile.DisplayName ?? string.Empty),
                new Claim("Department", userProfile.Department ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: credentials
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                userProfile
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}