using Microsoft.Data.SqlClient;
using XaaDirApi.Data;
using XaaDirApi.Models;

namespace XaaDirApi.Repositories;

public class StudentRepository
{
    private readonly DbConnectionFactory _connectionFactory;
    public StudentRepository(DbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public List<Student> GetAll() => Query(@"SELECT st.StudentId, st.FullName, st.Gender, st.Phone, st.Email, st.ClassId, c.ClassName, st.Status, st.CreatedAt
        FROM Students st INNER JOIN Classes c ON st.ClassId=c.ClassId ORDER BY st.StudentId;");

    public Student? GetById(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"SELECT st.StudentId, st.FullName, st.Gender, st.Phone, st.Email, st.ClassId, c.ClassName, st.Status, st.CreatedAt
            FROM Students st INNER JOIN Classes c ON st.ClassId=c.ClassId WHERE st.StudentId=@StudentId;", connection);
        command.Parameters.AddWithValue("@StudentId", id);
        connection.Open();
        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public List<Student> GetByClass(int classId)
    {
        var items = new List<Student>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"SELECT st.StudentId, st.FullName, st.Gender, st.Phone, st.Email, st.ClassId, c.ClassName, st.Status, st.CreatedAt
            FROM Students st INNER JOIN Classes c ON st.ClassId=c.ClassId WHERE st.ClassId=@ClassId ORDER BY st.StudentId;", connection);
        command.Parameters.AddWithValue("@ClassId", classId);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read()) items.Add(Map(reader));
        return items;
    }

    public List<Student> Search(string keyword)
    {
        var items = new List<Student>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"SELECT st.StudentId, st.FullName, st.Gender, st.Phone, st.Email, st.ClassId, c.ClassName, st.Status, st.CreatedAt
            FROM Students st INNER JOIN Classes c ON st.ClassId=c.ClassId
            WHERE st.FullName LIKE @Keyword OR st.Phone LIKE @Keyword OR st.Email LIKE @Keyword OR c.ClassName LIKE @Keyword ORDER BY st.StudentId;", connection);
        command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read()) items.Add(Map(reader));
        return items;
    }

    public int Create(Student item)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"INSERT INTO Students (FullName, Gender, Phone, Email, ClassId, Status)
            OUTPUT INSERTED.StudentId VALUES (@FullName,@Gender,@Phone,@Email,@ClassId,@Status);", connection);
        AddParams(command, item);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public bool Update(int id, Student item)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"UPDATE Students SET FullName=@FullName, Gender=@Gender, Phone=@Phone, Email=@Email, ClassId=@ClassId, Status=@Status WHERE StudentId=@StudentId;", connection);
        command.Parameters.AddWithValue("@StudentId", id);
        AddParams(command, item);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("DELETE FROM Students WHERE StudentId=@StudentId;", connection);
        command.Parameters.AddWithValue("@StudentId", id);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    private List<Student> Query(string sql)
    {
        var items = new List<Student>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read()) items.Add(Map(reader));
        return items;
    }

    private static void AddParams(SqlCommand command, Student item)
    {
        command.Parameters.AddWithValue("@FullName", item.FullName.Trim());
        command.Parameters.AddWithValue("@Gender", item.Gender.Trim());
        command.Parameters.AddWithValue("@Phone", item.Phone.Trim());
        command.Parameters.AddWithValue("@Email", item.Email.Trim());
        command.Parameters.AddWithValue("@ClassId", item.ClassId);
        command.Parameters.AddWithValue("@Status", item.Status.Trim());
    }

    private static Student Map(SqlDataReader r) => new()
    {
        StudentId = Convert.ToInt32(r["StudentId"]),
        FullName = r["FullName"].ToString() ?? "",
        Gender = r["Gender"].ToString() ?? "",
        Phone = r["Phone"].ToString() ?? "",
        Email = r["Email"].ToString() ?? "",
        ClassId = Convert.ToInt32(r["ClassId"]),
        ClassName = r["ClassName"].ToString(),
        Status = r["Status"].ToString() ?? "",
        CreatedAt = Convert.ToDateTime(r["CreatedAt"])
    };
}
