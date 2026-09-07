using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.Dtos.Calendar;

public record CalendarEventDto(
    Guid Id,
    string Title,
    string? Description,
    DateOnly Date,
    TimeOnly? Time,
    string Type,
    string? Color
);

public record CalendarEventCreateRequest(
    [Required]
    [StringLength(150, MinimumLength = 2)]
    string Title,

    [StringLength(500)]
    string? Description,

    [Required]
    DateOnly Date,

    TimeOnly? Time,

    [Required]
    string Type,

    string? Color
);

public record CalendarEventUpdateRequest(
    [Required]
    [StringLength(150, MinimumLength = 2)]
    string Title,

    [StringLength(500)]
    string? Description,

    [Required]
    DateOnly Date,

    TimeOnly? Time,

    [Required]
    string Type,

    string? Color
);