using Microsoft.AspNetCore.Mvc;
using XaaDirApi.Models;
using XaaDirApi.Repositories;

namespace XaaDirApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserRepository _repository;
    public UsersController(UserRepository repository) => _repository = repository;

    [HttpGet] public IActionResult GetAll() => Ok(_repository.GetAll());
    [HttpGet("{id:int}")] public IActionResult GetById(int id) => _repository.GetById(id) is { } user ? Ok(user) : NotFound("User not found.");
    [HttpGet("search")] public IActionResult Search([FromQuery] string keyword) => string.IsNullOrWhiteSpace(keyword) ? BadRequest("Search keyword is required.") : Ok(_repository.Search(keyword));

    [HttpPost]
    public IActionResult Create([FromBody] User user)
    {
        var validation = ValidateUser(user); if (validation != null) return BadRequest(validation);
        try { var id = _repository.Create(user); return CreatedAtAction(nameof(GetById), new { id }, _repository.GetById(id)); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] User user)
    {
        var validation = ValidateUser(user); if (validation != null) return BadRequest(validation);
        try { return _repository.Update(id, user) ? Ok(_repository.GetById(id)) : NotFound("User not found."); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try { return _repository.Delete(id) ? Ok("User deleted successfully.") : NotFound("User not found."); }
        catch (Exception ex) { return BadRequest("Cannot delete this user because it is connected to subjects or attendance. " + ex.Message); }
    }

    private static string? ValidateUser(User user)
    {
        if (string.IsNullOrWhiteSpace(user.FullName)) return "Full name is required.";
        if (string.IsNullOrWhiteSpace(user.Username)) return "Username is required.";
        if (string.IsNullOrWhiteSpace(user.Email)) return "Email is required.";
        if (string.IsNullOrWhiteSpace(user.Password)) return "Password is required.";
        if (user.Role != "Admin" && user.Role != "Teacher") return "Role must be Admin or Teacher.";
        return null;
    }
}
