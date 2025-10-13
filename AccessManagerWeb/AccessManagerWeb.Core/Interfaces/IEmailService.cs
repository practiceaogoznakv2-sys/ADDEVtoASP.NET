using System.Threading.Tasks;
using AccessManagerWeb.Core.Models;

namespace AccessManagerWeb.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendAccessRequestNotificationAsync(ResourceRequest request);
        Task SendAccessGrantedNotificationAsync(ResourceRequest request);
        Task SendAccessDeniedNotificationAsync(ResourceRequest request);
    }
}