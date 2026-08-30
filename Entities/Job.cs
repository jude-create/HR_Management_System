namespace HR_Management_System.Entities;

public class Job
{
    public Guid Id { get; set; }
    public required string Title { get; set; } 
    public string Description { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public string Location { get; set; } = string.Empty;
    public decimal SalaryMin { get; set; }
    public decimal SalaryMax { get; set; }
    public JobStatus Status { get; set; }

    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
}
