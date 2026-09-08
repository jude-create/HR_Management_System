using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.Dtos.Employees;

public record EmployeeLinksRequest(
    string? SlackId,
    string? SkypeId,
    string? GithubId
);


