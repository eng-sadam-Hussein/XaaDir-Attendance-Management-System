using Microsoft.Data.SqlClient;
using XaaDirApi.Data;
using XaaDirApi.Models;

namespace XaaDirApi.Repositories;

public class AuthRepository
{
    private readonly DbConnectionFactory _connectionFactory;
    public AuthRepository(DbConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public LoginResponse? Login(LoginRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var command = new SqlCommand(@"
            SELECT UserId, FullName, Username, Email, Role
            FROM Users
            WHERE Username = @Username AND Password = @Password AND IsActive = 1;", connection);
        command.Parameters.AddWithValue("@Username", request.Username.Trim());
        command.Parameters.AddWithValue("@Password", request.Password.Trim());
        connection.Open();
        using var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        var role = reader["Role"].ToString() ?? "";
        return new LoginResponse
        {
            UserId = Convert.ToInt32(reader["UserId"]),
            FullName = reader["FullName"].ToString() ?? "",
            Username = reader["Username"].ToString() ?? "",
            Email = reader["Email"].ToString() ?? "",
            Role = role,
            HasAdminAccess = RoleHelper.HasAdminAccess(role),
            HasTeacherAccess = RoleHelper.HasTeacherAccess(role)
        };
    }
}
