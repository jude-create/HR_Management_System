namespace HrManagement.Api.Dtos.Holidays;

public record HolidayDto(Guid Id, string Name, DateOnly Date, string DayOfWeek, string Type);

public record HolidayUpsertRequest(string Name, DateOnly Date, string Type);
