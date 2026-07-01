using Microsoft.Data.SqlClient;
using XaaDirApi.Data;
using XaaDirApi.Models;

namespace XaaDirApi.Repositories;

public class SubjectRepository
{
    private readonly DbConnectionFactory _connectionFactory;
    public SubjectRepository(DbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public List<Subject> GetAll()
    {
        var items = new List<Subject>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"SELECT s.SubjectId, s.SubjectName, s.TeacherUserId, u.FullName AS TeacherName, s.Description, s.CreatedAt
            FROM Subjects s INNER JOIN Users u ON s.TeacherUserId=u.UserId ORDER BY s.SubjectId;", connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read()) items.Add(Map(reader));
        return items;
    }

    public Subject? GetById(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"SELECT s.SubjectId, s.SubjectName, s.TeacherUserId, u.FullName AS TeacherName, s.Description, s.CreatedAt
            FROM Subjects s INNER JOIN Users u ON s.TeacherUserId=u.UserId WHERE s.SubjectId=@SubjectId;", connection);
        command.Parameters.AddWithValue("@SubjectId", id);
        connection.Open();
        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public List<Subject> GetByTeacher(int teacherUserId)
    {
        var items = new List<Subject>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"SELECT s.SubjectId, s.SubjectName, s.TeacherUserId, u.FullName AS TeacherName, s.Description, s.CreatedAt
            FROM Subjects s INNER JOIN Users u ON s.TeacherUserId=u.UserId WHERE s.TeacherUserId=@TeacherUserId ORDER BY s.SubjectName;", connection);
        command.Parameters.AddWithValue("@TeacherUserId", teacherUserId);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read()) items.Add(Map(reader));
        return items;
    }

    public int Create(Subject item)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("INSERT INTO Subjects (SubjectName, TeacherUserId, Description) OUTPUT INSERTED.SubjectId VALUES (@SubjectName,@TeacherUserId,@Description);", connection);
        AddParams(command, item);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public bool Update(int id, Subject item)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("UPDATE Subjects SET SubjectName=@SubjectName, TeacherUserId=@TeacherUserId, Description=@Description WHERE SubjectId=@SubjectId;", connection);
        command.Parameters.AddWithValue("@SubjectId", id);
        AddParams(command, item);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("DELETE FROM Subjects WHERE SubjectId=@SubjectId;", connection);
        command.Parameters.AddWithValue("@SubjectId", id);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool TeacherOwnsSubject(int teacherUserId, int subjectId)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("SELECT COUNT(*) FROM Subjects WHERE SubjectId=@SubjectId AND TeacherUserId=@TeacherUserId;", connection);
        command.Parameters.AddWithValue("@SubjectId", subjectId);
        command.Parameters.AddWithValue("@TeacherUserId", teacherUserId);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    private static void AddParams(SqlCommand command, Subject item)
    {
        command.Parameters.AddWithValue("@SubjectName", item.SubjectName.Trim());
        command.Parameters.AddWithValue("@TeacherUserId", item.TeacherUserId);
        command.Parameters.AddWithValue("@Description", (object?)item.Description?.Trim() ?? DBNull.Value);
    }

    private static Subject Map(SqlDataReader r) => new()
    {
        SubjectId = Convert.ToInt32(r["SubjectId"]),
        SubjectName = r["SubjectName"].ToString() ?? "",
        TeacherUserId = Convert.ToInt32(r["TeacherUserId"]),
        TeacherName = r["TeacherName"].ToString(),
        Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString(),
        CreatedAt = Convert.ToDateTime(r["CreatedAt"])
    };
}
