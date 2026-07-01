using Microsoft.Data.SqlClient;
using XaaDirApi.Data;
using XaaDirApi.Models;

namespace XaaDirApi.Repositories;

public class ReportRepository
{
    private readonly DbConnectionFactory _connectionFactory;
    public ReportRepository(DbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public DashboardSummary GetAdminSummary()
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        return new DashboardSummary
        {
            TotalUsers = Count(connection, "SELECT COUNT(*) FROM Users"),
            TotalTeachers = Count(connection, "SELECT COUNT(*) FROM Users WHERE Role='Teacher'"),
            TotalClasses = Count(connection, "SELECT COUNT(*) FROM Classes"),
            TotalSubjects = Count(connection, "SELECT COUNT(*) FROM Subjects"),
            TotalStudents = Count(connection, "SELECT COUNT(*) FROM Students"),
            TotalAttendance = Count(connection, "SELECT COUNT(*) FROM Attendance"),
            PresentCount = Count(connection, "SELECT COUNT(*) FROM Attendance WHERE Status='Present'"),
            AbsentCount = Count(connection, "SELECT COUNT(*) FROM Attendance WHERE Status='Absent'"),
            LateCount = Count(connection, "SELECT COUNT(*) FROM Attendance WHERE Status='Late'")
        };
    }

    public TeacherDashboardSummary GetTeacherSummary(int teacherUserId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        return new TeacherDashboardSummary
        {
            TeacherUserId = teacherUserId,
            TeacherName = Text(connection, "SELECT FullName FROM Users WHERE UserId=@TeacherUserId AND Role='Teacher'", teacherUserId) ?? "Unknown Teacher",
            MySubjects = Count(connection, "SELECT COUNT(*) FROM Subjects WHERE TeacherUserId=@TeacherUserId", teacherUserId),
            MyAttendanceRecords = Count(connection, "SELECT COUNT(*) FROM Attendance a INNER JOIN Subjects s ON a.SubjectId=s.SubjectId WHERE s.TeacherUserId=@TeacherUserId", teacherUserId),
            PresentCount = Count(connection, "SELECT COUNT(*) FROM Attendance a INNER JOIN Subjects s ON a.SubjectId=s.SubjectId WHERE s.TeacherUserId=@TeacherUserId AND a.Status='Present'", teacherUserId),
            AbsentCount = Count(connection, "SELECT COUNT(*) FROM Attendance a INNER JOIN Subjects s ON a.SubjectId=s.SubjectId WHERE s.TeacherUserId=@TeacherUserId AND a.Status='Absent'", teacherUserId),
            LateCount = Count(connection, "SELECT COUNT(*) FROM Attendance a INNER JOIN Subjects s ON a.SubjectId=s.SubjectId WHERE s.TeacherUserId=@TeacherUserId AND a.Status='Late'", teacherUserId)
        };
    }

    public List<SummaryItem> GetAttendanceByClass() => Summary("SELECT c.ClassName AS Name, COUNT(a.AttendanceId) AS Total FROM Classes c LEFT JOIN Attendance a ON c.ClassId=a.ClassId GROUP BY c.ClassName ORDER BY c.ClassName;");
    public List<SummaryItem> GetAttendanceByStatus() => Summary("SELECT Status AS Name, COUNT(*) AS Total FROM Attendance GROUP BY Status ORDER BY Status;");
    public List<SummaryItem> GetAttendanceByTeacher() => Summary("SELECT u.FullName AS Name, COUNT(a.AttendanceId) AS Total FROM Users u LEFT JOIN Attendance a ON u.UserId=a.MarkedByUserId WHERE u.Role='Teacher' GROUP BY u.FullName ORDER BY u.FullName;");

    private List<SummaryItem> Summary(string sql)
    {
        var items = new List<SummaryItem>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read()) items.Add(new SummaryItem { Name = reader["Name"].ToString() ?? "", Total = Convert.ToInt32(reader["Total"]) });
        return items;
    }

    private static int Count(SqlConnection connection, string sql, int? teacherUserId = null)
    {
        using var command = new SqlCommand(sql, connection);
        if (teacherUserId.HasValue) command.Parameters.AddWithValue("@TeacherUserId", teacherUserId.Value);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static string? Text(SqlConnection connection, string sql, int teacherUserId)
    {
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@TeacherUserId", teacherUserId);
        return command.ExecuteScalar()?.ToString();
    }
}
