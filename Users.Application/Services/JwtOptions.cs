namespace Users.Application.Services;

public class JwtOptions
{
    public string Key { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int LifetimeHours { get; set; } = 8;
}