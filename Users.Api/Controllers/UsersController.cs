using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Services;
using Users.Contracts.Users;

namespace Users.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }
    
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetMe()
    {
        var userId = Guid.Parse(User.FindFirst("userid")!.Value);
        var result = await _usersService.GetUserByIdAsync(userId);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost("me/change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken token)
    {
        var userId = Guid.Parse(User.FindFirst("userid")!.Value);
        try
        {
            await _usersService.ChangePasswordAsync(userId, request, token);
        }
        catch (Exception ex)
        {
            return BadRequest( new { message = ex.Message });
        }

        return NoContent();
    }
    
    [Authorize("Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
    {
        try
        {
            var result = await _usersService.GetUsersAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
    
    [Authorize("Admin")]
    [HttpGet("{username}")]
    public async Task<ActionResult<UserResponse>> GetUser(string username)
    {
        var user = await _usersService.GetUserByUsernameAsync(username);
        
        if (user == null) return NotFound();
        
        return Ok(user);
    }
    
    [Authorize("Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var result = await _usersService.CreateUserAsync(request);
        if (result)
        {
            return Ok();
        }

        return BadRequest();

    }

    [Authorize("Admin")]
    [HttpPost("delete")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        var result = await _usersService.DeleteUserAsync(userId);
        if (result) return Ok();
        
        return NotFound();
    }
    
    [Authorize("Admin")]
    [HttpPost("create-admin")]
    public async Task<IActionResult> CreateAdmin(CreateUserRequest request)
    {
        var result = await _usersService.CreateAdminAsync(request: request);
        if  (result) return Ok();
        return BadRequest();
    }
    
}
