using HR_Management_System.Dtos.Holidays;

namespace HR_Management_System.Dtos.Common;

public enum HolidayOperationError
{
    None,
    NotFound,
    InvalidType
}

public sealed record HolidayResult(HolidayDto? Holiday, HolidayOperationError Error)
{
    public static HolidayResult Success(HolidayDto dto) => new(dto, HolidayOperationError.None);
    public static HolidayResult Fail(HolidayOperationError error) => new(null, error);
}

public sealed record DeleteHolidayResult(bool Success, HolidayOperationError Error)
{
    public static DeleteHolidayResult Ok() => new(true, HolidayOperationError.None);
    public static DeleteHolidayResult Fail(HolidayOperationError error) => new(false, error);
}