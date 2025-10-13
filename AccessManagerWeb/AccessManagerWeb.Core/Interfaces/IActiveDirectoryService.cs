using System.Threading.Tasks;
using System.Collections.Generic;
using AccessManagerWeb.Core.Models;

namespace AccessManagerWeb.Core.Interfaces
{
    public interface IActiveDirectoryService
    {
        Task<UserProfile> GetUserProfileAsync(string username);
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
        Task<IEnumerable<string>> GetUserGroupsAsync(string username);
        Task<bool> AddUserToGroupAsync(string username, string groupName);
        Task<bool> RemoveUserFromGroupAsync(string username, string groupName);
    }
}