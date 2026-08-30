using AutoMapper;
using HR_Management_System.Data;
using HR_Management_System.Dtos.Common;
using HR_Management_System.Dtos.Holidays;
using HR_Management_System.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Services;

// HolidayService stores public/company/optional holiday records.
public interface IHolidayService
{
    IReadOnlyList<HolidayDto> GetHolidays();
    HolidayResult CreateHoliday(HolidayUpsertRequest request);
    HolidayResult UpdateHoliday(Guid id, HolidayUpsertRequest request);
    DeleteHolidayResult DeleteHoliday(Guid id);
}

// The service validates holiday types and keeps holiday data in memory.
public sealed class HolidayService : IHolidayService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public HolidayService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IReadOnlyList<HolidayDto> GetHolidays()
    {
        var holidays = _context.Holidays.AsNoTracking().OrderBy(x => x.Date).ToList();
        return _mapper.Map<List<HolidayDto>>(holidays);
    }

    public HolidayResult CreateHoliday(HolidayUpsertRequest request)
    {
        if (!Enum.TryParse<HolidayType>(request.Type, true, out var type))
        {
            return HolidayResult.Fail(HolidayOperationError.InvalidType);
        }

        var holiday = new Holiday
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Date = request.Date,
            Type = type
        };

        _context.Holidays.Add(holiday);
        _context.SaveChanges();
        return HolidayResult.Success(_mapper.Map<HolidayDto>(holiday));
    }

    public HolidayResult UpdateHoliday(Guid id, HolidayUpsertRequest request)
    {
        var holiday = _context.Holidays.FirstOrDefault(x => x.Id == id);
        if (holiday is null)
        {
            return HolidayResult.Fail(HolidayOperationError.NotFound);
        }

        if (!Enum.TryParse<HolidayType>(request.Type, true, out var type))
        {
            return HolidayResult.Fail(HolidayOperationError.InvalidType);
        }

        holiday.Name = request.Name.Trim();
        holiday.Date = request.Date;
        holiday.Type = type;

        _context.SaveChanges();
        return HolidayResult.Success(_mapper.Map<HolidayDto>(holiday));
    }

    public DeleteHolidayResult DeleteHoliday(Guid id)
    {
        var holiday = _context.Holidays.FirstOrDefault(x => x.Id == id);
        if (holiday is null)
        {
            return DeleteHolidayResult.Fail(HolidayOperationError.NotFound);
        }

        _context.Holidays.Remove(holiday);
        _context.SaveChanges();
        return DeleteHolidayResult.Ok();
    }
}