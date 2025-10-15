using System;
using System.Threading.Tasks;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AccessManagerWeb.Core.Interfaces;
using System.DirectoryServices.AccountManagement;

namespace AccessManagerWeb.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IActiveDirectoryService _adService;

        public AuthController(IActiveDirectoryService adService)
        {
            _adService = adService;
        }

        [HttpGet("userinfo")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Пользователь не аутентифицирован");
            }

            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Не удалось получить имя пользователя");
            }

            // Убираем домен из имени пользователя, если он есть
            if (username.Contains("\\"))
            {
                username = username.Split('\\')[1];
            }
            var userProfile = await _adService.GetUserProfileAsync(username);
            
            if (userProfile == null)
            {
                return NotFound("Профиль пользователя не найден в AD");
            }

            return Ok(new
            {
                userProfile.Username,
                userProfile.DisplayName,
                userProfile.Email,
                userProfile.Department,
                Groups = await _adService.GetUserGroupsAsync(username)
            });
        }
    }
}