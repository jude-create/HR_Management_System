namespace HR_Management_System.Dtos.Employees;

public record EmployeeLinksDto(
    string? SlackId,
    string? SkypeId,
    string? GithubId
);