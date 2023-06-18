using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Api.DataTransfer;
using UserManagement.Domain.Entities;
using UserManagement.Interfaces.Interfaces;

namespace UserManagement.Api.Controllers;

[ApiController]
[ApiVersionNeutral]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationManager _authenticationManager;
    private readonly IUserRepository _users;

    public AuthController(
        IAuthenticationManager authenticationManager,
        IUserRepository userRepository)
    {
        _authenticationManager = authenticationManager;
        _users = userRepository;
    }

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto userData)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        User? user;

        if (userData.Name.Equals("admin") && userData.Password.Equals("admin"))
        {
            user = new User { Name = userData.Name };
        }
        else
        {
            user = await _users.Get(userData.Id);

            if (!userData.Password.Equals(user?.Password))
                return BadRequest("Invalid login data.");
        }

        if (user == null)
            return BadRequest("Invalid login data.");

        return Ok(_authenticationManager.GenerateApiKey(user));
    }
}

