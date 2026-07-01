using Microsoft.AspNetCore.Mvc;
using XaaDirApi.Models;
using XaaDirApi.Repositories;

namespace XaaDirApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly AttendanceRepository _repository;
    public AttendanceController(AttendanceRepository repository) => _repository = repository;

    [HttpGet] public IActionResult GetAll() => Ok(_repository.GetAll());
    [HttpGet("{id:int}")] public IActionResult GetById(int id) => _repository.GetById(id) is { } item ? Ok(item) : NotFound("Attendance record not found.");
    [HttpGet("teacher/{teacherUserId:int}")] public IActionResult GetByTeacher(int teacherUserId) => Ok(_repository.GetByTeacher(teacherUserId));

    [HttpPost]
    public IActionResult Create([FromBody] Attendance item)
    {
        var validation = Validate(item); if (validation != null) return BadRequest(validation);
        try { var id = _repository.Create(item); return CreatedAtAction(nameof(GetById), new { id }, _repository.GetById(id)); }
        catch (UnauthorizedAccessException ex) { return StatusCode(StatusCodes.Status403Forbidden, ex.Message); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpPost("mark")] public IActionResult MarkAttendance([FromBody] Attendance item) => Create(item);

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Attendance item)
    {
        var validation = Validate(item); if (validation != null) return BadRequest(validation);
        try { return _repository.Update(id, item) ? Ok(_repository.GetById(id)) : NotFound("Attendance record not found."); }
        catch (UnauthorizedAccessException ex) { return StatusCode(StatusCodes.Status403Forbidden, ex.Message); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try { return _repository.Delete(id) ? Ok("Attendance deleted successfully.") : NotFound("Attendance record not found."); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    private static string? Validate(Attendance item)
    {
        if (item.StudentId <= 0) return "Student id is required.";
        if (item.ClassId <= 0) return "Class id is required.";
        if (item.SubjectId <= 0) return "Subject id is required.";
        if (item.MarkedByUserId <= 0) return "Marked by user id is required.";
        if (item.Status != "Present" && item.Status != "Absent" && item.Status != "Late") return "Status must be Present, Absent, or Late.";
        return null;
    }
}
