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

        [HttpGet("validate")]
        public async Task<IActionResult> ValidateUser()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Пользователь не аутентифицирован");
            }

            var windowsIdentity = User.Identity as WindowsIdentity;
            if (windowsIdentity == null)
            {
                return Unauthorized("Не удалось получить Windows Identity");
            }

            var username = windowsIdentity.Name;
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