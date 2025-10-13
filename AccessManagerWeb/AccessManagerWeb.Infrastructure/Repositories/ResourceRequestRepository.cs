using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AccessManagerWeb.Core.Models;
using AccessManagerWeb.Core.Interfaces;
using AccessManagerWeb.Infrastructure.Data;

namespace AccessManagerWeb.Infrastructure.Repositories
{
    public class ResourceRequestRepository : IResourceRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public ResourceRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ResourceRequest> GetByIdAsync(int id)
        {
            return await _context.ResourceRequests.FindAsync(id);
        }

        public async Task<IEnumerable<ResourceRequest>> GetAllAsync()
        {
            return await _context.ResourceRequests.ToListAsync();
        }

        public async Task<IEnumerable<ResourceRequest>> GetByUserAsync(string username)
        {
            return await _context.ResourceRequests
                .Where(r => r.RequestorUsername == username)
                .ToListAsync();
        }

        public async Task<IEnumerable<ResourceRequest>> GetPendingRequestsAsync()
        {
            return await _context.ResourceRequests
                .Where(r => r.Status == "Pending")
                .ToListAsync();
        }

        public async Task<ResourceRequest> CreateAsync(ResourceRequest request)
        {
            _context.ResourceRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<ResourceRequest> UpdateAsync(ResourceRequest request)
        {
            _context.Entry(request).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var request = await _context.ResourceRequests.FindAsync(id);
            if (request == null)
                return false;

            _context.ResourceRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}