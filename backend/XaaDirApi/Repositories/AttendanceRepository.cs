using Microsoft.Data.SqlClient;
using XaaDirApi.Data;
using XaaDirApi.Models;

namespace XaaDirApi.Repositories;

public class AttendanceRepository
{
    private readonly DbConnectionFactory _connectionFactory;
    public AttendanceRepository(DbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    private const string ReportSql = @"SELECT a.AttendanceId, a.StudentId, st.FullName AS StudentName, a.ClassId, c.ClassName,
        a.SubjectId, sub.SubjectName, a.AttendanceDate, a.Status, a.Remarks, a.MarkedByUserId,
        u.FullName AS MarkedByName, a.CreatedAt
        FROM Attendance a
        INNER JOIN Students st ON a.StudentId=st.StudentId
        INNER JOIN Classes c ON a.ClassId=c.ClassId
        INNER JOIN Subjects sub ON a.SubjectId=sub.SubjectId
        INNER JOIN Users u ON a.MarkedByUserId=u.UserId";

    public List<Attendance> GetAll() => Query(ReportSql + " ORDER BY a.AttendanceDate DESC, a.AttendanceId DESC;");

    public Attendance? GetById(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(ReportSql + " WHERE a.AttendanceId=@AttendanceId;", connection);
        command.Parameters.AddWithValue("@AttendanceId", id);
        connection.Open();
        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public List<Attendance> GetByTeacher(int teacherUserId)
    {
        var items = new List<Attendance>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(ReportSql + " WHERE sub.TeacherUserId=@TeacherUserId ORDER BY a.AttendanceDate DESC, c.ClassName, st.StudentId;", connection);
        command.Parameters.AddWithValue("@TeacherUserId", teacherUserId);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read()) items.Add(Map(reader));
        return items;
    }

    public int Create(Attendance item)
    {
        ValidateTeacherPermission(item.MarkedByUserId, item.SubjectId);
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"INSERT INTO Attendance (StudentId, ClassId, SubjectId, AttendanceDate, Status, Remarks, MarkedByUserId)
            OUTPUT INSERTED.AttendanceId VALUES (@StudentId,@ClassId,@SubjectId,@AttendanceDate,@Status,@Remarks,@MarkedByUserId);", connection);
        AddParams(command, item);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public bool Update(int id, Attendance item)
    {
        ValidateTeacherPermission(item.MarkedByUserId, item.SubjectId);
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"UPDATE Attendance SET StudentId=@StudentId, ClassId=@ClassId, SubjectId=@SubjectId, AttendanceDate=@AttendanceDate,
            Status=@Status, Remarks=@Remarks, MarkedByUserId=@MarkedByUserId WHERE AttendanceId=@AttendanceId;", connection);
        command.Parameters.AddWithValue("@AttendanceId", id);
        AddParams(command, item);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("DELETE FROM Attendance WHERE AttendanceId=@AttendanceId;", connection);
        command.Parameters.AddWithValue("@AttendanceId", id);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    private void ValidateTeacherPermission(int markedByUserId, int subjectId)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var roleCommand = new SqlCommand("SELECT Role FROM Users WHERE UserId=@UserId AND IsActive=1;", connection);
        roleCommand.Parameters.AddWithValue("@UserId", markedByUserId);
        connection.Open();
        var role = roleCommand.ExecuteScalar()?.ToString();
        if (string.IsNullOrWhiteSpace(role)) throw new InvalidOperationException("Invalid user. The marker does not exist or is inactive.");
        if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase)) return;
        using var permissionCommand = new SqlCommand("SELECT COUNT(*) FROM Subjects WHERE SubjectId=@SubjectId AND TeacherUserId=@TeacherUserId;", connection);
        permissionCommand.Parameters.AddWithValue("@SubjectId", subjectId);
        permissionCommand.Parameters.AddWithValue("@TeacherUserId", markedByUserId);
        if (Convert.ToInt32(permissionCommand.ExecuteScalar()) == 0)
            throw new UnauthorizedAccessException("Access denied. Teacher can only mark attendance for assigned subjects.");
    }

    private List<Attendance> Query(string sql)
    {
        var items = new List<Attendance>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read()) items.Add(Map(reader));
        return items;
    }

    private static void AddParams(SqlCommand command, Attendance item)
    {
        command.Parameters.AddWithValue("@StudentId", item.StudentId);
        command.Parameters.AddWithValue("@ClassId", item.ClassId);
        command.Parameters.AddWithValue("@SubjectId", item.SubjectId);
        command.Parameters.AddWithValue("@AttendanceDate", item.AttendanceDate.Date);
        command.Parameters.AddWithValue("@Status", item.Status.Trim());
        command.Parameters.AddWithValue("@Remarks", (object?)item.Remarks?.Trim() ?? DBNull.Value);
        command.Parameters.AddWithValue("@MarkedByUserId", item.MarkedByUserId);
    }

    private static Attendance Map(SqlDataReader r) => new()
    {
        AttendanceId = Convert.ToInt32(r["AttendanceId"]),
        StudentId = Convert.ToInt32(r["StudentId"]),
        StudentName = r["StudentName"].ToString(),
        ClassId = Convert.ToInt32(r["ClassId"]),
        ClassName = r["ClassName"].ToString(),
        SubjectId = Convert.ToInt32(r["SubjectId"]),
        SubjectName = r["SubjectName"].ToString(),
        AttendanceDate = Convert.ToDateTime(r["AttendanceDate"]),
        Status = r["Status"].ToString() ?? "",
        Remarks = r["Remarks"] == DBNull.Value ? null : r["Remarks"].ToString(),
        MarkedByUserId = Convert.ToInt32(r["MarkedByUserId"]),
        MarkedByName = r["MarkedByName"].ToString(),
        CreatedAt = Convert.ToDateTime(r["CreatedAt"])
    };
}
