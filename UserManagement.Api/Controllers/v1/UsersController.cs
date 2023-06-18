using Microsoft.AspNetCore.Mvc;
using UserManagement.Api.DataTransfer;
using UserManagement.Interfaces.Interfaces;
using UserManagement.Api.Validation.Attributes;
using UserManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using UserManagement.Domain.Entities;
using System.Security.Claims;

namespace UserManagement.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _users;

    public UsersController(IUserRepository userDataAccessor)
    {
        _users = userDataAccessor;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterUser(
        [FromBody] UserPutDto registrationData)
    {
        if(!ModelState.IsValid)
            return NotFound(ModelState);

        IActionResult result;
        try
        {
            User? added = await _users.Add(registrationData.CreateUser());
            result = added == null ? BadRequest() : Ok(added.ToInfoDto());
        }
        catch (Exception e)
        {
            result = CreateErrorResult(e);
        }

        return result;
    }

    [HttpGet]
    [ProducesResponseType(typeof(UserListDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers()
    {
        var list = await _users.Get();

        var userDtos = list
            .Select(u => u.ToSimpleDto())
            .ToList();

        UserListDto data = new UserListDto
        {
            Count = userDtos.Count,
            Users = userDtos
        };

        return Ok(data);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser([GuidId] Guid id)
    {
        UserInfoDto? result = (await _users.Get(id))?.ToInfoDto();

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [Authorize]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateUser(
        [FromBody] UserPutDto userData)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        if (!AccessAllowed(userData.Id))
            return Forbid();

        IActionResult result;
        try
        {
            bool success = await _users.Update(userData.CreateUser());
            result = success ? Ok() : NotFound();
        }
        catch (Exception e)
        {
            result = CreateErrorResult(e);
        }

        return result;
    }

    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser([GuidId] Guid id)
    {
        if (!AccessAllowed(id))
            return Forbid();

        bool success = await _users.Delete(id);
        return success ? Ok() : NotFound();
    }

    private IActionResult CreateErrorResult(Exception error)
    {
        if (error is DuplicateModelDataException e)
        {
            // will produce 'application/json' content type
            // not 'application/problem+json'
            return Conflict(new
            {
                Error = new
                {
                    e.Message,
                    Property = e.duplicatePropertyName
                }
            });
        }

        return BadRequest();
    }

    private bool AccessAllowed(Guid toId)
    {
        IEnumerable<Claim> claims = HttpContext.User.Claims;

        string? id = null;
        string? name = null;

        foreach (Claim claim in claims)
        {
            if (claim.Type.Equals("id"))
                id = claim.Value;
            else if (claim.Type.Equals("name"))
                name = claim.Value;
        }

        User user = new User();

        try { user.Id = new Guid(id); }
        catch (Exception e) { }

        if (name != null)
            user.Name = name;

        return toId == user.Id || IsAdmin(user);
    }

    private bool IsAdmin(User user)
    {
        return user.Name.Equals("admin");
    }
}

