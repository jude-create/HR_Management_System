namespace HrManagement.Api.Dtos.Recruitment;

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

public record CandidateStatusRequest(string Status);
