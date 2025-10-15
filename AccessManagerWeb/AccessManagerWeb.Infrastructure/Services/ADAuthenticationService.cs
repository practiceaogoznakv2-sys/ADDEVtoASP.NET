using System.DirectoryServices.AccountManagement;
using Microsoft.Extensions.Configuration;
using AccessManagerWeb.Core.Interfaces;

namespace AccessManagerWeb.Infrastructure.Services
{
    public class ADAuthenticationService
    {
        private readonly string _domain;
        private readonly string _container;

        public ADAuthenticationService(IConfiguration configuration)
        {
            _domain = configuration["ActiveDirectory:Domain"];
            _container = configuration["ActiveDirectory:Container"];
        }

        public bool ValidateCredentials(string username, string password)
        {
            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, _domain, _container))
                {
                    return context.ValidateCredentials(username, password);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public UserPrincipal GetUserDetails(string username)
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, _domain, _container))
            {
                return UserPrincipal.FindByIdentity(context, username);
            }
        }
    }
}