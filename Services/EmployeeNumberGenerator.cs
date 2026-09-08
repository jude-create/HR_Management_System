using HR_Management_System.Data;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Services;

public interface IEmployeeNumberGenerator
{
    string Generate();
}

public sealed class EmployeeNumberGenerator : IEmployeeNumberGenerator
{
    private readonly AppDbContext _context;

    public EmployeeNumberGenerator(AppDbContext context)
    {
        _context = context;
    }

    public string Generate()
    {
        var lastNumber = _context.Employees
            .AsNoTracking()
            .Select(x => x.EmployeeNumber)
            .AsEnumerable()
            .Select(ParseNumber)
            .DefaultIfEmpty(0)
            .Max();

        var nextNumber = lastNumber + 1;

        return $"EMP-{nextNumber:D6}";
    }

    private static int ParseNumber(string employeeNumber)
    {
        if (string.IsNullOrWhiteSpace(employeeNumber))
            return 0;

        var parts = employeeNumber.Split('-');

        if (parts.Length != 2)
            return 0;

        return int.TryParse(parts[1], out var number)
            ? number
            : 0;
    }
}