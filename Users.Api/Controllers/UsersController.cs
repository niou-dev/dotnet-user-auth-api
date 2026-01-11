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
        var userId = Guid.Parse(User.FindFirst("userid").Value);
        var result = await _usersService.GetUserById(userId);
        if (result == null) return NotFound();
        return Ok(result);
    }
    
    [Authorize("Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
    {
        var result = await _usersService.GetUsers();
        if (result == null) return StatusCode(StatusCodes.Status500InternalServerError);
        
        return Ok(result);
    }
    
    [Authorize("Admin")]
    [HttpGet("{username}")]
    public async Task<ActionResult<UserResponse>> GetUser(string username)
    {
        var user = await _usersService.GetUserByUsername(username);
        
        if (user == null) return NotFound();
        
        return Ok(user);
    }
    
    [Authorize("Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var result = await _usersService.CreateUser(request);
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
        var result = await _usersService.DeleteUser(userId);
        if (result) return Ok();
        
        return NotFound();
    }
    
    [Authorize("Admin")]
    [HttpPost("create-admin")]
    public async Task<IActionResult> CreateAdmin(CreateUserRequest request)
    {
        var result = await _usersService.CreateAdmin(request: request);
        if  (result) return Ok();
        return BadRequest();
    }
    
}
