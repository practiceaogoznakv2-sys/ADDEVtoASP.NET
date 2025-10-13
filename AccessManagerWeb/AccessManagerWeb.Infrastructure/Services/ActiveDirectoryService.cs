using System;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System.Collections.Generic;
using AccessManagerWeb.Core.Models;
using AccessManagerWeb.Core.Interfaces;

namespace AccessManagerWeb.Infrastructure.Services
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly string _domain;
        private readonly string _container;

        public ActiveDirectoryService(string domain, string container)
        {
            _domain = domain;
            _container = container;
        }

        public async Task<UserProfile> GetUserProfileAsync(string username)
        {
            using (var context = new PrincipalContext(ContextType.Domain, _domain, _container))
            {
                var user = await Task.Run(() => UserPrincipal.FindByIdentity(context, username));
                if (user == null)
                    return null;

                return new UserProfile
                {
                    Username = user.SamAccountName,
                    DisplayName = user.DisplayName,
                    Email = user.EmailAddress,
                    Department = user.Description
                };
            }
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            using (var context = new PrincipalContext(ContextType.Domain, _domain, _container))
            {
                return await Task.Run(() => context.ValidateCredentials(username, password));
            }
        }

        public async Task<IEnumerable<string>> GetUserGroupsAsync(string username)
        {
            var groups = new List<string>();
            using (var context = new PrincipalContext(ContextType.Domain, _domain, _container))
            {
                var user = await Task.Run(() => UserPrincipal.FindByIdentity(context, username));
                if (user != null)
                {
                    var userGroups = await Task.Run(() => user.GetGroups());
                    foreach (var group in userGroups)
                    {
                        groups.Add(group.Name);
                    }
                }
            }
            return groups;
        }

        public async Task<bool> AddUserToGroupAsync(string username, string groupName)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, _domain, _container))
                {
                    var user = await Task.Run(() => UserPrincipal.FindByIdentity(context, username));
                    var group = await Task.Run(() => GroupPrincipal.FindByIdentity(context, groupName));

                    if (user != null && group != null)
                    {
                        await Task.Run(() => group.Members.Add(user));
                        await Task.Run(() => group.Save());
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                // Log error
            }
            return false;
        }

        public async Task<bool> RemoveUserFromGroupAsync(string username, string groupName)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, _domain, _container))
                {
                    var user = await Task.Run(() => UserPrincipal.FindByIdentity(context, username));
                    var group = await Task.Run(() => GroupPrincipal.FindByIdentity(context, groupName));

                    if (user != null && group != null)
                    {
                        await Task.Run(() => group.Members.Remove(user));
                        await Task.Run(() => group.Save());
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                // Log error
            }
            return false;
        }
    }
}