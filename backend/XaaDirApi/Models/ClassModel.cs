namespace XaaDirApi.Models;

public class ClassModel
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? Section { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
