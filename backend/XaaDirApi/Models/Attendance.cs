namespace XaaDirApi.Models;

public class Attendance
{
    public int AttendanceId { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public int ClassId { get; set; }
    public string? ClassName { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public DateTime AttendanceDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public int MarkedByUserId { get; set; }
    public string? MarkedByName { get; set; }
    public DateTime CreatedAt { get; set; }
}
