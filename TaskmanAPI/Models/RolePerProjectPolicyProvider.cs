using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Data;
using TaskmanAPI.Contexts;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace TaskmanAPI.Models
{
    public class RolePerProjectPolicyProvider : IAuthorizationPolicyProvider
    {
        /*
        const string POLICY_PREFIX = "RoleInfo";
        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                var role = policyName.Substring(POLICY_PREFIX.Length);
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new RolePerProjectRequirement(role));
                return Task.FromResult<AuthorizationPolicy?>(policy.Build());
            }
            return Task.FromResult<AuthorizationPolicy?>(null);
        }*/

        private readonly DefaultContext _dbContext;
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; set; }

        public RolePerProjectPolicyProvider(IOptions<AuthorizationOptions> options, DefaultContext dbContext)
        {
            _dbContext = dbContext;
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
                            FallbackPolicyProvider.GetDefaultPolicyAsync();
        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
                                FallbackPolicyProvider.GetFallbackPolicyAsync();

        public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith("ProjectRole:"))
            {
                var projectId = int.Parse(policyName.Substring("ProjectRole:".Length));
                var roleName = await GetRoleForProjectAsync(projectId);

                if (!string.IsNullOrEmpty(roleName))
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireRole(roleName)
                        .Build();

                    return policy;
                }
            }

            return null;
        }

        private async Task<string> GetRoleForProjectAsync(int projectId)
        {
            var userId = _dbContext.Users.Find(ClaimTypes.NameIdentifier).Id;
            var role = await _dbContext.RolePerProjects
                .Where(rp => rp.UserId == userId && rp.ProjectId == projectId)
                .Select(rp => rp.RoleName)
                .FirstOrDefaultAsync();

            return role;
        }
    }
}