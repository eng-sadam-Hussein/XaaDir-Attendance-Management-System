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

    public List<Attendance> GetByClassSession(int classId, int subjectId, DateTime attendanceDate, int? teacherUserId)
    {
        var items = new List<Attendance>();
        using var connection = _connectionFactory.CreateConnection();

        var sql = ReportSql + @" WHERE a.ClassId=@ClassId AND a.SubjectId=@SubjectId AND a.AttendanceDate=@AttendanceDate";
        if (teacherUserId.HasValue) sql += " AND sub.TeacherUserId=@TeacherUserId";
        sql += " ORDER BY st.FullName;";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@ClassId", classId);
        command.Parameters.AddWithValue("@SubjectId", subjectId);
        command.Parameters.AddWithValue("@AttendanceDate", attendanceDate.Date);
        if (teacherUserId.HasValue) command.Parameters.AddWithValue("@TeacherUserId", teacherUserId.Value);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read()) items.Add(Map(reader));
        return items;
    }

    public int Create(Attendance item)
    {
        ValidateTeacherPermission(item.MarkedByUserId, item.SubjectId);
        ValidateStudentBelongsToClass(item.StudentId, item.ClassId);

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
        ValidateStudentBelongsToClass(item.StudentId, item.ClassId);

        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"UPDATE Attendance SET StudentId=@StudentId, ClassId=@ClassId, SubjectId=@SubjectId, AttendanceDate=@AttendanceDate,
            Status=@Status, Remarks=@Remarks, MarkedByUserId=@MarkedByUserId WHERE AttendanceId=@AttendanceId;", connection);
        command.Parameters.AddWithValue("@AttendanceId", id);
        AddParams(command, item);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public ClassAttendanceSummary SaveClassAttendance(ClassAttendanceRequest request)
    {
        ValidateTeacherPermission(request.MarkedByUserId, request.SubjectId);

        if (request.Students.Count == 0)
            throw new InvalidOperationException("No students were submitted for class attendance.");

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            foreach (var student in request.Students)
            {
                if (!IsValidStatus(student.Status))
                    throw new InvalidOperationException("Status must be Present, Absent, or Late.");

                ValidateStudentBelongsToClass(connection, transaction, student.StudentId, request.ClassId);

                using var upsert = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM Attendance WHERE StudentId=@StudentId AND SubjectId=@SubjectId AND AttendanceDate=@AttendanceDate)
BEGIN
    UPDATE Attendance
    SET ClassId=@ClassId, Status=@Status, Remarks=@Remarks, MarkedByUserId=@MarkedByUserId
    WHERE StudentId=@StudentId AND SubjectId=@SubjectId AND AttendanceDate=@AttendanceDate;
END
ELSE
BEGIN
    INSERT INTO Attendance (StudentId, ClassId, SubjectId, AttendanceDate, Status, Remarks, MarkedByUserId)
    VALUES (@StudentId, @ClassId, @SubjectId, @AttendanceDate, @Status, @Remarks, @MarkedByUserId);
END", connection, transaction);

                upsert.Parameters.AddWithValue("@StudentId", student.StudentId);
                upsert.Parameters.AddWithValue("@ClassId", request.ClassId);
                upsert.Parameters.AddWithValue("@SubjectId", request.SubjectId);
                upsert.Parameters.AddWithValue("@AttendanceDate", request.AttendanceDate.Date);
                upsert.Parameters.AddWithValue("@Status", student.Status.Trim());
                upsert.Parameters.AddWithValue("@Remarks", (object?)student.Remarks?.Trim() ?? DBNull.Value);
                upsert.Parameters.AddWithValue("@MarkedByUserId", request.MarkedByUserId);
                upsert.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }

        return GetClassAttendanceSummary(request.ClassId, request.SubjectId, request.AttendanceDate);
    }

    public ClassAttendanceSummary GetClassAttendanceSummary(int classId, int subjectId, DateTime attendanceDate)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"
SELECT
    COUNT(*) AS TotalStudents,
    SUM(CASE WHEN Status='Present' THEN 1 ELSE 0 END) AS PresentCount,
    SUM(CASE WHEN Status='Absent' THEN 1 ELSE 0 END) AS AbsentCount,
    SUM(CASE WHEN Status='Late' THEN 1 ELSE 0 END) AS LateCount
