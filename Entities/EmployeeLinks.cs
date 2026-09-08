namespace HR_Management_System.Entities;

public class EmployeeLinks
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public string? SlackId { get; set; }

    public string? SkypeId { get; set; }

    public string? GithubId { get; set; }
}