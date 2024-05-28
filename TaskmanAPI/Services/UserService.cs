using Microsoft.EntityFrameworkCore;
using TaskmanAPI.Contexts;
using TaskmanAPI.Exceptions;

namespace TaskmanAPI.Services;

public class UserService(DefaultContext context)
{
    public async Task<string> GetUserIdByUsername(string username)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == username);

        if (user == null)
            throw new EntityNotFoundException("User does not exist");

        return user.Id;
    }
}