namespace XaaDirApi.Models;

public class Subject
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int TeacherUserId { get; set; }
    public string? TeacherName { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
