using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AccessManagerWeb.Core.Models;
using AccessManagerWeb.Core.Interfaces;
using System.Security.Principal;
using System.Security.Claims;
using System.Linq;

namespace AccessManagerWeb.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ResourceRequestsController : ControllerBase
    {
        private readonly IResourceRequestRepository _repository;
        private readonly IEmailService _emailService;
        private readonly IActiveDirectoryService _adService;

        public ResourceRequestsController(
            IResourceRequestRepository repository, 
            IEmailService emailService,
            IActiveDirectoryService adService)
        {
            _repository = repository;
            _emailService = emailService;
            _adService = adService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceRequest>>> GetAll()
        {
            var windowsIdentity = User.Identity as WindowsIdentity;
            if (windowsIdentity == null)
                return Unauthorized("Не удалось получить Windows Identity");

            var userGroups = await _adService.GetUserGroupsAsync(windowsIdentity.Name);
            bool isAdmin = userGroups.Any(g => g.ToLower() == "accessmanageradmins");

            if (!isAdmin)
                return Forbid();

            var requests = await _repository.GetAllAsync();
            return Ok(requests);
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<ResourceRequest>>> GetMyRequests()
        {
            var windowsIdentity = User.Identity as WindowsIdentity;
            if (windowsIdentity == null)
                return Unauthorized("Не удалось получить Windows Identity");

            var requests = await _repository.GetByUserAsync(windowsIdentity.Name);
            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceRequest>> GetById(int id)
        {
            var windowsIdentity = User.Identity as WindowsIdentity;
            if (windowsIdentity == null)
                return Unauthorized("Не удалось получить Windows Identity");

            var request = await _repository.GetByIdAsync(id);
            if (request == null)
                return NotFound();

            var userGroups = await _adService.GetUserGroupsAsync(windowsIdentity.Name);
            bool isAdmin = userGroups.Any(g => g.ToLower() == "accessmanageradmins");

            if (!isAdmin && request.RequestorUsername != windowsIdentity.Name)
                return Forbid();

            return Ok(request);
        }

        [HttpPost]
        public async Task<ActionResult<ResourceRequest>> Create([FromBody] ResourceRequest request)
        {
            var windowsIdentity = User.Identity as WindowsIdentity;
            if (windowsIdentity == null)
                return Unauthorized("Не удалось получить Windows Identity");

            var userProfile = await _adService.GetUserProfileAsync(windowsIdentity.Name);
            if (userProfile == null)
                return NotFound("Профиль пользователя не найден");

            request.RequestorUsername = windowsIdentity.Name;
            request.Status = "Pending";
            
            var createdRequest = await _repository.CreateAsync(request);
            await _emailService.SendAccessRequestNotificationAsync(createdRequest);

            return CreatedAtAction(nameof(GetById), new { id = createdRequest.Id }, createdRequest);
        }

        [Authorize(Roles = "Approver")]
        [HttpPut("{id}/approve")]
        public async Task<ActionResult<ResourceRequest>> Approve(int id)
        {
            var request = await _repository.GetByIdAsync(id);
            if (request == null)
                return NotFound();

            request.Status = "Approved";
            request.ApprovedBy = User.FindFirst(ClaimTypes.Name)?.Value;
            
            var updatedRequest = await _repository.UpdateAsync(request);
            await _emailService.SendAccessGrantedNotificationAsync(updatedRequest);

            return Ok(updatedRequest);
        }

        [Authorize(Roles = "Approver")]
        [HttpPut("{id}/deny")]
        public async Task<ActionResult<ResourceRequest>> Deny(int id)
        {
            var request = await _repository.GetByIdAsync(id);
            if (request == null)
                return NotFound();

            request.Status = "Denied";
            request.ApprovedBy = User.FindFirst(ClaimTypes.Name)?.Value;
            
            var updatedRequest = await _repository.UpdateAsync(request);
            await _emailService.SendAccessDeniedNotificationAsync(updatedRequest);

            return Ok(updatedRequest);
        }

        [Authorize(Roles = "Approver")]
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<ResourceRequest>>> GetPendingRequests()
        {
            var requests = await _repository.GetPendingRequestsAsync();
            return Ok(requests);
        }
    }
}