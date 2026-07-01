namespace XaaDirApi.Models;

public class ClassAttendanceRequest
{
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public int MarkedByUserId { get; set; }
    public List<ClassAttendanceStudentItem> Students { get; set; } = new();
}

public class ClassAttendanceStudentItem
{
    public int StudentId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
}

public class ClassAttendanceSummary
{
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public int TotalStudents { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
    public decimal PresentPercentage { get; set; }
    public decimal AbsentPercentage { get; set; }
    public decimal LatePercentage { get; set; }
}

public class StudentAttendancePercentage
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
    public decimal AttendancePercentage { get; set; }
    public decimal AbsencePercentage { get; set; }
    public decimal LatePercentage { get; set; }
}
