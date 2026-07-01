namespace XaaDirApi.Models;

public class DashboardSummary
{
    public int TotalUsers { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalClasses { get; set; }
    public int TotalSubjects { get; set; }
    public int TotalStudents { get; set; }
    public int TotalAttendance { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
}

public class TeacherDashboardSummary
{
    public int TeacherUserId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int MySubjects { get; set; }
    public int MyAttendanceRecords { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
}

public class SummaryItem
{
    public string Name { get; set; } = string.Empty;
    public int Total { get; set; }
}