FROM Attendance
WHERE ClassId=@ClassId AND SubjectId=@SubjectId AND AttendanceDate=@AttendanceDate;", connection);
        command.Parameters.AddWithValue("@ClassId", classId);
        command.Parameters.AddWithValue("@SubjectId", subjectId);
        command.Parameters.AddWithValue("@AttendanceDate", attendanceDate.Date);
        connection.Open();

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return new ClassAttendanceSummary { ClassId = classId, SubjectId = subjectId, AttendanceDate = attendanceDate.Date };
        }

        var total = reader["TotalStudents"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalStudents"]);
        var present = reader["PresentCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PresentCount"]);
        var absent = reader["AbsentCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["AbsentCount"]);
        var late = reader["LateCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["LateCount"]);

        return new ClassAttendanceSummary
        {
            ClassId = classId,
            SubjectId = subjectId,
            AttendanceDate = attendanceDate.Date,
            TotalStudents = total,
            PresentCount = present,
            AbsentCount = absent,
            LateCount = late,
            PresentPercentage = Percent(present, total),
            AbsentPercentage = Percent(absent, total),
            LatePercentage = Percent(late, total)
        };
    }

    public StudentAttendancePercentage GetStudentAttendancePercentage(int studentId)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"
SELECT st.StudentId, st.FullName AS StudentName, st.ClassId, c.ClassName,
       COUNT(a.AttendanceId) AS TotalRecords,
       SUM(CASE WHEN a.Status='Present' THEN 1 ELSE 0 END) AS PresentCount,
       SUM(CASE WHEN a.Status='Absent' THEN 1 ELSE 0 END) AS AbsentCount,
       SUM(CASE WHEN a.Status='Late' THEN 1 ELSE 0 END) AS LateCount
FROM Students st
INNER JOIN Classes c ON st.ClassId=c.ClassId
LEFT JOIN Attendance a ON st.StudentId=a.StudentId
WHERE st.StudentId=@StudentId
GROUP BY st.StudentId, st.FullName, st.ClassId, c.ClassName;", connection);
        command.Parameters.AddWithValue("@StudentId", studentId);
        connection.Open();
        using var reader = command.ExecuteReader();
        if (!reader.Read()) throw new InvalidOperationException("Student not found.");

        return MapPercentage(reader);
    }

    public List<StudentAttendancePercentage> GetStudentPercentagesByClass(int classId)
    {
        var items = new List<StudentAttendancePercentage>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"
SELECT st.StudentId, st.FullName AS StudentName, st.ClassId, c.ClassName,
       COUNT(a.AttendanceId) AS TotalRecords,
       SUM(CASE WHEN a.Status='Present' THEN 1 ELSE 0 END) AS PresentCount,
       SUM(CASE WHEN a.Status='Absent' THEN 1 ELSE 0 END) AS AbsentCount,
       SUM(CASE WHEN a.Status='Late' THEN 1 ELSE 0 END) AS LateCount
FROM Students st
INNER JOIN Classes c ON st.ClassId=c.ClassId
LEFT JOIN Attendance a ON st.StudentId=a.StudentId
WHERE st.ClassId=@ClassId
GROUP BY st.StudentId, st.FullName, st.ClassId, c.ClassName
ORDER BY st.FullName;", connection);
        command.Parameters.AddWithValue("@ClassId", classId);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read()) items.Add(MapPercentage(reader));
        return items;
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

        if (string.IsNullOrWhiteSpace(role))
            throw new InvalidOperationException("Invalid user. The marker does not exist or is inactive.");

        if (RoleHelper.HasAdminAccess(role)) return;

        using var permissionCommand = new SqlCommand("SELECT COUNT(*) FROM Subjects WHERE SubjectId=@SubjectId AND TeacherUserId=@TeacherUserId;", connection);
        permissionCommand.Parameters.AddWithValue("@SubjectId", subjectId);
        permissionCommand.Parameters.AddWithValue("@TeacherUserId", markedByUserId);
        if (Convert.ToInt32(permissionCommand.ExecuteScalar()) == 0)
            throw new UnauthorizedAccessException("Access denied. Teacher can only mark attendance for assigned subjects.");
    }

    private void ValidateStudentBelongsToClass(int studentId, int classId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        ValidateStudentBelongsToClass(connection, null, studentId, classId);
    }

    private static void ValidateStudentBelongsToClass(SqlConnection connection, SqlTransaction? transaction, int studentId, int classId)
    {
        using var command = new SqlCommand("SELECT COUNT(*) FROM Students WHERE StudentId=@StudentId AND ClassId=@ClassId AND Status='Active';", connection, transaction);
        command.Parameters.AddWithValue("@StudentId", studentId);
        command.Parameters.AddWithValue("@ClassId", classId);

        if (Convert.ToInt32(command.ExecuteScalar()) == 0)
            throw new InvalidOperationException("Student does not belong to the selected class or is inactive.");
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

    private static bool IsValidStatus(string? status) => status == "Present" || status == "Absent" || status == "Late";

    private static decimal Percent(int value, int total) => total == 0 ? 0 : Math.Round((decimal)value / total * 100, 2);

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

    private static StudentAttendancePercentage MapPercentage(SqlDataReader r)
    {
        var total = r["TotalRecords"] == DBNull.Value ? 0 : Convert.ToInt32(r["TotalRecords"]);
        var present = r["PresentCount"] == DBNull.Value ? 0 : Convert.ToInt32(r["PresentCount"]);
        var absent = r["AbsentCount"] == DBNull.Value ? 0 : Convert.ToInt32(r["AbsentCount"]);
        var late = r["LateCount"] == DBNull.Value ? 0 : Convert.ToInt32(r["LateCount"]);

        return new StudentAttendancePercentage
        {
            StudentId = Convert.ToInt32(r["StudentId"]),
            StudentName = r["StudentName"].ToString() ?? "",
            ClassId = Convert.ToInt32(r["ClassId"]),
            ClassName = r["ClassName"].ToString() ?? "",
            TotalRecords = total,
            PresentCount = present,
            AbsentCount = absent,
            LateCount = late,
            AttendancePercentage = Percent(present + late, total),
            AbsencePercentage = Percent(absent, total),
            LatePercentage = Percent(late, total)
        };
    }
}
