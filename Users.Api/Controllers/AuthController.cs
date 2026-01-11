using Microsoft.AspNetCore.Mvc;
using Users.Application.Services;
using Users.Contracts.Auth;

namespace Users.Api.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("/api/auth/login")]
    public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        try
        {
            var result = await _authService.Login(loginRequest);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("/api/auth/signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest signUpRequest)
    {
        try
        {
            var result = await _authService.SignUp(signUpRequest);
            return Ok(result);
        }
        catch(Exception ex)
        {
            return BadRequest(ex);
        }
        
    }
    
}