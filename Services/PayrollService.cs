using AutoMapper;
using HrManagement.Api.Data;
using HrManagement.Api.Dtos.Common;
using HrManagement.Api.Dtos.Payroll;
using HrManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.Api.Services;

// PayrollService calculates and manages payroll data.
public interface IPayrollService
{
    IReadOnlyList<PayrollDto> GetPayrolls();
    IReadOnlyList<PayrollDto> GeneratePayrolls(PayrollGenerateRequest request);
    ApiMessageResponse ExportPayroll(PayrollExportRequest request);
    bool DeletePayroll(Guid id);
}

// This service creates payroll entries from active employees and the requested period.
public sealed class PayrollService : IPayrollService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public PayrollService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IReadOnlyList<PayrollDto> GetPayrolls()
    {
        var payrolls = _context.Payrolls
            .Include(x => x.Employee)
            .AsNoTracking()
            .OrderByDescending(x => x.Period)
            .ToList();
        return _mapper.Map<List<PayrollDto>>(payrolls);
    }

    public IReadOnlyList<PayrollDto> GeneratePayrolls(PayrollGenerateRequest request)
    {
        // Payroll is only generated for active employees.
        var activeEmployees = _context.Employees.Where(x => x.Status == EmployeeStatus.Active).ToList();
        var generated = new List<PayrollDto>();

        foreach (var employee in activeEmployees)
        {
            // Avoid duplicate payroll rows for the same employee and period.
            var existing = _context.Payrolls
                .Include(x => x.Employee)
                .FirstOrDefault(x => x.EmployeeId == employee.Id && x.Period == request.Period);
            if (existing is not null)
            {
                generated.Add(_mapper.Map<PayrollDto>(existing));
                continue;
            }

            // Simple demo salary calculation based on employee type.
            var baseCtc = HrServiceSupport.GetBaseCtc(employee);
            var payroll = new Payroll
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                Period = request.Period,
                Ctc = baseCtc,
                Salary = baseCtc * 0.75m,
                Deduction = baseCtc * 0.25m,
                Status = PayrollStatus.Processing,
                Employee = employee
            };

            _context.Payrolls.Add(payroll);
            generated.Add(_mapper.Map<PayrollDto>(payroll));
        }

        _context.SaveChanges();
        return generated;
    }

    public ApiMessageResponse ExportPayroll(PayrollExportRequest request)
        // The export is a placeholder message for now.
        => new($"Payroll export for {request.Period} is ready as {request.Format.ToLowerInvariant()}.");

    public bool DeletePayroll(Guid id)
    {
        // Remove payroll from the in-memory list if it exists.
        var payroll = _context.Payrolls.FirstOrDefault(x => x.Id == id);
        if (payroll is null)
        {
            return false;
        }

        _context.Payrolls.Remove(payroll);
        return _context.SaveChanges() > 0;
    }
}
