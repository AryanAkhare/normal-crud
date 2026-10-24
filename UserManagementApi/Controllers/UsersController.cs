using Microsoft.AspNetCore.Mvc;
using UserManagementApi.Models;
using UserManagementApi.Services;

namespace UserManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<User>>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<User>> GetAll()
    {
        return Ok(userService.GetAll());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<User>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<User> GetById(Guid id)
    {
        var user = userService.GetById(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [ProducesResponseType<User>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<User> Create(CreateUserRequest request)
    {
        var user = userService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<User>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<User> Update(Guid id, UpdateUserRequest request)
    {
        var user = userService.Update(id, request);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        return userService.Delete(id) ? NoContent() : NotFound();
    }
}
