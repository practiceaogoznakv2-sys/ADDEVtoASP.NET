using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AccessManagerWeb.Core.Models;
using AccessManagerWeb.Core.Interfaces;
using System.Security.Claims;

namespace AccessManagerWeb.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ResourceRequestsController : ControllerBase
    {
        private readonly IResourceRequestRepository _repository;
        private readonly IEmailService _emailService;

        public ResourceRequestsController(IResourceRequestRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceRequest>>> GetAll()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var requests = await _repository.GetByUserAsync(username);
            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceRequest>> GetById(int id)
        {
            var request = await _repository.GetByIdAsync(id);
            if (request == null)
                return NotFound();

            return Ok(request);
        }

        [HttpPost]
        public async Task<ActionResult<ResourceRequest>> Create([FromBody] ResourceRequest request)
        {
            request.RequestorUsername = User.FindFirst(ClaimTypes.Name)?.Value;
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