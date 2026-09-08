namespace HR_Management_System.Entities;

public class EmployeeDocument
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public required string DocumentType { get; set; }

    public required string FileName { get; set; }

    public required string FileUrl { get; set; }

    public DateTime UploadedAt { get; set; }
}