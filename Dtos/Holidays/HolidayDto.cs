using System.ComponentModel.DataAnnotations;

namespace HR_Management_System.Dtos.Holidays;

public record HolidayDto(Guid Id, string Name, DateOnly Date, string DayOfWeek, string Type);

public record HolidayUpsertRequest(
    [ Required, StringLength(100, MinimumLength = 2)]
    string Name,

    DateOnly Date,

    [ Required]
    string Type
);