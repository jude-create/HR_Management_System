using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.Dtos.Recruitment;

public record CandidateDto(
    Guid Id,
    string Name,
    string? AvatarUrl,
    Guid JobId,
    string AppliedForTitle,
    DateOnly AppliedDate,
    string Email,
    string PhoneNumber,
    string Status
);

public record CandidateStatusRequest(
     [ Required, StringLength(100, MinimumLength = 2)]
    string Status);
