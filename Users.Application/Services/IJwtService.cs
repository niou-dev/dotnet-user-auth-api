
using Users.Application.Models;

namespace Users.Application.Services;

public interface IJwtService
{
    string GenerateToken(User user, Dictionary<string, object>? customClaims = null);
}