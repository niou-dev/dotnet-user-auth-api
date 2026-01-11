using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Users.Application.Models;


namespace Users.Application.Services;

public class JwtService : IJwtService
{
    private readonly JwtOptions _options;
    private readonly byte[] _key;

    public JwtService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        _key = Encoding.UTF8.GetBytes(_options.Key);
    }
    
    public string GenerateToken(User user, Dictionary<string, object>? customClaims = null)
    {

        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("userid", user.Id.ToString())
        };

        if (customClaims != null)
        {
            foreach (var claimPair in customClaims)
            {
                
                var value = claimPair.Value;
                var valueType = value switch
                {
                    bool => ClaimValueTypes.Boolean,
                    int => ClaimValueTypes.Integer,
                    double => ClaimValueTypes.Double,
                    _ => ClaimValueTypes.String
                };

                claims.Add(new Claim(
                    claimPair.Key,
                    value.ToString()!,
                    valueType
                ));
            }
        }
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(_options.LifetimeHours),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwt = tokenHandler.WriteToken(token);

        return jwt;
    }
}