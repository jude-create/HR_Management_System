using HR_Management_System.Dtos.Holidays;

namespace HR_Management_System.Services.Holiday
{

    // HolidayService stores public/company/optional holiday records.
    public interface IHolidayService
    {
        IReadOnlyList<HolidayDto> GetHolidays();
        HolidayResult CreateHoliday(HolidayUpsertRequest request);
        HolidayResult UpdateHoliday(Guid id, HolidayUpsertRequest request);
        DeleteHolidayResult DeleteHoliday(Guid id);
    }
}
