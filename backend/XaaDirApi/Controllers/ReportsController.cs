using Microsoft.AspNetCore.Mvc;
using XaaDirApi.Repositories;

namespace XaaDirApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ReportRepository _reportRepository;
    private readonly AttendanceRepository _attendanceRepository;
    public ReportsController(ReportRepository reportRepository, AttendanceRepository attendanceRepository)
    {
        _reportRepository = reportRepository;
        _attendanceRepository = attendanceRepository;
    }

    [HttpGet("admin/summary")] public IActionResult AdminSummary() => Ok(_reportRepository.GetAdminSummary());
    [HttpGet("admin/attendance")] public IActionResult AdminAttendance() => Ok(_attendanceRepository.GetAll());
    [HttpGet("admin/by-class")] public IActionResult AttendanceByClass() => Ok(_reportRepository.GetAttendanceByClass());
    [HttpGet("admin/by-status")] public IActionResult AttendanceByStatus() => Ok(_reportRepository.GetAttendanceByStatus());
    [HttpGet("admin/by-teacher")] public IActionResult AttendanceByTeacher() => Ok(_reportRepository.GetAttendanceByTeacher());
    [HttpGet("teacher/{teacherUserId:int}/summary")] public IActionResult TeacherSummary(int teacherUserId) => Ok(_reportRepository.GetTeacherSummary(teacherUserId));
    [HttpGet("teacher/{teacherUserId:int}/attendance")] public IActionResult TeacherAttendance(int teacherUserId) => Ok(_attendanceRepository.GetByTeacher(teacherUserId));
}
