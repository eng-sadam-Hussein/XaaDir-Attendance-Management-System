using Microsoft.Data.SqlClient;
using XaaDirApi.Data;
using XaaDirApi.Models;

namespace XaaDirApi.Repositories;

public class ClassRepository
{
    private readonly DbConnectionFactory _connectionFactory;
    public ClassRepository(DbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public List<ClassModel> GetAll()
    {
        var classes = new List<ClassModel>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("SELECT ClassId, ClassName, Section, Description, CreatedAt FROM Classes ORDER BY ClassId;", connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read()) classes.Add(Map(reader));
        return classes;
    }

    public ClassModel? GetById(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("SELECT ClassId, ClassName, Section, Description, CreatedAt FROM Classes WHERE ClassId=@ClassId;", connection);
        command.Parameters.AddWithValue("@ClassId", id);
        connection.Open();
        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public int Create(ClassModel item)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("INSERT INTO Classes (ClassName, Section, Description) OUTPUT INSERTED.ClassId VALUES (@ClassName,@Section,@Description);", connection);
        AddParams(command, item);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public bool Update(int id, ClassModel item)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("UPDATE Classes SET ClassName=@ClassName, Section=@Section, Description=@Description WHERE ClassId=@ClassId;", connection);
        command.Parameters.AddWithValue("@ClassId", id);
        AddParams(command, item);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("DELETE FROM Classes WHERE ClassId=@ClassId;", connection);
        command.Parameters.AddWithValue("@ClassId", id);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    private static void AddParams(SqlCommand command, ClassModel item)
    {
        command.Parameters.AddWithValue("@ClassName", item.ClassName.Trim());
        command.Parameters.AddWithValue("@Section", (object?)item.Section?.Trim() ?? DBNull.Value);
        command.Parameters.AddWithValue("@Description", (object?)item.Description?.Trim() ?? DBNull.Value);
    }

    private static ClassModel Map(SqlDataReader r) => new()
    {
        ClassId = Convert.ToInt32(r["ClassId"]),
        ClassName = r["ClassName"].ToString() ?? "",
        Section = r["Section"] == DBNull.Value ? null : r["Section"].ToString(),
        Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString(),
        CreatedAt = Convert.ToDateTime(r["CreatedAt"])
    };
}
