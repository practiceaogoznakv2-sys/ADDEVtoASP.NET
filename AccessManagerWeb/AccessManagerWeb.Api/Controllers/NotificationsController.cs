using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Principal;
using AccessManagerWeb.Core.Models;
using AccessManagerWeb.Core.Interfaces;

namespace AccessManagerWeb.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IActiveDirectoryService _adService;

        public NotificationsController(IActiveDirectoryService adService)
        {
            _adService = adService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications()
        {
            var windowsIdentity = User.Identity as WindowsIdentity;
            if (windowsIdentity == null)
                return Unauthorized("Не удалось получить Windows Identity");

            var notifications = new List<Notification>();
            // В реальном приложении здесь будет логика получения уведомлений из базы данных
            
            return Ok(notifications);
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var windowsIdentity = User.Identity as WindowsIdentity;
            if (windowsIdentity == null)
                return Unauthorized("Не удалось получить Windows Identity");

            // В реальном приложении здесь будет логика отметки уведомления как прочитанного
            
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var windowsIdentity = User.Identity as WindowsIdentity;
            if (windowsIdentity == null)
                return Unauthorized("Не удалось получить Windows Identity");

            // В реальном приложении здесь будет логика удаления уведомления
            
            return Ok();
        }
    }
}