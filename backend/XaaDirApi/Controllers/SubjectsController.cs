using Microsoft.AspNetCore.Mvc;
using XaaDirApi.Models;
using XaaDirApi.Repositories;

namespace XaaDirApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly SubjectRepository _repository;
    public SubjectsController(SubjectRepository repository) => _repository = repository;

    [HttpGet] public IActionResult GetAll() => Ok(_repository.GetAll());
    [HttpGet("{id:int}")] public IActionResult GetById(int id) => _repository.GetById(id) is { } item ? Ok(item) : NotFound("Subject not found.");
    [HttpGet("teacher/{teacherUserId:int}")] public IActionResult GetByTeacher(int teacherUserId) => Ok(_repository.GetByTeacher(teacherUserId));
    [HttpGet("teacher-owns-subject")] public IActionResult TeacherOwnsSubject([FromQuery] int teacherUserId, [FromQuery] int subjectId) => Ok(new { teacherUserId, subjectId, ownsSubject = _repository.TeacherOwnsSubject(teacherUserId, subjectId) });

    [HttpPost]
    public IActionResult Create([FromBody] Subject item)
    {
        var validation = Validate(item); if (validation != null) return BadRequest(validation);
        try { var id = _repository.Create(item); return CreatedAtAction(nameof(GetById), new { id }, _repository.GetById(id)); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Subject item)
    {
        var validation = Validate(item); if (validation != null) return BadRequest(validation);
        try { return _repository.Update(id, item) ? Ok(_repository.GetById(id)) : NotFound("Subject not found."); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try { return _repository.Delete(id) ? Ok("Subject deleted successfully.") : NotFound("Subject not found."); }
        catch (Exception ex) { return BadRequest("Cannot delete this subject because attendance records are connected. " + ex.Message); }
    }

    private static string? Validate(Subject item)
    {
        if (string.IsNullOrWhiteSpace(item.SubjectName)) return "Subject name is required.";
        if (item.TeacherUserId <= 0) return "Teacher user id is required.";
        return null;
    }
}
