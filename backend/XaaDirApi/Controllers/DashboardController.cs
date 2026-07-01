using Microsoft.AspNetCore.Mvc;
using XaaDirApi.Repositories;

namespace XaaDirApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ReportRepository _reportRepository;
    public DashboardController(ReportRepository reportRepository) => _reportRepository = reportRepository;

    [HttpGet("admin")] public IActionResult AdminDashboard() => Ok(_reportRepository.GetAdminSummary());
    [HttpGet("teacher/{teacherUserId:int}")] public IActionResult TeacherDashboard(int teacherUserId) => Ok(_reportRepository.GetTeacherSummary(teacherUserId));
}
