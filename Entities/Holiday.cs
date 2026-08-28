namespace HrManagement.Api.Entities;

public class Holiday
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public HolidayType Type { get; set; }
}
