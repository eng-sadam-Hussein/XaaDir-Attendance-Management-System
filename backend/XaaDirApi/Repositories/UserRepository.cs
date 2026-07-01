using Microsoft.Data.SqlClient;
using XaaDirApi.Data;
using XaaDirApi.Models;

namespace XaaDirApi.Repositories;

public class UserRepository
{
    private readonly DbConnectionFactory _connectionFactory;
    public UserRepository(DbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public List<User> GetAll()
    {
        var users = new List<User>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("SELECT UserId, FullName, Username, Email, Password, Role, IsActive, CreatedAt FROM Users ORDER BY UserId;", connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read()) users.Add(MapUser(reader));
        return users;
    }

    public User? GetById(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("SELECT UserId, FullName, Username, Email, Password, Role, IsActive, CreatedAt FROM Users WHERE UserId=@UserId;", connection);
        command.Parameters.AddWithValue("@UserId", id);
        connection.Open();
        using var reader = command.ExecuteReader();
        return reader.Read() ? MapUser(reader) : null;
    }

    public List<User> Search(string keyword)
    {
        var users = new List<User>();
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"SELECT UserId, FullName, Username, Email, Password, Role, IsActive, CreatedAt FROM Users
            WHERE FullName LIKE @Keyword OR Username LIKE @Keyword OR Email LIKE @Keyword OR Role LIKE @Keyword ORDER BY UserId;", connection);
        command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read()) users.Add(MapUser(reader));
        return users;
    }

    public int Create(User user)
    {
        ValidateRoleRules(null, user.Role);

        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"INSERT INTO Users (FullName, Username, Email, Password, Role, IsActive)
            OUTPUT INSERTED.UserId VALUES (@FullName,@Username,@Email,@Password,@Role,@IsActive);", connection);
        AddParams(command, user);
        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public bool Update(int id, User user)
    {
        var existing = GetById(id);
        if (existing == null) return false;

        ValidateRoleRules(existing.Role, user.Role);

        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"UPDATE Users SET FullName=@FullName, Username=@Username, Email=@Email, Password=@Password, Role=@Role, IsActive=@IsActive WHERE UserId=@UserId;", connection);
        command.Parameters.AddWithValue("@UserId", id);
        AddParams(command, user);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand("DELETE FROM Users WHERE UserId=@UserId;", connection);
        command.Parameters.AddWithValue("@UserId", id);
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    private static void ValidateRoleRules(string? oldRole, string newRole)
    {
        if (!RoleHelper.IsValidRole(newRole))
            throw new InvalidOperationException("Role must be Admin, Teacher, or TeacherAdmin.");

        // Pure Admin accounts cannot be changed into Teacher/TeacherAdmin.
        if (oldRole == RoleHelper.Admin && newRole != RoleHelper.Admin)
            throw new InvalidOperationException("Admin account cannot take another role. Admin must remain Admin.");

        // Teacher can be promoted to TeacherAdmin, but not converted into pure Admin.
        if ((oldRole == RoleHelper.Teacher || oldRole == RoleHelper.TeacherAdmin) && newRole == RoleHelper.Admin)
            throw new InvalidOperationException("Teacher cannot be converted into pure Admin. Use TeacherAdmin to give admin access to a teacher.");
    }

    private static void AddParams(SqlCommand command, User user)
    {
        command.Parameters.AddWithValue("@FullName", user.FullName.Trim());
        command.Parameters.AddWithValue("@Username", user.Username.Trim());
        command.Parameters.AddWithValue("@Email", user.Email.Trim());
        command.Parameters.AddWithValue("@Password", user.Password.Trim());
        command.Parameters.AddWithValue("@Role", user.Role.Trim());
        command.Parameters.AddWithValue("@IsActive", user.IsActive);
    }

    private static User MapUser(SqlDataReader reader) => new()
    {
        UserId = Convert.ToInt32(reader["UserId"]),
        FullName = reader["FullName"].ToString() ?? "",
        Username = reader["Username"].ToString() ?? "",
        Email = reader["Email"].ToString() ?? "",
        Password = reader["Password"].ToString() ?? "",
        Role = reader["Role"].ToString() ?? "",
        IsActive = Convert.ToBoolean(reader["IsActive"]),
        CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
    };
}
