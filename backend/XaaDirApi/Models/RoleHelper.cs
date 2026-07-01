namespace XaaDirApi.Models;

public static class RoleHelper
{
    public const string Admin = "Admin";
    public const string Teacher = "Teacher";
    public const string TeacherAdmin = "TeacherAdmin";

    public static bool IsValidRole(string? role) =>
        role == Admin || role == Teacher || role == TeacherAdmin;

    public static bool HasAdminAccess(string? role) =>
        role == Admin || role == TeacherAdmin;

    public static bool HasTeacherAccess(string? role) =>
        role == Teacher || role == TeacherAdmin;
}
