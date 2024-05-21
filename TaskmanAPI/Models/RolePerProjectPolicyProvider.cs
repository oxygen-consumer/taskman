using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskmanAPI.Contexts;

namespace TaskmanAPI.Models;

public class RolePerProjectPolicyProvider : IAuthorizationPolicyProvider
{
    private const string POLICY_PREFIX = "ProjectRole:";

    private readonly DefaultContext _dbContext;

    public RolePerProjectPolicyProvider(IOptions<AuthorizationOptions> options, DefaultContext dbContext)
    {
        _dbContext = dbContext;
        FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; set; }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return FallbackPolicyProvider.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return FallbackPolicyProvider.GetFallbackPolicyAsync();
    }

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        return new AuthorizationPolicyBuilder(policyName).Build();
        /* not what i need
         * call a handler??
         *
         * if (!policyName.StartsWith("ProjectRole:")) return null;

        //policyname contine rolulr
        //requirements este ca selectul din roleperprojects sa nu fie nul pt rolul si userul respectiv
        var userId = int.Parse(policyName.Substring("ProjectRole:".Length));
        var roleNames = await GetRoleForProjectAsync(userId);

        var requirements =

        if (!string.IsNullOrEmpty(roleName))
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireRole(roleName)
                .Build();

            return policy;
        }*/
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