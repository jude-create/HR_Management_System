using AutoMapper;
using HrManagement.Api.Data;
using HrManagement.Api.Dtos.Holidays;
using HrManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.Api.Services;

// HolidayService stores public/company/optional holiday records.
public interface IHolidayService
{
    IReadOnlyList<HolidayDto> GetHolidays();
    HolidayDto? CreateHoliday(HolidayUpsertRequest request);
    HolidayDto? UpdateHoliday(Guid id, HolidayUpsertRequest request);
    bool DeleteHoliday(Guid id);
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
        // Sort by date so the calendar view is predictable.
    {
        var holidays = _context.Holidays.AsNoTracking().OrderBy(x => x.Date).ToList();
        return _mapper.Map<List<HolidayDto>>(holidays);
    }

    public HolidayDto? CreateHoliday(HolidayUpsertRequest request)
    {
        // Type is sent as text, so we parse it before creating the entity.
        if (!Enum.TryParse<HolidayType>(request.Type, true, out var type))
        {
            return null;
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
        return _mapper.Map<HolidayDto>(holiday);
    }

    public HolidayDto? UpdateHoliday(Guid id, HolidayUpsertRequest request)
    {
        // Update only if the holiday exists and the type is valid.
        var holiday = _context.Holidays.FirstOrDefault(x => x.Id == id);
        if (holiday is null || !Enum.TryParse<HolidayType>(request.Type, true, out var type))
        {
            return null;
        }

        holiday.Name = request.Name.Trim();
        holiday.Date = request.Date;
        holiday.Type = type;
        _context.SaveChanges();
        return _mapper.Map<HolidayDto>(holiday);
    }

    public bool DeleteHoliday(Guid id)
    {
        // Remove the holiday if found.
        var holiday = _context.Holidays.FirstOrDefault(x => x.Id == id);
        if (holiday is null)
        {
            return false;
        }

        _context.Holidays.Remove(holiday);
        return _context.SaveChanges() > 0;
    }
}
