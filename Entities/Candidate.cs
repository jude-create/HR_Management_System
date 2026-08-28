namespace HrManagement.Api.Entities;

public class Candidate
{
    public Guid Id { get; set; }
    public required string Name { get; set; } 
    public string? AvatarUrl { get; set; }
    public required string Email { get; set; } 
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly AppliedDate { get; set; }
    public CandidateStatus Status { get; set; }

    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;
}
