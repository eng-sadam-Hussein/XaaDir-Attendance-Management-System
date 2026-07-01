using Microsoft.AspNetCore.Mvc;
using XaaDirApi.Models;
using XaaDirApi.Repositories;

namespace XaaDirApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly StudentRepository _repository;
    public StudentsController(StudentRepository repository) => _repository = repository;

    [HttpGet] public IActionResult GetAll() => Ok(_repository.GetAll());
    [HttpGet("{id:int}")] public IActionResult GetById(int id) => _repository.GetById(id) is { } item ? Ok(item) : NotFound("Student not found.");
    [HttpGet("class/{classId:int}")] public IActionResult GetByClass(int classId) => Ok(_repository.GetByClass(classId));
    [HttpGet("search")] public IActionResult Search([FromQuery] string keyword) => string.IsNullOrWhiteSpace(keyword) ? BadRequest("Search keyword is required.") : Ok(_repository.Search(keyword));

    [HttpPost]
    public IActionResult Create([FromBody] Student item)
    {
        var validation = Validate(item); if (validation != null) return BadRequest(validation);
        try { var id = _repository.Create(item); return CreatedAtAction(nameof(GetById), new { id }, _repository.GetById(id)); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Student item)
    {
        var validation = Validate(item); if (validation != null) return BadRequest(validation);
        try { return _repository.Update(id, item) ? Ok(_repository.GetById(id)) : NotFound("Student not found."); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try { return _repository.Delete(id) ? Ok("Student deleted successfully.") : NotFound("Student not found."); }
        catch (Exception ex) { return BadRequest("Cannot delete this student because attendance records are connected. " + ex.Message); }
    }

    private static string? Validate(Student item)
    {
        if (string.IsNullOrWhiteSpace(item.FullName)) return "Full name is required.";
        if (item.Gender != "Male" && item.Gender != "Female") return "Gender must be Male or Female.";
        if (string.IsNullOrWhiteSpace(item.Phone)) return "Phone is required.";
        if (string.IsNullOrWhiteSpace(item.Email)) return "Email is required.";
        if (item.ClassId <= 0) return "Class id is required.";
        if (item.Status != "Active" && item.Status != "Inactive") return "Status must be Active or Inactive.";
        return null;
    }
}
