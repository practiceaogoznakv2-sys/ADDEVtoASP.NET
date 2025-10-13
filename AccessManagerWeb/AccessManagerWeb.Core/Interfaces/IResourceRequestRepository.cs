using System.Threading.Tasks;
using System.Collections.Generic;
using AccessManagerWeb.Core.Models;

namespace AccessManagerWeb.Core.Interfaces
{
    public interface IResourceRequestRepository
    {
        Task<ResourceRequest> GetByIdAsync(int id);
        Task<IEnumerable<ResourceRequest>> GetAllAsync();
        Task<IEnumerable<ResourceRequest>> GetByUserAsync(string username);
        Task<IEnumerable<ResourceRequest>> GetPendingRequestsAsync();
        Task<ResourceRequest> CreateAsync(ResourceRequest request);
        Task<ResourceRequest> UpdateAsync(ResourceRequest request);
        Task<bool> DeleteAsync(int id);
    }
}