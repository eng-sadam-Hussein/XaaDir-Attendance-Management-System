using Microsoft.AspNetCore.Mvc;
using XaaDirApi.Models;
using XaaDirApi.Repositories;

namespace XaaDirApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassesController : ControllerBase
{
    private readonly ClassRepository _repository;
    public ClassesController(ClassRepository repository) => _repository = repository;

    [HttpGet] public IActionResult GetAll() => Ok(_repository.GetAll());
    [HttpGet("{id:int}")] public IActionResult GetById(int id) => _repository.GetById(id) is { } item ? Ok(item) : NotFound("Class not found.");

    [HttpPost]
    public IActionResult Create([FromBody] ClassModel item)
    {
        if (string.IsNullOrWhiteSpace(item.ClassName)) return BadRequest("Class name is required.");
        try { var id = _repository.Create(item); return CreatedAtAction(nameof(GetById), new { id }, _repository.GetById(id)); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] ClassModel item)
    {
        if (string.IsNullOrWhiteSpace(item.ClassName)) return BadRequest("Class name is required.");
        try { return _repository.Update(id, item) ? Ok(_repository.GetById(id)) : NotFound("Class not found."); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try { return _repository.Delete(id) ? Ok("Class deleted successfully.") : NotFound("Class not found."); }
        catch (Exception ex) { return BadRequest("Cannot delete this class because students or attendance are connected. " + ex.Message); }
    }
}
